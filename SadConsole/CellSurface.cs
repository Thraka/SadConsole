using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An array of <see cref="ColoredGlyph"/> objects used to represent a 2D surface.
    /// </summary>
    [DataContract]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class CellSurface : IEnumerable<ColoredGlyph>
    {
        private bool _isDirty = true;
        private Color _defaultBackground;
        private Color _defaultForeground;

        [DataMember(Name = "AreaBounds")]
        private BoundedRectangle _viewArea;

        /// <summary>
        /// An event that is raised when <see cref="IsDirty"/> changes.
        /// </summary>
        public event EventHandler IsDirtyChanged;

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty == value) return;

                _isDirty = value;
                OnIsDirtyChanged();
            }
        }

        /// <summary>
        /// The default foreground for glyphs on this surface.
        /// </summary>
        [DataMember]
        public Color DefaultForeground
        {
            get => _defaultForeground;
            set { _defaultForeground = value; IsDirty = true; }
        }

        /// <summary>
        /// The default background for glyphs on this surface.
        /// </summary>
        [DataMember]
        public Color DefaultBackground
        {
            get => _defaultBackground;
            set { _defaultBackground = value; IsDirty = true; }
        }

        /// <summary>
        /// The default glyph used in clearing and erasing.
        /// </summary>
        [DataMember]
        public int DefaultGlyph { get; set; }

        /// <summary>
        /// The view presented by the surface.
        /// </summary>
        public Rectangle View
        {
            get => _viewArea.Area;
            set
            {
                _viewArea.SetArea(value);
                IsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the visible width of the surface in cells.
        /// </summary>
        public int ViewWidth
        {
            get => _viewArea.Area.Width;
            set => _viewArea.SetArea(_viewArea.Area.WithWidth(value));
        }

        /// <summary>
        /// Gets or sets the visible height of the surface in cells.
        /// </summary>
        public int ViewHeight
        {
            get => _viewArea.Area.Height;
            set => _viewArea.SetArea(_viewArea.Area.WithHeight(value));
        }

        /// <summary>
        /// Returns a rectangle that represents the size of the buffer.
        /// </summary>
        public Rectangle Buffer => _viewArea.BoundingBox;

        /// <summary>
        /// Width of the surface buffer.
        /// </summary>
        public int BufferWidth => _viewArea.BoundingBox.Width;

        /// <summary>
        /// Height of the surface buffer.
        /// </summary>
        public int BufferHeight => _viewArea.BoundingBox.Height;

        /// <summary>
        /// Returns <see langword="true"/> when the <see cref="CellSurface.View"/> width or height is different from <see cref="BufferHeight"/> or <see cref="BufferWidth"/>.
        /// </summary>
        public bool IsScrollable => BufferHeight != _viewArea.Area.Height || BufferWidth != _viewArea.Area.Width;

        /// <summary>
        /// The position of the buffer.
        /// </summary>
        public Point ViewPosition
        {
            get => _viewArea.Area.Position;
            set
            {
                _viewArea.SetArea(_viewArea.Area.WithPosition(value));
                IsDirty = true;
            }
        }

        /// <summary>
        /// All cells of the surface.
        /// </summary>
        [DataMember]
        public ColoredGlyph[] Cells { get; protected set; }

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public ColoredGlyph this[int x, int y]
        {
            get => Cells[Point.ToIndex(x, y, BufferWidth)];
            protected set { Cells[Point.ToIndex(x, y, BufferWidth)] = value; IsDirty = true; }
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
        public CellSurface(int width, int height) : this(width, height, width, height, null)
        {

        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the surface in cells.</param>
        /// <param name="height">The height of the surface in cells.</param>
        /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
        public CellSurface(int width, int height, ColoredGlyph[] initialCells): this(width, height, width, height, initialCells)
        {

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
            if (width == 0) throw new ArgumentOutOfRangeException(nameof(width), "Surface view width must be > 0");
            if (height == 0) throw new ArgumentOutOfRangeException(nameof(height), "Surface view height must be > 0");
            if (bufferWidth == 0) throw new ArgumentOutOfRangeException(nameof(bufferWidth), "Surface buffer width must be > 0");
            if (bufferHeight == 0) throw new ArgumentOutOfRangeException(nameof(bufferHeight), "Surface buffer height must be > 0");
            if (width > bufferWidth) throw new ArgumentOutOfRangeException(nameof(bufferWidth), "Buffer width must be less than or equal to the width.");
            if (height > bufferHeight) throw new ArgumentOutOfRangeException(nameof(bufferHeight), "Buffer height must be less than or equal to the height.");


            DefaultForeground = Color.White;
            DefaultBackground = Color.Transparent;

            _viewArea = new BoundedRectangle((0, 0, width, height),
                                             (0, 0, bufferWidth, bufferHeight));

            if (initialCells == null)
            {
                Cells = new ColoredGlyph[bufferWidth * bufferHeight];

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

            Effects = new Effects.EffectsManager(this);
        }

        [JsonConstructor]
        private CellSurface()
        {
            Effects = new Effects.EffectsManager(this);
        }


        /// <summary>
        /// Sets <see cref="IsDirty"/> to <see langword="true"/> without triggering <see cref="OnIsDirtyChanged"/>.
        /// </summary>
        protected void SetIsDirtySafe() =>
            _isDirty = true;

        /// <summary>
        /// Called when the <see cref="IsDirty"/> property changes.
        /// </summary>
        protected virtual void OnIsDirtyChanged() =>
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Called when the <see cref="Cells"/> property is reset.
        /// </summary>
        protected virtual void OnCellsReset() { }

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
