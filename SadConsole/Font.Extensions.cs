using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Extensions for <see cref="IFont"/>.
/// </summary>
public static class FontExtensions
{
    /// <summary>
    /// Returns a rectangle that is positioned and sized based on the font and the cell position specified.
    /// </summary>
    /// <param name="font">Unused.</param>
    /// <param name="x">The x-axis of the cell position.</param>
    /// <param name="y">The y-axis of the cell position.</param>
    /// <param name="fontSize">The size of the output cell.</param>
    /// <returns>A rectangle to representing a specific cell.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle GetRenderRect(this IFont font, int x, int y, Point fontSize) => new Rectangle(x * fontSize.X, y * fontSize.Y, fontSize.X, fontSize.Y);

    /// <summary>
    /// Gets the pixel position of a cell position based on the font size.
    /// </summary>
    /// <param name="font">Unused.</param>
    /// <param name="position">The cell position to convert.</param>
    /// <param name="fontSize">The size of the font used to calculate the pixel position.</param>
    /// <returns>A new pixel-positioned point.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point GetWorldPosition(this IFont font, Point position, Point fontSize) => new Point(position.X * fontSize.X, position.Y * fontSize.Y);


    /// <summary>
    /// Returns the ratio in size difference between the font's glyph width and height.
    /// </summary>
    /// <param name="font">Unused.</param>
    /// <param name="fontSize">The glyph size of the font used.</param>
    /// <returns>A tuple with the names (X, Y) where X is the difference of width to height and Y is the difference of height to width.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float X, float Y) GetGlyphRatio(this IFont font, Point fontSize) =>
        ((float)fontSize.X / fontSize.Y, (float)fontSize.Y / fontSize.X);
}
