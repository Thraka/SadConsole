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
    /// Ensures the base class <see cref="ControlBase.Surface"/> property is serialized when this control is serialized.
    /// </summary>
    [DataMember(Name = "ChildSurface")]
    private ICellSurface SurfaceSerialized
    {
        get => Surface;
        set => Surface = value;
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

        AddControl(HorizontalScroller);
        AddControl(VerticalScroller);

        HorizontalScroller.ValueChanged += HorizontalScroller_ValueChanged;
        VerticalScroller.ValueChanged += VerticalScroller_ValueChanged;

        Surface.IsDirtyChanged += _surface_IsDirtyChanged;

        if (surface != null)
            Surface = surface;

        UseMouse = true;
    }

    /// <summary>
    /// Resets the control's surface to a 1x1 surface transparent surface.
    /// </summary>
    public void ResetSurface()
    {
        Surface = new CellSurface(1, 1);
        Surface.DefaultBackground = Color.Transparent;
        Surface.Clear();

        IsDirty = true;
        Surface.IsDirty = true;
    }

    /// <summary>
    /// Handles and dehandles the <see cref="ICellSurface.IsDirtyChanged"/> event for the backing surface.
    /// </summary>
    /// <param name="oldSurface">The previous surface instance.</param>
    /// <param name="newSurface">The new surface instance.</param>
    protected override void OnSurfaceChanged(ICellSurface oldSurface, ICellSurface newSurface)
    {
        oldSurface.IsDirtyChanged -= _surface_IsDirtyChanged;
        newSurface.IsDirtyChanged += _surface_IsDirtyChanged;
    }

    /// <inheritdoc/>
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
            showWidthScroll = Surface.Width > Width;
            showHeightScroll = Surface.Height > Height;
        }

        // If only 1 scroller needs to be shown, but the opposite axis size == the available height,
        // it gets cut off so we need to show the other scroller.
        if (showWidthScroll && !showHeightScroll && Surface.Height == Height)
            showHeightScroll = true;

        else if (showHeightScroll && !showWidthScroll && Surface.Width == Width)
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

        if (Surface.ViewWidth != Surface.Width)
        {
            HorizontalScroller.Maximum = Surface.Width - Surface.ViewWidth;
            HorizontalScroller.IsEnabled = true;
        }
        if (Surface.ViewHeight != Surface.Height)
        {
            VerticalScroller.Maximum = Surface.Height - Surface.ViewHeight;
            VerticalScroller.IsEnabled = true;
        }

        base.UpdateAndRedraw(time);

        Surface.IsDirty = false;
    }

    private void _surface_IsDirtyChanged(object? sender, EventArgs e)
    {
        IsDirty = IsDirty || Surface.IsDirty;

        VerticalScroller.Value = Surface.ViewPosition.Y;
        HorizontalScroller.Value = Surface.ViewPosition.X;
    }

    private void VerticalScroller_ValueChanged(object? sender, EventArgs e) =>
        Surface.ViewPosition = (Surface.ViewPosition.X, VerticalScroller.Value);

    private void HorizontalScroller_ValueChanged(object? sender, EventArgs e) =>
        Surface.ViewPosition = (HorizontalScroller.Value, Surface.ViewPosition.Y);
}
