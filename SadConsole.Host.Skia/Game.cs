using System;
using System.IO;
using System.Linq;
using SadConsole.Configuration;
using SadConsole.Host;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;
using SkiaSharp;
using Color = SadRogue.Primitives.Color;

namespace SadConsole;

/// <summary>
/// A client-driven SadConsole host that renders through SkiaSharp.
/// </summary>
public sealed class Game : GameHost
{
    private Builder? _configuration;
    private readonly Host.Keyboard _keyboardState = new();
    private readonly Host.Mouse _mouseState = new();
    private bool _isRunning;
    private int _viewportWidth;
    private int _viewportHeight;

    /// <summary>
    /// Raised when the embedding client should toggle fullscreen mode.
    /// </summary>
    public event EventHandler? FullscreenRequested;

    /// <summary>
    /// Raised when the embedding client should resize its viewport.
    /// </summary>
    public event EventHandler<ViewportSizeChangedEventArgs>? ViewportSizeRequested;

    /// <summary>
    /// Gets the strongly typed game instance.
    /// </summary>
    public static new Game Instance
    {
        get => (Game)GameHost.Instance;
        private set => GameHost.Instance = value;
    }

    /// <summary>
    /// Gets the keyboard state populated by the embedding client.
    /// </summary>
    public Host.Keyboard KeyboardState => _keyboardState;

    /// <summary>
    /// Gets the mouse state populated by the embedding client.
    /// </summary>
    public Host.Mouse MouseState => _mouseState;

    /// <summary>
    /// Gets whether this host has started and accepts frames.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Gets the client-owned OpenGL context used by this host.
    /// </summary>
    public GRContext GraphicsContext => Host.Global.RequireDevice().Context;

    /// <summary>
    /// Gets or sets whether SadConsole should process client-provided input.
    /// </summary>
    public bool IsInputEnabled { get; set; } = true;

    private Game() { }

    /// <summary>
    /// Creates a game using the built-in font and default console.
    /// </summary>
    public static void Create(int cellCountX, int cellCountY, GRContext graphicsContext) =>
        Create(new Builder()
            .SetWindowSizeInCells(cellCountX, cellCountY)
            .UseDefaultConsole()
            .IsStartingScreenFocused(true)
            .ConfigureFonts(), graphicsContext);

    /// <summary>
    /// Creates a game using the built-in font and default console.
    /// </summary>
    public static void Create(int cellCountX, int cellCountY, EventHandler<GameHost> gameStarted, GRContext graphicsContext) =>
        Create(new Builder()
            .SetWindowSizeInCells(cellCountX, cellCountY)
            .UseDefaultConsole()
            .IsStartingScreenFocused(true)
            .ConfigureFonts()
            .OnStart(gameStarted), graphicsContext);

    /// <summary>
    /// Creates a game with a custom default font.
    /// </summary>
    public static void Create(int cellCountX, int cellCountY, string font, EventHandler<GameHost> gameStarted, GRContext graphicsContext) =>
        Create(new Builder()
            .SetWindowSizeInCells(cellCountX, cellCountY)
            .UseDefaultConsole()
            .IsStartingScreenFocused(true)
            .ConfigureFonts(font)
            .OnStart(gameStarted), graphicsContext);

    /// <summary>
    /// Creates and initializes a client-driven game.
    /// </summary>
    public static void Create(Builder configuration, GRContext graphicsContext)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(graphicsContext);

        if (GameHost.Instance is not null)
            throw new InvalidOperationException("The game has already been created.");

        var device = new Host.GpuDevice(graphicsContext);
        var game = new Game { _configuration = configuration };
        Instance = game;
        Host.Global.Device = device;

