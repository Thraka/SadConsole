using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.Entities
{
    /// <summary>
    /// A group of positions with a set of settings.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("Hotspot")]
    public class Hotspot : ScreenObject
    {
        /// <summary>
        /// The hotspot position on the map.
        /// </summary>
        [DataMember]
        public HashSet<Point> Positions { get; } = new HashSet<Point>();
        
        /// <summary>
        /// A visual for the area to help debug.
        /// </summary>
        [DataMember]
        public ColoredGlyph DebugAppearance { get; set; }

        /// <summary>
        /// Key-value pairs for the hotspot.
        /// </summary>
        [DataMember]
        public Dictionary<string, string> Settings { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new hotspot object.
        /// </summary>
        public Hotspot()
        {
            IsVisible = false;
            UseKeyboard = false;
            UseMouse = false;
        }

        /// <summary>
        /// Return true when the specified point is in the list of <see cref="Positions"/>.
        /// </summary>
        /// <param name="point">The position to check for.</param>
        /// <returns>True or false based on if the position is associated with the Hotspot.</returns>
        public bool Contains(Point point) =>
            Positions.Contains(point);
    }
}
