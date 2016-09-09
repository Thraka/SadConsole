#if SFML
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
#endif

using System;
using System.Runtime.Serialization;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Basic implementation of <see cref="ITextSurface"/>.
    /// </summary>
    [DataContract]
    public class TextSurfaceBasic : ITextSurface
    {
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
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">THe height of the surface.</param>
        public TextSurfaceBasic(int width, int height)
        {
            this.width = width;
            this.height = height;
            InitializeCells();
        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">THe height of the surface.</param>
        /// <param name="initialCells"></param>
        public TextSurfaceBasic(int width, int height, Cell[] initialCells)
        {
            if (initialCells.Length != width * height)
                throw new ArgumentOutOfRangeException("initialCells", "initialCells length must equal width * height");

            this.width = width;
            this.height = height;
            cells = initialCells;
        }

        /// <summary>
        /// Initializes the cells. This method caches all of the rendering points and rectangles and initializes each cell.
        /// </summary>
        protected virtual void InitializeCells()
        {
            cells = new Cell[width * height];

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell();
                cells[i].Foreground = this.DefaultForeground;
                cells[i].Background = this.DefaultBackground;
                cells[i].OnCreated();
            }
        }

        #region Serialization
        /// <summary>
        /// Saves the <see cref="TextSurfaceBasic"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file)
        {
            Serializer.Save(this, file);
        }

        /// <summary>
        /// Loads a <see cref="TextSurfaceBasic"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static TextSurfaceBasic Load(string file)
        {
            return Serializer.Load<TextSurfaceBasic>(file);
        }
        
        #endregion
    }
}
