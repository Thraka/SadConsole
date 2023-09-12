using System.Collections.Generic;
using System.Linq;

namespace SadConsole;

/// <summary>
/// Helpers for <see cref="CellDecorator"/> and <see cref="ColoredGlyphBase.Decorators"/> which manages null on the property.
/// </summary>
public static class DecoratorHelpers
{
    /// <summary>
    /// Replaces the decorators of a glyph.
    /// </summary>
    /// <param name="decorators">The decorators to set. <see langword="null"/> clears the decorators.</param>
    /// <param name="glyph">The glyph to alter.</param>
    public static void SetDecorators(IEnumerable<CellDecorator>? decorators, ColoredGlyphBase glyph)
    {
        if (decorators is null)
        {
            glyph.Decorators = null;
            return;
        }

        glyph.Decorators = new(decorators);

        if (glyph.Decorators.Count == 0)
            glyph.Decorators = null;
    }

    /// <summary>
    /// Replaces the decorators of a glyph with a single decorator.
    /// </summary>
    /// <param name="decorator">The decorator to set.</param>
    /// <param name="glyph">The glyph to alter.</param>
    public static void SetDecorator(CellDecorator decorator, ColoredGlyphBase glyph)
    {
        glyph.Decorators ??= new() { decorator };

        if (glyph.Decorators.Count == 0)
            glyph.Decorators = null;
    }

    /// <summary>
    /// Adds the decorators to a glyph.
    /// </summary>
    /// <param name="decorators">The decorators to add. Duplicates are skipped.</param>
    /// <param name="glyph">The glyph to alter.</param>
    public static void AddDecorators(IEnumerable<CellDecorator>? decorators, ColoredGlyphBase glyph)
    {
        if (decorators is null) return;

        glyph.Decorators ??= new List<CellDecorator>();

        glyph.Decorators.AddRange(decorators);

        if (glyph.Decorators.Count == 0)
            glyph.Decorators = null;
        else
            glyph.Decorators = glyph.Decorators.Distinct().ToList();
    }

    /// <summary>
    /// Adds the specified decorator to a glyph.
    /// </summary>
    /// <param name="decorator">The decorator to add.</param>
    /// <param name="glyph">The glyph to alter.</param>
    public static void AddDecorator(CellDecorator decorator, ColoredGlyphBase glyph)
    {
        glyph.Decorators ??= new List<CellDecorator>();

        if (!glyph.Decorators.Contains(decorator))
            glyph.Decorators.Add(decorator);
    }

    /// <summary>
    /// Removes the specified decorators from a glyph. If no decorators remain on the glyph, the <see cref="ColoredGlyphBase.Decorators"/> collection is set to <see langword="null"/>.
    /// </summary>
    /// <param name="decorators">The decorators to remove.</param>
    /// <param name="glyph">The glyph to alter.</param>
    public static void RemoveDecorators(IEnumerable<CellDecorator> decorators, ColoredGlyphBase glyph)
    {
        if (glyph.Decorators is null) return;

        foreach (CellDecorator item in decorators)
            glyph.Decorators.Remove(item);

        if (glyph.Decorators.Count == 0)
            glyph.Decorators = null;
    }

    /// <summary>
    /// Removes the specified decorator from a glyph. If no decorators remain on the glyph, the <see cref="ColoredGlyphBase.Decorators"/> collection is set to <see langword="null"/>.
    /// </summary>
    /// <param name="decorator">The decorator to remove.</param>
    /// <param name="glyph">The glyph to alter.</param>
    public static void RemoveDecorator(CellDecorator decorator, ColoredGlyphBase glyph)
    {
        if (glyph.Decorators is null) return;

        glyph.Decorators.Remove(decorator);

        if (glyph.Decorators.Count == 0)
            glyph.Decorators = null;
    }

    /// <summary>
    /// Returns a new list of decorators from the <see cref="ColoredGlyphBase.Decorators"/> property of <paramref name="glyph"/>.
    /// </summary>
    /// <param name="glyph">The glyph to copy the decorators from.</param>
    /// <returns>A list with all of the decorators from <paramref name="glyph"/>. If the glyph's decorators are <see langword="null"/>, <see langword="null"/> is returned.</returns>
    public static List<CellDecorator>? CloneDecorators(ColoredGlyphBase glyph)
    {
        if (glyph.Decorators == null) return null;

        return new List<CellDecorator>(glyph.Decorators);
    }
    /// <summary>
    /// Determines whether the contents of two <see cref="CellDecorator"/> arrays are equal.
    /// </summary>
    /// <param name="self">The first object to compare.</param>
    /// <param name="other">The second object to compare.</param>
    /// <returns>A boolean value to indicate whether or not the two arrays are considered equal.</returns>
    public static bool ItemsMatch(this List<CellDecorator>? self, List<CellDecorator>? other)
    {
        if (self == null && other == null) return true;
        if (self != null && other != null)
        {

            if (self.Count != other.Count) return false;

            for (int i = 0; i < self.Count; i++)

                if (self[i] != other[i]) return false;

            return true;
        }

        return false;
    }

}
