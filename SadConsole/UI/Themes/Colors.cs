using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// Default colors used by control themes.
    /// </summary>
    public class Colors
    {
        public bool IsLibrary { get; internal set; }

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

        public void CopyTo(ref Colors colors)
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
            CopyTo(ref newColors);
            return newColors;
        }
    }
}

