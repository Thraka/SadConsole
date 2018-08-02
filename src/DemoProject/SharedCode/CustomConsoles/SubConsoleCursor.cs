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
    class SubConsoleCursor : SadConsole.ConsoleContainer
    {
        Console mainView;
        Console subView;
        
        public SubConsoleCursor()
        {
            mainView = new Console(80, 23);
            subView = Console.FromSurface(mainView.GetViewSurface(new Rectangle(30, 4, 25, 10)));

            UseKeyboard = true;

            // Setup main view
            mainView.FillWithRandomGarbage();
            mainView.MouseMove += (s, e) => { if (e.MouseState.Mouse.LeftButtonDown) e.MouseState.Cell.Background = Color.Blue; };

            // Setup sub view
            subView.Position = new Point(4, 4);
            subView.DefaultBackground = Color.Black;
            subView.MouseMove += (s, e) => { if (e.MouseState.Mouse.LeftButtonDown) e.MouseState.Cell.Background = Color.Red; };
            subView.DirtyChanged += (s, e) => mainView.IsDirty = subView.IsDirty;
            subView.Clear();
            subView.Cursor.IsVisible = true;
            subView.Cursor
                .Print("The left box is a whole console which is a view into the box on the right.")
                .CarriageReturn()
                .LineFeed();


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
