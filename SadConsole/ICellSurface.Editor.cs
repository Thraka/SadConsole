﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using SadRogue.Primitives;
using SadConsole.StringParser;
using SadConsole.Effects;

namespace SadConsole
{
    /// <summary>
    /// Methods to interact with a <see cref="ICellSurface"/>.
    /// </summary>
    public static class CellSurfaceEditor
    {
        /// <summary>
        /// Sets each background of a cell to the array of colors. Must be the same length as this cell surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="pixels">The colors to place.</param>
        public static void SetPixels(this ICellSurface surface, Color[] pixels)
        {
            if (pixels.Length != surface.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(pixels), "The amount of colors do not match the size of this cell surface.");
            }

            for (int i = 0; i < pixels.Length; i++)
            {
                surface[i].Background = pixels[i];
            }

            surface.IsDirty = true;
        }

        /// <summary>
        /// Sets each background of a cell to the array of colors.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">An area to fill with pixels.</param>
        /// <param name="pixels">Colors for each cell of the surface.</param>
        public static void SetPixels(this ICellSurface surface, Rectangle area, Color[] pixels)
        {
            if (pixels.Length != area.Width * area.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(pixels), "The amount of colors do not match the size of the area.");
            }

            for (int x = area.X; x < area.X + area.Width; x++)
            {
                for (int y = area.Y; y < area.Y + area.Height; y++)
                {
                    int index = y * surface.Width + x;

                    if (surface.IsValidCell(index))
                    {
                        surface[y * surface.Width + x].Background = pixels[(y - area.Y) * area.Width + (x - area.X)];
                    }
                }
            }

