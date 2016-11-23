using SadConsole;

#if SFML
using Point = SFML.System.Vector2i;
using Matrix = SFML.Graphics.Transform;

namespace SFML.System
#elif MONOGAME
namespace Microsoft.Xna.Framework
#endif
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
        public static Point ConsoleLocationToWorld(this Point point, int cellWidth, int cellHeight)
        {
            return new Point(point.X * cellWidth, point.Y * cellHeight);
        }

        /// <summary>
        /// Translates a pixel to where it appears on a console cell.
        /// </summary>
        /// <param name="point">The current world position.</param>
        /// <param name="cellWidth">The width of a cell in pixels.</param>
        /// <param name="cellHeight">The height of a cell in pixels.</param>
        /// <returns>The cell position on the screen.</returns>
        public static Point WorldLocationToConsole(this Point point, int cellWidth, int cellHeight)
        {
            return new Point(point.X / cellWidth, point.Y / cellHeight);
        }

        /// <summary>
        /// Translates an x,y position to an array index.
        /// </summary>
        /// <param name="point">The position.</param>
        /// <param name="rowWidth">How many columns in a row.</param>
        /// <returns></returns>
        public static int ToIndex(this Point point, int rowWidth)
        {
            return point.Y * rowWidth + point.X;
        }

        /// <summary>
        /// Gets the cell coordinates of the <paramref name="targetFont"/> based on a cell in the <paramref name="sourceFont"/>.
        /// </summary>
        /// <param name="point">The position of the cell in the <paramref name="sourceFont"/>.</param>
        /// <param name="sourceFont">The source font translating from.</param>
        /// <param name="targetFont">The target font translating to.</param>
        /// <returns>The position of the cell in the <paramref name="targetFont"/>.</returns>
        public static Point TranslateFont(this Point point, Font sourceFont, Font targetFont)
        {
            var world = point.ConsoleLocationToWorld(sourceFont.Size.X, sourceFont.Size.Y);
            return world.WorldLocationToConsole(targetFont.Size.X, targetFont.Size.Y);
        }

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
                worldLocation = position;
            else
                worldLocation = position.ConsoleLocationToWorld(cellSize.X, cellSize.Y);

#if SFML
            var transform = Matrix.Identity;
            transform.Translate(worldLocation.X, worldLocation.Y);
            
            return transform;
#elif MONOGAME
            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
#endif
        }
    }

#if SFML
    public static class Vector2fExtensions
    {
        public static float Length(this Vector2f v)
        {
            return (float)global::System.Math.Sqrt((v.X * v.X) + (v.Y * v.Y));
        }
    }
#endif
}
