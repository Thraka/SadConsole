using System.Diagnostics;
using System.Reflection;
using Raylib_cs;
using SadConsole;
using SadConsole.Configuration;
using SadRogue.Primitives;
using SkiaGameRendering.Raylib.OGL;
using SkiaSharp;
using SadColor = SadRogue.Primitives.Color;
using SadRectangle = SadRogue.Primitives.Rectangle;

internal static class Program
{
    private const int ViewportWidth = 1080;
    private const int ViewportHeight = 480;
    private static Builder _startup = null!;

    private static void Main()
    {
        Settings.WindowTitle = "My SadConsole Game";

        _startup = Builder
            .GetBuilder()
            .SetWindowSizeInCells(90, 30)
            .ConfigureFonts(true)
            .UseDefaultConsole()
            .OnStart(Startup);

        Raylib.InitWindow(ViewportWidth, ViewportHeight, Settings.WindowTitle);
        Raylib.SetTargetFPS(60);
        SkiaRaylibRenderer.Initialize();

        var canvas = new SkiaRaylibRenderTarget2D(ViewportWidth, ViewportHeight);
        bool gameCreated = false;

        try
        {
            canvas.Begin();
            try
            {
                Game.Create(_startup, GetGraphicsContext());
                gameCreated = true;
                Game.Instance.Run();
            }
            finally
            {
                canvas.EndWithoutDrawing();
            }

            var clock = Stopwatch.StartNew();

            while (!Raylib.WindowShouldClose())
            {
                TimeSpan elapsed = clock.Elapsed;
                clock.Restart();

                Raylib.BeginDrawing();
                canvas.Begin();
                Game.Instance.Update(elapsed);
                Game.Instance.Render(canvas.Canvas, SKRect.Create(ViewportWidth, ViewportHeight), elapsed);
                canvas.End();
                Raylib.EndDrawing();
            }
        }
        finally
        {
            if (gameCreated)
            {
                canvas.Begin(clear: false);
                try
                {
                    Game.Instance.Stop();
                    Game.Instance.Dispose();
                }
                finally
                {
                    canvas.EndWithoutDrawing();
                }
            }

            canvas.Dispose();
            SkiaRaylibRenderer.Dispose();
            Raylib.CloseWindow();
        }
    }

    private static GRContext GetGraphicsContext()
    {
        FieldInfo contextField = typeof(SkiaRaylibRenderer).GetField("_context", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("The Raylib Skia renderer does not expose its graphics context.");
        object context = contextField.GetValue(null)
            ?? throw new InvalidOperationException("The Raylib Skia renderer has not been initialized.");
        PropertyInfo graphicsContextProperty = context.GetType().GetProperty("GrContext", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("The Raylib Skia renderer's graphics context is unavailable.");

        return (GRContext)(graphicsContextProperty.GetValue(context)
            ?? throw new InvalidOperationException("The Raylib Skia renderer's graphics context is unavailable."));
    }

    private static void Startup(object? sender, GameHost host)
    {
        Game.Instance.StartingConsole!.SadComponents.Add(new SadConsole.Instructions.CodeInstruction((obj, time) =>
        {
            Game.Instance.StartingConsole!.FillWithRandomGarbage(Game.Instance.StartingConsole.Font);
            Game.Instance.StartingConsole.Fill(new SadRectangle(3, 3, 23, 3), SadColor.Violet, SadColor.Black, 0, Mirror.None);
            Game.Instance.StartingConsole.Print(4, 4, "Hello from SadConsole");
            return false;
        }) { RemoveOnFinished = true });
    }
}
