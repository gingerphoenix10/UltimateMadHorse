using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.Entities;

[CustomEntity("UMH/UMHSpawn")]
[Tracked(true)]
public class UMHSpawn : Entity
{
    public UMHSpawn(EntityData data, Vector2 offset)
        : base(data.Position+offset)
    {}
}