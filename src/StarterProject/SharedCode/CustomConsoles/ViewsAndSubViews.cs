using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole.Surfaces;
using System;
using Console = SadConsole.Console;
using SadConsole;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class ViewsAndSubViews: ConsoleContainer
    {
        Console mainView;
        Console subView;
        Console titleAndLine;

        public ViewsAndSubViews()
        {
            titleAndLine = new Console(80, 25);
            mainView = new Console(59, 24);
            subView = new Console(19, 24);

            IsVisible = false;

            // Draw border and line stuff
            titleAndLine.Print(0, 0, " View and Sub View".Align(System.Windows.HorizontalAlignment.Left, 80), ColorHelper.GreenYellow, ColorHelper.DarkGreen);
            SadConsole.Shapes.Line line = new SadConsole.Shapes.Line();
            line.UseStartingCell = false;
            line.UseEndingCell = false;
            line.Cell.Glyph = 179;
            line.StartingLocation = new Point(59, 1);
            line.EndingLocation = new Point(59, 24);
            line.Draw(titleAndLine);

            // Setup main view
            mainView.Position = new Point(0, 1);
            mainView.Print(1, 1, "Click on a cell to draw");
            mainView.MouseMove += (s, e) => { if (e.LeftButtonDown) { e.Cell.Background = Color.Blue; mainView.TextSurface.IsDirty = true; } };
            ((BasicSurface)mainView.TextSurface).OnIsDirty = (t) => subView.TextSurface.IsDirty = true;

            // Setup sub view
            subView.Position = new Point(60, 1);
            subView.TextSurface = new SurfaceView(mainView.TextSurface, new Rectangle(0, 0, 20, 24));
            subView.MouseMove += (s, e) => { if (e.LeftButtonDown) { e.Cell.Background = Color.Red; subView.TextSurface.IsDirty = true; } };

            // Ad the consoles to the list.
            Children.Add(titleAndLine);
            Children.Add(mainView);
            Children.Add(subView);
        }

        public override void Update(TimeSpan delta)
        {
            if (isVisible)
                base.Update(delta);
        }
    }
}
