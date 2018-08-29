using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SadConsole.Themes
{
    /// <summary>
    /// Default colors used by control themes.
    /// </summary>
    public static class Colors
    {
        public static Color White = Color.White;
        public static Color Black = Color.Black;
        public static Color Gray = new Color(176, 196, 222);

        public static Color GrayDark = new Color(66,66,66);

        public static Color Red = new Color(255, 0, 51);
        public static Color Green = new Color(153, 224, 0);
        public static Color Blue = new Color(102, 204, 255);
        public static Color Purple = new Color(132, 0, 214);
        public static Color Yellow = new Color(255, 255, 102);
        public static Color Orange = new Color(255, 153, 0);
        public static Color Cyan = new Color(82, 242, 234);
        public static Color Brown = new Color(100, 59, 15);

        public static Color RedDark = new Color(153, 51, 51);
        public static Color GreenDark = new Color(110, 166, 23);
        public static Color BlueDark = new Color(51, 102, 153);
        public static Color PurpleDark = new Color(70, 0, 114);
        public static Color YellowDark = new Color(255, 207, 15);
        public static Color OrangeDark = new Color(255, 102, 0);
        public static Color CyanDark = new Color(33, 182, 168);
        public static Color BrownDark = new Color(119, 17, 0);

        public static Color Gold = new Color(255, 215, 0);
        public static Color GoldDark = new Color(127, 107, 0);
        public static Color Silver = new Color(192, 192, 192);
        public static Color SilverDark = new Color(169, 169, 169);
        public static Color Bronze = new Color(205, 127, 50);
        public static Color BronzeDark = new Color(144, 89, 35);


        public static Color MenuBack = BlueDark;
        public static Color MenuLines = Gray;
        public static Color TitleText = Orange;
        public static Color ModalBackground = new Color(20, 20, 20);

        public static Color TextBright = White;
        public static Color Text = Blue;
        public static Color TextSelected = Yellow;
        public static Color TextSelectedDark = Green;
        public static Color TextLight = Gray;
        public static Color TextDark = Green;
        public static Color ControlBack = MenuBack;
        public static Color ControlBackLight = (ControlBack * 1.3f).FillAlpha();
        public static Color ControlBackSelected = GreenDark;
        public static Color ControlBackDark = (ControlBack * 0.7f).FillAlpha();
        public static Color ControlHostBack = MenuBack;
        public static Color ControlHostFore = Text;
    }
}
