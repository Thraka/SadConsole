#nullable enable
using System;
using System.IO;
using SFML.Graphics;
using SadRogue.Primitives;
using SadConsole.Host;
using SadConsole.Configuration;
using System.Linq;

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
                .SetScreenSize(cellCountX, cellCountY)
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
                .SetScreenSize(cellCountX, cellCountY)
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
                .SetScreenSize(cellCountX, cellCountY)
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

        foreach (string font in fontConfig.CustomFonts)
            LoadFont(font);

        // Load screen size
        InternalStartupData startupData = _configuration.Configs.OfType<InternalStartupData>().FirstOrDefault()
            ?? throw new Exception($"You must call {nameof(Configuration.Extensions.SetScreenSize)} to set a default screen size.");

        ScreenCellsX = startupData.ScreenCellsX;
        ScreenCellsY = startupData.ScreenCellsY;

        if (startupData.TargetWindow == null)
        {
            Global.GraphicsDevice = new RenderWindow(new SFML.Window.VideoMode((uint)(DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize).X * ScreenCellsX), (uint)(DefaultFont.GetFontSize(DefaultFontSize).Y * ScreenCellsY)), Host.Settings.WindowTitle, SFML.Window.Styles.Titlebar | SFML.Window.Styles.Close | SFML.Window.Styles.Resize);
            Global.GraphicsDevice.SetTitle(Settings.WindowTitle);
        }
        else
            Global.GraphicsDevice = startupData.TargetWindow;

        Global.GraphicsDevice.Closed += (o, e) =>
        {
            ((SFML.Window.Window)o!).Close();
        };

        Global.GraphicsDevice.Resized += (o, e) =>
        {
            ResetRendering();
        };

        // Load FPS
        FpsConfig? fpsConfig = _configuration.Configs.OfType<FpsConfig>().FirstOrDefault();
        if (fpsConfig != null && fpsConfig.UnlimitedFPS)
            if (Host.Settings.FPS > 0)
                Global.GraphicsDevice.SetFramerateLimit((uint)Host.Settings.FPS);

        Global.SharedSpriteBatch = new SpriteBatch();

        //SadConsole.Host.Global.RenderStates.BlendMode = BlendMode.Multiply

        // Setup rendering sizes
        Settings.Rendering.RenderWidth = (int)Global.GraphicsDevice.Size.X;
        Settings.Rendering.RenderHeight = (int)Global.GraphicsDevice.Size.Y;
        ResetRendering();

        // Configure input
        _keyboard = new Keyboard(Global.GraphicsDevice);
        _mouse = new Mouse(Global.GraphicsDevice);

        // Create game loop timers
        Global.UpdateTimer = new SFML.System.Clock();
        Global.DrawTimer = new SFML.System.Clock();

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
        if (Settings.AutomaticAddColorsToMappings)
            LoadMappedColors();

        // Run all startup config objects, then destroy the config instance
        _configuration.Run(this);

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

        while (Global.GraphicsDevice.IsOpen)
        {
            // Update game loop part
            if (Settings.DoUpdate)
            {
                UpdateFrameDelta = TimeSpan.FromSeconds(Global.UpdateTimer.ElapsedTime.AsSeconds());

                if (Global.GraphicsDevice.HasFocus() && !Global.BlockSadConsoleInput)
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

                Global.UpdateTimer.Restart();
            }

            // Draw game loop part
            if (Settings.DoDraw)
            {
                Global.GraphicsDevice.Clear(Settings.ClearColor.ToSFMLColor());

                DrawFrameDelta = TimeSpan.FromSeconds(Global.DrawTimer.ElapsedTime.AsSeconds());

                // Clear draw calls for next run
                Instance.DrawCalls.Clear();

                // Make sure all items in the screen are drawn. (Build a list of draw calls)
                Screen?.Render(DrawFrameDelta);

                ((SadConsole.Game)Instance).InvokeFrameDraw();

                // Render to the global output texture
                Global.RenderOutput.Clear(Settings.ClearColor.ToSFMLColor());

                // Render each draw call
                Global.SharedSpriteBatch.Reset(Global.RenderOutput, Host.Settings.SFMLScreenBlendMode, Transform.Identity);

                foreach (DrawCalls.IDrawCall call in Instance.DrawCalls)
                    call.Draw();

                Global.SharedSpriteBatch.End();
                Global.RenderOutput.Display();

                // If we're going to draw to the screen, do it.
                if (Settings.DoFinalDraw)
                {
                    Global.SharedSpriteBatch.Reset(Global.GraphicsDevice, Host.Settings.SFMLScreenBlendMode, Transform.Identity, Host.Settings.SFMLScreenShader);
                    Global.SharedSpriteBatch.DrawQuad(Settings.Rendering.RenderRect.ToIntRect(), new IntRect(0, 0, (int)Global.RenderOutput.Size.X, (int)Global.RenderOutput.Size.Y), SFML.Graphics.Color.White, Global.RenderOutput.Texture);
                    Global.SharedSpriteBatch.End();
                }
            }

            Global.GraphicsDevice.Display();
            Global.GraphicsDevice.DispatchEvents();

            Global.DrawTimer.Restart();
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
    public override void ResizeWindow(int width, int height, bool resizeOutputSurface = false)
    {
        Global.GraphicsDevice.Size = new SFML.System.Vector2u((uint)width, (uint)height);

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
    private void RecreateRenderOutput(uint width, uint height)
    {
        if (Global.RenderOutput == null || Global.RenderOutput.Size.X != width || Global.RenderOutput.Size.Y != height)
        {
            Global.RenderOutput?.Dispose();
            Global.RenderOutput = new RenderTexture(width, height);
        }
    }

    /// <summary>
    /// Resets the <see cref="Host.Global.RenderOutput"/> target and determines the appropriate <see cref="SadConsole.Settings.Rendering.RenderRect"/> and <see cref="SadConsole.Settings.Rendering.RenderScale"/> based on the window or fullscreen state.
    /// </summary>
    public void ResetRendering()
    {
        if (Settings.ResizeMode == Settings.WindowResizeOptions.Center)
        {
            RecreateRenderOutput((uint)Settings.Rendering.RenderWidth, (uint)Settings.Rendering.RenderHeight);

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
            RecreateRenderOutput((uint)Settings.Rendering.RenderWidth, (uint)Settings.Rendering.RenderHeight);

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
            RecreateRenderOutput((uint)Settings.Rendering.RenderWidth, (uint)Settings.Rendering.RenderHeight);

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
            RecreateRenderOutput((uint)Settings.Rendering.RenderWidth, (uint)Settings.Rendering.RenderHeight);
            Settings.Rendering.RenderRect = new Rectangle(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight);
            Settings.Rendering.RenderScale = (1, 1);
            Global.GraphicsDevice.SetView(new View(new FloatRect(0, 0, Global.GraphicsDevice.Size.X, Global.GraphicsDevice.Size.Y)));
        }
        else
        {
            RecreateRenderOutput((uint)Settings.Rendering.RenderWidth, (uint)Settings.Rendering.RenderHeight);
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
