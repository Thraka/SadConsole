using Microsoft.Xna.Framework;

namespace SadConsole.Ansi
{

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

            if (input == ColorAnsi.BlackBright || input == ColorAnsi.Black)
                color = ColorAnsi.Black;
            else if (input == ColorAnsi.RedBright || input == ColorAnsi.Red)
                color = ColorAnsi.Red;
            else if (input == ColorAnsi.GreenBright || input == ColorAnsi.Green)
                color = ColorAnsi.Green;
            else if (input == ColorAnsi.YellowBright || input == ColorAnsi.Yellow)
                color = ColorAnsi.Yellow;
            else if (input == ColorAnsi.BlueBright || input == ColorAnsi.Blue)
                color = ColorAnsi.Blue;
            else if (input == ColorAnsi.MagentaBright || input == ColorAnsi.Magenta)
                color = ColorAnsi.Magenta;
            else if (input == ColorAnsi.CyanBright || input == ColorAnsi.Cyan)
                color = ColorAnsi.Cyan;
            else if (input == ColorAnsi.WhiteBright || input == ColorAnsi.White)
                color = ColorAnsi.White;

            return color;
        }

        /// <summary>
        /// Adjusts the provided color based on the <see cref="Attribute_Bold"/> value.
        /// </summary>
        /// <param name="input">The ansi color to adjust.</param>
        /// <returns>The adjusted color.</returns>
        /// <remarks>If the color provided is not an ansi color, dark or light, the passed in color will be returned.</remarks>
        public static Color AnsiAdjustColor(Color input, bool bold)
        {
            Color color = input;

            if (input == ColorAnsi.BlackBright || input == ColorAnsi.Black)
                color = bold ? ColorAnsi.BlackBright : ColorAnsi.Black;
            else if (input == ColorAnsi.RedBright || input == ColorAnsi.Red)
                color = bold ? ColorAnsi.RedBright : ColorAnsi.Red;
            else if (input == ColorAnsi.GreenBright || input == ColorAnsi.Green)
                color = bold ? ColorAnsi.GreenBright : ColorAnsi.Green;
            else if (input == ColorAnsi.YellowBright || input == ColorAnsi.Yellow)
                color = bold ? ColorAnsi.YellowBright : ColorAnsi.Yellow;
            else if (input == ColorAnsi.BlueBright || input == ColorAnsi.Blue)
                color = bold ? ColorAnsi.BlueBright : ColorAnsi.Blue;
            else if (input == ColorAnsi.MagentaBright || input == ColorAnsi.Magenta)
                color = bold ? ColorAnsi.MagentaBright : ColorAnsi.Magenta;
            else if (input == ColorAnsi.CyanBright || input == ColorAnsi.Cyan)
                color = bold ? ColorAnsi.CyanBright : ColorAnsi.Cyan;
            else if (input == ColorAnsi.WhiteBright || input == ColorAnsi.White)
                color = bold ? ColorAnsi.WhiteBright : ColorAnsi.White;

            return color;
        }

        /// <summary>
        /// Sets the print appearance of the cursor based on the ANSI.SYS code provided.
        /// </summary>
        /// <param name="isBackground">When true, changes the background color instead of the foreground.</param>
        /// <param name="code">The 0-7 color code.</param>
        public static void AnsiConfigurePrintColor(bool isBackground, int code, State ansiState)
        {
            Color color;

            if (code == 0)
                color = ansiState.Bold ? ColorAnsi.BlackBright : ColorAnsi.Black;
            else if (code == 1)
                color = ansiState.Bold ? ColorAnsi.RedBright : ColorAnsi.Red;
            else if (code == 2)
                color = ansiState.Bold ? ColorAnsi.GreenBright : ColorAnsi.Green;
            else if (code == 3)
                color = ansiState.Bold ? ColorAnsi.YellowBright : ColorAnsi.Yellow;
            else if (code == 4)
                color = ansiState.Bold ? ColorAnsi.BlueBright : ColorAnsi.Blue;
            else if (code == 5)
                color = ansiState.Bold ? ColorAnsi.MagentaBright : ColorAnsi.Magenta;
            else if (code == 6)
                color = ansiState.Bold ? ColorAnsi.CyanBright : ColorAnsi.Cyan;
            else if (code == 7)
                color = ansiState.Bold ? ColorAnsi.WhiteBright : ColorAnsi.White;
            else
                color = ColorAnsi.Black;

            if (isBackground)
                ansiState.Background = AnsiJustNormalColor(color);
            else
                ansiState.Foreground = color;
        }
    }
}
