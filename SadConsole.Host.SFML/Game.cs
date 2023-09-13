#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SFML.Graphics;
using SadRogue.Primitives;
using SadConsole.Host;

namespace SadConsole;

/// <summary>
/// The SadConsole game object.
/// </summary>
public sealed partial class Game : GameHost
{
    /// <summary>
    /// The configuration used in creating the game object.
    /// </summary>
    internal Configuration? _configuration;

    /// <summary>
    /// The keyboard translation object.
    /// </summary>
    private Keyboard _keyboard;

    /// <summary>
    /// The mouse translation object.
    /// </summary>
    private Mouse _mouse;

    /// <summary>
    /// Static instance to the game after the <see cref="Create()"/> method has been called.
    /// </summary>
    public new static Game Instance
    {
        get => (Game)GameHost.Instance;
        private set => GameHost.Instance = value;
    }

    internal string _font;

    /// <summary>
    /// Creates the game instance.
    /// </summary>
    private Game() { }

    /// <summary>
    /// Creates a new game with default configuration, an 80x25 console.
    /// </summary>
    public static void Create() =>
        Create(new Configuration());

    /// <summary>
    /// Creates a new game with the specific screen size, and an initialization callback. Uses the default IBM 8x16 font.
    /// </summary>
    /// <param name="cellCountX">The width of the screen, in cells.</param>
    /// <param name="cellCountY">The height of the screen, in cells.</param>
    /// <param name="initCallback">A method which is called after SadConsole has been started.</param>
    public static void Create(int cellCountX, int cellCountY, Action initCallback) =>
        Create(new Configuration()
                    .SetScreenSize(cellCountX, cellCountY)
                    .OnStart(initCallback));

    /// <summary>
    /// Creates a new game with the specific screen size, and an initialization callback. Loads the specified font as the default.
    /// </summary>
    /// <param name="cellCountX">The width of the screen, in cells.</param>
    /// <param name="cellCountY">The height of the screen, in cells.</param>
    /// <param name="font">The font file to load.</param>
    /// <param name="initCallback">A method which is called after SadConsole has been started.</param>
    public static void Create(int cellCountX, int cellCountY, string font, Action initCallback) =>
        Create(new Configuration()
                    .SetScreenSize(cellCountX, cellCountY)
                    .ConfigureFonts(loader => loader.UseCustomFont(font))
                    .OnStart(initCallback));

    /// <summary>
    /// Creates a new game and assigns it to the <see cref="Instance"/> property.
    /// </summary>
    /// <param name="configuration">The settings used in creating the game.</param>
    public static void Create(Configuration configuration)
    {
        if (Instance != null) throw new Exception("The game has already been created.");

        var game = new Game();

        game._configuration = configuration;

        Instance = game;

        game.Initialize();
    }

    /// <summary>
    /// Initializes SadConsole and sets up the window events, based on the <see cref="_configuration"/> variable.
    /// </summary>
    private void Initialize()
    {
        if (_configuration == null) throw new Exception("Configuration must be set.");

        OnStart = _configuration.OnStartCallback;
        OnEnd = _configuration.OnEndCallback;

        // Configure the fonts
        _configuration.RunFontConfig();
        SadConsole.Settings.UseDefaultExtendedFont = _configuration.FontLoaderData.UseExtendedFont;
        LoadDefaultFonts(_configuration.FontLoaderData.AlternativeDefaultFont);

        foreach (var font in _configuration.FontLoaderData.CustomFonts)
            LoadFont(font);

        // Load screen size
        ScreenCellsX = _configuration.ScreenSizeWidth;
        ScreenCellsY = _configuration.ScreenSizeHeight;

        if (_configuration.TargetWindow == null)
        {
            Global.GraphicsDevice = new RenderWindow(new SFML.Window.VideoMode((uint)(DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize).X * ScreenCellsX), (uint)(DefaultFont.GetFontSize(DefaultFontSize).Y * ScreenCellsY)), Host.Settings.WindowTitle, SFML.Window.Styles.Titlebar | SFML.Window.Styles.Close | SFML.Window.Styles.Resize);
            Global.GraphicsDevice.SetTitle(Settings.WindowTitle);
        }
        else
            Global.GraphicsDevice = _configuration.TargetWindow;

        Global.GraphicsDevice.Closed += (o, e) =>
        {
            ((SFML.Window.Window)o).Close();
        };

        Global.GraphicsDevice.Resized += (o, e) =>
        {
            ResetRendering();
        };

        if (!Settings.UnlimitedFPS)
            if (Host.Settings.FPS > 0)
                Global.GraphicsDevice.SetFramerateLimit((uint)Host.Settings.FPS);

        SadConsole.Host.Global.SharedSpriteBatch = new SpriteBatch();

        //SadConsole.Host.Global.RenderStates.BlendMode = BlendMode.Multiply

        // Setup rendering sizes
        SadConsole.Settings.Rendering.RenderWidth = (int)SadConsole.Host.Global.GraphicsDevice.Size.X;
        SadConsole.Settings.Rendering.RenderHeight = (int)SadConsole.Host.Global.GraphicsDevice.Size.Y;
        ResetRendering();

