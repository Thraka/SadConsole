//namespace SadConsole
//{
//    using System;
//    using SadConsole;
//    using SadConsole.Consoles;
//    using Console = SadConsole.Consoles.Console;
//    using Microsoft.Xna.Framework;
//    using Microsoft.Xna.Framework.Graphics;

//    public class Game : Microsoft.Xna.Framework.Game
//    {
//        GraphicsDeviceManager _graphics;
//        Console _defaultConsole;

//        string font;
//        int width;
//        int height;

//        public Game(string font, int consoleWidth, int consoleHeight)
//        {
//            _graphics = new GraphicsDeviceManager(this);
//            Content.RootDirectory = "Content";
//            this.font = font;
//            width = consoleWidth;
//            height = consoleHeight;
//        }

//        protected override void Initialize()
//        {
//            // Let the XNA framework show the mouse.
//            IsMouseVisible = true;

//            // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
//            SadConsole.Engine.Initialize(_graphics, font, width, height);

//            // Call the default initialize of the base class.
//            base.Initialize();
//        }


//        SadConsole.Consoles.ITextSurface tempSurface;
//        SadConsole.Consoles.ITextSurface tempSurface2;
//        SadConsole.Consoles.TextSurfaceRenderer tempRenderer;
//        SadConsole.Consoles.Console tempConsole;
//        SadConsole.Consoles.Console tempConsole2;

//        protected override void Update(GameTime gameTime)
//        {
//            // Update the SadConsole engine, handles the mouse, keyboard, and any special effects. You must call this.
//            SadConsole.Engine.Update(gameTime, this.IsActive);
            
//            base.Update(gameTime);
//        }

//        protected override void Draw(GameTime gameTime)
//        {
//            // Clear the screen with black, like a traditional console.
//            GraphicsDevice.Clear(Color.Black);

//            // Draw the consoles to the screen.
//            SadConsole.Engine.Draw(gameTime);

//            base.Draw(gameTime);
//        }
//    }
//}
