using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole.Surfaces;
using System;
using Console = SadConsole.Console;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class SubConsoleCursor : SadConsole.Console
    {
        Console mainView;
        Console subView;

        public SubConsoleCursor(): base(1,1)
        {
            mainView = new Console(80, 25);
            subView = new Console(25, 10);

            IsVisible = false;
            
            // Setup main view
            mainView.FillWithRandomGarbage();
            mainView.MouseMove += (s, e) => { if (e.LeftButtonDown) e.Cell.Background = Color.Blue; };

            // Setup sub view
            subView.Position = new Point(4, 4);
            subView.TextSurface = new SurfaceView(mainView.TextSurface, new Rectangle(4, 4, 25, 10));
            subView.MouseMove += (s, e) => { if (e.LeftButtonDown) e.Cell.Background = Color.Red; };
            subView.Clear();
            subView.VirtualCursor.IsVisible = true;

            // Ad the consoles to the list.
            Children.Add(mainView);
            Children.Add(subView);
        }
        
        public override void Draw(TimeSpan elapsed)
        {
            if (isVisible)
            {
                mainView.Draw(elapsed);
                SadConsole.Global.DrawCalls.Add(new SadConsole.DrawCallCursor(subView));
            }
        }

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            return subView.ProcessKeyboard(info);
        }
    }
}
