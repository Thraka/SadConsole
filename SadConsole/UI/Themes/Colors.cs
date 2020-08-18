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

        public Color TitleText { get; set; }
        public Color Lines { get; set; }
        public Color TextBright { get; set; }
        public Color Text { get; set; }
        public Color TextSelected { get; set; }
        public Color TextSelectedDark { get; set; }
        public Color TextLight { get; set; }
        public Color TextDark { get; set; }
        public Color TextFocused { get; set; }
        public Color ControlBack { get; set; }
        public Color ControlBackLight { get; set; }
        public Color ControlBackSelected { get; set; }
        public Color ControlBackDark { get; set; }
        public Color ControlHostBack { get; set; }
        public Color ControlHostFore { get; set; }
        public ColoredGlyph Appearance_ControlNormal { get; set; }
        public ColoredGlyph Appearance_ControlDisabled { get; set; }
        public ColoredGlyph Appearance_ControlOver { get; set; }
        public ColoredGlyph Appearance_ControlSelected { get; set; }
        public ColoredGlyph Appearance_ControlMouseDown { get; set; }
        public ColoredGlyph Appearance_ControlFocused { get; set; }


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
            Appearance_ControlDisabled = new ColoredGlyph(TextDark, ControlBackDark);
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
        public static Colors CreateAnsi()
        {
            // Create a new 
            var colors = new Colors()
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

            // TODO: Set every color property.

            // Overwrite some of the control elements
            colors.TitleText = colors.Orange;
            colors.TextBright = colors.White;
            colors.Text = colors.Gray;
            colors.TextSelected = colors.Yellow;
            colors.TextSelectedDark = colors.YellowDark;
            colors.TextLight = colors.White;
            colors.TextDark = colors.GrayDark;
            colors.TextFocused = colors.White;
            colors.Lines = colors.CyanDark;

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

