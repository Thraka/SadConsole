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
        private const string DefaultDebugTitle = "Zone";

        private string _title = DefaultDebugTitle;

        /// <summary>
        /// The area the zone covers.
        /// </summary>
        [DataMember] public readonly Rectangle Area;

        /// <summary>
        /// A title for the area.
        /// </summary>
        [DataMember]
        public string DebugTitle
        {
            get => _title;
            set
            {
                _title = value;

                if (string.IsNullOrWhiteSpace(_title))
                    _title = DefaultDebugTitle;
            }
        }

        /// <summary>
        /// A visual for the area to help debug.
        /// </summary>
        [DataMember]
        public ColoredGlyph DebugAppearance { get; set; }

        /// <summary>
        /// Key-value pairs for the zone.
        /// </summary>
        [DataMember] public Dictionary<string, string> Settings = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new zone object with the specified area.
        /// </summary>
        /// <param name="area">The area of the zone.</param>
        public Zone(Rectangle area)
        {
            IsVisible = false;
            UseMouse = false;
            UseKeyboard = false;

            Area = area;
        }
    }
}
