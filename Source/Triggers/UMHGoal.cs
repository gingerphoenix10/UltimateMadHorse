using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.Entities;
using Celeste.Mod.UMH.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Celeste.Mod.UMH.Triggers;

[Tracked]
[CustomEntity("UMH/UMHGoal")]
public class UMHGoal : CNetTrigger
{
    EntityData data;
    public UMHGoal(EntityData data, Vector2 offset) : base(data, offset)
    {
        this.data = data;

    }

    public override void Added(Scene scene)
    {
        foreach (Vector2 node in data.Nodes)
        {
            Decal flag = new("7-summit/SummitFlag00", node, Vector2.One*0.4f, Depth);
            scene.Add(flag);
        }
        base.Added(scene);
    }

    public override void Render()
    {
        base.Render();
            /*"decals/7-summit/SummitFlag00"
            string extension = Path.GetExtension(texture);
            string input = Path.Combine("decals", texture.Replace(extension, "")).Replace('\\', '/');
            Name = Regex.Replace(input, "\\d+$", string.Empty);
            textures = GFX.Game.GetAtlasSubtextures(Name);*/
            //Decal
            //GFX.Game.GetAtlasSubtextures("decals/")[0].DrawCentered(node + Position);
    }

    public override void Update()
    {
        base.Update();
        foreach (Actor actor in playersInside)
        {
            if (actor is Player player)
            {
                if (!player.onGround)
                    continue;
                player.StateMachine.State = 11;
                player.StateMachine.Locked = true;
                player.DummyAutoAnimate = false;
                player.DummyGravity = false;
                player.DummyFriction = false;
                player.Speed = Vector2.Zero;
                player.Sprite.Play("spin", false, false);
            } else if (actor is Ghost ghost)
            {
                ghost.Sprite.Play("spin", false, false);
            }
        }
    }
}