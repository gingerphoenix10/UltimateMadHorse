using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.ObjectTypes;

public abstract class UCHObject
{
    public abstract Entity Create();
    public virtual void MoveTo(Entity ent, Vector2 position)
    {
        ent.Position = position;
    }
}
