using System.Collections.Generic;
using System.Linq;

namespace SadConsole;

public static class DecoratorHelpers
{
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

    public static void SetDecorator(CellDecorator decorator, ColoredGlyphBase glyph)
    {
        glyph.Decorators ??= new() { decorator };

        if (glyph.Decorators.Count == 0)
            glyph.Decorators = null;
    }

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

    public static void AddDecorator(CellDecorator decorator, ColoredGlyphBase glyph)
    {
        glyph.Decorators ??= new List<CellDecorator>();

        if (!glyph.Decorators.Contains(decorator))
            glyph.Decorators.Add(decorator);
    }

    public static void RemoveDecorators(IEnumerable<CellDecorator> decorators, ColoredGlyphBase glyph)
    {
        if (glyph.Decorators is null) return;

        foreach (CellDecorator item in decorators)
            glyph.Decorators.Remove(item);

        if (glyph.Decorators.Count == 0)
            glyph.Decorators = null;
    }

    public static void RemoveDecorator(CellDecorator decorator, ColoredGlyphBase glyph)
    {
        if (glyph.Decorators is null) return;

        glyph.Decorators.Remove(decorator);

        if (glyph.Decorators.Count == 0)
            glyph.Decorators = null;
    }

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
