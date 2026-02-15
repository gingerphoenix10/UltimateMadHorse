using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.CNetHelper;
using Celeste.Mod.Entities;
using Celeste.Mod.UMH.Packets;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Entities;

[CustomEntity("UMH/UMHManager")]
[Tracked(false)]
public class UMHManager : Entity
{
    public MouseController mouse;
    public ItemsBox box;
    public int PoolIndex = 0;

    public enum MatchStates
    {
        PickItems,
        EditMode,
        PlayMode
    }
    public MatchStates matchState = MatchStates.PickItems;
    public int roundNumber = 1;
    public int itemsPerRound
    {
        get
        {
            int playerCount = players.Count + 1;
            return Math.Max(5, playerCount + 1);
        }
    }

    public static List<uint> players = new();
    public static int matchID = -1;

    public UMHManager()
        : base(Vector2.Zero)
    {
        mouse = new(this);
        mouse.Visible = false;

    }

    public override async void Added(Scene scene)
    {
        scene.Add(mouse);
        base.Added(scene);
        await Task.Delay(1000);
        scene.Add(box = new ItemsBox(this));
        mouse.Visible = true;
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

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        matchID = -1;
        players.Clear();
        Console.WriteLine("Reset");
        CNetHelperModule.Send(new MatchLeave(), false);
    }

    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        matchID = -1;
        players.Clear();
        CNetHelperModule.Send(new MatchLeave(), false);
    }

    public int DeterministicRandom(
        int index,
        int min,
        int max)
    {
        if (min >= max)
        {
            int newMax = min;
            min = max;
            max = newMax;
        }

        unchecked
        {
            int hash = matchID;
            hash = hash * 31 + index;
            hash ^= (hash << 13);
            hash ^= (hash >> 17);
            hash ^= (hash << 5);

            int range = max - min;
            int value = (hash & int.MaxValue) % range;

            return min + value;
        }
    }

    public void CheckForEditMode()
    {
        if (matchState != MatchStates.PickItems)
            return;
        if (mouse.picked.Count == players.Count && !mouse.localVisible)
        {
            box.RemoveSelf();
            mouse.localVisible = true;
            mouse.picked.Clear();
            matchState = MatchStates.EditMode;
        }
    }
}