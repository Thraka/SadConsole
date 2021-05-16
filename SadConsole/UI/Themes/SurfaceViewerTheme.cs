using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// A basic theme for a drawing surface that simply fills the surface based on the state.
    /// </summary>
    [DataContract]
    public class SurfaceViewerTheme : ThemeBase
    {
        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            if (!(control is SurfaceViewer surfaceView)) return;

            RefreshTheme(control.FindThemeColors(), control);

            bool showWidthScroll = false;
            bool showHeightScroll = false;

            // Determine if any scroller is shown
            if (surfaceView.ScrollBarMode == SurfaceViewer.ScrollBarModes.Always)
            {
                showWidthScroll = true;
                showHeightScroll = true;
            }
            else if (surfaceView.ScrollBarMode == SurfaceViewer.ScrollBarModes.AsNeeded)
            {
                showWidthScroll = surfaceView.ChildSurface.Width > surfaceView.Width;
                showHeightScroll = surfaceView.ChildSurface.Height > surfaceView.Height;
            }

            // If only 1 scroller needs to be shown, but the opposite axis size == the available height,
            // it gets cut off so we need to show the other scroller.
            if (showWidthScroll && !showHeightScroll && surfaceView.ChildSurface.Height == surfaceView.Height)
                showHeightScroll = true;

            else if (showHeightScroll && !showWidthScroll && surfaceView.ChildSurface.Width == surfaceView.Width)
                showWidthScroll = true;

            // Show or hide the scrollers
            surfaceView.HorizontalScroller.IsVisible = showWidthScroll;
            surfaceView.VerticalScroller.IsVisible = showHeightScroll;

            // Based on which are shown, they may be different sizes
            // Resize and show them
            if (showWidthScroll && showHeightScroll)
            {
                // Account for the corner between them
                if (surfaceView.HorizontalScroller.Width != surfaceView.Width - 1)
                    surfaceView.HorizontalScroller.Resize(surfaceView.Width - 1, 1);

                if (surfaceView.VerticalScroller.Height != surfaceView.Height - 1)
                    surfaceView.VerticalScroller.Resize(1, surfaceView.Height - 1);

                surfaceView.SurfaceControl.Surface.ViewWidth = surfaceView.Width - 1;
                surfaceView.SurfaceControl.Surface.ViewHeight = surfaceView.Height - 1;
            }
            else if (showWidthScroll)
            {
                if (surfaceView.HorizontalScroller.Width != surfaceView.Width)
                    surfaceView.HorizontalScroller.Resize(surfaceView.Width, 1);

                surfaceView.SurfaceControl.Surface.ViewWidth = surfaceView.Width;
                surfaceView.SurfaceControl.Surface.ViewHeight = surfaceView.Height - 1;
            }
            else if (showHeightScroll)
            {
                if (surfaceView.VerticalScroller.Height != surfaceView.Height)
                    surfaceView.VerticalScroller.Resize(1, surfaceView.Height);

                surfaceView.SurfaceControl.Surface.ViewWidth = surfaceView.Width - 1;
                surfaceView.SurfaceControl.Surface.ViewHeight = surfaceView.Height;
            }

            // Ensure scroll bars positioned correctly
            surfaceView.VerticalScroller.Position = new Point(surfaceView.Width - 1, 0);
            surfaceView.HorizontalScroller.Position = new Point(0, surfaceView.Height -1);
            surfaceView.HorizontalScroller.IsEnabled = false;
            surfaceView.VerticalScroller.IsEnabled = false;

            if (surfaceView.ChildSurface.ViewWidth != surfaceView.ChildSurface.Width)
            {
                surfaceView.HorizontalScroller.Maximum = surfaceView.ChildSurface.Width - surfaceView.ChildSurface.ViewWidth;
                surfaceView.HorizontalScroller.IsEnabled = true;
            }
            if (surfaceView.ChildSurface.ViewHeight != surfaceView.ChildSurface.Height)
            {
                surfaceView.VerticalScroller.Maximum = surfaceView.ChildSurface.Height - surfaceView.ChildSurface.ViewHeight;
                surfaceView.VerticalScroller.IsEnabled = true;
            }

            surfaceView.ChildSurface.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new SurfaceViewerTheme()
        {
            ControlThemeState = ControlThemeState.Clone(),
        };
    }
}
