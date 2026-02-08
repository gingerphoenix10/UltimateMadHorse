using Celeste.Mod.CNetHelper.Data;
using Celeste.Mod.UMH.Entities;
using Celeste.Mod.UMH.ObjectTypes;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Packets;

public class MatchStart
{
    public int matchID { get; set; } = -1;
    public string map { get; set; }
    public string room { get; set; }

    public MatchStart(int matchID, string map, string room)
    {
        this.matchID = matchID;
        this.map = map;
        this.room = room;
    }

    public static void Receive(PlayerData playerInfo, MatchStart msg)
    {
        if (UMHModule.currentMap != msg.map || UMHModule.currentRoom != msg.room)
            return;

    }
}