using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameFirst
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Some global variables for SadConsole
            SadConsole.Game.Instance.MonoGameInstance = this;
            SadConsole.Host.Global.GraphicsDeviceManager = _graphics;

            // Initialize the SadConsole engine
            SadConsole.Host.Global.SadConsoleComponent = new SadConsole.Host.SadConsoleGameComponent(this);
            Components.Add(SadConsole.Host.Global.SadConsoleComponent);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Do your offscreen drawing (the SadConsole component gets drawn here)
            base.Draw(gameTime);

            // Clear the graphics device
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Render your scene and render SadConsole to something
            _spriteBatch.Begin();
            _spriteBatch.Draw(SadConsole.Host.Global.RenderOutput, new Rectangle(50, 100, SadConsole.Host.Global.RenderOutput.Width / 3, SadConsole.Host.Global.RenderOutput.Height / 3), Color.White);
            _spriteBatch.Draw(SadConsole.Host.Global.RenderOutput, new Rectangle(150, 25, (int)(SadConsole.Host.Global.RenderOutput.Width / 1.5), (int)(SadConsole.Host.Global.RenderOutput.Height / 1.5)), Color.White);
            _spriteBatch.End();
        }
    }
}
