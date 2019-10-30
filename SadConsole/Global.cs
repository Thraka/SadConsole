using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole
{
    public static class Global
    {
        /// <summary>
        /// The active screen processed by the game.
        /// </summary>
        public static Console Screen { get; set; }

        /// <summary>
        /// A global random number generator.
        /// </summary>
        public static Random Random { get; set; } = new Random();

        /// <summary>
        /// The elapsed time between now and the last update call.
        /// </summary>
        public static TimeSpan UpdateFrameDelta { get; set; }

        /// <summary>
        /// The elapsed time between now and the last draw call.
        /// </summary>
        public static TimeSpan DrawFrameDelta { get; set; }
    }
}
