#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.Themes
{
    /// <summary>
    /// Default colors used by control themes.
    /// </summary>
    public class Colors
    {
#pragma warning disable 1591
        public Color White = Color.White;
        public Color Black = Color.Black;
        public Color Gray = new Color(176, 196, 222);

        public Color GrayDark = new Color(66, 66, 66);

        public Color Red = new Color(255, 0, 51);
        public Color Green = new Color(153, 224, 0);
        public Color Blue = new Color(102, 204, 255);
        public Color Purple = new Color(132, 0, 214);
        public Color Yellow = new Color(255, 255, 102);
        public Color Orange = new Color(255, 153, 0);
        public Color Cyan = new Color(82, 242, 234);
        public Color Brown = new Color(100, 59, 15);

        public Color RedDark = new Color(153, 51, 51);
        public Color GreenDark = new Color(110, 166, 23);
        public Color BlueDark = new Color(51, 102, 153);
        public Color PurpleDark = new Color(70, 0, 114);
        public Color YellowDark = new Color(255, 207, 15);
        public Color OrangeDark = new Color(255, 102, 0);
        public Color CyanDark = new Color(33, 182, 168);
        public Color BrownDark = new Color(119, 17, 0);

        public Color Gold = new Color(255, 215, 0);
        public Color GoldDark = new Color(127, 107, 0);
        public Color Silver = new Color(192, 192, 192);
        public Color SilverDark = new Color(169, 169, 169);
        public Color Bronze = new Color(205, 127, 50);
        public Color BronzeDark = new Color(144, 89, 35);
#pragma warning restore 1591

        /// <summary>
        /// The color used to darken the background when <see cref="o:Window.Show"/> is called and <see cref="Window.IsModal"/> is <see langword="true"/>.
        /// </summary>
        public Color ModalBackground = new Color(20, 20, 20, 200);

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

        public Cell Appearance_ControlNormal;
        public Cell Appearance_ControlDisabled;
        public Cell Appearance_ControlOver;
        public Cell Appearance_ControlSelected;
        public Cell Appearance_ControlMouseDown;
        public Cell Appearance_ControlFocused;

        /// <summary>
        /// Creates a new colors object with the default blue theme of SadConsole.
        /// </summary>
        public Colors() =>
            SetMemberValues();

        /// <summary>
        /// Sets the color values of all members, such as <see cref="Text"/>, to the current color values, and then calls <see cref="RebuildAppearances"/>.
        /// </summary>
        public void SetMemberValues()
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
            Appearance_ControlNormal = new Cell(Text, ControlBack);
            Appearance_ControlDisabled = new Cell(TextLight, ControlBackDark);
            Appearance_ControlOver = new Cell(TextSelectedDark, ControlBackSelected);
            Appearance_ControlSelected = new Cell(TextSelected, ControlBackSelected);
            Appearance_ControlMouseDown = new Cell(ControlBackSelected, TextSelected);
            Appearance_ControlFocused = new Cell(TextFocused, ControlBackLight);
        }

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>A new Colors object.</returns>
        public Colors Clone() => new Colors()
        {
            White = White,
            Black = Black,
            Gray = Gray,
            GrayDark = GrayDark,

            Red = Red,
            Green = Green,
            Blue = Blue,
            Purple = Purple,
            Yellow = Yellow,
            Orange = Orange,
            Cyan = Cyan,
            Brown = Brown,

            RedDark = RedDark,
            GreenDark = GreenDark,
            BlueDark = BlueDark,
            PurpleDark = PurpleDark,
            YellowDark = YellowDark,
            OrangeDark = OrangeDark,
            CyanDark = CyanDark,
            BrownDark = BrownDark,

            Gold = Gold,
            GoldDark = GoldDark,
            Silver = Silver,
            SilverDark = SilverDark,
            Bronze = Bronze,
            BronzeDark = BronzeDark,

            ModalBackground = ModalBackground,

            TitleText = TitleText,

            Lines = Lines,

            TextBright = TextBright,
            Text = Text,
            TextSelected = TextSelected,
            TextSelectedDark = TextSelectedDark,
            TextLight = TextLight,
            TextDark = TextDark,
            ControlBack = ControlBack,
            ControlBackLight = ControlBackLight,
            ControlBackSelected = ControlBackSelected,
            ControlBackDark = ControlBackDark,
            ControlHostBack = ControlHostBack,
            ControlHostFore = ControlHostFore,

            Appearance_ControlNormal = Appearance_ControlNormal.Clone(),
            Appearance_ControlDisabled = Appearance_ControlDisabled.Clone(),
            Appearance_ControlOver = Appearance_ControlOver.Clone(),
            Appearance_ControlSelected = Appearance_ControlSelected.Clone(),
            Appearance_ControlMouseDown = Appearance_ControlMouseDown.Clone(),
            Appearance_ControlFocused = Appearance_ControlFocused.Clone(),
        };

        /// <summary>
        /// Creates a new colors object with the default blue colors theme of SadConsole.
        /// </summary>
        /// <returns>A colors object with the default colors.</returns>
        public static Colors CreateDefault() =>
            new Colors();

        /// <summary>
        /// Creates a new colors object with a standard black-based theme.
        /// </summary>
        /// <returns>A colors object based on classic ANSI colors.</returns>
        public static Colors CreateAnsi()
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

            // Set the default values for controls and control elements
            colors.SetMemberValues();

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
            //colors.ControlBackSelected = GreenDark;
            colors.ControlBackDark = (colors.ControlBack * 0.7f).FillAlpha();
            colors.ControlHostBack = colors.Black;
            colors.ControlHostFore = colors.Text;

            // Rebuild the controls
            colors.RebuildAppearances();

            return colors;
        }
    }
}

