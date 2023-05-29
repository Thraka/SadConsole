using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole.DrawCalls;
using SadRogue.Primitives;

namespace SadConsole.Entities
{
    /// <summary>
    /// Defines an area for a scene.
    /// </summary>
    [DataContract]
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
        /// The name of the zone.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Key-value pairs for the zone.
        /// </summary>
        [DataMember] public Dictionary<string, string> Settings = new Dictionary<string, string>();

        internal List<Entity> _members = new List<Entity>();

        /// <summary>
        /// A list of entities in the 
        /// </summary>
        public IReadOnlyList<Entity> Entities => _members;

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

        /// <summary>
        /// Returns the string "Zone - " followed by the <see cref="Name"/> of the zone. If the name is empty, appends the <see cref="Area"/> bounds.
        /// </summary>
        /// <returns>The name of the zone.</returns>
        public override string ToString() =>
            string.IsNullOrEmpty(Name) ? $"Zone - {Area.Bounds}" : $"Zone - {Name}";
    }
}
