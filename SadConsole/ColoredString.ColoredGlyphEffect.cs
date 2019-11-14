using System;
using System.Collections.Generic;
using SadConsole.StringParser;
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
            public ColoredGlyphEffect Clone()
            {
                return new ColoredGlyphEffect()
                {
                    Foreground = Foreground,
                    Background = Background,
                    Glyph = Glyph,
                    Mirror = Mirror,
                    Effect = Effect
                };
            }
        }
    }        
}
