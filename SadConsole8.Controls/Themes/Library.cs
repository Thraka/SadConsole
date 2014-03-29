
namespace SadConsole.Themes
{
    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

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
        /// Theme for the <see cref="SadConsole.Controls.InputBox"/> control.
        /// </summary>
        [DataMember]
        public InputBoxTheme InputBoxTheme;

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
            ScrollBarTheme.Bar.Normal = new CellAppearance(Color.LightGray, Color.Black);
            ScrollBarTheme.Bar.Focused = new CellAppearance(Color.LightGray, Color.Black);
            ScrollBarTheme.Bar.Disabled = new CellAppearance(Color.Gray, Color.Black);
            ScrollBarTheme.Bar.MouseOver = new CellAppearance(Color.LightGray, Color.Black);
            ScrollBarTheme.Ends = new ThemePartBase();
            ScrollBarTheme.Ends.Normal = new CellAppearance(Color.White, Color.Black);
            ScrollBarTheme.Ends.Focused = new CellAppearance(Color.LightGray, Color.Black);
            ScrollBarTheme.Ends.Disabled = new CellAppearance(Color.Gray, Color.Black);
            ScrollBarTheme.Ends.MouseOver = new CellAppearance(Color.White, Color.Black);
            ScrollBarTheme.Slider = new ThemePartBase();
            ScrollBarTheme.Slider.Normal = new CellAppearance(Color.White, Color.Black);
            ScrollBarTheme.Slider.Focused = new CellAppearance(Color.LightGray, Color.Black);
            ScrollBarTheme.Slider.Disabled = new CellAppearance(Color.Gray, Color.Black);
            ScrollBarTheme.Slider.MouseOver = new CellAppearance(Color.White, Color.Black);

            WindowTheme = new WindowTheme();
            WindowTheme.TitleStyle = new CellAppearance(ColorAnsi.Black, ColorAnsi.WhiteBright);
            WindowTheme.BorderStyle = new CellAppearance(ColorAnsi.WhiteBright, ColorAnsi.Black);
            WindowTheme.FillStyle = new CellAppearance(ColorAnsi.WhiteBright, ColorAnsi.Black);

            RadioButtonTheme = new RadioButtonTheme();
            RadioButtonTheme.CheckedIcon = 7;
            RadioButtonTheme.UncheckedIcon = 0;
            RadioButtonTheme.Button = new ThemePartSelected();
            RadioButtonTheme.Button.Normal = new CellAppearance(ColorAnsi.WhiteBright, Color.Transparent);
            RadioButtonTheme.Button.Focused = new CellAppearance(ColorAnsi.Blue, Color.Transparent);
            RadioButtonTheme.Button.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            RadioButtonTheme.Button.MouseClicking = new CellAppearance(ColorAnsi.WhiteBright, Color.Transparent);
            RadioButtonTheme.Button.Disabled = new CellAppearance(ColorAnsi.Black, Color.Transparent);
            RadioButtonTheme.Button.Selected = new CellAppearance(ColorAnsi.YellowBright, Color.Transparent);
            RadioButtonTheme.Normal = new CellAppearance(ColorAnsi.WhiteBright, Color.Transparent);
            RadioButtonTheme.Focused = new CellAppearance(ColorAnsi.Blue, Color.Transparent);
            RadioButtonTheme.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            RadioButtonTheme.MouseClicking = new CellAppearance(ColorAnsi.WhiteBright, Color.Transparent);
            RadioButtonTheme.Disabled = new CellAppearance(ColorAnsi.Black, Color.Transparent);
            RadioButtonTheme.Selected = new CellAppearance(ColorAnsi.YellowBright, Color.Transparent);

            ListBoxTheme = new ListBoxTheme();
            ListBoxTheme.Border = new CellAppearance(Color.LightGray, Color.Black);
            ListBoxTheme.Item = new ThemePartSelected();
            ListBoxTheme.Item.Normal = new CellAppearance(Color.White, Color.Transparent);
            ListBoxTheme.Item.Focused = new CellAppearance(Color.White, Color.Transparent);
            ListBoxTheme.Item.MouseClicking = new CellAppearance(Color.White, Color.Transparent);
            ListBoxTheme.Item.Disabled = new CellAppearance(Color.White, Color.Transparent);
            ListBoxTheme.Item.MouseOver = new CellAppearance(Color.LightGray, Color.Gray);
            ListBoxTheme.Item.Selected = new CellAppearance(Color.Yellow, Color.Gray);
            ListBoxTheme.ScrollBarTheme = (ScrollBarTheme)ScrollBarTheme.Clone();
            ListBoxTheme.Normal = new CellAppearance(ColorAnsi.WhiteBright, Color.Transparent);
            ListBoxTheme.Focused = new CellAppearance(ColorAnsi.Blue, Color.Transparent);
            ListBoxTheme.MouseOver = new CellAppearance(ColorAnsi.White, ColorAnsi.WhiteBright);
            ListBoxTheme.Disabled = new CellAppearance(ColorAnsi.Black, Color.Transparent);

            InputBoxTheme = new InputBoxTheme();
            InputBoxTheme.Normal = new CellAppearance(Color.Blue, Color.DimGray);
            InputBoxTheme.Focused = new CellAppearance(Color.DarkBlue, Color.DarkGray);
            InputBoxTheme.MouseOver = new CellAppearance(Color.DarkBlue, Color.DarkGray);
            InputBoxTheme.Disabled = new CellAppearance(Color.Black, ColorAnsi.White);
            InputBoxTheme.CarrotEffect = new Effects.BlinkCharacter()
            {
                CharacterIndex = 95,
                BlinkSpeed = 0.4f
            };
        }
    }
}
