using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Readers
{
    public partial class REXPaintImage
    {
        /// <summary>
        /// A layer of a RexPaint image.
        /// </summary>
        public class Layer
        {
            private Cell[] cells;

            /// <summary>
            /// The width of the layer.
            /// </summary>
            public int Width { get; private set; }

            /// <summary>
            /// The height of the layer.
            /// </summary>
            public int Height { get; private set; }

            /// <summary>
            /// Represents all cells of the layer.
            /// </summary>
            public System.Collections.ObjectModel.ReadOnlyCollection<Cell> Cells { get { return new System.Collections.ObjectModel.ReadOnlyCollection<Cell>(cells); } }

            /// <summary>
            /// Gets a cell by coordinates.
            /// </summary>
            /// <param name="x">The x (0-based) position of the cell.</param>
            /// <param name="y">The y (0-based) position of the cell.</param>
            /// <returns>The cell.</returns>
            public Cell this[int x, int y]
            {
                get { CheckForBounds(x, y); return cells[y * Width + x]; }
                set { CheckForBounds(x, y); cells[y * Width + x] = value; }
            }

            /// <summary>
            /// Gets a cell by index.
            /// </summary>
            /// <param name="index">The index of the cell.</param>
            /// <returns>The cell.</returns>
            public Cell this[int index]
            {
                get { CheckForIndexBounds(index); return cells[index]; }
                set { CheckForIndexBounds(index); cells[index] = value; }
            }

            /// <summary>
            /// Creates a new layer with the specified width and height.
            /// </summary>
            /// <param name="width">The width of the layer.</param>
            /// <param name="height">The height of the layer.</param>
            public Layer(int width, int height)
            {
                Width = width;
                Height = height;
                cells = new Cell[width * height];
            }

            private void CheckForIndexBounds(int index)
            {
                if (index < 0 || index >= cells.Length)
                    throw new IndexOutOfRangeException();
            }

            private void CheckForBounds(int x, int y)
            {
                if (x < 0 || x >= Width)
                    throw new ArgumentOutOfRangeException("x");

                if (y < 0 || y >= Height)
                    throw new ArgumentOutOfRangeException("y");
            }
        }
    }
}
