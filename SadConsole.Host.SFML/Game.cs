using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SFML.Graphics;
using SadRogue.Primitives;
using SadConsole.Host;

namespace SadConsole
{
    /// <summary>
    /// The SadConsole game object.
    /// </summary>
    public class Game : GameHost
    {
        /// <summary>
        /// The keyboard translation object.
        /// </summary>
        protected Keyboard _keyboard;

        /// <summary>
        /// The mouse translation object.
        /// </summary>
        protected Mouse _mouse;

        /// <summary>
        /// Static instance to the game after the <see cref="Create(int, int, string, RenderWindow)"/> method has been called.
        /// </summary>
        public new static Game Instance
        {
            get => (Game)GameHost.Instance;
            protected set => GameHost.Instance = value;
        }

        internal string _font;

        /// <summary>
        /// Creates the game instance.
        /// </summary>
        protected Game() { }

        /// <summary>
        /// Create's a new SadConsole game.
        /// </summary>
        /// <param name="cellCountX">How many cells wide the window should be based on the font used.</param>
        /// <param name="cellCountY">How many cells high the window should be based on the font used.</param>
        /// <param name="font">An optional font; otherwise a default 8x16 IBM font is used.</param>
        /// <param name="window">A optional window object; otherwise the window is created for you.</param>
        public static void Create(int cellCountX, int cellCountY, string font = "", RenderWindow window = null)
        {
            var game = new Game();
            game.ScreenCellsX = cellCountX;
            game.ScreenCellsY = cellCountY;
            game._font = font;

            Instance = game;
            game.Initialize(window);
        }

        /// <summary>
        /// Initializes SadConsole and sets up the window events. If the <paramref name="window"/> is <see langword="null"/>, a new window is created based on the <see cref="GameHost.ScreenCellsX"/>, <see cref="GameHost.ScreenCellsY"/>, and the <see cref="GameHost.DefaultFontSize"/>.
        /// </summary>
        /// <param name="window"></param>
        protected void Initialize(RenderWindow window)
        {
            LoadEmbeddedFont();

            if (string.IsNullOrEmpty(_font))
                if (Settings.UseDefaultExtendedFont)
                    DefaultFont = EmbeddedFontExtended;
                else
                    DefaultFont = EmbeddedFont;
            else
                DefaultFont = LoadFont(_font);

            if (window == null)
            {
                window = new RenderWindow(new SFML.Window.VideoMode((uint)(DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize).X * ScreenCellsX), (uint)(DefaultFont.GetFontSize(DefaultFontSize).Y * ScreenCellsY)), Host.Settings.WindowTitle, SFML.Window.Styles.Titlebar | SFML.Window.Styles.Close | SFML.Window.Styles.Resize);

                // SETUP RENDER vars for global screen size data.
            }

            WindowSize = new Point((int)window.Size.X, (int)window.Size.Y);

            window.Closed += (o, e) =>
            {
                ((SFML.Window.Window)o).Close();
            };

            window.Resized += (o, e) =>
            {
                ResetRendering();
            };

            if (Host.Settings.FPS != 0)
                window.SetFramerateLimit((uint)Host.Settings.FPS);

            SadConsole.Host.Global.GraphicsDevice = window;
            SadConsole.Host.Global.SharedSpriteBatch = new SpriteBatch();

            //SadConsole.Host.Global.RenderStates.BlendMode = BlendMode.Multiply

            SadConsole.Settings.Rendering.RenderWidth = WindowSize.X;
            SadConsole.Settings.Rendering.RenderHeight = WindowSize.Y;
            ResetRendering();

            _keyboard = new Keyboard(window);
            _mouse = new Mouse(window);

            Host.Global.UpdateTimer = new SFML.System.Clock();
            Host.Global.DrawTimer = new SFML.System.Clock();

            SetRenderer("window", typeof(Renderers.Window));
            SetRenderer("controls", typeof(Renderers.ControlsConsole));
            SetRenderer("layered", typeof(Renderers.LayeredScreenSurface));
            SetRenderer("default", typeof(Renderers.ScreenSurfaceRenderer));

            // Create the default console.
            SadConsole.GameHost.Instance.Screen = new Console(ScreenCellsX, ScreenCellsY);
        }

