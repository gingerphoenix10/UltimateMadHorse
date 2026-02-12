using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Entities;

[CustomEntity("UMH/UMHManager")]
[Tracked(false)]
public class UMHManager : Entity
{
    public MouseController mouse;
    public int PoolIndex = 0;
    public static List<uint> players = new();
    public static int matchID = -1;

    public UMHManager()
        : base(Vector2.Zero)
    {
        mouse = new();
        mouse.manager = this;
    }

    public override void Added(Scene scene)
    {
        scene.Add(mouse);
        base.Added(scene);
    }

    public override void Update()
    {
        if (matchID == -1)
            return;
        foreach (Ghost ghost in Scene.Tracker.GetEntities<Ghost>())
        {
            if (!players.Contains(ghost.PlayerInfo.ID))
            {
                ghost.RemoveSelf();
            }
        }
        base.Update();
    }

    public void NewRemoteObject(Entity remoteObject)
    {
        Scene.Add(remoteObject);
    }

}