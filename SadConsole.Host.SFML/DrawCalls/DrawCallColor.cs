using Rectangle = SFML.Graphics.IntRect;
using Color = SFML.Graphics.Color;
using SFML.Graphics;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws a colored rectangle to the active <see cref="Host.Global.SharedSpriteBatch"/>.
/// </summary>
public class DrawCallColor : IDrawCall
{
    /// <summary>
    /// The texture, most likely a SadConsole font texture, containing the solid white rectangle referenced by <see cref="FontSolidRect"/>.
    /// </summary>
    public Texture FontTexture;

    /// <summary>
    /// The solid white glyph rectangle from <see cref="Texture"/> used for shading.
    /// </summary>
    public Rectangle FontSolidRect;

    /// <summary>
    /// The color of the target rectangle.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Where on the <see cref="Host.Global.SharedSpriteBatch"/> the glyph should be drawn.
    /// </summary>
    public Rectangle TargetRect;

    /// <summary>
    /// Creates a new instance of this draw call.
    /// </summary>
    /// <param name="color">The folor of the rectangle.</param>
    /// <param name="texture">The texture containing a solid white rectangle referenced by <paramref name="fontSolidRect"/>.</param>
    /// <param name="targetRect">The drawing location of the rectangle.</param>
    /// <param name="fontSolidRect">The rectangle of the solid white glyph in the <paramref name="texture"/>.</param>
    public DrawCallColor(Color color, Texture texture, Rectangle targetRect, Rectangle fontSolidRect)
    {
        FontTexture = texture;
        FontSolidRect = fontSolidRect;
        TargetRect = targetRect;
        Color = color;
    }

    /// <inheritdoc/>
    public void Draw() =>
        Host.Global.SharedSpriteBatch.DrawQuad(TargetRect, FontSolidRect, Color, FontTexture);
}