        /// <inheritdoc/>
        public override void Run()
        {
            OnStart?.Invoke();

            SplashScreens.SplashScreenManager.CheckRun();

            // Update keyboard/mouse with base info
            SadConsole.GameHost.Instance.Keyboard.Update(TimeSpan.Zero);
            SadConsole.GameHost.Instance.Mouse.Update(TimeSpan.Zero);

            while (SadConsole.Host.Global.GraphicsDevice.IsOpen)
            {
                // Update game loop part
                if (SadConsole.Settings.DoUpdate)
                {
                    SadConsole.GameHost.Instance.UpdateFrameDelta = TimeSpan.FromSeconds(Host.Global.UpdateTimer.ElapsedTime.AsSeconds());

                    if (SadConsole.Host.Global.GraphicsDevice.HasFocus())
                    {
                        if (SadConsole.Settings.Input.DoKeyboard)
                        {
                            SadConsole.GameHost.Instance.Keyboard.Update(SadConsole.GameHost.Instance.UpdateFrameDelta);

                            if (SadConsole.GameHost.Instance.FocusedScreenObjects.ScreenObject != null && SadConsole.GameHost.Instance.FocusedScreenObjects.ScreenObject.UseKeyboard)
                            {
                                SadConsole.GameHost.Instance.FocusedScreenObjects.ScreenObject.ProcessKeyboard(SadConsole.GameHost.Instance.Keyboard);
                            }

                        }

                        if (SadConsole.Settings.Input.DoMouse)
                        {
                            SadConsole.GameHost.Instance.Mouse.Update(SadConsole.GameHost.Instance.UpdateFrameDelta);
                            SadConsole.GameHost.Instance.Mouse.Process();
                        }
                    }

                    SadConsole.GameHost.Instance.Screen?.Update(SadConsole.GameHost.Instance.UpdateFrameDelta);
                    
                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameUpdate();

                    Host.Global.UpdateTimer.Restart();
                }

                // Draw game loop part
                if (SadConsole.Settings.DoDraw)
                {
                    SadConsole.Host.Global.GraphicsDevice.Clear(SadConsole.Settings.ClearColor.ToSFMLColor());

                    SadConsole.GameHost.Instance.DrawFrameDelta = TimeSpan.FromSeconds(Host.Global.DrawTimer.ElapsedTime.AsSeconds());

                    // Clear draw calls for next run
                    SadConsole.Game.Instance.DrawCalls.Clear();

                    // Make sure all items in the screen are drawn. (Build a list of draw calls)
                    SadConsole.GameHost.Instance.Screen?.Render(SadConsole.GameHost.Instance.DrawFrameDelta);

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameDraw();

                    // Render to the global output texture
                    Host.Global.RenderOutput.Clear(SadConsole.Settings.ClearColor.ToSFMLColor());

                    // Render each draw call
                    Host.Global.SharedSpriteBatch.Reset(Host.Global.RenderOutput, SadConsole.Host.Settings.SFMLScreenBlendMode, Transform.Identity);

                    foreach (DrawCalls.IDrawCall call in SadConsole.Game.Instance.DrawCalls)
                        call.Draw();

                    Host.Global.SharedSpriteBatch.End();
                    Host.Global.RenderOutput.Display();

                    // If we're going to draw to the screen, do it.
                    if (SadConsole.Settings.DoFinalDraw)
                    {
                        Host.Global.SharedSpriteBatch.Reset(Host.Global.GraphicsDevice, SadConsole.Host.Settings.SFMLScreenBlendMode, Transform.Identity, SadConsole.Host.Settings.SFMLScreenShader);
                        Host.Global.SharedSpriteBatch.DrawQuad(Settings.Rendering.RenderRect.ToIntRect(), new IntRect(0, 0, (int)Host.Global.RenderOutput.Size.X, (int)Host.Global.RenderOutput.Size.Y), SFML.Graphics.Color.White, Host.Global.RenderOutput.Texture);
                        Host.Global.SharedSpriteBatch.End();
                    }
                }

                SadConsole.Host.Global.GraphicsDevice.Display();
                SadConsole.Host.Global.GraphicsDevice.DispatchEvents();

                Host.Global.DrawTimer.Restart();
            }

            OnEnd?.Invoke();
        }

        /// <inheritdoc/> 
        public override ITexture GetTexture(string resourcePath) =>
            new SadConsole.Host.GameTexture(resourcePath);

        /// <inheritdoc/> 
        public override ITexture GetTexture(Stream textureStream) =>
            new SadConsole.Host.GameTexture(textureStream);

        /// <inheritdoc/> 
        public override SadConsole.Input.IKeyboardState GetKeyboardState() =>
            _keyboard;

        /// <inheritdoc/> 
        public override SadConsole.Input.IMouseState GetMouseState() =>
            _mouse;


        /// <summary>
        /// Opens a read-only stream with MonoGame.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="mode">File open or create mode.</param>
        /// <param name="access">Read or write access.</param>
        /// <returns>The stream.</returns>
        public override Stream OpenStream(string file, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read) =>
             File.Open(file, mode, access);

        /// <summary>
        /// Toggles between windowed and fullscreen rendering for SadConsole.
        /// </summary>
        public void ToggleFullScreen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resizes the game window.
        /// </summary>
        /// <param name="width">The width of the window in pixels.</param>
        /// <param name="height">The height of the window in pixels.</param>
        public void ResizeWindow(int width, int height) =>
            Host.Global.GraphicsDevice.Size = new SFML.System.Vector2u((uint)width, (uint)height);

        /// <summary>
        /// Resets the <see cref="Host.Global.RenderOutput"/> target and determines the appropriate <see cref="SadConsole.Settings.Rendering.RenderRect"/> and <see cref="SadConsole.Settings.Rendering.RenderScale"/> based on the window or fullscreen state.
        /// </summary>
        public void ResetRendering()
        {
            Host.Global.RenderOutput?.Dispose();

            if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Center)
            {
                Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);

