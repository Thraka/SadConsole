using System;
using SadRogue.Primitives;

namespace SadConsole.UI
{
    /// <summary>
    /// Default colors used by control themes.
    /// </summary>
    public partial class Colors
    {
        /// <summary>
        /// Creates a new colors object with the default blue colors theme of SadConsole.
        /// </summary>
        /// <returns></returns>
        public static Colors CreateDefault()
        {
            var colors = new Colors(0)
            {
                White = Color.White,
                Black = Color.Black,
                Gray = new Color(176, 196, 222),
                GrayDark = new Color(66, 66, 66),
                Red = new Color(255, 0, 51),
                Green = new Color(153, 224, 0),
                Blue = new Color(102, 204, 255),
                Purple = new Color(132, 0, 214),
                Yellow = new Color(255, 255, 102),
                Orange = new Color(255, 153, 0),
                Cyan = new Color(82, 242, 234),
                Brown = new Color(100, 59, 15),
                RedDark = new Color(153, 51, 51),
                GreenDark = new Color(110, 166, 23),
                BlueDark = new Color(51, 102, 153),
                PurpleDark = new Color(70, 0, 114),
                YellowDark = new Color(255, 207, 15),
                OrangeDark = new Color(255, 102, 0),
                CyanDark = new Color(33, 182, 168),
                BrownDark = new Color(119, 17, 0),
                Gold = new Color(255, 215, 0),
                GoldDark = new Color(127, 107, 0),
                Silver = new Color(192, 192, 192),
                SilverDark = new Color(169, 169, 169),
                Bronze = new Color(205, 127, 50),
                BronzeDark = new Color(144, 89, 35)
            };

            colors.TitleText = new AdjustableColor(ColorNames.Orange, "TitleText", colors);
            colors.Lines = new AdjustableColor(ColorNames.Gray, "Lines", colors);
            colors.TextBright = new AdjustableColor(ColorNames.White, "TextBright", colors);
            colors.Text = new AdjustableColor(ColorNames.Blue, "Text", colors);
            colors.TextSelected = new AdjustableColor(ColorNames.Yellow, "TextSelected", colors);
            colors.TextSelectedDark = new AdjustableColor(ColorNames.Green, "TextSelectedDark", colors);
            colors.TextLight = new AdjustableColor(ColorNames.Gray, "TextLight", colors);
            colors.TextDark = new AdjustableColor(ColorNames.Green, "TextDark", colors);
            colors.TextFocused = new AdjustableColor(ColorNames.Cyan, "TextFocused", colors);

            colors.ControlBack = new AdjustableColor(ColorNames.BlueDark, "ControlBack", colors);
            colors.ControlBackLight = new AdjustableColor(colors.ControlBack, "ControlBackLight", colors) { Brightness = Brightness.Bright };
            colors.ControlBackMouseOver = new AdjustableColor(ColorNames.Green, "ControlBackMouseOver", colors);
            colors.ControlBackSelected = new AdjustableColor(ColorNames.GreenDark, "ControlBackSelected", colors);
            colors.ControlBackDark = new AdjustableColor(colors.ControlBack, "ControlBackDark", colors) { Brightness = Brightness.Darkest };
            colors.ControlHostBack = new AdjustableColor(ColorNames.BlueDark, "ControlHostBack", colors);
            colors.ControlHostFore = new AdjustableColor(colors.Text, "ControlHostFore", colors);

            // Rebuild the controls
            colors.RebuildAppearances();

            return colors;
        }
        
        /// <summary>
        /// Creates a new colors object with a standard black-based theme.
        /// </summary>
        /// <returns></returns>
        public static Colors CreateAnsi()
        {
            // Create a new 
            var colors = new Colors(0)
            {
                White = Color.White,
                Black = Color.Black,
                Gray = Color.AnsiWhite,
                GrayDark = (Color.AnsiWhite * 0.50f).FillAlpha(),
                Red = Color.AnsiRedBright,
                Green = Color.AnsiGreenBright,
                Blue = Color.AnsiBlueBright,
                Purple = Color.AnsiMagentaBright,
                Yellow = Color.AnsiYellowBright,
                Orange = Color.Orange,
                Cyan = Color.AnsiCyanBright,
                Brown = Color.AnsiYellow,
                RedDark = Color.AnsiRed,
                GreenDark = Color.AnsiGreen,
                BlueDark = Color.AnsiBlue,
                PurpleDark = Color.AnsiMagenta,
                YellowDark = (Color.AnsiYellowBright * 0.50f).FillAlpha(),
                OrangeDark = Color.DarkOrange,
                CyanDark = Color.AnsiCyan,
                BrownDark = (Color.AnsiYellow * 0.50f).FillAlpha(),
                Gold = Color.Goldenrod,
                GoldDark = Color.DarkGoldenrod,
                Silver = Color.Silver,
                SilverDark = (Color.Silver * 0.50f).FillAlpha(),
                Bronze = new Color(205, 127, 50),
                BronzeDark = (new Color(205, 127, 50) * 0.50f).FillAlpha(),
            };

            colors.TitleText = new AdjustableColor(ColorNames.Orange, "TitleText", colors);
            colors.Lines = new AdjustableColor(ColorNames.CyanDark, "Lines", colors);
            colors.TextBright = new AdjustableColor(ColorNames.White, "TextBright", colors);
            colors.Text = new AdjustableColor(ColorNames.Gray, "Text", colors);
            colors.TextSelected = new AdjustableColor(ColorNames.Yellow, "TextSelected", colors);
            colors.TextSelectedDark = new AdjustableColor(ColorNames.YellowDark, "TextSelectedDark", colors);
            colors.TextLight = new AdjustableColor(ColorNames.White, "TextLight", colors);
            colors.TextDark = new AdjustableColor(ColorNames.GrayDark, "TextDark", colors);
            colors.TextFocused = new AdjustableColor(ColorNames.White, "TextFocused", colors);

            colors.ControlBack = new AdjustableColor(ColorNames.Black, "ControlBack", colors);
            colors.ControlBackLight = new AdjustableColor(colors.ControlBack, "ControlBackLight", colors) { Brightness = Brightness.Bright };
            colors.ControlBackMouseOver = new AdjustableColor(ColorNames.Green, "ControlBackMouseOver", colors);
            colors.ControlBackSelected = new AdjustableColor(ColorNames.GreenDark, "ControlBackSelected", colors);
            colors.ControlBackDark = new AdjustableColor(colors.ControlBack, "ControlBackDark", colors) { Brightness = Brightness.Darkest };
            colors.ControlHostBack = new AdjustableColor(ColorNames.Black, "ControlHostBack", colors);
            colors.ControlHostFore = new AdjustableColor(colors.Text, "ControlHostFore", colors);

            // Rebuild the controls
            colors.RebuildAppearances();

            return colors;
        }
    }
}
