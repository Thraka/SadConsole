#if MONOGAME
using Microsoft.Xna.Framework;

namespace SadConsole
{
    /// <summary>
    /// A MonoGame <see cref="Game"/> instance that runs SadConsole. This is used when you don't provide one and call <see cref="Engine.Initialize(string, int, int)"/>.
    /// </summary>
    public class SadConsoleGame : Game
    {
        private string font;
        private int consoleWidth;
        private int consoleHeight;
        public GraphicsDeviceManager GraphicsDeviceManager;

        internal SadConsoleGame(string font, int consoleWidth, int consoleHeight)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.font = font;
            this.consoleHeight = consoleHeight;
            this.consoleWidth = consoleWidth;
        }

        protected override void Initialize()
        {
            if (Engine.UnlimitedFPS)
            {
                GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }

            // Let the XNA framework show the mouse.
            IsMouseVisible = true;

            // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
            Components.Add(new EngineGameComponent(this, GraphicsDeviceManager, font, consoleWidth, consoleHeight, () => { }));

            // Call the default initialize of the base class.
            base.Initialize();

            Engine.InitializeCompleted();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Engine.ClearFrameColor);

            base.Draw(gameTime);
        }
    }
}
#endif