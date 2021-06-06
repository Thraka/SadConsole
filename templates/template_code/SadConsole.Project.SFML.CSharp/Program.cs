using System;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace MyGame
{
    class Program
    {
        private static void Main(string[] args)
        {
            var SCREEN_WIDTH = 80;
            var SCREEN_HEIGHT = 25;

            SadConsole.Settings.WindowTitle = "SadConsole Game";
            SadConsole.Settings.UseDefaultExtendedFont = true;

            SadConsole.Game.Create(SCREEN_WIDTH, SCREEN_HEIGHT);
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            // This code uses the default console created for you at start
            var startingConsole = Game.Instance.StartingConsole;

            startingConsole.FillWithRandomGarbage(SadConsole.Game.Instance.StartingConsole.Font);
            startingConsole.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, Mirror.None);
            startingConsole.Print(4, 4, "Hello from SadConsole");

            // --------------------------------------------------------------
            // This code replaces the default starting console with your own.
            // If you use this code, delete the code above.
            // --------------------------------------------------------------
            /*
            var console = new Console(Game.Instance.ScreenCellsX, SadConsole.Game.Instance.ScreenCellsY);
            console.FillWithRandomGarbage(console.Font);
            console.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, 0);
            console.Print(4, 4, "Hello from SadConsole");

            Game.Instance.Screen = console;

            // This is needed because we replaced the initial screen object with our own.
            Game.Instance.DestroyDefaultStartingConsole();
            */
        }
    }
}