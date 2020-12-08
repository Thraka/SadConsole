using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SadConsole.DrawCalls;
using SadRogue.Primitives;

namespace SadConsole.Entities
{
    /// <summary>
    /// Defines an area for a scene.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("Zone")]
    public class Zone : ScreenObject
    {
        /// <summary>
        /// The area the zone covers.
        /// </summary>
        [DataMember] public readonly Area Area;
        
        /// <summary>
        /// A visual for the area to help debug.
        /// </summary>
        [DataMember]
        public ColoredGlyph Appearance { get; set; }

        /// <summary>
        /// Key-value pairs for the zone.
        /// </summary>
        [DataMember] public Dictionary<string, string> Settings = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new zone object with the specified area.
        /// </summary>
        /// <param name="area">The area of the zone.</param>
        public Zone(Area area)
        {
            IsVisible = false;
            UseMouse = false;
            UseKeyboard = false;

            Area = area;
        }
    }
}
