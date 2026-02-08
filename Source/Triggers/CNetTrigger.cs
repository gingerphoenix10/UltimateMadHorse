using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Triggers;

public abstract class CNetTrigger : Trigger
{
    public readonly List<Ghost> remotePlayersInside = new();
    public List<Actor> playersInside {
        get
        {
            List<Actor> players = new(remotePlayersInside.Cast<Actor>());
            if (PlayerIsInside)
                players.Add(Scene.Tracker.GetEntity<Player>());
            return players;
        }
    }
    public CNetTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        OnPlayerEntered(player);
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        OnPlayerLeft(player);
    }

    public virtual void OnPlayerEntered(Actor player)
    {
    }

    public virtual void OnPlayerLeft(Actor player)
    {
    }

    public override void Update()
    {
        foreach (Ghost ghost in Scene.Tracker.GetEntities<Ghost>())
        {
            if (playersInside.Contains(ghost)) {
                if (CollideCheck(ghost))
                    continue;
                remotePlayersInside.Remove(ghost);
                OnPlayerLeft(ghost);
            }
            else
            {
                if (!CollideCheck(ghost))
                    continue;
                remotePlayersInside.Add(ghost);
                OnPlayerEntered(ghost);
            }
        }
        base.Update();
    }
}