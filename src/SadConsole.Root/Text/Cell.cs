using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

#if MONOGAME
using Color = Microsoft.Xna.Framework.Color;
#endif

namespace SadConsole.Text
{
    /// <summary>
    /// Represents an individual glyph on the screen with a foreground, background, and effect.
    /// </summary>
    public class Cell
    {
        public Color Foreground;
        public Color Background;
        public int Glyph;
        public SpriteEffects SpriteEffect;

        public Cell(Color foreground, Color background, int glyph)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
        }
    }
}
