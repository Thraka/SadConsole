using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An array of <see cref="Cell"/> objects used to represent a 2D surface.
    /// </summary>
    public partial class CellSurface : IEnumerable<Cell>
    {
        private bool _isDirty = true;
        private Color _defaultBackground;
        private Color _defaultForeground;
        private Point _bufferPosition;
        private int _width;
        private int _height;



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
        public int Width
        {
            get => _width;
            set
            {
                if (value > BufferWidth) throw new ArgumentOutOfRangeException(nameof(Width), $"{nameof(Width)} cannot be bigger than {nameof(BufferWidth)}.");
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(Width), $"{nameof(Width)} must be bigger than 1.");
                if (IsBufferLocked) throw new ArgumentException(nameof(Width), $"Cannot modify {nameof(Width)} when {nameof(IsBufferLocked)} is true.");

                _width = value;
                BufferPosition = _bufferPosition;
                IsDirty = true;
            }
        }

        /// <summary>
        /// How many cells high the surface is.
        /// </summary>
        public int Height
        {
            get => _height;
            set
            {
                if (value > BufferHeight) throw new ArgumentOutOfRangeException(nameof(Height), $"{nameof(Height)} cannot be bigger than {nameof(BufferHeight)}.");
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(Height), $"{nameof(Height)} must be bigger than 1.");
                if (IsBufferLocked) throw new ArgumentException(nameof(Height), $"Cannot modify {nameof(Height)} when {nameof(IsBufferLocked)} is true.");

                _height = value;

                BufferPosition = _bufferPosition;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Width of the surface buffer.
        /// </summary>
        public int BufferWidth { get; protected set; }

        /// <summary>
        /// Height of the surface buffer.
        /// </summary>
        public int BufferHeight { get; protected set; }

        /// <summary>
        /// The position of the buffer.
        /// </summary>
        public Point BufferPosition
        {
            get => _bufferPosition;
            set
            {
                if (IsBufferLocked) throw new ArgumentException(nameof(Height), $"Cannot modify {nameof(BufferPosition)} when {nameof(IsBufferLocked)} is true.");

                int x = _bufferPosition.X;
                int y = _bufferPosition.Y;

                if (value.X + _width <= BufferWidth)
                    x = value.X;
                else
                    x = BufferWidth - _width;

                if (value.Y + _height <= BufferHeight)
                    y = value.Y;
                else
                    y = BufferHeight - _height;

                if (x < 0)
                    x = 0;

                if (y < 0)
                    y = 0;

                _bufferPosition = new Point(x, y);
                IsDirty = true;
            }
        }

        /// <summary>
        /// When <see langword="true"/> indicates the buffer can't be moved or resized; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsBufferLocked { get; protected set; }

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
            get => Cells[y * BufferWidth + x];
            protected set => Cells[y * BufferWidth + x] = value;
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
            _width = BufferWidth = width;
            _height = BufferHeight = height;

            IsBufferLocked = true;

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

            //Effects = new Effects.EffectsManager(this);
        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The visible width of the surface in cells.</param>
        /// <param name="height">The visible height of the surface in cells.</param>
        /// <param name="bufferWidth">The total width of the surface in cells.</param>
        /// <param name="bufferHeight">The total height of the surface in cells.</param>
        public CellSurface(int width, int height, int bufferWidth, int bufferHeight) : this(width, height, bufferWidth, bufferHeight, null)
        {

        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the surface in cells.</param>
        /// <param name="height">The height of the surface in cells.</param>
        /// <param name="bufferWidth">The total width of the surface in cells.</param>
        /// <param name="bufferHeight">The total height of the surface in cells.</param>
        /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
        public CellSurface(int width, int height, int bufferWidth, int bufferHeight, Cell[] initialCells)
        {
            if (width > bufferWidth) throw new ArgumentOutOfRangeException(nameof(bufferWidth), "Buffer width must be less than or equal to the width.");
            if (height > bufferHeight) throw new ArgumentOutOfRangeException(nameof(bufferHeight), "Buffer height must be less than or equal to the height.");

            DefaultForeground = Color.White;
            DefaultBackground = Color.Transparent;
            _width = width;
            _height = height;
            BufferWidth = bufferWidth;
            BufferHeight = bufferHeight;

            IsBufferLocked = false;

            if (initialCells == null)
            {
                Cells = new Cell[bufferWidth * bufferWidth];

                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new Cell(DefaultForeground, DefaultBackground, 0);
                }
            }
            else
            {
                if (initialCells.Length != bufferWidth * bufferHeight)
                {
                    throw new Exception("Buffer Width * Buffer Height does not match initialCells.Length");
                }

                Cells = initialCells;
            }

            //Effects = new Effects.EffectsManager(this);
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
