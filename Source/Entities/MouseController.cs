using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Entities;

[CustomEntity("UMH/MouseController")]
[Tracked(false)]
public class MouseController : Entity
{
    public MTexture birb;
    public PlacementController placement;
    public Vector2? virtualCursorPos = null;
    public Vector2? prevCursorPos = null;

    public MouseController()
        : base(new Vector2(1920f / 2f, 1080f / 2f))
    {
        birb = GFX.Game["birb"];
        Tag = Tags.HUD;

        placement = new PlacementController(Position);
    }

    public override void Added(Scene scene)
    {
        scene.Add(placement);
        
        base.Added(scene);
    }

    public override void Render()
    {
        birb.DrawCentered(Position, Color.White, 1f);
        
        base.Render();
    }

    bool placed = false;
    public override void Update()
    {
        placement.Active = true;
        placement.Visible = true;

        var plr = Engine.Scene.Tracker.GetEntity<Player>();
        foreach (Ghost nplr in Engine.Scene.Tracker.GetEntities<Ghost>())
            nplr.Visible = false;
        Camera currentCam = null;
        if (plr != null)
        {
            plr.StateMachine.State = 11;
            plr.StateMachine.Locked = true;
            plr.level.CanRetry = false;
            plr.DummyGravity = false;
            plr.Position = placement.Position;
            plr.Visible = false;
            //plr.level.InCutscene = true;
            currentCam = plr.level.Camera;
            Engine.Commands.Log($"freeze");
        }
        MouseState state = Mouse.GetState();

        Engine.Commands.Log($"M X: {Position.X}, Y: {Position.Y}, wW: {Engine.ViewWidth}, wH: {Engine.ViewHeight}");
        if (currentCam != null)
        {
            placement.Position.X = (float)Position.X / 1920f * currentCam.Viewport.Width / currentCam.zoom.X + currentCam.X;
            placement.Position.Y = (float)Position.Y / 1080f * currentCam.Viewport.Height / currentCam.zoom.Y + currentCam.Y;
            Engine.Commands.Log($"P X: {placement.Position.X}, Y: {placement.Position.Y}, wW: {currentCam.Viewport.Width}, wH: {currentCam.Viewport.Height}");
        }

        if (prevCursorPos == null)
            prevCursorPos = MInput.Mouse.Position;
        if (virtualCursorPos == null)
            virtualCursorPos = new Vector2(1920f/2f, 1080f/2f);

        if (prevCursorPos != MInput.Mouse.Position)
        {
            prevCursorPos = MInput.Mouse.Position;
            Position = (Vector2)prevCursorPos;
        }
        else if (Input.Feather.value != Vector2.Zero)
        {
            virtualCursorPos += Input.Feather.value * 8;
            Position = (Vector2)virtualCursorPos;
        }

        if (placement.holding == null)
        {
            if (!placed)
            {
                DreamBlock block = Engine.Scene.Tracker.GetEntity<DreamBlock>();
                if (block != null)
                {
                    placement.holding = block;
                }
                else
                {
                    Engine.Commands.Log($"doesn't exist");
                }
                placed = true;
            }
        }
        else
        {
            if (Input.Jump.Pressed/* || state.LeftButton == ButtonState.Pressed*/)
            {
                if (placement.Place())
                {
                    this.Active = false;
                    this.Visible = false;
                    placement.Active = false;
                    placement.Visible = false;
                    if (plr != null)
                    {
                        plr.level.InCutscene = false;
                        plr.StateMachine.Locked = false;
                        plr.StateMachine.State = 0;
                        plr.level.CanRetry = true;
                        plr.Visible = true;
                        plr.DummyGravity = true;
                        foreach (Ghost nplr in Engine.Scene.Tracker.GetEntities<Ghost>())
                            nplr.Visible = true;
                        Engine.Commands.Log($"freed");
                    }
                }
            }
        }

        base.Update();
    }
}