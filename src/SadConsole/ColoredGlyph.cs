#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    using SadConsole.Effects;

    /// <summary>
    /// Represents a single character that has a foreground and background color.
    /// </summary>
    public class ColoredGlyph : Cell
    {
        /// <summary>
        /// The glyph.
        /// </summary>
        public char GlyphCharacter
        {
            get => (char)Glyph;
            set => Glyph = value;
        }

        /// <summary>
        /// The effect for the glyph.
        /// </summary>
        public ICellEffect Effect;

        /// <summary>
        /// Creates a new colored glyph with a white foreground, black background, and a glyph index of 0.
        /// </summary>
        public ColoredGlyph() : base(Color.White, Color.Black, 0) { }

        /// <summary>
        /// Creates a new colored glyph based on the provided cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public ColoredGlyph(Cell cell) : base(cell.Foreground, cell.Background, cell.Glyph, cell.Mirror) => GlyphCharacter = (char)cell.Glyph;

        /// <summary>
        /// Creates a new colored glyph with a white foreground and black background.
        /// </summary>
        /// <param name="glyph">The glyph.</param>
        public ColoredGlyph(int glyph) : base(Color.White, Color.Black, glyph) { }

        /// <summary>
        /// Creates a new colored glyph with a given foreground and background.
        /// </summary>
        /// <param name="glyph">The glyph.</param>
        /// <param name="background">The color of the foreground.</param>
        /// <param name="foreground">The color of the background.</param>
        public ColoredGlyph(int glyph, Color foreground, Color background) : base(foreground, background, glyph) { }

        /// <summary>
        /// Creates a new copy of this cell appearance.
        /// </summary>
        /// <returns>The cloned cell appearance.</returns>
        public new ColoredGlyph Clone() => new ColoredGlyph()
        {
            Foreground = Foreground,
            Background = Background,
            Effect = Effect != null ?
                            (Effect.CloneOnApply ? Effect.Clone() : Effect)
                            : null,
            GlyphCharacter = GlyphCharacter,
            Mirror = Mirror
        };
    }
}