        // Configure input
        _keyboard = new Keyboard(SadConsole.Host.Global.GraphicsDevice);
        _mouse = new Mouse(SadConsole.Host.Global.GraphicsDevice);

        // Create game loop timers
        Host.Global.UpdateTimer = new SFML.System.Clock();
        Host.Global.DrawTimer = new SFML.System.Clock();

        // Setup renderers
        SetRenderer(Renderers.Constants.RendererNames.Default, typeof(Renderers.ScreenSurfaceRenderer));
        SetRenderer(Renderers.Constants.RendererNames.ScreenSurface, typeof(Renderers.ScreenSurfaceRenderer));
        SetRenderer(Renderers.Constants.RendererNames.Window, typeof(Renderers.WindowRenderer));
        SetRenderer(Renderers.Constants.RendererNames.LayeredScreenSurface, typeof(Renderers.LayeredRenderer));

        SetRendererStep(Renderers.Constants.RenderStepNames.ControlHost, typeof(Renderers.ControlHostRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Cursor, typeof(Renderers.CursorRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.EntityManager, typeof(Renderers.EntityRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Output, typeof(Renderers.OutputSurfaceRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Surface, typeof(Renderers.SurfaceRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.SurfaceDirtyCells, typeof(Renderers.SurfaceDirtyCellsRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Tint, typeof(Renderers.TintSurfaceRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Window, typeof(Renderers.WindowRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.SurfaceLayered, typeof(Renderers.LayeredSurfaceRenderStep));

        // Load the mapped colors
        LoadMappedColors();

        // Hook up any events via config
        if (_configuration.event_FrameUpdate != null)
            FrameUpdate += _configuration.event_FrameUpdate;

        if (_configuration.event_FrameRender != null)
            FrameRender += _configuration.event_FrameRender;

        // Setup default starting console, otherwise, use the config starting object.
        if (_configuration.CreateStartingConsole)
        {
            StartingConsole = new Console(ScreenCellsX, ScreenCellsY);
            StartingConsole.IsFocused = true;
            Screen = StartingConsole;
        }
        else
        {
            try
            {
                Screen = _configuration.GenerateStartingObject(this);
                Screen.IsFocused = _configuration.FocusStartingObject;
            }
            catch (NullReferenceException e)
            {
                throw new NullReferenceException("'Settings.CreateStartingConsole' is false; configuration 'SetStartingScreen' method must return a valid object.");
            }
        }

        // Set splash screens from config
        if (_configuration.GenerateSplashScreen != null)
            SetSplashScreens(_configuration.GenerateSplashScreen(this));

        // Kill off config instance
        _configuration = null;

        // NOTE: Compared to the monogame game, this init method is invoked at create, but the game isn't yet running.
        // in monogame, this same method is invoked when the game starts running.
    }

    /// <inheritdoc/>
    public override void Run()
    {
        OnStart?.Invoke();

        SplashScreens.SplashScreenManager.CheckRun();

        // Update keyboard/mouse with base info
        Keyboard.Update(TimeSpan.Zero);
        Mouse.Update(TimeSpan.Zero);

        while (SadConsole.Host.Global.GraphicsDevice.IsOpen)
        {
            // Update game loop part
            if (SadConsole.Settings.DoUpdate)
            {
                UpdateFrameDelta = TimeSpan.FromSeconds(Host.Global.UpdateTimer.ElapsedTime.AsSeconds());

                if (SadConsole.Host.Global.GraphicsDevice.HasFocus() && !Global.BlockSadConsoleInput)
                {
                    if (SadConsole.Settings.Input.DoKeyboard)
                    {
                        Keyboard.Update(UpdateFrameDelta);

                        if (FocusedScreenObjects.ScreenObject != null && FocusedScreenObjects.ScreenObject.UseKeyboard)
                        {
                            FocusedScreenObjects.ScreenObject.ProcessKeyboard(Keyboard);
                        }

                    }

                    if (SadConsole.Settings.Input.DoMouse)
                    {
                        Mouse.Update(UpdateFrameDelta);
                        Mouse.Process();
                    }
                }

                Screen?.Update(UpdateFrameDelta);
                
                ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameUpdate();

                Host.Global.UpdateTimer.Restart();
            }

            // Draw game loop part
            if (SadConsole.Settings.DoDraw)
            {
                SadConsole.Host.Global.GraphicsDevice.Clear(SadConsole.Settings.ClearColor.ToSFMLColor());

                DrawFrameDelta = TimeSpan.FromSeconds(Host.Global.DrawTimer.ElapsedTime.AsSeconds());

                // Clear draw calls for next run
                SadConsole.Game.Instance.DrawCalls.Clear();

                // Make sure all items in the screen are drawn. (Build a list of draw calls)
                Screen?.Render(DrawFrameDelta);

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
    public override ITexture CreateTexture(int width, int height) =>
        new Host.GameTexture((uint)width, (uint)height);

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
    /// Toggles between windowed and full screen rendering for SadConsole.
    /// </summary>
    public void ToggleFullScreen()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override void ResizeWindow(int width, int height) =>
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
#nullable disable
