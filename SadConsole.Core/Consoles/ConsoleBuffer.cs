using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Consoles
{
    public class ConsoleBuffer: IEnumerable<Cell>, IConsoleBufferView
    {
        protected Font font;

        [DataMember]
        protected int width;

        [DataMember]
        protected int height;

        [DataMember]
        protected Cell[] cells;

        [DataMember]
        public Rectangle[] RenderRects { get; protected set; }

        [DataMember]
        public Cell[] Cells { get { return cells; } }

        [DataMember]
        public Rectangle AbsoluteArea { get; protected set; }

        public Font Font { get { return font; } set { font = value; SetRenderData(); } }

        [DataMember]
        public Color DefaultBackground { get; set; }

        [DataMember]
        public Color DefaultForeground { get; set; }

        [DataMember]
        public Color Tint { get; set; }

        public int Width { get { return width; } }
        public int Height { get { return height; } }

        /// <summary>
        /// Gets a cell based on it's coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get { return Cells[y * width + x]; }
            protected set { Cells[y * width + x] = value; }
        }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int index]
        {
            get { return Cells[index]; }
            protected set { Cells[index] = value; }
        }

        public ConsoleBuffer(int width, int height) : this(width, height, Engine.DefaultFont) { }

        public ConsoleBuffer(int width, int height, Font font)
        {
            this.width = width;
            this.height = height;

            DefaultBackground = Color.Transparent;
            DefaultForeground = Color.White;
            //ReuseEffects = true;
            this.font = font;
            InitializeCells();

            //_effects = new Dictionary<ICellEffect, CellEffectData>(20);
            //_effectCells = new Dictionary<Cell, CellEffectData>(50);
        }
        
        /// <summary>
        /// Does nothing. Must be initialized in a derived class.
        /// </summary>
        protected ConsoleBuffer() { }
        
        /// <summary>
        /// Initializes the cells. This method caches all of the rendering points and rectangles and initializes each cell.
        /// </summary>
        /// <param name="oldWidth">The old size of the surface in width. Used when resizing to preserve existing cells.</param>
        protected virtual void InitializeCells()
        {
            bool processOldCells = Cells != null;

            cells = new Cell[width * height];

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell();
                cells[i].Foreground = this.DefaultForeground;
                cells[i].Background = this.DefaultBackground;
                cells[i].Position = GetPointFromIndex(i);
                cells[i].Index = i;
                cells[i].OnCreated();
            }

            // Setup the new render area
            SetRenderData();
        }
        
        /// <summary>
        /// Keeps the text view data in sync with this surface.
        /// </summary>
        protected virtual void SetRenderData()
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

        #region IsValidCell
        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        public bool IsValidCell(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <param name="index">If the cell is valid, the index of the cell when found.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        public bool IsValidCell(int x, int y, out int index)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                index = y * width + x;
                return true;
            }
            else
            {
                index = -1;
                return false;
            }
        }

        /// <summary>
        /// Tests if a cell is valid based on its index.
        /// </summary>
        /// <param name="index">The index to test.</param>
        /// <returns>A true value indicating the cell index is in this cell surface.</returns>
        public bool IsValidCell(int index)
        {
            return index >= 0 && index < Cells.Length;
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

        #region Copy
        /// <summary>
        /// Copies the contents of this cell surface to the destination.
        /// </summary>
        /// <remarks>If the sizes to not match, it will always start at 0,0 and work with what it can and move on to the next row when either surface runs out of columns being processed</remarks>
        /// <param name="destination">The destination surface.</param>
        public void Copy(ConsoleBuffer destination)
        {
            int maxX = width >= destination.width ? destination.width : width;
            int maxY = height >= destination.height ? destination.height : height;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (IsValidCell(x, y) && destination.IsValidCell(x, y))
                    {
                        var sourceCell = this[x, y];
                        var desCell = destination[x, y];
                        sourceCell.CopyAppearanceTo(desCell);
                        //destination.SetEffect(desCell, sourceCell.Effect);
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface to the destination at the specified x,y.
        /// </summary>
        /// <param name="x">The x coordinate of the destination.</param>
        /// <param name="y">The y coordinate of the destination.</param>
        /// <param name="destination">The destination surface.</param>
        public void Copy(ConsoleBuffer destination, int x, int y)
        {
            for (int curx = 0; curx < width; curx++)
            {
                for (int cury = 0; cury < height; cury++)
                {
                    var sourceCell = this[curx, cury];

                    if (destination.IsValidCell(x + curx, y + cury))
                    {
                        var desCell = destination[x + curx, y + cury];
                        sourceCell.CopyAppearanceTo(desCell);
                        //destination.SetEffect(desCell, sourceCell.Effect);
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination.
        /// </summary>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="destination">The destination surface.</param>
        public void Copy(int x, int y, ConsoleBuffer destination)
        {
            int maxX = width - x >= destination.width ? destination.width : width;
            int maxY = height - y >= destination.height ? destination.height : height;

            int destX = 0;
            int destY = 0;

            for (int curx = x; curx < maxX; curx++)
            {
                for (int cury = y; cury < maxY; cury++)
                {
                    if (IsValidCell(curx, cury) && destination.IsValidCell(destX, destY))
                    {
                        var sourceCell = this[curx, cury];
                        var desCell = destination[destX, destY];
                        sourceCell.CopyAppearanceTo(desCell);
                        //destination.SetEffect(desCell, sourceCell.Effect);
                        destY++;
                    }
                }
                destY = 0;
                destX++;
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination, only with the specified width and height.
        /// </summary>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="width">The width to copy from.</param>
        /// <param name="height">The height to copy from.</param>
        /// <param name="destination">The destination surface.</param>
        public void Copy(int x, int y, int width, int height, ConsoleBuffer destination)
        {
            int maxX = width > destination.width ? destination.width : x + width;
            int maxY = height > destination.height ? destination.height : y + height;

            int destX = 0;
            int destY = 0;

            for (int curx = 0; curx < maxX; curx++)
            {
                for (int cury = 0; cury < maxY; cury++)
                {
                    if (IsValidCell(curx + x, cury + y) && destination.IsValidCell(destX, destY))
                    {
                        var sourceCell = this[curx + x, cury + y];
                        var desCell = destination[destX, destY];
                        sourceCell.CopyAppearanceTo(desCell);
                        //destination.SetEffect(desCell, sourceCell.Effect);
                        destY++;
                    }
                }
                destY = 0;
                destX++;
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination, only with the specified width and height, and copies it to the specified <paramref name="destinationX"/> and <paramref name="destinationY"/> position.
        /// </summary>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="width">The width to copy from.</param>
        /// <param name="height">The height to copy from.</param>
        /// <param name="destination">The destination surface.</param>
        /// <param name="destinationX">The x coordinate to copy to.</param>
        /// <param name="destinationY">The y coordinate to copy to.</param>
        public void Copy(int x, int y, int width, int height, ConsoleBuffer destination, int destinationX, int destinationY)
        {
            int destX = destinationX;
            int destY = destinationY;

            for (int curx = 0; curx < width; curx++)
            {
                for (int cury = 0; cury < height; cury++)
                {
                    if (IsValidCell(curx + x, cury + y) && destination.IsValidCell(destX, destY))
                    {
                        var sourceCell = this[curx + x, cury + y];
                        var desCell = destination[destX, destY];
                        sourceCell.CopyAppearanceTo(desCell);
                        //destination.SetEffect(desCell, sourceCell.Effect);
                    }
                    destY++;
                }
                destY = destinationY;
                destX++;
            }
        }
        #endregion

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
    }

    public class ConsoleSubBuffer : ConsoleBuffer, IEnumerable<Cell>
    {
        ConsoleBuffer parentBuffer;
        Rectangle area;

        public Point AreaPosition { get { return area.Location; } }
        
        public ConsoleSubBuffer(ConsoleBuffer buffer, int x, int y, int width, int height)
        {
            this.width = width;
            this.height = height;

            DefaultBackground = buffer.DefaultBackground;
            DefaultForeground = buffer.DefaultForeground;

            parentBuffer = buffer;
            font = buffer.Font;
            
            area = new Rectangle(x, y, width, height);
        }

        protected override void SetRenderData()
        {
            RenderRects = new Rectangle[area.Width * area.Height];
            cells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = new Rectangle(x * font.Size.X, y * font.Size.Y, font.Size.X, font.Size.Y);
                    cells[index] = parentBuffer[(y + area.Top) * area.Width + (x + area.Left)];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, area.Width * font.Size.X, area.Height * font.Size.Y);
        }

        public void MoveAreaPosition(int x, int y)
        {
            area.X = x;
            area.Y = y;
            SetRenderData();
        }

        public void MoveAreaPosition(Point position)
        {
            MoveAreaPosition(position.X, position.Y);
        }
    }
}
