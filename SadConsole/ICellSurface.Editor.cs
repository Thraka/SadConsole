using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using SadConsole.Effects;
using SadConsole.Readers;
using SadRogue.Primitives;

namespace SadConsole;

// Since shift algorithms make temporary copies of appearance, we use a struct to make sure the copy
// is as efficient as possible
internal readonly struct ColoredGlyphAppearance
{
    public readonly Color Foreground;
    public readonly Color Background;
    public readonly int Glyph;
    public readonly Mirror Mirror;
    public readonly List<CellDecorator>? Decorators;

    public ColoredGlyphAppearance(ColoredGlyphBase cell)
    {
        Foreground = cell.Foreground;
        Background = cell.Background;
        Glyph = cell.Glyph;
        Mirror = cell.Mirror;

        // Shallow copy is OK here because, in the algorithm we use to shift, the item we're copying
        // is immediately replaced by another; this shallow-copy therefore avoids some allocation.
        Decorators = cell.Decorators;
    }

    /// <summary>
    /// Copies the ColoredGlyphAppearance to the appearance fields of the given ColoredGlyphs.
    /// The Decorators array is copied using a shallow-copy.
    /// </summary>
    /// <remarks>
    /// This is more performant than a deep copy if you don't need to preserve the state of the ColoredGlyphBase object.
    /// </remarks>
    /// <param name="cell">Cell to copy to.</param>
    public void ShallowCopyTo(ColoredGlyphBase cell)
    {
        cell.Foreground = Foreground;
        cell.Background = Background;
        cell.Glyph = Glyph;
        cell.Mirror = Mirror;
        cell.Decorators = Decorators;
    }
}

/// <summary>
/// Methods to interact with a <see cref="ICellSurface"/>.
/// </summary>
public static class CellSurfaceEditor
{
    /// <summary>
    /// Sets each background of a cell to the array of colors. <paramref name="pixels"/> must be the same length as the amount of cells in the surface.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="pixels">The colors to place.</param>
    public static void SetPixels(this ISurface obj, Color[] pixels)
    {
        if (pixels.Length != obj.Surface.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(pixels), "The amount of colors do not match the size of this cell obj.Surface.");
        }

