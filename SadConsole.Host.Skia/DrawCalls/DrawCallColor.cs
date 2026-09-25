using System;
using SadConsole.Host;
using SadRogue.Primitives;
using SkiaSharp;
using Color = SadRogue.Primitives.Color;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws a solid color rectangle into the active Skia canvas.
/// </summary>
public sealed class DrawCallColor : IDrawCall
{
    /// <summary>
    /// Gets the color to draw.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Gets the destination rectangle.
    /// </summary>
    public Rectangle Area { get; }

    /// <summary>
    /// Creates a color draw call.
    /// </summary>
    public DrawCallColor(Color color, Rectangle area)
    {
        Color = color;
        Area = area;
    }

    /// <inheritdoc />
    public void Draw()
    {
        SKCanvas canvas = Global.Canvas ?? throw new InvalidOperationException("No Skia canvas is active.");
        Global.SKPaint_SrcOver.Color = GameTexture.ToSkColor(Color);

        canvas.DrawRect(SKRect.Create(Area.X, Area.Y, Area.Width, Area.Height), Global.SKPaint_SrcOver);
    }
}
