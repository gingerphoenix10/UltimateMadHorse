using Celeste.Mod.CelesteNet.Client.Entities;
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

public class ObjectPick
{
    public int setIndex { get; set; }

    public ObjectPick(int setIndex) {
        this.setIndex = setIndex;
    }

    public static void Receive(PlayerData playerInfo, ObjectPick msg)
    {
        if (!UMHManager.players.Contains(playerInfo.ID))
            return;
        UMHManager manager = Engine.Scene.Tracker.GetEntity<UMHManager>();
        if (manager == null)
            return;
        if (manager.mouse != null)
            manager.mouse.picked.Add(playerInfo.ID);
        manager.CheckForEditMode();

        if (manager.box == null) 
            return;
        if (manager.box.options.Length >= msg.setIndex && manager.box.options[msg.setIndex].Value != null)
        {
            manager.box.options[msg.setIndex].Value.Visible = false;
            manager.box.options[msg.setIndex].Value.RemoveSelf();

            Ghost player = null;
            foreach (Ghost ghost in Engine.Scene.Tracker.GetEntities<Ghost>())
            {
                if (ghost.PlayerInfo.ID == playerInfo.ID)
                {
                    player = ghost;
                    break;
                }
            }
            if (player == null)
                return;

            if (!manager.mouse.remotePlacements.ContainsKey(playerInfo.ID) || manager.mouse.remotePlacements[playerInfo.ID] == null)
            {
                manager.mouse.remotePlacements[playerInfo.ID] = new PlacementController(manager.mouse);
                manager.mouse.remotePlacements[playerInfo.ID].HoldingIndex = manager.box.options[msg.setIndex].Key;
                manager.mouse.remotePlacements[playerInfo.ID].remote = player;
                Engine.Scene.Add(manager.mouse.remotePlacements[playerInfo.ID]);
                Engine.Scene.Add(manager.mouse.remotePlacements[playerInfo.ID].holding);
            }
            else manager.mouse.remotePlacements[playerInfo.ID].HoldingIndex = manager.box.options[msg.setIndex].Key;
        }
    }
}