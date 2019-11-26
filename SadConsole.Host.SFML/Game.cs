using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SFML.Graphics;
using SadRogue.Primitives;
using SadConsole.Host;

namespace SadConsole
{
    public class Game : GameHost
    {
        private int _preFullScreenWidth;
        private int _preFullScreenHeight;
        private bool _handleResizeNone;

        private Keyboard _keyboard;
        private Mouse _mouse;
        
        public new static Game Instance
        {
            get => (Game)GameHost.Instance;
            protected set => GameHost.Instance = value;
        }

        internal string _font;


        private Game() { }

        public static void Create(int cellCountX, int cellCountY, string font = "", RenderWindow window = null)
        {
            var game = new Game();
            game.ScreenCellsX = cellCountX;
            game.ScreenCellsY = cellCountY;
            game._font = font;

            Instance = game;
            game.Initialize(window);
        }

        private void Initialize(RenderWindow window)
        {
            if (string.IsNullOrEmpty(_font))
                LoadEmbeddedFont();
            else
                Global.DefaultFont = LoadFont(_font);

            if (window == null)
            {
                window = new RenderWindow(new SFML.Window.VideoMode((uint)(Global.DefaultFont.GetFontSize(Global.DefaultFontSize).X * ScreenCellsX), (uint)(Global.DefaultFont.GetFontSize(Global.DefaultFontSize).Y * ScreenCellsY)), Host.Settings.WindowTitle, SFML.Window.Styles.Titlebar | SFML.Window.Styles.Close);
                // SETUP RENDER vars for global screen size data.
            }

            WindowSize = new Point((int)window.Size.X, (int)window.Size.Y);

            window.Closed += (o, e) =>
            {
                ((SFML.Window.Window)o).Close();
            };

            if (Host.Settings.FPS != 0)
                window.SetFramerateLimit((uint)Host.Settings.FPS);

            SadConsole.Host.Global.GraphicsDevice = window;
            SadConsole.Host.Global.SharedSpriteBatch = new SpriteBatch();

            SadConsole.Settings.Rendering.RenderWidth = WindowSize.X;
            SadConsole.Settings.Rendering.RenderHeight = WindowSize.Y;
            ResetRendering();

            _keyboard = new Keyboard(window);
            _mouse = new Mouse(window);
            Host.Global.UpdateTimer = new SFML.System.Clock();
            Host.Global.DrawTimer = new SFML.System.Clock();

            // Create the default console.
            SadConsole.Global.Screen = new Console(ScreenCellsX, ScreenCellsY);
        }

