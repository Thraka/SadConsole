#if SFML
using Point = SFML.System.Vector2i;
using Rectangle = SFML.Graphics.IntRect;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using System.Collections.Generic;
#endif

namespace SadConsole.Game
{
    public class Hotspot
    {
        /// <summary>
        /// The hotspot position on the map.
        /// </summary>
        public Point Position;

        /// <summary>
        /// A title for the area.
        /// </summary>
        public string Title;

        /// <summary>
        /// A visual for the hotspot to help debug.
        /// </summary>
        public CellAppearance DebugAppearance = new CellAppearance(Color.White, Color.Black, 0);

        /// <summary>
        /// Key-value pairs for the hotspot.
        /// </summary>
        public Dictionary<string, string> Settings = new Dictionary<string, string>();
    }
}
