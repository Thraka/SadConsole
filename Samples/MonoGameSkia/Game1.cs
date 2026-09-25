using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Configuration;
using SadRogue.Primitives;
using SkiaGameRendering;
using SkiaSharp;
using SadColor = SadRogue.Primitives.Color;
using SadGame = SadConsole.Game;
using SadRectangle = SadRogue.Primitives.Rectangle;

namespace MonoGameSkia;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private Builder _startup;
    private SkiaRenderTarget2D? _canvas;
    private bool _isSadConsoleInitialized;
    private TimeSpan _fpsElapsed;
    private int _framesSinceFpsUpdate;

    public Game1()
    {
        SadConsole.Settings.WindowTitle = "My SadConsole Game";

        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = SadConsole.Settings.WindowTitle;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 900;
        _graphics.PreferredBackBufferHeight = 480;
        _graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        _startup = Builder
            .GetBuilder()
            .SetWindowSizeInCells(90, 30)
            .ConfigureFonts(true)
            .UseDefaultConsole()
            .OnStart(Startup);


        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_isSadConsoleInitialized)
            SadGame.Instance.Update(gameTime.ElapsedGameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (!SkiaRenderer.IsInitialized && SkiaRenderer.IsReady)
            InitializeSadConsole();

        if (_isSadConsoleInitialized && _canvas is not null)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            UpdateFpsCounter(gameTime.ElapsedGameTime);

            _canvas.Begin();
            SadGame.Instance.Render(
                _canvas.Canvas,
                SKRect.Create(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                gameTime.ElapsedGameTime);
            _canvas.End();
        }

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_isSadConsoleInitialized && _canvas is not null)
            {
                _canvas.Begin(clear: false);
                try
                {
                    SadGame.Instance.Stop();
                    SadGame.Instance.Dispose();
                }
                finally
                {
                    _canvas.EndWithoutDrawing();
                }
            }

            _canvas?.Dispose();

            if (SkiaRenderer.IsInitialized)
                SkiaRenderer.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeSadConsole()
    {
        SkiaRenderer.Initialize(GraphicsDevice);
        _canvas = new SkiaRenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

        _canvas.Begin();
        try
        {
            SadGame.Create(_startup, SkiaRenderer.CurrentBackend.GRContext);
            _isSadConsoleInitialized = true;
            SadGame.Instance.Run();
        }
        finally
        {
            _canvas.EndWithoutDrawing();
        }
    }

    private void UpdateFpsCounter(TimeSpan elapsed)
    {
        _fpsElapsed += elapsed;
        _framesSinceFpsUpdate++;

        if (_fpsElapsed < TimeSpan.FromMilliseconds(500))
            return;

        double framesPerSecond = _framesSinceFpsUpdate / _fpsElapsed.TotalSeconds;
        _fpsSurface.Print(0, 0, $"{framesPerSecond,6:0.0} FPS   ", SadColor.Yellow, SadColor.Black);

        _fpsElapsed = TimeSpan.Zero;
        _framesSinceFpsUpdate = 0;
    }

    private static SadConsole.ScreenSurface _fpsSurface;

    private static void Startup(object? sender, SadConsole.GameHost host)
    {
        _fpsSurface = new ScreenSurface(10, 1);
        SadGame.Instance.StartingConsole!.Children.Add(_fpsSurface);

        SadGame.Instance.StartingConsole!.FillWithRandomGarbage(SadGame.Instance.StartingConsole.Font);
        SadGame.Instance.StartingConsole.Fill(new SadRectangle(3, 3, 23, 3), SadColor.Violet, SadColor.Black, 0, Mirror.None);
        SadGame.Instance.StartingConsole.Print(4, 4, "Hello from SadConsole");

        //SadGame.Instance.StartingConsole!.SadComponents.Add(new SadConsole.Instructions.CodeInstruction((obj, time) =>
        //{
        //    SadGame.Instance.StartingConsole!.FillWithRandomGarbage(SadGame.Instance.StartingConsole.Font);
        //    SadGame.Instance.StartingConsole.Fill(new SadRectangle(3, 3, 23, 3), SadColor.Violet, SadColor.Black, 0, Mirror.None);
        //    SadGame.Instance.StartingConsole.Print(4, 4, "Hello from SadConsole");
        //    return false;
        //}) { RemoveOnFinished = true });
    }
}
