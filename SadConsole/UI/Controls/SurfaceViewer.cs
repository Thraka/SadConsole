using System;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// Draws a <see cref="ICellSurface"/> within an area. Optionally supports scroll bars.
    /// </summary>
    [DataContract]
    public class SurfaceViewer : CompositeControl
    {
        /// <summary>
        /// Determines how the control draws the scroll bars.
        /// </summary>
        public enum ScrollBarModes
        {
            /// <summary>
            /// Always draw the scroll bars on the control.
            /// </summary>
            Always,
            /// <summary>
            /// Never show the scroll bars on the control.
            /// </summary>
            Never,
            /// <summary>
            /// Display the scroll bars if needed.
            /// </summary>
            AsNeeded
        }

        private bool _isNullSurface;
        private ICellSurface _surface;
        private ScrollBarModes _scrollModes;
        private ICellSurface _originalSurface;

        /// <summary>
        /// Sets the visual behavior of the scroll bars for the control.
        /// </summary>
        [DataMember]
        public ScrollBarModes ScrollBarMode
        {
            get => _scrollModes;
            set { _scrollModes = value; IsDirty = true; }
        }

        /// <summary>
        /// The surface rendered on this control.
        /// </summary>
        [DataMember(Name = "ChildSurface")]
        public ICellSurface ChildSurface
        {
            get => _surface;
            set
            {
                if (_surface == value) return;

                if (_surface != null)
                    _surface.IsDirtyChanged -= _surface_IsDirtyChanged;

                if (value == null)
                {
                    _surface = new CellSurface(1, 1);
                    _surface.DefaultBackground = Color.Transparent;
                    _surface.Clear();
                    _isNullSurface = true;
                }
                else
                {
                    _surface = new CellSurface(value, Width, Height);
                    _surface.DefaultBackground = value.DefaultBackground;
                    _surface.DefaultForeground = value.DefaultForeground;
                    _isNullSurface = false;
                }

                IsDirty = true;

                SurfaceControl.Surface = _surface;
                _surface.IsDirty = true;
                _surface.IsDirtyChanged += _surface_IsDirtyChanged;
            }
        }

        /// <summary>
        /// The horizontal scroll bar. This shouldn't be changed.
        /// </summary>
        public ScrollBar HorizontalScroller;

        /// <summary>
        /// The vertical scroll bar. This shouldn't be changed.
        /// </summary>
        public ScrollBar VerticalScroller;

        /// <summary>
        /// A control that displays the view area of the <see cref="ChildSurface"/>.
        /// </summary>
        public DrawingArea SurfaceControl;

        private void _surface_IsDirtyChanged(object sender, EventArgs e)
        {
            IsDirty = _surface.IsDirty;
        }

        /// <summary>
        /// Creates a new drawing surface control with the specified width and height.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        /// <param name="surface">The surface to view.</param>
        public SurfaceViewer(int width, int height, ICellSurface surface = null) : base(width, height)
        {
            UseMouse = false;
            UseKeyboard = false;
            TabStop = false;

            HorizontalScroller = new ScrollBar(Orientation.Horizontal, width);
            VerticalScroller = new ScrollBar(Orientation.Vertical, height);
            SurfaceControl = new DrawingArea(width, height);
            SurfaceControl.Theme = new FauxDrawingTheme();

            AddControl(HorizontalScroller);
            AddControl(VerticalScroller);
            AddControl(SurfaceControl);

            HorizontalScroller.ValueChanged += HorizontalScroller_ValueChanged;
            VerticalScroller.ValueChanged += VerticalScroller_ValueChanged;
            ChildSurface = surface;

            UseMouse = true;
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            return base.ProcessMouse(state);
        }

        private void VerticalScroller_ValueChanged(object sender, EventArgs e) =>
            _surface.ViewPosition = (_surface.ViewPosition.X, VerticalScroller.Value);

        private void HorizontalScroller_ValueChanged(object sender, EventArgs e)
        {
            _surface.ViewPosition = (HorizontalScroller.Value, _surface.ViewPosition.Y);
        }

        private class FauxDrawingTheme: Themes.DrawingAreaTheme
        {
            public override void UpdateAndDraw(ControlBase control, TimeSpan time) { }
        }
    }
}
