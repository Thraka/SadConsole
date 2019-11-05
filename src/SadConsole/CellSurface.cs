#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// An array of <see cref="Cell"/> objects used to represent a 2D surface.
    /// </summary>
    [JsonConverter(typeof(SerializedTypes.CellSurfaceJsonConverter))]
    public partial class CellSurface : IEnumerable<Cell>
    {
        private bool _isDirty = true;
        private Color _defaultBackground;
        private Color _defaultForeground;

        /// <summary>
        /// An event that is raised when <see cref="IsDirty"/> is set to true.
        /// </summary>
        public event EventHandler DirtyChanged;

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty == value)
                {
                    return;
                }

                _isDirty = value;
                OnDirtyChanged();
            }
        }

        /// <summary>
        /// The default foreground for glyphs on this surface.
        /// </summary>
        public Color DefaultForeground
        {
            get => _defaultForeground;
            set { _defaultForeground = value; IsDirty = true; }
        }

        /// <summary>
        /// The default background for glyphs on this surface.
        /// </summary>
        public Color DefaultBackground
        {
            get => _defaultBackground;
            set { _defaultBackground = value; IsDirty = true; }
        }

        /// <summary>
        /// How many cells wide the surface is.
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// How many cells high the surface is.
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// All cells of the surface.
        /// </summary>
        public Cell[] Cells { get; protected set; }

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get => Cells[y * Width + x];
            protected set => Cells[y * Width + x] = value;
        }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int index]
        {
            get => Cells[index];
            protected set => Cells[index] = value;
        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the surface in cells.</param>
        /// <param name="height">The height of the surface in cells.</param>
        public CellSurface(int width, int height) : this(width, height, null)
        {

        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the surface in cells.</param>
        /// <param name="height">The height of the surface in cells.</param>
        /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
        public CellSurface(int width, int height, Cell[] initialCells)
        {
            DefaultForeground = Color.White;
            DefaultBackground = Color.Transparent;
            Width = width;
            Height = height;

            if (initialCells == null)
            {
                Cells = new Cell[width * height];

                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new Cell(DefaultForeground, DefaultBackground, 0);
                }
            }
            else
            {
                if (initialCells.Length != Width * Height)
                {
                    throw new Exception("Width * Height does not match initialCells.Length");
                }

                Cells = initialCells;
            }

            Effects = new Effects.EffectsManager(this);
        }

        /// <summary>
        /// Called when the <see cref="IsDirty"/> property changes.
        /// </summary>
        protected virtual void OnDirtyChanged() => DirtyChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Called when the <see cref="Cells"/> property is reset.
        /// </summary>
        protected virtual void OnCellsReset() { }

        /// <summary>
        /// Gets an enumerator for <see cref="Cells"/>.
        /// </summary>
        /// <returns>An enumerator for <see cref="Cells"/>.</returns>
        public IEnumerator<Cell> GetEnumerator() => ((IEnumerable<Cell>)Cells).GetEnumerator();

        /// <summary>
        /// Gets an enumerator for <see cref="Cells"/>.
        /// </summary>
        /// <returns>An enumerator for <see cref="Cells"/>.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Cells.GetEnumerator();
    }
}
