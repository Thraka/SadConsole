using Microsoft.Xna.Framework;

using System.Collections.Generic;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// A group of positions with a set of settings.
    /// </summary>
    public class Hotspot
    {
        /// <summary>
        /// The hotspot position on the map.
        /// </summary>
        public List<Point> Positions = new List<Point>();

        /// <summary>
        /// A title for the area.
        /// </summary>
        public string Title;

        /// <summary>
        /// A visual for the hotspot to help debug.
        /// </summary>
        public Cell DebugAppearance = new Cell(Color.White, Color.Black, 0);

        /// <summary>
        /// Key-value pairs for the hotspot.
        /// </summary>
        public Dictionary<string, string> Settings = new Dictionary<string, string>();

        /// <summary>
        /// Return true when the specified point is in the list of <see cref="Positions"/>.
        /// </summary>
        /// <param name="point">The position to check for.</param>
        /// <returns>True or false based on if the position is associated with the Hotspot.</returns>
        public bool Contains(Point point)
        {
            return Positions.Contains(point);
        }
    }
}
