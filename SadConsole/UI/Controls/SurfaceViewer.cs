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

        UpdateAndRedraw(TimeSpan.Zero);
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

    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        RefreshThemeStateColors(FindThemeColors());

        bool showWidthScroll = false;
        bool showHeightScroll = false;

        // Determine if any scroller is shown
        if (ScrollBarMode == SurfaceViewer.ScrollBarModes.Always)
        {
            showWidthScroll = true;
            showHeightScroll = true;
        }
        else if (ScrollBarMode == SurfaceViewer.ScrollBarModes.AsNeeded)
        {
            showWidthScroll = ChildSurface.Width > Width;
            showHeightScroll = ChildSurface.Height > Height;
        }

        // If only 1 scroller needs to be shown, but the opposite axis size == the available height,
        // it gets cut off so we need to show the other scroller.
        if (showWidthScroll && !showHeightScroll && ChildSurface.Height == Height)
            showHeightScroll = true;

        else if (showHeightScroll && !showWidthScroll && ChildSurface.Width == Width)
            showWidthScroll = true;

        // Show or hide the scrollers
        HorizontalScroller.IsVisible = showWidthScroll;
        VerticalScroller.IsVisible = showHeightScroll;

        // Based on which are shown, they may be different sizes
        // Resize and show them
        if (showWidthScroll && showHeightScroll)
        {
            // Account for the corner between them
            if (HorizontalScroller.Width != Width - 1)
                HorizontalScroller.Resize(Width - 1, 1);

            if (VerticalScroller.Height != Height - 1)
                VerticalScroller.Resize(1, Height - 1);

            Surface.ViewWidth = Width - 1;
            Surface.ViewHeight = Height - 1;
        }
        else if (showWidthScroll)
        {
            if (HorizontalScroller.Width != Width)
                HorizontalScroller.Resize(Width, 1);

            Surface.ViewWidth = Width;
            Surface.ViewHeight = Height - 1;
        }
        else if (showHeightScroll)
        {
            if (VerticalScroller.Height != Height)
                VerticalScroller.Resize(1, Height);

            Surface.ViewWidth = Width - 1;
            Surface.ViewHeight = Height;
        }

        // Ensure scroll bars positioned correctly
        VerticalScroller.Position = new Point(Width - 1, 0);
        HorizontalScroller.Position = new Point(0, Height - 1);
        HorizontalScroller.IsEnabled = false;
        VerticalScroller.IsEnabled = false;

        if (ChildSurface.ViewWidth != ChildSurface.Width)
        {
            HorizontalScroller.Maximum = ChildSurface.Width - ChildSurface.ViewWidth;
            HorizontalScroller.IsEnabled = true;
        }
        if (ChildSurface.ViewHeight != ChildSurface.Height)
        {
            VerticalScroller.Maximum = ChildSurface.Height - ChildSurface.ViewHeight;
            VerticalScroller.IsEnabled = true;
        }

        ChildSurface.IsDirty = false;
    }

    private void _surface_IsDirtyChanged(object? sender, EventArgs e)
    {
        IsDirty = _surface.IsDirty;

        VerticalScroller.Value = _surface.ViewPosition.Y;
        HorizontalScroller.Value = _surface.ViewPosition.X;
    }

    private void VerticalScroller_ValueChanged(object? sender, EventArgs e) =>
        _surface.ViewPosition = (_surface.ViewPosition.X, VerticalScroller.Value);

    private void HorizontalScroller_ValueChanged(object? sender, EventArgs e) =>
        _surface.ViewPosition = (HorizontalScroller.Value, _surface.ViewPosition.Y);
}
