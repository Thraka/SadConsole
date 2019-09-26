#if XNA
using Microsoft.Xna.Framework;
#endif

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SadConsole.Entities
{
    // TODO: make sure hotspot works.

    /// <summary>
    /// A group of positions with a set of settings.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("Hotspot")]
    public class Hotspot : Console
    {
        private readonly CellSurface _debugSurface;
        private readonly CellSurface _parentSurface;
        private Cell _debugAppearance = new Cell(Color.White, Color.Black, 0);

        /// <summary>
        /// The hotspot position on the map.
        /// </summary>
        [DataMember]
        public HashSet<Point> Positions { get; } = new HashSet<Point>();

        /// <summary>
        /// A visual for the area to help debug.
        /// </summary>
        [DataMember]
        public Cell DebugAppearance
        {
            get => _debugAppearance;
            set
            {
                _debugAppearance = value;
                Rebuild();
            }
        }

        /// <summary>
        /// Key-value pairs for the hotspot.
        /// </summary>
        [DataMember]
        public Dictionary<string, string> Settings { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new hotspot object.
        /// </summary>
        public Hotspot() : base(1, 1) => IsVisible = false;

        /// <summary>
        /// Return true when the specified point is in the list of <see cref="Positions"/>.
        /// </summary>
        /// <param name="point">The position to check for.</param>
        /// <returns>True or false based on if the position is associated with the Hotspot.</returns>
        public bool Contains(Point point) => Positions.Contains(point);

        /// <inheritdoc />
        protected override void OnParentChanged(Console oldParent, Console newParent)
        {
            //_parentSurface = newParent as SurfaceBase;
        }

        /// <inheritdoc />
        protected override void OnVisibleChanged() => Rebuild();

        public override void Draw(TimeSpan timeElapsed)
        {
            //if (IsVisible && _parentSurface != null)
            //{
            //    foreach (var spot in Positions)
            //    {
            //        if (!_parentSurface.ViewPort.Contains(spot)) continue;

            //        Global.DrawCalls.Add(new DrawCalls.DrawCallScreenObject(_debugSurface,
            //            spot - _parentSurface.ViewPort.Location + _parentSurface.CalculatedPosition,
            //            _parentSurface.UsePixelPositioning));
            //    }
            //}

            //base.Draw(timeElapsed);
        }


        private void Rebuild()
        {
            //if (IsVisible)
            //{
            //    _debugSurface = new Basic(1, 1);
            //    _debugSurface.DefaultBackground = _debugAppearance.Background;
            //    _debugSurface.DefaultForeground = _debugAppearance.Foreground;
            //    _debugSurface.SetCellAppearance(0, 0, _debugAppearance);
            //    _debugSurface.Draw(TimeSpan.Zero);
            //}
            //else
            //    _debugSurface = null;
        }
    }
}
