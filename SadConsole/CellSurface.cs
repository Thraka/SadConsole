using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An array of <see cref="ColoredGlyph"/> objects used to represent a 2D surface.
    /// </summary>
    public partial class CellSurface : IEnumerable<ColoredGlyph>
    {
        private bool _isDirty = true;
        private Color _defaultBackground;
        private Color _defaultForeground;
        private Point _viewPosition;
        private int _viewWidth;
        private int _viewHeight;

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
        /// How many cells wide are visible.
        /// </summary>
        public int ViewWidth
        {
            get => _viewWidth;
            set
            {
                if (value > BufferWidth) throw new ArgumentOutOfRangeException(nameof(ViewWidth), $"{nameof(ViewWidth)} cannot be bigger than {nameof(BufferWidth)}.");
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(ViewWidth), $"{nameof(ViewWidth)} must be bigger than 1.");

                _viewWidth = value;
                ViewPosition = _viewPosition;
                IsDirty = true;
            }
        }

        /// <summary>
        /// How many cells high are visible.
        /// </summary>
        public int ViewHeight
        {
            get => _viewHeight;
            set
            {
                if (value > BufferHeight) throw new ArgumentOutOfRangeException(nameof(ViewHeight), $"{nameof(ViewHeight)} cannot be bigger than {nameof(BufferHeight)}.");
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(ViewHeight), $"{nameof(ViewHeight)} must be bigger than 1.");

                _viewHeight = value;
                ViewPosition = _viewPosition;
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
        /// Returns <see langword="true"/> when the <see cref="ViewHeight"/> or <see cref="ViewWidth"/> is different from <see cref="BufferHeight"/> or <see cref="BufferWidth"/>, respectively.
        /// </summary>
        public bool IsScrollable => BufferHeight != ViewHeight || BufferWidth != ViewWidth;

        /// <summary>
        /// The position of the buffer.
        /// </summary>
        public Point ViewPosition
        {
            get => _viewPosition;
            set
            {
                int x = _viewPosition.X;
                int y = _viewPosition.Y;

                if (value.X + _viewWidth <= BufferWidth)
                    x = value.X;
                else
                    x = BufferWidth - _viewWidth;

                if (value.Y + _viewHeight <= BufferHeight)
                    y = value.Y;
                else
                    y = BufferHeight - _viewHeight;

                if (x < 0)
                    x = 0;

                if (y < 0)
                    y = 0;

                _viewPosition = new Point(x, y);
                IsDirty = true;
            }
        }

        /// <summary>
        /// All cells of the surface.
        /// </summary>
        public ColoredGlyph[] Cells { get; protected set; }

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public ColoredGlyph this[int x, int y]
        {
            get => Cells[y * BufferWidth + x];
            protected set { Cells[y * BufferWidth + x] = value; IsDirty = true; }
        }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public ColoredGlyph this[int index]
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
        public CellSurface(int width, int height, ColoredGlyph[] initialCells)
        {
            DefaultForeground = Color.White;
            DefaultBackground = Color.Transparent;
            _viewWidth = BufferWidth = width;
            _viewHeight = BufferHeight = height;

            if (initialCells == null)
            {
                Cells = new ColoredGlyph[width * height];

                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new ColoredGlyph(DefaultForeground, DefaultBackground, 0);
                }
            }
            else
            {
                if (initialCells.Length != ViewWidth * ViewHeight)
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
        public CellSurface(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[] initialCells)
        {
            if (width > bufferWidth) throw new ArgumentOutOfRangeException(nameof(bufferWidth), "Buffer width must be less than or equal to the width.");
            if (height > bufferHeight) throw new ArgumentOutOfRangeException(nameof(bufferHeight), "Buffer height must be less than or equal to the height.");

            DefaultForeground = Color.White;
            DefaultBackground = Color.Transparent;
            _viewWidth = width;
            _viewHeight = height;
            BufferWidth = bufferWidth;
            BufferHeight = bufferHeight;

            if (initialCells == null)
            {
                Cells = new ColoredGlyph[bufferWidth * bufferWidth];

                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new ColoredGlyph(DefaultForeground, DefaultBackground, 0);
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
        protected virtual void OnDirtyChanged() =>
            DirtyChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Called when the <see cref="Cells"/> property is reset.
        /// </summary>
        protected virtual void OnCellsReset() { }

        /// <summary>
        /// Gets a rectangle representing the visible portion of the surface.
        /// </summary>
        /// <returns>A rectangle with only the visible area.</returns>
        public Rectangle GetViewRectangle() =>
            new Rectangle(ViewPosition.X, ViewPosition.Y, ViewWidth, ViewHeight);

        /// <summary>
        /// Gets an enumerator for <see cref="Cells"/>.
        /// </summary>
        /// <returns>An enumerator for <see cref="Cells"/>.</returns>
        public IEnumerator<ColoredGlyph> GetEnumerator() => ((IEnumerable<ColoredGlyph>)Cells).GetEnumerator();

        /// <summary>
        /// Gets an enumerator for <see cref="Cells"/>.
        /// </summary>
        /// <returns>An enumerator for <see cref="Cells"/>.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Cells.GetEnumerator();
    }
}
