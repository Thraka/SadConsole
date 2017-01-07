using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using System.Runtime.Serialization;

namespace SadConsole.Themes
{

    /// <summary>
    /// The library of themes. Holds the themes of all controls.
    /// </summary>
    [DataContract]
    public class Library
    {
        /// <summary>
        /// If a control does not specify its own theme, the theme from this property will be used.
        /// </summary>
        public static Library Default { get; set; }

        /// <summary>
        /// Theme for the <see cref="SadConsole.Controls.Button"/> control.
        /// </summary>
        [DataMember]
        public ButtonTheme ButtonTheme;

        /// <summary>
        /// Theme for the <see cref="SadConsole.Controls.SelectionButton"/> control.
        /// </summary>
        [DataMember]
        public ButtonTheme SelectionButtonTheme;

        /// <summary>
        /// Theme for the <see cref="SadConsole.Consoles.Window"/> control.
        /// </summary>
        [DataMember]
        public WindowTheme WindowTheme;

        /// <summary>
        /// Theme for the <see cref="SadConsole.Controls.ScrollBar"/> control.
        /// </summary>
        [DataMember]
        public ScrollBarTheme ScrollBarTheme;

        /// <summary>
        /// Theme for the <see cref="SadConsole.Controls.RadioButton"/> control.
        /// </summary>
        [DataMember]
        public RadioButtonTheme RadioButtonTheme;

        /// <summary>
        /// Theme for the <see cref="SadConsole.Controls.ListBox"/> control.
        /// </summary>
        [DataMember]
        public ListBoxTheme ListBoxTheme;

        /// <summary>
        /// Theme for the <see cref="SadConsole.Controls.CheckBox"/> control.
        /// </summary>
        [DataMember]
        public CheckBoxTheme CheckBoxTheme;

        /// <summary>
        /// Theme for the <see cref="SadConsole.Controls.InputBox"/> control.
        /// </summary>
        [DataMember]
        public InputBoxTheme InputBoxTheme;

        /// <summary>
        /// Theme for <see cref="Consoles.ControlsConsole"/>.
        /// </summary>
        [DataMember]
        public ControlsConsoleTheme ControlsConsoleTheme;

        static Library()
        {
            if (Default == null)
                Default = new Library();
        }

