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
    public bool _localVisible = true;
    public bool localVisible
    {
        get => _localVisible && Visible;
        set => _localVisible = value;
    }
    public List<uint> picked = new();

    public delegate void mouseClick(MouseController mouse);
    public static event mouseClick MouseClick;

    public MouseController(UMHManager manager)
        : base(new Vector2(1920f / 2f, 1080f / 2f))
    {
        this.manager = manager;
        birb = GFX.Game["birb"];
        Tag = Tags.HUD;

        MouseClick += Click;
        //placement = new PlacementController(this);
        //placement.Active = true;
    }

    public async override void Added(Scene scene)
    {
        base.Added(scene);

        level = SceneAs<Level>();

        //scene.Add(placement);
        //placement.Scene = scene; //idk fixes error
        //placement.HoldingIndex = 0;
        await Task.Delay(5000);
        if (placement != null)
            placement.HoldingIndex = 1;
    }

    public override void Render()
    {
        base.Render();

        if (localVisible)
            birb.DrawCentered(Position, Color.White, 1f);

        foreach (Ghost remotePlayer in Engine.Scene.Tracker.GetEntities<Ghost>())
            if (manager.matchState != UMHManager.MatchStates.PickItems || !picked.Contains(remotePlayer.PlayerInfo.ID))
            {
                birb.DrawCentered(ToScreenspace(remotePlayer.Position), Color.Red, 1f);
                remotePlayer.NameTag.Visible = true;
            } else remotePlayer.NameTag.Visible = false;
    }

    bool wasClicking = false;
    public override void Update()
    {
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

        if (localVisible)
        {
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
        }

        if ((Input.Jump.Check || MInput.Mouse.CheckLeftButton) && !wasClicking)
        {
            wasClicking = true;
            MouseClick.Invoke(this);
            cancelPlace = false;
        }
        else if (!Input.Jump.Pressed && !MInput.Mouse.CheckLeftButton) wasClicking = false;

            foreach (Ghost nplr in Engine.Scene.Tracker.GetEntities<Ghost>())
                nplr.Visible = false;

        base.Update();
    }

    public bool cancelPlace = false;
    public static void Click(MouseController mouse)
    {
        if (mouse.cancelPlace)
        {
            mouse.cancelPlace = false;
            return;
        }
        if (mouse.Visible && mouse.localVisible && mouse.placement != null && mouse.placement.holding != null)
        {
            if (mouse.placement.Place())
            {
                Player plr = Engine.Scene.Tracker.GetEntity<Player>();
                if (plr != null)
                    mouse.SwitchToGameplay(plr);
                mouse.placement.RemoveSelf();
                mouse.placement = null;
            }
        }
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        Player plr = Engine.Scene.Tracker.GetEntity<Player>();
        if (plr != null)
            SwitchToGameplay(plr);
        MouseClick -= Click;
    }

    public override void SceneEnd(Scene scene)
    {
        base.Removed(scene);
        Player plr = Engine.Scene.Tracker.GetEntity<Player>();
        if (plr != null)
            SwitchToGameplay(plr);
        MouseClick -= Click;
    }

    private void SwitchToGameplay(Player plr)
    {
        if (plr != null)
        {
            plr.StateMachine.Locked = false;
            plr.StateMachine.State = 0;
            plr.level.CanRetry = true;
            plr.DummyGravity = true;
            plr.Speed = Vector2.Zero;
            UMHSpawn spawn = Engine.Scene.Tracker.GetEntity<UMHSpawn>();
            if (spawn != null)
                plr.Position = spawn.Position;
            plr.Visible = true;
        }

        foreach (Ghost nplr in Engine.Scene.Tracker.GetEntities<Ghost>())
            nplr.Visible = true;

        this.Visible = false;
        this.Active = false;
    }

    public Vector2 ToWorldspace(Vector2 screenPos)
    {
        return new Vector2(
            (float)screenPos.X / 1920f * level.Camera.Viewport.Width / level.Camera.zoom.X + level.Camera.X,
            (float)screenPos.Y / 1080f * level.Camera.Viewport.Height / level.Camera.zoom.Y + level.Camera.Y
        );
    }

    public Vector2 ToScreenspace(Vector2 worldPos)
    {
        return new Vector2(
            (worldPos.X - level.Camera.X) * level.Camera.zoom.X / level.Camera.Viewport.Width * 1920f,
            (worldPos.Y - level.Camera.Y) * level.Camera.zoom.Y / level.Camera.Viewport.Height * 1080f
        );
    }
}