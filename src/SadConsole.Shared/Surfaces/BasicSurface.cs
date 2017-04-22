using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// The base class for a text surface. Provides code for the view port and basic cell access.
    /// </summary>
    public class BasicSurface : IEnumerable<Cell>, ISurface
    {
        protected Font font;

        protected Rectangle area;

        protected Color tint = Color.Transparent;

        protected bool isDirty = true;

        /// <summary>
        /// An array of all cells in this surface.
        /// </summary>
        /// <remarks>This array is calculated internally and its size shouldn't be modified. Use the <see cref="width"/> and <see cref="height"/> properties instead. The cell data can be changed.</remarks>
        [DataMember(Name = "Cells")]
        protected Cell[] cells;

        /// <summary>
        /// The width of the surface.
        /// </summary>
        [DataMember(Name = "Width")]
        protected int width = 1;

        /// <summary>
        /// The height of the surface.
        /// </summary>
        [DataMember(Name = "Height")]
        protected int height = 1;
        
        /// <summary>
        /// The default foreground for glyphs on this surface.
        /// </summary>
        [DataMember]
        public Color DefaultForeground { get; set; } = Color.White;

        /// <summary>
        /// The default background for glyphs on this surface.
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

        /// <summary>
        /// All cells of the surface.
        /// </summary>
        public Cell[] Cells { get { return cells; } }

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
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
        /// Font used with rendering.
        /// </summary>
        public Font Font { get { return font; } set { font = value; OnFontChanged(); } }
        
        /// <summary>
        /// Pixel area of the render cells.
        /// </summary>
        public Rectangle AbsoluteArea { get; set; }

        /// <summary>
        /// Destination rectangles for rendering.
        /// </summary>
        public Rectangle[] RenderRects { get; set; }
        
        /// <summary>
        /// Cells that will be rendered.
        /// </summary>
        public Cell[] RenderCells { get; set; }

        /// <summary>
        /// A tint used in rendering.
        /// </summary>
        public Color Tint { get { return tint; } set { tint = value; IsDirty = true; } }

        /// <summary>
        /// Sets the area of the text surface that should be rendered.
        /// </summary>
        public Rectangle RenderArea
        {
            get { return area; }
            set
            {
                area = value;

                if (area == null)
                    area = new Rectangle(0, 0, width, height);
                if (area.Width > width)
                    area.Width = width;
                if (area.Height > height)
                    area.Height = height;

                if (area.X < 0)
                    area.X = 0;
                if (area.Y < 0)
                    area.Y = 0;

                if (area.X + area.Width > width)
                    area.X = width - area.Width;
                if (area.Y + area.Height > height)
                    area.Y = height - area.Height;

                IsDirty = true;
                ResetArea();
            }
        }

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                bool old = isDirty;
                isDirty = value;
                if (value && !old)
                    OnIsDirty?.Invoke(this);
            }
        }

        /// <summary>
        /// The last texture render pass for this surface.
        /// </summary>
        public RenderTarget2D LastRenderResult { get; set; }

        /// <summary>
        /// A callback that happens when <see cref="IsDirty"/> is set to true.
        /// </summary>
        public Action<ISurface> OnIsDirty { get; set; }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">THe height of the surface.</param>
        public BasicSurface(int width, int height) : this(width, height, null, Global.FontDefault, new Rectangle(0, 0, width, height))
        {

        }

        /// <summary>
        /// Disposes <see cref="LastRenderResult"/>.
        /// </summary>
        ~BasicSurface()
        {
            //System.Diagnostics.Debugger.Break();
            if (LastRenderResult != null)
                LastRenderResult.Dispose();
        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="renderArea">Initial value for the <see cref="RenderArea"/> view.</param>
        public BasicSurface(int width, int height, Rectangle renderArea) : this(width, height, null, Global.FontDefault, renderArea)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        public BasicSurface(int width, int height, Font font) : this(width, height, null, font, new Rectangle(0, 0, width, height))
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="renderArea">Initial value for the <see cref="RenderArea"/> view.</param>
        public BasicSurface(int width, int height, Font font, Rectangle renderArea) : this(width, height, null, font, renderArea)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="initialCells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        /// <param name="renderArea">Initial value for the <see cref="RenderArea"/> view.</param>
        public BasicSurface(int width, int height, Cell[] initialCells, Font font, Rectangle renderArea)
        {
            this.area = renderArea;
            this.width = width;
            this.height = height;

            LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, renderArea.Width * font.Size.X, renderArea.Height * font.Size.Y);

            if (initialCells == null)
                InitializeCells();
            else if (initialCells.Length != width * height)
                throw new ArgumentOutOfRangeException("initialCells", "initialCells length must equal width * height");
            else
            {
                cells = new Cell[initialCells.Length];
                RenderCells = new Cell[initialCells.Length];
                initialCells.CopyTo(cells, 0);
                initialCells.CopyTo(RenderCells, 0);
            }

            Font = font;
        }

        /// <summary>
        /// Initialization is left to the derived class.
        /// </summary>
        protected BasicSurface() { }
        
        /// <summary>
        /// Sets <see cref="RenderCells"/> to <see cref="TextSurfaceBasic.cells"/>.
        /// </summary>
        protected virtual void InitializeCells()
        {
            cells = new Cell[width * height];

            for (int i = 0; i < cells.Length; i++)
                cells[i] = new Cell(this.DefaultForeground, this.DefaultBackground, 0);

            RenderCells = cells;
        }

        /// <summary>
        /// Keeps the text view data in sync with this surface.
        /// </summary>
        protected virtual void ResetArea()
        {
            RenderRects = new Rectangle[area.Width * area.Height];
            RenderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = font.GetRenderRect(x, y);
                    RenderCells[index] = cells[(y + area.Top) * width + (x + area.Left)];
                    index++;
                }
            }

            // TODO: Optimization by calculating AbsArea and seeing if it's diff from current, if so, don't create new RenderRects
            AbsoluteArea = new Rectangle(0, 0, area.Width * font.Size.X, area.Height * font.Size.Y);

            if (LastRenderResult.Bounds.Size != AbsoluteArea.Size)
            {
                LastRenderResult.Dispose();
                LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }
        }

        protected virtual void OnFontChanged()
        {
            ResetArea();
        }


#region Static Methods


        public static int GetIndexFromPoint(Point location, int width)
        {
            return location.Y * width + location.X;
        }

        

        public static int GetIndexFromPoint(int x, int y, int width)
        {
            return y * width + x;
        }
        
        public static Point GetPointFromIndex(int index, int width)
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

        /// <summary>
        /// Saves the <see cref="BasicSurface"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file)
        {
            Serializer.Save((SerializedTypes.BasicSurfaceSerialized)this, file);
        }

        /// <summary>
        /// Loads a <see cref="BasicSurface"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static BasicSurface Load(string file)
        {
            return Serializer.Load<SerializedTypes.BasicSurfaceSerialized>(file);
        }
    }
}