        public override void Run()
        {
            OnStart?.Invoke();

            while (SadConsole.Host.Global.GraphicsDevice.IsOpen)
            {
                SadConsole.Host.Global.GraphicsDevice.Clear(SadConsole.Settings.ClearColor.ToSFMLColor());

                // Update game loop part
                if (SadConsole.Settings.DoUpdate)
                {
                    SadConsole.Global.UpdateFrameDelta = TimeSpan.FromSeconds(Host.Global.UpdateTimer.ElapsedTime.AsSeconds());

                    if (SadConsole.Host.Global.GraphicsDevice.HasFocus())
                    {
                        if (SadConsole.Settings.Input.DoKeyboard)
                        {
                            SadConsole.Global.Keyboard.Update(SadConsole.Global.UpdateFrameDelta);

                            if (SadConsole.Global.FocusedScreenObjects.ScreenObject != null && SadConsole.Global.FocusedScreenObjects.ScreenObject.UseKeyboard)
                            {
                                SadConsole.Global.FocusedScreenObjects.ScreenObject.ProcessKeyboard(SadConsole.Global.Keyboard);
                            }

                        }

                        if (SadConsole.Settings.Input.DoMouse)
                        {
                            SadConsole.Global.Mouse.Update(SadConsole.Global.UpdateFrameDelta);
                            SadConsole.Global.Mouse.Process();
                        }
                    }

                    SadConsole.Global.Screen?.Update();

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameUpdate();

                    Host.Global.UpdateTimer.Restart();
                }

                // Draw game loop part
                if (SadConsole.Settings.DoDraw)
                {
                    SadConsole.Global.DrawFrameDelta = TimeSpan.FromSeconds(Host.Global.DrawTimer.ElapsedTime.AsSeconds());

                    // Clear draw calls for next run
                    SadConsole.Game.Instance.DrawCalls.Clear();

                    // Make sure all items in the screen are drawn. (Build a list of draw calls)
                    SadConsole.Global.Screen?.Draw();

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameDraw();

                    // Render to the global output texture
                    Host.Global.RenderOutput.Clear(SadConsole.Settings.ClearColor.ToSFMLColor());

                    // Render each draw call
                    Host.Global.SharedSpriteBatch.Reset(Host.Global.RenderOutput, RenderStates.Default, Transform.Identity);

                    foreach (DrawCalls.IDrawCall call in SadConsole.Game.Instance.DrawCalls)
                        call.Draw();

                    Host.Global.SharedSpriteBatch.End();
                    Host.Global.RenderOutput.Display();

                    // If we're going to draw to the screen, do it.
                    if (SadConsole.Settings.DoFinalDraw)
                    {
                        Host.Global.SharedSpriteBatch.Reset(Host.Global.GraphicsDevice, RenderStates.Default, Transform.Identity);
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

        public override ITexture GetTexture(string resourcePath) =>
            new SadConsole.Host.GameTexture(resourcePath);

        public override ITexture GetTexture(Stream textureStream) =>
            new SadConsole.Host.GameTexture(textureStream);

        public override Renderers.IRenderer GetDefaultRenderer(IScreenSurface screenObject) =>
            screenObject switch
            {
                UI.ControlsConsole _ => new Renderers.ControlsConsole(),
                Console _ => new Renderers.ConsoleRenderer(),
                _ => new Renderers.ScreenObjectRenderer(),
            };


        public override SadConsole.Input.IKeyboardState GetKeyboardState() =>
            _keyboard;

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

        internal void MonoGameLoadEmbeddedFont() =>
            LoadEmbeddedFont();

        /// <summary>
        /// Toggles between windowed and fullscreen rendering for SadConsole.
        /// </summary>
        public void ToggleFullScreen()
        {
            // TODO full screen

            //MonoGame.Global.GraphicsDeviceManager.ApplyChanges();

            //// Coming back from fullscreen
            //if (MonoGame.Global.GraphicsDeviceManager.IsFullScreen)
            //{
            //    MonoGame.Global.GraphicsDeviceManager.IsFullScreen = !MonoGame.Global.GraphicsDeviceManager.IsFullScreen;

            //    MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferWidth = _preFullScreenWidth;
            //    MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferHeight = _preFullScreenHeight;
            //    MonoGame.Global.GraphicsDeviceManager.ApplyChanges();
            //}

            //// Going full screen
            //else
            //{
            //    _preFullScreenWidth = MonoGame.Global.GraphicsDevice.PresentationParameters.BackBufferWidth;
            //    _preFullScreenHeight = MonoGame.Global.GraphicsDevice.PresentationParameters.BackBufferHeight;

            //    if (Settings.ResizeMode == Settings.WindowResizeOptions.None)
            //    {
            //        _handleResizeNone = true;
            //        Settings.ResizeMode = Settings.WindowResizeOptions.Scale;
            //    }

            //    MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferWidth = MonoGame.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            //    MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferHeight = MonoGame.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

            //    MonoGame.Global.GraphicsDeviceManager.IsFullScreen = !MonoGame.Global.GraphicsDeviceManager.IsFullScreen;
            //    MonoGame.Global.GraphicsDeviceManager.ApplyChanges();

            //    if (_handleResizeNone)
            //    {
            //        _handleResizeNone = false;
            //        Settings.ResizeMode = Settings.WindowResizeOptions.None;
            //    }
            //}
        }

        /// <summary>
        /// Resizes the game window.
        /// </summary>
        /// <param name="width">The width of the window in pixels.</param>
        /// <param name="height">The height of the window in pixels.</param>
        public void ResizeWindow(int width, int height)
        {
            // TODO: Resize window
            //MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            //MonoGame.Global.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            //MonoGame.Global.GraphicsDeviceManager.ApplyChanges();

            //((Game)SadConsole.Game.Instance).MonoGameInstance.ResetRendering();
        }

        /// <summary>
        /// Resets the <see cref="RenderOutput"/> target and determines the appropriate <see cref="RenderRect"/> and <see cref="RenderScale"/> based on the window or fullscreen state.
        /// </summary>
        public void ResetRendering()
        {
            Host.Global.RenderOutput?.Dispose();

            //if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Center)
            //{
            //    Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
            //    SadConsole.Settings.Rendering.RenderRect = new Rectangle(
            //                                                (Host.Global.GraphicsDevice.DefaultView.Viewport.Width - SadConsole.Settings.Rendering.RenderWidth) / 2,
            //                                                (GraphicsDevice.PresentationParameters.BackBufferHeight - SadConsole.Settings.Rendering.RenderHeight) / 2,
            //                                                SadConsole.Settings.Rendering.RenderWidth,
            //                                                SadConsole.Settings.Rendering.RenderHeight).ToRectangle();

            //    SadConsole.Settings.Rendering.RenderScale = new System.Numerics.Vector2(1);
            //}
            //else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Scale)
            //{
            //    Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
            //    int multiple = 2;

            //    // Find the bounds
            //    while (true)
            //    {
            //        if (SadConsole.Settings.Rendering.RenderWidth * multiple > GraphicsDevice.PresentationParameters.BackBufferWidth || SadConsole.Settings.Rendering.RenderHeight * multiple > GraphicsDevice.PresentationParameters.BackBufferHeight)
            //        {
            //            multiple--;
            //            break;
            //        }

            //        multiple++;
            //    }

            //    SadConsole.Settings.Rendering.RenderRect = new Rectangle((GraphicsDevice.PresentationParameters.BackBufferWidth - (SadConsole.Settings.Rendering.RenderWidth * multiple)) / 2,
            //                                                             (GraphicsDevice.PresentationParameters.BackBufferHeight - (SadConsole.Settings.Rendering.RenderHeight * multiple)) / 2,
            //                                                             SadConsole.Settings.Rendering.RenderWidth * multiple,
            //                                                             SadConsole.Settings.Rendering.RenderHeight * multiple).ToRectangle();
            //    SadConsole.Settings.Rendering.RenderScale = new System.Numerics.Vector2(SadConsole.Settings.Rendering.RenderWidth / ((float)SadConsole.Settings.Rendering.RenderWidth * multiple), SadConsole.Settings.Rendering.RenderHeight / (float)(SadConsole.Settings.Rendering.RenderHeight * multiple));
            //}
            //else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Fit)
            //{
            //    Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
            //    float heightRatio = GraphicsDevice.PresentationParameters.BackBufferHeight / (float)SadConsole.Settings.Rendering.RenderHeight;
            //    float widthRatio = GraphicsDevice.PresentationParameters.BackBufferWidth / (float)SadConsole.Settings.Rendering.RenderWidth;

            //    float fitHeight = SadConsole.Settings.Rendering.RenderHeight * widthRatio;
            //    float fitWidth = SadConsole.Settings.Rendering.RenderWidth * heightRatio;

            //    if (fitHeight <= GraphicsDevice.PresentationParameters.BackBufferHeight)
            //    {
            //        // Render width = window width, pad top and bottom

            //        SadConsole.Settings.Rendering.RenderRect = new Rectangle(0,
            //                                                                (int)((GraphicsDevice.PresentationParameters.BackBufferHeight - fitHeight) / 2),
            //                                                                GraphicsDevice.PresentationParameters.BackBufferWidth,
            //                                                                (int)fitHeight).ToRectangle();

            //        SadConsole.Settings.Rendering.RenderScale = new System.Numerics.Vector2(SadConsole.Settings.Rendering.RenderWidth / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, SadConsole.Settings.Rendering.RenderHeight / fitHeight);
            //    }
            //    else
            //    {
            //        // Render height = window height, pad left and right

            //        SadConsole.Settings.Rendering.RenderRect = new Rectangle((int)((GraphicsDevice.PresentationParameters.BackBufferWidth - fitWidth) / 2),
            //                                                                 0,
            //                                                                 (int)fitWidth,
            //                                                                 GraphicsDevice.PresentationParameters.BackBufferHeight).ToRectangle();

            //        SadConsole.Settings.Rendering.RenderScale = new System.Numerics.Vector2(SadConsole.Settings.Rendering.RenderWidth / fitWidth, SadConsole.Settings.Rendering.RenderHeight / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);
            //    }
            //}
            //else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.None)
            //{
            //    SadConsole.Settings.Rendering.RenderWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            //    SadConsole.Settings.Rendering.RenderHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            //    Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
            //    SadConsole.Settings.Rendering.RenderRect = GraphicsDevice.Viewport.Bounds.ToRectangle();
            //    SadConsole.Settings.Rendering.RenderScale = new System.Numerics.Vector2(1);
            //}
            //else
            {
                Host.Global.RenderOutput = new RenderTexture((uint)SadConsole.Settings.Rendering.RenderWidth, (uint)SadConsole.Settings.Rendering.RenderHeight);
                var view = Host.Global.GraphicsDevice.GetView();
                SadConsole.Settings.Rendering.RenderRect = new Rectangle(0, 0, (int)view.Size.X, (int)view.Size.Y);
                SadConsole.Settings.Rendering.RenderScale = new System.Numerics.Vector2(SadConsole.Settings.Rendering.RenderWidth / (float)Host.Global.GraphicsDevice.Size.X, SadConsole.Settings.Rendering.RenderHeight / (float)Host.Global.GraphicsDevice.Size.Y);
            }
        }

        internal void InvokeFrameDraw() =>
            OnFrameDraw();

        internal void InvokeFrameUpdate() =>
            OnFrameUpdate();
    }
}
