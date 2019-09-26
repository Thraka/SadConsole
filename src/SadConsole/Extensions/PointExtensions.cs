using System.Runtime.CompilerServices;
using SadConsole;

namespace Microsoft.Xna.Framework
{
    public static class PointExtensions
    {
        /// <summary>
        /// Translates a console cell position to where it appears on the screen in pixels.
        /// </summary>
        /// <param name="point">The current cell position.</param>
        /// <param name="cellWidth">The width of a cell in pixels.</param>
        /// <param name="cellHeight">The height of a cell in pixels.</param>
        /// <returns>The pixel position of the top-left of the cell.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ConsoleLocationToPixel(this Point point, int cellWidth, int cellHeight) =>
            new Point(point.X * cellWidth, point.Y * cellHeight);

        /// <summary>
        /// Translates a console cell position to where it appears on the screen in pixels.
        /// </summary>
        /// <param name="point">The current cell position.</param>
        /// <param name="font">The font to use in calculating the position.</param>
        /// <returns>The pixel position of the top-left of the cell.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ConsoleLocationToPixel(this Point point, Font font) =>
            new Point(point.X * font.Size.X, point.Y * font.Size.Y);

        /// <summary>
        /// Translates a pixel to where it appears on a console cell.
        /// </summary>
        /// <param name="point">The current world position.</param>
        /// <param name="cellWidth">The width of a cell in pixels.</param>
        /// <param name="cellHeight">The height of a cell in pixels.</param>
        /// <returns>The cell position on the screen.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point PixelLocationToConsole(this Point point, int cellWidth, int cellHeight) =>
            new Point(point.X / cellWidth, point.Y / cellHeight);

        /// <summary>
        /// Translates a pixel to where it appears on a console cell.
        /// </summary>
        /// <param name="point">The current world position.</param>
        /// <param name="font">The font to use in calculating the position.</param>
        /// <returns>The cell position on the screen.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point PixelLocationToConsole(this Point point, Font font) =>
            new Point(point.X / font.Size.X, point.Y / font.Size.Y);

        /// <summary>
        /// Translates an x,y position to an array index.
        /// </summary>
        /// <param name="point">The position.</param>
        /// <param name="rowWidth">How many columns in a row.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndex(this Point point, int rowWidth) =>
            Helpers.GetIndexFromPoint(point.X, point.Y, rowWidth);

        /// <summary>
        /// Translates an array index to a Point.
        /// </summary>
        /// <param name="index">The index in the array.</param>
        /// <param name="rowWidth">How many columns in a row.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point ToPoint(this int index, int rowWidth) =>
            Helpers.GetPointFromIndex(index, rowWidth);

        /// <summary>
        /// Gets the cell coordinates of the <paramref name="targetFont"/> based on a cell in the <paramref name="sourceFont"/>.
        /// </summary>
        /// <param name="point">The position of the cell in the <paramref name="sourceFont"/>.</param>
        /// <param name="sourceFont">The source font translating from.</param>
        /// <param name="targetFont">The target font translating to.</param>
        /// <returns>The position of the cell in the <paramref name="targetFont"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point TranslateFont(this Point point, Font sourceFont, Font targetFont) =>
            point.ConsoleLocationToPixel(sourceFont.Size.X, sourceFont.Size.Y).PixelLocationToConsole(targetFont.Size.X, targetFont.Size.Y);

        /// <summary>
        /// Creates a position matrix (in pixels) based on the position of a cell.
        /// </summary>
        /// <param name="position">The cell position.</param>
        /// <param name="cellSize">The size of the cell in pixels.</param>
        /// <param name="absolutePositioning">When true, indicates that the <paramref name="position"/> indicates pixels, not cell coordinates.</param>
        /// <returns>A matrix for rendering.</returns>
        public static Matrix ToPositionMatrix(this Point position, Point cellSize, bool absolutePositioning)
        {
            Point worldLocation;

            if (absolutePositioning)
            {
                worldLocation = position;
            }
            else
            {
                worldLocation = position.ConsoleLocationToPixel(cellSize.X, cellSize.Y);
            }

            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
        }
    }
}
