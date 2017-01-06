#if MONOGAME
using Microsoft.Xna.Framework;
using System;

namespace SadConsole
{
    /// <summary>
    /// A MonoGame <see cref="Game"/> instance that runs SadConsole. This is used when you don't provide one and call <see cref="Engine.Initialize(string, int, int)"/>.
    /// </summary>
    public class SadConsoleGame : Game
    {
        private bool resizeBusy = false;
        private string font;
        private int consoleWidth;
        private int consoleHeight;
        public GraphicsDeviceManager GraphicsDeviceManager;

        /// <summary>
        /// The type of resizing options for the window.
        /// </summary>
        public WindowResizeOptions DisplayOptions;

        

        internal SadConsoleGame(string font, int consoleWidth, int consoleHeight, Action<SadConsoleGame> ctorCallback = null)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.font = font;
            this.consoleHeight = consoleHeight;
            this.consoleWidth = consoleWidth;
            GraphicsDeviceManager.HardwareModeSwitch = false;

            ctorCallback?.Invoke(this);


            DisplayOptions = WindowResizeOptions.Scale;
        }


        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (!resizeBusy)
            {
                if (DisplayOptions != WindowResizeOptions.Stretch)
                {
                    GraphicsDeviceManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
                    GraphicsDeviceManager.PreferredBackBufferHeight = Window.ClientBounds.Height;

                    resizeBusy = true;
                    GraphicsDeviceManager.ApplyChanges();
                    resizeBusy = false;
                }
            }

            Engine.ResetRendering();
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

            // Hook window change for resolution fixes
            Window.ClientSizeChanged += Window_ClientSizeChanged;

            // Tell the main engine we're ready
            Engine.InitializeCompleted();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Engine.ClearFrameColor);

            base.Draw(gameTime);
        }

        public enum WindowResizeOptions
        {
            Stretch,
            Center,
            Scale,
        }
    }
}
#endif