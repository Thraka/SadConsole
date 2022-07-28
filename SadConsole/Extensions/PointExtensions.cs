using System.Runtime.CompilerServices;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Extensions for the <see cref="Point"/> type.
/// </summary>
public static class PointExtensions
{
    /// <summary>
    /// Translates a surface cell position to where it appears on the screen in pixels.
    /// </summary>
    /// <param name="point">The current cell position.</param>
    /// <param name="cellWidth">The width of a cell in pixels.</param>
    /// <param name="cellHeight">The height of a cell in pixels.</param>
    /// <returns>The pixel position of the top-left of the cell.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point SurfaceLocationToPixel(this Point point, int cellWidth, int cellHeight) =>
        new Point(point.X * cellWidth, point.Y * cellHeight);

    /// <summary>
    /// Translates a surface cell position to where it appears on the screen in pixels.
    /// </summary>
    /// <param name="point">The current cell position.</param>
    /// <param name="fontSize">The font to use in calculating the position.</param>
    /// <returns>The pixel position of the top-left of the cell.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point SurfaceLocationToPixel(this Point point, Point fontSize) =>
        new Point(point.X * fontSize.X, point.Y * fontSize.Y);

    /// <summary>
    /// Translates a pixel to where it appears on a surface cell.
    /// </summary>
    /// <param name="point">The current world position.</param>
    /// <param name="cellWidth">The width of a cell in pixels.</param>
    /// <param name="cellHeight">The height of a cell in pixels.</param>
    /// <returns>The cell position on the screen.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point PixelLocationToSurface(this Point point, int cellWidth, int cellHeight) =>
        new Point(point.X / cellWidth, point.Y / cellHeight);

    /// <summary>
    /// Translates a pixel to where it appears on a surface cell.
    /// </summary>
    /// <param name="point">The current world position.</param>
    /// <param name="fontSize">The font to use in calculating the position.</param>
    /// <returns>The cell position on the screen.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point PixelLocationToSurface(this Point point, Point fontSize) =>
        new Point(point.X / fontSize.X, point.Y / fontSize.Y);

    /// <summary>
    /// Translates the coordinates of a cell from the source font size to a target font size.
    /// </summary>
    /// <param name="point">The position of the cell in the <paramref name="sourceFontSize"/>.</param>
    /// <param name="sourceFontSize">The source font translating from.</param>
    /// <param name="targetFontSize">The target font translating to.</param>
    /// <returns>The position of the cell in the <paramref name="targetFontSize"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point TranslateFont(this Point point, Point sourceFontSize, Point targetFontSize) =>
        point.SurfaceLocationToPixel(sourceFontSize.X, sourceFontSize.Y).PixelLocationToSurface(targetFontSize.X, targetFontSize.Y);

    /// <summary>
    /// Gets a list of indexed boolean values to indicate if the direction from the <paramref name="position"/> falls in the <paramref name="area"/>.
    /// </summary>
    /// <param name="position">The position to test from.</param>
    /// <param name="area">The area to test.</param>
    /// <returns>An array of bool values indicating if the direction is valid or not; indexed by a <see cref="Direction.Types"/> enumeration. Index 0 in the array represents the <paramref name="position"/>.</returns>
    public static bool[] GetValidDirections(this Point position, Rectangle area) =>
        new bool[]
        {
            area.Contains(position),
            area.Contains(position + Direction.Up),
            area.Contains(position + Direction.UpRight),
            area.Contains(position + Direction.Right),
            area.Contains(position + Direction.DownRight),
            area.Contains(position + Direction.Down),
            area.Contains(position + Direction.DownLeft),
            area.Contains(position + Direction.Left),
            area.Contains(position + Direction.UpLeft),
        };

    /// <summary>
    /// Gets an indexed array of direction positions based on the <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The source position.</param>
    /// <returns>An array of positions indexed by a <see cref="Direction.Types"/> enumeration. Index 0 in the array represents the <paramref name="position"/>.</returns>
    public static Point[] GetDirectionPoints(this Point position) =>
        new Point[]
        {
            position,
            position + Direction.Up,
            position + Direction.UpRight,
            position + Direction.Right,
            position + Direction.DownRight,
            position + Direction.Down,
            position + Direction.DownLeft,
            position + Direction.Left,
            position + Direction.UpLeft,
        };

    /// <summary>
    /// Gets an array of indexes of a surface based on a position and then a relative <see cref="Direction.Type"/> direction enumeration..
    /// </summary>
    /// <param name="position">The position center.</param>
    /// <param name="area">The area containing the position.</param>
    /// <param name="width">The width to use in converting each index to a point.</param>
    /// <returns>Returns the an array of values indidcating the index in the area surface of each direction where -1 represents a position outside the bounds of the area. Indexed by a <see cref="Direction.Types"/> enumeration.</returns>
    public static int[] GetDirectionIndexes(this Point position, Rectangle area, int width)
    {
        bool[] valids = GetValidDirections(position, area);
        Point[] points = GetDirectionPoints(position);

        return new int[]
        {
            valids[0] ? points[0].ToIndex(width) : -1,
            valids[1] ? points[1].ToIndex(width) : -1,
            valids[2] ? points[2].ToIndex(width) : -1,
            valids[3] ? points[3].ToIndex(width) : -1,
            valids[4] ? points[4].ToIndex(width) : -1,
            valids[5] ? points[5].ToIndex(width) : -1,
            valids[6] ? points[6].ToIndex(width) : -1,
            valids[7] ? points[7].ToIndex(width) : -1,
            valids[8] ? points[8].ToIndex(width) : -1
        };
    }

    ///// <summary>
    ///// Creates a position matrix (in pixels) based on the position of a cell.
    ///// </summary>
    ///// <param name="position">The cell position.</param>
    ///// <param name="cellSize">The size of the cell in pixels.</param>
    ///// <param name="absolutePositioning">When true, indicates that the <paramref name="position"/> indicates pixels, not cell coordinates.</param>
    ///// <returns>A matrix for rendering.</returns>
    //public static Matrix ToPositionMatrix(this Point position, Point cellSize, bool absolutePositioning)
    //{
    //    Point worldLocation;

    //    if (absolutePositioning)
    //    {
    //        worldLocation = position;
    //    }
    //    else
    //    {
    //        worldLocation = position.SurfaceLocationToPixel(cellSize.X, cellSize.Y);
    //    }

    //    return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
    //}
}
