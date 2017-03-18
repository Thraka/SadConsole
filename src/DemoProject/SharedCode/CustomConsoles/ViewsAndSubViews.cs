using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole.Surfaces;
using System;
using Console = SadConsole.Console;
using SadConsole;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class ViewsAndSubViews: ConsoleContainer, IConsoleMetadata
    {
        Console mainView;
        Console subView;

        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Sub Views", Summary = "Single text surface with two views into it. Click on either view." };
            }
        }

        public ViewsAndSubViews()
        {
            mainView = new Console(60, 23);
            subView = new Console(19, 23);

            IsVisible = false;
            UseMouse = true;

            //titleAndLine.Print(0, 0, " View and Sub View".Align(System.Windows.HorizontalAlignment.Left, 80), ColorHelper.GreenYellow, ColorHelper.DarkGreen);
            SadConsole.Shapes.Line line = new SadConsole.Shapes.Line();
            line.UseStartingCell = false;
            line.UseEndingCell = false;
            line.Cell.Glyph = 179;
            line.StartingLocation = new Point(59, 0);
            line.EndingLocation = new Point(59, 22);
            line.Draw(mainView);

            // Setup main view
            mainView.Position = new Point(0, 2);
            mainView.MouseMove += (s, e) => { if (e.MouseState.Mouse.LeftButtonDown) { e.MouseState.Cell.Background = Color.Blue; mainView.TextSurface.IsDirty = true; } };
            ((BasicSurface)mainView.TextSurface).OnIsDirty = (t) => subView.TextSurface.IsDirty = true;

            // Setup sub view
            subView.Position = new Point(60, 2);
            subView.TextSurface = new SurfaceView(mainView.TextSurface, new Rectangle(0, 0, 20, 23));
            subView.MouseMove += (s, e) => { if (e.MouseState.Mouse.LeftButtonDown) { e.MouseState.Cell.Background = Color.Red; subView.TextSurface.IsDirty = true; } };

            // Ad the consoles to the list.
            Children.Add(mainView);
            Children.Add(subView);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            // Process mouse for each console
            var childState = new MouseConsoleState(mainView, state.Mouse);

            if (childState.IsOnConsole)
                return mainView.ProcessMouse(childState);

            childState = new MouseConsoleState(subView, state.Mouse);

            if (childState.IsOnConsole)
                return subView.ProcessMouse(childState);

            return false;
        }
    }
}
