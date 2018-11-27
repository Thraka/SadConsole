using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SadConsole.Themes
{
    /// <summary>
    /// Default colors used by control themes.
    /// </summary>
    public class Colors
    {
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

        public Color ModalBackground = new Color(20, 20, 20, 200);

        public Color MenuBack;
        public Color MenuLines;
        public Color TitleText;

        public Color TextBright;
        public Color Text;
        public Color TextSelected;
        public Color TextSelectedDark;
        public Color TextLight;
        public Color TextDark;
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


        public Colors()
        {
            MenuBack = BlueDark;
            MenuLines = Gray;
            TitleText = Orange;

            TextBright = White;
            Text = Blue;
            TextSelected = Yellow;
            TextSelectedDark = Green;
            TextLight = Gray;
            TextDark = Green;

            RebuildAppearances();
        }

        public void RebuildAppearances()
        {
            ControlBack = MenuBack;
            ControlBackLight = (ControlBack * 1.3f).FillAlpha();
            ControlBackSelected = GreenDark;
            ControlBackDark = (ControlBack * 0.7f).FillAlpha();
            ControlHostBack = MenuBack;
            ControlHostFore = Text;

            Appearance_ControlNormal = new Cell(Text, ControlBack);
            Appearance_ControlDisabled = new Cell(TextLight, ControlBackDark);
            Appearance_ControlOver = new Cell(TextSelectedDark, ControlBackSelected);
            Appearance_ControlSelected = new Cell(TextSelected, ControlBackSelected);
            Appearance_ControlMouseDown = new Cell(Appearance_ControlSelected.Background, Appearance_ControlSelected.Foreground);
            Appearance_ControlFocused = new Cell(Cyan, ControlBackLight);
        }

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>A new Colors object.</returns>
        public Colors Clone()
        {
            return new Colors()
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

                MenuBack = MenuBack,
                MenuLines = MenuLines,
                TitleText = TitleText,

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
        }
    }
}

