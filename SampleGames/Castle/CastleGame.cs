using System;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Castle
{
    internal class CastleGame : Game
    {
        private const int gameSpeedMs = 90;
        private TimeSpan gameSpeed;
        private TimeSpan lastGameUpdate;

        private GraphicsDeviceManager graphics;
        private GameMenuConsole gameMenuConsole;
        private CastleConsole castleConsole;
        private GameScoreConsole gameScoreConsole;

        public CastleGame()
        {

            this.Window.Title = "Castle Adventure";
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            gameSpeed = new TimeSpan(0, 0, 0, 0, gameSpeedMs);
            lastGameUpdate = new TimeSpan(0, 0, 0, 0, 0);

        }

        protected override void Initialize()
        {
            // Let the XNA framework show the mouse.
            IsMouseVisible = true;
            IsFixedTimeStep = true;

            // Initialize the SadConsole engine and the first effects library (provided by the SadConsole.Effects.dll binary)
            SadConsole.Engine.Initialize(graphics, "Cheepicus12.font", 40, 25);

            // Tell SadConsole to track the mouse.
            SadConsole.Engine.UseMouse = true;

            // Create the default console, show the cursor, and let the console accept keyboard input.
            gameMenuConsole = new GameMenuConsole();
            gameMenuConsole.PlayGame += StartGame;
            
            // Add the default console to the list of consoles.
            SadConsole.Engine.ConsoleRenderStack = new ConsoleList() { 
                                                                        gameMenuConsole
                                                                     };
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = true;

            // Set the first console in the console list as the "active" console. This allows the keyboard to be processed on the console.
            SadConsole.Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[0];

            // Call the default initialize of the base class.
            base.Initialize();
        }


        protected override void Update(GameTime gameTime)
        {

            lastGameUpdate = lastGameUpdate.Add(gameTime.ElapsedGameTime);
            if (lastGameUpdate > gameSpeed)
            {
                lastGameUpdate = lastGameUpdate.Subtract(gameSpeed);
                if (castleConsole != null)
                {
                    castleConsole.RunTick = true;
                }
                
            }
            SadConsole.Engine.Update(gameTime, this.IsActive);
            
            
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen with black, like a traditional console.
            GraphicsDevice.Clear(Color.Black);

            // Draw the consoles to the screen.
            SadConsole.Engine.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void StartGame(object sender, EventArgs args)
        {
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = false;
            SadConsole.Engine.ConsoleRenderStack.Remove(gameMenuConsole);

            castleConsole = new CastleConsole();
            castleConsole.StopGamePlay += ProcessGameOver;

            SadConsole.Engine.ConsoleRenderStack.Add(castleConsole);
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = true;
            SadConsole.Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[0];
            

        }

        private void ProcessGameOver(object sender, EventArgs args)
        {
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = false;
            SadConsole.Engine.ConsoleRenderStack.Remove(castleConsole);

            gameScoreConsole = new GameScoreConsole(castleConsole);
            gameScoreConsole.RestartGame += RestartGame;
            gameScoreConsole.QuitGame += QuitGame;

            SadConsole.Engine.ConsoleRenderStack.Add(gameScoreConsole);
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = true;

            SadConsole.Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[0];
        }

        private void RestartGame(object sender, EventArgs args)
        {
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = false;
            SadConsole.Engine.ConsoleRenderStack.Remove(gameScoreConsole);

            castleConsole = new CastleConsole();
            castleConsole.StopGamePlay += ProcessGameOver;

            SadConsole.Engine.ConsoleRenderStack.Add(castleConsole);
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = true;
            SadConsole.Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[0];


        }
        private void QuitGame(object sender, EventArgs args)
        {
            this.Exit();
        }
        

    }
}
