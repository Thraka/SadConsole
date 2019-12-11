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
        private int _preFullScreenWidth;
        private int _preFullScreenHeight;
        private bool _handleResizeNone;

        /// <summary>
        /// The <see cref="Microsoft.Xna.Framework.Game"/> instance.
        /// </summary>
        public MonoGame.Game MonoGameInstance { get; private set; }

        public new static Game Instance
        {
            get => (Game)GameHost.Instance;
            protected set => GameHost.Instance = value;
        }

        internal string _font;


        private Game() { }

        public static void Create(int cellCountX, int cellCountY, string font = "", Action<MonoGame.Game> monogameCtorCallback = null)
        {
            var game = new Game();
            game.ScreenCellsX = cellCountX;
            game.ScreenCellsY = cellCountY;
            game._font = font;

            Instance = game;
            game.MonoGameInstance = new MonoGame.Game(monogameCtorCallback, game.MonoGameInit);
        }

        private void MonoGameInit(MonoGame.Game game)
        {
            if (string.IsNullOrEmpty(_font))
                LoadEmbeddedFont();
            else
                Global.DefaultFont = LoadFont(_font);

            MonoGameInstance.ResizeGraphicsDeviceManager(Global.DefaultFont.GetFontSize(Global.DefaultFontSize).ToMonoPoint(), ScreenCellsX, ScreenCellsY, 0, 0);

            SadConsole.Global.Screen = new Console(ScreenCellsX, ScreenCellsY);

            OnStart?.Invoke();
        }

        public override void Run()
        {
            MonoGameInstance.Run();
            OnEnd?.Invoke();
            MonoGameInstance.Dispose();
        }

        public override ITexture GetTexture(string resourcePath) =>
            new MonoGame.GameTexture(resourcePath);

        public override ITexture GetTexture(Stream textureStream) =>
            new MonoGame.GameTexture(textureStream);

        public override Renderers.IRenderer GetDefaultRenderer(IScreenSurface screenObject) =>
            screenObject switch
            {
                UI.Window _ => new Renderers.Window(),
                UI.ControlsConsole _ => new Renderers.ControlsConsole(),
                Console _ => new Renderers.ConsoleRenderer(),
                _ => new Renderers.ScreenObjectRenderer(),
            };
            

        public override SadConsole.Input.IKeyboardState GetKeyboardState() =>
            new SadConsole.MonoGame.Keyboard();

        public override SadConsole.Input.IMouseState GetMouseState() =>
            new SadConsole.MonoGame.Mouse();


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

        internal void MonoGameLoadEmbeddedFont() =>
            LoadEmbeddedFont();

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
