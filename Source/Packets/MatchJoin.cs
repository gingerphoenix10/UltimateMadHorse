using Celeste.Mod.CNetHelper.Data;
using Celeste.Mod.UMH.Entities;
using Celeste.Mod.UMH.ObjectTypes;
using Celeste.Mod.UMH.Triggers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Packets;

public class MatchJoin
{
    public int matchID { get; set; } = -1;
    public string map { get; set; }
    public string room { get; set; }

    public MatchJoin(int matchID, string map, string room)
    {
        this.matchID = matchID;
        this.map = map;
        this.room = room;
    }

    public static void Receive(PlayerData playerInfo, MatchJoin msg)
    {
        if (msg.map != UMHModule.currentMap)
            return;
        if (msg.matchID == -1)
        {
            Logger.Log("MatchJoin", "lmaoo someone tried to start a match on id -1");
            return;
        }
        if (!UMHManager.players.Contains(playerInfo.ID))
            UMHManager.players.Add(playerInfo.ID);
    }
}