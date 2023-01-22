using Console = SadConsole.Console;
using SadConsole.Input;
using SadConsole;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class SubConsoleCursor : ScreenObject
    {
        Console mainView;
        Console subView;
        
        public SubConsoleCursor()
        {
            mainView = new Console(80, 23);
            subView = new Console(mainView.GetSubSurface(new Rectangle(mainView.Width - 26, mainView.Height - 11, 25, 10)), mainView.Font);
            subView.Cursor.IsEnabled = true;

            UseKeyboard = true;

            // Setup main view
            mainView.FillWithRandomGarbage(mainView.Font);
            mainView.MouseMove += (s, e) => { if (e.Mouse.LeftButtonDown) e.Cell.Background = Color.Blue; };

            // Setup sub view
            subView.Position = new Point(4, 4);
            subView.Surface.DefaultBackground = Color.Black;
            subView.MouseMove += (s, e) => { if (e.Mouse.LeftButtonDown) e.Cell.Background = Color.Red; };
            subView.Surface.IsDirtyChanged += (s, e) => mainView.IsDirty = subView.IsDirty;
            subView.Clear();
            subView.Cursor.IsVisible = true;
            subView.Cursor
                .Print("The left-side box is a small console that uses a small area of the bottom-right of this whole console.")
                .CarriageReturn()
                .LineFeed();


            // Ad the consoles to the list.
            Children.Add(mainView);
            Children.Add(subView);

            IsVisible = false;
        }
        
        public override bool ProcessKeyboard(Keyboard info)
        {
            return subView.ProcessKeyboard(info);
        }
    }
}
