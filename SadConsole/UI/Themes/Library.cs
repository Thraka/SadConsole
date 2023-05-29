using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SadConsole.Renderers;
using System.Xml.Linq;
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
    private Dictionary<Type, Type> _controlThemes;

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
        SetControlTheme<Button, ButtonTheme>();
        SetControlTheme<CheckBox, CheckBoxTheme>();
        SetControlTheme<ComboBox, ComboBoxTheme>();
        SetControlTheme<DrawingArea, DrawingAreaTheme>();
        SetControlTheme<ScrollBar, ScrollBarTheme>();
        SetControlTheme<Label, LabelTheme>();
        SetControlTheme<ListBox, ListBoxTheme>();
        SetControlTheme<NumberBox, TextBoxTheme>();
        SetControlTheme<Panel, PanelTheme>();
        SetControlTheme<ProgressBar, ProgressBarTheme>();
        SetControlTheme<RadioButton, RadioButtonTheme>();
        SetControlTheme<SelectionButton, ButtonTheme>();
        SetControlTheme<SurfaceViewer, SurfaceViewerTheme>();
        SetControlTheme<TextBox, TextBoxTheme>();
        SetControlTheme<Table, TableTheme>();
    }

    /// <summary>
    /// Creates a new instance of the theme library with default themes.
    /// </summary>
    public Library()
    {
        _colors = Colors.CreateAnsi();
        _colors.IsLibrary = true;

        _controlThemes = new Dictionary<Type, Type>(13);
    }

    /// <summary>
    /// Creates and returns a theme based on the type of control provided.
    /// </summary>
    /// <param name="control">The control type.</param>
    /// <returns>A theme that is associated with the control.</returns>
    public ThemeBase GetControlTheme(Type control)
    {
        if (_controlThemes.TryGetValue(control, out Type? objType))
            return Activator.CreateInstance(objType) as ThemeBase
                ?? throw new NullReferenceException($"Theme was found registered, but the system was unable to create an instance of it as an {nameof(ThemeBase)}.");

        throw new KeyNotFoundException($"Theme not found. If this is a new control, register the theme with the {nameof(Library)}.{nameof(SetControlTheme)} method.");
    }

    /// <summary>
    /// Creates and returns a theme based on the type of control provided.
    /// </summary>
    /// <typeparam name="TControl">The control type to get a theme of.</typeparam>
    /// <returns>A theme.</returns>
    public ThemeBase GetControlTheme<TControl>() =>
        GetControlTheme(typeof(TControl));

    /// <summary>
    /// Creates and returns a theme based on the type of control provided.
    /// </summary>
    /// <typeparam name="TControl">The control type to get a theme of.</typeparam>
    /// <typeparam name="TTheme">The expected theme type.</typeparam>
    /// <returns>A theme.</returns>
    public TTheme GetControlTheme<TControl, TTheme>() where TTheme : ThemeBase =>
        (TTheme)GetControlTheme(typeof(TControl));

    /// <summary>
    /// Sets a control theme based on the control type.
    /// </summary>
    /// <param name="control">The control type to register a theme.</param>
    /// <param name="theme">The theme to associate with the control.</param>
    /// <returns>A theme that is associated with the control.</returns>
    public void SetControlTheme(Type control, Type theme)
    {
        if (null == control) throw new ArgumentNullException(nameof(control), "Cannot use a null control type");
        _controlThemes[control] = theme ?? throw new ArgumentNullException(nameof(theme), "Cannot set the theme of a control to null");
    }

    /// <summary>
    /// Sets a control theme based on the control type.
    /// </summary>
    /// <typeparam name="TControl">The control type to register a theme.</typeparam>
    /// <typeparam name="TTheme">The theme to associate with the control</typeparam>
    public void SetControlTheme<TControl, TTheme>() =>
        SetControlTheme(typeof(TControl), typeof(TTheme));

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
