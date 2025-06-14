#nullable enable
using System;
using System.IO;
using SadRogue.Primitives;
using SadConsole.Host;
using SadConsole.Configuration;
using System.Linq;
using Raylib_cs;
using Color = SadRogue.Primitives.Color;
using System.Numerics;

namespace SadConsole;

/// <summary>
/// The SadConsole game object.
/// </summary>
public sealed partial class Game : GameHost
{
    /// <summary>
    /// The configuration used in creating the game object.
    /// </summary>
    internal Builder? _configuration;

    /// <summary>
    /// The keyboard translation object.
    /// </summary>
    private Keyboard _keyboard;

    /// <summary>
    /// The mouse translation object.
    /// </summary>
    private Mouse _mouse;

    /// <summary>
    /// Strongly typed version of <see cref="GameHost.Instance"/>.
    /// </summary>
    public static new Game Instance
    {
        get => (Game)GameHost.Instance;
        set => GameHost.Instance = value;
    }

    internal string _font;

    /// <summary>
    /// Creates the game instance.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Game() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Creates a new game with an initialization callback and a console set to the specific cell count that uses the default SadConsole IBM font.
    /// </summary>
    /// <param name="cellCountX">The width of the screen, in cells.</param>
    /// <param name="cellCountY">The height of the screen, in cells.</param>
    public static void Create(int cellCountX, int cellCountY) =>
        Create(new Builder()
                .SetWindowSizeInCells(cellCountX, cellCountY)
                .UseDefaultConsole()
                .IsStartingScreenFocused(true)
                .ConfigureFonts()
                );

    /// <summary>
    /// Creates a new game with an initialization callback and a console set to the specific cell count that uses the specified font.
    /// </summary>
    /// <param name="cellCountX">The width of the screen, in cells.</param>
    /// <param name="cellCountY">The height of the screen, in cells.</param>
    /// <param name="gameStarted">An event handler to be invoked when the game starts.</param>
    public static void Create(int cellCountX, int cellCountY, EventHandler<GameHost> gameStarted) =>
        Create(new Builder()
                .SetWindowSizeInCells(cellCountX, cellCountY)
                .UseDefaultConsole()
                .IsStartingScreenFocused(true)
                .ConfigureFonts()
                .OnStart(gameStarted)
                );

    /// <summary>
    /// Creates a new game with the specific screen size, and an initialization callback. Loads the specified font as the default.
    /// </summary>
    /// <param name="cellCountX">The width of the screen, in cells.</param>
    /// <param name="cellCountY">The height of the screen, in cells.</param>
    /// <param name="font">The font file to load.</param>
    /// <param name="gameStarted">An event handler to be invoked when the game starts.</param>
    public static void Create(int cellCountX, int cellCountY, string font, EventHandler<GameHost> gameStarted) =>
        Create(new Builder()
                .SetWindowSizeInCells(cellCountX, cellCountY)
                .UseDefaultConsole()
                .IsStartingScreenFocused(true)
                .ConfigureFonts(font)
                .OnStart(gameStarted)
                );

    /// <summary>
    /// Creates a new game and assigns it to the <see cref="Instance"/> property.
    /// </summary>
    /// <param name="configuration">The settings used in creating the game.</param>
    public static void Create(Builder configuration)
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

        // Configure the fonts
        FontConfig fontConfig = _configuration.Configs.OfType<FontConfig>().FirstOrDefault()
            ?? new FontConfig();

        _configuration.Configs.Remove(fontConfig);
        fontConfig.Run(_configuration, this);

        LoadDefaultFonts(fontConfig.AlternativeDefaultFont);

        DefaultFontSize = fontConfig.DefaultFontSize;

        foreach (string font in fontConfig.CustomFonts)
            LoadFont(font);

        // Load screen size and window
        ConfigureWindowConfig windowConfig = _configuration.Configs.OfType<ConfigureWindowConfig>().FirstOrDefault()
            ?? throw new Exception("The starting window or screen hasn't been configured.");

        _configuration.Configs.Remove(windowConfig);
        ((IConfigurator)windowConfig).Run(_configuration, this);

        // Get the existing window instance, or create a new one.
        InternalHostStartupData hostStartupData = _configuration.Configs.OfType<InternalHostStartupData>().FirstOrDefault() ?? new();

