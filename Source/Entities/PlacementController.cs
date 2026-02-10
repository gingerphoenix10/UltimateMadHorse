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
                    if (Scene != null && holding != null)
                        Scene.Add(holding);
                }
                _holdingIndex = value;
            }
        }
    }

    public MouseController mouse;
    public PlacementController(Vector2 pos, MouseController mouse)
        : base(pos)
    {
        base.Collider = new Hitbox(8f, 8f);
        this.mouse = mouse;
    }

    private void Interact(Player player)
    {
        //Celeste.Instance.Exit();
    }

    const int gridSize = 8;
    public override void Update()
    {
        Position = new Vector2(
            (float)Math.Floor(Position.X / gridSize) * gridSize, 
            (float)Math.Floor(Position.Y / gridSize) * gridSize
        );

        if (holding == null)
        {
            base.Update();
            return;
        }

        Pools.pools[mouse.manager.PoolIndex][HoldingIndex].MoveTo(holding, new Vector2(
            Position.X,
            Position.Y
        ));

        holding.Collidable = false;
    }

    public bool Place()
    {
        if (holding != null)
        {
            holding.Collidable = true;
            var placemsg = new ObjectPlace(mouse.manager.PoolIndex, HoldingIndex, (int)Math.Floor(Position.X / gridSize) * gridSize, (int)Math.Floor(Position.Y / gridSize) * gridSize, UMHModule.currentMap, UMHModule.currentRoom);
            CNetHelperModule.Send(placemsg, false);
            Engine.Commands.Log(System.Text.Json.JsonSerializer.Serialize(placemsg));
        }
        else return false;

        holding = null;
        return true;
    }
}