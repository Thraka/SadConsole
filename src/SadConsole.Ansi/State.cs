#if SFML
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
#endif


namespace SadConsole.Ansi
{
    public class State
    {
        public bool Bold { get; set; }
        public bool Reverse { get; set; }
        public bool Conceled { get; set; }

        public Color Foreground { get; set; }
        public Color Background { get; set; }

        public int Character { get; set; }

        /// <summary>
        /// Forces the Background of the print appearance to be the darkened color and the foreground to be bright or not based on the <see cref="Attribute_Bold"/> property.
        /// </summary>
        public void AnsiCorrectPrintColor()
        {
            Background = Helpers.AnsiJustNormalColor(Background);
            Foreground = Helpers.AnsiAdjustColor(Foreground, Bold);
        }

        /// <summary>
        /// Resets all of the print appearance and ansi settings back to the default.
        /// </summary>
        public void AnsiResetVideo()
        {
            Bold = false;
            Conceled = false;
            Reverse = false;
            Foreground = ColorAnsi.White;
            Background = ColorAnsi.Black;
            AnsiCorrectPrintColor();
        }
    }
}
