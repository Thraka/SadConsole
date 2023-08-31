using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Represents an individual piece of a <see cref="ICellSurface"/> containing a glyph, foreground color, background color, and a mirror effect.
/// </summary>
public class ColoredGlyph : ColoredGlyphBase, IMatchable<ColoredGlyph>
{
    /// <summary>
    /// Creates a cell with a white foreground, black background, glyph 0, and no mirror effect.
    /// </summary>
    public ColoredGlyph() : this(Color.White, Color.Black, 0, Mirror.None) { }

    /// <summary>
    /// Creates a cell with the specified foreground, black background, glyph 0, and no mirror effect.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    public ColoredGlyph(Color foreground) : this(foreground, Color.Black, 0, Mirror.None) { }

    /// <summary>
    /// Creates a cell with the specified foreground, specified background, glyph 0, and no mirror effect.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    public ColoredGlyph(Color foreground, Color background) : this(foreground, background, 0, Mirror.None) { }

    /// <summary>
    /// Creates a cell with the specified foreground, background, and glyph, with no mirror effect.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    public ColoredGlyph(Color foreground, Color background, int glyph) : this(foreground, background, glyph, Mirror.None) { }

    /// <summary>
    /// Creates a cell with the specified foreground, background, glyph, and mirror effect.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    /// <param name="mirror">The mirror effect.</param>
    public ColoredGlyph(Color foreground, Color background, int glyph, Mirror mirror)
    {
        Foreground = foreground;
        Background = background;
        Glyph = glyph;
        Mirror = mirror;
    }

    /// <summary>
    /// Creates a cell with the specified foreground, background, glyph, mirror, and visibility.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    /// <param name="mirror">The mirror effect.</param>
    /// <param name="isVisible">The visiblity of the glyph.</param>
    public ColoredGlyph(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible)
    {
        Foreground = foreground;
        Background = background;
        Glyph = glyph;
        Mirror = mirror;
        IsVisible = isVisible;
    }

    /// <summary>
    /// Creates a cell with the specified foreground, background, glyph, mirror effect, visibility and decorators.
    /// </summary>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    /// <param name="mirror">The mirror effect.</param>
    /// <param name="isVisible">The visiblity of the glyph.</param>
    /// <param name="decorators">Decorators for the cell.</param>
    public ColoredGlyph(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible, CellDecorator[] decorators)
    {
        Foreground = foreground;
        Background = background;
        Glyph = glyph;
        Mirror = mirror;
        IsVisible = isVisible;
        Decorators = decorators;
    }
    /// <inheritdoc/>
    public override void Clear()
    {
        Foreground = Color.White;
        Background = Color.Black;
        Glyph = 0;
        Mirror = Mirror.None;
        Decorators = Array.Empty<CellDecorator>();
    }

    /// <inheritdoc/>
    public override ColoredGlyphBase Clone() =>
        new ColoredGlyph(Foreground, Background, Glyph, Mirror)
        {
            IsVisible = IsVisible,
            Decorators = Decorators.Length != 0 ? Decorators.ToArray() : Array.Empty<CellDecorator>()
        };

    /// <summary>
    /// Checks if this <see cref="ColoredGlyph"/> object's properties match another's.
    /// </summary>
    /// <param name="other">The other object to check.</param>
    /// <returns>Returns <see langword="true"/> when the object's properties match; otherwise <see langword="false"/>.</returns>
    public bool Matches(ColoredGlyph? other)
    {
        return other != null &&
               Decorators.ItemsMatch(other.Decorators) &&
               Foreground.Equals(other.Foreground) &&
               Background.Equals(other.Background) &&
               Glyph == other.Glyph &&
               Mirror == other.Mirror &&
               IsVisible == other.IsVisible;
    }
}

/// <summary>
/// Represents an individual piece of a <see cref="ICellSurface"/> containing a glyph, foreground color, background color, and a mirror effect.
/// </summary>
public abstract class ColoredGlyphBase: IMatchable<ColoredGlyphBase>
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
    private CellDecorator[] _decorators = Array.Empty<CellDecorator>();

    /// <summary>
    /// Modifies the look of a cell with additional character.
    /// </summary>
    public CellDecorator[] Decorators
    {
        get => _decorators;
        set { _decorators = value ?? Array.Empty<CellDecorator>(); IsDirty = true; }
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
            cell.Decorators = Decorators.Length != 0 ? Decorators.ToArray() : Array.Empty<CellDecorator>();
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
            Decorators = cell.Decorators.Length != 0 ? cell.Decorators.ToArray() : Array.Empty<CellDecorator>();
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
        if (Decorators.Length == 0)
            Decorators = Array.Empty<CellDecorator>();
    }
}
