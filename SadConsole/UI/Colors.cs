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
        /// Indicates that this color object is set to a library object.
        /// </summary>
        public bool IsLibrary { get; internal set; }

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

        //TODO: Change to adjustable color.
        public AdjustableColor TitleText { get; set; }
        public AdjustableColor Lines { get; set; }
        public AdjustableColor TextBright { get; set; }
        public AdjustableColor Text { get; set; }
        public AdjustableColor TextSelected { get; set; }
        public AdjustableColor TextSelectedDark { get; set; }
        public AdjustableColor TextLight { get; set; }
        public AdjustableColor TextDark { get; set; }
        public AdjustableColor TextFocused { get; set; }
        public AdjustableColor ControlBack { get; set; }
        public AdjustableColor ControlBackLight { get; set; }
        public AdjustableColor ControlBackMouseOver { get; set; }
        public AdjustableColor ControlBackSelected { get; set; }
        public AdjustableColor ControlBackDark { get; set; }
        public AdjustableColor ControlHostBack { get; set; }
        public AdjustableColor ControlHostFore { get; set; }


        public ColoredGlyph Appearance_ControlNormal { get; set; } = new ColoredGlyph();
        public ColoredGlyph Appearance_ControlDisabled { get; set; } = new ColoredGlyph();
        public ColoredGlyph Appearance_ControlOver { get; set; } = new ColoredGlyph();
        public ColoredGlyph Appearance_ControlSelected { get; set; } = new ColoredGlyph();
        public ColoredGlyph Appearance_ControlMouseDown { get; set; } = new ColoredGlyph();
        public ColoredGlyph Appearance_ControlFocused { get; set; } = new ColoredGlyph();

        /// <summary>
        /// Creates a color object based on the <see cref="Colors.CreateAnsi"/> scheme.
        /// </summary>
        public Colors()
        {
            var colors = CreateAnsi();

            colors.CopyTo(this);
        }

        /// <summary>
        /// Internal constructor to avoid creating any predefined colors.
        /// </summary>
        /// <param name="_">Discarded parameter</param>
        protected Colors(int _) { }

        /// <summary>
        /// Sets all Appearance* properties based on the existing colors and settings.
        /// </summary>
        public void RebuildAppearances()
        {
            Appearance_ControlNormal = new ColoredGlyph(Text, ControlBack);
            Appearance_ControlDisabled = new ColoredGlyph(TextDark, ControlBackDark);
            Appearance_ControlOver = new ColoredGlyph(TextSelectedDark, ControlBackMouseOver);
            Appearance_ControlSelected = new ColoredGlyph(TextSelected, ControlBackSelected);
            Appearance_ControlMouseDown = new ColoredGlyph(ControlBackSelected, TextSelected);
            Appearance_ControlFocused = new ColoredGlyph(TextFocused, ControlBackLight);
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

            colors.TitleText = TitleText.Clone();
            colors.Lines = Lines.Clone();
            colors.TextBright = TextBright.Clone();
            colors.Text = Text.Clone();
            colors.TextSelected = TextSelected.Clone();
            colors.TextSelectedDark = TextSelectedDark.Clone();
            colors.TextLight = TextLight.Clone();
            colors.TextDark = TextDark.Clone();
            colors.TextFocused = TextFocused.Clone();
            colors.ControlBack = ControlBack.Clone();
            colors.ControlBackLight = ControlBackLight.Clone();
            colors.ControlBackMouseOver = ControlBackMouseOver.Clone();
            colors.ControlBackSelected = ControlBackSelected.Clone();
            colors.ControlBackDark = ControlBackDark.Clone();
            colors.ControlHostBack = ControlHostBack.Clone();
            colors.ControlHostFore = ControlHostFore.Clone();

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
        /// Adds each color here to the <see cref="ColorExtensions.ColorMappings"/> dictionary. Uses the format of "theme.property-name".
        /// </summary>
        public void AddToColorMappings()
        {
            ColorExtensions.ColorMappings["theme.white"] = White;
            ColorExtensions.ColorMappings["theme.black"] = Black;
            ColorExtensions.ColorMappings["theme.gray"] = Gray;
            ColorExtensions.ColorMappings["theme.graydark"] = GrayDark;

            ColorExtensions.ColorMappings["theme.red"] = Red;
            ColorExtensions.ColorMappings["theme.green"] = Green;
            ColorExtensions.ColorMappings["theme.blue"] = Blue;
            ColorExtensions.ColorMappings["theme.purple"] = Purple;
            ColorExtensions.ColorMappings["theme.yellow"] = Yellow;
            ColorExtensions.ColorMappings["theme.orange"] = Orange;
            ColorExtensions.ColorMappings["theme.cyan"] = Cyan;
            ColorExtensions.ColorMappings["theme.brown"] = Brown;

            ColorExtensions.ColorMappings["theme.reddark"] = RedDark;
            ColorExtensions.ColorMappings["theme.greendark"] = GreenDark;
            ColorExtensions.ColorMappings["theme.bluedark"] = BlueDark;
            ColorExtensions.ColorMappings["theme.purpledark"] = PurpleDark;
            ColorExtensions.ColorMappings["theme.yellowdark"] = YellowDark;
            ColorExtensions.ColorMappings["theme.orangedark"] = OrangeDark;
            ColorExtensions.ColorMappings["theme.cyandark"] = CyanDark;
            ColorExtensions.ColorMappings["theme.browndark"] = BrownDark;

            ColorExtensions.ColorMappings["theme.gold"] = Gold;
            ColorExtensions.ColorMappings["theme.golddark"] = GoldDark;
            ColorExtensions.ColorMappings["theme.silver"] = Silver;
            ColorExtensions.ColorMappings["theme.silverdark"] = SilverDark;
            ColorExtensions.ColorMappings["theme.bronze"] = Bronze;
            ColorExtensions.ColorMappings["theme.bronzedark"] = BronzeDark;

            ColorExtensions.ColorMappings["theme.modalbackground"] = ModalBackground;

            ColorExtensions.ColorMappings["theme.titletext"] = TitleText;

            ColorExtensions.ColorMappings["theme.lines"] = Lines;

            ColorExtensions.ColorMappings["theme.textbright"] = TextBright;
            ColorExtensions.ColorMappings["theme.text"] = Text;
            ColorExtensions.ColorMappings["theme.textselected"] = TextSelected;
            ColorExtensions.ColorMappings["theme.textselecteddark"] = TextSelectedDark;
            ColorExtensions.ColorMappings["theme.textlight"] = TextLight;
            ColorExtensions.ColorMappings["theme.textdark"] = TextDark;
            ColorExtensions.ColorMappings["theme.textfocused"] = TextFocused;

            ColorExtensions.ColorMappings["theme.controlback"] = ControlBack;
            ColorExtensions.ColorMappings["theme.controlbacklight"] = ControlBackLight;
            ColorExtensions.ColorMappings["theme.controlbackmouseover"] = ControlBackMouseOver;
            ColorExtensions.ColorMappings["theme.controlbackselected"] = ControlBackSelected;
            ColorExtensions.ColorMappings["theme.controlbackdark"] = ControlBackDark;
            ColorExtensions.ColorMappings["theme.controlhostback"] = ControlHostBack;
            ColorExtensions.ColorMappings["theme.controlhostfore"] = ControlHostFore;
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
}