        Raylib.InitWindow(windowConfig.WindowWidthInPixels, windowConfig.WindowHeightInPixels, Settings.WindowTitle);

        // Configure window style
        ConfigFlags flags = ConfigFlags.ResizableWindow;

        if (windowConfig.Fullscreen)
            flags |= ConfigFlags.FullscreenMode;

        Raylib.SetWindowState(flags);

        // Load FPS
        FpsConfig fpsConfig = _configuration.Configs.OfType<FpsConfig>().FirstOrDefault() ?? new FpsConfig();
        if (!fpsConfig.UnlimitedFPS)
            if (Host.Settings.FPS > 0)
                Raylib.SetTargetFPS(Host.Settings.FPS);

        // Setup rendering sizes
        Settings.Rendering.RenderWidth = windowConfig.GameResolutionWidthInPixels;
        Settings.Rendering.RenderHeight = windowConfig.GameResolutionHeightInPixels;
        ResetRendering();

        // Configure input
        _keyboard = new Keyboard(Global.GraphicsDevice);
        _mouse = new Mouse(Global.GraphicsDevice);

        // Setup renderers
        SetRenderer(Renderers.Constants.RendererNames.Default, typeof(Renderers.ScreenSurfaceRenderer));
        SetRenderer(Renderers.Constants.RendererNames.ScreenSurface, typeof(Renderers.ScreenSurfaceRenderer));
        SetRenderer(Renderers.Constants.RendererNames.OptimizedScreenSurface, typeof(Renderers.OptimizedScreenSurfaceRenderer));
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
        if (Settings.AutomaticAddColorsToMappings)
            LoadMappedColors();

        // Run all startup config objects, then destroy the config instance
        _configuration.ProcessConfigs(this);

