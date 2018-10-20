using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicTutorial.Maps
{
    public static class TileTypes
    {
        /// <summary>Represents a floor tile type.</summary>
        public const int Floor = 0;

        /// <summary>Represents a wall tile type.</summary>
        public const int Wall = 1;

        /// <summary>Represents a door tile type.</summary>
        public const int Door = 2;
    }

    public static class TileStates
    {
        public enum Door
        {
            Opened = 0,
            Closed = 1
        }
    }
}
