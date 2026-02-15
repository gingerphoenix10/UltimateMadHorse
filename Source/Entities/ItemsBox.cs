using Celeste.Mod.CelesteNet.Client;
using Celeste.Mod.CelesteNet.Client.Components;
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
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Entities;


[CustomEntity("UMH/ItemsBox")]
[Tracked(false)]
public class ItemsBox : Entity
{
    public UMHManager manager;
    public KeyValuePair<int, Entity>[] options;
    public ItemsBox(UMHManager manager)
        : base(new Vector2())
    {
        this.manager = manager;
        this.Collider = new Hitbox((int)(320 * 0.8f), (int)(180 * 0.8));
        this.Center = new Vector2(320 / 2, 180 / 2);
        this.Depth = Depths.Top + 1;
        MouseController.MouseClick += Click;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        options = new KeyValuePair<int, Entity>[manager.itemsPerRound];
        for (var i = 0; i < manager.itemsPerRound; i++)
        {
            int index = manager.itemsPerRound * manager.roundNumber + i;
            int objectIndex = manager.DeterministicRandom(index, 0, Pools.pools[manager.PoolIndex].Length);
            options[i] = new KeyValuePair<int, Entity>(objectIndex, Pools.pools[manager.PoolIndex][objectIndex].Create());
            Console.WriteLine($"Option {i}: Pool {manager.PoolIndex}, Index {objectIndex}");
            options[i].Value.Depth = Depths.Top;
            scene.Add(options[i].Value);

            float spacing = (320f - (manager.itemsPerRound * 80f)) / (manager.itemsPerRound + 1);
            float leftEdge = spacing + i * (80f + spacing);

            //options[i].Value.Center = Center;
            var xPos = i - manager.itemsPerRound / 2f + 0.5f;
            //options[i].Value.Position.X = (CenterX + Width / manager.itemsPerRound * xPos) - options[i].Value.Collider.CenterX;
            //options[i].Value.CenterX = CenterX + Width / manager.itemsPerRound * xPos;
            Pools.pools[manager.PoolIndex][objectIndex].MoveTo(options[i].Value, new Vector2((int)((CenterX + Width / manager.itemsPerRound * xPos) - options[i].Value.Collider.CenterX), Position.Y + Collider.Height/2 - options[i].Value.Collider.Height/2));
        }
    }

    public void Click(MouseController controller)
    {
        for (int i = 0; i < options.Length; i++)
        {
            KeyValuePair<int, Entity> entityInfo = options[i];

            if (entityInfo.Value == null || !entityInfo.Value.Visible || !entityInfo.Value.CollidePoint(controller.ToWorldspace(controller.Position)))
                continue;
            controller.cancelPlace = true;
            if (controller.placement == null)
            {
                controller.placement = new PlacementController(controller);
                controller.placement.HoldingIndex = entityInfo.Key;
                Scene.Add(controller.placement);
                Scene.Add(controller.placement.holding);
            }
            else controller.placement.HoldingIndex = entityInfo.Key;
            controller.localVisible = false;
            CNetHelperModule.Send(new ObjectPick(i, entityInfo.Key));
            entityInfo.Value.Visible = false;
            entityInfo.Value.RemoveSelf();
            CelesteNetClientContext context = CelesteNetClientModule.Instance.Context;
            if (context != null)
            {
                CelesteNetMainComponent main = context.Main;
                if (main != null)
                {
                    GhostNameTag name = main.PlayerNameTag;
                    if (name != null)
                        name.Visible = false;
                }
            }
            MouseController.MouseClick -= Click;
            break;
        }
        manager.CheckForEditMode();
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        MouseController.MouseClick -= Click;
        foreach (KeyValuePair<int, Entity> option in options)
        {
            if (option.Value != null)
                option.Value.RemoveSelf();
        }
    }

    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        MouseController.MouseClick -= Click;
    }

    public override void Render()
    {
        base.Render();
        Draw.Rect(Position, Width, Height, Color.DarkRed * 0.5f);
    }
}