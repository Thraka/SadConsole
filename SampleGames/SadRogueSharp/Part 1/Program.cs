using Microsoft.Xna.Framework;
using System;
using Console = SadConsole.Console;

namespace SadRogue
{
    /// <summary>
    /// The main class.
    /// </summary>
    class Program
    {
        public const int ScreenWidth = 80;
        public const int ScreenHeight = 25;

        private static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create("IBM.font", ScreenWidth, ScreenHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //
        }

        private static void Update(GameTime time)
        {
            // Called each logic update.

            // As an example, we'll use the F5 key to make the game full screen
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                SadConsole.Settings.ToggleFullScreen();
            }
            else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                SadConsole.Game.Instance.Exit();
            }
        }

        private static void Init()
        {
            SadConsole.Console startingConsole = new Console(ScreenWidth, ScreenHeight);

            startingConsole.Print(ScreenWidth / 2, ScreenHeight / 2, "@", ColorAnsi.CyanBright);

            // Set our new console as the "thing" to render and process
            SadConsole.Global.CurrentScreen = startingConsole;
        }
    }
}