using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH.ObjectTypes;

public class UCHDreamBlock : UCHObject
{
    public override Entity Create()
    {
        return new DreamBlock(Vector2.Zero, 8*2, 8*8, null, false, false);
    }
}
