using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole.Surfaces;
using System;
using Console = SadConsole.Console;
using SadConsole.Input;
using SadConsole;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class SubConsoleCursor : SadConsole.ConsoleContainer, IConsoleMetadata
    {
        Console mainView;
        Console subView;

        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Subconsole Cursor", Summary = "Two consoles with a single backing TextSurface" };
            }
        }

        public SubConsoleCursor()
        {
            mainView = new Console(80, 23);
            subView = new Console(new SurfaceView(mainView.TextSurface, new Rectangle(4, 4, 25, 10)));

            UseKeyboard = true;

            // Setup main view
            mainView.FillWithRandomGarbage();
            mainView.MouseMove += (s, e) => { if (e.MouseState.Mouse.LeftButtonDown) e.MouseState.Cell.Background = Color.Blue; };

            // Setup sub view
            subView.Position = new Point(4, 4);
            subView.MouseMove += (s, e) => { if (e.MouseState.Mouse.LeftButtonDown) e.MouseState.Cell.Background = Color.Red; };
            subView.Clear();
            subView.VirtualCursor.IsVisible = true;

            // Ad the consoles to the list.
            Children.Add(mainView);
            Children.Add(subView);
        }
        
        public override bool ProcessKeyboard(Keyboard info)
        {
            return subView.ProcessKeyboard(info);
        }

        //public override void Update(TimeSpan delta)
        //{
        //    if (IsVisible)
        //    {
        //        if (mainView.Position != Position)
        //        {
        //            mainView.Position = Position;
        //            subView.Position = mainView.Position + ((SurfaceView)subView.TextSurface).ViewArea.Location;
        //        }

        //        foreach (var child in Children)
        //            child.Update(delta);
        //    }
        //}

    }
}
