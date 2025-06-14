
using Raylib_cs;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws an image using the host drawing system.
/// </summary>
public class DrawCallTexture : IDrawCall
{
    /// <summary>
    /// The image to draw.
    /// </summary>
    public Texture2D Texture;

    /// <summary>
    /// Where to draw the texture.
    /// </summary>
    public SadRogue.Primitives.Point Position;

    /// <summary>
    /// A color tint to apply when drawn.
    /// </summary>
    public Color Tint;

    /// <summary>
    /// Creates a new instance of this draw call.
    /// </summary>
    /// <param name="texture">The image to draw.</param>
    /// <param name="position">The position to draw the image.</param>
    /// <param name="tint">A color tint to apply to the drawn image.</param>
    public DrawCallTexture(Texture2D texture, SadRogue.Primitives.Point position, Color? tint = null)
    {
        Texture = texture;
        Position = position;

        if (tint.HasValue)
            Tint = tint.Value;
        else
            Tint = Color.White;
    }

    /// <inheritdoc/>
    public void Draw() =>
        Raylib.DrawTexture(Texture, Position.X, Position.Y, Tint);
}
