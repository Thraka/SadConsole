using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.DrawCalls
{
    /// <summary>
    /// Draws a glyph to active <see cref="Host.Global.SharedSpriteBatch"/>.
    /// </summary>
    public class DrawCallGlyph : IDrawCall
    {
        /// <summary>
        /// The texture containing the cell.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The rectangle from the <see cref="Texture"/> of the glyph.
        /// </summary>
        public Rectangle FontGlyphRect;

        /// <summary>
        /// The solid white glyph rectangle from <see cref="Texture"/> used for shading.
        /// </summary>
        public Rectangle FontSolidRect;

        /// <summary>
        /// The glyph to be drawn.
        /// </summary>
        public ColoredGlyph Cell;

        /// <summary>
        /// Where on the <see cref="Host.Global.SharedSpriteBatch"/> the glyph should be drawn.
        /// </summary>
        public Rectangle TargetRect;

        /// <summary>
        /// Creates a new instance of this draw call.
        /// </summary>
        /// <param name="cell">The glyph to be drawn.</param>
        /// <param name="texture">The texture containing the cell.</param>
        /// <param name="targetRect">Where on the <see cref="Host.Global.SharedSpriteBatch"/> the glyph should be drawn.</param>
        /// <param name="fontGlyphRect">The rectangle from the <see cref="Texture"/> of the glyph.</param>
        /// <param name="fontSolidRect">The solid white glyph rectangle from <see cref="Texture"/> used for shading.</param>
        public DrawCallGlyph(ColoredGlyph cell, Texture2D texture, Rectangle targetRect, Rectangle fontGlyphRect, Rectangle fontSolidRect)
        {
            Texture = texture;
            FontGlyphRect = fontGlyphRect;
            FontSolidRect = fontSolidRect;
            TargetRect = targetRect;
            Cell = cell;
        }

        /// <inheritdoc/>
        public void Draw()
        {
            Host.Global.SharedSpriteBatch.Draw(Texture, TargetRect, FontSolidRect, Cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
            Host.Global.SharedSpriteBatch.Draw(Texture, TargetRect, FontGlyphRect, Cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.65f);
        }
    }
}
