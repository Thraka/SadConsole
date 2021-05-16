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
            var console = new Console(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY);
            console.FillWithRandomGarbage(console.Font);
            console.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, 0);
            console.Print(4, 4, "Hello from SadConsole");

            Game.Instance.Screen = console;

            // This is needed because we replaced the initial screen object with our own.
            Game.Instance.DestroyDefaultStartingConsole();
        }
    }
}