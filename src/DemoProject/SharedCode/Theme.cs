using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

namespace StarterProject
{
    public class Theme
    {
        #region Colors
        public static Color White = ColorHelper.White;
        public static Color Black = ColorHelper.Black;

        public static Color Red = new Color(255, 0, 51);
        public static Color Blue = new Color(102, 204, 255);
        public static Color Green = new Color(153, 224, 0);
        public static Color Purple = new Color(132, 0, 214);
        public static Color Gray = new Color(176, 196, 222);
        public static Color Yellow = new Color(255, 255, 102);
        public static Color Orange = new Color(255, 153, 0);
        public static Color Cyan = new Color(82, 242, 234);
        public static Color Brown = new Color(100, 59, 15);
        
        public static Color RedDark = new Color(153, 51, 51);
        public static Color BlueDark = new Color(51, 102, 153);
        public static Color GreenDark = new Color(110, 166, 23);
        public static Color PurpleDark = new Color(70, 0, 114);
        public static Color GrayDark = new Color(94, 94, 94);
        public static Color YellowDark = new Color(255, 207, 15);
        public static Color OrangeDark = new Color(255, 102, 0);
        public static Color CyanDark = new Color(33, 182, 168);
        public static Color BrownDark = new Color(119, 17, 0);

        public static Color Gold = new Color(255, 215, 0);
        public static Color GoldDark = new Color(127, 107, 0);


        public static Color Color_MenuBack = BlueDark;
        public static Color Color_MenuLines = Gray;
        public static Color Color_TitleText = Orange;
        public static Color Color_WorkAreaBack = new Color(20, 20, 20);

        public static Color Color_TextBright = White;
        public static Color Color_Text = Blue;
        public static Color Color_TextSelected = Yellow;
        public static Color Color_TextSelectedDark = Green;
        public static Color Color_TextDim = Gray;
        public static Color Color_TextDark = Green;
        public static Color Color_ControlBack = Color_MenuBack;
        public static Color Color_ControlBackDim = CyanDark;
        public static Color Color_ControlBackSelected = GreenDark;

        public static SadConsole.Cell Appearance_ListBoxItem_Normal = new SadConsole.Cell(Color_Text, Color_ControlBack);
        public static SadConsole.Cell Appearance_ListBoxItem_SelectedItem = new SadConsole.Cell(Yellow, Color_ControlBack);


        public static SadConsole.Cell Appearance_ControlNormal = new SadConsole.Cell(Color_Text, Color_ControlBack);
        public static SadConsole.Cell Appearance_ControlDisabled = new SadConsole.Cell(Color_TextDim, Color_ControlBack);
        public static SadConsole.Cell Appearance_ControlOver = new SadConsole.Cell(Color_TextSelectedDark, Color_ControlBackSelected);
        public static SadConsole.Cell Appearance_ControlSelected = new SadConsole.Cell(Color_TextSelected, Color_ControlBackSelected);
        public static SadConsole.Cell Appearance_ControlMouseDown = new SadConsole.Cell(Color_ControlBackSelected, Color_TextSelectedDark);
        public static SadConsole.Cell Appearance_ControlFocused = new SadConsole.Cell(Cyan, Color_ControlBackDim);

        //public static SadConsole.Cell Appearance_ControlTextBoxNormal = new SadConsole.Cell(Color_Text, ClearAlpha(new Color((byte)(Color_ControlBack.R * 0.7f), (byte)(Color_ControlBack.R * 0.7f), (byte)(Color_ControlBack.R * 0.7f), 255)));

        public static SadConsole.Themes.RadioButtonTheme NoCheckRadioButtonTheme;
        #endregion

        private static Color ClearAlpha(Color color)
        {
            color.A = 255;
            return color;
        }

