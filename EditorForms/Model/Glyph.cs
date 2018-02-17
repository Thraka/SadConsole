using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Model
{
    class GlyphItem
    {
        private Font font;

        public Color Foreground = Color.White;
        public Color Background = Color.Black;
        public int Glyph = 1;
        public Font Font
        {
            get
            {
                if (font == null)
                    return SadConsole.Global.FontDefault;
                else
                    return font;
            }

            set { font = value; }

        }

    }
}
