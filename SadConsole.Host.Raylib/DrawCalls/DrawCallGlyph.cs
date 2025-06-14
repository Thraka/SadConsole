using System.Numerics;
using Raylib_cs;
using SadRogue.Primitives;

using Rectangle = Raylib_cs.Rectangle;

namespace SadConsole.DrawCalls;

/// <summary>
/// Draws a glyph using the host drawing system.
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
    /// Where the glyph should be drawn.
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
    /// <param name="targetRect">Where the glyph should be drawn.</param>
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
        Texture2D fontImage = ((Host.GameTexture)Font.Image).Texture;

        if (DrawBackground)
            Raylib.DrawTexturePro(fontImage, Font.SolidGlyphRectangle.ToHostRectangle(), TargetRect, Vector2.Zero, 0f, Cell.Background.ToHostColor());

        Raylib.DrawTexturePro(fontImage, Font.GetGlyphSourceRectangle(Cell.Glyph).ToHostRectangle(Cell.Mirror), TargetRect, Vector2.Zero, 0f, Cell.Foreground.ToHostColor());

    }
}
