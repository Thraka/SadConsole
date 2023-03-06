using System;
using System.IO;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// The MonoGame implementation of the SadConsole Game Host.
    /// </summary>
    public partial class Game : GameHost
    {
        private Host.Mouse _mouseState = new Host.Mouse();
        private Host.Keyboard _keyboardState = new Host.Keyboard();

        /// <summary>
        /// When <see langword="true"/>, forces the <see cref="OpenStream"/> method to use <code>TitalContainer</code> when creating a stream to read a file.
        /// </summary>
        public bool UseTitleContainer { get; set; } = true;

        /// <summary>
        /// The <see cref="Microsoft.Xna.Framework.Game"/> instance.
        /// </summary>
        public Host.Game MonoGameInstance { get; set; }

        /// <summary>
        /// Strongly typed version of <see cref="GameHost.Instance"/>.
        /// </summary>
        public new static Game Instance
        {
            get => (Game)GameHost.Instance;
            set => GameHost.Instance = value;
        }

        /// <summary>
        /// Method called by the <see cref="Host.Game"/> class for initializing SadConsole specifics. Called prior to <see cref="Host.Game.ResetRendering"/>.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public void MonoGameInit(int pixelWidth, int pixelHeight, string font)
        {
            LoadDefaultFonts(font);

            SadConsole.Settings.Rendering.RenderWidth = pixelWidth;
            SadConsole.Settings.Rendering.RenderHeight = pixelHeight;

            SetRenderer(Renderers.Constants.RendererNames.Default, typeof(Renderers.ScreenSurfaceRenderer));

            SetRendererStep(Renderers.Constants.RenderStepNames.ControlHost, typeof(Renderers.ControlHostRenderStep));
            SetRendererStep(Renderers.Constants.RenderStepNames.Cursor, typeof(Renderers.CursorRenderStep));
            SetRendererStep(Renderers.Constants.RenderStepNames.EntityRenderer, typeof(Renderers.EntityLiteRenderStep));
            SetRendererStep(Renderers.Constants.RenderStepNames.Output, typeof(Renderers.OutputSurfaceRenderStep));
            SetRendererStep(Renderers.Constants.RenderStepNames.SurfaceDirtyCells, typeof(Renderers.SurfaceDirtyCellsRenderStep));
            SetRendererStep(Renderers.Constants.RenderStepNames.Surface, typeof(Renderers.SurfaceRenderStep));
            SetRendererStep(Renderers.Constants.RenderStepNames.Tint, typeof(Renderers.TintSurfaceRenderStep));
            SetRendererStep(Renderers.Constants.RenderStepNames.Window, typeof(Renderers.WindowRenderStep));

            LoadMappedColors();

            var fontSize = DefaultFont.GetFontSize(DefaultFontSize);

            if (fontSize.X > pixelWidth || fontSize.Y > pixelHeight) throw new Exception("WPF control is too small for the font.");

            if (Settings.CreateStartingConsole)
            {
                StartingConsole = new Console(pixelWidth / fontSize.X, pixelHeight / fontSize.Y);
                //StartingConsole = new Console(ScreenCellsX, ScreenCellsY);
                StartingConsole.IsFocused = true;
                Screen = StartingConsole;
            }
            else
                Screen = new ScreenObject() { IsFocused = true };



            OnStart?.Invoke();
            SplashScreens.SplashScreenManager.CheckRun();
        }

        /// <inheritdoc/>
        public override ITexture GetTexture(string resourcePath) =>
            new Host.GameTexture(resourcePath);

        /// <inheritdoc/>
        public override ITexture GetTexture(Stream textureStream) =>
            new Host.GameTexture(textureStream);

        /// <inheritdoc/>
        public override SadConsole.Input.IKeyboardState GetKeyboardState()
        {
            _keyboardState.Refresh();
            return _keyboardState;
        }

        /// <inheritdoc/>
        public override SadConsole.Input.IMouseState GetMouseState()
        {
            _mouseState.Refresh();
            return _mouseState;
        }


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

            return UseTitleContainer ? Microsoft.Xna.Framework.TitleContainer.OpenStream(file) : File.OpenRead(file);
        }


        public override void Run()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void ResizeWindow(int width, int height)
        {
            throw new NotImplementedException();
        }

        internal void InvokeFrameDraw() =>
            OnFrameRender();

        internal void InvokeFrameUpdate() =>
            OnFrameUpdate();
    }
}
