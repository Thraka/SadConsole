using System.Linq;
using SadConsole.Effects;

namespace SadConsole
{
    public partial class ColoredString
    {
        /// <summary>
        /// A <see cref="ColoredGlyph"/> with a <see cref="ICellEffect"/>.
        /// </summary>
        public class ColoredGlyphEffect: ColoredGlyph
        {
            ICellEffect _effect;

            /// <summary>
            /// The effect of this cell.
            /// </summary>
            public ICellEffect Effect
            {
                get => _effect;
                set
                {
                    _effect = value;
                    IsDirty = true;
                }
            }

            /// <summary>
            /// Creates a copy of this <see cref="ColoredGlyphEffect"/>.
            /// </summary>
            /// <returns>A copy of this <see cref="ColoredGlyphEffect"/>.</returns>
            public new ColoredGlyphEffect Clone()
            {
                return new ColoredGlyphEffect()
                {
                    Foreground = Foreground,
                    Background = Background,
                    Glyph = Glyph,
                    Mirror = Mirror,
                    Effect = Effect,
                    Decorators = Decorators.ToArray()
                };
            }

            /// <summary>
            /// Creates a new <see cref="ColoredGlyphEffect"/> from a <see cref="ColoredGlyph"/> with the specified effect.
            /// </summary>
            /// <param name="glyph">The glyph.</param>
            /// <param name="effect">When provided, sets the <see cref="ColoredGlyphEffect.Effect"/>.</param>
            /// <returns></returns>
            public static ColoredGlyphEffect FromColoredGlyph(ColoredGlyph glyph, ICellEffect effect = null) =>
                new ColoredGlyphEffect() { Foreground = glyph.Foreground, Background = glyph.Background, Glyph = glyph.Glyph, Decorators = glyph.Decorators.ToArray(), Effect = effect };
        }
    }        
}
