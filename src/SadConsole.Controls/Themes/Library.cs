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
            ButtonTheme.Normal = new Cell(ColorAnsi.WhiteBright, ColorAnsi.White);
            ButtonTheme.Focused = new Cell(ColorAnsi.Blue, ColorAnsi.White);
            ButtonTheme.MouseOver = new Cell(ColorAnsi.White, ColorAnsi.WhiteBright);
            ButtonTheme.MouseClicking = new Cell(ColorAnsi.WhiteBright, ColorAnsi.White);
            ButtonTheme.Disabled = new Cell(ColorAnsi.WhiteBright, ColorAnsi.White);

            SelectionButtonTheme = (ButtonTheme)ButtonTheme.Clone();

            ScrollBarTheme = new ScrollBarTheme();
            ScrollBarTheme.Bar = new ThemePartBase();
            ScrollBarTheme.Bar.Normal = new Cell(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Bar.Focused = new Cell(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Bar.Disabled = new Cell(ColorHelper.Gray, ColorHelper.Black);
            ScrollBarTheme.Bar.MouseOver = new Cell(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Ends = new ThemePartBase();
            ScrollBarTheme.Ends.Normal = new Cell(ColorHelper.White, ColorHelper.Black);
            ScrollBarTheme.Ends.Focused = new Cell(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Ends.Disabled = new Cell(ColorHelper.Gray, ColorHelper.Black);
            ScrollBarTheme.Ends.MouseOver = new Cell(ColorHelper.White, ColorHelper.Black);
            ScrollBarTheme.Slider = new ThemePartBase();
            ScrollBarTheme.Slider.Normal = new Cell(ColorHelper.White, ColorHelper.Black);
            ScrollBarTheme.Slider.Focused = new Cell(ColorHelper.LightGray, ColorHelper.Black);
            ScrollBarTheme.Slider.Disabled = new Cell(ColorHelper.Gray, ColorHelper.Black);
            ScrollBarTheme.Slider.MouseOver = new Cell(ColorHelper.White, ColorHelper.Black);

            WindowTheme = new WindowTheme();
            WindowTheme.TitleStyle = new Cell(ColorAnsi.Black, ColorAnsi.WhiteBright);
            WindowTheme.BorderStyle = new Cell(ColorAnsi.WhiteBright, ColorAnsi.Black);
            WindowTheme.FillStyle = new Cell(ColorAnsi.WhiteBright, ColorAnsi.Black);

            ControlsConsoleTheme = new ControlsConsoleTheme();
            ControlsConsoleTheme.FillStyle = new Cell(ColorAnsi.WhiteBright, ColorAnsi.Black);

            CheckBoxTheme = new CheckBoxTheme();
            CheckBoxTheme.CheckedIcon = 251;
            CheckBoxTheme.UncheckedIcon = 0;
            CheckBoxTheme.Button = new ThemePartSelected();
            CheckBoxTheme.Button.Normal = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            CheckBoxTheme.Button.Focused = new Cell(ColorAnsi.Blue, ColorHelper.Transparent);
            CheckBoxTheme.Button.MouseOver = new Cell(ColorAnsi.White, ColorAnsi.WhiteBright);
            CheckBoxTheme.Button.MouseClicking = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            CheckBoxTheme.Button.Disabled = new Cell(ColorAnsi.Black, ColorHelper.Transparent);
            CheckBoxTheme.Button.Selected = new Cell(ColorAnsi.YellowBright, ColorHelper.Transparent);
            CheckBoxTheme.Normal = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            CheckBoxTheme.Focused = new Cell(ColorAnsi.Blue, ColorHelper.Transparent);
            CheckBoxTheme.MouseOver = new Cell(ColorAnsi.White, ColorAnsi.WhiteBright);
            CheckBoxTheme.MouseClicking = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            CheckBoxTheme.Disabled = new Cell(ColorAnsi.Black, ColorHelper.Transparent);
            CheckBoxTheme.Selected = new Cell(ColorAnsi.YellowBright, ColorHelper.Transparent);

            RadioButtonTheme = new RadioButtonTheme();
            RadioButtonTheme.CheckedIcon = 7;
            RadioButtonTheme.UncheckedIcon = 0;
            RadioButtonTheme.Button = new ThemePartSelected();
            RadioButtonTheme.Button.Normal = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            RadioButtonTheme.Button.Focused = new Cell(ColorAnsi.Blue, ColorHelper.Transparent);
            RadioButtonTheme.Button.MouseOver = new Cell(ColorAnsi.White, ColorAnsi.WhiteBright);
            RadioButtonTheme.Button.MouseClicking = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            RadioButtonTheme.Button.Disabled = new Cell(ColorAnsi.Black, ColorHelper.Transparent);
            RadioButtonTheme.Button.Selected = new Cell(ColorAnsi.YellowBright, ColorHelper.Transparent);
            RadioButtonTheme.Normal = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            RadioButtonTheme.Focused = new Cell(ColorAnsi.Blue, ColorHelper.Transparent);
            RadioButtonTheme.MouseOver = new Cell(ColorAnsi.White, ColorAnsi.WhiteBright);
            RadioButtonTheme.MouseClicking = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            RadioButtonTheme.Disabled = new Cell(ColorAnsi.Black, ColorHelper.Transparent);
            RadioButtonTheme.Selected = new Cell(ColorAnsi.YellowBright, ColorHelper.Transparent);

            ListBoxTheme = new ListBoxTheme();
            ListBoxTheme.Border = new Cell(ColorHelper.LightGray, ColorHelper.Black);
            ListBoxTheme.Item = new ThemePartSelected();
            ListBoxTheme.Item.Normal = new Cell(ColorHelper.White, ColorHelper.Transparent);
            ListBoxTheme.Item.Focused = new Cell(ColorHelper.White, ColorHelper.Transparent);
            ListBoxTheme.Item.MouseClicking = new Cell(ColorHelper.White, ColorHelper.Transparent);
            ListBoxTheme.Item.Disabled = new Cell(ColorHelper.White, ColorHelper.Transparent);
            ListBoxTheme.Item.MouseOver = new Cell(ColorHelper.LightGray, ColorHelper.Gray);
            ListBoxTheme.Item.Selected = new Cell(ColorHelper.Yellow, ColorHelper.Gray);
            ListBoxTheme.ScrollBarTheme = (ScrollBarTheme)ScrollBarTheme.Clone();
            ListBoxTheme.Normal = new Cell(ColorAnsi.WhiteBright, ColorHelper.Transparent);
            ListBoxTheme.Focused = new Cell(ColorAnsi.Blue, ColorHelper.Transparent);
            ListBoxTheme.MouseOver = new Cell(ColorAnsi.White, ColorAnsi.WhiteBright);
            ListBoxTheme.Disabled = new Cell(ColorAnsi.Black, ColorHelper.Transparent);

            InputBoxTheme = new InputBoxTheme();
            InputBoxTheme.Normal = new Cell(ColorHelper.Blue, ColorHelper.DimGray);
            InputBoxTheme.Focused = new Cell(ColorHelper.DarkBlue, ColorHelper.DarkGray);
            InputBoxTheme.MouseOver = new Cell(ColorHelper.DarkBlue, ColorHelper.DarkGray);
            InputBoxTheme.Disabled = new Cell(ColorHelper.Black, ColorAnsi.White);
            InputBoxTheme.CarrotEffect = new Effects.BlinkGlyph()
            {
                GlyphIndex = 95,
                BlinkSpeed = 0.4f
            };
        }
    }
}
