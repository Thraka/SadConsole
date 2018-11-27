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
        /// Colors for the theme library.
        /// </summary>
        [DataMember]
        public Colors Colors { get; set; }

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
        /// Theme for <see cref="DrawingSurface"/>.
        /// </summary>
        [DataMember]
        public DrawingSurfaceTheme DrawingSurfaceTheme;

        /// <summary>
        /// Theme for <see cref="ControlsConsole"/>.
        /// </summary>
        [DataMember]
        public ControlsConsoleTheme ControlsConsoleTheme;

        /// <summary>
        /// Theme for the <see cref="Window"/> control.
        /// </summary>
        [DataMember]
        public WindowTheme WindowTheme;

        
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
            ControlsConsoleTheme = new ControlsConsoleTheme(Colors);
            WindowTheme = new WindowTheme(Colors);

            ScrollBarTheme = new ScrollBarTheme();
            ButtonTheme = new ButtonTheme();
            CheckBoxTheme = new CheckBoxTheme();
            ListBoxTheme = new ListBoxTheme(new ScrollBarTheme(), new ListBoxItemTheme(Colors));
            ProgressBarTheme = new ProgressBarTheme();
            RadioButtonTheme = new RadioButtonTheme();
            TextBoxTheme = new TextBoxTheme();
            SelectionButtonTheme = new ButtonTheme();
            DrawingSurfaceTheme = new DrawingSurfaceTheme();
        }


        /// <summary>
        /// Creates a new instance of the theme library with default themes.
        /// </summary>
        public Library()
        {
            Colors = new Colors();
        }

        /// <summary>
        /// Gets a new control theme based on the control passed.
        /// </summary>
        /// <param name="control">The control instance</param>
        /// <returns>A theme that is associated with the control.</returns>
        public virtual ThemeBase GetControlTheme(ControlBase control)
        {
            switch (control)
            {
                case SelectionButton c:
                    return SelectionButtonTheme.Clone();
                    
                case ScrollBar c:
                    return ScrollBarTheme.Clone();

                case RadioButton c:
                    return RadioButtonTheme.Clone();

                case ListBox c:
                    return ListBoxTheme.Clone();

                case CheckBox c:
                    return CheckBoxTheme.Clone();

                case TextBox c:
                    return TextBoxTheme.Clone();

                case ProgressBar c:
                    return ProgressBarTheme.Clone();

                case DrawingSurface c:
                    return DrawingSurfaceTheme.Clone();

                case Button c:
                    return ButtonTheme.Clone();

                default:
                    throw new System.Exception("Control does not have associated theme.");
            }
        }

        /// <summary>
        /// Clonse this library.
        /// </summary>
        /// <returns>A new instance of a library.</returns>
        public virtual Library Clone()
        {
            return new Library()
            {
                Colors = Colors.Clone(),

                ButtonTheme = (ButtonTheme) ButtonTheme.Clone(),
                SelectionButtonTheme = (ButtonTheme) SelectionButtonTheme.Clone(),
                ScrollBarTheme = (ScrollBarTheme) ScrollBarTheme.Clone(),
                RadioButtonTheme = (RadioButtonTheme) RadioButtonTheme.Clone(),
                ListBoxTheme = (ListBoxTheme) ListBoxTheme.Clone(),
                CheckBoxTheme = (CheckBoxTheme) CheckBoxTheme.Clone(),
                TextBoxTheme = (TextBoxTheme) TextBoxTheme.Clone(),
                ProgressBarTheme = (ProgressBarTheme) ProgressBarTheme.Clone(),
                DrawingSurfaceTheme = (DrawingSurfaceTheme) DrawingSurfaceTheme.Clone(),

                ControlsConsoleTheme = ControlsConsoleTheme.Clone(),
                WindowTheme = WindowTheme.Clone(),
            };
        }
    }
}

