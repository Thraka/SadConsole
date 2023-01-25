using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;

namespace SadConsole.UI.Themes;

/// <summary>
/// The library of themes. Holds the themes of all controls.
/// </summary>
[DataContract]
public class Library
{
    private static Library _libraryInstance;

    [DataMember(Name = "ControlThemes")]
    private Dictionary<System.Type, ThemeBase> _controlThemes;

    private Colors _colors;

    /// <summary>
    /// If a control does not specify its own theme, the theme from this property will be used.
    /// </summary>
    [MemberNotNull("_libraryInstance")]
    public static Library Default
    {
        get
        {
            if (_libraryInstance != null) return _libraryInstance;

            _libraryInstance = new Library();
            _libraryInstance.ApplyDefaults();
            _libraryInstance.Colors.AddToColorMappings();

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

            _libraryInstance.Colors.AddToColorMappings();
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
            _colors = value;

            if (this == _libraryInstance)
                _colors.AddToColorMappings();
        }
    }

    static Library()
    {
        if (Default == null)
        {
            Default = new Library();
            Default.ApplyDefaults();
        }
    }

    /// <summary>
    /// Seeds the library with the default themes.
    /// </summary>
    public void ApplyDefaults()
    {
        SetControlTheme(typeof(ScrollBar), new ScrollBarTheme());
        SetControlTheme(typeof(CheckBox), new CheckBoxTheme());
        SetControlTheme(typeof(ListBox), new ListBoxTheme(new ScrollBarTheme()));
        SetControlTheme(typeof(ProgressBar), new ProgressBarTheme());
        SetControlTheme(typeof(RadioButton), new RadioButtonTheme());
        SetControlTheme(typeof(TextBox), new TextBoxTheme());
        SetControlTheme(typeof(SelectionButton), new ButtonTheme());
        SetControlTheme(typeof(DrawingArea), new DrawingAreaTheme());
        SetControlTheme(typeof(Button), new ButtonTheme());
        SetControlTheme(typeof(Label), new LabelTheme());
        SetControlTheme(typeof(Panel), new PanelTheme());
        SetControlTheme(typeof(SurfaceViewer), new SurfaceViewerTheme());
        SetControlTheme(typeof(Table), new TableTheme(new ScrollBarTheme()));
    }

    /// <summary>
    /// Creates a new instance of the theme library with default themes.
    /// </summary>
    public Library()
    {
        _colors = Colors.CreateAnsi();
        _colors.IsLibrary = true;

        _controlThemes = new Dictionary<Type, ThemeBase>(15);
    }

    /// <summary>
    /// Creates and returns a theme based on the type of control provided.
    /// </summary>
    /// <param name="control">The control type.</param>
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
    /// Copies the colors from the specified <see cref="SadConsole.UI.Colors"/> object to <see cref="Colors"/> property.
    /// </summary>
    /// <param name="colors">The colors to copy.</param>
    public void SetColors(Colors colors) =>
        colors.CopyTo(Colors);

    /// <summary>
    /// Clones this library.
    /// </summary>
    /// <returns>A new instance of a library.</returns>
    public Library Clone()
    {
        var library = new Library();

        foreach (var item in _controlThemes)
            library.SetControlTheme(item.Key, item.Value);

        library.Colors = Colors.Clone();
        library.Colors.IsLibrary = true;

        return library;
    }
}
