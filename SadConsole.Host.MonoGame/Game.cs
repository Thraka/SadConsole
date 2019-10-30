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
                DefaultFont = LoadFont(_font).GetFont(Font.FontSizes.One);

            MonoGameInstance.ResizeGraphicsDeviceManager(DefaultFont, ScreenCellsX, ScreenCellsY, 0, 0);

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

        public override Renderers.IRenderer GetDefaultRenderer() =>
            new Renderers.ConsoleRenderer();

        /// <summary>
        /// Opens a read-only stream with MonoGame.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="mode">Unused by monogame.</param>
        /// <param name="access">Unused by monogame.</param>
        /// <returns>The stream.</returns>
        public override Stream OpenStream(string file, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read) =>
             Microsoft.Xna.Framework.TitleContainer.OpenStream(file);

        internal void MonoGameLoadEmbeddedFont() =>
            LoadEmbeddedFont();

        /// <summary>
        /// Toggles between windowed and fullscreen rendering for SadConsole.
        /// </summary>
        public void ToggleFullScreen()
        {
            MonoGame.Game.Instance.GraphicsDeviceManager.ApplyChanges();

            // Coming back from fullscreen
            if (MonoGame.Game.Instance.GraphicsDeviceManager.IsFullScreen)
            {
                MonoGame.Game.Instance.GraphicsDeviceManager.IsFullScreen = !MonoGame.Game.Instance.GraphicsDeviceManager.IsFullScreen;

                MonoGame.Game.Instance.GraphicsDeviceManager.PreferredBackBufferWidth = _preFullScreenWidth;
                MonoGame.Game.Instance.GraphicsDeviceManager.PreferredBackBufferHeight = _preFullScreenHeight;
                MonoGame.Game.Instance.GraphicsDeviceManager.ApplyChanges();
            }

            // Going full screen
            else
            {
                _preFullScreenWidth = MonoGame.Game.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                _preFullScreenHeight = MonoGame.Game.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                if (Settings.ResizeMode == Settings.WindowResizeOptions.None)
                {
                    _handleResizeNone = true;
                    Settings.ResizeMode = Settings.WindowResizeOptions.Scale;
                }

                MonoGame.Game.Instance.GraphicsDeviceManager.PreferredBackBufferWidth = MonoGame.Game.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                MonoGame.Game.Instance.GraphicsDeviceManager.PreferredBackBufferHeight = MonoGame.Game.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

                MonoGame.Game.Instance.GraphicsDeviceManager.IsFullScreen = !MonoGame.Game.Instance.GraphicsDeviceManager.IsFullScreen;
                MonoGame.Game.Instance.GraphicsDeviceManager.ApplyChanges();

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
            MonoGame.Game.Instance.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            MonoGame.Game.Instance.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            MonoGame.Game.Instance.GraphicsDeviceManager.ApplyChanges();

            MonoGame.Game.Instance.ResetRendering();
        }

        internal void InvokeFrameDraw() =>
            OnFrameDraw();

        internal void InvokeFrameUpdate() =>
            OnFrameUpdate();
    }
}
