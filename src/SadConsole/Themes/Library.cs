namespace SadConsole.Themes
{
    using System.Runtime.Serialization;
    using SadConsole.Controls;

    /// <summary>
    /// The library of themes. Holds the themes of all controls.
    /// </summary>
    [DataContract]
    public class Library
    {
        private Colors _colors;

        /// <summary>
        /// If a control does not specify its own theme, the theme from this property will be used.
        /// </summary>
        public static Library Default { get; set; }

        /// <summary>
        /// Colors for the theme library.
        /// </summary>
        [DataMember]
        public Colors Colors
        {
            get => _colors;
            set
            {
                if (_colors == null) throw new System.NullReferenceException("Colors cannot be set to null");
                _colors = value;

                OnColorsChanged();
            }
        }

        /// <summary>
        /// Theme for the <see cref="Button"/> control.
        /// </summary>
        [DataMember]
        public ButtonTheme ButtonTheme { get; set; }

        /// <summary>
        /// Theme for the <see cref="SelectionButton"/> control.
        /// </summary>
        [DataMember]
        public ButtonTheme SelectionButtonTheme { get; set; }

        /// <summary>
        /// Theme for the <see cref="ScrollBar"/> control.
        /// </summary>
        [DataMember]
        public ScrollBarTheme ScrollBarTheme { get; set; }

        /// <summary>
        /// Theme for the <see cref="RadioButton"/> control.
        /// </summary>
        [DataMember]
        public RadioButtonTheme RadioButtonTheme { get; set; }

        /// <summary>
        /// Theme for the <see cref="ListBox"/> control.
        /// </summary>
        [DataMember]
        public ListBoxTheme ListBoxTheme { get; set; }

        /// <summary>
        /// Theme for the <see cref="CheckBox"/> control.
        /// </summary>
        [DataMember]
        public CheckBoxTheme CheckBoxTheme { get; set; }

        /// <summary>
        /// Theme for the <see cref="TextBox"/> control.
        /// </summary>
        [DataMember]
        public TextBoxTheme TextBoxTheme { get; set; }

        /// <summary>
        /// Theme for the <see cref="ProgressBar"/> control.
        /// </summary>
        [DataMember]
        public ProgressBarTheme ProgressBarTheme { get; set; }

        /// <summary>
        /// Theme for <see cref="DrawingSurface"/>.
        /// </summary>
        [DataMember]
        public DrawingSurfaceTheme DrawingSurfaceTheme { get; set; }

        /// <summary>
        /// Theme for <see cref="Label"/>.
        /// </summary>
        [DataMember]
        public LabelTheme LabelTheme { get; set; }

        /// <summary>
        /// Theme for <see cref="ControlsConsole"/>.
        /// </summary>
        [DataMember]
        public ControlsConsoleTheme ControlsConsoleTheme { get; set; }

        /// <summary>
        /// Theme for the <see cref="Window"/> control.
        /// </summary>
        [DataMember]
        public WindowTheme WindowTheme { get; set; }

        static Library()
        {
            if (Default == null)
            {
                Default = new Library(false);
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
            ListBoxTheme = new ListBoxTheme(new ScrollBarTheme());
            ProgressBarTheme = new ProgressBarTheme();
            RadioButtonTheme = new RadioButtonTheme();
            TextBoxTheme = new TextBoxTheme();
            SelectionButtonTheme = new ButtonTheme();
            DrawingSurfaceTheme = new DrawingSurfaceTheme();
            LabelTheme = new LabelTheme();
        }

        /// <summary>
        /// Creates a new instance of the theme library with default themes.
        /// </summary>
        public Library()
        {
            _colors = new Colors();
            Init();
        }

        /// <summary>
        /// Create the instance for the singleton.
        /// </summary>
        /// <param name="_">Not used.</param>
        private Library(bool _)
        {
            _colors = new Colors();
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

                case Label c:
                    return LabelTheme.Clone();

                default:
                    throw new System.Exception("Control does not have an associated theme.");
            }
        }

        /// <summary>
        /// Refreshes the theme colors of every control. Called when the <see cref="Colors"/> property has changed.
        /// </summary>
        protected virtual void OnColorsChanged()
        {
            ControlsConsoleTheme.RefreshTheme(Colors);
            WindowTheme.RefreshTheme(Colors);

            ScrollBarTheme.RefreshTheme(Colors);
            ButtonTheme.RefreshTheme(Colors);
            CheckBoxTheme.RefreshTheme(Colors);
            ListBoxTheme.RefreshTheme(Colors);
            ProgressBarTheme.RefreshTheme(Colors);
            RadioButtonTheme.RefreshTheme(Colors);
            TextBoxTheme.RefreshTheme(Colors);
            SelectionButtonTheme.RefreshTheme(Colors);
            DrawingSurfaceTheme.RefreshTheme(Colors);
            LabelTheme.RefreshTheme(Colors);
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

                ButtonTheme = (ButtonTheme)ButtonTheme.Clone(),
                SelectionButtonTheme = (ButtonTheme)SelectionButtonTheme.Clone(),
                ScrollBarTheme = (ScrollBarTheme)ScrollBarTheme.Clone(),
                RadioButtonTheme = (RadioButtonTheme)RadioButtonTheme.Clone(),
                ListBoxTheme = (ListBoxTheme)ListBoxTheme.Clone(),
                CheckBoxTheme = (CheckBoxTheme)CheckBoxTheme.Clone(),
                TextBoxTheme = (TextBoxTheme)TextBoxTheme.Clone(),
                ProgressBarTheme = (ProgressBarTheme)ProgressBarTheme.Clone(),
                DrawingSurfaceTheme = (DrawingSurfaceTheme)DrawingSurfaceTheme.Clone(),
                LabelTheme = (LabelTheme)LabelTheme.Clone(),

                ControlsConsoleTheme = ControlsConsoleTheme.Clone(),
                WindowTheme = WindowTheme.Clone(),
            };
        }
    }
}
