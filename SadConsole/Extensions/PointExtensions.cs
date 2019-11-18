using System.Runtime.CompilerServices;
using SadConsole;

namespace SadRogue.Primitives
{
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
        /// Gets the cell coordinates of the <paramref name="targetFont"/> based on a cell in the <paramref name="sourceFontSize"/>.
        /// </summary>
        /// <param name="point">The position of the cell in the <paramref name="sourceFontSize"/>.</param>
        /// <param name="sourceFontSize">The source font translating from.</param>
        /// <param name="targetFontSize">The target font translating to.</param>
        /// <returns>The position of the cell in the <paramref name="targetFontSize"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point TranslateFont(this Point point, Point sourceFontSize, Point targetFontSize) =>
            point.SurfaceLocationToPixel(sourceFontSize.X, sourceFontSize.Y).PixelLocationToSurface(targetFontSize.X, targetFontSize.Y);

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
}
