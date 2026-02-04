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
    public int holdingIndex;
    public MouseController mouse;
    public PlacementController(Vector2 pos)
        : base(pos)
    {
        base.Collider = new Hitbox(8f, 8f);
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

        holding.Position = new Vector2(
            Position.X,// + holding.Width / 2f,
            Position.Y// + holding.Height / 2f
        );

        holding.Collidable = false;
    }

    public bool Place()
    {
        if (holding != null)
        {
            holding.Collidable = true;
            var placemsg = new ObjectPlace(mouse.manager.PoolIndex, holdingIndex, (int)Math.Floor(Position.X / gridSize) * gridSize, (int)Math.Floor(Position.Y / gridSize) * gridSize);
            CNetHelperModule.Send(placemsg, false);
            Engine.Commands.Log(System.Text.Json.JsonSerializer.Serialize(placemsg));
        }
        else return false;

        holding = null;
        return true;
    }
}