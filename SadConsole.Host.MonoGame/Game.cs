using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;

namespace SadConsole
{
    public class Game : GameHost
    {
        protected int _preFullScreenWidth;
        protected int _preFullScreenHeight;
        protected bool _handleResizeNone;

        /// <summary>
        /// The <see cref="Microsoft.Xna.Framework.Game"/> instance.
        /// </summary>
        public MonoGame.Game MonoGameInstance { get; protected set; }

        /// <summary>
        /// Strongly typed version of <see cref="GameHost.Instance"/>.
        /// </summary>
        public new static Game Instance
        {
            get => (Game)GameHost.Instance;
            protected set => GameHost.Instance = value;
        }

        internal string _font;

        protected Game() { }

        /// <summary>
        /// Creates a new game and assigns it to the <see cref="MonoGameInstance"/> property.
        /// </summary>
        /// <param name="cellCountX"></param>
        /// <param name="cellCountY"></param>
        /// <param name="font"></param>
        /// <param name="monogameCtorCallback"></param>
        public static void Create(int cellCountX, int cellCountY, string font = "", Action<MonoGame.Game> monogameCtorCallback = null)
        {
            var game = new Game();
            game.ScreenCellsX = cellCountX;
            game.ScreenCellsY = cellCountY;
            game._font = font;

            Instance = game;
            game.MonoGameInstance = new MonoGame.Game(monogameCtorCallback, game.MonoGameInit);
        }

        /// <summary>
        /// Method called by the <see cref="MonoGame.Game"/> class for initializing SadConsole specifics. Called prior to <see cref="MonoGame.Game.ResetRendering"/>.
        /// </summary>
        /// <param name="game">The game instance.</param>
        protected void MonoGameInit(MonoGame.Game game)
        {
            LoadEmbeddedFont();

            if (string.IsNullOrEmpty(_font))
                if (Settings.UseDefaultExtendedFont)
                    DefaultFont = EmbeddedFontExtended;
                else
                    DefaultFont = EmbeddedFont;
            else
                DefaultFont = LoadFont(_font);

            MonoGameInstance.ResizeGraphicsDeviceManager(DefaultFont.GetFontSize(DefaultFontSize).ToMonoPoint(), ScreenCellsX, ScreenCellsY, 0, 0);

            SadConsole.GameHost.Instance.Screen = new Console(ScreenCellsX, ScreenCellsY);

            OnStart?.Invoke();
        }

        /// <inheritdoc/>
        public override void Run()
        {
            MonoGameInstance.Run();
            OnEnd?.Invoke();
            MonoGameInstance.Dispose();
        }

        /// <inheritdoc/>
        public override ITexture GetTexture(string resourcePath) =>
            new Host.GameTexture(resourcePath);

        /// <inheritdoc/>
        public override ITexture GetTexture(Stream textureStream) =>
            new Host.GameTexture(textureStream);

        /// <inheritdoc/>
        public override Renderers.IRenderer GetRenderer(string name) =>
            name switch
            {
                "window" => new Renderers.Window(),
                "controls" => new Renderers.ControlsConsole(),
                "layered" => new Renderers.LayeredScreenObject(),
                _ => new Renderers.ScreenObjectRenderer(),
            };


        /// <inheritdoc/>
        public override Renderers.IRenderer GetDefaultRenderer(IScreenSurface screenObject) =>
            screenObject switch
            {
                UI.Window _ => new Renderers.Window(),
                LayeredScreenSurface _ => new Renderers.LayeredScreenObject(),
                _ => new Renderers.ScreenObjectRenderer(),
            };

        /// <inheritdoc/>
        public override SadConsole.Input.IKeyboardState GetKeyboardState() =>
            new Host.Keyboard();

        /// <inheritdoc/>
        public override SadConsole.Input.IMouseState GetMouseState() =>
            new Host.Mouse();


        /// <summary>
        /// Opens a read-only stream with MonoGame.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="mode">Unused by monogame.</param>
        /// <param name="access">Unused by monogame.</param>
        /// <returns>The stream.</returns>
        public override Stream OpenStream(string file, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
        {
            if (mode == FileMode.Create || mode == FileMode.CreateNew || mode == FileMode.OpenOrCreate)
                return System.IO.File.OpenWrite(file);
            
            return Microsoft.Xna.Framework.TitleContainer.OpenStream(file);
        }

        /// <summary>
        /// Toggles between windowed and fullscreen rendering for SadConsole.
        /// </summary>
        public void ToggleFullScreen()
        {
            MonoGame.Global.GraphicsDeviceManager.ApplyChanges();

            // Coming back from fullscreen
            if (MonoGame.Global.GraphicsDeviceManager.IsFullScreen)
            {
                MonoGame.Global.GraphicsDeviceManager.IsFullScreen = !MonoGame.Global.GraphicsDeviceManager.IsFullScreen;

                MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferWidth = _preFullScreenWidth;
                MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferHeight = _preFullScreenHeight;
                MonoGame.Global.GraphicsDeviceManager.ApplyChanges();
            }

            // Going full screen
            else
            {
                _preFullScreenWidth = MonoGame.Global.GraphicsDevice.PresentationParameters.BackBufferWidth;
                _preFullScreenHeight = MonoGame.Global.GraphicsDevice.PresentationParameters.BackBufferHeight;

                if (Settings.ResizeMode == Settings.WindowResizeOptions.None)
                {
                    _handleResizeNone = true;
                    Settings.ResizeMode = Settings.WindowResizeOptions.Scale;
                }

                MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferWidth = MonoGame.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferHeight = MonoGame.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

                MonoGame.Global.GraphicsDeviceManager.IsFullScreen = !MonoGame.Global.GraphicsDeviceManager.IsFullScreen;
                MonoGame.Global.GraphicsDeviceManager.ApplyChanges();

                if (_handleResizeNone)
                {
                    _handleResizeNone = false;
                    Settings.ResizeMode = Settings.WindowResizeOptions.None;
                }
            }
        }

        /// <summary>
        /// Resizes the game window.
        /// </summary>
        /// <param name="width">The width of the window in pixels.</param>
        /// <param name="height">The height of the window in pixels.</param>
        public void ResizeWindow(int width, int height)
        {
            MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            MonoGame.Global.GraphicsDeviceManager.ApplyChanges();

            ((Game)SadConsole.Game.Instance).MonoGameInstance.ResetRendering();
        }

        internal void InvokeFrameDraw() =>
            OnFrameDraw();

        internal void InvokeFrameUpdate() =>
            OnFrameUpdate();
    }
}
