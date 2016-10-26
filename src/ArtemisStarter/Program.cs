
using System;
using Console = SadConsole.Consoles.Console;
using SadConsole.Consoles;
using Microsoft.Xna.Framework;

namespace SadConsoleArtemis
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Engine.Initialize("IBM.font", 80, 25);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Engine.EngineStart += Engine_EngineStart;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Engine.EngineUpdated += Engine_EngineUpdated;

            // Start the game.
            SadConsole.Engine.Run();
        }

        private static void Engine_EngineStart(object sender, EventArgs e)
        {
            var defaultConsole = (Console)SadConsole.Engine.ActiveConsole;

            defaultConsole.Print(1, 1, "Welcome to SadConsole", Color.Aqua, Color.Black);
        }

        private static void Engine_EngineUpdated(object sender, EventArgs e)
        {

        }
    }
}