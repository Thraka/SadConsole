using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole.Consoles;
using System;
using Console = SadConsole.Consoles.Console;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class ViewsAndSubViews: SadConsole.Consoles.ConsoleList
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
            line.CellAppearance.GlyphIndex = 179;
            line.StartingLocation = new Point(59, 1);
            line.EndingLocation = new Point(59, 24);
            line.Draw(titleAndLine);

            // Setup main view
            mainView.Position = new Point(0, 1);
            mainView.Print(1, 1, "Click on a cell to draw");
            mainView.MouseMove += (s, e) => { if (e.LeftButtonDown) e.Cell.Background = Color.Blue; };

            // Setup sub view
            subView.Position = new Point(60, 1);
            subView.TextSurface = new TextSurfaceView(mainView.TextSurface, new Rectangle(0, 0, 20, 24));
            subView.MouseMove += (s, e) => { if (e.LeftButtonDown) e.Cell.Background = Color.Red; };

            // Ad the consoles to the list.
            Add(titleAndLine);
            Add(mainView);
            Add(subView);
        }
    }
}
