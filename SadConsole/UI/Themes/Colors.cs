using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// Default colors used by control themes.
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// Indicates that this color object is set to a library object.
        /// </summary>
        public bool IsLibrary { get; internal set; }

        /// <summary>
        /// The white color. Defaults to <see cref="Color.White"/>.
        /// </summary>
        public Color White { get; set; } = Color.White;

        /// <summary>
        /// The black color. Defaults to <see cref="Color.Black"/>.
        /// </summary>
        public Color Black { get; set; } = Color.Black;

        /// <summary>
        /// The gray color. Defaults to (176, 196, 222).
        /// </summary>
        public Color Gray { get; set; } = new Color(176, 196, 222);

        /// <summary>
        /// The dark gray color. Defaults to (66, 66, 66).
        /// </summary>
        public Color GrayDark { get; set; } = new Color(66, 66, 66);

        /// <summary>
        /// The red color. Defaults to (255, 0, 51).
        /// </summary>
        public Color Red { get; set; } = new Color(255, 0, 51);

        /// <summary>
        /// The green color. Defaults to (153, 224, 0).
        /// </summary>
        public Color Green { get; set; } = new Color(153, 224, 0);

        /// <summary>
        /// The blue color. Defaults to (102, 204, 255).
        /// </summary>
        public Color Blue { get; set; } = new Color(102, 204, 255);

        /// <summary>
        /// The purple color. Defaults to (132, 0, 214).
        /// </summary>
        public Color Purple { get; set; } = new Color(132, 0, 214);

        /// <summary>
        /// The yellow color. Defaults to (255, 255, 102).
        /// </summary>
        public Color Yellow { get; set; } = new Color(255, 255, 102);

        /// <summary>
        /// The orange color. Defaults to (255, 153, 0).
        /// </summary>
        public Color Orange { get; set; } = new Color(255, 153, 0);

        /// <summary>
        /// The cyan color. Defaults to (82, 242, 234).
        /// </summary>
        public Color Cyan { get; set; } = new Color(82, 242, 234);

        /// <summary>
        /// The brown color. Defaults to (100, 59, 15).
        /// </summary>
        public Color Brown { get; set; } = new Color(100, 59, 15);

        /// <summary>
        /// The dark red color. Defaults to (153, 51, 51).
        /// </summary>
        public Color RedDark { get; set; } = new Color(153, 51, 51);

        /// <summary>
        /// The dark green color. Defaults to (110, 166, 23).
        /// </summary>
        public Color GreenDark { get; set; } = new Color(110, 166, 23);

        /// <summary>
        /// The dark blue color. Defaults to (51, 102, 153).
        /// </summary>
        public Color BlueDark { get; set; } = new Color(51, 102, 153);

        /// <summary>
        /// The dark purple color. Defaults to (70, 0, 114).
        /// </summary>
        public Color PurpleDark { get; set; } = new Color(70, 0, 114);

        /// <summary>
        /// The dark yellow color. Defaults to (255, 207, 15).
        /// </summary>
        public Color YellowDark { get; set; } = new Color(255, 207, 15);

        /// <summary>
        /// The dark orange color. Defaults to (255, 102, 0).
        /// </summary>
        public Color OrangeDark { get; set; } = new Color(255, 102, 0);

        /// <summary>
        /// The dark cyan color. Defaults to (33, 182, 168).
        /// </summary>
        public Color CyanDark { get; set; } = new Color(33, 182, 168);

        /// <summary>
        /// The dark brown color. Defaults to (119, 17, 0).
        /// </summary>
        public Color BrownDark { get; set; } = new Color(119, 17, 0);

        /// <summary>
        /// The gold color. Defaults to (255, 215, 0).
        /// </summary>
        public Color Gold { get; set; } = new Color(255, 215, 0);

        /// <summary>
        /// The dark gold color. Defaults to (127, 107, 0).
        /// </summary>
        public Color GoldDark { get; set; } = new Color(127, 107, 0);

        /// <summary>
        /// The silver color. Defaults to (192, 192, 192).
        /// </summary>
        public Color Silver { get; set; } = new Color(192, 192, 192);

        /// <summary>
        /// The dark silver color. Defaults to (169, 169, 169).
        /// </summary>
        public Color SilverDark { get; set; } = new Color(169, 169, 169);

        /// <summary>
        /// The bronze color. Defaults to (205, 127, 50).
        /// </summary>
        public Color Bronze { get; set; } = new Color(205, 127, 50);

        /// <summary>
        /// The dark bronze color. Defaults to (144, 89, 35).
        /// </summary>
        public Color BronzeDark { get; set; } = new Color(144, 89, 35);

        /// <summary>
        /// The color used to darken the background when <see cref="o:UI.Window.Show"/> is called and <see cref="UI.Window.IsModal"/> is <see langword="true"/>. Defaults to (20, 20, 20, 200).
        /// </summary>
        public Color ModalBackground { get; set; } = new Color(20, 20, 20, 200);

        public Color TitleText;
        public Color Lines;
        public Color TextBright;
        public Color Text;
        public Color TextSelected;
        public Color TextSelectedDark;
        public Color TextLight;
        public Color TextDark;
        public Color TextFocused;
        public Color ControlBack;
        public Color ControlBackLight;
        public Color ControlBackSelected;
        public Color ControlBackDark;
        public Color ControlHostBack;
        public Color ControlHostFore;
        public ColoredGlyph Appearance_ControlNormal;
        public ColoredGlyph Appearance_ControlDisabled;
        public ColoredGlyph Appearance_ControlOver;
        public ColoredGlyph Appearance_ControlSelected;
        public ColoredGlyph Appearance_ControlMouseDown;
        public ColoredGlyph Appearance_ControlFocused;


        public Colors()
        {
            TitleText = Orange;

            TextBright = White;
            Text = Blue;
            TextSelected = Yellow;
            TextSelectedDark = Green;
            TextLight = Gray;
            TextDark = Green;
            TextFocused = Cyan;

            Lines = Gray;

            ControlBack = BlueDark;
            ControlBackLight = (ControlBack * 1.3f).FillAlpha();
            ControlBackSelected = GreenDark;
            ControlBackDark = (ControlBack * 0.7f).FillAlpha();
            ControlHostBack = BlueDark;
            ControlHostFore = Text;

            RebuildAppearances();
        }

        /// <summary>
        /// Sets all Appearance* properties based on the existing colors and settings.
        /// </summary>
        public void RebuildAppearances()
        {
            Appearance_ControlNormal = new ColoredGlyph(Text, ControlBack);
            Appearance_ControlDisabled = new ColoredGlyph(TextLight, ControlBackDark);
            Appearance_ControlOver = new ColoredGlyph(TextSelectedDark, ControlBackSelected);
            Appearance_ControlSelected = new ColoredGlyph(TextSelected, ControlBackSelected);
            Appearance_ControlMouseDown = new ColoredGlyph(ControlBackSelected, TextSelected);
            Appearance_ControlFocused = new ColoredGlyph(TextFocused, ControlBackLight);
        }

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

            colors.TitleText = TitleText;

            colors.Lines = Lines;

            colors.TextBright = TextBright;
            colors.Text = Text;
            colors.TextSelected = TextSelected;
            colors.TextSelectedDark = TextSelectedDark;
            colors.TextLight = TextLight;
            colors.TextDark = TextDark;
            colors.TextFocused = TextFocused;

            colors.ControlBack = ControlBack;
            colors.ControlBackLight = ControlBackLight;
            colors.ControlBackSelected = ControlBackSelected;
            colors.ControlBackDark = ControlBackDark;
            colors.ControlHostBack = ControlHostBack;
            colors.ControlHostFore = ControlHostFore;

            colors.Appearance_ControlNormal.CopyAppearanceFrom(Appearance_ControlNormal);
            colors.Appearance_ControlDisabled.CopyAppearanceFrom(Appearance_ControlDisabled);
            colors.Appearance_ControlOver.CopyAppearanceFrom(Appearance_ControlOver);
            colors.Appearance_ControlSelected.CopyAppearanceFrom(Appearance_ControlSelected);
            colors.Appearance_ControlMouseDown.CopyAppearanceFrom(Appearance_ControlMouseDown);
            colors.Appearance_ControlFocused.CopyAppearanceFrom(Appearance_ControlFocused);
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

        /// <summary>
        /// Creates a new colors object with the default blue colors theme of SadConsole.
        /// </summary>
        /// <returns></returns>
        public static Colors CreateDefault() =>
            new Colors();

        /// <summary>
        /// Creates a new colors object with a standard black-based theme.
        /// </summary>
        /// <returns></returns>
        public static Colors CreateFromAnsi()
        {
            // Create a new 
            var colors = new Colors()
            {
                White = Color.White,
                Black = Color.Black,
                Gray = ColorAnsi.White,
                GrayDark = (ColorAnsi.White * 0.50f).FillAlpha(),
                Red = ColorAnsi.RedBright,
                Green = ColorAnsi.GreenBright,
                Blue = ColorAnsi.BlueBright,
                Purple = ColorAnsi.MagentaBright,
                Yellow = ColorAnsi.YellowBright,
                Orange = Color.Orange,
                Cyan = ColorAnsi.CyanBright,
                Brown = ColorAnsi.Yellow,
                RedDark = ColorAnsi.Red,
                GreenDark = ColorAnsi.Green,
                BlueDark = ColorAnsi.Blue,
                PurpleDark = ColorAnsi.Magenta,
                YellowDark = (ColorAnsi.YellowBright * 0.50f).FillAlpha(),
                OrangeDark = Color.DarkOrange,
                CyanDark = ColorAnsi.Cyan,
                BrownDark = (ColorAnsi.Yellow * 0.50f).FillAlpha(),
                Gold = Color.Goldenrod,
                GoldDark = Color.DarkGoldenrod,
                Silver = Color.Silver,
                SilverDark = (Color.Silver * 0.50f).FillAlpha(),
                Bronze = new Color(205, 127, 50),
                BronzeDark = (new Color(205, 127, 50) * 0.50f).FillAlpha(),
            };

            // TODO: Set every color property.

            // Overwrite some of the control elements
            colors.TitleText = colors.Orange;
            colors.TextBright = colors.White;
            colors.Text = colors.Blue;
            colors.TextSelected = colors.Yellow;
            colors.TextSelectedDark = colors.YellowDark;
            colors.TextLight = colors.Gray;
            colors.TextDark = colors.GrayDark;
            colors.TextFocused = colors.White;

            colors.ControlBack = colors.Black;
            colors.ControlBackLight = (colors.ControlBack * 1.3f).FillAlpha();
            colors.ControlBackSelected = colors.GreenDark;
            colors.ControlBackDark = (colors.ControlBack * 0.7f).FillAlpha();
            colors.ControlHostBack = colors.Black;
            colors.ControlHostFore = colors.Text;

            // Rebuild the controls
            colors.RebuildAppearances();

            return colors;
        }
    }
}

