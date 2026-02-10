using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.Entities;
using Celeste.Mod.UMH.ObjectTypes;
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
    public Level level;
    public Vector2 virtualCursorPos = new Vector2(1920 / 2, 1080 / 2);
    public Vector2? prevCursorPos = null;
    public UMHManager manager;

    public MouseController()
        : base(new Vector2(1920f / 2f, 1080f / 2f))
    {
        birb = GFX.Game["birb"];
        Tag = Tags.HUD;

        placement = new PlacementController(Position, this);
        placement.Active = true;
    }

    public async override void Added(Scene scene)
    {
        base.Added(scene);

        level = SceneAs<Level>();

        scene.Add(placement);
        placement.Scene = scene; //idk fixes error
        placement.HoldingIndex = 0;
        await Task.Delay(5000);
        if (placement != null)
            placement.HoldingIndex = 1;
    }

    public override void Render()
    {
        birb.DrawCentered(Position, Color.White, 1f);
        foreach (Ghost remotePlayer in Engine.Scene.Tracker.GetEntities<Ghost>())
        {
            birb.DrawCentered(ToScreenspace(remotePlayer.Position), Color.Red, 1f);
            Console.WriteLine(ToScreenspace(remotePlayer.Position));
        }

        base.Render();
    }

    public override void Update()
    {
        MouseState state = Mouse.GetState();
        Player plr = Engine.Scene.Tracker.GetEntity<Player>();

        if (placement != null)
            placement.Position = ToWorldspace(Position);

        if (plr != null)
        {
            plr.Position = ToWorldspace(Position);
            plr.StateMachine.State = 23;
            plr.StateMachine.Locked = true;
            plr.level.CanRetry = false;
            plr.DummyGravity = false;
            plr.Speed = Vector2.Zero;
            plr.Position = ToWorldspace(Position);
            plr.Visible = false;
        }

        if (prevCursorPos == null)
            prevCursorPos = MInput.Mouse.Position;

        if (prevCursorPos != MInput.Mouse.Position)
        {
            prevCursorPos = MInput.Mouse.Position;
            Position = (Vector2)prevCursorPos;
        }
        else if (Input.Feather.value != Vector2.Zero)
        {
            virtualCursorPos += Input.Feather.value * 8;
            Position = virtualCursorPos;
        }

        if (Input.Jump.Pressed || MInput.Mouse.CheckLeftButton)
        {
            if (placement != null && placement.holding != null)
            {
                if (placement.Place())
                {
                    SwitchToGameplay(plr);
                    placement.RemoveSelf();
                    placement = null;
                }
            }
        }

        foreach (Ghost nplr in Engine.Scene.Tracker.GetEntities<Ghost>())
            nplr.Visible = false;

        base.Update();
    }

    private void SwitchToGameplay(Player plr)
    {
        plr.StateMachine.Locked = false;
        plr.StateMachine.State = 0;
        plr.level.CanRetry = true;
        plr.DummyGravity = true;
        plr.Speed = Vector2.Zero;
        plr.Position = Engine.Scene.Tracker.GetEntity<UMHSpawn>().Position;
        plr.Visible = true;

        foreach (Ghost nplr in Engine.Scene.Tracker.GetEntities<Ghost>())
            nplr.Visible = true;

        this.Visible = false;
        this.Active = false;
    }

    private Vector2 ToWorldspace(Vector2 screenPos)
    {
        return new Vector2(
            (float)screenPos.X / 1920f * level.Camera.Viewport.Width / level.Camera.zoom.X + level.Camera.X,
            (float)screenPos.Y / 1080f * level.Camera.Viewport.Height / level.Camera.zoom.Y + level.Camera.Y
        );
    }

    private Vector2 ToScreenspace(Vector2 worldPos)
    {
        return new Vector2(
            (worldPos.X - level.Camera.X) * level.Camera.zoom.X / level.Camera.Viewport.Width * 1920f,
            (worldPos.Y - level.Camera.Y) * level.Camera.zoom.Y / level.Camera.Viewport.Height * 1080f
        );
    }
}