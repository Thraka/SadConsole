using SadRogue.Primitives;

namespace SadConsole.Ansi
{
    /// <summary>
    /// Helpers related to ANSI processing.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Returns the a normal, dark, ansi color based on the ansi color provided.
        /// </summary>
        /// <param name="input">An ansi color.</param>
        /// <returns>The adjusted color.</returns>
        /// <remarks>If the color provided is not an ansi color, dark or light, the passed in color will be returned.</remarks>
        public static Color AnsiJustNormalColor(Color input)
        {
            Color color = input;

            if (input == Color.AnsiBlackBright || input == Color.AnsiBlack)
            {
                color = Color.AnsiBlack;
            }
            else if (input == Color.AnsiRedBright || input == Color.AnsiRed)
            {
                color = Color.AnsiRed;
            }
            else if (input == Color.AnsiGreenBright || input == Color.AnsiGreen)
            {
                color = Color.AnsiGreen;
            }
            else if (input == Color.AnsiYellowBright || input == Color.AnsiYellow)
            {
                color = Color.AnsiYellow;
            }
            else if (input == Color.AnsiBlueBright || input == Color.AnsiBlue)
            {
                color = Color.AnsiBlue;
            }
            else if (input == Color.AnsiMagentaBright || input == Color.AnsiMagenta)
            {
                color = Color.AnsiMagenta;
            }
            else if (input == Color.AnsiCyanBright || input == Color.AnsiCyan)
            {
                color = Color.AnsiCyan;
            }
            else if (input == Color.AnsiWhiteBright || input == Color.AnsiWhite)
            {
                color = Color.AnsiWhite;
            }

            return color;
        }

        /// <summary>
        /// Adjusts the provided color based on the <paramref name="bold"/> value.
        /// </summary>
        /// <param name="input">The ansi color to adjust.</param>
        /// <param name="bold">When <see langword="true"/>, adjusts the color to the bright version. When <see langword="false"/>, adjusts the color to the normal version.</param>
        /// <returns>The adjusted color.</returns>
        /// <remarks>If the color provided is not an ansi color, dark or light, the passed in color will be returned.</remarks>
        public static Color AnsiAdjustColor(Color input, bool bold)
        {
            Color color = input;

            if (input == Color.AnsiBlackBright || input == Color.AnsiBlack)
            {
                color = bold ? Color.AnsiBlackBright : Color.AnsiBlack;
            }
            else if (input == Color.AnsiRedBright || input == Color.AnsiRed)
            {
                color = bold ? Color.AnsiRedBright : Color.AnsiRed;
            }
            else if (input == Color.AnsiGreenBright || input == Color.AnsiGreen)
            {
                color = bold ? Color.AnsiGreenBright : Color.AnsiGreen;
            }
            else if (input == Color.AnsiYellowBright || input == Color.AnsiYellow)
            {
                color = bold ? Color.AnsiYellowBright : Color.AnsiYellow;
            }
            else if (input == Color.AnsiBlueBright || input == Color.AnsiBlue)
            {
                color = bold ? Color.AnsiBlueBright : Color.AnsiBlue;
            }
            else if (input == Color.AnsiMagentaBright || input == Color.AnsiMagenta)
            {
                color = bold ? Color.AnsiMagentaBright : Color.AnsiMagenta;
            }
            else if (input == Color.AnsiCyanBright || input == Color.AnsiCyan)
            {
                color = bold ? Color.AnsiCyanBright : Color.AnsiCyan;
            }
            else if (input == Color.AnsiWhiteBright || input == Color.AnsiWhite)
            {
                color = bold ? Color.AnsiWhiteBright : Color.AnsiWhite;
            }

            return color;
        }

        /// <summary>
        /// Sets the print appearance of the cursor based on the ANSI.SYS code provided.
        /// </summary>
        /// <param name="isBackground">When true, changes the background color instead of the foreground.</param>
        /// <param name="code">The 0-7 color code.</param>
        /// <param name="ansiState">The current state of the ANSI settings.</param>
        public static void AnsiConfigurePrintColor(bool isBackground, int code, State ansiState)
        {
            Color color;

            if (code == 0)
            {
                color = ansiState.Bold ? Color.AnsiBlackBright : Color.AnsiBlack;
            }
            else if (code == 1)
            {
                color = ansiState.Bold ? Color.AnsiRedBright : Color.AnsiRed;
            }
            else if (code == 2)
            {
                color = ansiState.Bold ? Color.AnsiGreenBright : Color.AnsiGreen;
            }
            else if (code == 3)
            {
                color = ansiState.Bold ? Color.AnsiYellowBright : Color.AnsiYellow;
            }
            else if (code == 4)
            {
                color = ansiState.Bold ? Color.AnsiBlueBright : Color.AnsiBlue;
            }
            else if (code == 5)
            {
                color = ansiState.Bold ? Color.AnsiMagentaBright : Color.AnsiMagenta;
            }
            else if (code == 6)
            {
                color = ansiState.Bold ? Color.AnsiCyanBright : Color.AnsiCyan;
            }
            else if (code == 7)
            {
                color = ansiState.Bold ? Color.AnsiWhiteBright : Color.AnsiWhite;
            }
            else
            {
                color = Color.AnsiBlack;
            }

            if (isBackground)
            {
                ansiState.Background = AnsiJustNormalColor(color);
            }
            else
            {
                ansiState.Foreground = color;
            }
        }
    }
}
