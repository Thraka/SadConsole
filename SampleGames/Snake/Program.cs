using System;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create("Cheepicus12.font", 50, 60);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Start the game.
            SadConsole.Game.Instance.Run();
        }

        private static void Init()
        {
            var console = new SnakeConsole();
            Global.CurrentScreen.Children.Add(console);
            Global.FocusedConsoles.Set(console);
        }
    }
}
