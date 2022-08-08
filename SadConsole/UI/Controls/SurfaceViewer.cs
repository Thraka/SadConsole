using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

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

    private ICellSurface _surface;
    private ScrollBarModes _scrollModes;

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
    public ICellSurface ChildSurface => _surface;

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


    /// <summary>
    /// Creates a new drawing surface control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control.</param>
    /// <param name="surface">The surface to view.</param>
    public SurfaceViewer(int width, int height, ICellSurface? surface = null) : base(width, height)
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
        SetSurface(surface);

        UseMouse = true;
    }

    /// <summary>
    /// Sets the surface for the view.
    /// </summary>
    /// <param name="surface"></param>
    [MemberNotNull("_surface")]
    public void SetSurface(ICellSurface? surface)
    {
        if (surface == null)
        {
            ResetSurface();
            return;
        }

        if (_surface == surface) return;

        if (_surface != null)
            _surface.IsDirtyChanged -= _surface_IsDirtyChanged;

        _surface = new CellSurface(surface, Width, Height);
        _surface.DefaultBackground = surface.DefaultBackground;
        _surface.DefaultForeground = surface.DefaultForeground;

        IsDirty = true;

        SurfaceControl.Surface = _surface;
        _surface.IsDirty = true;
        _surface.IsDirtyChanged += _surface_IsDirtyChanged;
    }

    /// <summary>
    /// Resets <see cref="ChildSurface"/> to a 1x1 surface.
    /// </summary>
    [MemberNotNull("_surface")]
    public void ResetSurface()
    {
        if (_surface != null)
            _surface.IsDirtyChanged -= _surface_IsDirtyChanged;

        _surface = new CellSurface(1, 1);
        _surface.DefaultBackground = Color.Transparent;
        _surface.Clear();

        IsDirty = true;

        SurfaceControl.Surface = _surface;
        _surface.IsDirty = true;
        _surface.IsDirtyChanged += _surface_IsDirtyChanged;
    }

    /// <summary>
    /// Processes the mouse for the scrollers and then the surface area.
    /// </summary>
    /// <param name="state">The state of the mouse.</param>
    /// <returns><see langword="true"/> when the mouse was processed by a scroller or the surface area.</returns>
    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (IsEnabled && UseMouse)
        {
            var newState = new ControlMouseState(this, state);

            if (newState.IsMouseOver)
            {
                if (VerticalScroller.IsVisible && VerticalScroller.ProcessMouse(state))
                    return true;

                if (HorizontalScroller.IsVisible && HorizontalScroller.ProcessMouse(state))
                    return true;

                if (MouseState_IsMouseOver != true)
                {
                    MouseState_IsMouseOver = true;
                    OnMouseEnter(newState);
                }

                bool preventClick = MouseState_EnteredWithButtonDown;
                OnMouseIn(newState);

                if (!preventClick && state.Mouse.LeftClicked)
                    OnLeftMouseClicked(newState);

                if (!preventClick && state.Mouse.RightClicked)
                    OnRightMouseClicked(newState);

                return true;
            }
            else
            {
                if (MouseState_IsMouseOver)
                {
                    MouseState_IsMouseOver = false;
                    OnMouseExit(newState);
                }
            }
        }

        return false;
    }

    private void _surface_IsDirtyChanged(object? sender, EventArgs e) =>
        IsDirty = _surface.IsDirty;

    private void VerticalScroller_ValueChanged(object? sender, EventArgs e) =>
        _surface.ViewPosition = (_surface.ViewPosition.X, VerticalScroller.Value);

    private void HorizontalScroller_ValueChanged(object? sender, EventArgs e) =>
        _surface.ViewPosition = (HorizontalScroller.Value, _surface.ViewPosition.Y);

    private class FauxDrawingTheme : Themes.DrawingAreaTheme
    {
        public override void UpdateAndDraw(ControlBase control, TimeSpan time) { }
    }
}