                SadConsole.Settings.Rendering.RenderRect = new Rectangle(
                                                            ((int)Host.Global.GraphicsDevice.Size.X - SadConsole.Settings.Rendering.RenderWidth) / 2,
                                                            ((int)Host.Global.GraphicsDevice.Size.Y - SadConsole.Settings.Rendering.RenderHeight) / 2,
                                                            SadConsole.Settings.Rendering.RenderWidth,
                                                            SadConsole.Settings.Rendering.RenderHeight);

                Host.Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Host.Global.GraphicsDevice.Size.X, Host.Global.GraphicsDevice.Size.Y)));

                SadConsole.Settings.Rendering.RenderScale = (1, 1);
            }
            else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Scale)
            {
                Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
                int multiple = 2;

                // Find the bounds
                while (true)
                {
                    if (SadConsole.Settings.Rendering.RenderWidth * multiple > Host.Global.GraphicsDevice.Size.X || SadConsole.Settings.Rendering.RenderHeight * multiple > Host.Global.GraphicsDevice.Size.Y)
                    {
                        multiple--;
                        break;
                    }

                    multiple++;
                }

                SadConsole.Settings.Rendering.RenderRect = new Rectangle(((int)Host.Global.GraphicsDevice.Size.X - (SadConsole.Settings.Rendering.RenderWidth * multiple)) / 2,
                                                                         ((int)Host.Global.GraphicsDevice.Size.Y - (SadConsole.Settings.Rendering.RenderHeight * multiple)) / 2,
                                                                         SadConsole.Settings.Rendering.RenderWidth * multiple,
                                                                         SadConsole.Settings.Rendering.RenderHeight * multiple);
                SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / ((float)SadConsole.Settings.Rendering.RenderWidth * multiple), SadConsole.Settings.Rendering.RenderHeight / (float)(SadConsole.Settings.Rendering.RenderHeight * multiple));

                Host.Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Host.Global.GraphicsDevice.Size.X, Host.Global.GraphicsDevice.Size.Y)));
            }
            else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Fit)
            {
                Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
                float heightRatio = Host.Global.GraphicsDevice.Size.Y / (float)SadConsole.Settings.Rendering.RenderHeight;
                float widthRatio = Host.Global.GraphicsDevice.Size.X / (float)SadConsole.Settings.Rendering.RenderWidth;

                float fitHeight = SadConsole.Settings.Rendering.RenderHeight * widthRatio;
                float fitWidth = SadConsole.Settings.Rendering.RenderWidth * heightRatio;

                if (fitHeight <= Host.Global.GraphicsDevice.Size.Y)
                {
                    // Render width = window width, pad top and bottom

                    SadConsole.Settings.Rendering.RenderRect = new Rectangle(0,
                                                                            (int)((Host.Global.GraphicsDevice.Size.Y - fitHeight) / 2),
                                                                            (int)Host.Global.GraphicsDevice.Size.X,
                                                                            (int)fitHeight);

                    SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / (float)Host.Global.GraphicsDevice.Size.X, SadConsole.Settings.Rendering.RenderHeight / fitHeight);
                }
                else
                {
                    // Render height = window height, pad left and right

                    SadConsole.Settings.Rendering.RenderRect = new Rectangle((int)((Host.Global.GraphicsDevice.Size.X - fitWidth) / 2),
                                                                             0,
                                                                             (int)fitWidth,
                                                                             (int)Host.Global.GraphicsDevice.Size.Y);

                    SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / fitWidth, SadConsole.Settings.Rendering.RenderHeight / (float)Host.Global.GraphicsDevice.Size.Y);
                }

                Host.Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Host.Global.GraphicsDevice.Size.X, Host.Global.GraphicsDevice.Size.Y)));
            }
            else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.None)
            {
                SadConsole.Settings.Rendering.RenderWidth = (int)Host.Global.GraphicsDevice.Size.X;
                SadConsole.Settings.Rendering.RenderHeight = (int)Host.Global.GraphicsDevice.Size.Y;
                Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
                SadConsole.Settings.Rendering.RenderRect = new Rectangle(0, 0, SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
                SadConsole.Settings.Rendering.RenderScale = (1, 1);
                Host.Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Host.Global.GraphicsDevice.Size.X, Host.Global.GraphicsDevice.Size.Y)));
            }
            else
            {
                Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
                Host.Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Host.Global.GraphicsDevice.Size.X, Host.Global.GraphicsDevice.Size.Y)));
                var view = Host.Global.GraphicsDevice.GetView();
                SadConsole.Settings.Rendering.RenderRect = new Rectangle(0, 0, (int)view.Size.X, (int)view.Size.Y);
                SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / (float)Host.Global.GraphicsDevice.Size.X, SadConsole.Settings.Rendering.RenderHeight / (float)Host.Global.GraphicsDevice.Size.Y);
            }
        }

        internal void InvokeFrameDraw() =>
            OnFrameRender();

        internal void InvokeFrameUpdate() =>
            OnFrameUpdate();
    }
}
