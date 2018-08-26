using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using SadConsole.DrawCalls;
using SadConsole.Surfaces;
using SadConsole;

namespace SadConsole.Entities
{
    /// <summary>
    /// Defines an area for a scene.
    /// </summary>
    [DataContract]
    public class Zone: ScreenObject
    {
        private Surfaces.Basic _debugSurface;
        private Surfaces.SurfaceBase _parentSurface;
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

        protected override void OnParentChanged(ScreenObject oldParent, ScreenObject newParent)
        {
            _parentSurface = newParent as SurfaceBase;
        }

        protected override void OnVisibleChanged()
        {
            Rebuild();
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible && _parentSurface != null)
            {
                if (_parentSurface.ViewPort.Intersects(Area))
                {
                    Global.DrawCalls.Add(new DrawCalls.DrawCallSurface(_debugSurface,
                        Area.Location - _parentSurface.ViewPort.Location + _parentSurface.CalculatedPosition,
                        _parentSurface.UsePixelPositioning));
                }
            }

            base.Draw(timeElapsed);
        }

        private void Rebuild()
        {
            if (IsVisible)
            {
                _debugSurface = new Basic(Area.Width, Area.Height);
                _debugSurface.DefaultBackground = DebugAppearance.Background;
                _debugSurface.Clear();
                _debugSurface.Print(0, 0, DebugTitle, DebugAppearance);
                _debugSurface.Draw(TimeSpan.Zero);
            }
            else
                _debugSurface = null;
        }

    }
}
