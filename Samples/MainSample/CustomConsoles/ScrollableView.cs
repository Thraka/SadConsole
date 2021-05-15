using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;
using SadConsole;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class ScrollableView: ControlsConsole
    {
        CellSurface _sharedSurface;
        Palette mouseColors;
        bool isDrawing;

        public ScrollableView(): base (Program.MainWidth, Program.MainHeight)
        {
            // Create the shared surface
            _sharedSurface = new CellSurface(37, Program.MainHeight - 2);
            _sharedSurface.FillWithRandomGarbage(Font);

            // Create a 012345... border around the surface
            int counter = 0;
            for (int i = 0; i < Surface.Width; i++)
            {
                _sharedSurface.SetGlyph(i, 0, counter + 48, Color.Green, Color.Black, Mirror.None);
                _sharedSurface.SetGlyph(i, _sharedSurface.Height - 1, counter + 48, Color.Green, Color.Black, Mirror.None);

                counter++;

                if (counter == 10)
                    counter = 0;
            }

            counter = 0;
            for (int i = 0; i < Surface.Height; i++)
            {
                _sharedSurface.SetGlyph(0, i, counter + 48, Color.Green, Color.Black, Mirror.None);
                _sharedSurface.SetGlyph(_sharedSurface.Width - 1, i, counter + 48, Color.Green, Color.Black, Mirror.None);

                counter++;

                if (counter == 10)
                    counter = 0;
            }

            // Create viewer controls and attach them to the surface
            SadConsole.UI.Controls.SurfaceViewer viewer = new SadConsole.UI.Controls.SurfaceViewer(15, 15);
            viewer.ScrollBarMode = SadConsole.UI.Controls.SurfaceViewer.ScrollBarModes.AsNeeded;
            viewer.SetSurface(_sharedSurface);
            viewer.Position = (2, 1);
            viewer.MouseMove += Viewer_MouseMove;
            Controls.Add(viewer);

            viewer = new SadConsole.UI.Controls.SurfaceViewer(20, 10);
            viewer.ScrollBarMode = SadConsole.UI.Controls.SurfaceViewer.ScrollBarModes.AsNeeded;
            viewer.SetSurface(_sharedSurface);
            viewer.Position = (19, 1);
            viewer.MouseMove += Viewer_MouseMove;
            Controls.Add(viewer);

            viewer = new SadConsole.UI.Controls.SurfaceViewer(20, 10);
            viewer.ScrollBarMode = SadConsole.UI.Controls.SurfaceViewer.ScrollBarModes.AsNeeded;
            viewer.SetSurface(_sharedSurface);
            viewer.Position = (19, 12);
            viewer.MouseMove += Viewer_MouseMove;
            Controls.Add(viewer);

            viewer = new SadConsole.UI.Controls.SurfaceViewer(_sharedSurface.Width, _sharedSurface.Height);
            viewer.ScrollBarMode = SadConsole.UI.Controls.SurfaceViewer.ScrollBarModes.AsNeeded;
            viewer.SetSurface(_sharedSurface);
            viewer.Position = (41, 1);
            viewer.MouseMove += Viewer_MouseMove;
            Controls.Add(viewer);

            // Setup mouse palette
            mouseColors = new Palette(new[] { Color.AnsiBlue, Color.AnsiCyan, Color.AnsiGreen, Color.AnsiGreenBright, Color.AnsiRed, Color.AnsiRedBright });
        }

        private void Viewer_MouseMove(object sender, SadConsole.UI.Controls.ControlBase.ControlMouseState e)
        {
            if (e.OriginalMouseState.Mouse.LeftButtonDown)
            {
                if (!isDrawing)
                {
                    isDrawing = true;
                    mouseColors.ShiftLeft();
                }

                var viewer = (SadConsole.UI.Controls.SurfaceViewer)sender;
                if (viewer.IsMouseButtonStateClean && viewer.SurfaceControl.MouseArea.Contains(e.MousePosition))
                {
                    _sharedSurface.SetGlyph(e.MousePosition.X + viewer.ChildSurface.ViewPosition.X, e.MousePosition.Y + viewer.ChildSurface.ViewPosition.Y, 0, mouseColors[0], mouseColors[0]);
                }
            }
            else
            {
                if (isDrawing)
                    isDrawing = false;
            }
        }
    }
}
