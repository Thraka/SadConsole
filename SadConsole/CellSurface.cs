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
    public class CellSurface : IEnumerable<ColoredGlyph>, ICellSurface
    {
        private bool _isDirty = true;
        private Color _defaultBackground;
        private Color _defaultForeground;

        [DataMember(Name = "AreaBounds")]
        private BoundedRectangle _viewArea;

        /// <inheritdoc />
        public event EventHandler IsDirtyChanged;

        /// <inheritdoc />
        [DataMember]
        public int TimesShiftedDown { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int TimesShiftedRight { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int TimesShiftedLeft { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int TimesShiftedUp { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool UsePrintProcessor { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        public Effects.EffectsManager Effects { get; protected set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        [DataMember]
        public Color DefaultForeground
        {
            get => _defaultForeground;
            set { _defaultForeground = value; IsDirty = true; }
        }

        /// <inheritdoc />
        [DataMember]
        public Color DefaultBackground
        {
            get => _defaultBackground;
            set { _defaultBackground = value; IsDirty = true; }
        }

        /// <inheritdoc />
        [DataMember]
        public int DefaultGlyph { get; set; }

        /// <inheritdoc />
        public Rectangle View
        {
            get => _viewArea.Area;
            set
            {
                _viewArea.SetArea(value);
                IsDirty = true;
            }
        }

        /// <inheritdoc />
        public int ViewWidth
        {
            get => _viewArea.Area.Width;
            set => _viewArea.SetArea(_viewArea.Area.WithWidth(value));
        }

        /// <inheritdoc />
        public int ViewHeight
        {
            get => _viewArea.Area.Height;
            set => _viewArea.SetArea(_viewArea.Area.WithHeight(value));
        }

        /// <inheritdoc />
        public Rectangle Buffer => _viewArea.BoundingBox;

        /// <inheritdoc />
        public int BufferWidth => _viewArea.BoundingBox.Width;

        /// <inheritdoc />
        public int BufferHeight => _viewArea.BoundingBox.Height;

        /// <inheritdoc />
        public bool IsScrollable => BufferHeight != _viewArea.Area.Height || BufferWidth != _viewArea.Area.Width;

        /// <inheritdoc />
        public Point ViewPosition
        {
            get => _viewArea.Area.Position;
            set
            {
                _viewArea.SetArea(_viewArea.Area.WithPosition(value));
                IsDirty = true;
            }
        }

        /// <inheritdoc />
        [DataMember]
        public ColoredGlyph[] Cells { get; protected set; }

        /// <inheritdoc />
        public ColoredGlyph this[int x, int y]
        {
            get => Cells[Point.ToIndex(x, y, BufferWidth)];
            protected set { Cells[Point.ToIndex(x, y, BufferWidth)] = value; IsDirty = true; }
        }

        /// <inheritdoc />
        public ColoredGlyph this[int index]
        {
            get => Cells[index];
            protected set => Cells[index] = value;
        }

        /// <inheritdoc />
        public ColoredGlyph this[Point position]
        {
            get => Cells[position.ToIndex(BufferWidth)];
            protected set => Cells[position.ToIndex(BufferWidth)] = value;
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
        public CellSurface(int width, int height, ColoredGlyph[] initialCells) : this(width, height, width, height, initialCells)
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
        
        /// <inheritdoc />
        public void Resize(int width, int height, int bufferWidth, int bufferHeight, bool clear)
        {
            var newCells = new ColoredGlyph[bufferWidth * bufferHeight];

            for (int y = 0; y < bufferHeight; y++)
            {
                for (int x = 0; x < bufferWidth; x++)
                {
                    int index = new Point(x, y).ToIndex(bufferWidth);

                    if (this.IsValidCell(x, y))
                    {
                        newCells[index] = this[x, y];

                        if (clear)
                        {
                            newCells[index].Foreground = DefaultForeground;
                            newCells[index].Background = DefaultBackground;
                            newCells[index].Glyph = 0;
                            newCells[index].Mirror = Mirror.None;
                        }
                    }
                    else
                    {
                        newCells[index] = new ColoredGlyph(DefaultForeground, DefaultBackground, 0);
                    }
                }
            }

            Cells = newCells;
            _viewArea = new BoundedRectangle((0, 0, width, height),
                                             (0, 0, bufferWidth, bufferHeight));
            Effects = new Effects.EffectsManager(this);
            IsDirty = true;
            OnCellsReset();
        }

        /// <inheritdoc />
        public ICellSurface GetSubSurface(Rectangle view)
        {
            if (!new Rectangle(0, 0, BufferWidth, BufferHeight).Contains(view))
            {
                throw new Exception("View is outside of surface bounds.");
            }

            var cells = new ColoredGlyph[view.Width * view.Height];

            int index = 0;

            for (int y = 0; y < view.Height; y++)
            {
                for (int x = 0; x < view.Width; x++)
                {
                    cells[index] = this[x + view.X, y + view.Y];
                    index++;
                }
            }

            return new CellSurface(view.Width, view.Height, cells);
        }

        /// <inheritdoc />
        public void SetSurface(in ICellSurface surface, Rectangle view = default)
        {
            Rectangle rect = view == Rectangle.Empty ? new Rectangle(0, 0, surface.BufferWidth, surface.BufferHeight) : view;

            if (!new Rectangle(0, 0, surface.BufferWidth, surface.BufferHeight).Contains(rect))
            {
                throw new ArgumentOutOfRangeException(nameof(view), "The view is outside the bounds of the surface.");
            }

            _viewArea = new BoundedRectangle(new Rectangle(0, 0, rect.Width, rect.Height), new Rectangle(0, 0, rect.Width, rect.Height));
            Cells = new ColoredGlyph[rect.Width * rect.Height];

            int index = 0;

            for (int y = 0; y < rect.Height; y++)
            {
                for (int x = 0; x < rect.Width; x++)
                {
                    Cells[index] = surface.Cells[(y + rect.Y) * surface.BufferWidth + (x + rect.X)];
                    index++;
                }
            }

            IsDirty = true;
            OnCellsReset();
        }

        /// <inheritdoc />
        public void SetSurface(in ColoredGlyph[] cells, int width, int height, int bufferWidth, int bufferHeight)
        {
            if (cells.Length != bufferWidth * bufferHeight) throw new Exception("buffer width * buffer height must match the amount of cells.");

            _viewArea = new BoundedRectangle((0, 0, width, height),
                                             (0, 0, bufferWidth, bufferHeight));
            Cells = cells;

            IsDirty = true;
            OnCellsReset();
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
