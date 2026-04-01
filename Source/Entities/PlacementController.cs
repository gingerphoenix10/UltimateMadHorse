using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.CNetHelper;
using Celeste.Mod.Entities;
using Celeste.Mod.UMH.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Entities;


[CustomEntity("UMH/PlacementController")]
[Tracked(false)]
public class PlacementController : Entity
{
    public Entity holding;
    private int _holdingIndex = -1;
    public int HoldingIndex
    {
        get => _holdingIndex;
        set
        {
            if (value != _holdingIndex)
            {
                if (holding != null)
                {
                    holding.RemoveSelf();
                    holding = null;
                }
                if (value != -1)
                {
                    holding = Pools.pools[mouse.manager.PoolIndex][value].Create();
                    if (Scene == null)
                        Console.WriteLine("Bad 1");
                    if (holding == null)
                        Console.WriteLine("Bad 2");
                    if (Scene != null && holding != null)
                        Scene.Add(holding);
                }
                _holdingIndex = value;
            }
        }
    }

    public MouseController mouse;
    public Ghost remote = null;
    public PlacementController(MouseController mouse, Ghost remotePlayer = null)
        : base(mouse.Position)
    {
        base.Collider = new Hitbox(8f, 8f);
        this.mouse = mouse;
        this.remote = remotePlayer;
    }

    const int gridSize = 8;
    public override void Update()
    {
        Position = new Vector2(
            (float)Math.Floor(Position.X / gridSize) * gridSize, 
            (float)Math.Floor(Position.Y / gridSize) * gridSize
        );
        Visible = mouse.localVisible;
        if (remote != null)
        {
            Position = new Vector2(
                (float)Math.Floor(remote.Position.X / gridSize) * gridSize,
                (float)Math.Floor(remote.Position.Y / gridSize) * gridSize
            );
            Visible = mouse.manager.matchState == UMHManager.MatchStates.EditMode;
        }

        if (holding == null)
        {
            base.Update();
            Console.WriteLine("Holding is null");
            return;
        }

        Pools.pools[mouse.manager.PoolIndex][HoldingIndex].MoveTo(holding, new Vector2(
            Position.X,
            Position.Y
        ));

        holding.Collidable = false;
        holding.Visible = Visible;
    }

    public bool Place(Vector2? position = null)
    {
        if (position != null)
        {
            this.Position = new Vector2(
                (float)Math.Floor(((Vector2)(position)).X / gridSize) * gridSize,
                (float)Math.Floor(((Vector2)(position)).Y / gridSize) * gridSize
            );

            Pools.pools[mouse.manager.PoolIndex][HoldingIndex].MoveTo(holding, new Vector2(
                Position.X,
                Position.Y
            ));
        }

        if (holding != null)
        {
            holding.Collidable = true;
            if (remote == null)
            {
                var placemsg = new ObjectPlace((int)Math.Floor(Position.X / gridSize) * gridSize, (int)Math.Floor(Position.Y / gridSize) * gridSize);
                CNetHelperModule.Send(placemsg, false);
                Engine.Commands.Log(System.Text.Json.JsonSerializer.Serialize(placemsg));
            }
        }
        else return false;

        holding = null;
        return true;
    }
}