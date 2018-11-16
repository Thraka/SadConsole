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
        private Basic _debugSurface;
        private SurfaceBase _parentSurface;
        private string _title = "Zone";
        private Cell _debugAppearance = new Cell(Color.White, Color.Black, 0);
        private DrawCallSurface _drawCall;

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
        
        /// <summary>
        /// Creates a new zone object with the specified area.
        /// </summary>
        /// <param name="area">The area of the zone.</param>
        public Zone(Rectangle area)
        {
            IsVisible = false;
            Area = area;
        }

        /// <inheritdoc />
        protected override void OnParentChanged(ScreenObject oldParent, ScreenObject newParent)
        {
            _parentSurface = newParent as SurfaceBase;
        }

        /// <inheritdoc />
        protected override void OnVisibleChanged()
        {
            Rebuild();
        }

        /// <inheritdoc />
        public override void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible && _parentSurface != null)
            {
                if (_parentSurface.ViewPort.Intersects(Area))
                {
                    if (_parentSurface.UsePixelPositioning)
                    {
                        _drawCall.Position = (Area.Location - _parentSurface.ViewPort.Location +
                                              _parentSurface.CalculatedPosition).ToVector2();
                    }
                    else
                    {
                        _drawCall.Position = _debugSurface.Font.GetWorldPosition((Area.Location - _parentSurface.ViewPort.Location +
                                              _parentSurface.CalculatedPosition)).ToVector2();
                    }
                    
                    Global.DrawCalls.Add(_drawCall);
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
                _drawCall = new DrawCallSurface(_debugSurface, Point.Zero, _parentSurface.UsePixelPositioning);
            }
            else
                _debugSurface = null;
        }

    }
}
