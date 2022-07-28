using System;
using System.Linq;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// A <see cref="ColoredGlyph"/> with state information.
/// </summary>
public readonly struct ColoredGlyphState
{
    /// <summary>
    /// A copy of the <see cref="ColoredGlyph.Decorators"/> property.
    /// </summary>
    public CellDecorator[] Decorators { get; }

    /// <summary>
    /// A copy of the <see cref="ColoredGlyph.Foreground"/> property.
    /// </summary>
    public Color Foreground { get; }

    /// <summary>
    /// A copy of the <see cref="ColoredGlyph.Background"/> property.
    /// </summary>
    public Color Background { get; }

    /// <summary>
    /// A copy of the <see cref="ColoredGlyph.Glyph"/> property.
    /// </summary>
    public int Glyph { get; }

    /// <summary>
    /// A copy of the <see cref="ColoredGlyph.Mirror"/> property.
    /// </summary>
    public Mirror Mirror { get; }

    /// <summary>
    /// A copy of the <see cref="ColoredGlyph.IsVisible"/> property.
    /// </summary>
    public bool IsVisible { get; }

    /// <summary>
    /// Creates a new state from a cell.
    /// </summary>
    /// <param name="cell">The colored glyph this state is a copy of.</param>
    public ColoredGlyphState(ColoredGlyph cell)
    {
        Foreground = cell.Foreground;
        Background = cell.Background;
        Mirror = cell.Mirror;
        Glyph = cell.Glyph;
        IsVisible = cell.IsVisible;
        Decorators = cell.Decorators.Length != 0 ? cell.Decorators.ToArray() : Array.Empty<CellDecorator>();
    }

    /// <summary>
    /// Creates a new state.
    /// </summary>
    /// <param name="decorators">Decorators for the cell.</param>
    /// <param name="foreground">Foreground color.</param>
    /// <param name="background">Background color.</param>
    /// <param name="glyph">The glyph index.</param>
    /// <param name="mirror">The mirror effect.</param>
    /// <param name="isVisible">The visiblity of the glyph.</param>
    [JsonConstructor]
    public ColoredGlyphState(CellDecorator[] decorators, Color foreground, Color background, int glyph, Mirror mirror, bool isVisible)
    {
        Decorators = decorators;
        Foreground = foreground;
        Background = background;
        Glyph = glyph;
        Mirror = mirror;
        IsVisible = isVisible;
    }

    /// <summary>
    /// Restores this state to the specified cell.
    /// </summary>
    public void RestoreState(ref ColoredGlyph cell)
    {
        cell.Foreground = Foreground;
        cell.Background = Background;
        cell.Mirror = Mirror;
        cell.Glyph = Glyph;
        cell.IsVisible = IsVisible;

        if (Decorators == null)
            cell.Decorators = Array.Empty<CellDecorator>();
        else
            cell.Decorators = Decorators.Length != 0 ? Decorators.ToArray() : Array.Empty<CellDecorator>();
    }
}
