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
    public int poolIndex { get; set; }

    public ObjectPick(int setIndex, int poolIndex) {
        this.setIndex = setIndex;
        this.poolIndex = poolIndex;
    }

    public static void Receive(PlayerData playerInfo, ObjectPick msg)
    {
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
        }
    }
}