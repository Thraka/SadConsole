#nullable enable

using System;
using System.IO;
using System.Linq;
using SadConsole.Configuration;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// The MonoGame implementation of the SadConsole Game Host.
/// </summary>
public sealed partial class Game : GameHost
{
    /// <summary>
    /// The configuration used in creating the game object.
    /// </summary>
    internal Builder? _configuration;

    /// <summary>
    /// Temporary variable to store the screen width before going full screen.
    /// </summary>
    private int _preFullScreenWidth;

    /// <summary>
    /// Temporary variable to store the screen height before going full screen.
    /// </summary>
    private int _preFullScreenHeight;

    /// <summary>
    /// Temporary variable to store the state of the <see cref="Settings.ResizeMode"/> when it's set to None, before going full screen.
    /// </summary>
    private bool _handleResizeNone;

    private Host.Mouse _mouseState = new Host.Mouse();
    private Host.Keyboard _keyboardState = new Host.Keyboard();

    /// <summary>
    /// The <see cref="Microsoft.Xna.Framework.Game"/> instance.
    /// </summary>
    public Host.Game MonoGameInstance { get; private set; }

    /// <summary>
    /// Strongly typed version of <see cref="GameHost.Instance"/>.
    /// </summary>
    public new static Game Instance
    {
        get => (Game)GameHost.Instance;
        set => GameHost.Instance = value;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Game() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
                .SetScreenSize(cellCountX, cellCountY)
                .UseDefaultConsole()
                .IsStartingScreenFocused(true)
                .ConfigureFonts((loader, game) => loader.UseBuiltinFont())
                );

    /// <summary>
    /// Creates a new game with an initialization callback and a console set to the specific cell count that uses the specified font.
    /// </summary>
    /// <param name="cellCountX">The width of the screen, in cells.</param>
    /// <param name="cellCountY">The height of the screen, in cells.</param>
    /// <param name="gameStarted">An event handler to be invoked when the game starts.</param>
    public static void Create(int cellCountX, int cellCountY, EventHandler<GameHost> gameStarted) =>
        Create(new Builder()
                .SetScreenSize(cellCountX, cellCountY)
                .UseDefaultConsole()
                .IsStartingScreenFocused(true)
                .ConfigureFonts((loader, game) => loader.UseBuiltinFont())
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
                .SetScreenSize(cellCountX, cellCountY)
                .UseDefaultConsole()
                .IsStartingScreenFocused(true)
                .ConfigureFonts((loader, game) => loader.UseCustomFont(font))
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
    private void MonoGameInit(Host.Game game)
    {
        if (_configuration == null) throw new Exception("Configuration must be set.");

        // Configure the fonts
        FontConfig fontConfig = _configuration.Configs.OfType<FontConfig>().FirstOrDefault()
            ?? throw new Exception($"Configuration object must have a {nameof(FontConfig)} configurator added to it.");

        _configuration.Configs.Remove(fontConfig);
        fontConfig.Run(_configuration, this);

        LoadDefaultFonts(fontConfig.AlternativeDefaultFont);

        foreach (string font in fontConfig.CustomFonts)
            LoadFont(font);

        // Load screen size
        InternalStartupData startupData = _configuration.Configs.OfType<InternalStartupData>().FirstOrDefault()
            ?? throw new Exception($"You must call {nameof(Configuration.Extensions.SetScreenSize)} to set a default screen size.");

        ScreenCellsX = startupData.ScreenCellsX;
        ScreenCellsY = startupData.ScreenCellsY;

        MonoGameInstance.ResizeGraphicsDeviceManager(DefaultFont.GetFontSize(DefaultFontSize).ToMonoPoint(), ScreenCellsX, ScreenCellsY, 0, 0);

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

        // Load special FPS visual
        FpsConfig? fpsConfig = _configuration.Configs.OfType<FpsConfig>().FirstOrDefault();
        if (fpsConfig != null && fpsConfig.ShowFPSVisual)
            Instance.MonoGameInstance.Components.Add(new Host.Game.FPSCounterComponent(Instance.MonoGameInstance));

        // Run all startup config objects
        _configuration.Run(this);

        // Normal start
        OnGameStarted();
        _configuration = null;
        SplashScreens.SplashScreenManager.CheckRun();
    }

    /// <inheritdoc/>
    public override void Run()
    {
        MonoGameInstance.Run();
        OnGameEnding();
        MonoGameInstance.Dispose();
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

    /// <summary>
    /// Toggles between windowed and full screen rendering for SadConsole.
    /// </summary>
    public void ToggleFullScreen()
    {
        Host.Global.GraphicsDeviceManager.ApplyChanges();

        // Coming back from full screen
        if (Host.Global.GraphicsDeviceManager.IsFullScreen)
        {
            Host.Global.GraphicsDeviceManager.IsFullScreen = !Host.Global.GraphicsDeviceManager.IsFullScreen;

            Host.Global.GraphicsDeviceManager.PreferredBackBufferWidth = _preFullScreenWidth;
            Host.Global.GraphicsDeviceManager.PreferredBackBufferHeight = _preFullScreenHeight;
            Host.Global.GraphicsDeviceManager.ApplyChanges();
        }

        // Going full screen
        else
        {
            _preFullScreenWidth = Host.Global.GraphicsDevice.PresentationParameters.BackBufferWidth;
            _preFullScreenHeight = Host.Global.GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (Settings.ResizeMode == Settings.WindowResizeOptions.None)
            {
                _handleResizeNone = true;
                Settings.ResizeMode = Settings.WindowResizeOptions.Scale;
            }

            Host.Global.GraphicsDeviceManager.PreferredBackBufferWidth = Host.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            Host.Global.GraphicsDeviceManager.PreferredBackBufferHeight = Host.Global.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

            Host.Global.GraphicsDeviceManager.IsFullScreen = !Host.Global.GraphicsDeviceManager.IsFullScreen;
            Host.Global.GraphicsDeviceManager.ApplyChanges();

            if (_handleResizeNone)
            {
                _handleResizeNone = false;
                Settings.ResizeMode = Settings.WindowResizeOptions.None;
            }
        }

        Instance.MonoGameInstance.ResetRendering();
    }
    
    /// <inheritdoc/>
    public override void ResizeWindow(int width, int height)
    {
        Host.Global.GraphicsDeviceManager.PreferredBackBufferWidth = width;
        Host.Global.GraphicsDeviceManager.PreferredBackBufferHeight = height;
        Host.Global.GraphicsDeviceManager.ApplyChanges();

        Instance.MonoGameInstance.ResetRendering();
    }

    internal void InvokeFrameDraw() =>
        OnFrameRender();

    internal void InvokeFrameUpdate() =>
        OnFrameUpdate();

    internal void SetStartingConsole(Console? console) =>
        StartingConsole = console;
}
