using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole.Consoles;
using System;
using Console = SadConsole.Consoles.Console;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class SubConsoleCursor : SadConsole.Consoles.ConsoleList
    {
        Console mainView;
        Console subView;

        public SubConsoleCursor()
        {
            mainView = new Console(80, 25);
            subView = new Console(25, 10);

            IsVisible = false;
            
            // Setup main view
            mainView.FillWithRandomGarbage();
            mainView.MouseMove += (s, e) => { if (e.LeftButtonDown) e.Cell.Background = Color.Blue; };

            // Setup sub view
            subView.Position = new Point(4, 4);
            subView.TextSurface = new TextSurfaceView(mainView.TextSurface, new Rectangle(4, 4, 25, 10));
            subView.MouseMove += (s, e) => { if (e.LeftButtonDown) e.Cell.Background = Color.Red; };
            subView.Clear();
            
            // Ad the consoles to the list.
            Add(mainView);
            //Add(subView);
        }

        public override void Update()
        {
            subView.Update();
        }

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            return subView.ProcessKeyboard(info);
        }
    }
}
