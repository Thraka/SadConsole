#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.Ansi
{
    /// <summary>
    /// Represents the state of an ANSI.SYS processor.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Indicates that the state is using bold colors.
        /// </summary>
        public bool Bold { get; set; }

        /// <summary>
        /// Indicates that the state is printing reverse colors.
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// Not supported.
        /// </summary>
        public bool Concealed { get; set; }

        /// <summary>
        /// Foreground color for the state of the ANSI.SYS processor.
        /// </summary>
        public Color Foreground { get; set; }

        /// <summary>
        /// Background color for the state of the ANSI.SYS processor.
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// Creates a new object to track the state of the ansi cursor.
        /// </summary>
        public State() => AnsiResetVideo();

        /// <summary>
        /// Forces the Background of the print appearance to be the darkened color and the foreground to be bright or not based on the <see cref="Bold"/> property.
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
            Concealed = false;
            Reverse = false;
            Foreground = ColorAnsi.White;
            Background = ColorAnsi.Black;
            AnsiCorrectPrintColor();
        }
    }
}
