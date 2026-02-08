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

[Tracked]
[CustomEntity("UMH/UMHStartZone")]
public class UMHStartZone : CNetTrigger
{
    public UMHStartZone(EntityData data, Vector2 offset) : base(data, offset)
    {
    }

    public override void OnPlayerLeft(Actor player)
    {
        base.OnPlayerLeft(player);
        string players = "";
        foreach (Actor plr in playersInside)
        {
            string usrname = (plr is Ghost ghost) ? ghost.PlayerInfo.Name : "localplayer";
            players += (players.Length == 0 ? "" : ", ") + usrname;
        }
        Console.WriteLine(players);
    }

    public override void OnPlayerEntered(Actor player)
    {
        base.OnPlayerEntered(player);
        string players = "";
        foreach (Actor plr in playersInside)
        {
            string usrname = (plr is Ghost ghost) ? ghost.PlayerInfo.Name : "localplayer";
            players += (players.Length == 0 ? "" : ", ") + usrname;
        }
        Console.WriteLine(players);
    }

    public void StartGame()
    {
        int MatchID = Calc.Random.Next(int.MaxValue);
    }
}