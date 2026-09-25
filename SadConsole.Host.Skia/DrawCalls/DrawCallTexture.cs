using System;
using System.Data;
using SadConsole.Host;
using SadRogue.Primitives;
using SkiaSharp;
using Color = SadRogue.Primitives.Color;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws a GPU-backed Skia image into the active client-provided canvas.
/// </summary>
public sealed class DrawCallTexture : IDrawCall
{
    /// <summary>
    /// The texture to draw.
    /// </summary>
    public GameTexture Texture { get; }

    /// <summary>
    /// The destination position in logical render pixels.
    /// </summary>
    public Point Position { get; }

    /// <summary>
    /// The color applied to the texture.
    /// </summary>
    public Color Tint { get; }

    /// <summary>
    /// Creates a texture draw call.
    /// </summary>
    public DrawCallTexture(GameTexture texture, Point position, Color? tint = null)
    {
        Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        Position = position;
        Tint = tint ?? Color.White;
    }

    /// <inheritdoc />
    public void Draw()
    {
        SKCanvas canvas = Global.Canvas ?? throw new InvalidOperationException("No Skia canvas is active.");
        SKRect destination = SKRect.Create(Position.X, Position.Y, Texture.Width, Texture.Height);

        Global.SKPaint_SrcOver.Color = GameTexture.ToSkColor(Tint);

        if (Tint.A != 255)
        {
            Global.SKPaint_SrcOver.ColorFilter = SKColorFilter.CreateBlendMode(GameTexture.ToSkColor(Tint), SKBlendMode.Modulate);
            canvas.DrawImage(Texture.Image, destination, Global.Sampling, Global.SKPaint_SrcOver);
            Global.SKPaint_SrcOver.ColorFilter.Dispose();
            Global.SKPaint_SrcOver.ColorFilter = null;
        }
        else
        {
            canvas.DrawImage(Texture.Image, destination, Global.Sampling, Global.SKPaint_SrcOver);

        }

    }
}
