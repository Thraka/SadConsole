using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Host;
using SadRogue.Primitives;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws a glyph to the active <see cref="Host.Global.SharedSpriteBatch"/>.
/// </summary>
public class DrawCallGlyph : IDrawCall
{
    /// <summary>
    /// The font to use when drawing the glyph.
    /// </summary>
    public IFont Font;

    /// <summary>
    /// The glyph to be drawn.
    /// </summary>
    public ColoredGlyphBase Cell;

    /// <summary>
    /// Where on the <see cref="Host.Global.SharedSpriteBatch"/> the glyph should be drawn.
    /// </summary>
    public Rectangle TargetRect;

    /// <summary>
    /// When <see langword="true"/>, draws the <see cref="ColoredGlyphBase.Background"/> color for the glyph; otherwise <see langword="false"/>.
    /// </summary>
    public bool DrawBackground;

    /// <summary>
    /// Creates a new instance of this draw call.
    /// </summary>
    /// <param name="cell">The glyph to be drawn.</param>
    /// <param name="targetRect">Where on the <see cref="Host.Global.SharedSpriteBatch"/> the glyph should be drawn.</param>
    /// <param name="font">The font to use when drawing the glyph.</param>
    /// <param name="drawBackground">When <see langword="true"/>, draws the <see cref="ColoredGlyphBase.Background"/> color for the glyph; otherwise <see langword="false"/>.</param>
    public DrawCallGlyph(ColoredGlyphBase cell, Rectangle targetRect, IFont font, bool drawBackground)
    {
        Font = font;
        TargetRect = targetRect;
        Cell = cell;
        DrawBackground = drawBackground;
    }

    /// <inheritdoc/>
    public void Draw()
    {
        Texture2D texture = ((GameTexture)Font.Image).Texture;

        if (DrawBackground)
            Host.Global.SharedSpriteBatch.Draw(texture, TargetRect, Font.SolidGlyphRectangle.ToMonoRectangle(), Cell.Background.ToMonoColor(), 0f, Vector2.Zero, Cell.Mirror.ToMonoGame(), 0.6f);

        Host.Global.SharedSpriteBatch.Draw(texture, TargetRect, Font.GetGlyphSourceRectangle(Cell.Glyph).ToMonoRectangle(), Cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, Cell.Mirror.ToMonoGame(), 0.65f);
    }
}
