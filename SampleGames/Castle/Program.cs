using System;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace Castle
{
    class Program
    {
        private const int gameSpeedMs = 90;
        private static TimeSpan gameSpeed;
        private static TimeSpan lastGameUpdate;

        private static GraphicsDeviceManager graphics;
        private static GameMenuConsole gameMenuConsole;
        private static CastleConsole castleConsole;
        private static GameScoreConsole gameScoreConsole;

        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create("Cheepicus12.font", 40, 25);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // We need to get into the update loop
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();
        }

        private static void Init()
        {
            // Setup window
            SadConsole.Game.Instance.Window.Title = "Castle Adventure - Powered by SadConsole";

            // Setup variables
            gameSpeed = new TimeSpan(0, 0, 0, 0, gameSpeedMs);
            lastGameUpdate = new TimeSpan(0, 0, 0, 0, 0);

            // Create consoles
            gameMenuConsole = new GameMenuConsole();
            gameMenuConsole.PlayGame += StartGame;

            SadConsole.Global.CurrentScreen = gameMenuConsole;
            SadConsole.Global.FocusedConsoles.Set(gameMenuConsole);
        }

        private static void Update(GameTime delta)
        {
            lastGameUpdate = lastGameUpdate.Add(delta.ElapsedGameTime);
            if (lastGameUpdate > gameSpeed)
            {
                lastGameUpdate = lastGameUpdate.Subtract(gameSpeed);
                if (castleConsole != null)
                {
                    castleConsole.RunTick = true;
                }
                
            }
        }

        private static void StartGame(object sender, EventArgs args)
        {
            castleConsole = new CastleConsole();
            castleConsole.StopGamePlay += ProcessGameOver;

            SadConsole.Global.CurrentScreen = castleConsole;
            SadConsole.Global.FocusedConsoles.Set(castleConsole);
        }

        private static void ProcessGameOver(object sender, EventArgs args)
        {
            gameScoreConsole = new GameScoreConsole(castleConsole);
            gameScoreConsole.RestartGame += RestartGame;
            gameScoreConsole.QuitGame += QuitGame;

            SadConsole.Global.CurrentScreen = gameScoreConsole;
            SadConsole.Global.FocusedConsoles.Set(gameScoreConsole);
        }

        private static void RestartGame(object sender, EventArgs args)
        {
            castleConsole = new CastleConsole();
            castleConsole.StopGamePlay += ProcessGameOver;

            SadConsole.Global.CurrentScreen = castleConsole;
            SadConsole.Global.FocusedConsoles.Set(castleConsole);

        }
        private static void QuitGame(object sender, EventArgs args)
        {
            SadConsole.Game.Instance.Exit();
        }


    }
}
