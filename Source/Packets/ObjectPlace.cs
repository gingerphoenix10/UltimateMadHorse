using Celeste.Mod.CNetHelper.Data;
using Celeste.Mod.UMH.Entities;
using Celeste.Mod.UMH.ObjectTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Packets;

public class ObjectPlace
{
    public int EntityPool { get; set; } = -1;
    public int EntityID { get; set; } = -1;
    public int x { get; set; }
    public int y { get; set; }
    public string map { get; set; }
    public string room { get; set; }

    [JsonIgnore]
    public Entity _createdEntity = null;
    [JsonIgnore]
    public Entity CreatedEntity {
        get {
            if (_createdEntity != null)
                return _createdEntity;
            if (EntityPool != -1 && EntityID != -1)
            {
                UCHObject[] pool = Pools.pools[EntityPool];
                UCHObject entity = pool[EntityID];
                Entity newEntity = entity.Create();
                entity.MoveTo(newEntity, new Vector2(x, y));
                _createdEntity = newEntity;
                return newEntity;
            }
            return null;
        }
    }

    public ObjectPlace(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static void Receive(PlayerData playerInfo, ObjectPlace msg)
    {
        if (!UMHManager.players.Contains(playerInfo.ID))
            return;
        UMHManager manager = Engine.Scene.Tracker.GetEntity<UMHManager>();
        if (manager.mouse.remotePlacements.TryGetValue(playerInfo.ID, out PlacementController placement))
        {
            if (placement.Place(new Vector2(msg.x, msg.y)))
            {
                manager.mouse.placed.Add(playerInfo.ID);
                placement.RemoveSelf();
                manager.mouse.remotePlacements[playerInfo.ID] = null;
                manager.CheckForPlayMode();
            }
        }
    }
}