        public static void SetupThemes()
        {
            SadConsole.Themes.Library.Default.RadioButtonTheme.Button.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.RadioButtonTheme.Button.MouseOver = Appearance_ControlOver;
            SadConsole.Themes.Library.Default.RadioButtonTheme.Button.Selected = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.RadioButtonTheme.Button.Focused = Appearance_ControlFocused;

            SadConsole.Themes.Library.Default.RadioButtonTheme.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.RadioButtonTheme.MouseOver = Appearance_ControlOver;
            SadConsole.Themes.Library.Default.RadioButtonTheme.Selected = Appearance_ListBoxItem_SelectedItem;
            SadConsole.Themes.Library.Default.RadioButtonTheme.Focused = Appearance_ControlFocused;

            SadConsole.Themes.Library.Default.CheckBoxTheme.Button.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.CheckBoxTheme.Button.MouseOver = Appearance_ControlOver;
            SadConsole.Themes.Library.Default.CheckBoxTheme.Button.Selected = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.CheckBoxTheme.Button.Focused = Appearance_ControlFocused;

            SadConsole.Themes.Library.Default.CheckBoxTheme.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.CheckBoxTheme.MouseOver = Appearance_ControlOver;
            SadConsole.Themes.Library.Default.CheckBoxTheme.Selected = Appearance_ListBoxItem_SelectedItem;
            SadConsole.Themes.Library.Default.CheckBoxTheme.Focused = Appearance_ControlFocused;

            NoCheckRadioButtonTheme = (SadConsole.Themes.RadioButtonTheme)SadConsole.Themes.Library.Default.RadioButtonTheme.Clone();
            NoCheckRadioButtonTheme.Button.Selected = Appearance_ControlSelected;
            NoCheckRadioButtonTheme.Selected = Appearance_ControlFocused;

            SadConsole.Themes.Library.Default.ButtonTheme.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.ButtonTheme.Disabled = Appearance_ControlDisabled;
            SadConsole.Themes.Library.Default.ButtonTheme.MouseOver = Appearance_ControlOver;
            SadConsole.Themes.Library.Default.ButtonTheme.MouseClicking = Appearance_ControlMouseDown;
            SadConsole.Themes.Library.Default.ButtonTheme.Focused = Appearance_ControlFocused;

            SadConsole.Themes.Library.Default.InputBoxTheme.Normal = new SadConsole.Cell(Appearance_ControlNormal.Background, Appearance_ControlNormal.Foreground);
            SadConsole.Themes.Library.Default.InputBoxTheme.Focused = Appearance_ControlFocused;
            SadConsole.Themes.Library.Default.InputBoxTheme.MouseOver = Appearance_ControlOver;

            SadConsole.Themes.Library.Default.ListBoxTheme.Border.Background = Color_ControlBack;
            SadConsole.Themes.Library.Default.ListBoxTheme.Border.Foreground = Color_Text;
            SadConsole.Themes.Library.Default.ListBoxTheme.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.ListBoxTheme.MouseOver = Appearance_ControlOver;
            SadConsole.Themes.Library.Default.ListBoxTheme.Item.Selected = Appearance_ListBoxItem_SelectedItem;
            SadConsole.Themes.Library.Default.ListBoxTheme.Item.Normal = Appearance_ListBoxItem_Normal;
            SadConsole.Themes.Library.Default.ListBoxTheme.Item.MouseOver = Appearance_ControlOver;
            SadConsole.Themes.Library.Default.ListBoxTheme.ScrollBarTheme.Bar.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.ListBoxTheme.ScrollBarTheme.Ends.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.ListBoxTheme.ScrollBarTheme.Slider.Normal = Appearance_ControlNormal;

            SadConsole.Themes.Library.Default.WindowTheme.BorderStyle.Background = Color_MenuBack;
            SadConsole.Themes.Library.Default.WindowTheme.BorderStyle.Foreground = Color_Text;
            SadConsole.Themes.Library.Default.WindowTheme.TitleStyle.Background = Color_MenuBack;
            SadConsole.Themes.Library.Default.WindowTheme.TitleStyle.Foreground = Color_TitleText;
            SadConsole.Themes.Library.Default.WindowTheme.FillStyle.Background = Color_MenuBack;
            SadConsole.Themes.Library.Default.WindowTheme.FillStyle.Foreground = Color_Text;

            SadConsole.Themes.Library.Default.ControlsConsoleTheme.FillStyle.Background = Color_MenuBack;
            SadConsole.Themes.Library.Default.ControlsConsoleTheme.FillStyle.Foreground = Color_Text;

            SadConsole.Themes.Library.Default.ScrollBarTheme.Bar.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.ScrollBarTheme.Ends.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.ScrollBarTheme.Slider.Normal = Appearance_ControlNormal;

            SadConsole.Themes.Library.Default.SelectionButtonTheme.Normal = Appearance_ControlNormal;
            SadConsole.Themes.Library.Default.SelectionButtonTheme.Disabled = Appearance_ControlDisabled;
            SadConsole.Themes.Library.Default.SelectionButtonTheme.MouseOver = Appearance_ControlOver;
            SadConsole.Themes.Library.Default.SelectionButtonTheme.MouseClicking = Appearance_ControlMouseDown;
            SadConsole.Themes.Library.Default.SelectionButtonTheme.Focused = Appearance_ControlFocused;

        }
    }
}
