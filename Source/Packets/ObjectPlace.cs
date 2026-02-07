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

    public ObjectPlace(int EntityPool, int EntityID, int x, int y, string map, string room)
    {
        this.EntityPool = EntityPool;
        this.EntityID = EntityID;
        this.x = x;
        this.y = y;
        this.map = map;
        this.room = room;
    }

    public static void Receive(PlayerData playerInfo, ObjectPlace msg)
    {
        if (UMHModule.currentMap != msg.map || UMHModule.currentRoom != msg.room)
            return;
        Engine.Commands.Log($"Received new object placement");
        if (msg.CreatedEntity != null)
        {
            Engine.Commands.Log($"Created entity from pool {msg.EntityPool} with ID {msg.EntityID} at position ({msg.x}, {msg.y})");
            var manager = Engine.Scene.Tracker.GetEntity<UMHManager>();
            if (manager != null)
                manager.NewRemoteObject(msg.CreatedEntity);
        }
    }
}