        /// <summary>
        /// Creates a new instance of the theme library with default themes.
        /// </summary>
        public Library()
        {
            ButtonTheme = new ButtonTheme();
            ButtonTheme.Normal = new CellAppearance(ColorAnsi.WhiteBright, ColorAnsi.White);
            ButtonTheme.Focused = new CellAppearance(ColorAnsi.Blue, ColorAnsi.White);
            ButtonTheme.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            ButtonTheme.MouseClicking = new CellAppearance(ColorAnsi.WhiteBright, ColorAnsi.White);
            ButtonTheme.Disabled = new CellAppearance(ColorAnsi.WhiteBright, ColorAnsi.White);

            SelectionButtonTheme = (ButtonTheme)ButtonTheme.Clone();

            ScrollBarTheme = new ScrollBarTheme();
            ScrollBarTheme.Bar = new ThemePartBase();
            ScrollBarTheme.Bar.Normal = new CellAppearance(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Bar.Focused = new CellAppearance(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Bar.Disabled = new CellAppearance(ColorHelper.Gray, ColorHelper.Black);
            ScrollBarTheme.Bar.MouseOver = new CellAppearance(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Ends = new ThemePartBase();
            ScrollBarTheme.Ends.Normal = new CellAppearance(ColorHelper.White, ColorHelper.Black);
            ScrollBarTheme.Ends.Focused = new CellAppearance(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Ends.Disabled = new CellAppearance(ColorHelper.Gray, ColorHelper.Black);
            ScrollBarTheme.Ends.MouseOver = new CellAppearance(ColorHelper.White, ColorHelper.Black);
            ScrollBarTheme.Slider = new ThemePartBase();
            ScrollBarTheme.Slider.Normal = new CellAppearance(ColorHelper.White, ColorHelper.Black);
            ScrollBarTheme.Slider.Focused = new CellAppearance(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Slider.Disabled = new CellAppearance(ColorHelper.Gray, ColorHelper.Black);
            ScrollBarTheme.Slider.MouseOver = new CellAppearance(ColorHelper.White, ColorHelper.Black);

            WindowTheme = new WindowTheme();
            WindowTheme.TitleStyle = new CellAppearance(ColorAnsi.Black, ColorAnsi.WhiteBright);
            WindowTheme.BorderStyle = new CellAppearance(ColorAnsi.WhiteBright, ColorAnsi.Black);
            WindowTheme.FillStyle = new CellAppearance(ColorAnsi.WhiteBright, ColorAnsi.Black);

            ControlsConsoleTheme = new ControlsConsoleTheme();
            ControlsConsoleTheme.FillStyle = new CellAppearance(ColorAnsi.WhiteBright, ColorAnsi.Black);

            CheckBoxTheme = new CheckBoxTheme();
            CheckBoxTheme.CheckedIcon = 251;
            CheckBoxTheme.UncheckedIcon = 0;
            CheckBoxTheme.Button = new ThemePartSelected();
            CheckBoxTheme.Button.Normal = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            CheckBoxTheme.Button.Focused = new CellAppearance(ColorAnsi.Blue, ColorHelper.Transparent);
            CheckBoxTheme.Button.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            CheckBoxTheme.Button.MouseClicking = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            CheckBoxTheme.Button.Disabled = new CellAppearance(ColorAnsi.Black, ColorHelper.Transparent);
            CheckBoxTheme.Button.Selected = new CellAppearance(ColorAnsi.YellowBright, ColorHelper.Transparent);
            CheckBoxTheme.Normal = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            CheckBoxTheme.Focused = new CellAppearance(ColorAnsi.Blue, ColorHelper.Transparent);
            CheckBoxTheme.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            CheckBoxTheme.MouseClicking = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            CheckBoxTheme.Disabled = new CellAppearance(ColorAnsi.Black, ColorHelper.Transparent);
            CheckBoxTheme.Selected = new CellAppearance(ColorAnsi.YellowBright, ColorHelper.Transparent);

            RadioButtonTheme = new RadioButtonTheme();
            RadioButtonTheme.CheckedIcon = 7;
            RadioButtonTheme.UncheckedIcon = 0;
            RadioButtonTheme.Button = new ThemePartSelected();
            RadioButtonTheme.Button.Normal = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            RadioButtonTheme.Button.Focused = new CellAppearance(ColorAnsi.Blue, ColorHelper.Transparent);
            RadioButtonTheme.Button.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            RadioButtonTheme.Button.MouseClicking = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            RadioButtonTheme.Button.Disabled = new CellAppearance(ColorAnsi.Black, ColorHelper.Transparent);
            RadioButtonTheme.Button.Selected = new CellAppearance(ColorAnsi.YellowBright, ColorHelper.Transparent);
            RadioButtonTheme.Normal = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            RadioButtonTheme.Focused = new CellAppearance(ColorAnsi.Blue, ColorHelper.Transparent);
            RadioButtonTheme.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            RadioButtonTheme.MouseClicking = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            RadioButtonTheme.Disabled = new CellAppearance(ColorAnsi.Black, ColorHelper.Transparent);
            RadioButtonTheme.Selected = new CellAppearance(ColorAnsi.YellowBright, ColorHelper.Transparent);

            ListBoxTheme = new ListBoxTheme();
            ListBoxTheme.Border = new CellAppearance(ColorHelper.LightGray, ColorHelper.Black);
            ListBoxTheme.Item = new ThemePartSelected();
            ListBoxTheme.Item.Normal = new CellAppearance(ColorHelper.White, ColorHelper.Transparent);
            ListBoxTheme.Item.Focused = new CellAppearance(ColorHelper.White, ColorHelper.Transparent);
            ListBoxTheme.Item.MouseClicking = new CellAppearance(ColorHelper.White, ColorHelper.Transparent);
            ListBoxTheme.Item.Disabled = new CellAppearance(ColorHelper.White, ColorHelper.Transparent);
            ListBoxTheme.Item.MouseOver = new CellAppearance(ColorHelper.LightGray, ColorHelper.Gray);
            ListBoxTheme.Item.Selected = new CellAppearance(ColorHelper.Yellow, ColorHelper.Gray);
            ListBoxTheme.ScrollBarTheme = (ScrollBarTheme)ScrollBarTheme.Clone();
            ListBoxTheme.Normal = new CellAppearance(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            ListBoxTheme.Focused = new CellAppearance(ColorAnsi.Blue, ColorHelper.Transparent);
            ListBoxTheme.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            ListBoxTheme.Disabled = new CellAppearance(ColorAnsi.Black, ColorHelper.Transparent);

            InputBoxTheme = new InputBoxTheme();
            InputBoxTheme.Normal = new CellAppearance(ColorHelper.Blue, ColorHelper.DimGray);
            InputBoxTheme.Focused = new CellAppearance(ColorHelper.DarkBlue, ColorHelper.DarkGray);
            InputBoxTheme.MouseOver = new CellAppearance(ColorHelper.DarkBlue, ColorHelper.DarkGray);
            InputBoxTheme.Disabled = new CellAppearance(ColorHelper.Black, ColorAnsi.White);
            InputBoxTheme.CarrotEffect = new Effects.BlinkGlyph()
            {
                GlyphIndex = 95,
                BlinkSpeed = 0.4f
            };
        }
    }
}
