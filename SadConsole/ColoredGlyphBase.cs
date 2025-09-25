using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Represents an individual piece of a <see cref="ICellSurface"/> containing a glyph, foreground color, background color, and a mirror effect.
/// </summary>
public abstract partial class ColoredGlyphBase: IMatchable<ColoredGlyphBase>
{
    /// <summary>
    /// An event that is raised when the <see cref="IsDirty"/> property is set to <see langword="true"/>.
    /// </summary>
    public event EventHandler? IsDirtySet;

    private Color _foreground;
    private Color _background;
    private Mirror _mirror;
    private int _glyph;
    private bool _isDirty;
    private bool _isVisible = true;
    private List<CellDecorator>? _decorators;

    /// <summary>
    /// Modifies the look of a cell with additional character.
    /// </summary>
    public List<CellDecorator>? Decorators
    {
        get => _decorators;
        set { _decorators = value; IsDirty = true; }
    }

    /// <summary>
    /// The foreground color of this cell.
    /// </summary>
    public Color Foreground
    {
        get => _foreground;
        set { _foreground = value; IsDirty = true; }
    }

    /// <summary>
    /// The background color of this cell.
    /// </summary>
    public Color Background
    {
        get => _background;
        set { _background = value; IsDirty = true; }
    }

    /// <summary>
    /// The glyph index from a font for this cell.
    /// </summary>
    public int Glyph
    {
        get => _glyph;
        set { _glyph = value; IsDirty = true; }
    }

    /// <summary>
    /// The mirror effect for this cell.
    /// </summary>
    public Mirror Mirror
    {
        get => _mirror;
        set { _mirror = value; IsDirty = true; }
    }

    /// <summary>
    /// The glyph.
    /// </summary>
    [IgnoreDataMember]
    public char GlyphCharacter
    {
        get => (char)Glyph;
        set => Glyph = value;
    }

    /// <summary>
    /// <see langword="true"/> when this cell should be drawn; otherwise, <see langword="false"/>.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set { _isVisible = value; IsDirty = true; }
    }

    /// <summary>
    /// <see langword="true"/> when this cell needs to be redrawn; otherwise, <see langword="false"/>.
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            _isDirty = value;

            if (value)
                IsDirtySet?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Copies the visual appearance to the specified cell. This includes foreground, background, glyph, mirror effect and decorators.
    /// </summary>
    /// <param name="cell">The target cell to copy to.</param>
    /// <param name="deepCopy">
    /// Whether to perform a deep copy.  Decorators are copied to a new array when true; when false, the old
    /// decorator array reference is moved directly.
    /// </param>
    public virtual void CopyAppearanceTo(ColoredGlyphBase cell, bool deepCopy = true)
    {
        cell.Foreground = Foreground;
        cell.Background = Background;
        cell.Glyph = Glyph;
        cell.Mirror = Mirror;
        if (deepCopy)
            cell.Decorators = Decorators != null ? new List<CellDecorator>(Decorators) : null;
        else
            cell.Decorators = Decorators;
    }

    /// <summary>
    /// Sets the foreground, background, glyph, mirror effect and decorators to the same as the specified cell.
    /// </summary>
    /// <param name="cell">The target cell to copy from.</param>
    /// <param name="deepCopy">
    /// Whether to perform a deep copy.  Decorators are copied to a new array when true; when false, the old
    /// decorator array reference is moved directly.
    /// </param>
    public virtual void CopyAppearanceFrom(ColoredGlyphBase cell, bool deepCopy = true)
    {
        Foreground = cell.Foreground;
        Background = cell.Background;
        Glyph = cell.Glyph;
        Mirror = cell.Mirror;
        if (deepCopy)
            Decorators = cell.Decorators != null ? new List<CellDecorator>(cell.Decorators) : null;
        else
            Decorators = cell.Decorators;
    }

    /// <summary>
    /// Resets the foreground, background, glyph, mirror effect and decorators.
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Returns a new cell with the same properties as this one.
    /// </summary>
    /// <returns>The new cell.</returns>
    public abstract ColoredGlyphBase Clone();

    /// <summary>
    /// Checks if this <see cref="ColoredGlyph"/> object's properties match another's.
    /// </summary>
    /// <param name="other">The other object to check.</param>
    /// <returns>Returns <see langword="true"/> when the object's properties match; otherwise <see langword="false"/>.</returns>
    public bool Matches(ColoredGlyphBase? other)
    {
        return other != null &&
               Decorators.ItemsMatch(other.Decorators) &&
               Foreground.Equals(other.Foreground) &&
               Background.Equals(other.Background) &&
               Glyph == other.Glyph &&
               Mirror == other.Mirror &&
               IsVisible == other.IsVisible;
    }

    /// <summary>
    /// Creates an array of colored glyphs.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static ColoredGlyphBase[] CreateArray(int size)
    {
        ColoredGlyph[] cells = new ColoredGlyph[size];

        for (int i = 0; i < size; i++)
            cells[i] = new ColoredGlyph();

        return cells;
    }

    /// <inheritdoc/>
    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (Decorators!= null && Decorators.Count == 0)
            Decorators = null;
    }
}
