using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Maps
{
    [Flags]
    public enum TileFlags
    {
        None = 0,
        Seen = 1 << 0,
        BlockMove = 1 << 1,
        BlockLOS = 1 << 2,
        Lighted = 1 << 3,
        PermaLight = 1 << 4,
        InLOS = 1 << 5,
        PermaInLOS = 1 << 6,
        RegionLighted = 1 << 7
    }
}
