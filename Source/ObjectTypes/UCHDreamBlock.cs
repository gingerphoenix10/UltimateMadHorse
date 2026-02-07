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
    public int width = 2;
    public int height = 8;
    public UCHDreamBlock(int width = 2, int height = 8)
    {
        this.width = width;
        this.height = height;
    }
    public override Entity Create()
    {
        return new DreamBlock(Vector2.Zero, width*8, height*8, null, false, false);
    }
}
