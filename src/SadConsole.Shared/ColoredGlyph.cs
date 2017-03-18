using Microsoft.Xna.Framework;
using SadConsole.Effects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// Represents a single character that has a foreground and background color.
    /// </summary>
    public class ColoredGlyph : Cell
    {
        private char _character;

        /// <summary>
        /// The glyph.
        /// </summary>
        public char GlyphCharacter
        {
            get { return _character; }
            set
            {
                _character = value;
                base.Glyph = _character;
            }
        }

        /// <summary>
        /// Sets the glyph by index.
        /// </summary>
        public new int Glyph
        {
            get { return base.Glyph; }
            set { GlyphCharacter = (char)value; }
        }

        /// <summary>
        /// The effect for the glyph.
        /// </summary>
        public ICellEffect Effect;

        /// <summary>
        /// Creates a new colored glyph with a white foreground, transparent background, and a glyph index of 0.
        /// </summary>
        public ColoredGlyph() : base(Color.White, Color.Black, 0) { }

        /// <summary>
        /// Creates a new colored glyph based on the provided cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public ColoredGlyph(Cell cell) : base(cell.Foreground, cell.Background, cell.Glyph)
        {
            GlyphCharacter = (char)cell.Glyph;
        }

        /// <summary>
        /// Creates a new copy of this cell appearance.
        /// </summary>
        /// <returns>The cloned cell appearance.</returns>
        public new ColoredGlyph Clone()
        {
            return new ColoredGlyph() { Foreground = this.Foreground, Background = this.Background, Effect = this.Effect != null ? this.Effect.Clone() : null, GlyphCharacter = this.GlyphCharacter, Mirror = this.Mirror };
        }
    }
}
