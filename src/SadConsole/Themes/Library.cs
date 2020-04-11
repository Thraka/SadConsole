namespace SadConsole.Themes
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using SadConsole.Controls;

    /// <summary>
    /// The library of themes. Holds the themes of all controls.
    /// </summary>
    [DataContract]
    public class Library
    {
        private static Library _libraryInstance;

        private Colors _colors;
        private Dictionary<System.Type, ThemeBase> _controlThemes;

        /// <summary>
        /// If a control does not specify its own theme, the theme from this property will be used.
        /// </summary>
        public static Library Default
        {
            get
            {
                if (_libraryInstance != null) return _libraryInstance;

                _libraryInstance = new Library();
                _libraryInstance.ApplyDefaults();

                return _libraryInstance;
            }
            set
            {
                if (null == value)
                {
                    _libraryInstance = new Library();
                    _libraryInstance.ApplyDefaults();
                }
                else
                    _libraryInstance = value;
            }
        }

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
            }
        }

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
                Default.ApplyDefaults();
            }
        }

        /// <summary>
        /// Seeds the library with the default themes.
        /// </summary>
        public void ApplyDefaults()
        {
            ControlsConsoleTheme = new ControlsConsoleTheme();
            WindowTheme = new WindowTheme();

            SetControlTheme(typeof(ScrollBar), new ScrollBarTheme());
            SetControlTheme(typeof(SelectionButton), new ButtonTheme());
            SetControlTheme(typeof(Button), new ButtonTheme());
            SetControlTheme(typeof(CheckBox), new CheckBoxTheme());
            SetControlTheme(typeof(ListBox), new ListBoxTheme(new ScrollBarTheme()));
            SetControlTheme(typeof(ProgressBar), new ProgressBarTheme());
            SetControlTheme(typeof(RadioButton), new RadioButtonTheme());
            SetControlTheme(typeof(TextBox), new TextBoxTheme());
            SetControlTheme(typeof(DrawingSurface), new DrawingSurfaceTheme());
            SetControlTheme(typeof(Label), new LabelTheme());
        }

        /// <summary>
        /// Creates a new instance of the theme library with default themes.
        /// </summary>
        public Library()
        {
            _colors = new Colors();
            _controlThemes = new Dictionary<Type, ThemeBase>(15);
        }

        /// <summary>
        /// Create the instance for the singleton.
        /// </summary>
        /// <param name="_">Not used.</param>
        private Library(bool _) => _colors = new Colors();

        /// <summary>
        /// Creates and returns a theme based on the type of control provided.
        /// </summary>
        /// <param name="control">The control instance</param>
        /// <returns>A theme that is associated with the control.</returns>
        public ThemeBase GetControlTheme(Type control)
        {
            if (_controlThemes.ContainsKey(control))
                return _controlThemes[control].Clone();

            throw new System.Exception("Control does not have an associated theme.");
        }

        /// <summary>
        /// Sets a control theme based on the control type.
        /// </summary>
        /// <param name="control">The control type to register a theme.</param>
        /// <param name="theme">The theme to associate with the control.</param>
        /// <returns>A theme that is associated with the control.</returns>
        public void SetControlTheme(Type control, ThemeBase theme)
        {
            if (null == control) throw new ArgumentNullException(nameof(control), "Cannot use a null control type");
            _controlThemes[control] = theme ?? throw new ArgumentNullException(nameof(theme), "Cannot set the theme of a control to null");
        }

        /// <summary>
        /// Clones this library.
        /// </summary>
        /// <returns>A new instance of a library.</returns>
        public Library Clone()
        {
            var library = new Library();

            foreach (var item in _controlThemes)
                library.SetControlTheme(item.Key, item.Value);

            library._colors = _colors.Clone();

            return library;
        }
    }
}
