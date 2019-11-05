#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    /// <summary>
    /// Extension methods for the <see cref="Cell"/> type.
    /// </summary>
    public static class CellExtensions
    {
        /// <summary>
        /// Expands a 1-dimensional array into a 2-dimensional array.
        /// </summary>
        /// <param name="items">The original array to use.</param>
        /// <param name="width">The width the array represents.</param>
        /// <returns>A new 2-dimensional array.</returns>
        public static TCell[,] Expand<TCell>(this TCell[] items, int width)
        {
            var result = new TCell[width, items.Length / width];

            for (int i = 0; i < items.Length; i++)
            {
                var point = i.ToPoint(width);
                result[point.X, point.Y] = items[i];
            }

            return result;
        }

        /// <summary>
        /// Flattens a 2-dimensional array into a 1-dimensional array.
        /// </summary>
        /// <typeparam name="TCell"></typeparam>
        /// <param name="items"></param>
        /// <returns>A flattened array of items.</returns>
        public static TCell[] Flatten<TCell>(this TCell[,] items)
        {
            var result = new TCell[items.Length];
            int width = items.GetUpperBound(0) + 1;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y <= items.GetUpperBound(1); y++)
                {
                    result[Helpers.GetIndexFromPoint(x, y, width)] = items[x, y];
                }
            }
            return result;
        }
    }
}
