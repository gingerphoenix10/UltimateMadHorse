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


[CustomEntity("UMH/ItemsBox")]
[Tracked(false)]
public class ItemsBox : Entity
{
    public UMHManager manager;
    public List<KeyValuePair<int, Entity>> options = new();
    public ItemsBox(UMHManager manager)
        : base(new Vector2())
    {
        this.manager = manager;
        this.Collider = new Hitbox((int)(320 * 0.8f), (int)(180 * 0.8));
        this.Center = new Vector2(320 / 2, 180 / 2);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        for (var i = 0; i < manager.itemsPerRound; i++)
        {
            int index = manager.itemsPerRound * manager.roundNumber + i;
            int objectIndex = manager.DeterministicRandom(index, 0, Pools.pools[manager.PoolIndex].Length);
            options.Add(new KeyValuePair<int, Entity>(objectIndex, Pools.pools[manager.PoolIndex][objectIndex].Create()));
            Console.WriteLine($"Option {i}: Pool {manager.PoolIndex}, Index {objectIndex}");
            scene.Add(options[i].Value);

            float spacing = (320f - (manager.itemsPerRound * 80f)) / (manager.itemsPerRound + 1);
            float leftEdge = spacing + i * (80f + spacing);

            options[i].Value.CenterY = 180f / 2f;
        }
    }

    public override void Render()
    {
        base.Render();
        Draw.Rect(Position, Width, Height, Color.DarkRed * 0.5f);
    }
}