#nullable enable
using System;
using System.IO;
using System.Linq;
using MonoGame.Framework.WpfInterop;
using SadConsole.Configuration;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// The MonoGame implementation of the SadConsole Game Host.
/// </summary>
public partial class Game : GameHost
{
    private Host.Mouse _mouseState = new Host.Mouse();
    private Host.Keyboard _keyboardState = new Host.Keyboard();

    /// <summary>
    /// The configuration used in creating the game object.
    /// </summary>
    internal Configuration.Builder? _configuration;

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
    /// Creates a new game with an 80x25 console that uses the default SadConsole IBM font.
    /// </summary>
    public static void Create() =>
        Create(80, 25);

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
    /// Creates a new game and assigns it to the <see cref="MonoGameInstance"/> property.
    /// </summary>
    /// <param name="configuration">The settings used in creating the game.</param>
    public static void Create(Builder configuration)
    {
        if (Instance != null) throw new Exception("The game has already been created.");

        var game = new Game();

        // Make sure the MonoGame Game instance calls back to SadConsole config after it's initialized.
        configuration.WithMonoGameInit(game.MonoGameInit);

        game._configuration = configuration;

        Instance = game;

        // This creates the monogame game instance, which:
        // 1. Runs the ctor
        // 2. ctor runs the config.WithMonoGameCtor method added by user
        //
        // When the user then calls this object's .Run method, monogame initializes itself
        // and after it's done, calls back to game.MonoGameInit to finish SadConsole
        // init.
        game.MonoGameInstance = new Host.Game();
    }

    /// <summary>
    /// Method called by the <see cref="Host.Game"/> class for initializing SadConsole specifics. Called prior to <see cref="Host.Game.ResetRendering"/>.
    /// </summary>
    /// <param name="game">The game instance.</param>
    internal void MonoGameInit(WpfGame game)
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

        // Load screen size
        // Load screen size and window
        ConfigureWindowConfig windowConfig = _configuration.Configs.OfType<ConfigureWindowConfig>().FirstOrDefault()
            ?? throw new Exception("The starting window or screen hasn't been configured.");

        _configuration.Configs.Remove(windowConfig);
        ((IConfigurator)windowConfig).Run(_configuration, this);

        Settings.Rendering.RenderWidth = windowConfig.GameResolutionWidthInPixels;
        Settings.Rendering.RenderHeight = windowConfig.GameResolutionHeightInPixels;

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

        // Load special FPS visual
        //FpsConfig? fpsConfig = _configuration.Configs.OfType<FpsConfig>().FirstOrDefault();
        //if (fpsConfig != null && fpsConfig.ShowFPSVisual)
        //    Instance.MonoGameInstance.Components.Add(new Host.Game.FPSCounterComponent(Instance.MonoGameInstance));

        // Run all startup config objects
        _configuration.ProcessConfigs(this);

        var fontSize = DefaultFont.GetFontSize(DefaultFontSize);
        if (fontSize.X > Settings.Rendering.RenderWidth || fontSize.Y > Settings.Rendering.RenderHeight)
            throw new Exception("WPF control is too small to present a single cell in the font size.");

        // Normal start
        OnGameStarted();
        _configuration = null;
        SplashScreens.SplashScreenManager.CheckRun();
    }

    /// <inheritdoc/>
    public override ITexture GetTexture(string resourcePath) =>
        new Host.GameTexture(resourcePath);

    /// <inheritdoc/>
    public override ITexture GetTexture(Stream textureStream) =>
        new Host.GameTexture(textureStream);

    /// <inheritdoc/>
    public override ITexture CreateTexture(int width, int height) =>
        new Host.GameTexture(width, height);

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

    /// <inheritdoc/>
    public override void GetDeviceScreenSize(out int width, out int height)
    {
        width = MonoGameInstance.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        height = MonoGameInstance.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
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

        return File.OpenRead(file);
    }

    internal void SetStartingConsole(Console? console) =>
        StartingConsole = console;

    public override void Run()
    {
        throw new NotImplementedException();
    }

    public override void ResizeWindow(int width, int height, bool resizeOutputSurface = false)
    {
        throw new NotImplementedException();
    }

    internal void InvokeFrameDraw() =>
        OnFrameRender();

    internal void InvokeFrameUpdate() =>
        OnFrameUpdate();
}
