using Microsoft.Xna.Framework;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Extension methods relating to all text surface types.
    /// </summary>
    public static class TextSurfaceExtensions
    {
        #region Basic cell access
        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <param name="index">If the cell is valid, the index of the cell when found.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        public static bool IsValidCell(this ITextSurface surface, int x, int y, out int index)
        {
            if (x >= 0 && x < surface.Width && y >= 0 && y < surface.Height)
            {
                index = y * surface.Width + x;
                return true;
            }
            else
            {
                index = -1;
                return false;
            }
        }

        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        public static bool IsValidCell(this ITextSurface surface, int x, int y)
        {
            if (x >= 0 && x < surface.Width && y >= 0 && y < surface.Height)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets a cell based on it's coordinates on the surface.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public static Cell GetCell(this ITextSurface surface, int x, int y)
        {
            return surface.Cells[y * surface.Width + x];
        }

        /// <summary>
        /// Gets the index of a location on the surface by point.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="location">The location of the index to get.</param>
        /// <returns>The cell index.</returns>
        public static int GetIndexFromPoint(this ITextSurface surface, Point location)
        {
            return location.Y * surface.Width + location.X;
        }

        /// <summary>
        /// Gets the index of a location on the surface by coordinate.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="x">The x of the location.</param>
        /// <param name="y">The y of the location.</param>
        /// <returns>The cell index.</returns>
        public static int GetIndexFromPoint(this ITextSurface surface, int x, int y)
        {
            return y * surface.Width + x;
        }

        /// <summary>
        /// Gets the x,y of an index on the surface.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="index">The index to get.</param>
        /// <returns>The x,y as a <see cref="Point"/>.</returns>
        public static Point GetPointFromIndex(this ITextSurface surface, int index)
        {
            return new Point(index % surface.Width, index / surface.Width);
        }
        #endregion

        #region Copy
        /// <summary>
        /// Copies the contents of the cell surface to the destination.
        /// </summary>
        /// <remarks>If the sizes to not match, it will always start at 0,0 and work with what it can and move on to the next row when either surface runs out of columns being processed</remarks>
        /// <param name="source">The source surface</param>
        /// <param name="destination">The destination surface.</param>
        public static void Copy(this ITextSurface source, ITextSurface destination)
        {
            int maxX = source.Width >= destination.Width ? destination.Width : source.Width;
            int maxY = source.Height >= destination.Height ? destination.Height : source.Height;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    int sourceIndex;
                    int destIndex;

                    if (source.IsValidCell(x, y, out sourceIndex) && destination.IsValidCell(x, y, out destIndex))
                    {
                        var sourceCell = source.Cells[sourceIndex];
                        var desCell = destination.Cells[destIndex];
                        sourceCell.CopyAppearanceTo(desCell);
                        desCell.Effect = sourceCell.Effect;
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of the cell surface to the destination at the specified x,y.
        /// </summary>
        /// <param name="x">The x coordinate of the destination.</param>
        /// <param name="y">The y coordinate of the destination.</param>
        /// <param name="source">The source surface</param>
        /// <param name="destination">The destination surface.</param>
        public static void Copy(this ITextSurface source, ITextSurface destination, int x, int y)
        {
            for (int curx = 0; curx < source.Width; curx++)
            {
                for (int cury = 0; cury < source.Height; cury++)
                {
                    int sourceIndex;
                    int destIndex;

                    if (source.IsValidCell(curx, cury, out sourceIndex) && destination.IsValidCell(x + curx, y + cury, out destIndex))
                    {
                        var sourceCell = source.Cells[sourceIndex];
                        var desCell = destination.Cells[destIndex];
                        sourceCell.CopyAppearanceTo(desCell);
                        desCell.Effect = sourceCell.Effect;
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination, only with the specified width and height, and copies it to the specified <paramref name="destinationX"/> and <paramref name="destinationY"/> position.
        /// </summary>
        /// <param name="source">The source surface</param>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="width">The width to copy from.</param>
        /// <param name="height">The height to copy from.</param>
        /// <param name="destination">The destination surface.</param>
        /// <param name="destinationX">The x coordinate to copy to.</param>
        /// <param name="destinationY">The y coordinate to copy to.</param>
        public static void Copy(this ITextSurface source, int x, int y, int width, int height, ITextSurface destination, int destinationX, int destinationY)
        {
            int destX = destinationX;
            int destY = destinationY;

            for (int curx = 0; curx < width; curx++)
            {
                for (int cury = 0; cury < height; cury++)
                {
                    int sourceIndex;
                    int destIndex;

                    if (source.IsValidCell(curx + x, cury + y, out sourceIndex) && destination.IsValidCell(destX, destY, out destIndex))
                    {
                        var sourceCell = source.Cells[sourceIndex];
                        var desCell = destination.Cells[destIndex];
                        sourceCell.CopyAppearanceTo(desCell);
                        desCell.Effect = sourceCell.Effect;
                    }
                    destY++;
                }
                destY = destinationY;
                destX++;
            }
        }
        #endregion

    }
}
