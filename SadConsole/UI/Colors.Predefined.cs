using SadRogue.Primitives;

namespace SadConsole.UI;

/// <summary>
/// Default colors used by control themes.
/// </summary>
public partial class Colors
{
    /// <summary>
    /// Creates a new colors object with the default blue colors theme of SadConsole.
    /// </summary>
    /// <returns></returns>
    public static Colors CreateSadConsoleBlue()
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

        colors.IsLightTheme = true;

        colors.Title = new AdjustableColor(ColorNames.Orange, "Title", colors);
        colors.Lines = new AdjustableColor(ColorNames.Gray, "Lines", colors);

        colors.ControlForegroundNormal = new AdjustableColor(ColorNames.Blue, "Control Foreground Normal", colors);
        colors.ControlForegroundDisabled = new AdjustableColor(ColorNames.Gray, "Control Foreground Disabled", colors);
        colors.ControlForegroundMouseOver = new AdjustableColor(ColorNames.Blue, "Control Foreground MouseOver", colors);
        colors.ControlForegroundMouseDown = new AdjustableColor(ColorNames.BlueDark, "Control Foreground MouseDown", colors);
        colors.ControlForegroundSelected = new AdjustableColor(ColorNames.Yellow, "Control Foreground Selected", colors);
        colors.ControlForegroundFocused = new AdjustableColor(ColorNames.Cyan, "Control Foreground Focused", colors);

        colors.ControlBackgroundNormal = new AdjustableColor(ColorNames.BlueDark, "Control Background Normal", colors);
        colors.ControlBackgroundDisabled = new AdjustableColor(ColorNames.BlueDark, "Control Background Disabled", colors);
        colors.ControlBackgroundMouseOver = new AdjustableColor(ColorNames.BlueDark, "Control Background MouseOver", colors) { Brightness = Brightness.Dark };
        colors.ControlBackgroundMouseDown = new AdjustableColor(ColorNames.Blue, "Control Background MouseDown", colors);
        colors.ControlBackgroundSelected = new AdjustableColor(ColorNames.BlueDark, "Control Background Selected", colors);
        colors.ControlBackgroundFocused = new AdjustableColor(ColorNames.BlueDark, "Control Background Focused", colors) { Brightness = Brightness.Dark };

        colors.ControlHostForeground = new AdjustableColor(ColorNames.Blue, "Control Host Foreground", colors);
        colors.ControlHostBackground = new AdjustableColor(ColorNames.BlueDark, "Control Host Background", colors);

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

        colors.IsLightTheme = false;

        colors.Title = new AdjustableColor(ColorNames.Orange, "Title", colors);
        colors.Lines = new AdjustableColor(ColorNames.CyanDark, "Lines", colors);

        colors.ControlForegroundNormal = new AdjustableColor(ColorNames.Gray, "Control Foreground Normal", colors);
        colors.ControlForegroundDisabled = new AdjustableColor(ColorNames.GrayDark, "Control Foreground Disabled", colors);
        colors.ControlForegroundMouseOver = new AdjustableColor(ColorNames.White, "Control Foreground MouseOver", colors);
        colors.ControlForegroundMouseDown = new AdjustableColor(ColorNames.White, "Control Foreground MouseDown", colors);
        colors.ControlForegroundSelected = new AdjustableColor(ColorNames.Yellow, "Control Foreground Selected", colors);
        colors.ControlForegroundFocused = new AdjustableColor(ColorNames.White, "Control Foreground Focused", colors);

        colors.ControlBackgroundNormal = new AdjustableColor(ColorNames.Black, "Control Background Normal", colors);
        colors.ControlBackgroundDisabled = new AdjustableColor(ColorNames.Black, "Control Background Disabled", colors);
        colors.ControlBackgroundMouseOver = new AdjustableColor(ColorNames.Black, "Control Background MouseOver", colors);
        colors.ControlBackgroundMouseDown = new AdjustableColor(ColorNames.Gray, "Control Background MouseDown", colors);
        colors.ControlBackgroundSelected = new AdjustableColor(ColorNames.Black, "Control Background Selected", colors);
        colors.ControlBackgroundFocused = new AdjustableColor(ColorNames.Black, "Control Background Focused", colors);

        colors.ControlHostForeground = new AdjustableColor(ColorNames.Gray, "Control Host Foreground", colors);
        colors.ControlHostBackground = new AdjustableColor(ColorNames.Black, "Control Host Background", colors);

        // Rebuild the controls
        colors.RebuildAppearances();

        return colors;
    }
}
