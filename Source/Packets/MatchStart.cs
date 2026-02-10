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

public class MatchStart
{
    public int matchID { get; set; } = -1;
    public int lobbyID { get; set; } = -1;
    public string map { get; set; }
    public string room { get; set; }

    public MatchStart(int matchID, int lobbyID, string map, string room)
    {
        this.matchID = matchID;
        this.lobbyID = lobbyID;
        this.map = map;
        this.room = room;
    }

    public static void Receive(PlayerData playerInfo, MatchStart msg)
    {
        if (UMHModule.currentMap != msg.map || UMHModule.currentRoom != msg.room)
            return;
        UMHStartZone zone = null;
        foreach (UMHStartZone entity in Engine.Scene.Tracker.GetEntities<UMHStartZone>())
        {
            if (entity.StartID == msg.lobbyID)
            {
                zone = entity;
                break;
            }
        }
        if (zone == null)
            return;

        Player plr = zone.Scene.Tracker.GetEntity<Player>();
        if (plr != null)
        {
            plr.Position = zone.Position;
        }

        if (zone.PlayerIsInside)
        {
            
        }

    }
}