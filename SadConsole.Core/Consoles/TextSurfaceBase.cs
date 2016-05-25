using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Effects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Consoles
{
    [DataContract]
    public abstract class TextSurfaceBase : IEnumerable<Cell>, ITextSurface
    {
        protected Cell[] renderCells;
        protected Font font;

        /// <summary>
        /// An array of all cells in this surface.
        /// </summary>
        /// <remarks>This array is calculated internally and its size shouldn't be modified. Use the <see cref="_width"/> and <see cref="_height"/> properties instead. The cell data can be changed.</remarks>
        [DataMember(Name = "Cells")]
        protected Cell[] cells;
        [DataMember(Name = "Width")]
        protected int width = 1;
        [DataMember(Name = "Height")]
        protected int height = 1;

        /// <summary>
        /// Gets a cell based on it's coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get { return cells[y * width + x]; }
            protected set { cells[y * width + x] = value; }
        }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int index]
        {
            get { return cells[index]; }
            protected set { cells[index] = value; }
        }

        /// <summary>
        /// The total cells for this surface.
        /// </summary>
        public int CellCount { get { return cells.Length; } }

        /// <summary>
        /// The default foreground for characters on this surface.
        /// </summary>
        [DataMember]
        public Color DefaultForeground { get; set; } = Color.White;

        /// <summary>
        /// The default background for characters on this surface.
        /// </summary>
        [DataMember]
        public Color DefaultBackground { get; set; } = Color.Transparent;

        /// <summary>
        /// How many cells wide the surface is.
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// How many cells high the surface is.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        public Font Font { get { return font; } set { font = value; OnFontChanged(); } }

        #region ITextSurfaceView
        public Rectangle AbsoluteArea { get; protected set; }

        public Rectangle[] RenderRects { get; protected set; }

        public Cell[] Cells { get { return cells; } }

        public Cell[] RenderCells { get { return renderCells; } }

        public Color Tint { get; set; } = Color.Transparent;

        #endregion

        protected virtual void OnFontChanged()
        {
            RenderRects = new Rectangle[width * height];

            int index = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    RenderRects[index] = new Rectangle(x * font.Size.X, y * font.Size.Y, font.Size.X, font.Size.Y);
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, width * font.Size.X, height * font.Size.Y);
        }




        #region Static Methods
        public static int GetIndexFromPoint(Point location, int width)
        {
            return location.Y * width + location.X;
        }

        public int GetIndexFromPoint(Point location)
        {
            return location.Y * width + location.X;
        }

        public static int GetIndexFromPoint(int x, int y, int width)
        {
            return y * width + x;
        }

        public int GetIndexFromPoint(int x, int y)
        {
            return y * width + x;
        }

        public static Point GetPointFromIndex(int index, int width)
        {
            return new Point(index % width, index / width);
        }

        public Point GetPointFromIndex(int index)
        {
            return new Point(index % width, index / width);
        }
        #endregion


        public IEnumerator<Cell> GetEnumerator()
        {
            return ((IEnumerable<Cell>)cells).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return cells.GetEnumerator();
        }



    }
}
