#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.UI;

/// <summary>
/// Default colors used by control themes.
/// </summary>
public partial class Colors
{
    /// <summary>
    /// An enumeration of each color name defined by the library colors object.
    /// </summary>
    public enum ColorNames
    {
        White,
        Black,
        Gray,
        GrayDark,
        Red,
        RedDark,
        Green,
        GreenDark,
        Cyan,
        CyanDark,
        Blue,
        BlueDark,
        Purple,
        PurpleDark,
        Yellow,
        YellowDark,
        Orange,
        OrangeDark,
        Brown,
        BrownDark,
        Gold,
        GoldDark,
        Silver,
        SilverDark,
        Bronze,
        BronzeDark
    }

    /// <summary>
    /// A brightness value that can be applied to a color.
    /// </summary>
    public enum Brightness
    {
        Brightest,
        Bright,
        Normal,
        Dark,
        Darkest
    }
}
