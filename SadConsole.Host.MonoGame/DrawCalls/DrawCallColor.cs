using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws a colored rectangle to the active <see cref="Host.Global.SharedSpriteBatch"/>.
/// </summary>
public class DrawCallColor : IDrawCall
{
    /// <summary>
    /// The texture, most likely a SadConsole font texture, containing the solid white rectangle referenced by <see cref="FontSolidRect"/>.
    /// </summary>
    public Texture2D Texture;

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
    public DrawCallColor(Color color, Texture2D texture, Rectangle targetRect, Rectangle fontSolidRect)
    {
        Texture = texture;
        FontSolidRect = fontSolidRect;
        TargetRect = targetRect;
        Color = color;
    }

    /// <inheritdoc/>
    public void Draw() =>
        Host.Global.SharedSpriteBatch.Draw(Texture, TargetRect, FontSolidRect, Color, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
}
