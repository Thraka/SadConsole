using System.Numerics;
using Raylib_cs;
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws a colored rectangle with the host's rendering system.
/// </summary>
public class DrawCallColor : IDrawCall
{
    /// <summary>
    /// The texture, most likely a SadConsole font texture, containing the solid white rectangle referenced by <see cref="FontSolidRect"/>.
    /// </summary>
    public Texture2D FontTexture;

    /// <summary>
    /// The solid white glyph rectangle from <see cref="FontTexture"/> used for shading.
    /// </summary>
    public Rectangle FontSolidRect;

    /// <summary>
    /// The color of the target rectangle.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Where the glyph should be drawn.
    /// </summary>
    public Rectangle TargetRect;

    /// <summary>
    /// Creates a new instance of this draw call.
    /// </summary>
    /// <param name="color">The color of the rectangle.</param>
    /// <param name="texture">The texture containing a solid white rectangle referenced by <paramref name="fontSolidRect"/>.</param>
    /// <param name="targetRect">The drawing location of the rectangle.</param>
    /// <param name="fontSolidRect">The rectangle of the solid white glyph in the <paramref name="texture"/>.</param>
    public DrawCallColor(Color color, Texture2D texture, Rectangle targetRect, Rectangle fontSolidRect)
    {
        FontTexture = texture;
        FontSolidRect = fontSolidRect;
        TargetRect = targetRect;
        Color = color;
    }

    /// <inheritdoc/>
    public void Draw()
    {
        Raylib.DrawTexturePro(FontTexture, FontSolidRect, TargetRect, Vector2.Zero, 0f, Color);
    }
}
