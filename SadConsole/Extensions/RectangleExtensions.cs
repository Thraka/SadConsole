using System.Runtime.CompilerServices;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Extensions for the <see cref="Rectangle"/> type.
/// </summary>
public static class RectangleExtensions
{
    /// <summary>
    /// Converts a rectangle from cells to pixels.
    /// </summary>
    /// <param name="rect">The rectangle to work with.</param>
    /// <param name="fontSize">The font size used for translation.</param>
    /// <returns>A new rectangle in pixels.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle ToPixels(this Rectangle rect, Point fontSize) =>
        new Rectangle(rect.Position * fontSize, rect.Size * fontSize);

    /// <summary>
    /// Converts a rectangle from cells to pixels.
    /// </summary>
    /// <param name="rect">The rectangle to work with.</param>
    /// <param name="cellWidth">The width of a cell used in converting.</param>
    /// <param name="cellHeight">The height of a cell used in converting.</param>
    /// <returns>A new rectangle in pixels.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle ToPixels(this Rectangle rect, int cellWidth, int cellHeight) =>
        new Rectangle(rect.X * cellWidth, rect.Y * cellHeight, rect.Width * cellWidth, rect.Height * cellHeight);

    /// <summary>
    /// Converts a rectangle from pixels to cells.
    /// </summary>
    /// <param name="rect">The rectangle to work with.</param>
    /// <param name="fontSize">The font size used for translation.</param>
    /// <returns>A new rectangle in cell coordinates.</returns>
    public static Rectangle ToConsole(this Rectangle rect, Point fontSize) =>
        new Rectangle(rect.Position / fontSize, rect.Size / fontSize);

    /// <summary>
    /// Converts a rectangle from pixels to cells.
    /// </summary>
    /// <param name="rect">The rectangle to work with.</param>
    /// <param name="cellWidth">The width of a cell used in converting.</param>
    /// <param name="cellHeight">The height of a cell used in converting.</param>
    /// <returns>A new rectangle in cell coordinates.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle ToConsole(this Rectangle rect, int cellWidth, int cellHeight) =>
        new Rectangle(rect.X / cellWidth, rect.Y / cellHeight, rect.Width / cellWidth, rect.Height / cellHeight);
}
