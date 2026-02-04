using Celeste.Mod.UMH.ObjectTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.UMH
{
    public static class Pools
    {
        readonly static UCHObject[] All = new UCHObject[]
        {
            new UCHDreamBlock()
        };

        public readonly static List<UCHObject[]> pools = new()
        {
            All
        };
    }
}
