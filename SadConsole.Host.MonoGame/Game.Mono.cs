#nullable enable

using System;
using System.IO;
using System.Transactions;
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
    internal Configuration? _configuration;

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

    /// <summary>
    /// When <see langword="true"/>, forces the <see cref="OpenStream"/> method to use <code>TitalContainer</code> when creating a stream to read a file.
    /// </summary>
    public bool UseTitleContainer { get; set; } = true;

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
        protected set => GameHost.Instance = value;
    }

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
    /// Creates a new game and assigns it to the <see cref="MonoGameInstance"/> property.
    /// </summary>
    /// <param name="configuration">The settings used in creating the game.</param>
    public static void Create(Configuration configuration)
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

        OnStart = _configuration.OnStartCallback;
        OnEnd = _configuration.OnEndCallback;

        // Configure the fonts
        _configuration.RunFontConfig();
        LoadDefaultFonts(_configuration.FontLoaderData.AlternativeDefaultFont);

        foreach (var font in _configuration.FontLoaderData.CustomFonts)
            LoadFont(font);

        // Load screen size
        ScreenCellsX = _configuration.ScreenSizeWidth;
        ScreenCellsY = _configuration.ScreenSizeHeight;

        MonoGameInstance.ResizeGraphicsDeviceManager(DefaultFont.GetFontSize(DefaultFontSize).ToMonoPoint(), ScreenCellsX, ScreenCellsY, 0, 0);

        // Setup renderers
        SetRenderer(Renderers.Constants.RendererNames.Default, typeof(Renderers.ScreenSurfaceRenderer));

        SetRendererStep(Renderers.Constants.RenderStepNames.ControlHost, typeof(Renderers.ControlHostRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Cursor, typeof(Renderers.CursorRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.EntityRenderer, typeof(Renderers.EntityLiteRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Output, typeof(Renderers.OutputSurfaceRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Surface, typeof(Renderers.SurfaceRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.SurfaceDirtyCells, typeof(Renderers.SurfaceDirtyCellsRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Tint, typeof(Renderers.TintSurfaceRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Window, typeof(Renderers.WindowRenderStep));

        // Load the mapped colors
        LoadMappedColors();

        // Hook up any events via config
        if (_configuration.event_FrameUpdate != null)
            FrameUpdate += _configuration.event_FrameUpdate;

        if (_configuration.event_FrameRender != null)
            FrameRender += _configuration.event_FrameRender;

        // Setup default starting console, otherwise, use the config starting object.
        if (Settings.CreateStartingConsole)
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
            }
            catch (NullReferenceException e)
            {
                throw new NullReferenceException("'Settings.CreateStartingConsole' is false; configuration 'SetStartingScreen' method must return a valid object.");
            }
        }

        if (_configuration.UseUnlimitedFPSVisual)
            Instance.MonoGameInstance.Components.Add(new Host.Game.FPSCounterComponent(Instance.MonoGameInstance));

        // Kill off config instance
        _configuration = null;

        OnStart?.Invoke();
        SplashScreens.SplashScreenManager.CheckRun();
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

        return UseTitleContainer ? Microsoft.Xna.Framework.TitleContainer.OpenStream(file) : File.OpenRead(file);
    }

    /// <summary>
    /// Toggles between windowed and fullscreen rendering for SadConsole.
    /// </summary>
    public void ToggleFullScreen()
    {
        Host.Global.GraphicsDeviceManager.ApplyChanges();

        // Coming back from fullscreen
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
}
#nullable disable
