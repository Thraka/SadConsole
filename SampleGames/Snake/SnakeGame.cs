using System;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    
    internal class SnakeGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SnakeConsole gameMenuConsole;

        public SnakeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Let the XNA framework show the mouse.
            IsMouseVisible = true;
            IsFixedTimeStep = true;

            // Initialize the SadConsole engine and the first effects library (provided by the SadConsole.Effects.dll binary)
            SadConsole.Engine.Initialize(GraphicsDevice);

            // Tell SadConsole to track the mouse.
            SadConsole.Engine.UseMouse = true;

            // Load the default font.
            using (var stream = System.IO.File.OpenRead("Fonts/Cheepicus12.font"))
            {
                SadConsole.Engine.DefaultFont = SadConsole.Serializer.Deserialize<Font>(stream);
            }

            // Using the default font, resize the window to a Width,Height of cells. This example uses the MS-DOS default of 80 columns by 25 rows.
            SadConsole.Engine.DefaultFont.ResizeGraphicsDeviceManager(graphics, 50, 60, 0, 0);

            // Create the default console, show the cursor, and let the console accept keyboard input.
            gameMenuConsole = new SnakeConsole();

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

    }


}
