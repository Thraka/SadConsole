﻿using SadRogue.Primitives;
using Rectangle = SFML.Graphics.IntRect;
using Color = SFML.Graphics.Color;
using SFML.Graphics;

namespace SadConsole.DrawCalls
{
    /// <summary>
    /// Draws a glyph to active <see cref="Host.Global.SharedSpriteBatch"/>.
    /// </summary>
    public class DrawCallCell : IDrawCall
    {
        /// <summary>
        /// The font to use when drawing the glyph.
        /// </summary>
        public IFont Font;

        /// <summary>
        /// The glyph to be drawn.
        /// </summary>
        public ColoredGlyph Cell;

        /// <summary>
        /// Where on the <see cref="Host.Global.SharedSpriteBatch"/> the glyph should be drawn.
        /// </summary>
        public Rectangle TargetRect;

        /// <summary>
        /// When <see langword="true"/>, draws the <see cref="ColoredGlyph.Background"/> color for the glyph; otherwise <see langword="false"/>.
        /// </summary>
        public bool DrawBackground;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell">The glyph to be drawn.</param>
        /// <param name="targetRect">Where on the <see cref="Host.Global.SharedSpriteBatch"/> the glyph should be drawn.</param>
        /// <param name="font">The font to use when drawing the glyph.</param>
        /// <param name="drawBackground">When <see langword="true"/>, draws the <see cref="ColoredGlyph.Background"/> color for the glyph; otherwise <see langword="false"/>.</param>
        public DrawCallCell(SadConsole.ColoredGlyph cell, Rectangle targetRect, IFont font, bool drawBackground)
        {
            Font = font;
            TargetRect = targetRect;
            Cell = cell;
            DrawBackground = drawBackground;
        }

        public void Draw()
        {
            Host.Global.SharedSpriteBatch.DrawCell(Cell, TargetRect, DrawBackground, Font); 
        }
    }
}
