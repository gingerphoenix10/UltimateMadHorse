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

public class MatchLeave
{
    public MatchLeave() {}

    public static void Receive(PlayerData playerInfo, MatchLeave msg)
    {
        if (UMHManager.players.Contains(playerInfo.ID))
            UMHManager.players.Remove(playerInfo.ID);
    }
}