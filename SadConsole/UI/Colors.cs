using System;
using SadRogue.Primitives;

namespace SadConsole.UI;

/// <summary>
/// Default colors used by control themes.
/// </summary>
public partial class Colors
{
    private static Colors s_default;

    /// <summary>
    /// The default colors used by controls and your code.
    /// </summary>
    public static Colors Default
    {
        get => s_default;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            s_default = value;
            s_default.AddToColorMappings();
        }
    }

    static Colors()
    {
        s_default = CreateAnsi();
        s_default.AddToColorMappings();
    }

    /// <summary>
    /// Status flag to indicate that the primary backgrounds of the controls are considered light rather than dark.
    /// </summary>
    public bool IsLightTheme { get; set; }

    /// <summary>
    /// The name of this color scheme.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The white color.
    /// </summary>
    public Color White { get; set; }

    /// <summary>
    /// The black color.
    /// </summary>
    public Color Black { get; set; }

    /// <summary>
    /// The gray color.
    /// </summary>
    public Color Gray { get; set; }

    /// <summary>
    /// The dark gray color.
    /// </summary>
    public Color GrayDark { get; set; }

    /// <summary>
    /// The red color.
    /// </summary>
    public Color Red { get; set; }

    /// <summary>
    /// The green color.
    /// </summary>
    public Color Green { get; set; }

    /// <summary>
    /// The blue color.
    /// </summary>
    public Color Blue { get; set; }

    /// <summary>
    /// The purple color.
    /// </summary>
    public Color Purple { get; set; }

    /// <summary>
    /// The yellow color.
    /// </summary>
    public Color Yellow { get; set; }

    /// <summary>
    /// The orange color.
    /// </summary>
    public Color Orange { get; set; }

    /// <summary>
    /// The cyan color.
    /// </summary>
    public Color Cyan { get; set; }

    /// <summary>
    /// The brown color.
    /// </summary>
    public Color Brown { get; set; }

    /// <summary>
    /// The dark red color.
    /// </summary>
    public Color RedDark { get; set; }

    /// <summary>
    /// The dark green color.
    /// </summary>
    public Color GreenDark { get; set; }

    /// <summary>
    /// The dark blue color.
    /// </summary>
    public Color BlueDark { get; set; }

    /// <summary>
    /// The dark purple color.
    /// </summary>
    public Color PurpleDark { get; set; }

    /// <summary>
    /// The dark yellow color.
    /// </summary>
    public Color YellowDark { get; set; }

    /// <summary>
    /// The dark orange color.
    /// </summary>
    public Color OrangeDark { get; set; }

    /// <summary>
    /// The dark cyan color.
    /// </summary>
    public Color CyanDark { get; set; }

    /// <summary>
    /// The dark brown color.
    /// </summary>
    public Color BrownDark { get; set; }

    /// <summary>
    /// The gold color.
    /// </summary>
    public Color Gold { get; set; }

    /// <summary>
    /// The dark gold color.
    /// </summary>
    public Color GoldDark { get; set; }

    /// <summary>
    /// The silver color.
    /// </summary>
    public Color Silver { get; set; }

    /// <summary>
    /// The dark silver color.
    /// </summary>
    public Color SilverDark { get; set; }

    /// <summary>
    /// The bronze color.
    /// </summary>
    public Color Bronze { get; set; }

    /// <summary>
    /// The dark bronze color.
    /// </summary>
    public Color BronzeDark { get; set; }

    /// <summary>
    /// The color used to darken the background when <see cref="o:UI.Window.Show"/> is called and <see cref="UI.Window.IsModal"/> is <see langword="true"/>. Defaults to (20, 20, 20, 200).
    /// </summary>
    public Color ModalBackground { get; set; } = new Color(20, 20, 20, 200);

    /// <summary>
    /// Color used for titles.
    /// </summary>
    public AdjustableColor Title { get; set; }

    /// <summary>
    /// Color used in drawing lines.
    /// </summary>
    public AdjustableColor Lines { get; set; }

    /// <summary>
    /// Color of a control's foreground in normal state.
    /// </summary>
    public AdjustableColor ControlForegroundNormal { get; set; }

    /// <summary>
    /// Color of a control's foreground in disabled state.
    /// </summary>
    public AdjustableColor ControlForegroundDisabled { get; set; }

    /// <summary>
    /// Color of a control's foreground in mouse-over state.
    /// </summary>
    public AdjustableColor ControlForegroundMouseOver { get; set; }

    /// <summary>
    /// Color of a control's foreground in mouse-button-down state.
    /// </summary>
    public AdjustableColor ControlForegroundMouseDown { get; set; }

    /// <summary>
    /// Color of a control's foreground in selected state.
    /// </summary>
    public AdjustableColor ControlForegroundSelected { get; set; }

    /// <summary>
    /// Color of a control's foreground in focused state.
    /// </summary>
    public AdjustableColor ControlForegroundFocused { get; set; }

    /// <summary>
    /// Color of a control's background in normal state.
    /// </summary>
    public AdjustableColor ControlBackgroundNormal { get; set; }

    /// <summary>
    /// Color of a control's background in disabled state.
    /// </summary>
    public AdjustableColor ControlBackgroundDisabled { get; set; }

    /// <summary>
    /// Color of a control's background in normal state.
    /// </summary>
    public AdjustableColor ControlBackgroundMouseOver { get; set; }

    /// <summary>
    /// Color of a control's background in mouse-button-down state.
    /// </summary>
    public AdjustableColor ControlBackgroundMouseDown { get; set; }

    /// <summary>
    /// Color of a control's background in selected state.
    /// </summary>
    public AdjustableColor ControlBackgroundSelected { get; set; }

    /// <summary>
    /// Color of a control's background in focused state.
    /// </summary>
    public AdjustableColor ControlBackgroundFocused { get; set; }

    /// <summary>
    /// Color of the control host's foreground.
    /// </summary>
    public AdjustableColor ControlHostForeground { get; set; }

    /// <summary>
    /// Color of the control host's background.
    /// </summary>
    public AdjustableColor ControlHostBackground { get; set; }

    /// <summary>
    /// The appearance of a control when in the normal state.
    /// </summary>
    public ColoredGlyphBase Appearance_ControlNormal { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of a control when in the disabled state.
    /// </summary>
    public ColoredGlyphBase Appearance_ControlDisabled { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of a control when in the mouse-over state.
    /// </summary>
    public ColoredGlyphBase Appearance_ControlOver { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of a control when in the selected state.
    /// </summary>
    public ColoredGlyphBase Appearance_ControlSelected { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of a control when in the mouse-down state.
    /// </summary>
    public ColoredGlyphBase Appearance_ControlMouseDown { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of a control when in the focused state.
    /// </summary>
    public ColoredGlyphBase Appearance_ControlFocused { get; set; } = new ColoredGlyph();

    /// <summary>
    /// Creates a color object based on the <see cref="Colors.CreateAnsi"/> scheme.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Colors()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        Colors colors = CreateAnsi();

        colors.CopyTo(this);
    }

    /// <summary>
    /// Internal constructor to avoid creating any predefined colors.
    /// </summary>
    /// <param name="_">Discarded parameter</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Colors(int _) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Sets all Appearance* properties based on the existing colors and settings.
    /// </summary>
    public virtual void RebuildAppearances()
    {
        Appearance_ControlNormal = new ColoredGlyph(ControlForegroundNormal, ControlBackgroundNormal);
        Appearance_ControlDisabled = new ColoredGlyph(ControlForegroundDisabled, ControlBackgroundDisabled);
        Appearance_ControlOver = new ColoredGlyph(ControlForegroundMouseOver, ControlBackgroundMouseOver);
        Appearance_ControlMouseDown = new ColoredGlyph(ControlForegroundMouseDown, ControlBackgroundMouseDown);
        Appearance_ControlSelected = new ColoredGlyph(ControlForegroundSelected, ControlBackgroundSelected);
        Appearance_ControlFocused = new ColoredGlyph(ControlForegroundFocused, ControlBackgroundFocused);
    }

    /// <summary>
    /// Compares two colors and if they match, returns a color that is lighter or darker based on if <see cref="Colors.IsLightTheme"/>.
    /// </summary>
    /// <param name="inColor">The base color.</param>
    /// <param name="compareColor">The color to compare with.</param>
    /// <returns>A new color.</returns>
    public Color GetOffColor(Color inColor, Color compareColor)
    {
        if (inColor == compareColor)
            inColor = IsLightTheme ? compareColor.GetDark() : compareColor.GetBright();

        return NormalizeBlack(inColor);
    }

    /// <summary>
    /// Normalizes a dark color to at least R:25 G:25 B:25 A:255.
    /// </summary>
    /// <param name="inColor">The color to check.</param>
    /// <returns>A new color.</returns>
    public static Color NormalizeBlack(Color inColor)
    {
        if (inColor.R < 25
            && inColor.G < 25
            && inColor.B < 25)
            return new Color(25, 25, 25, 255);
        else
            return inColor;
    }

    /// <summary>
    /// Copies the colors to another color object.
    /// </summary>
    /// <param name="colors">The color object to copy to.</param>
    public void CopyTo(Colors colors)
    {
        colors.White = White;
        colors.Black = Black;
        colors.Gray = Gray;
        colors.GrayDark = GrayDark;

        colors.Red = Red;
        colors.Green = Green;
        colors.Blue = Blue;
        colors.Purple = Purple;
        colors.Yellow = Yellow;
        colors.Orange = Orange;
        colors.Cyan = Cyan;
        colors.Brown = Brown;

        colors.RedDark = RedDark;
        colors.GreenDark = GreenDark;
        colors.BlueDark = BlueDark;
        colors.PurpleDark = PurpleDark;
        colors.YellowDark = YellowDark;
        colors.OrangeDark = OrangeDark;
        colors.CyanDark = CyanDark;
        colors.BrownDark = BrownDark;

        colors.Gold = Gold;
        colors.GoldDark = GoldDark;
        colors.Silver = Silver;
        colors.SilverDark = SilverDark;
        colors.Bronze = Bronze;
        colors.BronzeDark = BronzeDark;

        colors.ModalBackground = ModalBackground;

        colors.IsLightTheme = IsLightTheme;

        colors.Title = Title.Clone();
        colors.Lines = Lines.Clone();
        colors.ControlForegroundNormal = ControlForegroundNormal.Clone();
        colors.ControlForegroundDisabled = ControlForegroundDisabled.Clone();
        colors.ControlForegroundMouseOver = ControlForegroundMouseOver.Clone();
        colors.ControlForegroundMouseDown = ControlForegroundMouseDown.Clone();
        colors.ControlForegroundSelected = ControlForegroundSelected.Clone();
        colors.ControlForegroundFocused = ControlForegroundFocused.Clone();
        colors.ControlBackgroundNormal = ControlBackgroundNormal.Clone();
        colors.ControlBackgroundDisabled = ControlBackgroundDisabled.Clone();
        colors.ControlBackgroundMouseOver = ControlBackgroundMouseOver.Clone();
        colors.ControlBackgroundMouseDown = ControlBackgroundMouseDown.Clone();
        colors.ControlBackgroundSelected = ControlBackgroundSelected.Clone();
        colors.ControlBackgroundFocused = ControlBackgroundFocused.Clone();
        colors.ControlHostForeground = ControlHostForeground.Clone();
        colors.ControlHostBackground = ControlHostBackground.Clone();

        colors.Appearance_ControlNormal.CopyAppearanceFrom(Appearance_ControlNormal);
        colors.Appearance_ControlDisabled.CopyAppearanceFrom(Appearance_ControlDisabled);
        colors.Appearance_ControlOver.CopyAppearanceFrom(Appearance_ControlOver);
        colors.Appearance_ControlSelected.CopyAppearanceFrom(Appearance_ControlSelected);
        colors.Appearance_ControlMouseDown.CopyAppearanceFrom(Appearance_ControlMouseDown);
        colors.Appearance_ControlFocused.CopyAppearanceFrom(Appearance_ControlFocused);
    }

    /// <summary>
    /// Gets a color by enumeration.
    /// </summary>
    /// <param name="color">The color to get.</param>
    /// <returns>A color.</returns>
    public Color FromColorName(ColorNames color) => color switch
    {
        ColorNames.White => White,
        ColorNames.Black => Black,
        ColorNames.Gray => Gray,
        ColorNames.GrayDark => GrayDark,
        ColorNames.Red => Red,
        ColorNames.RedDark => RedDark,
        ColorNames.Green => Green,
        ColorNames.GreenDark => GreenDark,
        ColorNames.Cyan => Cyan,
        ColorNames.CyanDark => CyanDark,
        ColorNames.Blue => Blue,
        ColorNames.BlueDark => BlueDark,
        ColorNames.Purple => Purple,
        ColorNames.PurpleDark => PurpleDark,
        ColorNames.Yellow => Yellow,
        ColorNames.YellowDark => YellowDark,
        ColorNames.Orange => Orange,
        ColorNames.OrangeDark => OrangeDark,
        ColorNames.Brown => Brown,
        ColorNames.BrownDark => BrownDark,
        ColorNames.Gold => Gold,
        ColorNames.GoldDark => GoldDark,
        ColorNames.Silver => Silver,
        ColorNames.SilverDark => SilverDark,
        ColorNames.Bronze => Bronze,
        ColorNames.BronzeDark => BronzeDark,
        _ => throw new NotImplementedException()
    };

    /// <summary>
    /// Sets a named color to a specified value.
    /// </summary>
    /// <param name="name">The name of the color.</param>
    /// <param name="color">The color value.</param>
    public void SetColorByName(ColorNames name, Color color)
    {
        switch (name)
        {
            case ColorNames.White:
                White = color;
                break;
            case ColorNames.Black:
                Black = color;
                break;
            case ColorNames.Gray:
                Gray = color;
                break;
            case ColorNames.GrayDark:
                GrayDark = color;
                break;
            case ColorNames.Red:
                Red = color;
                break;
            case ColorNames.RedDark:
                RedDark = color;
                break;
            case ColorNames.Green:
                Green = color;
                break;
            case ColorNames.GreenDark:
                GreenDark = color;
                break;
            case ColorNames.Cyan:
                Cyan = color;
                break;
            case ColorNames.CyanDark:
                CyanDark = color;
                break;
            case ColorNames.Blue:
                Blue = color;
                break;
            case ColorNames.BlueDark:
                BlueDark = color;
                break;
            case ColorNames.Purple:
                Purple = color;
                break;
            case ColorNames.PurpleDark:
                PurpleDark = color;
                break;
            case ColorNames.Yellow:
                Yellow = color;
                break;
            case ColorNames.YellowDark:
                YellowDark = color;
                break;
            case ColorNames.Orange:
                Orange = color;
                break;
            case ColorNames.OrangeDark:
                OrangeDark = color;
                break;
            case ColorNames.Brown:
                Brown = color;
                break;
            case ColorNames.BrownDark:
                BrownDark = color;
                break;
            case ColorNames.Gold:
                Gold = color;
                break;
            case ColorNames.GoldDark:
                GoldDark = color;
                break;
            case ColorNames.Silver:
                Silver = color;
                break;
            case ColorNames.SilverDark:
                SilverDark = color;
                break;
            case ColorNames.Bronze:
                Bronze = color;
                break;
            case ColorNames.BronzeDark:
                BronzeDark = color;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Tries to get the color name of the specified color.
    /// </summary>
    /// <param name="color">The color to match.</param>
    /// <param name="colorName">Set to the color enumeration value when the <paramref name="color"/> parameter matches a defined color.</param>
    /// <returns><see langword="true"/> if the specified color matched a defined color name; otherwise <see langword="false"/>.</returns>
    public bool TryToColorName(Color color, out ColorNames colorName)
    {
        foreach (ColorNames item in (ColorNames[])Enum.GetValues(typeof(ColorNames)))
        {
            if (FromColorName(item) == color)
            {
                colorName = item;
                return true;
            }
        }
        colorName = ColorNames.Black;
        return false;
    }

    /// <summary>
    /// Adds each color here to the <see cref="ColorExtensions2.ColorMappings"/> dictionary. Uses the format of "theme.property-name".
    /// </summary>
    public void AddToColorMappings()
    {
        ColorExtensions2.ColorMappings["theme.white"] = White;
        ColorExtensions2.ColorMappings["theme.black"] = Black;
        ColorExtensions2.ColorMappings["theme.gray"] = Gray;
        ColorExtensions2.ColorMappings["theme.graydark"] = GrayDark;

        ColorExtensions2.ColorMappings["theme.red"] = Red;
        ColorExtensions2.ColorMappings["theme.green"] = Green;
        ColorExtensions2.ColorMappings["theme.blue"] = Blue;
        ColorExtensions2.ColorMappings["theme.purple"] = Purple;
        ColorExtensions2.ColorMappings["theme.yellow"] = Yellow;
        ColorExtensions2.ColorMappings["theme.orange"] = Orange;
        ColorExtensions2.ColorMappings["theme.cyan"] = Cyan;
        ColorExtensions2.ColorMappings["theme.brown"] = Brown;

        ColorExtensions2.ColorMappings["theme.reddark"] = RedDark;
        ColorExtensions2.ColorMappings["theme.greendark"] = GreenDark;
        ColorExtensions2.ColorMappings["theme.bluedark"] = BlueDark;
        ColorExtensions2.ColorMappings["theme.purpledark"] = PurpleDark;
        ColorExtensions2.ColorMappings["theme.yellowdark"] = YellowDark;
        ColorExtensions2.ColorMappings["theme.orangedark"] = OrangeDark;
        ColorExtensions2.ColorMappings["theme.cyandark"] = CyanDark;
        ColorExtensions2.ColorMappings["theme.browndark"] = BrownDark;

        ColorExtensions2.ColorMappings["theme.gold"] = Gold;
        ColorExtensions2.ColorMappings["theme.golddark"] = GoldDark;
        ColorExtensions2.ColorMappings["theme.silver"] = Silver;
        ColorExtensions2.ColorMappings["theme.silverdark"] = SilverDark;
        ColorExtensions2.ColorMappings["theme.bronze"] = Bronze;
        ColorExtensions2.ColorMappings["theme.bronzedark"] = BronzeDark;

        ColorExtensions2.ColorMappings["theme.modalbackground"] = ModalBackground;

        ColorExtensions2.ColorMappings["theme.title"] = Title;
        ColorExtensions2.ColorMappings["theme.lines"] = Lines;

        ColorExtensions2.ColorMappings["theme.controlforegroundnormal"] = ControlForegroundNormal;
        ColorExtensions2.ColorMappings["theme.controlforegrounddisabled"] = ControlForegroundDisabled;
        ColorExtensions2.ColorMappings["theme.controlforegroundmouseover"] = ControlForegroundMouseOver;
        ColorExtensions2.ColorMappings["theme.controlforegroundmousedown"] = ControlForegroundMouseDown;
        ColorExtensions2.ColorMappings["theme.controlforegroundselected"] = ControlForegroundSelected;
        ColorExtensions2.ColorMappings["theme.controlforegroundfocused"] = ControlForegroundFocused;
        ColorExtensions2.ColorMappings["theme.controlbackgroundnormal"] = ControlBackgroundNormal;
        ColorExtensions2.ColorMappings["theme.controlbackgrounddisabled"] = ControlBackgroundDisabled;
        ColorExtensions2.ColorMappings["theme.controlbackgroundmouseover"] = ControlBackgroundMouseOver;
        ColorExtensions2.ColorMappings["theme.controlbackgroundmousedown"] = ControlBackgroundMouseDown;
        ColorExtensions2.ColorMappings["theme.controlbackgroundselected"] = ControlBackgroundSelected;
        ColorExtensions2.ColorMappings["theme.controlbackgroundfocused"] = ControlBackgroundFocused;
        ColorExtensions2.ColorMappings["theme.controlhostforeground"] = ControlHostForeground;
        ColorExtensions2.ColorMappings["theme.controlhostbackground"] = ControlHostBackground;
    }

    /// <summary>
    /// Returns a clone of this object.
    /// </summary>
    /// <returns>A new Colors object.</returns>
    public Colors Clone()
    {
        Colors newColors = new Colors();
        CopyTo(newColors);
        return newColors;
    }
}

