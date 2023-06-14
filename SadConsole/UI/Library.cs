using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SadConsole.Renderers;
using System.Xml.Linq;
using SadConsole.UI.Controls;

namespace SadConsole.UI;

/// <summary>
/// The library of themes. Holds the themes of all controls.
/// </summary>
[DataContract]
public class Library
{
    private static Library _libraryInstance;
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
            _libraryInstance.Colors.AddToColorMappings();

            return _libraryInstance;
        }
        set
        {
            if (null == value)
                _libraryInstance = new Library();
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

    static Library() =>
        Default ??= new Library();

    /// <summary>
    /// Creates a new instance of the theme library with default themes.
    /// </summary>
    public Library()
    {
        _colors = Colors.CreateAnsi();
        _colors.IsLibrary = true;
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

        library.Colors = Colors.Clone();
        library.Colors.IsLibrary = true;

        return library;
    }
}