            surface.IsDirty = true;
        }

        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidCell(this ICellSurface surface, int x, int y) =>
            x >= 0 && x < surface.Width && y >= 0 && y < surface.Height;

        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <param name="index">If the cell is valid, the index of the cell when found.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidCell(this ICellSurface surface, int x, int y, out int index)
        {
            if (x >= 0 && x < surface.Width && y >= 0 && y < surface.Height)
            {
                index = y * surface.Width + x;
                return true;
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Tests if a cell is valid based on its index.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="index">The index to test.</param>
        /// <returns>A true value indicating the cell index is in this cell surface.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidCell(this ICellSurface surface, int index) =>
            index >= 0 && index < surface.Count;

        /// <summary>
        /// Changes the glyph of a specified cell to a new value.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph of the cell.</param>
        public static void SetGlyph(this ICellSurface surface, int x, int y, int glyph)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return;
            }

            surface[index].Glyph = glyph;
            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph and foreground of a cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        public static void SetGlyph(this ICellSurface surface, int x, int y, int glyph, Color foreground)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return;
            }

            surface[index].Foreground = foreground;
            surface[index].Glyph = glyph;
            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph, foreground, and background of a cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        public static void SetGlyph(this ICellSurface surface, int x, int y, int glyph, Color foreground, Color background)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return;
            }

            surface[index].Background = background;
            surface[index].Foreground = foreground;
            surface[index].Glyph = glyph;
            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph, foreground, background, and mirror of a cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        /// <param name="mirror">Sets how the glyph will be mirrored.</param>
        public static void SetGlyph(this ICellSurface surface, int x, int y, int glyph, Color foreground, Color background, Mirror mirror)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return;
            }

            surface[index].Background = background;
            surface[index].Foreground = foreground;
            surface[index].Glyph = glyph;
            surface[index].Mirror = mirror;

            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph, foreground, background, and mirror of a cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        /// <param name="mirror">Sets how the glyph will be mirrored.</param>
        /// <param name="decorators">Decorators to set on the cell. Will clear existing decorators first.</param>
        public static void SetGlyph(this ICellSurface surface, int x, int y, int glyph, Color foreground, Color background, Mirror mirror, IEnumerable<CellDecorator> decorators)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return;
            }

            surface[index].Background = background;
            surface[index].Foreground = foreground;
            surface[index].Glyph = glyph;
            surface[index].Mirror = mirror;
            surface[index].Decorators = decorators?.ToArray() ?? Array.Empty<CellDecorator>();

            surface.IsDirty = true;
        }

        /// <summary>
        /// Gets the glyph of a specified cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The glyph index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetGlyph(this ICellSurface surface, int x, int y) =>
            surface[y * surface.Width + x].Glyph;

        /// <summary>
        /// Changes the foreground of a specified cell to a new color.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public static void SetForeground(this ICellSurface surface, int x, int y, Color color)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return;
            }

            surface[index].Foreground = color;
            surface.IsDirty = true;
        }

        /// <summary>
        /// Gets the foreground of a specified cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetForeground(this ICellSurface surface, int x, int y) =>
            surface[y * surface.Width + x].Foreground;

        /// <summary>
        /// Changes the background of a cell to the specified color.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public static void SetBackground(this ICellSurface surface, int x, int y, Color color)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return;
            }

            surface[index].Background = color;
            surface.IsDirty = true;
        }

        /// <summary>
        /// Gets the background of a specified cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetBackground(this ICellSurface surface, int x, int y) =>
            surface[y * surface.Width + x].Background;

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public static void SetEffect(this ICellSurface surface, int x, int y, ICellEffect effect)
        {
            if (!surface.IsValidCell(x, y, out int index))
                return;

            surface.Effects.SetEffect(index, effect);
            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="index">Index of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public static void SetEffect(this ICellSurface surface, int index, ICellEffect effect)
        {
            if (!surface.IsValidCell(index))
                return;

            surface.Effects.SetEffect(index, effect);
            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a list of cells to the specified effect.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="cells">The cells for the effect.</param>
        /// <param name="effect">The desired effect.</param>
        public static void SetEffect(this ICellSurface surface, IEnumerable<Point> cells, ICellEffect effect)
        {
            var cellList = new List<int>(5);

            foreach (Point item in cells)
                cellList.Add(item.ToIndex(surface.Width));

            surface.Effects.SetEffect(cellList, effect);
            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a list of cells to the specified effect.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="cells">The cells for the effect.</param>
        /// <param name="effect">The desired effect.</param>
        public static void SetEffect(this ICellSurface surface, IEnumerable<int> cells, ICellEffect effect)
        {
            surface.Effects.SetEffect(cells, effect);
            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="cell">The cells for the effect.</param>
        /// <param name="effect">The desired effect.</param>
        public static void SetEffect(this ICellSurface surface, ColoredGlyph cell, ICellEffect effect)
        {
            int index = surface.ToList().IndexOf(cell);

            if (index == -1)
                throw new ArgumentOutOfRangeException(nameof(cell), "Cell doesn't exist in surface.");

            surface.Effects.SetEffect(index, effect);
            surface.IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="cells">The cells for the effect.</param>
        /// <param name="effect">The desired effect.</param>
        public static void SetEffect(this ICellSurface surface, IEnumerable<ColoredGlyph> cells, ICellEffect effect)
        {
            int counter = 0;
            var allCells = surface.ToList();
            List<int> cellIndecies = new List<int>(5);

            foreach (ColoredGlyph item in cells)
            {
                int index = allCells.IndexOf(item);

                if (index == -1)
                    throw new ArgumentOutOfRangeException(nameof(cells), $"Cell doesn't exist in surface, counter was {counter}");

                cellIndecies.Add(index);
                counter++;
            }

            surface.Effects.SetEffect(cellIndecies, effect);
            surface.IsDirty = true;
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The effect.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICellEffect GetEffect(this ICellSurface surface, int x, int y) =>
            surface.Effects.GetEffect(Point.ToIndex(x, y, surface.Width));

        /// <summary>
        /// Changes the appearance of the cell. The appearance represents the look of a cell and will first be cloned, then applied to the cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="appearance">The desired appearance of the cell. A null value cannot be passed.</param>
        public static void SetCellAppearance(this ICellSurface surface, int x, int y, ColoredGlyph appearance)
        {
            if (appearance == null)
                throw new NullReferenceException("Appearance may not be null.");

            if (!surface.IsValidCell(x, y, out int index))
                return;

            appearance.CopyAppearanceTo(surface[index]);
            surface.IsDirty = true;
        }

        /// <summary>
        /// Gets the appearance of a cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The appearance.</returns>
        public static ColoredGlyph GetCellAppearance(this ICellSurface surface, int x, int y)
        {
            var appearance = new ColoredGlyph();
            surface[y * surface.Width + x].CopyAppearanceTo(appearance);
            return appearance;
        }

        /// <summary>
        /// Gets an enumerable of cells over a specific area.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">The area to get cells from.</param>
        /// <returns>A new array with references to each cell in the area.</returns>
        public static IEnumerable<ColoredGlyph> GetCells(this ICellSurface surface, Rectangle area)
        {
            area = Rectangle.GetIntersection(area, new Rectangle(0, 0, surface.Width, surface.Height));

            if (area == Rectangle.Empty)
                yield break;

            for (int y = 0; y < area.Height; y++)
                for (int x = 0; x < area.Width; x++)
                    yield return surface[(y + area.Y) * surface.Width + (x + area.X)];
        }

        /// <summary>
        /// Returns a new surface with reference to each cell inside of the <paramref name="view"/>.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="view">An area of the surface to create a view of.</param>
        /// <returns>A new surface</returns>
        public static ICellSurface GetSubSurface(this ICellSurface surface, Rectangle view)
        {
            if (!new Rectangle(0, 0, surface.Width, surface.Height).Contains(view))
                throw new Exception("View is outside of surface bounds.");

            var cells = new ColoredGlyph[view.Width * view.Height];

            int index = 0;

            for (int y = 0; y < view.Height; y++)
            {
                for (int x = 0; x < view.Width; x++)
                {
                    cells[index] = surface[x + view.X, y + view.Y];
                    index++;
                }
            }

            return new CellSurface(view.Width, view.Height, cells);
        }

        /// <summary>
        /// Gets the mirror of a specified cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mirror GetMirror(this ICellSurface surface, int x, int y) =>
            surface[y * surface.Width + x].Mirror;

        /// <summary>
        /// Sets the mirror of a specified cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="mirror">The mirror of the cell.</param>
        public static void SetMirror(this ICellSurface surface, int x, int y, Mirror mirror)
        {
            if (!surface.IsValidCell(x, y, out int index)) return;

            surface[index].Mirror = mirror;
            surface.IsDirty = true;
        }

        /// <summary>
        /// Sets the decorator of one or more surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x coordinate of the cell.</param>
        /// <param name="y">The y coordinate of the cell.</param>
        /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
        /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDecorator(this ICellSurface surface, int x, int y, int count, params CellDecorator[] decorators) =>
            SetDecorator(surface, Point.ToIndex(x, y, surface.Width), count, decorators);

        /// <summary>
        /// Sets the decorator of one or more surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="index">The index of the cell to start applying.</param>
        /// <param name="count">The count of cells to use from the index (inclusive).</param>
        /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
        public static void SetDecorator(this ICellSurface surface, int index, int count, params CellDecorator[] decorators)
        {
            if (count <= 0) return;
            if (!surface.IsValidCell(index)) return;

            if (index + count > surface.Count)
                count = surface.Count - index;

            if (decorators != null && decorators.Length == 0)
                decorators = null;

            for (int i = index; i < index + count; i++)
            {
                if (decorators == null)
                    surface[i].Decorators = Array.Empty<CellDecorator>();
                else
                    surface[i].Decorators = (CellDecorator[])decorators.Clone();
            }

            surface.IsDirty = true;
        }

        /// <summary>
        /// Appends the decorators to one or more cells
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x coordinate of the cell.</param>
        /// <param name="y">The y coordinate of the cell.</param>
        /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
        /// <param name="decorators">The decorators. Use <code>null</code> to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDecorator(this ICellSurface surface, int x, int y, int count, params CellDecorator[] decorators) =>
            AddDecorator(surface, Point.ToIndex(x, y, surface.Width), count, decorators);

        /// <summary>
        /// Appends the decorators to one or more cells
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="index">The index of the cell to start applying.</param>
        /// <param name="count">The count of cells to use from the index (inclusive).</param>
        /// <param name="decorators">The decorators. If <code>null</code>, does nothing.</param>
        public static void AddDecorator(this ICellSurface surface, int index, int count, params CellDecorator[] decorators)
        {
            if (count <= 0) return;
            if (!surface.IsValidCell(index)) return;
            if (decorators == null || decorators.Length == 0) return;

            if (index + count > surface.Count)
                count = surface.Count - index;

            for (int i = index; i < index + count; i++)
                surface[i].Decorators = surface[i].Decorators.Union(decorators).Distinct().ToArray();

            surface.IsDirty = true;
        }

        /// <summary>
        /// Clears the decorators of the specified surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x coordinate of the cell.</param>
        /// <param name="y">The y coordinate of the cell.</param>
        /// <param name="count">The count of cells to use from the x,y coordinate (inclusive).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearDecorators(this ICellSurface surface, int x, int y, int count) =>
            SetDecorator(surface, x, y, count, null);

        /// <summary>
        /// Clears the decorators of the specified surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="index">The index of the cell to start applying.</param>
        /// <param name="count">The count of cells to use from the index (inclusive).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearDecorators(this ICellSurface surface, int index, int count) =>
            SetDecorator(surface, index, count, null);

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        public static void Print(this ICellSurface surface, int x, int y, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!surface.IsValidCell(x, y, out int index)) return;

            if (!surface.UsePrintProcessor)
            {
                int end = index + text.Length > surface.Count ? surface.Count : index + text.Length;
                int charIndex = 0;
                for (; index < end; index++)
                {
                    surface[index].Glyph = text[charIndex];
                    charIndex++;
                }
            }
            else
                PrintNoCheck(surface, index, ColoredString.Parse(text, index, surface));

            surface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location and color, wrapping if needed.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        public static void Print(this ICellSurface surface, int x, int y, string text, Color foreground)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!surface.IsValidCell(x, y, out int index)) return;

            if (!surface.UsePrintProcessor)
            {
                int end = index + text.Length > surface.Count ? surface.Count : index + text.Length;
                int charIndex = 0;
                for (; index < end; index++)
                {
                    surface[index].Glyph = text[charIndex];
                    surface[index].Foreground = foreground;
                    charIndex++;
                }
            }
            else
            {
                var behavior = new ParseCommandRecolor { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground };
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(behavior);
                PrintNoCheck(surface, index, ColoredString.Parse(text, index, surface, stacks));
            }
            surface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified foreground and background color, wrapping if needed.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        /// <param name="background">Sets the background of all characters in the text.</param>
        public static void Print(this ICellSurface surface, int x, int y, string text, Color foreground, Color background)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!surface.IsValidCell(x, y, out int index)) return;

            if (!surface.UsePrintProcessor)
            {
                int end = index + text.Length > surface.Count ? surface.Count : index + text.Length;
                int charIndex = 0;
                for (; index < end; index++)
                {
                    surface[index].Glyph = text[charIndex];
                    surface[index].Background = background;
                    surface[index].Foreground = foreground;
                    charIndex++;
                }
            }
            else
            {
                var behaviorFore = new ParseCommandRecolor { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground };
                var behaviorBack = new ParseCommandRecolor { R = background.R, G = background.G, B = background.B, A = background.A, CommandType = CommandTypes.Background };
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(behaviorFore);
                stacks.AddSafe(behaviorBack);
                PrintNoCheck(surface, index, ColoredString.Parse(text, index, surface, stacks));
            }
            surface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified settings. 
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        /// <param name="background">Sets the background of all characters in the text.</param>
        /// <param name="mirror">The mirror to set on all characters in the text.</param>
        public static void Print(this ICellSurface surface, int x, int y, string text, Color foreground, Color background, Mirror mirror)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!surface.IsValidCell(x, y, out int index)) return;

            if (!surface.UsePrintProcessor)
            {
                int end = index + text.Length > surface.Count ? surface.Count : index + text.Length;
                int charIndex = 0;
                for (; index < end; index++)
                {
                    surface[index].Glyph = text[charIndex];

                    surface[index].Background = background;
                    surface[index].Foreground = foreground;
                    surface[index].Mirror = mirror;

                    charIndex++;
                }
            }
            else
            {
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(new ParseCommandRecolor { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground });
                stacks.AddSafe(new ParseCommandRecolor { R = background.R, G = background.G, B = background.B, A = background.A, CommandType = CommandTypes.Background });
                stacks.AddSafe(new ParseCommandMirror { Mirror = mirror, CommandType = CommandTypes.Mirror });

                PrintNoCheck(surface, index, ColoredString.Parse(text, index, surface, stacks));
            }
            surface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified settings. 
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="mirror">The mirror to set on all characters in the text.</param>
        public static void Print(this ICellSurface surface, int x, int y, string text, Mirror mirror)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!surface.IsValidCell(x, y, out int index)) return;

            if (!surface.UsePrintProcessor)
            {
                int total = index + text.Length > surface.Count ? surface.Count : index + text.Length;
                int charIndex = 0;
                for (; index < total; index++)
                {
                    surface[index].Glyph = text[charIndex];
                    surface[index].Mirror = mirror;

                    charIndex++;
                }
            }
            else
            {
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(new ParseCommandMirror { Mirror = mirror, CommandType = CommandTypes.Mirror });

                PrintNoCheck(surface, index, ColoredString.Parse(text, index, surface, stacks));
            }
            surface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="appearance">The appearance of the cell</param>
        /// <param name="effect">An optional effect to apply to the printed surface.</param>
        public static void Print(this ICellSurface surface, int x, int y, string text, ColoredGlyph appearance, ICellEffect effect = null)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (!surface.IsValidCell(x, y, out int index)) return;

            int end = index + text.Length > surface.Count ? surface.Count : index + text.Length;
            int charIndex = 0;

            List<int> effectIndicies = new List<int>(text.Length);

            for (; index < end; index++)
            {
                ColoredGlyph cell = surface[index];
                appearance.CopyAppearanceTo(cell);
                cell.Glyph = text[charIndex];
                effectIndicies.Add(index);
                charIndex++;
            }

            surface.Effects.SetEffect(effectIndicies, effect);

            surface.IsDirty = true;
        }

        /// <summary>
        /// Draws a single glyph on the console at the specified location.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="glyph">The glyph to display.</param>
        public static void Print(this ICellSurface surface, int x, int y, ColoredGlyph glyph)
        {
            if (glyph == null) return;
            if (!surface.IsValidCell(x, y, out int index)) return;

            ColoredGlyph cell = surface[index];
            glyph.CopyAppearanceTo(cell);
            cell.Glyph = glyph.Glyph;
            surface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        public static void Print(this ICellSurface surface, int x, int y, ColoredString text)
        {
            if (!surface.IsValidCell(x, y, out int index)) return;

            PrintNoCheck(surface, index, text);
            surface.IsDirty = true;
        }


        private static void PrintNoCheck(this ICellSurface surface, int index, ColoredString text)
        {
            int end = index + text.Length > surface.Count ? surface.Count : index + text.Length;
            int charIndex = 0;

            for (; index < end; index++)
            {
                if (!text.IgnoreGlyph)
                    surface[index].Glyph = text[charIndex].GlyphCharacter;

                if (!text.IgnoreBackground)
                    surface[index].Background = text[charIndex].Background;

                if (!text.IgnoreForeground)
                    surface[index].Foreground = text[charIndex].Foreground;

                if (!text.IgnoreMirror)
                    surface[index].Mirror = text[charIndex].Mirror;

                if (!text.IgnoreEffect)
                    SetEffect(surface, index, text[charIndex].Effect);

                charIndex++;
            }
            surface.IsDirty = true;
        }

        /// <summary>
        /// Builds a string from the text surface from the specified coordinates.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x position of the surface to start at.</param>
        /// <param name="y">The y position of the surface to start at.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(this ICellSurface surface, int x, int y, int length) =>
            GetString(surface, y * surface.Width + x, length);

        /// <summary>
        /// Builds a string from the text surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="index">Where to start getting characters from.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public static string GetString(this ICellSurface surface, int index, int length)
        {
            if (index >= 0 && index < surface.Count)
            {
                var sb = new StringBuilder(length);
                for (int i = 0; i < length; i++)
                {
                    int tempIndex = i + index;

                    if (tempIndex < surface.Count)
                    {
                        sb.Append((char)surface[tempIndex].Glyph);
                    }
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Builds a string from the text surface from the specified coordinates.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x position of the surface to start at.</param>
        /// <param name="y">The y position of the surface to start at.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColoredString GetStringColored(this ICellSurface surface, int x, int y, int length) =>
            GetStringColored(surface, y * surface.Width + x, length);

        /// <summary>
        /// Builds a string from the text surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="index">Where to start getting characters from.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public static ColoredString GetStringColored(this ICellSurface surface, int index, int length)
        {
            if (index < 0 || index >= surface.Count)
            {
                return new ColoredString(string.Empty);
            }

            var sb = new ColoredString(length);

            for (int i = 0; i < length; i++)
            {
                int tempIndex = i + index;
                var cell = (ColoredGlyph)sb[i];
                if (tempIndex < surface.Count)
                {
                    surface[tempIndex].CopyAppearanceTo(cell);
                }
            }

            return sb;

        }

        /// <summary>
        /// Resets the shifted amounts to 0, as if the surface has never shifted.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        public static void ClearShiftValues(this ICellSurface surface)
        {
            surface.TimesShiftedDown = 0;
            surface.TimesShiftedUp = 0;
            surface.TimesShiftedLeft = 0;
            surface.TimesShiftedRight = 0;
        }

        public static void ShiftRow(this ICellSurface surface, int row, int startingX, int count, bool wrap)
        {
            if (count == 0) return;
            if (startingX < 0 || startingX >= surface.Width) throw new ArgumentOutOfRangeException(nameof(startingX), "Column must be 0 or more and less than the width of the surface.");
            if (row < 0 || row >= surface.Height) throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0 or more and less than the height of the surface.");

            if (count < 0)
                ShiftRowLeftUnchecked(surface, row, startingX, -count, wrap); 
            else
                ShiftRowRightUnchecked(surface, row, startingX, count, wrap);
        }

        public static void ShiftRowRight(this ICellSurface surface, int row, int count, bool wrap)
        {
            if (count == 0) return;
            if (row < 0 || row >= surface.Height) throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0 or more and less than the height of the surface.");

            ShiftRowRightUnchecked(surface, row, 0, count, wrap);
        }

        public static void ShiftRowLeft(this ICellSurface surface, int row, int count, bool wrap)
        {
            if (count == 0) return;
            if (row < 0 || row >= surface.Height) throw new ArgumentOutOfRangeException(nameof(row), "Row must be 0 or more and less than the height of the surface.");

            ShiftRowLeftUnchecked(surface, row, surface.Width - 1, count, wrap);
        }

        public static void ShiftRowRightUnchecked(this ICellSurface surface, int row, int startingX, int count, bool wrap)
        {
            if (wrap)
            {
                // TODO Wrap on ShiftRowRightUnchecked
            }
            else
            {
                if (count > surface.Width - startingX)
                {
                    // Shifting all off the side. Clear everything.
                    for (int x = startingX; x < surface.Width; x++)
                        Clear(surface, x, row, surface.Width - x);
                }
                else
                {
                    int startIndex = new Point(surface.Width - count - 1, row).ToIndex(surface.Width);
                    int copyStopX = new Point(startingX, row).ToIndex(surface.Width);

                    for (int x = startIndex; x >= copyStopX; x--)
                    {
                        surface[x].CopyAppearanceTo(surface[x + count]);
                    }

                    for (int x = 0; x < count; x++)
                    {
                        surface[x + copyStopX].Clear();
                    }
                }
            }
        }

        public static void ShiftRowLeftUnchecked(this ICellSurface surface, int row, int startingX, int count, bool wrap)
        {
            if (wrap)
            {
                // TODO Wrap on ShiftRowLeftUnchecked
            }
            else
            {
                if (count + 1 > startingX)
                {
                    // Shifting all off the side. Clear everything.
                    for (int x = 0; x < startingX; x++)
                        Clear(surface, 0, row, startingX + 1);
                }
                else
                {
                    int startIndex = new Point(count, row).ToIndex(surface.Width);
                    int copyStopX = new Point(startingX, row).ToIndex(surface.Width);

                    for (int x = startIndex; x <= copyStopX; x++)
                        surface[x].CopyAppearanceTo(surface[x - count]);

                    for (int x = 0; x < count; x++)
                        surface[startingX - x].Clear();
                }
            }
        }

        /// <summary>
        /// Scrolls all the console data up by one.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShiftUp(this ICellSurface surface) =>
            ShiftUp(surface, 1);

        /// <summary>
        /// Scrolls all the console data up by the specified amount of rows.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="amount">How many rows to shift.</param>
        /// <param name="wrap">When false, a blank line appears at the bottom. When true, the top line appears at the bottom.</param>
        public static void ShiftUp(this ICellSurface surface, int amount, bool wrap = false)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                ShiftDown(surface, Math.Abs(amount), wrap);
                return;
            }

            surface.TimesShiftedUp += amount;

            List<Tuple<ColoredGlyph, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<ColoredGlyph, int>>(surface.Height * amount);

                for (int y = 0; y < amount; y++)
                {
                    for (int x = 0; x < surface.Width; x++)
                    {
                        var tempCell = new ColoredGlyph();
                        surface[y * surface.Width + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<ColoredGlyph, int>(tempCell, (surface.Height - amount + y) * surface.Width + x));
                    }
                }
            }

            for (int y = amount; y < surface.Height; y++)
            {
                for (int x = 0; x < surface.Width; x++)
                {
                    ColoredGlyph destination = surface[(y - amount) * surface.Width + x];
                    ColoredGlyph source = surface[y * surface.Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                    destination.Decorators = source.Decorators;
                }
            }


            if (!wrap)
            {
                for (int y = surface.Height - amount; y < surface.Height; y++)
                {
                    for (int x = 0; x < surface.Width; x++)
                    {
                        Clear(surface, x, y);
                    }
                }
            }
            else
            {
                foreach (Tuple<ColoredGlyph, int> cellTuple in wrappedCells)
                {
                    ColoredGlyph destination = surface[cellTuple.Item2];

                    destination.Background = cellTuple.Item1.Background;
                    destination.Foreground = cellTuple.Item1.Foreground;
                    destination.Glyph = cellTuple.Item1.Glyph;
                    destination.Mirror = cellTuple.Item1.Mirror;
                    destination.Decorators = cellTuple.Item1.Decorators;
                }
            }

            surface.IsDirty = true;
        }

        /// <summary>
        /// Scrolls all the console data down by one.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShiftDown(this ICellSurface surface) =>
            ShiftDown(surface, 1);

        /// <summary>
        /// Scrolls all the console data down by the specified amount of rows.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="amount">How many rows to shift.</param>
        /// <param name="wrap">When false, a blank line appears at the top. When true, the bottom line appears at the top.</param>
        public static void ShiftDown(this ICellSurface surface, int amount, bool wrap = false)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                ShiftUp(surface, Math.Abs(amount), wrap);
                return;
            }

            surface.TimesShiftedDown += amount;

            List<Tuple<ColoredGlyph, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<ColoredGlyph, int>>(surface.Height * amount);

                for (int y = surface.Height - amount; y < surface.Height; y++)
                {
                    for (int x = 0; x < surface.Width; x++)
                    {
                        var tempCell = new ColoredGlyph();
                        surface[y * surface.Width + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<ColoredGlyph, int>(tempCell, (amount - (surface.Height - y)) * surface.Width + x));
                    }
                }
            }

            for (int y = (surface.Height - 1) - amount; y >= 0; y--)
            {
                for (int x = 0; x < surface.Width; x++)
                {
                    ColoredGlyph destination = surface[(y + amount) * surface.Width + x];
                    ColoredGlyph source = surface[y * surface.Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                    destination.Decorators = source.Decorators;
                }
            }

            if (!wrap)
            {
                for (int y = 0; y < amount; y++)
                {
                    for (int x = 0; x < surface.Width; x++)
                    {
                        ColoredGlyph source = surface[y * surface.Width + x];
                        source.Clear();
                    }
                }
            }
            else
            {
                foreach (Tuple<ColoredGlyph, int> cellTuple in wrappedCells)
                {
                    ColoredGlyph destination = surface[cellTuple.Item2];

                    destination.Background = cellTuple.Item1.Background;
                    destination.Foreground = cellTuple.Item1.Foreground;
                    destination.Glyph = cellTuple.Item1.Glyph;
                    destination.Mirror = cellTuple.Item1.Mirror;
                    destination.Decorators = cellTuple.Item1.Decorators;
                }
            }

            surface.IsDirty = true;
        }

        /// <summary>
        /// Scrolls all the console data right by one.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShiftRight(this ICellSurface surface) =>
            ShiftRight(surface, 1);

        /// <summary>
        /// Scrolls all the console data right by the specified amount.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="amount">How much to scroll.</param>
        /// <param name="wrap">When false, a blank line appears at the left. When true, the right line appears at the left.</param>
        public static void ShiftRight(this ICellSurface surface, int amount, bool wrap = false)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                ShiftLeft(surface, Math.Abs(amount), wrap);
                return;
            }

            surface.TimesShiftedRight += amount;

            List<Tuple<ColoredGlyph, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<ColoredGlyph, int>>(surface.Height * amount);

                for (int x = surface.Width - amount; x < surface.Width; x++)
                {
                    for (int y = 0; y < surface.Height; y++)
                    {
                        var tempCell = new ColoredGlyph();
                        surface[y * surface.Width + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<ColoredGlyph, int>(tempCell, y * surface.Width + amount - (surface.Width - x)));
                    }
                }
            }


            for (int x = surface.Width - 1 - amount; x >= 0; x--)
            {
                for (int y = 0; y < surface.Height; y++)
                {
                    ColoredGlyph destination = surface[y * surface.Width + (x + amount)];
                    ColoredGlyph source = surface[y * surface.Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                    destination.Decorators = source.Decorators;
                }
            }

            if (!wrap)
            {
                for (int x = 0; x < amount; x++)
                {
                    for (int y = 0; y < surface.Height; y++)
                    {
                        Clear(surface, x, y);

                    }
                }
            }
            else
            {
                foreach (Tuple<ColoredGlyph, int> cellTuple in wrappedCells)
                {
                    ColoredGlyph destination = surface[cellTuple.Item2];

                    destination.Background = cellTuple.Item1.Background;
                    destination.Foreground = cellTuple.Item1.Foreground;
                    destination.Glyph = cellTuple.Item1.Glyph;
                    destination.Mirror = cellTuple.Item1.Mirror;
                    destination.Decorators = cellTuple.Item1.Decorators;
                }
            }

            surface.IsDirty = true;
        }

        /// <summary>
        /// Scrolls all the console data left by one.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShiftLeft(this ICellSurface surface) =>
            ShiftLeft(surface, 1);

        /// <summary>
        /// Scrolls all the console data left by the specified amount.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="amount">How much to scroll.</param>
        /// <param name="wrap">When false, a blank line appears at the right. When true, the left line appears at the right.</param>
        public static void ShiftLeft(this ICellSurface surface, int amount, bool wrap = false)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                ShiftRight(surface, Math.Abs(amount), wrap);
                return;
            }

            surface.TimesShiftedLeft += amount;

            List<Tuple<ColoredGlyph, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<ColoredGlyph, int>>(surface.Height * amount);

                for (int x = 0; x < amount; x++)
                {
                    for (int y = 0; y < surface.Height; y++)
                    {
                        var tempCell = new ColoredGlyph();
                        surface[y * surface.Width + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<ColoredGlyph, int>(tempCell, y * surface.Width + (surface.Width - amount + x)));
                    }
                }
            }

            for (int x = amount; x < surface.Width; x++)
            {
                for (int y = 0; y < surface.Height; y++)
                {
                    ColoredGlyph destination = surface[y * surface.Width + (x - amount)];
                    ColoredGlyph source = surface[y * surface.Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                    destination.Decorators = source.Decorators;
                }
            }

            if (!wrap)
            {
                for (int x = surface.Width - amount; x < surface.Width; x++)
                {
                    for (int y = 0; y < surface.Height; y++)
                    {
                        Clear(surface, x, y);
                    }
                }
            }
            else
            {
                foreach (Tuple<ColoredGlyph, int> cellTuple in wrappedCells)
                {
                    ColoredGlyph destination = surface[cellTuple.Item2];

                    destination.Background = cellTuple.Item1.Background;
                    destination.Foreground = cellTuple.Item1.Foreground;
                    destination.Glyph = cellTuple.Item1.Glyph;
                    destination.Mirror = cellTuple.Item1.Mirror;
                    destination.Decorators = cellTuple.Item1.Decorators;
                }
            }

            surface.IsDirty = true;
        }

        /// <summary>
        /// Starting at the specified coordinate, clears the glyph, mirror, and decorators, for the specified count of surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="count">The count of glyphs to erase.</param>
        /// <returns>The cells processed by this method.</returns>
        /// <remarks>
        /// Cells altered by this method has the <see cref="ColoredGlyph.Glyph"/> set to <see cref="ICellSurface.DefaultGlyph"/>, the <see cref="ColoredGlyph.Decorators"/> array reset, and the <see cref="ColoredGlyph.Mirror"/> set to <see cref="Mirror.None"/>.
        /// </remarks>
        public static ColoredGlyph[] Erase(this ICellSurface surface, int x, int y, int count)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return Array.Empty<ColoredGlyph>();
            }

            int end = index + count > surface.Count ? surface.Count - index : index + count;
            int total = end - index;
            ColoredGlyph[] result = new ColoredGlyph[total];
            int resultIndex = 0;
            for (; index < end; index++)
            {
                ColoredGlyph c = surface[index];

                c.Glyph = surface.DefaultGlyph;
                c.Mirror = Mirror.None;
                c.Decorators = Array.Empty<CellDecorator>();

                result[resultIndex] = c;
                resultIndex++;
            }

            surface.IsDirty = true;
            return result;
        }

        /// <summary>
        /// Clears the glyph, mirror, and decorators, for the specified cell.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <remarks>
        /// The cell altered by this method has the <see cref="ColoredGlyph.Glyph"/> set to <see cref="ICellSurface.DefaultGlyph"/>, the <see cref="ColoredGlyph.Decorators"/> array reset, and the <see cref="ColoredGlyph.Mirror"/> set to <see cref="Mirror.None"/>.
        /// </remarks>
        public static void Erase(this ICellSurface surface, int x, int y)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return;
            }

            surface[index].Glyph = surface.DefaultGlyph;
            surface[index].Mirror = Mirror.None;
            surface[index].Decorators = Array.Empty<CellDecorator>();

            surface.IsDirty = true;
        }

        /// <summary>
        /// Erases all cells which clears the glyph, mirror, and decorators.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <remarks>
        /// All cells have <see cref="ColoredGlyph.Glyph"/> set to <see cref="ICellSurface.DefaultGlyph"/>, the <see cref="ColoredGlyph.Decorators"/> array reset, and the <see cref="ColoredGlyph.Mirror"/> set to <see cref="Mirror.None"/>.
        /// </remarks>
        public static void Erase(this ICellSurface surface)
        {
            for (int i = 0; i < surface.Count; i++)
            {
                surface[i].Glyph = surface.DefaultGlyph;
                surface[i].Mirror = Mirror.None;
                surface[i].Decorators = Array.Empty<CellDecorator>();
            }

            surface.IsDirty = true;
        }


        /// <summary>
        /// Clears the console data. Characters are reset to 0, the foreground and background are set to default, and mirror set to none. Clears cell decorators.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(this ICellSurface surface) =>
            Fill(surface, surface.DefaultForeground, surface.DefaultBackground, surface.DefaultGlyph, Mirror.None);

        /// <summary>
        /// Clears a cell. Character is reset to 0, the foreground and background is set to default, and mirror is set to none. Clears cell decorators.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        public static void Clear(this ICellSurface surface, int x, int y)
        {
            if (!surface.IsValidCell(x, y, out int index))
                return;

            surface.Effects.SetEffect(index, null);

            ColoredGlyph cell = surface[index];
            cell.Clear();
            cell.Foreground = surface.DefaultForeground;
            cell.Background = surface.DefaultBackground;
            cell.Glyph = surface.DefaultGlyph;
            surface.IsDirty = true;
        }

        /// <summary>
        /// Clears a segment of cells, starting from the left, extending to the right, and wrapping if needed. Character is reset to 0, the foreground and background is set to default, and mirror is set to none. Clears cell decorators.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x position of the left end of the segment.</param>
        /// <param name="y">The y position of the segment.</param>
        /// <param name="length">The length of the segment. If it extends beyond the line, it will wrap to the next line. If it extends beyond the console, then it automatically ends at the last valid cell.</param>
        /// <remarks>This works similarly to printing a string of whitespace</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(this ICellSurface surface, int x, int y, int length)
        {
            ColoredGlyph[] cells = Fill(surface, x, y, length, surface.DefaultForeground, surface.DefaultBackground, surface.DefaultGlyph, Mirror.None);
        }

        /// <summary>
        /// Clears an area of surface. Character is reset to 0, the foreground and background is set to default, and mirror is set to none. Clears cell decorators.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">The area to clear.</param>
        public static void Clear(this ICellSurface surface, Rectangle area) =>
            Fill(surface, area, surface.DefaultForeground, surface.DefaultBackground, surface.DefaultGlyph, Mirror.None);

        /// <summary>
        /// Fills the console. Clears cell decorators.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="foreground">Foreground to apply. If null, skips.</param>
        /// <param name="background">Foreground to apply. If null, skips.</param>
        /// <param name="glyph">Glyph to apply. If null, skips.</param>
        /// <param name="mirror">Mirror to apply. If null, skips.</param>
        /// <returns>The array of all cells in this console, starting from the top left corner.</returns>
        public static ColoredGlyph[] Fill(this ICellSurface surface, Color? foreground = null, Color? background = null, int? glyph = null, Mirror? mirror = null)
        {
            ColoredGlyph[] glyphs = new ColoredGlyph[surface.Count];

            for (int i = 0; i < surface.Count; i++)
            {
                if (background.HasValue)
                    surface[i].Background = background.Value;

                if (foreground.HasValue)
                    surface[i].Foreground = foreground.Value;

                if (glyph.HasValue)
                    surface[i].Glyph = glyph.Value;

                if (mirror.HasValue)
                    surface[i].Mirror = mirror.Value;

                surface[i].Decorators = Array.Empty<CellDecorator>();

                glyphs[i] = surface[i];
            }

            surface.IsDirty = true;

            return glyphs;
        }

        /// <summary>
        /// Fills a segment of cells, starting from the left, extending to the right, and wrapping if needed. Clears cell decorators.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x position of the left end of the segment. </param>
        /// <param name="y">The y position of the segment.</param>
        /// <param name="length">The length of the segment. If it extends beyond the line, it will wrap to the next line. If it extends beyond the console, then it automatically ends at the last valid cell.</param>
        /// <param name="foreground">Foreground to apply. If null, skips.</param>
        /// <param name="background">Background to apply. If null, skips.</param>
        /// <param name="glyph">Glyph to apply. If null, skips.</param>
        /// <param name="mirror">Mirror to apply. If null, skips.</param>
        /// <returns>An array containing the affected cells, starting from the top left corner. If x or y are out of bounds, nothing happens and an empty array is returned</returns>
        public static ColoredGlyph[] Fill(this ICellSurface surface, int x, int y, int length, Color? foreground = null, Color? background = null, int? glyph = null, Mirror? mirror = null)
        {
            if (!surface.IsValidCell(x, y, out int index))
            {
                return Array.Empty<ColoredGlyph>();
            }

            int end = index + length > surface.Count ? surface.Count - index : index + length;
            int total = end - index;
            ColoredGlyph[] result = new ColoredGlyph[total];
            int resultIndex = 0;
            for (; index < end; index++)
            {
                ColoredGlyph c = surface[index];
                if (background.HasValue)
                    c.Background = background.Value;

                if (foreground.HasValue)
                    c.Foreground = foreground.Value;

                if (glyph.HasValue)
                    c.Glyph = glyph.Value;

                if (mirror.HasValue)
                    c.Mirror = mirror.Value;

                c.Decorators = Array.Empty<CellDecorator>();

                result[resultIndex] = c;
                resultIndex++;
            }


            surface.IsDirty = true;
            return result;
        }

        /// <summary>
        /// Fills the specified area. Clears cell decorators.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">The area to fill.</param>
        /// <param name="foreground">Foreground to apply. If null, skips.</param>
        /// <param name="background">Background to apply. If null, skips.</param>
        /// <param name="glyph">Glyph to apply. If null, skips.</param>
        /// <param name="mirror">Mirror to apply. If null, skips.</param>
        /// <returns>An array containing the affected cells, starting from the top left corner. If the area is out of bounds, nothing happens and an empty array is returned.</returns>
        public static ColoredGlyph[] Fill(this ICellSurface surface, Rectangle area, Color? foreground = null, Color? background = null, int? glyph = null, Mirror? mirror = null)
        {
            area = Rectangle.GetIntersection(area, new Rectangle(0, 0, surface.Width, surface.Height));

            if (area == Rectangle.Empty)
                return new ColoredGlyph[0];

            var result = new ColoredGlyph[area.Width * area.Height];
            int resultIndex = 0;

            for (int x = area.X; x < area.X + area.Width; x++)
            {
                for (int y = area.Y; y < area.Y + area.Height; y++)
                {
                    ColoredGlyph cell = surface[y * surface.Width + x];

                    if (background.HasValue)
                        cell.Background = background.Value;

                    if (foreground.HasValue)
                        cell.Foreground = foreground.Value;

                    if (glyph.HasValue)
                        cell.Glyph = glyph.Value;

                    if (mirror.HasValue)
                        cell.Mirror = mirror.Value;

                    cell.Decorators = Array.Empty<CellDecorator>();

                    result[resultIndex] = cell;
                    resultIndex++;
                }
            }

            surface.IsDirty = true;
            return result;
        }

        /// <summary>
        /// Draws a line from <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="start">Starting point of the line.</param>
        /// <param name="end">Ending point of the line.</param>
        /// <param name="foreground">Foreground to set. If null, skipped.</param>
        /// <param name="background">Background to set. If null, skipped.</param>
        /// <param name="glyph">Glyph to set. If null, skipped.</param>
        /// <param name="mirror">Mirror to set. If null, skipped.</param>
        /// <returns>A list of cells the line touched; ordered from first to last.</returns>
        /// <remarks>To simply return the list of cells that would be drawn to, use <see langword="null"/> for <paramref name="glyph"/>, <paramref name="foreground"/>, <paramref name="background"/>, and <paramref name="mirror"/>.</remarks>
        public static IEnumerable<ColoredGlyph> DrawLine(this ICellSurface surface, Point start, Point end, int? glyph, Color? foreground = null, Color? background = null, Mirror? mirror = null)
        {
            var result = new List<ColoredGlyph>();
            Func<int, int, bool> processor;

            if (foreground.HasValue || background.HasValue || glyph.HasValue)
            {
                processor = (x, y) =>
                {
                    if (surface.IsValidCell(x, y, out int index))
                    {
                        ColoredGlyph cell = surface[index];
                        result.Add(cell);

                        if (foreground.HasValue)
                        {
                            cell.Foreground = foreground.Value;
                            surface.IsDirty = true;
                        }
                        if (background.HasValue)
                        {
                            cell.Background = background.Value;
                            surface.IsDirty = true;
                        }
                        if (glyph.HasValue)
                        {
                            cell.Glyph = glyph.Value;
                            surface.IsDirty = true;
                        }

                        if (mirror.HasValue)
                        {
                            cell.Mirror = mirror.Value;
                            surface.IsDirty = true;
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
                    if (surface.IsValidCell(x, y, out int index))
                    {
                        result.Add(surface[index]);
                        return true;
                    }

                    return false;
                };
            }

            Algorithms.Line(start.X, start.Y, end.X, end.Y, processor);

            return result;
        }

        /// <summary>
        /// Draws a box.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">The area of the box.</param>
        /// <param name="parameters">Provides the options for drawing a border and filling the box.</param>
        public static void DrawBox(this ICellSurface surface, Rectangle area, ShapeParameters parameters)
        {
            Rectangle fillRect = area.Expand(-1, -1);

            if (parameters.HasFill && parameters.FillGlyph != null)
            {
                surface.Fill(fillRect,
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

                    ColoredGlyph border = parameters.BorderGlyph;

                    // Draw the major sides
                    DrawLine(surface, area.Position, area.Position + new Point(area.Width - 1, 0), connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Top],
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    DrawLine(surface, area.Position + new Point(0, area.Height - 1), area.Position + new Point(area.Width - 1, area.Height - 1), connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Bottom],
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    DrawLine(surface, area.Position, area.Position + new Point(0, area.Height - 1), connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Left],
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    DrawLine(surface, area.Position + new Point(area.Width - 1, 0), area.Position + new Point(area.Width - 1, area.Height - 1), connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Right],
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);

                    // Tweak the corners
                    surface.SetGlyph(area.X, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopLeft]);
                    surface.SetGlyph(area.MaxExtentX, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopRight]);
                    surface.SetGlyph(area.X, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomLeft]);
                    surface.SetGlyph(area.MaxExtentX, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomRight]);
                }
                // Using full glyph line style
                else if (parameters.BoxBorderStyleGlyphs != null)
                {
                    ColoredGlyph[] connectedLineStyle = parameters.BoxBorderStyleGlyphs;

                    if (!ICellSurface.ValidateLineStyle(connectedLineStyle))
                        throw new ArgumentException("Array is either null or does not have the required line style elements", nameof(connectedLineStyle));

                    // Draw the major sides
                    ColoredGlyph border = connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Top];
                    DrawLine(surface, area.Position, area.Position + new Point(area.Width - 1, 0), border.Glyph,
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    border = connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Bottom];
                    DrawLine(surface, area.Position + new Point(0, area.Height - 1), area.Position + new Point(area.Width - 1, area.Height - 1), border.Glyph,
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    border = connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Left];
                    DrawLine(surface, area.Position, area.Position + new Point(0, area.Height - 1), border.Glyph,
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    border = connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Right];
                    DrawLine(surface, area.Position + new Point(area.Width - 1, 0), area.Position + new Point(area.Width - 1, area.Height - 1), border.Glyph,
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);

                    // Tweak the corners
                    surface.SetGlyph(area.X, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopLeft].Glyph);
                    surface.SetGlyph(area.MaxExtentX, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopRight].Glyph);
                    surface.SetGlyph(area.X, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomLeft].Glyph);
                    surface.SetGlyph(area.MaxExtentX, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomRight].Glyph);
                }
                // Using a single glyph
                else
                {
                    // Draw the major sides
                    ColoredGlyph border = parameters.BorderGlyph;
                    DrawLine(surface, area.Position, area.Position + new Point(area.Width - 1, 0), border.Glyph,
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    DrawLine(surface, area.Position + new Point(0, area.Height - 1), area.Position + new Point(area.Width - 1, area.Height - 1), border.Glyph,
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    DrawLine(surface, area.Position, area.Position + new Point(0, area.Height - 1), border.Glyph,
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                    DrawLine(surface, area.Position + new Point(area.Width - 1, 0), area.Position + new Point(area.Width - 1, area.Height - 1), border.Glyph,
                                parameters.IgnoreBorderForeground ? (Color?)null : border.Foreground,
                                parameters.IgnoreBorderBackground ? (Color?)null : border.Background,
                                parameters.IgnoreBorderMirror ? (Mirror?)null : border.Mirror);
                }
            }
        }

        /// <summary>
        /// Draws a box.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">The area of the box.</param>
        /// <param name="border">The border style.</param>
        /// <param name="fill">The fill style. If null, the box is not filled.</param>
        /// <param name="connectedLineStyle">The lien style of the border. If null, <paramref name="border"/> glyph is used.</param>
        [Obsolete("Use the other DrawBox method")]
        public static void DrawBox(this ICellSurface surface, Rectangle area, ColoredGlyph border, ColoredGlyph fill = null, int[] connectedLineStyle = null)
        {
            if (connectedLineStyle == null)
            {
                connectedLineStyle = Enumerable.Range(0, Enum.GetValues(typeof(ICellSurface.ConnectedLineIndex)).Length)
                    .Select(_ => border.Glyph).ToArray();
            }

            if (!ICellSurface.ValidateLineStyle(connectedLineStyle))
            {
                throw new ArgumentException("Array is either null or does not have the required line style elements", nameof(connectedLineStyle));
            }

            // Draw the major sides
            DrawLine(surface, area.Position, area.Position + new Point(area.Width - 1, 0), connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Top], border.Foreground, border.Background, border.Mirror);
            DrawLine(surface, area.Position + new Point(0, area.Height - 1), area.Position + new Point(area.Width - 1, area.Height - 1), connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Bottom], border.Foreground, border.Background, border.Mirror);
            DrawLine(surface, area.Position, area.Position + new Point(0, area.Height - 1), connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Left], border.Foreground, border.Background, border.Mirror);
            DrawLine(surface, area.Position + new Point(area.Width - 1, 0), area.Position + new Point(area.Width - 1, area.Height - 1), connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.Right], border.Foreground, border.Background, border.Mirror);

            // Tweak the corners
            surface.SetGlyph(area.X, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopLeft]);
            surface.SetGlyph(area.MaxExtentX, area.Y, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.TopRight]);
            surface.SetGlyph(area.X, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomLeft]);
            surface.SetGlyph(area.MaxExtentX, area.MaxExtentY, connectedLineStyle[(int)ICellSurface.ConnectedLineIndex.BottomRight]);

            // Fill
            if (fill == null)
            {
                return;
            }

            area = area.Expand(-1, -1);
            Fill(surface, area, fill.Foreground, fill.Background, fill.Glyph, fill.Mirror);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">The area the ellipse </param>
        /// <param name="parameters">Provides the options for drawing a border and filling the circle.</param>
        public static void DrawCircle(this ICellSurface surface, Rectangle area, ShapeParameters parameters)
        {
            var cells = new List<int>(area.Width * area.Height);

            Algorithms.Ellipse(area.X, area.Y, area.MaxExtentX, area.MaxExtentY, (x, y) =>
            {
                if (parameters.HasBorder && surface.IsValidCell(x, y))
                {
                    ColoredGlyph cell = surface[x, y];

                    if (!parameters.IgnoreBorderForeground) cell.Foreground = parameters.BorderGlyph.Foreground;
                    if (!parameters.IgnoreBorderBackground) cell.Background = parameters.BorderGlyph.Background;
                    if (!parameters.IgnoreBorderGlyph) cell.Glyph = parameters.BorderGlyph.Glyph;
                    if (!parameters.IgnoreBorderMirror) cell.Mirror = parameters.BorderGlyph.Mirror;
                }

                cells.Add(Point.ToIndex(x, y, surface.Width));
            });

            if (parameters.HasFill && parameters.FillGlyph != null)
            {
                Func<int, bool> isTargetCell = c => !cells.Contains(c);
                Action<int> fillCell = c =>
                {
                    if (surface.IsValidCell(c))
                    {
                        ColoredGlyph cell = surface[c];

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

                    (int x, int y) = Point.FromIndex(c, surface.Width);

                    if (IsValidCell(surface, x - 1, y))
                    {
                        connections.West = Point.ToIndex(x - 1, y, surface.Width);
                        connections.HasWest = true;
                    }
                    if (IsValidCell(surface, x + 1, y))
                    {
                        connections.East = Point.ToIndex(x + 1, y, surface.Width);
                        connections.HasEast = true;
                    }
                    if (IsValidCell(surface, x, y - 1))
                    {
                        connections.North = Point.ToIndex(x, y - 1, surface.Width);
                        connections.HasNorth = true;
                    }
                    if (IsValidCell(surface, x, y + 1))
                    {
                        connections.South = Point.ToIndex(x, y + 1, surface.Width);
                        connections.HasSouth = true;
                    }

                    return connections;
                };

                Algorithms.FloodFill(area.Center.ToIndex(surface.Width), isTargetCell, fillCell, getConnectedCells);
            }
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">The area the ellipse </param>
        /// <param name="outer">The appearance of the outer line of the ellipse.</param>
        /// <param name="inner">The appearance of the inside of hte ellipse. If null, it will not be filled.</param>
        [Obsolete("Use the other DrawCircle method")]
        public static void DrawCircle(this ICellSurface surface, Rectangle area, ColoredGlyph outer, ColoredGlyph inner = null)
        {
            var cells = new List<int>(area.Width * area.Height);

            Algorithms.Ellipse(area.X, area.Y, area.MaxExtentX, area.MaxExtentY, (x, y) =>
            {
                if (surface.IsValidCell(x, y))
                {
                    SetCellAppearance(surface, x, y, outer);
                    cells.Add(Point.ToIndex(x, y, surface.Width));
                }
            });

            if (inner != null)
            {
                Func<int, bool> isTargetCell = c => !cells.Contains(c);
                Action<int> fillCell = c =>
                {
                    inner.CopyAppearanceTo(surface[c]);
                    cells.Add(c);
                };
                Func<int, Algorithms.NodeConnections<int>> getConnectedCells = c =>
                {
                    var connections = new Algorithms.NodeConnections<int>();

                    (int x, int y) = Point.FromIndex(c, surface.Width);

                    if (IsValidCell(surface, x - 1, y))
                    {
                        connections.West = Point.ToIndex(x - 1, y, surface.Width);
                        connections.HasWest = true;
                    }
                    if (IsValidCell(surface, x + 1, y))
                    {
                        connections.East = Point.ToIndex(x + 1, y, surface.Width);
                        connections.HasEast = true;
                    }
                    if (IsValidCell(surface, x, y - 1))
                    {
                        connections.North = Point.ToIndex(x, y - 1, surface.Width);
                        connections.HasNorth = true;
                    }
                    if (IsValidCell(surface, x, y + 1))
                    {
                        connections.South = Point.ToIndex(x, y + 1, surface.Width);
                        connections.HasSouth = true;
                    }

                    return connections;
                };

                Algorithms.FloodFill(area.Center.ToIndex(surface.Width), isTargetCell, fillCell, getConnectedCells);
            }
        }

        /// <summary>
        /// Connects all lines in a surface for both <see cref="ICellSurface.ConnectedLineThin"/> and <see cref="ICellSurface.ConnectedLineThick"/> styles.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConnectLines(this ICellSurface surface)
        {
            ConnectLines(surface, ICellSurface.ConnectedLineThin);
            ConnectLines(surface, ICellSurface.ConnectedLineThick);
        }

        /// <summary>
        /// Connects all lines in this based on the <paramref name="lineStyle"/> style provided.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="lineStyle">The array of line styles indexed by <see cref="ICellSurface.ConnectedLineIndex"/>.</param>
        public static void ConnectLines(this ICellSurface surface, int[] lineStyle) =>
            ConnectLines(surface, lineStyle, surface.Area);

        /// <summary>
        /// Connects all lines in this based on the <paramref name="lineStyle"/> style provided.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="lineStyle">The array of line styles indexed by <see cref="ICellSurface.ConnectedLineIndex"/>.</param>
        /// <param name="area">The area to process.</param>
        public static void ConnectLines(this ICellSurface surface, int[] lineStyle, Rectangle area)
        {
            for (int x = area.X; x <= area.MaxExtentX; x++)
            {
                for (int y = area.Y; y <= area.MaxExtentY; y++)
                {
                    var pos = new Point(x, y);
                    int index = pos.ToIndex(surface.Width);

                    // Check if this pos is a road
                    if (!lineStyle.Contains(surface[index].Glyph))
                        continue;

                    // Get all valid positions and indexes around this point
                    bool[] valids = pos.GetValidDirections(area);
                    int[] posIndexes = pos.GetDirectionIndexes(area, surface.Width);
                    bool[] roads = new[] { false, false, false, false, false, false, false, false, false };

                    for (int i = 1; i < 9; i++)
                    {
                        if (!valids[i])
                            continue;

                        if (lineStyle.Contains(surface[posIndexes[i]].Glyph))
                            roads[i] = true;
                    }

                    if (roads[(int)Direction.Types.Up] &&
                    roads[(int)Direction.Types.Down] &&
                    roads[(int)Direction.Types.Right] &&
                    roads[(int)Direction.Types.Left])
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.Middle];
                        surface.IsDirty = true;
                    }
                    else if (!roads[(int)Direction.Types.Up] &&
                    roads[(int)Direction.Types.Down] &&
                    roads[(int)Direction.Types.Right] &&
                    roads[(int)Direction.Types.Left])
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.TopMiddleToDown];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)Direction.Types.Up] &&
                    !roads[(int)Direction.Types.Down] &&
                    roads[(int)Direction.Types.Right] &&
                    roads[(int)Direction.Types.Left])
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.BottomMiddleToTop];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)Direction.Types.Up] &&
                    roads[(int)Direction.Types.Down] &&
                    !roads[(int)Direction.Types.Right] &&
                    roads[(int)Direction.Types.Left])
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.RightMiddleToLeft];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)Direction.Types.Up] &&
                    roads[(int)Direction.Types.Down] &&
                    roads[(int)Direction.Types.Right] &&
                    !roads[(int)Direction.Types.Left])
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.LeftMiddleToRight];
                        surface.IsDirty = true;
                    }
                    else if (!roads[(int)Direction.Types.Up] &&
                    !roads[(int)Direction.Types.Down] &&
                    (roads[(int)Direction.Types.Right] ||
                    roads[(int)Direction.Types.Left]))
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.Top];
                        surface.IsDirty = true;
                    }
                    else if ((roads[(int)Direction.Types.Up] ||
                    roads[(int)Direction.Types.Down]) &&
                    !roads[(int)Direction.Types.Right] &&
                    !roads[(int)Direction.Types.Left])
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.Left];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)Direction.Types.Up] &&
                    !roads[(int)Direction.Types.Down] &&
                    !roads[(int)Direction.Types.Right] &&
                    roads[(int)Direction.Types.Left])
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.BottomRight];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)Direction.Types.Up] &&
                    !roads[(int)Direction.Types.Down] &&
                    roads[(int)Direction.Types.Right] &&
                    !roads[(int)Direction.Types.Left])
                    {

                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.BottomLeft];
                        surface.IsDirty = true;
                    }
                    else if (!roads[(int)Direction.Types.Up] &&
                    roads[(int)Direction.Types.Down] &&
                    !roads[(int)Direction.Types.Right] &&
                    roads[(int)Direction.Types.Left])
                    {
                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.TopRight];
                        surface.IsDirty = true;
                    }
                    else if (!roads[(int)Direction.Types.Up] &&
                    roads[(int)Direction.Types.Down] &&
                    roads[(int)Direction.Types.Right] &&
                    !roads[(int)Direction.Types.Left])
                    {
                        surface[index].Glyph = lineStyle[(int)ICellSurface.ConnectedLineIndex.TopLeft];
                        surface.IsDirty = true;
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of the cell surface to the destination.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <remarks>If the sizes to not match, it will always start at 0,0 and work with what it can and move on to the next row when either surface runs out of columns being processed</remarks>
        /// <param name="destination">The destination surface.</param>
        public static void Copy(this ICellSurface surface, ICellSurface destination)
        {
            if (surface.Width == destination.Width && surface.Height == destination.Height)
            {
                for (int i = 0; i < surface.Count; i++)
                    surface[i].CopyAppearanceTo(destination[i]);

                return;
            }

            int maxX = surface.Width >= destination.Width ? destination.Width : surface.Width;
            int maxY = surface.Height >= destination.Height ? destination.Height : surface.Height;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (surface.IsValidCell(x, y, out int sourceIndex) && destination.IsValidCell(x, y, out int destIndex))
                        surface[sourceIndex].CopyAppearanceTo(destination[destIndex]);
                }
            }

            destination.IsDirty = true;
        }

        /// <summary>
        /// Copies the contents of the cell surface to the destination at the specified x,y.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x coordinate of the destination.</param>
        /// <param name="y">The y coordinate of the destination.</param>
        /// <param name="destination">The destination surface.</param>
        public static void Copy(this ICellSurface surface, ICellSurface destination, int x, int y)
        {
            for (int curX = 0; curX < surface.Width; curX++)
            {
                for (int curY = 0; curY < surface.Height; curY++)
                {
                    if (IsValidCell(surface, curX, curY, out int sourceIndex) && destination.IsValidCell(x + curX, y + curY, out int destIndex))
                        surface[sourceIndex].CopyAppearanceTo(destination[destIndex]);
                }
            }

            destination.IsDirty = true;
        }

        /// <summary>
        /// Copies an area of this cell surface to the destination surface.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="area">The area to copy.</param>
        /// <param name="destination">The destination surface.</param>
        /// <param name="destinationX">The x coordinate to copy to.</param>
        /// <param name="destinationY">The y coordinate to copy to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(this ICellSurface surface, Rectangle area, ICellSurface destination, int destinationX, int destinationY) =>
            Copy(surface, area.X, area.Y, area.Width, area.Height, destination, destinationX, destinationY);

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination, only with the specified surface.BufferWidth and surface.BufferHeight, and copies it to the specified <paramref name="destinationX"/> and <paramref name="destinationY"/> position.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="width">The BufferWidth to copy from.</param>
        /// <param name="height">The BufferHeight to copy from.</param>
        /// <param name="destination">The destination surface.</param>
        /// <param name="destinationX">The x coordinate to copy to.</param>
        /// <param name="destinationY">The y coordinate to copy to.</param>
        public static void Copy(this ICellSurface surface, int x, int y, int width, int height, ICellSurface destination, int destinationX, int destinationY)
        {
            int destX = destinationX;
            int destY = destinationY;

            for (int curX = 0; curX < width; curX++)
            {
                for (int curY = 0; curY < height; curY++)
                {
                    if (IsValidCell(surface, curX + x, curY + y, out int sourceIndex) && destination.IsValidCell(destX, destY, out int destIndex))
                        surface[sourceIndex].CopyAppearanceTo(destination[destIndex]);

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
        /// <param name="surface">The surface being edited.</param>
        /// <param name="associatedFont">The font used in assigning glyphs randomly.</param>
        public static void FillWithRandomGarbage(this ICellSurface surface, IFont associatedFont) =>
            FillWithRandomGarbage(surface, associatedFont.TotalGlyphs);

        /// <summary>
        /// Fills a console with random colors and glyphs.
        /// </summary>
        /// <param name="surface">The surface being edited.</param>
        /// <param name="maxGlyphValue">The maximum glyph value to use on the surface.</param>
        public static void FillWithRandomGarbage(this ICellSurface surface, int maxGlyphValue)
        {
            //pulse.Reset();
            int charCounter = 0;
            for (int y = 0; y < surface.Height; y++)
            {
                for (int x = 0; x < surface.Width; x++)
                {
                    SetGlyph(surface, x, y, charCounter);
                    SetForeground(surface, x, y, new Color((byte)GameHost.Instance.Random.Next(0, 256), (byte)GameHost.Instance.Random.Next(0, 256), (byte)GameHost.Instance.Random.Next(0, 256), (byte)255));
                    SetBackground(surface, x, y, surface.DefaultBackground);
                    SetBackground(surface, x, y, new Color((byte)GameHost.Instance.Random.Next(0, 256), (byte)GameHost.Instance.Random.Next(0, 256), (byte)GameHost.Instance.Random.Next(0, 256), (byte)255));
                    SetMirror(surface, x, y, (Mirror)GameHost.Instance.Random.Next(0, 4));

                    if (charCounter > maxGlyphValue)
                        charCounter = 0;
                    charCounter++;
                }
            }

            surface.IsDirty = true;
        }
    }
}
