using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;
using SadConsole;
using SadRogue.Primitives;

namespace SadConsole.Examples;

internal class DemoScrollableViews : IDemo
{
    public string Title => "UI Scrolling Surface";

    public string Description => "Example of a UI control that can display a surface and scroll it.";

    public string CodeFile => "DemoScrollableViews.cs";

    public IScreenSurface CreateDemoScreen() =>
        new ScrollableView();

    public override string ToString() =>
        Title;
}

internal class ScrollableView: ControlsConsole
{
    CellSurface _sharedSurface;
    Palette mouseColors;
    bool isDrawing;

    public ScrollableView() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        // Create the shared surface
        _sharedSurface = new CellSurface(37, GameSettings.ScreenDemoBounds.Height - 2);
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
        SadConsole.UI.Controls.SurfaceViewer viewer = new SadConsole.UI.Controls.SurfaceViewer(15, 15, _sharedSurface);
        viewer.ScrollBarMode = SadConsole.UI.Controls.SurfaceViewer.ScrollBarModes.AsNeeded;
        viewer.Position = (2, 1);
        viewer.MouseMove += Viewer_MouseMove;
        viewer.HorizontalScroller.Value = 5;
        viewer.VerticalScroller.Value = 5;
        Controls.Add(viewer);

        viewer = new SadConsole.UI.Controls.SurfaceViewer(20, 10);
        viewer.ScrollBarMode = SadConsole.UI.Controls.SurfaceViewer.ScrollBarModes.AsNeeded;
        viewer.Surface = _sharedSurface.GetSubSurface(new Rectangle(0, 0, _sharedSurface.Width, _sharedSurface.Height));
        viewer.Position = (19, 1);
        viewer.MouseMove += Viewer_MouseMove;
        Controls.Add(viewer);

        viewer = new SadConsole.UI.Controls.SurfaceViewer(20, 10);
        viewer.ScrollBarMode = SadConsole.UI.Controls.SurfaceViewer.ScrollBarModes.AsNeeded;
        viewer.Surface = _sharedSurface.GetSubSurface();
        viewer.Position = (19, 12);
        viewer.MouseMove += Viewer_MouseMove;
        Controls.Add(viewer);

        viewer = new SadConsole.UI.Controls.SurfaceViewer(_sharedSurface.Width, _sharedSurface.Height);
        viewer.ScrollBarMode = SadConsole.UI.Controls.SurfaceViewer.ScrollBarModes.AsNeeded;
        viewer.Surface = _sharedSurface.GetSubSurface();
        viewer.Position = (41, 1);
        viewer.MouseMove += Viewer_MouseMove;
        Controls.Add(viewer);

        // Setup mouse palette
        mouseColors = new Palette(new[] { Color.AnsiBlue, Color.AnsiCyan, Color.AnsiGreen, Color.AnsiGreenBright, Color.AnsiRed, Color.AnsiRedBright });

        var button = new SadConsole.UI.Controls.Button(10)
        {
            Text = "move"
        };

        button.Click += (s, e) =>
        {
            ((SadConsole.UI.Controls.SurfaceViewer)Controls[1]).Surface.ViewPosition = (10, 10);
        };
        Controls.Add(button);
    }

    private void Viewer_MouseMove(object? sender, SadConsole.UI.Controls.ControlBase.ControlMouseState e)
    {
        if (e.OriginalMouseState.Mouse.LeftButtonDown)
        {
            if (!isDrawing)
            {
                isDrawing = true;
                mouseColors.ShiftLeft();
            }

            var viewer = (SadConsole.UI.Controls.SurfaceViewer)sender!;
            if (viewer.IsMouseButtonStateClean && viewer.MouseArea.Contains(e.MousePosition))
            {
                _sharedSurface.SetGlyph(e.MousePosition.X + viewer.Surface.ViewPosition.X, e.MousePosition.Y + viewer.Surface.ViewPosition.Y, 0, mouseColors[0], mouseColors[0]);
            }
        }
        else
        {
            if (isDrawing)
                isDrawing = false;
        }
    }
}