        // NOTE: Compared to the monogame game, this init method is invoked at create, but the game isn't yet running.
        // in monogame, this same method is invoked when the game starts running.
    }

    /// <inheritdoc/>
    public override void Run()
    {
        OnGameStarted();
        _configuration = null;
        SplashScreens.SplashScreenManager.CheckRun();

        // Update keyboard/mouse with base info
        Keyboard.Update(TimeSpan.Zero);
        Mouse.Update(TimeSpan.Zero);

        while (!Raylib.WindowShouldClose())
        {
            if (Raylib.IsWindowResized())
                ResetRendering();

            // Update timer
            TimeSpan timerCounter = TimeSpan.FromSeconds(Raylib.GetTime());
            
            // Update game loop part
            if (Settings.DoUpdate)
            {
                // Process any pre-Screen logic components
                foreach (Components.RootComponent item in Instance.RootComponents)
                    item.Run(UpdateFrameDelta);
                
                if (Raylib.IsWindowFocused() && !Global.BlockSadConsoleInput)
                {
                    if (Settings.Input.DoKeyboard)
                    {
                        Keyboard.Update(UpdateFrameDelta);

                        if (FocusedScreenObjects.ScreenObject != null && FocusedScreenObjects.ScreenObject.UseKeyboard)
                        {
                            FocusedScreenObjects.ScreenObject.ProcessKeyboard(Keyboard);
                        }

                    }

                    if (Settings.Input.DoMouse)
                    {
                        Mouse.Update(UpdateFrameDelta);
                        Mouse.Process();
                    }
                }

                Screen?.Update(UpdateFrameDelta);

                ((SadConsole.Game)Instance).InvokeFrameUpdate();
            }
            UpdateFrameDelta = TimeSpan.FromSeconds(Raylib.GetTime()) - timerCounter;


            // Draw timer
            timerCounter = TimeSpan.FromSeconds(Raylib.GetTime());

            // Draw game loop part
            if (Settings.DoDraw)
            {
                // Clear draw calls for next run
                Instance.DrawCalls.Clear();

                // Make sure all items in the screen are drawn. (Build a list of draw calls)
                Screen?.Render(DrawFrameDelta);

                ((SadConsole.Game)Instance).InvokeFrameDraw();

                // Render to the global output texture
                Raylib.BeginTextureMode(Global.RenderOutput);
                Raylib.ClearBackground(Settings.ClearColor.ToHostColor());
                Raylib.BeginBlendMode(Host.Settings.ScreenBlendMode);

                foreach (DrawCalls.IDrawCall call in Instance.DrawCalls)
                    call.Draw();

                Raylib.EndBlendMode();
                Raylib.EndTextureMode();

                // If we're going to draw to the screen, do it.
                if (Settings.DoFinalDraw)
                {
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Settings.ClearColor.ToHostColor());
                    Raylib.DrawTexturePro(Global.RenderOutput.Texture,
                                          new(0, 0, Global.RenderOutput.Texture.Width, Global.RenderOutput.Texture.Height),
                                          Settings.Rendering.RenderRect.ToHostRectangle(),
                                          Vector2.Zero,
                                          0f,
                                          Raylib_cs.Color.White);
                    Raylib.EndDrawing();
                }
                else
                    // Raylib.EndDrawing usually calls this, so if not drawing, refresh input
                    Raylib.PollInputEvents();
            }

            
            DrawFrameDelta = TimeSpan.FromSeconds(Raylib.GetTime()) - timerCounter;
        }

        OnGameEnding();
    }

    /// <inheritdoc/>
    public override ITexture GetTexture(string resourcePath) =>
        new SadConsole.Host.GameTexture(resourcePath);

    /// <inheritdoc/>
    public override ITexture GetTexture(Stream textureStream) =>
        new SadConsole.Host.GameTexture(textureStream);

    /// <inheritdoc/>
    public override ITexture CreateTexture(int width, int height) =>
        new Host.GameTexture(width, height);

    /// <inheritdoc/>
    public override SadConsole.Input.IKeyboardState GetKeyboardState()
    {
        _keyboard.Refresh();
        return _keyboard;
    }

    /// <inheritdoc/>
    public override SadConsole.Input.IMouseState GetMouseState() =>
        _mouse;

    /// <inheritdoc/>
    public override void GetDeviceScreenSize(out int width, out int height)
    {
        width = Raylib.GetScreenWidth();
        height = Raylib.GetScreenHeight();
    }

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
        throw new NotSupportedException("SFML doesn't support full screen yet.");
        // Check out https://stackoverflow.com/questions/61992029/sfml-c-sharp-how-do-i-switch-between-fullscreen
        // Would that help?
    }

    /// <inheritdoc/>
    public override void ResizeWindow(int width, int height, bool resizeOutputSurface = false)
    {
        Raylib.SetWindowSize(width, height);

        if (resizeOutputSurface)
        {
            Settings.Rendering.RenderWidth = width;
            Settings.Rendering.RenderHeight = height;
        }
    }

    /// <summary>
    /// Regenerates the <see cref="Global.RenderOutput"/> if the desired size doesn't match the current size.
    /// </summary>
    /// <param name="width">The width of the render output.</param>
    /// <param name="height">The height of the render output.</param>
    private void RecreateRenderOutput(int width, int height)
    {
        bool isOutputValid = Raylib.IsRenderTextureValid(Global.RenderOutput);

        if (!isOutputValid || Global.RenderOutput.Texture.Width != width || Global.RenderOutput.Texture.Width != height)
        {
            if (isOutputValid)
                Raylib.UnloadRenderTexture(Global.RenderOutput);

            Global.RenderOutput = Raylib.LoadRenderTexture(width, height);
        }
    }

    /// <summary>
    /// Resets the <see cref="Host.Global.RenderOutput"/> target and determines the appropriate <see cref="SadConsole.Settings.Rendering.RenderRect"/> and <see cref="SadConsole.Settings.Rendering.RenderScale"/> based on the window or fullscreen state.
    /// </summary>
    public void ResetRendering()
    {
        if (Settings.ResizeMode == Settings.WindowResizeOptions.Center)
        {
            RecreateRenderOutput(Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight);

            Settings.Rendering.RenderRect = new Rectangle(
                                                        Math.Max(0, ((int)Global.GraphicsDevice.Size.X - Settings.Rendering.RenderWidth) / 2),
                                                        Math.Max(0, ((int)Global.GraphicsDevice.Size.Y - Settings.Rendering.RenderHeight) / 2),
                                                        Settings.Rendering.RenderWidth,
                                                        Settings.Rendering.RenderHeight);

            Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Global.GraphicsDevice.Size.X, Global.GraphicsDevice.Size.Y)));

            Settings.Rendering.RenderScale = (1, 1);
        }
        else if (Settings.ResizeMode == Settings.WindowResizeOptions.Scale)
        {
            RecreateRenderOutput(Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight);

            int multiple = 2;

            // Find the bounds
            while (true)
            {
                if (Settings.Rendering.RenderWidth * multiple > Global.GraphicsDevice.Size.X || Settings.Rendering.RenderHeight * multiple > Global.GraphicsDevice.Size.Y)
                {
                    multiple--;
                    break;
                }

                multiple++;
            }

            Settings.Rendering.RenderRect = new Rectangle(
                                                        Math.Max(0, ((int)Global.GraphicsDevice.Size.X - (Settings.Rendering.RenderWidth * multiple)) / 2),
                                                        Math.Max(0, ((int)Global.GraphicsDevice.Size.Y - (Settings.Rendering.RenderHeight * multiple)) / 2),
                                                        Settings.Rendering.RenderWidth * multiple,
                                                        Settings.Rendering.RenderHeight * multiple);

            Settings.Rendering.RenderScale = (Settings.Rendering.RenderWidth / ((float)Settings.Rendering.RenderWidth * multiple), Settings.Rendering.RenderHeight / (float)(Settings.Rendering.RenderHeight * multiple));

            Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Global.GraphicsDevice.Size.X, Global.GraphicsDevice.Size.Y)));
        }
        else if (Settings.ResizeMode == Settings.WindowResizeOptions.Fit)
        {
            RecreateRenderOutput(Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight);

            float heightRatio = Global.GraphicsDevice.Size.Y / (float)Settings.Rendering.RenderHeight;
            float widthRatio = Global.GraphicsDevice.Size.X / (float)Settings.Rendering.RenderWidth;

            float fitHeight = Settings.Rendering.RenderHeight * widthRatio;
            float fitWidth = Settings.Rendering.RenderWidth * heightRatio;

            if (fitHeight <= Global.GraphicsDevice.Size.Y)
            {
                // Render width = window width, pad top and bottom

                Settings.Rendering.RenderRect = new Rectangle(0,
                                                            Math.Max(0, (int)((Global.GraphicsDevice.Size.Y - fitHeight) / 2)),
                                                            (int)Global.GraphicsDevice.Size.X,
                                                            (int)fitHeight);

                Settings.Rendering.RenderScale = (Settings.Rendering.RenderWidth / (float)Global.GraphicsDevice.Size.X, Settings.Rendering.RenderHeight / fitHeight);
            }
            else
            {
                // Render height = window height, pad left and right
                Settings.Rendering.RenderRect = new Rectangle(Math.Max(0, (int)((Global.GraphicsDevice.Size.X - fitWidth) / 2)),
                                                                0,
                                                                (int)fitWidth,
                                                                (int)Global.GraphicsDevice.Size.Y);

                Settings.Rendering.RenderScale = (Settings.Rendering.RenderWidth / fitWidth, Settings.Rendering.RenderHeight / (float)Global.GraphicsDevice.Size.Y);
            }

            Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Global.GraphicsDevice.Size.X, Global.GraphicsDevice.Size.Y)));
        }
        else if (Settings.ResizeMode == Settings.WindowResizeOptions.None)
        {
            Settings.Rendering.RenderWidth = (int)Global.GraphicsDevice.Size.X;
            Settings.Rendering.RenderHeight = (int)Global.GraphicsDevice.Size.Y;
            RecreateRenderOutput(Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight);
            Settings.Rendering.RenderRect = new Rectangle(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight);
            Settings.Rendering.RenderScale = (1, 1);
            Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Global.GraphicsDevice.Size.X, Global.GraphicsDevice.Size.Y)));
        }
        else
        {
            RecreateRenderOutput(Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight);
            Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Global.GraphicsDevice.Size.X, Global.GraphicsDevice.Size.Y)));
            var view = Global.GraphicsDevice.GetView();
            Settings.Rendering.RenderRect = new Rectangle(0, 0, (int)view.Size.X, (int)view.Size.Y);
            Settings.Rendering.RenderScale = (Settings.Rendering.RenderWidth / (float)Global.GraphicsDevice.Size.X, Settings.Rendering.RenderHeight / (float)Global.GraphicsDevice.Size.Y);
        }
    }

    internal void InvokeFrameDraw() =>
        OnFrameRender();

    internal void InvokeFrameUpdate() =>
        OnFrameUpdate();

    internal void SetStartingConsole(Console? console) =>
        StartingConsole = console;
}
#nullable disable
