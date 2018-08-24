using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using SadConsole.DrawCalls;
using SadConsole.Surfaces;

namespace SadConsole.Entities
{
    /// <summary>
    /// Defines an area for a scene.
    /// </summary>
    [DataContract]
    public class Zone: ScreenObject
    {
        private Surfaces.Basic _debugSurface;
        private string _title = "Zone";
        private Cell _debugAppearance = new Cell(Color.White, Color.Black, 0);

        /// <summary>
        /// The area the zone covers.
        /// </summary>
        [DataMember] public readonly Rectangle Area;

        /// <summary>
        /// A title for the area.
        /// </summary>
        [DataMember] public string DebugTitle
        {
            get => _title;
            set
            {
                _title = value;
                Rebuild();
            }
        }

        /// <summary>
        /// A visual for the area to help debug.
        /// </summary>
        [DataMember] public Cell DebugAppearance
        {
            get => _debugAppearance;
            set
            {
                _debugAppearance = value;
                Rebuild();
            }
        }

        /// <summary>
        /// Key-value pairs for the zone.
        /// </summary>
        [DataMember] public Dictionary<string, string> Settings = new Dictionary<string, string>();

        public Zone(Rectangle area)
        {
            IsVisible = false;
            Area = area;
        }

        protected override void OnVisibleChanged()
        {
            Rebuild();
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            Global.DrawCalls.Add(new DrawCalls.DrawCallColoredRect(Area, DebugAppearance.Background));

            base.Draw(timeElapsed);
        }
        private void Rebuild()
        {
            if (IsVisible)
            {
                //_debugSurface = new Basic(Area.Width, Area.Height);
                //_debugSurface.
                //_debugSurface.Print(0, 0, DebugTitle);
                //_debugSurface.Draw(TimeSpan.Zero);
            }
            else
                _debugSurface = null;
        }

    }
}
