using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using System.Runtime.Serialization;
using SadConsole.Controls;
using SadConsole.Surfaces;

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
        /// Theme for the <see cref="TextBox"/> control.
        /// </summary>
        [DataMember]
        public TextBoxTheme TextBoxTheme;

        /// <summary>
        /// Theme for the <see cref="SadConsole.Controls.ProgressBar"/> control.
        /// </summary>
        [DataMember]
        public ProgressBarTheme ProgressBarTheme;

        /// <summary>
        /// Theme for <see cref="Consoles.ControlsConsole"/>.
        /// </summary>
        [DataMember]
        public ControlsConsoleTheme ControlsConsoleTheme;


        /// <summary>
        /// Theme for the <see cref="SadConsole.Consoles.Window"/> control.
        /// </summary>
        [DataMember]
        public WindowTheme WindowTheme;

        public Cell Appearance_ControlNormal;
        public Cell Appearance_ControlDisabled;
        public Cell Appearance_ControlOver;
        public Cell Appearance_ControlSelected;
        public Cell Appearance_ControlMouseDown;
        public Cell Appearance_ControlFocused;


        static Library()
        {
            if (Default == null)
            {
                Default = new Library();
                Default.Init();
            }
        }

        private void Init()
        {
            ControlsConsoleTheme = new ControlsConsoleTheme();
            WindowTheme = new WindowTheme();

            ScrollBarTheme = new ScrollBarTheme();
            ButtonTheme = new ButtonTheme();
            CheckBoxTheme = new CheckBoxTheme();
            ListBoxTheme = new ListBoxTheme();
            ProgressBarTheme = new ProgressBarTheme();
            RadioButtonTheme = new RadioButtonTheme();
            TextBoxTheme = new TextBoxTheme();
            SelectionButtonTheme = new ButtonTheme();
        }


        /// <summary>
        /// Creates a new instance of the theme library with default themes.
        /// </summary>
        public Library()
        {
            Appearance_ControlNormal = new Cell(Colors.Text, Colors.ControlBack);
            Appearance_ControlDisabled = new Cell(Colors.TextLight, Colors.ControlBackDark);
            Appearance_ControlOver = new Cell(Colors.TextSelectedDark, Colors.ControlBackSelected);
            Appearance_ControlSelected = new Cell(Colors.TextSelected, Colors.ControlBackSelected);
            Appearance_ControlMouseDown = new Cell(Appearance_ControlSelected.Background, Appearance_ControlSelected.Foreground);
            Appearance_ControlFocused = new Cell(Colors.Cyan, Colors.ControlBackLight);
        }
    }
}