        for (int i = 0; i < pixels.Length; i++)
        {
            obj.Surface[i].Background = pixels[i];
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Sets each background of a cell to the array of colors.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="area">An area to fill with pixels.</param>
    /// <param name="pixels">Colors for each cell of the obj.Surface.</param>
    public static void SetPixels(this ISurface obj, Rectangle area, Color[] pixels)
    {
        if (pixels.Length != area.Width * area.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(pixels), "The amount of colors do not match the size of the area.");
        }

        for (int x = area.X; x < area.X + area.Width; x++)
        {
            for (int y = area.Y; y < area.Y + area.Height; y++)
            {
                int index = y * obj.Surface.Width + x;

                if (obj.Surface.IsValidCell(index))
                {
                    obj.Surface[y * obj.Surface.Width + x].Background = pixels[(y - area.Y) * area.Width + (x - area.X)];
                }
            }
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Tests if a cell is valid based on its x,y position.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x coordinate of the cell to test.</param>
    /// <param name="y">The y coordinate of the cell to test.</param>
    /// <returns>A true value indicating the cell by x,y does exist in this cell obj.Surface.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidCell(this ISurface obj, int x, int y) =>
        IsValidCell(obj, new Point(x, y));

    /// <summary>
    /// Tests if a cell is valid based on its x,y position.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="coordinate">The coordinate of the cell to test.</param>
    /// <returns>A true value indicating the cell by x,y does exist in this cell obj.Surface.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidCell(this ISurface obj, Point coordinate) =>
        obj.Surface.Area.Contains(coordinate);

    /// <summary>
    /// Tests if a cell is valid based on its x,y position.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x coordinate of the cell to test.</param>
    /// <param name="y">The y coordinate of the cell to test.</param>
    /// <param name="index">If the cell is valid, the index of the cell when found.</param>
    /// <returns>A true value indicating the cell by x,y does exist in this cell obj.Surface.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidCell(this ISurface obj, int x, int y, out int index)
    {
        if (obj.Surface.IsValidCell(x, y))
        {
            index = y * obj.Surface.Width + x;
            return true;
        }

        index = -1;
        return false;
    }

    /// <summary>
    /// Tests if a cell is valid based on its x,y position.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="coordinate">The coordinate of the cell to test.</param>
    /// <param name="index">If the cell is valid, the index of the cell when found.</param>
    /// <returns>A true value indicating the cell by x,y does exist in this cell obj.Surface.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidCell(this ISurface obj, Point coordinate, out int index) =>
        IsValidCell(obj, coordinate.X, coordinate.Y, out index);

    /// <summary>
    /// Tests if a cell is valid based on its index.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index to test.</param>
    /// <returns>A true value indicating the cell index is in this cell obj.Surface.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidCell(this ISurface obj, int index) =>
        index >= 0 && index < obj.Surface.Count;

    /// <summary>
    /// Changes the glyph and mirror of the specified cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="definition">The glyph and mirror of the cell.</param>
    public static void SetGlyph(this ISurface obj, int x, int y, GlyphDefinition definition)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
            return;

        ColoredGlyphBase cell = obj.Surface[index];

        cell.Glyph = definition.Glyph;
        cell.Mirror = definition.Mirror;

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the glyph of a specified cell to a new value.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="glyph">The desired glyph of the cell.</param>
    public static void SetGlyph(this ISurface obj, int x, int y, int glyph)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
        {
            return;
        }

        obj.Surface[index].Glyph = glyph;
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the glyph and foreground of a cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="glyph">The desired glyph.</param>
    /// <param name="foreground">The desired foreground.</param>
    public static void SetGlyph(this ISurface obj, int x, int y, int glyph, Color foreground)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
        {
            return;
        }

        obj.Surface[index].Foreground = foreground;
        obj.Surface[index].Glyph = glyph;
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the glyph, foreground, and background of a cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="glyph">The desired glyph.</param>
    /// <param name="foreground">The desired foreground.</param>
    /// <param name="background">The desired background.</param>
    public static void SetGlyph(this ISurface obj, int x, int y, int glyph, Color foreground, Color background)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
        {
            return;
        }

        obj.Surface[index].Background = background;
        obj.Surface[index].Foreground = foreground;
        obj.Surface[index].Glyph = glyph;
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the glyph, foreground, background, and mirror of a cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="glyph">The desired glyph.</param>
    /// <param name="foreground">The desired foreground.</param>
    /// <param name="background">The desired background.</param>
    /// <param name="mirror">Sets how the glyph will be mirrored.</param>
    public static void SetGlyph(this ISurface obj, int x, int y, int glyph, Color foreground, Color background, Mirror mirror)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
        {
            return;
        }

        obj.Surface[index].Background = background;
        obj.Surface[index].Foreground = foreground;
        obj.Surface[index].Glyph = glyph;
        obj.Surface[index].Mirror = mirror;

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the glyph, foreground, background, and mirror of a cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="glyph">The desired glyph.</param>
    /// <param name="foreground">The desired foreground.</param>
    /// <param name="background">The desired background.</param>
    /// <param name="mirror">Sets how the glyph will be mirrored.</param>
    /// <param name="decorators">Decorators to set on the cell. Will clear existing decorators first.</param>
    public static void SetGlyph(this ISurface obj, int x, int y, int glyph, Color foreground, Color background, Mirror mirror, IEnumerable<CellDecorator> decorators)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        obj.Surface[index].Background = background;
        obj.Surface[index].Foreground = foreground;
        obj.Surface[index].Glyph = glyph;
        obj.Surface[index].Mirror = mirror;
        CellDecoratorHelpers.SetDecorators(decorators, obj.Surface[index]);

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Gets the glyph of a specified cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <returns>The glyph index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetGlyph(this ISurface obj, int x, int y) =>
        obj.Surface[y * obj.Surface.Width + x].Glyph;

    /// <summary>
    /// Changes the foreground of a specified cell to a new color.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="color">The desired color of the cell.</param>
    public static void SetForeground(this ISurface obj, int x, int y, Color color)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
        {
            return;
        }

        obj.Surface[index].Foreground = color;
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Gets the foreground of a specified cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <returns>The color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetForeground(this ISurface obj, int x, int y) =>
        obj.Surface[y * obj.Surface.Width + x].Foreground;

    /// <summary>
    /// Changes the background of a cell to the specified color.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="color">The desired color of the cell.</param>
    public static void SetBackground(this ISurface obj, int x, int y, Color color)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
        {
            return;
        }

        obj.Surface[index].Background = color;
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Gets the background of a specified cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <returns>The color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetBackground(this ISurface obj, int x, int y) =>
        obj.Surface[y * obj.Surface.Width + x].Background;

    /// <summary>
    /// Changes the effect of a cell to the specified effect.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="effect">The desired effect.</param>
    public static void SetEffect(this ISurface obj, int x, int y, ICellEffect? effect)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
            return;

        obj.Surface.Effects.SetEffect(obj.Surface[index], effect);
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the effect of a cell to the specified effect.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">Index of the cell.</param>
    /// <param name="effect">The desired effect.</param>
    public static void SetEffect(this ISurface obj, int index, ICellEffect? effect)
    {
        if (!obj.Surface.IsValidCell(index))
            return;

        obj.Surface.Effects.SetEffect(obj.Surface[index], effect);
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the effect of a list of cells to the specified effect.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="cells">The cells for the effect.</param>
    /// <param name="effect">The desired effect.</param>
    public static void SetEffect(this ISurface obj, IEnumerable<Point> cells, ICellEffect? effect)
    {
        var cellList = new List<int>(5);

        foreach (Point item in cells)
            cellList.Add(item.ToIndex(obj.Surface.Width));

        obj.Surface.Effects.SetEffect((IEnumerable<ColoredGlyphBase>)cellList, effect);
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the effect of a list of cells to the specified effect.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="cells">The cells for the effect.</param>
    /// <param name="effect">The desired effect.</param>
    public static void SetEffect(this ISurface obj, IEnumerable<int> cells, ICellEffect? effect)
    {
        List<ColoredGlyphBase> glyphs = new(5);

        foreach (var index in cells)
            glyphs.Add(obj.Surface[index]);

        obj.Surface.Effects.SetEffect(glyphs, effect);
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the effect of a cell to the specified effect.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="cell">The cells for the effect.</param>
    /// <param name="effect">The desired effect.</param>
    public static void SetEffect(this ISurface obj, ColoredGlyphBase cell, ICellEffect? effect)
    {
        obj.Surface.Effects.SetEffect(cell, effect);
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the effect of a cell to the specified effect.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="cells">The cells for the effect.</param>
    /// <param name="effect">The desired effect.</param>
    public static void SetEffect(this ISurface obj, IEnumerable<ColoredGlyphBase> cells, ICellEffect? effect)
    {
        obj.Surface.Effects.SetEffect(cells, effect);
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Gets the effect of the specified cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <returns>The effect.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ICellEffect? GetEffect(this ISurface obj, int x, int y) =>
        obj.Surface.Effects.GetEffect(obj.Surface[x, y]);

    /// <summary>
    /// Gets the effect of the specified cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index of the cell.</param>
    /// <returns>The effect.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ICellEffect? GetEffect(this ISurface obj, int index) =>
        obj.Surface.Effects.GetEffect(obj.Surface[index]);

    /// <summary>
    /// Changes the appearance of the cell to that of the provided colored glyph object.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="appearance">The desired appearance of the cell. A null value cannot be passed.</param>
    public static void SetCellAppearance(this ISurface obj, int x, int y, ColoredGlyphBase appearance)
    {
        if (appearance == null)
            throw new NullReferenceException("Appearance may not be null.");

        if (!obj.Surface.IsValidCell(x, y, out int index))
            return;

        appearance.CopyAppearanceTo(obj.Surface[index]);
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Changes the appearance of the cell to that of the provided colored glyph object.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="definition">The glyph and mirror of the cell.</param>
    public static void SetCellAppearance(this ISurface obj, int x, int y, GlyphDefinition definition) =>
        SetGlyph(obj, x, y, definition);

    /// <summary>
    /// Gets the appearance of a cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <returns>The appearance.</returns>
    public static ColoredGlyphBase GetCellAppearance(this ISurface obj, int x, int y) =>
        obj.Surface[y * obj.Surface.Width + x].Clone();

    /// <summary>
    /// Gets an enumerable of cells over a specific area.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="area">The area to get cells from.</param>
    /// <returns>A new array with references to each cell in the area.</returns>
    public static IEnumerable<ColoredGlyphBase> GetCells(this ISurface obj, Rectangle area)
    {
        area = Rectangle.GetIntersection(area, new Rectangle(0, 0, obj.Surface.Width, obj.Surface.Height));

        if (area == Rectangle.Empty)
            yield break;

        if (area == obj.Surface.Area)
        {
            for (int i = 0; i < obj.Surface.Width * obj.Surface.Height; i++)
                yield return obj.Surface[i];
        }
        else
        {
            for (int y = 0; y < area.Height; y++)
                for (int x = 0; x < area.Width; x++)
                    yield return obj.Surface[(y + area.Y) * obj.Surface.Width + (x + area.X)];
        }
    }

    /// <summary>
    /// Returns a new surface with reference to each cell inside of the <paramref name="view"/>.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="view">An area of the surface to create a view of.</param>
    /// <returns>A new surface</returns>
    public static ICellSurface GetSubSurface(this ISurface obj, Rectangle view)
    {
        if (!new Rectangle(0, 0, obj.Surface.Width, obj.Surface.Height).Contains(view))
            throw new Exception("View is outside of surface bounds.");

        return new CellSurface(view.Width, view.Height, obj.Surface.GetCells(view).ToArray());
    }

    /// <summary>
    /// Returns a new surface using the cells from the current surface.
    /// </summary>
    /// <param name="obj">The surface.</param>
    /// <returns>A new surface instance.</returns>
    public static ICellSurface GetSubSurface(this ISurface obj) =>
        new CellSurface(obj.Surface.Width, obj.Surface.Height, obj.Surface.GetCells(new Rectangle(0, 0, obj.Surface.Width, obj.Surface.Height)).ToArray());

    /// <summary>
    /// Gets the mirror of a specified cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <returns>The <see cref="Mirror"/> of the cell.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Mirror GetMirror(this ISurface obj, int x, int y) =>
        obj.Surface[y * obj.Surface.Width + x].Mirror;

    /// <summary>
    /// Sets the mirror of a specified cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    /// <param name="mirror">The mirror of the cell.</param>
    public static void SetMirror(this ISurface obj, int x, int y, Mirror mirror)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        obj.Surface[index].Mirror = mirror;
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Sets the decorator of one or more cells.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x coordinate of the cell.</param>
    /// <param name="y">The y coordinate of the cell.</param>
    /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
    /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetDecorator(this ISurface obj, int x, int y, int count, params CellDecorator[]? decorators) =>
        SetDecorator(obj, Point.ToIndex(x, y, obj.Surface.Width), count, decorators);

    /// <summary>
    /// Sets the decorators of a cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="position">The coordinate of the cell.</param>
    /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetDecorator(this ISurface obj, Point position, params CellDecorator[]? decorators) =>
        SetDecorator(obj, position.ToIndex(obj.Surface.Width), decorators);

    /// <summary>
    /// Sets the decorator of one or more cells.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index of the cell to start applying.</param>
    /// <param name="count">The count of cells to use from the index (inclusive).</param>
    /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
    public static void SetDecorator(this ISurface obj, int index, int count, params CellDecorator[]? decorators)
    {
        if (count <= 0) return;
        if (!obj.Surface.IsValidCell(index)) return;

        if (index + count > obj.Surface.Count)
            count = obj.Surface.Count - index;

        for (int i = index; i < index + count; i++)
            CellDecoratorHelpers.SetDecorators(decorators, obj.Surface[i]);

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Sets the decorators of a single cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index of the cell to start applying.</param>
    /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
    public static void SetDecorator(this ISurface obj, int index, params CellDecorator[]? decorators)
    {
        if (!obj.Surface.IsValidCell(index)) return;

        CellDecoratorHelpers.SetDecorators(decorators, obj.Surface[index]);

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Appends the decorators to one or more cells
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x coordinate of the cell.</param>
    /// <param name="y">The y coordinate of the cell.</param>
    /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
    /// <param name="decorators">The decorators.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddDecorator(this ISurface obj, int x, int y, int count, params CellDecorator[] decorators) =>
        AddDecorator(obj, Point.ToIndex(x, y, obj.Surface.Width), count, decorators);

    /// <summary>
    /// Appends the decorators to one or more cells
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="position">The x,y coordinate of the cell.</param>
    /// <param name="decorators">The decorators.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddDecorator(this ISurface obj, Point position, params CellDecorator[] decorators) =>
        AddDecorator(obj, position.ToIndex(obj.Surface.Width), decorators);

    /// <summary>
    /// Appends the decorators to one or more cells
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index of the cell to start applying.</param>
    /// <param name="count">The count of cells to use from the index (inclusive).</param>
    /// <param name="decorators">The decorators.</param>
    public static void AddDecorator(this ISurface obj, int index, int count, params CellDecorator[] decorators)
    {
        if (count <= 0) return;
        if (!obj.Surface.IsValidCell(index)) return;
        if (decorators == null || decorators.Length == 0) return;

        if (index + count > obj.Surface.Count)
            count = obj.Surface.Count - index;

        for (int i = index; i < index + count; i++)
            CellDecoratorHelpers.AddDecorators(decorators, obj.Surface[i]);

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Appends the decorators to one or more cells
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index of the cell to start applying.</param>
    /// <param name="decorators">The decorators.</param>
    public static void AddDecorator(this ISurface obj, int index, params CellDecorator[]? decorators)
    {
        if (!obj.Surface.IsValidCell(index)) return;
        if (decorators == null || decorators.Length == 0) return;

        CellDecoratorHelpers.AddDecorators(decorators, obj.Surface[index]);

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Removes the decorators from one or more cells.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x coordinate of the cell.</param>
    /// <param name="y">The y coordinate of the cell.</param>
    /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
    /// <param name="decorators">The decorators.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveDecorator(this ISurface obj, int x, int y, int count, params CellDecorator[] decorators) =>
        RemoveDecorator(obj, Point.ToIndex(x, y, obj.Surface.Width), count, decorators);

    /// <summary>
    /// Removes the decorators from a cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="position">The x,y coordinate of the cell.</param>
    /// <param name="decorators">The decorators.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveDecorator(this ISurface obj, Point position, params CellDecorator[] decorators) =>
        RemoveDecorator(obj, position.ToIndex(obj.Surface.Width), decorators);

    /// <summary>
    /// Removes the decorators from one or more cells.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index of the cell to start applying.</param>
    /// <param name="count">The count of cells to use from the index (inclusive).</param>
    /// <param name="decorators">The decorators.</param>
    public static void RemoveDecorator(this ISurface obj, int index, int count, params CellDecorator[] decorators)
    {
        if (count <= 0) return;
        if (!obj.Surface.IsValidCell(index)) return;
        if (decorators == null || decorators.Length == 0) return;

        if (index + count > obj.Surface.Count)
            count = obj.Surface.Count - index;

        for (int i = index; i < index + count; i++)
            CellDecoratorHelpers.RemoveDecorators(decorators, obj.Surface[i]);

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Removes the decorators from a cell.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index of the cell to start applying.</param>
    /// <param name="decorators">The decorators.</param>
    public static void RemoveDecorator(this ISurface obj, int index, params CellDecorator[] decorators)
    {
        if (!obj.Surface.IsValidCell(index)) return;
        if (decorators == null || decorators.Length == 0) return;

        CellDecoratorHelpers.RemoveDecorators(decorators, obj.Surface[index]);

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Clears the decorators of the specified cells.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x coordinate of the cell.</param>
    /// <param name="y">The y coordinate of the cell.</param>
    /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ClearDecorators(this ISurface obj, int x, int y, int count) =>
        SetDecorator(obj, x, y, count, null);

    /// <summary>
    /// Clears the decorators of the specified cells
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">The index of the cell to start applying.</param>
    /// <param name="count">The count of cells to use from the index (inclusive).</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ClearDecorators(this ISurface obj, int index, int count) =>
        SetDecorator(obj, index, count, null);

    /// <summary>
    /// Draws the string on the console at the specified location, wrapping if needed.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="text">The string to display.</param>
    public static void Print(this ISurface obj, int x, int y, string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        if (!obj.Surface.UsePrintProcessor)
        {
            var effectIndices = new List<int>(text.Length);
            int end = index + text.Length > obj.Surface.Count ? obj.Surface.Count : index + text.Length;
            int charIndex = 0;

            for (; index < end; index++)
            {
                obj.Surface[index].Glyph = text[charIndex];

                effectIndices.Add(index);
                charIndex++;
            }

            SetEffect(obj, effectIndices, null);
        }
        else
            PrintNoCheck(obj, index, ColoredString.Parser.Parse(text, index, obj.Surface));

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Draws the string on the console at the specified location and color, wrapping if needed.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="text">The string to display.</param>
    /// <param name="foreground">Sets the foreground of all characters in the text.</param>
    public static void Print(this ISurface obj, int x, int y, string text, Color foreground)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        if (!obj.Surface.UsePrintProcessor)
        {
            var effectIndices = new List<int>(text.Length);
            int end = index + text.Length > obj.Surface.Count ? obj.Surface.Count : index + text.Length;
            int charIndex = 0;

            for (; index < end; index++)
            {
                obj.Surface[index].Glyph = text[charIndex];
                obj.Surface[index].Foreground = foreground;

                effectIndices.Add(index);
                charIndex++;
            }

            SetEffect(obj, effectIndices, null);
        }
        else
        {
            ColoredString stringValue = ColoredString.Parser.Parse(text, index, obj.Surface);
            stringValue.SetForeground(foreground);
            PrintNoCheck(obj, index, stringValue);
        }
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Draws the string on the console at the specified location with the specified foreground and background color, wrapping if needed.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="text">The string to display.</param>
    /// <param name="foreground">Sets the foreground of all characters in the text.</param>
    /// <param name="background">Sets the background of all characters in the text.</param>
    public static void Print(this ISurface obj, int x, int y, string text, Color foreground, Color background)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        if (!obj.Surface.UsePrintProcessor)
        {
            var effectIndices = new List<int>(text.Length);
            int end = index + text.Length > obj.Surface.Count ? obj.Surface.Count : index + text.Length;
            int charIndex = 0;

            for (; index < end; index++)
            {
                obj.Surface[index].Glyph = text[charIndex];
                obj.Surface[index].Background = background;
                obj.Surface[index].Foreground = foreground;

                effectIndices.Add(index);
                charIndex++;
            }

            SetEffect(obj, effectIndices, null);
        }
        else
        {
            ColoredString stringValue = ColoredString.Parser.Parse(text, index, obj.Surface);
            stringValue.SetForeground(foreground);
            stringValue.SetBackground(background);

            PrintNoCheck(obj, index, stringValue);
        }
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Draws the string on the console at the specified location with the specified settings.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="text">The string to display.</param>
    /// <param name="foreground">Sets the foreground of all characters in the text.</param>
    /// <param name="background">Sets the background of all characters in the text.</param>
    /// <param name="mirror">The mirror to set on all characters in the text.</param>
    public static void Print(this ISurface obj, int x, int y, string text, Color foreground, Color background, Mirror mirror)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        if (!obj.Surface.UsePrintProcessor)
        {
            var effectIndices = new List<int>(text.Length);
            int end = index + text.Length > obj.Surface.Count ? obj.Surface.Count : index + text.Length;
            int charIndex = 0;

            for (; index < end; index++)
            {
                obj.Surface[index].Glyph = text[charIndex];

                obj.Surface[index].Background = background;
                obj.Surface[index].Foreground = foreground;
                obj.Surface[index].Mirror = mirror;

                effectIndices.Add(index);
                charIndex++;
            }

            SetEffect(obj, effectIndices, null);
        }
        else
        {
            ColoredString stringValue = ColoredString.Parser.Parse(text, index, obj.Surface);
            stringValue.SetForeground(foreground);
            stringValue.SetBackground(background);
            stringValue.SetMirror(mirror);

            PrintNoCheck(obj, index, stringValue);
        }
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Draws the string on the console at the specified location with the specified settings.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="text">The string to display.</param>
    /// <param name="foreground">Sets the foreground of all characters in the text.</param>
    /// <param name="background">Sets the background of all characters in the text.</param>
    /// <param name="mirror">The mirror to set on all characters in the text.</param>
    /// <param name="decorators">An array of cell decorators to use on each glyph. A <see langword="null"/> value will clear the decorators.</param>
    public static void Print(this ISurface obj, int x, int y, string text, Color foreground, Color background, Mirror mirror, CellDecorator[] decorators)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        if (!obj.Surface.UsePrintProcessor)
        {
            var effectIndices = new List<int>(text.Length);
            int end = index + text.Length > obj.Surface.Count ? obj.Surface.Count : index + text.Length;
            int charIndex = 0;

            for (; index < end; index++)
            {
                obj.Surface[index].Glyph = text[charIndex];

                obj.Surface[index].Background = background;
                obj.Surface[index].Foreground = foreground;
                obj.Surface[index].Mirror = mirror;
                CellDecoratorHelpers.SetDecorators(decorators, obj.Surface[index]);

                effectIndices.Add(index);
                charIndex++;
            }

            SetEffect(obj, effectIndices, null);
        }
        else
        {
            ColoredString stringValue = ColoredString.Parser.Parse(text, index, obj.Surface);
            stringValue.SetForeground(foreground);
            stringValue.SetBackground(background);
            stringValue.SetMirror(mirror);
            stringValue.SetDecorators(decorators);

            PrintNoCheck(obj, index, stringValue);
        }
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Draws the string on the console at the specified location with the specified settings.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="text">The string to display.</param>
    /// <param name="mirror">The mirror to set on all characters in the text.</param>
    public static void Print(this ISurface obj, int x, int y, string text, Mirror mirror)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        if (!obj.Surface.UsePrintProcessor)
        {
            var effectIndices = new List<int>(text.Length);
            int total = index + text.Length > obj.Surface.Count ? obj.Surface.Count : index + text.Length;
            int charIndex = 0;

            for (; index < total; index++)
            {
                obj.Surface[index].Glyph = text[charIndex];
                obj.Surface[index].Mirror = mirror;

                effectIndices.Add(index);
                charIndex++;
            }

            SetEffect(obj, effectIndices, null);
        }
        else
        {
            ColoredString stringValue = ColoredString.Parser.Parse(text, index, obj.Surface);
            stringValue.SetMirror(mirror);

            PrintNoCheck(obj, index, stringValue);
        }
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Draws the string on the console at the specified location, wrapping if needed.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="text">The string to display.</param>
    /// <param name="appearance">The appearance of the cell</param>
    /// <param name="effect">An optional effect to apply to the printed obj.Surface.</param>
    public static void Print(this ISurface obj, int x, int y, string text, ColoredGlyphBase appearance, ICellEffect? effect = null)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        if (!obj.Surface.UsePrintProcessor)
        {
            var effectIndices = new List<int>(text.Length);
            int end = index + text.Length > obj.Surface.Count ? obj.Surface.Count : index + text.Length;
            int charIndex = 0;

            for (; index < end; index++)
            {
                ColoredGlyphBase cell = obj.Surface[index];
                appearance.CopyAppearanceTo(cell, false);
                cell.Glyph = text[charIndex];
                effectIndices.Add(index);
                charIndex++;
            }

            SetEffect(obj, effectIndices, effect);
        }
        else
        {
            ColoredString stringValue = ColoredString.Parser.Parse(text, index, obj.Surface);
            stringValue.SetForeground(appearance.Foreground);
            stringValue.SetBackground(appearance.Background);
            stringValue.SetMirror(appearance.Mirror);
            stringValue.SetDecorators(appearance.Decorators is not null ? appearance.Decorators : Array.Empty<CellDecorator>());

            PrintNoCheck(obj, index, stringValue);
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Draws the string on the console at the specified location, wrapping if needed.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="text">The string to display.</param>
    public static void Print(this ISurface obj, int x, int y, ColoredString text)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index)) return;

        PrintNoCheck(obj, index, text);
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Draws the string on the console at the specified location, wrapping if needed.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="glyphs">An array of glyphs to print at the specified position.</param>
    public static void Print(this ISurface obj, int x, int y, ColoredGlyphBase[] glyphs) =>
        Print(obj, x, y, new ColoredString(glyphs));

    /// <summary>
    /// Draws the string on the console at the specified location, wrapping if needed.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">X location of the text.</param>
    /// <param name="y">Y location of the text.</param>
    /// <param name="glyphs">An enumeration of glyphs to print at the specified position.</param>
    public static void Print(this ISurface obj, int x, int y, IEnumerable<ColoredGlyphBase> glyphs) =>
        Print(obj, x, y, new ColoredString(glyphs.ToArray()));

    private static void PrintNoCheck(this ISurface obj, int index, ColoredString text)
    {
        int end = index + text.Length > obj.Surface.Count ? obj.Surface.Count : index + text.Length;
        int charIndex = 0;

        for (; index < end; index++)
        {
            if (!text.IgnoreGlyph)
                obj.Surface[index].Glyph = text[charIndex].GlyphCharacter;

            if (!text.IgnoreBackground)
                obj.Surface[index].Background = text[charIndex].Background;

            if (!text.IgnoreForeground)
                obj.Surface[index].Foreground = text[charIndex].Foreground;

            if (!text.IgnoreMirror)
                obj.Surface[index].Mirror = text[charIndex].Mirror;

            if (!text.IgnoreEffect)
                SetEffect(obj, index, text[charIndex].Effect);

            if (!text.IgnoreDecorators)
                obj.Surface[index].Decorators = text[charIndex].Decorators;

            charIndex++;
        }
        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Builds a string from the text surface from the specified coordinates.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x position of the surface to start at.</param>
    /// <param name="y">The y position of the surface to start at.</param>
    /// <param name="length">How many characters to fill the string with.</param>
    /// <returns>A string built from the text surface data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetString(this ISurface obj, int x, int y, int length) =>
        GetString(obj, y * obj.Surface.Width + x, length);

    /// <summary>
    /// Builds a string from the cells.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">Where to start getting characters from.</param>
    /// <param name="length">How many characters to fill the string with.</param>
    /// <returns>A string built from the text surface data.</returns>
    public static string GetString(this ISurface obj, int index, int length)
    {
        if (index >= 0 && index < obj.Surface.Count)
        {
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int tempIndex = i + index;

                if (tempIndex < obj.Surface.Count)
                {
                    sb.Append((char)obj.Surface[tempIndex].Glyph);
                }
            }

            return sb.ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// Builds a string from the text surface from the specified coordinates.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x position of the surface to start at.</param>
    /// <param name="y">The y position of the surface to start at.</param>
    /// <param name="length">How many characters to fill the string with.</param>
    /// <returns>A string built from the text surface data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColoredString GetStringColored(this ISurface obj, int x, int y, int length) =>
        GetStringColored(obj, y * obj.Surface.Width + x, length);

    /// <summary>
    /// Builds a string from the text surface.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="index">Where to start getting characters from.</param>
    /// <param name="length">How many characters to fill the string with.</param>
    /// <returns>A string built from the text surface data.</returns>
    public static ColoredString GetStringColored(this ISurface obj, int index, int length)
    {
        if (index < 0 || index >= obj.Surface.Count)
        {
            return new ColoredString(string.Empty);
        }

        var sb = new ColoredString(length);

        for (int i = 0; i < length; i++)
        {
            int tempIndex = i + index;
            ColoredGlyphAndEffect cell = sb[i];
            if (tempIndex < obj.Surface.Count)
            {
                obj.Surface[tempIndex].CopyAppearanceTo(cell);
            }
        }

        return sb;

    }

    /// <summary>
    /// Resets the shifted amounts to 0, as if the surface has never shifted.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    public static void ClearShiftValues(this ISurface obj)
    {
        obj.Surface.TimesShiftedDown = 0;
        obj.Surface.TimesShiftedUp = 0;
        obj.Surface.TimesShiftedLeft = 0;
        obj.Surface.TimesShiftedRight = 0;
    }

    #region Shift Rows

    /// <summary>
    /// Shifts the entire row by the specified amount.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="row">The row to shift.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    public static void ShiftRow(this ISurface obj, int row, int amount, bool wrap)
    {
        if (amount == 0) return;
        if (amount < 0)
            ShiftRowLeft(obj, row, 0, obj.Surface.Width, -amount, wrap);
        else
            ShiftRowRight(obj, row, 0, obj.Surface.Width, amount, wrap);
    }

    /// <summary>
    /// Shifts the specified row from an X position, by the specified amount.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="row">The row to shift.</param>
    /// <param name="startingX">The starting column to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingX"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    /// <exception cref="ArgumentOutOfRangeException">One of the parameters is outside of the surface width.</exception>
    public static void ShiftRow(this ISurface obj, int row, int startingX, int count, int amount, bool wrap)
    {
        if (amount == 0) return;
        if (startingX < 0 || startingX >= obj.Surface.Width) throw new ArgumentOutOfRangeException(nameof(startingX), "Column must be 0 or more and less than the width of the obj.Surface.");
        if (row < 0 || row > obj.Surface.Height) throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0 or more and less than the height of the obj.Surface.");
        if (startingX + count > obj.Surface.Width)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Count must be less than the width of the console subtract the starting X position of the shift.");

        if (amount < 0)
            ShiftRowLeftUnchecked(obj, row, startingX, count, -amount, wrap);
        else
            ShiftRowRightUnchecked(obj, row, startingX, count, amount, wrap);
    }

    /// <summary>
    /// Shifts the specified row from an X position, by the specified amount, to the right.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="row">The row to shift.</param>
    /// <param name="startingX">The starting column to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingX"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    /// <exception cref="ArgumentOutOfRangeException">One of the parameters is outside of the surface width.</exception>
    public static void ShiftRowRight(this ISurface obj, int row, int startingX, int count, int amount, bool wrap)
    {
        if (amount == 0) return;
        if (row < 0 || row >= obj.Surface.Height) throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0 or more and less than the height of the obj.Surface.");
        if (startingX + count > obj.Surface.Width)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Count must be less than the width of the console subtract the starting X position of the shift.");

        ShiftRowRightUnchecked(obj, row, startingX, count, amount, wrap);
    }

    /// <summary>
    /// Shifts the specified row from an X position, by the specified amount, to the left.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="row">The row to shift.</param>
    /// <param name="startingX">The starting column to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingX"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    /// <exception cref="ArgumentOutOfRangeException">One of the parameters is outside of the surface width.</exception>
    public static void ShiftRowLeft(this ISurface obj, int row, int startingX, int count, int amount, bool wrap)
    {
        if (amount == 0) return;
        if (row < 0 || row >= obj.Surface.Height) throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0 or more and less than the height of the obj.Surface.");
        if (startingX + count > obj.Surface.Width)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Count must be less than the width of the console subtract the starting X position of the shift.");

        ShiftRowLeftUnchecked(obj, row, startingX, count, amount, wrap);
    }

    /// <summary>
    /// Internal use. Doesn't do any checks on valid values. Shifts the specified row from an X position, by the specified amount, to the right.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="row">The row to shift.</param>
    /// <param name="startingX">The starting column to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingX"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    public static void ShiftRowRightUnchecked(this ISurface obj, int row, int startingX, int count, int amount,
                                              bool wrap)
    {
        if (wrap)
        {
            // Simplify wrap to minimum needed number
            amount %= count;

            // If count was a multiple of width, everything will end up back where it started so we're done
            if (amount == 0) return;

            // Any wrapping shift-right by n is equivalent to a shift-left by width - n.  Because we have to
            // allocate a temporary array the size of the value we're shifting during the algorithm,
            // we'll optimize it by making sure that value is as small as possible.  The largest shift
            // value we will actually process will then be width / 2.
            if (count - amount < amount)
            {
                ShiftRowLeftUnchecked(obj, row, startingX, count, count - amount, true);
                return;
            }
        }

        ShiftRowRightUncheckedReuseArray(obj.Surface, null, row, startingX, count, amount, wrap);
    }

    private static void ShiftRowRightUncheckedReuseArray(ICellSurface surface, ColoredGlyphAppearance[]? tempArray, int row, int startingX, int count, int amount, bool wrap)
    {
        if (wrap)
        {
            // Temporary array size of shift value
            tempArray ??= new ColoredGlyphAppearance[amount];
            // Offset for tempArray
            int tempArrayOffset = count - amount;

            // Shift each cell to its proper location, using temporary storage as needed.
            for (int i = count - 1; i >= 0; i--)
            {
                int x = i + startingX;
                // In this case, we'll be replacing a wrapped-around cell; so save the cell off
                // before we overwrite so that we can get the value back later when we need to shift
                // it down.
                if (i >= tempArrayOffset)
                    tempArray[i - tempArrayOffset] = new ColoredGlyphAppearance(surface[x, row]);

                // Copy appearance from the appropriate location
                int copyFromX = x - amount;
                if (copyFromX >= startingX)
                    surface[x, row].CopyAppearanceFrom(surface[copyFromX, row], false);
                else
                    tempArray[i].ShallowCopyTo(surface[x, row]);
            }
        }
        else // Shift and clear as needed
        {
            for (int i = count - 1; i >= 0; i--)
            {
                int x = i + startingX;
                int copyFromX = x - amount;
                if (copyFromX >= startingX)
                    surface[x, row].CopyAppearanceFrom(surface[copyFromX, row]);
                else
                    surface.Clear(x, row);
            }
        }

        surface.IsDirty = true;
    }

    /// <summary>
    /// Internal use. Doesn't do any checks on valid values. Shifts the specified row from an X position, by the specified amount, to the left.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="row">The row to shift.</param>
    /// <param name="startingX">The starting column to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingX"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    public static void ShiftRowLeftUnchecked(this ISurface obj, int row, int startingX, int count, int amount,
                                             bool wrap)
    {
        if (wrap)
        {
            // Simplify wrap to minimum needed number
            amount %= count;

            // If count was a multiple of width, everything will end up back where it started so we're done
            if (amount == 0) return;

            // Any wrapping shift-left by n is equivalent to a shift-right by width - n.  Because we have to
            // allocate a temporary array the size of the value we're shifting during the algorithm,
            // we'll optimize it by making sure that value is as small as possible.  The largest shift
            // value we will actually process will then be width / 2.
            if (count - amount < amount)
            {
                ShiftRowRightUnchecked(obj, row, startingX, count, count - amount, true);
                return;
            }
        }

        ShiftRowLeftUncheckedReuseArray(obj.Surface, null, row, startingX, count, amount, wrap);
    }

    private static void ShiftRowLeftUncheckedReuseArray(ICellSurface surface, ColoredGlyphAppearance[]? tempArray, int row, int startingX, int count, int amount, bool wrap)
    {
        if (wrap)
        {
            // Temporary array size of shift value
            tempArray ??= new ColoredGlyphAppearance[amount];

            // Shift each cell to its proper location, using temporary storage as needed.
            for (int i = 0; i < count; i++)
            {
                int x = i + startingX;
                // In this case, we'll be replacing a wrapped-around cell; so save the cell off
                // before we overwrite so that we can get the value back later when we need to shift
                // it down.
                if (i < amount)
                    tempArray[i] = new ColoredGlyphAppearance(surface[x, row]);

                if (i + amount < count)
                    surface[x, row].CopyAppearanceFrom(surface[x + amount, row], false);
                else
                    tempArray[i + amount - count].ShallowCopyTo(surface[x, row]);
            }
        }
        else // Shift and clear as needed
        {
            for (int i = 0; i < count; i++)
            {
                int x = i + startingX;
                int copyFromX = x + amount;
                if (copyFromX < startingX + count)
                    surface[x, row].CopyAppearanceFrom(surface[copyFromX, row], false);
                else
                    surface.Clear(x, row);
            }
        }

        surface.IsDirty = true;
    }

    #endregion

    #region Shift Columns

    /// <summary>
    /// Shifts the entire column by the specified amount.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="col">The column to shift.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    public static void ShiftColumn(this ISurface obj, int col, int amount, bool wrap)
    {
        if (amount == 0) return;
        if (amount < 0)
            ShiftColumnUp(obj, col, 0, obj.Surface.Height, -amount, wrap);
        else
            ShiftColumnDown(obj, col, 0, obj.Surface.Height, amount, wrap);
    }

    /// <summary>
    /// Shifts the specified row from an X position, by the specified amount.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="col">The column to shift.</param>
    /// <param name="startingY">The starting row to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingY"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    /// <exception cref="ArgumentOutOfRangeException">One of the parameters is outside of the surface height.</exception>
    public static void ShiftColumn(this ISurface obj, int col, int startingY, int count, int amount, bool wrap)
    {
        if (amount == 0) return;
        if (startingY < 0 || startingY >= obj.Surface.Height) throw new ArgumentOutOfRangeException(nameof(startingY), "Column must be 0 or more and less than the width of the obj.Surface.");
        if (col < 0 || col > obj.Surface.Width) throw new ArgumentOutOfRangeException(nameof(col), "Col must be 0 or more and less than the width of the obj.Surface.");
        if (startingY + count > obj.Surface.Height)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Count must be less than the height of the console subtract the starting Y position of the shift.");

        if (amount < 0)
            ShiftColumnUpUnchecked(obj, col, startingY, count, -amount, wrap);
        else
            ShiftColumnDownUnchecked(obj, col, startingY, count, amount, wrap);
    }

    /// <summary>
    /// Shifts the specified row from n Y position, by the specified amount, down.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="col">The column to shift.</param>
    /// <param name="startingY">The starting row to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingY"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    /// <exception cref="ArgumentOutOfRangeException">One of the parameters is outside of the surface height.</exception>
    public static void ShiftColumnDown(this ISurface obj, int col, int startingY, int count, int amount, bool wrap)
    {
        if (amount == 0) return;
        if (col < 0 || col > obj.Surface.Width) throw new ArgumentOutOfRangeException(nameof(col), "Col must be 0 or more and less than the width of the obj.Surface.");
        if (startingY + count > obj.Surface.Height)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Count must be less than the height of the console subtract the starting Y position of the shift.");

        ShiftColumnDownUnchecked(obj, col, startingY, count, amount, wrap);
    }

    /// <summary>
    /// Shifts the specified row from n Y position, by the specified amount, up.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="col">The column to shift.</param>
    /// <param name="startingY">The starting row to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingY"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    /// <exception cref="ArgumentOutOfRangeException">One of the parameters is outside of the surface height.</exception>
    public static void ShiftColumnUp(this ISurface obj, int col, int startingY, int count, int amount, bool wrap)
    {
        if (amount == 0) return;
        if (col < 0 || col > obj.Surface.Width) throw new ArgumentOutOfRangeException(nameof(col), "Col must be 0 or more and less than the width of the obj.Surface.");
        if (startingY + count > obj.Surface.Height)
            throw new ArgumentOutOfRangeException(nameof(count),
                "Count must be less than the height of the console subtract the starting X position of the shift.");

        ShiftColumnUpUnchecked(obj, col, startingY, count, amount, wrap);
    }

    /// <summary>
    /// Internal use. Doesn't do any checks on valid values. Shifts the specified row from a Y position, by the specified amount, down.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="col">The column to shift.</param>
    /// <param name="startingY">The starting column to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingY"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    public static void ShiftColumnDownUnchecked(this ISurface obj, int col, int startingY, int count,
                                                int amount, bool wrap)
    {
        if (wrap)
        {
            // Simplify wrap to minimum needed number
            amount %= count;

            // If count was a multiple of width, everything will end up back where it started so we're done
            if (amount == 0) return;

            // Any wrapping shift-down by n is equivalent to a shift-up by count - n.  Because we have to
            // allocate a temporary array the size of the value we're shifting during the algorithm,
            // we'll optimize it by making sure that value is as small as possible.  The largest shift
            // value we will actually process will then be count / 2.
            if (count - amount < amount)
            {
                ShiftColumnUpUnchecked(obj, col, startingY, count, count - amount, true);
                return;
            }
        }

        ShiftColumnDownUncheckedReuseArray(obj.Surface, null, col, startingY, count, amount, wrap);
    }

    private static void ShiftColumnDownUncheckedReuseArray(ICellSurface surface, ColoredGlyphAppearance[]? tempArray, int col, int startingY, int count, int amount, bool wrap)
    {
        if (wrap)
        {
            // Temporary array size of shift value
            tempArray ??= new ColoredGlyphAppearance[amount];
            // Offset for tempArray
            int tempArrayOffset = count - amount;

            // Shift each cell to its proper location, using temporary storage as needed.
            for (int i = count - 1; i >= 0; i--)
            {
                int y = i + startingY;
                // In this case, we'll be replacing a wrapped-around cell; so save the cell off
                // before we overwrite so that we can get the value back later when we need to shift
                // it down.
                if (i >= tempArrayOffset)
                    tempArray[i - tempArrayOffset] = new ColoredGlyphAppearance(surface[col, y]);

                // Copy appearance from the appropriate location
                int copyFromY = y - amount;
                if (copyFromY >= startingY)
                    surface[col, y].CopyAppearanceFrom(surface[col, copyFromY], false);
                else
                    tempArray[i].ShallowCopyTo(surface[col, y]);
            }
        }
        else // Shift and clear as needed
        {
            for (int i = count - 1; i >= 0; i--)
            {
                int y = i + startingY;
                int copyFromY = y - amount;
                if (copyFromY >= startingY)
                    surface[col, y].CopyAppearanceFrom(surface[col, copyFromY]);
                else
                    surface.Clear(col, y);
            }
        }

        surface.IsDirty = true;
    }

    /// <summary>
    /// Internal use. Doesn't do any checks on valid values. Shifts the specified row from a Y position, by the specified amount, up.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="col">The column to shift.</param>
    /// <param name="startingY">The starting column to shift from.</param>
    /// <param name="count">The number of cells to shift starting from <paramref name="startingY"/>.</param>
    /// <param name="amount">The amount to shift by. A negative value shifts left and a positive value shifts right.</param>
    /// <param name="wrap">When <see langword="true" />, wraps the glyph data from one side to another, otherwise clears the glyphs left behind.</param>
    public static void ShiftColumnUpUnchecked(this ISurface obj, int col, int startingY, int count, int amount,
                                              bool wrap)
    {
        if (wrap)
        {
            // Simplify wrap to minimum needed number
            amount %= count;

            // If count was a multiple of width, everything will end up back where it started so we're done
            if (amount == 0) return;

            // Any wrapping shift-left by n is equivalent to a shift-right by width - n.  Because we have to
            // allocate a temporary array the size of the value we're shifting during the algorithm,
            // we'll optimize it by making sure that value is as small as possible.  The largest shift
            // value we will actually process will then be width / 2.
            if (count - amount < amount)
            {
                ShiftColumnDownUnchecked(obj, col, startingY, count, count - amount, true);
                return;
            }
        }

        ShiftColumnUpUncheckedReuseArray(obj.Surface, null, col, startingY, count, amount, wrap);
    }

    private static void ShiftColumnUpUncheckedReuseArray(ICellSurface surface, ColoredGlyphAppearance[]? tempArray, int col, int startingY, int count, int amount, bool wrap)
    {
        if (wrap)
        {
            // Temporary array size of shift value
            tempArray ??= new ColoredGlyphAppearance[amount];

            // Shift each cell to its proper location, using temporary storage as needed.
            for (int i = 0; i < count; i++)
            {
                int y = i + startingY;
                // In this case, we'll be replacing a wrapped-around cell; so save the cell off
                // before we overwrite so that we can get the value back later when we need to shift
                // it down.
                if (i < amount)
                    tempArray[i] = new ColoredGlyphAppearance(surface[col, y]);

                if (i + amount < count)
                    surface[col, y].CopyAppearanceFrom(surface[col, y + amount], false);
                else
                    tempArray[i + amount - count].ShallowCopyTo(surface[col, y]);
            }
        }
        else // Shift and clear as needed
        {
            for (int i = 0; i < count; i++)
            {
                int y = i + startingY;
                int copyFromY = y + amount;
                if (copyFromY < startingY + count)
                    surface[col, y].CopyAppearanceFrom(surface[col, copyFromY], false);
                else
                    surface.Clear(col, y);
            }
        }

        surface.IsDirty = true;
    }
    #endregion

    /// <summary>
    /// Scrolls all the console data up by one.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftUp(this ISurface obj) =>
        ShiftUp(obj, 1);

    /// <summary>
    /// Scrolls all the console data up by the specified amount of rows.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="amount">How many rows to shift.</param>
    /// <param name="wrap">When false, a blank line appears at the bottom. When true, the top line appears at the bottom.</param>
    public static void ShiftUp(this ISurface obj, int amount, bool wrap = false)
    {
        if (amount == 0)
            return;

        if (amount < 0)
        {
            ShiftDown(obj, Math.Abs(amount), wrap);
            return;
        }

        obj.Surface.TimesShiftedUp += amount;
        ShiftUpUnchecked(obj.Surface, amount, wrap);
    }

    private static void ShiftUpUnchecked(ICellSurface surface, int amount, bool wrap = false)
    {
        int count = surface.Height;
        if (wrap)
        {
            // Simplify wrap to minimum needed number
            amount %= count;

            // If count was a multiple of height, everything will end up back where it started so we're done
            if (amount == 0) return;

            ColoredGlyphAppearance[]? tempArray;

            // Any wrapping shift-up by n is equivalent to a shift-down by height - n.  Because we have to
            // allocate a temporary array the size of the value we're shifting during the algorithm,
            // we'll optimize it by making sure that value is as small as possible.  The largest shift
            // value we will actually process will then be height / 2.
            if (count - amount < amount)
            {
                amount = count - amount;
                tempArray = new ColoredGlyphAppearance[amount];
                for (int x = 0; x < surface.Width; x++)
                    ShiftColumnDownUncheckedReuseArray(surface, tempArray, x, 0, count, amount, true);
                return;
            }

            tempArray = new ColoredGlyphAppearance[amount];
            for (int x = 0; x < surface.Width; x++)
                ShiftColumnUpUncheckedReuseArray(surface, tempArray, x, 0, count, amount, true);
        }
        else
            for (int x = 0; x < surface.Width; x++)
                ShiftColumnUpUncheckedReuseArray(surface, null, x, 0, count, amount, false);
    }

    /// <summary>
    /// Scrolls all the console data down by one.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftDown(this ISurface obj) =>
        ShiftDown(obj, 1);

    /// <summary>
    /// Scrolls all the console data down by the specified amount of rows.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="amount">How many rows to shift.</param>
    /// <param name="wrap">When false, a blank line appears at the top. When true, the bottom line appears at the top.</param>
    public static void ShiftDown(this ISurface obj, int amount, bool wrap = false)
    {
        if (amount == 0)
            return;

        if (amount < 0)
        {
            ShiftUp(obj, Math.Abs(amount), wrap);
            return;
        }

        obj.Surface.TimesShiftedDown += amount;
        ShiftDownUnchecked(obj.Surface, amount, wrap);
    }

    private static void ShiftDownUnchecked(ICellSurface surface, int amount, bool wrap = false)
    {
        int count = surface.Surface.Height;
        if (wrap)
        {
            // Simplify wrap to minimum needed number
            amount %= count;

            // If count was a multiple of height, everything will end up back where it started so we're done
            if (amount == 0) return;

            ColoredGlyphAppearance[]? tempArray;

            // Any wrapping shift-up by n is equivalent to a shift-down by height - n.  Because we have to
            // allocate a temporary array the size of the value we're shifting during the algorithm,
            // we'll optimize it by making sure that value is as small as possible.  The largest shift
            // value we will actually process will then be height / 2.
            if (count - amount < amount)
            {
                amount = count - amount;
                tempArray = new ColoredGlyphAppearance[amount];
                for (int x = 0; x < surface.Surface.Width; x++)
                    ShiftColumnUpUncheckedReuseArray(surface, tempArray, x, 0, count, amount, true);
                return;
            }

            tempArray = new ColoredGlyphAppearance[amount];
            for (int x = 0; x < surface.Surface.Width; x++)
                ShiftColumnDownUncheckedReuseArray(surface, tempArray, x, 0, count, amount, true);
        }
        else
            for (int x = 0; x < surface.Surface.Width; x++)
                ShiftColumnDownUncheckedReuseArray(surface, null, x, 0, count, amount, false);
    }

    /// <summary>
    /// Scrolls all the console data right by one.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftRight(this ISurface obj) =>
        ShiftRight(obj, 1);

    /// <summary>
    /// Scrolls all the console data right by the specified amount.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="amount">How much to scroll.</param>
    /// <param name="wrap">When false, a blank line appears at the left. When true, the right line appears at the left.</param>
    public static void ShiftRight(this ISurface obj, int amount, bool wrap = false)
    {
        if (amount == 0)
            return;

        if (amount < 0)
        {
            ShiftLeft(obj, Math.Abs(amount), wrap);
            return;
        }

        obj.Surface.TimesShiftedRight += amount;
        ShiftRightUnchecked(obj.Surface, amount, wrap);
    }

    private static void ShiftRightUnchecked(ICellSurface surface, int amount, bool wrap = false)
    {
        int count = surface.Width;
        if (wrap)
        {
            // Simplify wrap to minimum needed number
            amount %= count;

            // If count was a multiple of height, everything will end up back where it started so we're done
            if (amount == 0) return;

            ColoredGlyphAppearance[]? tempArray;

            // Any wrapping shift-right by n is equivalent to a shift-left by width - n.  Because we have to
            // allocate a temporary array the size of the value we're shifting during the algorithm,
            // we'll optimize it by making sure that value is as small as possible.  The largest shift
            // value we will actually process will then be width / 2.
            if (count - amount < amount)
            {
                amount = count - amount;
                tempArray = new ColoredGlyphAppearance[amount];
                for (int y = 0; y < surface.Surface.Height; y++)
                    ShiftRowLeftUncheckedReuseArray(surface, tempArray, y, 0, count, amount, true);
                return;
            }

            tempArray = new ColoredGlyphAppearance[amount];
            for (int y = 0; y < surface.Height; y++)
                ShiftRowRightUncheckedReuseArray(surface, tempArray, y, 0, count, amount, true);
        }
        else
            for (int y = 0; y < surface.Height; y++)
                ShiftRowRightUncheckedReuseArray(surface, null, y, 0, count, amount, false);
    }

    /// <summary>
    /// Scrolls all the console data left by one.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ShiftLeft(this ISurface obj) =>
        ShiftLeft(obj, 1);

    /// <summary>
    /// Scrolls all the console data left by the specified amount.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="amount">How much to scroll.</param>
    /// <param name="wrap">When false, a blank line appears at the right. When true, the left line appears at the right.</param>
    public static void ShiftLeft(this ISurface obj, int amount, bool wrap = false)
    {
        if (amount == 0)
            return;

        if (amount < 0)
        {
            ShiftRight(obj, Math.Abs(amount), wrap);
            return;
        }

        obj.Surface.TimesShiftedLeft += amount;
        ShiftLeftUnchecked(obj.Surface, amount, wrap);
    }

    private static void ShiftLeftUnchecked(ICellSurface surface, int amount, bool wrap = false)
    {
        int count = surface.Width;
        if (wrap)
        {
            // Simplify wrap to minimum needed number
            amount %= count;

            // If count was a multiple of height, everything will end up back where it started so we're done
            if (amount == 0) return;

            ColoredGlyphAppearance[]? tempArray;

            // Any wrapping shift-right by n is equivalent to a shift-left by width - n.  Because we have to
            // allocate a temporary array the size of the value we're shifting during the algorithm,
            // we'll optimize it by making sure that value is as small as possible.  The largest shift
            // value we will actually process will then be width / 2.
            if (count - amount < amount)
            {
                amount = count - amount;
                tempArray = new ColoredGlyphAppearance[amount];
                for (int y = 0; y < surface.Height; y++)
                    ShiftRowRightUncheckedReuseArray(surface, tempArray, y, 0, count, amount, true);
                return;
            }

            tempArray = new ColoredGlyphAppearance[amount];
            for (int y = 0; y < surface.Height; y++)
                ShiftRowLeftUncheckedReuseArray(surface, tempArray, y, 0, count, amount, true);
        }
        else
            for (int y = 0; y < surface.Height; y++)
                ShiftRowLeftUncheckedReuseArray(surface, null, y, 0, count, amount, false);
    }

    /// <summary>
    /// Starting at the specified coordinate, clears the glyph, mirror, and decorators, for the specified count of obj.Surface. Doesn't clear the effect, foreground, or background.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="count">The count of glyphs to erase.</param>
    /// <returns>The cells processed by this method.</returns>
    /// <remarks>
    /// Cells altered by this method has the <see cref="ColoredGlyphBase.Glyph"/> set to <see cref="ICellSurface.DefaultGlyph"/>, the <see cref="ColoredGlyphBase.Decorators"/> array reset, and the <see cref="ColoredGlyphBase.Mirror"/> set to <see cref="Mirror.None"/>.
    /// </remarks>
    public static ColoredGlyphBase[] Erase(this ISurface obj, int x, int y, int count)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
        {
            return Array.Empty<ColoredGlyphBase>();
        }

        int end = index + count > obj.Surface.Count ? obj.Surface.Count : index + count;
        int total = end - index;
        ColoredGlyphBase[] result = new ColoredGlyphBase[total];
        int resultIndex = 0;
        for (; index < end; index++)
        {
            ColoredGlyphBase c = obj.Surface[index];

            c.Glyph = obj.Surface.DefaultGlyph;
            c.Mirror = Mirror.None;
            CellDecoratorHelpers.RemoveAllDecorators(c);

            result[resultIndex] = c;
            resultIndex++;
        }

        obj.Surface.IsDirty = true;
        return result;
    }

    /// <summary>
    /// Clears the glyph, mirror, and decorators, for the specified cell. Doesn't clear the effect.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <remarks>
    /// The cell altered by this method has the <see cref="ColoredGlyphBase.Glyph"/> set to <see cref="ICellSurface.DefaultGlyph"/>, the <see cref="ColoredGlyphBase.Decorators"/> array reset, and the <see cref="ColoredGlyphBase.Mirror"/> set to <see cref="Mirror.None"/>.
    /// </remarks>
    public static void Erase(this ISurface obj, int x, int y)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
        {
            return;
        }

        obj.Surface[index].Glyph = obj.Surface.DefaultGlyph;
        obj.Surface[index].Mirror = Mirror.None;
        obj.Surface[index].Decorators = null;

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Erases all cells which clears the glyph, mirror, and decorators. Doesn't clear the effect.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <remarks>
    /// All cells have <see cref="ColoredGlyphBase.Glyph"/> set to <see cref="ICellSurface.DefaultGlyph"/>, the <see cref="ColoredGlyphBase.Decorators"/> array reset, and the <see cref="ColoredGlyphBase.Mirror"/> set to <see cref="Mirror.None"/>.
    /// </remarks>
    public static void Erase(this ISurface obj)
    {
        for (int i = 0; i < obj.Surface.Count; i++)
        {
            obj.Surface[i].Glyph = obj.Surface.DefaultGlyph;
            obj.Surface[i].Mirror = Mirror.None;
            obj.Surface[i].Decorators = null;
        }

        obj.Surface.IsDirty = true;
    }


    /// <summary>
    /// Clears the console data. Characters are reset to 0, the foreground and background are set to default, and mirror set to none. Clears cell decorators.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    public static void Clear(this ISurface obj)
    {
        ColoredGlyphBase cell;

        obj.Surface.Effects.RemoveAll();

        for (int i = 0; i < obj.Surface.Count; i++)
        {
            cell = obj.Surface[i];
            cell.Clear();
            cell.Foreground = obj.Surface.DefaultForeground;
            cell.Background = obj.Surface.DefaultBackground;
            cell.Glyph = obj.Surface.DefaultGlyph;
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Clears a cell. Character is reset to 0, the foreground and background is set to default, and mirror is set to none. Clears cell decorators.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x location of the cell.</param>
    /// <param name="y">The y location of the cell.</param>
    public static void Clear(this ISurface obj, int x, int y)
    {
        if (!obj.Surface.IsValidCell(x, y)) return;

        ColoredGlyphBase cell = obj.Surface[x, y];

        obj.Surface.Effects.SetEffect(cell, null);

        cell.Clear();
        cell.Foreground = obj.Surface.DefaultForeground;
        cell.Background = obj.Surface.DefaultBackground;
        cell.Glyph = obj.Surface.DefaultGlyph;

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Clears a segment of cells, starting from the left, extending to the right, and wrapping if needed. Character is reset to 0, the foreground and background is set to default, and mirror is set to none. Clears cell decorators.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x position of the left end of the segment.</param>
    /// <param name="y">The y position of the segment.</param>
    /// <param name="length">The length of the segment. If it extends beyond the line, it will wrap to the next line. If it extends beyond the console, then it automatically ends at the last valid cell.</param>
    /// <remarks>This works similarly to printing a string of whitespace</remarks>
    public static void Clear(this ISurface obj, int x, int y, int length)
    {
        int index = new Point(x, y).ToIndex(obj.Surface.Width);
        if (index < 0 || index >= obj.Surface.Count) return;

        int end = index + length > obj.Surface.Count ? obj.Surface.Count : index + length;

        for (int i = index; i < end; i++)
        {
            ColoredGlyphBase cell = obj.Surface[i];
            obj.Surface.Effects.SetEffect(cell, null);
            cell.Clear();
            cell.Foreground = obj.Surface.DefaultForeground;
            cell.Background = obj.Surface.DefaultBackground;
            cell.Glyph = obj.Surface.DefaultGlyph;
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Clears an area of obj.Surface. Each cell is reset to its default state. Then, Glyph, foreground, and background, are reset to the surface's default values.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="area">The area to clear.</param>
    public static void Clear(this ISurface obj, Rectangle area)
    {
        area = Rectangle.GetIntersection(area, new Rectangle(0, 0, obj.Surface.Width, obj.Surface.Height));

        if (area == Rectangle.Empty) return;

        ColoredGlyphBase cell;
        
        for (int x = area.X; x < area.X + area.Width; x++)
        {
            for (int y = area.Y; y < area.Y + area.Height; y++)
            {
                cell = obj.Surface[x, y];
                obj.Surface.Effects.SetEffect(cell, null);
                cell.Clear();
                cell.Foreground = obj.Surface.DefaultForeground;
                cell.Background = obj.Surface.DefaultBackground;
                cell.Glyph = obj.Surface.DefaultGlyph;
            }
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Clears an area of obj.Surface. Each cell is reset to its default state. Then, Glyph, foreground, and background, are reset to the surface's default values.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="cellPositions">The cells to clear.</param>
    public static void Clear(this ISurface obj, IEnumerable<Point> cellPositions)
    {
        ColoredGlyphBase cell;

        foreach (Point position in cellPositions)
        {
            cell = obj.Surface[position.X, position.Y];
            obj.Surface.Effects.SetEffect(cell, null);
            cell.Clear();
            cell.Foreground = obj.Surface.DefaultForeground;
            cell.Background = obj.Surface.DefaultBackground;
            cell.Glyph = obj.Surface.DefaultGlyph;
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Fills the console. Clears cell decorators and effects.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="iconAppearance">The appearance that is copied to every cell.</param>
    /// <returns>The array of all cells in this console, starting from the top left corner.</returns>
    public static ColoredGlyphBase[] Fill(this ISurface obj, ColoredGlyphBase iconAppearance) =>
        Fill(obj, iconAppearance.Foreground, iconAppearance.Background, iconAppearance.Glyph, iconAppearance.Mirror);

    /// <summary>
    /// Fills the console. Clears cell decorators and effects.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="foreground">Foreground to apply. If null, skips.</param>
    /// <param name="background">Foreground to apply. If null, skips.</param>
    /// <param name="glyph">Glyph to apply. If null, skips.</param>
    /// <param name="mirror">Mirror to apply. If null, skips.</param>
    /// <returns>The array of all cells in this console, starting from the top left corner.</returns>
    public static ColoredGlyphBase[] Fill(this ISurface obj, Color? foreground = null, Color? background = null, int? glyph = null, Mirror? mirror = null)
    {
        ColoredGlyphBase[] glyphs = new ColoredGlyphBase[obj.Surface.Count];

        obj.Surface.Effects.RemoveAll();

        for (int i = 0; i < obj.Surface.Count; i++)
        {
            if (background.HasValue)
                obj.Surface[i].Background = background.Value;

            if (foreground.HasValue)
                obj.Surface[i].Foreground = foreground.Value;

            if (glyph.HasValue)
                obj.Surface[i].Glyph = glyph.Value;

            if (mirror.HasValue)
                obj.Surface[i].Mirror = mirror.Value;

            obj.Surface[i].Decorators = null;

            glyphs[i] = obj.Surface[i];
        }

        obj.Surface.IsDirty = true;

        return obj.Surface.GetCells(obj.Surface.Area).ToArray();
    }

    /// <summary>
    /// Fills a segment of cells, starting from the left, extending to the right, and wrapping if needed. Clears cell decorators.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x position of the left end of the segment. </param>
    /// <param name="y">The y position of the segment.</param>
    /// <param name="length">The length of the segment. If it extends beyond the line, it will wrap to the next line. If it extends beyond the console, then it automatically ends at the last valid cell.</param>
    /// <param name="foreground">Foreground to apply. If null, skips.</param>
    /// <param name="background">Background to apply. If null, skips.</param>
    /// <param name="glyph">Glyph to apply. If null, skips.</param>
    /// <param name="mirror">Mirror to apply. If null, skips.</param>
    /// <returns>An array containing the affected cells, starting from the top left corner. If x or y are out of bounds, nothing happens and an empty array is returned</returns>
    public static ColoredGlyphBase[] Fill(this ISurface obj, int x, int y, int length, Color? foreground = null, Color? background = null, int? glyph = null, Mirror? mirror = null)
    {
        if (!obj.Surface.IsValidCell(x, y, out int index))
            return Array.Empty<ColoredGlyphBase>();

        int end = index + length > obj.Surface.Count ? obj.Surface.Count : index + length;

        ColoredGlyphBase[] result = new ColoredGlyphBase[end - index];
        int counter = 0;

        for (int i = index; i < end; i++)
        {
            ColoredGlyphBase c = obj.Surface[i];

            obj.Surface.Effects.SetEffect(c, null);

            if (background.HasValue)
                c.Background = background.Value;

            if (foreground.HasValue)
                c.Foreground = foreground.Value;

            if (glyph.HasValue)
                c.Glyph = glyph.Value;

            if (mirror.HasValue)
                c.Mirror = mirror.Value;

            c.Decorators = null;

            result[counter] = c;
            counter++;
        }

        obj.Surface.IsDirty = true;
        return result;
    }

    /// <summary>
    /// Fills the specified area. Clears cell decorators.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="area">The area to fill.</param>
    /// <param name="foreground">Foreground to apply. If null, skips.</param>
    /// <param name="background">Background to apply. If null, skips.</param>
    /// <param name="glyph">Glyph to apply. If null, skips.</param>
    /// <param name="mirror">Mirror to apply. If null, skips.</param>
    /// <returns>An array containing the affected cells, starting from the top left corner. If the area is out of bounds, nothing happens and an empty array is returned.</returns>
    public static ColoredGlyphBase[] Fill(this ISurface obj, Rectangle area, Color? foreground = null, Color? background = null, int? glyph = null, Mirror? mirror = null)
    {
        area = Rectangle.GetIntersection(area, new Rectangle(0, 0, obj.Surface.Width, obj.Surface.Height));

        if (area == Rectangle.Empty)
            return Array.Empty<ColoredGlyphBase>();

        var result = new ColoredGlyphBase[area.Width * area.Height];
        int resultIndex = 0;

        for (int x = area.X; x < area.X + area.Width; x++)
        {
            for (int y = area.Y; y < area.Y + area.Height; y++)
            {
                ColoredGlyphBase cell = obj.Surface[y * obj.Surface.Width + x];

                obj.Surface.Effects.SetEffect(cell, null);

                if (background.HasValue)
                    cell.Background = background.Value;

                if (foreground.HasValue)
                    cell.Foreground = foreground.Value;

                if (glyph.HasValue)
                    cell.Glyph = glyph.Value;

                if (mirror.HasValue)
                    cell.Mirror = mirror.Value;

                cell.Decorators = null;

                result[resultIndex] = cell;
                resultIndex++;
            }
        }

        obj.Surface.IsDirty = true;
        return result;
    }

    /// <summary>
    /// Draws a line from <paramref name="start"/> to <paramref name="end"/>.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="start">Starting point of the line.</param>
    /// <param name="end">Ending point of the line.</param>
    /// <param name="foreground">Foreground to set. If null, skipped.</param>
    /// <param name="background">Background to set. If null, skipped.</param>
    /// <param name="glyph">Glyph to set. If null, skipped.</param>
    /// <param name="mirror">Mirror to set. If null, skipped.</param>
    /// <returns>A list of cells the line touched; ordered from first to last.</returns>
    /// <remarks>To simply return the list of cells that would be drawn to, use <see langword="null"/> for <paramref name="glyph"/>, <paramref name="foreground"/>, <paramref name="background"/>, and <paramref name="mirror"/>.</remarks>
    public static IEnumerable<ColoredGlyphBase> DrawLine(this ISurface obj, Point start, Point end, int? glyph, Color? foreground = null, Color? background = null, Mirror? mirror = null)
    {
        var result = new List<ColoredGlyphBase>();
        Func<int, int, bool> processor;

        if (foreground.HasValue || background.HasValue || glyph.HasValue)
        {
            processor = (x, y) =>
            {
                if (obj.Surface.IsValidCell(x, y, out int index))
                {
                    ColoredGlyphBase cell = obj.Surface[index];
                    result.Add(cell);

                    if (foreground.HasValue)
                    {
                        cell.Foreground = foreground.Value;
                        obj.Surface.IsDirty = true;
                    }
                    if (background.HasValue)
                    {
                        cell.Background = background.Value;
                        obj.Surface.IsDirty = true;
                    }
                    if (glyph.HasValue)
                    {
                        cell.Glyph = glyph.Value;
                        obj.Surface.IsDirty = true;
                    }

                    if (mirror.HasValue)
                    {
                        cell.Mirror = mirror.Value;
                        obj.Surface.IsDirty = true;
                    }

                    return true;
                }

                return false;
            };
        }
        else
        {
            processor = (x, y) =>
            {
                if (obj.Surface.IsValidCell(x, y, out int index))
                {
                    result.Add(obj.Surface[index]);
                    return true;
                }

                return false;
            };
        }
        foreach (Point location in Lines.GetBresenhamLine(start, end))
            processor(location.X, location.Y);

        return result;
    }

    /// <summary>
    /// Draws a box.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="area">The area of the box.</param>
    /// <param name="parameters">Provides the options for drawing a border and filling the box.</param>
    public static void DrawBox(this ISurface obj, Rectangle area, ShapeParameters parameters)
    {
        Rectangle fillRect = area.Expand(-1, -1);

        if (parameters.HasFill && parameters.FillGlyph != null && fillRect.Area > 0)
        {
            obj.Surface.Fill(fillRect,
                         parameters.IgnoreFillForeground ? (Color?)null : parameters.FillGlyph.Foreground,
                         parameters.IgnoreFillBackground ? (Color?)null : parameters.FillGlyph.Background,
                         parameters.IgnoreFillGlyph ? (int?)null : parameters.FillGlyph.Glyph,
                         parameters.IgnoreFillMirror ? (Mirror?)null : parameters.FillGlyph.Mirror
                         );
        }

        if (parameters.HasBorder)
        {
            // Using a line style
            if (parameters.BoxBorderStyle != null)
            {
                int[] connectedLineStyle = parameters.BoxBorderStyle;

                if (!ICellSurface.ValidateLineStyle(connectedLineStyle))
                    throw new ArgumentException("Array is either null or does not have the required line style elements", nameof(connectedLineStyle));

                ColoredGlyphBase border = parameters.BorderGlyph ?? throw new NullReferenceException("Shape parameters is missing the border glyph.");

                // Draw the major sides
                DrawLine(obj, area.Position, area.Position + new Point(area.Width - 1, 0),
                            parameters.IgnoreBorderGlyph ? (int?)null : connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Top],
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                DrawLine(obj, area.Position + new Point(0, area.Height - 1), area.Position + new Point(area.Width - 1, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Bottom],
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                DrawLine(obj, area.Position, area.Position + new Point(0, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Left],
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                DrawLine(obj, area.Position + new Point(area.Width - 1, 0), area.Position + new Point(area.Width - 1, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Right],
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);

                // Tweak the corners
                obj.Surface.SetGlyph(area.X, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopLeft]);
                obj.Surface.SetGlyph(area.MaxExtentX, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopRight]);
                obj.Surface.SetGlyph(area.X, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomLeft]);
                obj.Surface.SetGlyph(area.MaxExtentX, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomRight]);
            }
            // Using full glyph line style
            else if (parameters.BoxBorderStyleGlyphs != null)
            {
                ColoredGlyphBase[] connectedLineStyle = parameters.BoxBorderStyleGlyphs;

                if (!ICellSurface.ValidateLineStyle(connectedLineStyle))
                    throw new ArgumentException("Array is either null or does not have the required line style elements", nameof(connectedLineStyle));

                // Draw the major sides
                ColoredGlyphBase border = connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Top];
                DrawLine(obj, area.Position, area.Position + new Point(area.Width - 1, 0),
                            parameters.IgnoreBorderGlyph ? (int?)null : border.Glyph,
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                border = connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Bottom];
                DrawLine(obj, area.Position + new Point(0, area.Height - 1), area.Position + new Point(area.Width - 1, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : border.Glyph,
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                border = connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Left];
                DrawLine(obj, area.Position, area.Position + new Point(0, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : border.Glyph,
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                border = connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Right];
                DrawLine(obj, area.Position + new Point(area.Width - 1, 0), area.Position + new Point(area.Width - 1, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : border.Glyph,
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);

                // Tweak the corners
                obj.Surface.SetGlyph(area.X, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopLeft].Glyph);
                obj.Surface.SetGlyph(area.MaxExtentX, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopRight].Glyph);
                obj.Surface.SetGlyph(area.X, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomLeft].Glyph);
                obj.Surface.SetGlyph(area.MaxExtentX, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomRight].Glyph);
            }
            // Using a single glyph
            else
            {
                // Draw the major sides
                ColoredGlyphBase border = parameters.BorderGlyph ?? throw new NullReferenceException("Shape parameters is missing the border glyph.");
                DrawLine(obj, area.Position, area.Position + new Point(area.Width - 1, 0),
                            parameters.IgnoreBorderGlyph ? (int?)null : border.Glyph,
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                DrawLine(obj, area.Position + new Point(0, area.Height - 1), area.Position + new Point(area.Width - 1, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : border.Glyph,
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                DrawLine(obj, area.Position, area.Position + new Point(0, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : border.Glyph,
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                DrawLine(obj, area.Position + new Point(area.Width - 1, 0), area.Position + new Point(area.Width - 1, area.Height - 1),
                            parameters.IgnoreBorderGlyph ? (int?)null : border.Glyph,
                            parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                            parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                            parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
            }
        }
    }

    /// <summary>
    /// Draws an ellipse.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="area">The area the ellipse </param>
    /// <param name="parameters">Provides the options for drawing a border and filling the circle.</param>
    public static void DrawCircle(this ISurface obj, Rectangle area, ShapeParameters parameters)
    {
        var cells = new List<int>(area.Width * area.Height);

        if (parameters.BorderGlyph == null) throw new NullReferenceException("Shape parameters is missing the border glyph.");

        foreach (Point location in Shapes.GetEllipse(area.Position, area.MaxExtent))
        {
            if (parameters.HasBorder && obj.Surface.IsValidCell(location.X, location.Y))
            {
                ColoredGlyphBase cell = obj.Surface[location.X, location.Y];

                if (!parameters.IgnoreBorderForeground) cell.Foreground = parameters.BorderGlyph.Foreground;
                if (!parameters.IgnoreBorderBackground) cell.Background = parameters.BorderGlyph.Background;
                if (!parameters.IgnoreBorderGlyph) cell.Glyph = parameters.BorderGlyph.Glyph;
                if (!parameters.IgnoreBorderMirror) cell.Mirror = parameters.BorderGlyph.Mirror;
            }

            cells.Add(Point.ToIndex(location.X, location.Y, obj.Surface.Width));
        }

        if (parameters.HasFill && parameters.FillGlyph != null)
        {
            Func<int, bool> isTargetCell = c => !cells.Contains(c);
            Action<int> fillCell = c =>
            {
                if (obj.Surface.IsValidCell(c))
                {
                    ColoredGlyphBase cell = obj.Surface[c];

                    if (!parameters.IgnoreFillForeground) cell.Foreground = parameters.FillGlyph.Foreground;
                    if (!parameters.IgnoreFillBackground) cell.Background = parameters.FillGlyph.Background;
                    if (!parameters.IgnoreFillGlyph) cell.Glyph = parameters.FillGlyph.Glyph;
                    if (!parameters.IgnoreFillMirror) cell.Mirror = parameters.FillGlyph.Mirror;
                }

                cells.Add(c);
            };
            Func<int, Algorithms.NodeConnections<int>> getConnectedCells = c =>
            {
                var connections = new Algorithms.NodeConnections<int>();

                (int x, int y) = Point.FromIndex(c, obj.Surface.Width);

                if (IsValidCell(obj, x - 1, y))
                {
                    connections.West = Point.ToIndex(x - 1, y, obj.Surface.Width);
                    connections.HasWest = true;
                }
                if (IsValidCell(obj, x + 1, y))
                {
                    connections.East = Point.ToIndex(x + 1, y, obj.Surface.Width);
                    connections.HasEast = true;
                }
                if (IsValidCell(obj, x, y - 1))
                {
                    connections.North = Point.ToIndex(x, y - 1, obj.Surface.Width);
                    connections.HasNorth = true;
                }
                if (IsValidCell(obj, x, y + 1))
                {
                    connections.South = Point.ToIndex(x, y + 1, obj.Surface.Width);
                    connections.HasSouth = true;
                }

                return connections;
            };

            Algorithms.FloodFill(area.Center.ToIndex(obj.Surface.Width), isTargetCell, fillCell, getConnectedCells);
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Connects all lines in a surface for both <see cref="ICellSurface.ConnectedLineThin"/> and <see cref="ICellSurface.ConnectedLineThick"/> styles.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConnectLines(this ISurface obj)
    {
        ConnectLines(obj, ICellSurface.ConnectedLineThin);
        ConnectLines(obj, ICellSurface.ConnectedLineThick);
    }

    /// <summary>
    /// Connects all lines in this based on the <paramref name="lineStyle"/> style provided.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="lineStyle">The array of line styles indexed by <see cref="ICellSurface.ConnectedLineIndex"/>.</param>
    public static void ConnectLines(this ISurface obj, int[] lineStyle) =>
        ConnectLines(obj, lineStyle, obj.Surface.Area);

    /// <summary>
    /// Connects all lines in this based on the <paramref name="lineStyle"/> style provided.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="lineStyle">The array of line styles indexed by <see cref="ICellSurface.ConnectedLineIndex"/>.</param>
    /// <param name="area">The area to process.</param>
    public static void ConnectLines(this ISurface obj, int[] lineStyle, Rectangle area)
    {
        for (int x = area.X; x <= area.MaxExtentX; x++)
        {
            for (int y = area.Y; y <= area.MaxExtentY; y++)
            {
                var pos = new Point(x, y);
                int index = pos.ToIndex(obj.Surface.Width);

                // Check if this pos is a road
                if (!lineStyle.Contains(obj.Surface[index].Glyph))
                    continue;

                // Get all valid positions and indexes around this point
                bool[] validDirs = pos.GetValidDirections(area);
                int[] posIndexes = pos.GetDirectionIndexes(area, obj.Surface.Width);
                bool[] roads = new[] { false, false, false, false, false, false, false, false, false };

                for (int i = 1; i < 9; i++)
                {
                    if (!validDirs[i])
                        continue;

                    if (lineStyle.Contains(obj.Surface[posIndexes[i]].Glyph))
                        roads[i] = true;
                }

                if (roads[(int)Direction.Types.Up] &&
                roads[(int)Direction.Types.Down] &&
                roads[(int)Direction.Types.Right] &&
                roads[(int)Direction.Types.Left])
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.Middle];
                    obj.Surface.IsDirty = true;
                }
                else if (!roads[(int)Direction.Types.Up] &&
                roads[(int)Direction.Types.Down] &&
                roads[(int)Direction.Types.Right] &&
                roads[(int)Direction.Types.Left])
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.TopMiddleToDown];
                    obj.Surface.IsDirty = true;
                }
                else if (roads[(int)Direction.Types.Up] &&
                !roads[(int)Direction.Types.Down] &&
                roads[(int)Direction.Types.Right] &&
                roads[(int)Direction.Types.Left])
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.BottomMiddleToTop];
                    obj.Surface.IsDirty = true;
                }
                else if (roads[(int)Direction.Types.Up] &&
                roads[(int)Direction.Types.Down] &&
                !roads[(int)Direction.Types.Right] &&
                roads[(int)Direction.Types.Left])
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.RightMiddleToLeft];
                    obj.Surface.IsDirty = true;
                }
                else if (roads[(int)Direction.Types.Up] &&
                roads[(int)Direction.Types.Down] &&
                roads[(int)Direction.Types.Right] &&
                !roads[(int)Direction.Types.Left])
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.LeftMiddleToRight];
                    obj.Surface.IsDirty = true;
                }
                else if (!roads[(int)Direction.Types.Up] &&
                !roads[(int)Direction.Types.Down] &&
                (roads[(int)Direction.Types.Right] ||
                roads[(int)Direction.Types.Left]))
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.Top];
                    obj.Surface.IsDirty = true;
                }
                else if ((roads[(int)Direction.Types.Up] ||
                roads[(int)Direction.Types.Down]) &&
                !roads[(int)Direction.Types.Right] &&
                !roads[(int)Direction.Types.Left])
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.Left];
                    obj.Surface.IsDirty = true;
                }
                else if (roads[(int)Direction.Types.Up] &&
                !roads[(int)Direction.Types.Down] &&
                !roads[(int)Direction.Types.Right] &&
                roads[(int)Direction.Types.Left])
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.BottomRight];
                    obj.Surface.IsDirty = true;
                }
                else if (roads[(int)Direction.Types.Up] &&
                !roads[(int)Direction.Types.Down] &&
                roads[(int)Direction.Types.Right] &&
                !roads[(int)Direction.Types.Left])
                {

                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.BottomLeft];
                    obj.Surface.IsDirty = true;
                }
                else if (!roads[(int)Direction.Types.Up] &&
                roads[(int)Direction.Types.Down] &&
                !roads[(int)Direction.Types.Right] &&
                roads[(int)Direction.Types.Left])
                {
                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.TopRight];
                    obj.Surface.IsDirty = true;
                }
                else if (!roads[(int)Direction.Types.Up] &&
                roads[(int)Direction.Types.Down] &&
                roads[(int)Direction.Types.Right] &&
                !roads[(int)Direction.Types.Left])
                {
                    obj.Surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.TopLeft];
                    obj.Surface.IsDirty = true;
                }
            }
        }
    }

    /// <summary>
    /// Copies the contents of the cell surface to the destination.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <remarks>If the sizes to not match, it will always start at 0,0 and work with what it can and move on to the next row when either surface runs out of columns being processed</remarks>
    /// <param name="destination">The destination obj.Surface.</param>
    public static void Copy(this ISurface obj, ICellSurface destination)
    {
        if (obj.Surface.Width == destination.Width && obj.Surface.Height == destination.Height)
        {
            for (int i = 0; i < obj.Surface.Count; i++)
                obj.Surface[i].CopyAppearanceTo(destination[i]);

            return;
        }

        int maxX = obj.Surface.Width >= destination.Width ? destination.Width : obj.Surface.Width;
        int maxY = obj.Surface.Height >= destination.Height ? destination.Height : obj.Surface.Height;

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                if (obj.Surface.IsValidCell(x, y, out int sourceIndex) && destination.IsValidCell(x, y, out int destIndex))
                    obj.Surface[sourceIndex].CopyAppearanceTo(destination[destIndex]);
            }
        }

        destination.IsDirty = true;
    }

    /// <summary>
    /// Copies the contents of the cell surface to the destination at the specified x,y.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x coordinate of the destination.</param>
    /// <param name="y">The y coordinate of the destination.</param>
    /// <param name="destination">The destination obj.Surface.</param>
    public static void Copy(this ISurface obj, ICellSurface destination, int x, int y)
    {
        for (int curX = 0; curX < obj.Surface.Width; curX++)
        {
            for (int curY = 0; curY < obj.Surface.Height; curY++)
            {
                if (IsValidCell(obj, curX, curY, out int sourceIndex) && destination.IsValidCell(x + curX, y + curY, out int destIndex))
                    obj.Surface[sourceIndex].CopyAppearanceTo(destination[destIndex]);
            }
        }

        destination.IsDirty = true;
    }

    /// <summary>
    /// Copies an area of this cell surface to the destination surface.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="area">The area to copy.</param>
    /// <param name="destination">The destination obj.Surface.</param>
    /// <param name="destinationX">The x coordinate to copy to.</param>
    /// <param name="destinationY">The y coordinate to copy to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(this ISurface obj, Rectangle area, ICellSurface destination, int destinationX, int destinationY) =>
        Copy(obj, area.X, area.Y, area.Width, area.Height, destination, destinationX, destinationY);

    /// <summary>
    /// Copies the contents of this cell surface at the specified x,y coordinates to the destination, only with the specified obj.Surface.BufferWidth and obj.Surface.BufferHeight, and copies it to the specified <paramref name="destinationX"/> and <paramref name="destinationY"/> position.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="x">The x coordinate to start from.</param>
    /// <param name="y">The y coordinate to start from.</param>
    /// <param name="width">The BufferWidth to copy from.</param>
    /// <param name="height">The BufferHeight to copy from.</param>
    /// <param name="destination">The destination obj.Surface.</param>
    /// <param name="destinationX">The x coordinate to copy to.</param>
    /// <param name="destinationY">The y coordinate to copy to.</param>
    public static void Copy(this ISurface obj, int x, int y, int width, int height, ICellSurface destination, int destinationX, int destinationY)
    {
        int destX = destinationX;
        int destY = destinationY;

        for (int curX = 0; curX < width; curX++)
        {
            for (int curY = 0; curY < height; curY++)
            {
                if (IsValidCell(obj, curX + x, curY + y, out int sourceIndex) && destination.IsValidCell(destX, destY, out int destIndex))
                    obj.Surface[sourceIndex].CopyAppearanceTo(destination[destIndex]);

                destY++;
            }
            destY = destinationY;
            destX++;
        }

        destination.IsDirty = true;
    }

    /// <summary>
    /// Fills a console with random colors and glyphs.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="associatedFont">The font used in assigning glyphs randomly.</param>
    public static void FillWithRandomGarbage(this ISurface obj, IFont associatedFont) =>
        FillWithRandomGarbage(obj, associatedFont.TotalGlyphs);

    /// <summary>
    /// Fills a console with random colors and glyphs.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="maxGlyphValue">The maximum glyph value to use on the obj.Surface.</param>
    public static void FillWithRandomGarbage(this ISurface obj, int maxGlyphValue) =>
        FillWithRandomGarbage(obj, maxGlyphValue, obj.Surface.Area);

    /// <summary>
    /// Fills a console with random colors and glyphs.
    /// </summary>
    /// <param name="obj">The surface being edited.</param>
    /// <param name="maxGlyphValue">The maximum glyph value to use on the obj.Surface.</param>
    /// <param name="area">The area to fill with random garbage.</param>
    public static void FillWithRandomGarbage(this ISurface obj, int maxGlyphValue, Rectangle area)
    {
        //pulse.Reset();
        int charCounter = 0;
        for (int y = area.Y; y <= area.MaxExtentY; y++)
        {
            for (int x = area.X; x <= area.MaxExtentX; x++)
            {
                SetGlyph(obj, x, y, charCounter);
                SetForeground(obj, x, y, new Color((byte)GameHost.Instance.Random.Next(0, 256), (byte)GameHost.Instance.Random.Next(0, 256), (byte)GameHost.Instance.Random.Next(0, 256), (byte)255));
                SetBackground(obj, x, y, obj.Surface.DefaultBackground);
                SetBackground(obj, x, y, new Color((byte)GameHost.Instance.Random.Next(0, 256), (byte)GameHost.Instance.Random.Next(0, 256), (byte)GameHost.Instance.Random.Next(0, 256), (byte)255));
                SetMirror(obj, x, y, (Mirror)GameHost.Instance.Random.Next(0, 4));

                if (charCounter > maxGlyphValue)
                    charCounter = 0;
                charCounter++;
            }
        }

        obj.Surface.IsDirty = true;
    }

    /// <summary>
    /// Prints text using <see cref="TheDrawFont"/> and horizontal alignment specified. Calculates x coordinate. Truncates string to fit it in one line.
    /// </summary>
    /// <param name="cellSurface">Class implementing <see cref="ICellSurface"/>.</param>
    /// <param name="y">Y coordinate of the obj.Surface.</param>
    /// <param name="text">Text to print.</param>
    /// <param name="drawFont">Instance of the <see cref="TheDrawFont"/> to use.</param>
    /// <param name="alignment"><see cref="HorizontalAlignment"/> to use.</param>
    /// <param name="padding">Amount of regular font characters used as horizontal padding on both sides of the output.</param>
    public static void PrintTheDraw(this ICellSurface cellSurface, int y, string text, TheDrawFont drawFont, HorizontalAlignment alignment, int padding = 0)
    {
        if (drawFont is null) return;

        int spaceWidth = GetTheDrawSpaceCharWidth(drawFont),
            textLength = 0,
            printWidth = cellSurface.Width - padding * 2;

        string tempText = string.Empty;

        int count = text.Length;
        for (int i = 0; i < count; i++)
        {
            char item = text[i];
            char currentChar = item;
            int charWidth = 0;

            if (drawFont.IsCharacterSupported(item))
            {
                var charInfo = drawFont.GetCharacter(currentChar);
                charWidth = charInfo.Width;
            }
            else
            {
                currentChar = ' ';
                charWidth = spaceWidth;
            }

            textLength += charWidth;

            if (textLength > printWidth)
            {
                textLength -= charWidth;
                break;
            }

            tempText += currentChar;
        }

        int x = alignment switch
        {
            HorizontalAlignment.Center => (printWidth - textLength) / 2,
            HorizontalAlignment.Right => printWidth - textLength,
            _ => 0
        };

        PrintTheDraw(cellSurface, x + padding, y, tempText, drawFont);
    }

    static int GetTheDrawSpaceCharWidth(TheDrawFont drawFont) => drawFont.IsCharacterSupported(' ') ? drawFont.GetCharacter(' ').Width :
                                                                 drawFont.IsCharacterSupported('a') ? drawFont.GetCharacter('a').Width :
                                                                 drawFont.IsCharacterSupported('i') ? drawFont.GetCharacter('i').Width :
                                                                 drawFont.IsCharacterSupported('1') ? drawFont.GetCharacter('1').Width :
                                                                 2;

    /// <summary>
    /// Prints text using <see cref="TheDrawFont"/>.
    /// </summary>
    /// <param name="cellSurface">Class implementing <see cref="ICellSurface"/>.</param>
    /// <param name="x">X coordinate of the obj.Surface.</param>
    /// <param name="y">Y coordinate of the obj.Surface.</param>
    /// <param name="text">Text to print.</param>
    /// <param name="drawFont">Instance of the <see cref="TheDrawFont"/> to use.</param>
    public static void PrintTheDraw(this ICellSurface cellSurface, int x, int y, string text, TheDrawFont drawFont)
    {
        if (drawFont is null) return;

        int xPos = x;
        int yPos = y;
        int tempHeight = 0;

        int count = text.Length;
        for (int i = 0; i < count; i++)
        {
            char item = text[i];
            if (drawFont.IsCharacterSupported(item))
            {
                var charInfo = drawFont.GetCharacter(item);

                if (xPos + charInfo.Width >= cellSurface.Width)
                {
                    yPos += tempHeight + 1;
                    xPos = 0;
                }

                if (yPos >= cellSurface.Height)
                    break;

                var surfaceCharacter = drawFont.GetSurface(item);

                if (surfaceCharacter != null)
                {
                    surfaceCharacter.Copy(cellSurface, xPos, yPos);

                    if (surfaceCharacter.Height > tempHeight)
                        tempHeight = surfaceCharacter.Height;
                }

                xPos += charInfo.Width;
            }
            else if (item == ' ')
            {
                xPos += GetTheDrawSpaceCharWidth(drawFont);
            }
        }
    }
}
