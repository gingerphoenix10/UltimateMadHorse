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


[CustomEntity("UMH/PlacementController")]
[Tracked(false)]
public class PlacementController : Entity
{
    public Entity holding;
    public PlacementController(Vector2 pos)
        : base(pos)
    {
        base.Collider = new Hitbox(8f, 8f);

        TalkComponent talker = new TalkComponent(new Rectangle(0, 0, 8, 8), new Vector2(-0.5f, -20f), Interact)
        {
            PlayerMustBeFacing = false
        };
        talker.Enabled = true;
        talker.Visible = true;
        Add(talker);
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
            holding.Collidable = true;
        else return false;

        holding = null;
        return true;
    }
}