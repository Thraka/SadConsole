using System.Collections.Generic;
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
    public ColoredGlyph(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible, List<CellDecorator> decorators)
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
        Decorators = null;
    }

    /// <inheritdoc/>
    public override ColoredGlyphBase Clone()
    {
        ColoredGlyph glyph = new(Foreground, Background, Glyph, Mirror)
        {
            IsVisible = IsVisible,
        };
        DecoratorHelpers.SetDecorators(Decorators, glyph);
        return glyph;
    }


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
