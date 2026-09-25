using SadConsole.Host;
using SadRogue.Primitives;
using SkiaSharp;
using Color = SadRogue.Primitives.Color;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace SadConsole.Renderers;

internal static class SkiaRendererHelpers
{
    internal static SKPaint PaintObj = new() { BlendMode = SKBlendMode.SrcOver  };

    private static bool init = false;

    internal static void DrawTexture(SKCanvas canvas, GameTexture texture, SKRect destination, Color tint)
    {
        using var paint = new SKPaint
        {
            BlendMode = SKBlendMode.SrcOver,
            ColorFilter = tint == Color.White
                ? null
                : SKColorFilter.CreateBlendMode(GameTexture.ToSkColor(tint), SKBlendMode.Modulate)
        };
        canvas.DrawImage(texture.Image, destination, Host.Global.Sampling, paint);
    }

    internal static void DrawGlyph(SKCanvas canvas, GameTexture fontTexture, Rectangle sourceRectangle,
                                   SKRect destination, Color color, Mirror mirror)
    {
        if (color.A == 0)
            return;

        SKRect source = SKRect.Create(sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height);
        var paint = Global.SKPaint_SrcOver;

        //paint.Color = GameTexture.ToSkColor(color);
        paint.ColorFilter = SKColorFilter.CreateBlendMode(GameTexture.ToSkColor(color), SKBlendMode.SrcIn);

        canvas.Save();

        float scaleX = (mirror & Mirror.Horizontal) != 0 ? -1f : 1f;
        float scaleY = (mirror & Mirror.Vertical) != 0 ? -1f : 1f;
        if (scaleX < 0 || scaleY < 0)
        {
            canvas.Translate(scaleX < 0 ? destination.Left + destination.Right : 0,
                             scaleY < 0 ? destination.Top + destination.Bottom : 0);
            canvas.Scale(scaleX, scaleY);
        }
        canvas.DrawImage(fontTexture.Image, source, destination, Host.Global.Sampling, paint);
        paint.ColorFilter.Dispose();
        paint.ColorFilter = null;
        canvas.Restore();
    }
}