        try
        {
            game.Initialize();
        }
        catch
        {
            Host.Global.Device = null;
            Instance = null!;
            throw;
        }
    }

    /// <inheritdoc />
    public override void Run()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        OnGameStarted();
        _configuration = null;
        SplashScreens.SplashScreenManager.CheckRun();
        Keyboard.Update(TimeSpan.Zero);
        Mouse.Update(TimeSpan.Zero);
    }

    /// <summary>
    /// Advances SadConsole by one client frame.
    /// </summary>
    public void Update(TimeSpan elapsed)
    {
        if (!_isRunning || !Settings.DoUpdate)
            return;

        UpdateFrameDelta = elapsed;

        foreach (Components.RootComponent component in RootComponents)
            component.Run(elapsed);

        if (IsInputEnabled)
        {
            if (Settings.Input.DoKeyboard)
            {
                Keyboard.Update(elapsed);
                if (FocusedScreenObjects.ScreenObject is { UseKeyboard: true } focused)
                    focused.ProcessKeyboard(Keyboard);
            }

            if (Settings.Input.DoMouse)
            {
                Mouse.Update(elapsed);
                Mouse.Process();
            }
        }

        Screen?.Update(elapsed);
        OnFrameUpdate();
    }

    /// <summary>
    /// Renders one SadConsole frame into the supplied canvas at the logical render size.
    /// </summary>
    public void Render(SKCanvas canvas, TimeSpan elapsed) =>
        Render(canvas, SKRect.Create(Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), elapsed);

    /// <summary>
    /// Renders one SadConsole frame into a destination rectangle supplied by the client.
    /// </summary>
    public void Render(SKCanvas canvas, SKRect destination, TimeSpan elapsed)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!_isRunning || !Settings.DoDraw || destination.Width <= 0 || destination.Height <= 0)
            return;

        Host.Global.RequireDevice().ValidateCanvas(canvas);

        DrawFrameDelta = elapsed;
        DrawCalls.Clear();
        Screen?.Render(elapsed);
        OnFrameRender();

        if (!Settings.DoFinalDraw)
            return;

        canvas.Save();
        canvas.ClipRect(destination);

        Global.SKPaint_Src.Color = Host.GameTexture.ToSkColor(Settings.ClearColor);

        canvas.DrawRect(destination, Global.SKPaint_SrcOver);

        canvas.Translate(destination.Left, destination.Top);
        canvas.Scale(destination.Width / Settings.Rendering.RenderWidth,
                     destination.Height / Settings.Rendering.RenderHeight);

        Host.Global.Canvas = canvas;
        try
        {
            foreach (DrawCalls.IDrawCall drawCall in DrawCalls)
                drawCall.Draw();
        }
        finally
        {
            Host.Global.Canvas = null;
            canvas.Restore();
        }
    }

    /// <inheritdoc />
    public override void Stop()
    {
        if (!_isRunning)
            return;

        _isRunning = false;
        OnGameEnding();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        try
        {
            base.Dispose(disposing);
        }
        finally
        {
            if (disposing)
            {
                Host.Global.Canvas = null;
                Host.Global.Device = null;
            }
        }
    }

    /// <inheritdoc />
    public override ITexture GetTexture(string resourcePath) => new Host.GameTexture(resourcePath);

    /// <inheritdoc />
    public override ITexture GetTexture(Stream textureStream) => new Host.GameTexture(textureStream);

    /// <inheritdoc />
    public override ITexture CreateTexture(int width, int height) => new Host.GameTexture(width, height);

    /// <inheritdoc />
    public override IKeyboardState GetKeyboardState() => _keyboardState;

    /// <inheritdoc />
    public override IMouseState GetMouseState() => _mouseState;

    /// <inheritdoc />
    public override void GetDeviceScreenSize(out int width, out int height)
    {
        width = _viewportWidth;
        height = _viewportHeight;
    }

    /// <inheritdoc />
    public override Stream OpenStream(string file, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read) =>
        File.Open(file, mode, access);

    /// <inheritdoc />
    public override void ToggleFullScreen() => FullscreenRequested?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
    public override void ResizeWindow(int width, int height, bool resizeOutputSurface = false)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        _viewportWidth = width;
        _viewportHeight = height;

        if (resizeOutputSurface)
        {
            Settings.Rendering.RenderWidth = width;
            Settings.Rendering.RenderHeight = height;
        }

        Settings.Rendering.RenderRect = new Rectangle(0, 0, width, height);
        ViewportSizeRequested?.Invoke(this, new ViewportSizeChangedEventArgs(width, height));
    }

    private void Initialize()
    {
        Builder configuration = _configuration ?? throw new InvalidOperationException("Configuration must be set.");

        FontConfig fontConfig = configuration.Configs.OfType<FontConfig>().FirstOrDefault() ?? new FontConfig();
        configuration.Configs.Remove(fontConfig);
        fontConfig.Run(configuration, this);
        LoadDefaultFonts(fontConfig.AlternativeDefaultFont);
        DefaultFontSize = fontConfig.DefaultFontSize;

        foreach (string font in fontConfig.CustomFonts)
            LoadFont(font);

        ConfigureWindowConfig windowConfig = configuration.Configs.OfType<ConfigureWindowConfig>().FirstOrDefault()
            ?? throw new InvalidOperationException("The starting window or screen hasn't been configured.");
        configuration.Configs.Remove(windowConfig);
        ((IConfigurator)windowConfig).Run(configuration, this);

        Settings.Rendering.RenderWidth = windowConfig.GameResolutionWidthInPixels;
        Settings.Rendering.RenderHeight = windowConfig.GameResolutionHeightInPixels;
        _viewportWidth = windowConfig.WindowWidthInPixels;
        _viewportHeight = windowConfig.WindowHeightInPixels;
        Settings.Rendering.RenderRect = new Rectangle(0, 0, _viewportWidth, _viewportHeight);

        SetRenderer(Renderers.Constants.RendererNames.Default, typeof(Renderers.ScreenSurfaceRenderer));
        SetRenderer(Renderers.Constants.RendererNames.ScreenSurface, typeof(Renderers.ScreenSurfaceRenderer));
        SetRenderer(Renderers.Constants.RendererNames.OptimizedScreenSurface, typeof(Renderers.ScreenSurfaceRenderer));

        SetRendererStep(Renderers.Constants.RenderStepNames.Surface, typeof(Renderers.SurfaceRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Cursor, typeof(Renderers.CursorRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Output, typeof(Renderers.OutputSurfaceRenderStep));
        SetRendererStep(Renderers.Constants.RenderStepNames.Tint, typeof(Renderers.TintSurfaceRenderStep));

        if (Settings.AutomaticAddColorsToMappings)
            LoadMappedColors();

        configuration.ProcessConfigs(this);
    }
}

/// <summary>
/// Provides a client with a requested viewport size.
/// </summary>
public sealed class ViewportSizeChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the requested width in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the requested height in pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Creates viewport size event data.
    /// </summary>
    public ViewportSizeChangedEventArgs(int width, int height)
    {
        Width = width;
        Height = height;
    }
}
