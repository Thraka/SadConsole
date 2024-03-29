﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace SadConsole;

/// <summary>
/// An array of <see cref="ColoredGlyphBase"/> objects used to represent a 2D surface.
/// </summary>
[DataContract]
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class CellSurface : ICellSurface, ICellSurfaceResize, ICellSurfaceSettable
{
    private bool _isDirty = true;
    private Color _defaultBackground;
    private Color _defaultForeground;

    [DataMember(Name = "AreaBounds")]
    private BoundedRectangle _viewArea;

    /// <inheritdoc />
    public event EventHandler? IsDirtyChanged;

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

    /// <summary>
    /// Returns this object.
    /// </summary>
    [IgnoreDataMember]
    public ICellSurface Surface => this;

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
            if (_viewArea.Area == value) return;
            _viewArea.SetArea(value);
            IsDirty = true;
        }
    }

    /// <inheritdoc />
    public int ViewWidth
    {
        get => _viewArea.Area.Width;
        set
        {
            if (value == _viewArea.Area.Width) return;
            _viewArea.SetArea(_viewArea.Area.WithWidth(value));
            IsDirty = true;
        }
    }

    /// <inheritdoc />
    public int ViewHeight
    {
        get => _viewArea.Area.Height;
        set
        {
            if (value == _viewArea.Area.Height) return;
            _viewArea.SetArea(_viewArea.Area.WithHeight(value));
            IsDirty = true;
        }
    }

    /// <inheritdoc />
    public Rectangle Area => _viewArea.BoundingBox;

    /// <summary>
    /// The total width of the surface.
    /// </summary>
    public int Width => _viewArea.BoundingBox.Width;

    /// <summary>
    /// The total height of the surface.
    /// </summary>
    public int Height => _viewArea.BoundingBox.Height;

    /// <inheritdoc />
    public bool IsScrollable => Height != _viewArea.Area.Height || Width != _viewArea.Area.Width;

    /// <inheritdoc />
    public Point ViewPosition
    {
        get => _viewArea.Area.Position;
        set
        {
            if (value == _viewArea.Area.Position) return;
            _viewArea.SetArea(_viewArea.Area.WithPosition(value));
            IsDirty = true;
        }
    }

    /// <inheritdoc />
    [DataMember]
    public ColoredGlyphBase[] Cells { get; protected set; }

    /// <summary>
    /// The count of glyphs this surface contains.
    /// </summary>
    public int Count => Cells.Length;

    /// <inheritdoc />
    public ColoredGlyphBase this[int x, int y]
    {
        get => Cells[Point.ToIndex(x, y, Width)];
        protected set { Cells[Point.ToIndex(x, y, Width)] = value; IsDirty = true; }
    }

    /// <inheritdoc />
    public ColoredGlyphBase this[int index]
    {
        get => Cells[index];
        protected set { Cells[index] = value; IsDirty = true; }
    }

    /// <inheritdoc />
    public ColoredGlyphBase this[Point position]
    {
        get => Cells[position.ToIndex(Width)];
        protected set { Cells[position.ToIndex(Width)] = value; IsDirty = true; }
    }

    /// <summary>
    /// Given a position, returns the "value" associated with that location.
    /// </summary>
    /// <param name="range">The cells to return from the array.</param>
    /// <returns>The cells associated with the specified range.</returns>
    public Span<ColoredGlyphBase> this[Range range]
    {
        get => Cells.AsSpan(range);
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
    public CellSurface(int width, int height, ColoredGlyphBase[] initialCells) : this(width, height, width, height, initialCells)
    {

    }

    /// <summary>
    /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
    /// </summary>
    /// <param name="viewWidth">The visible width of the surface in cells.</param>
    /// <param name="viewHeight">The visible height of the surface in cells.</param>
    /// <param name="totalWidth">The total width of the surface in cells.</param>
    /// <param name="totalHeight">The total height of the surface in cells.</param>
    public CellSurface(int viewWidth, int viewHeight, int totalWidth, int totalHeight) : this(viewWidth, viewHeight, totalWidth, totalHeight, null)
    {

    }

    /// <summary>
    /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
    /// </summary>
    /// <param name="viewWidth">The width of the surface in cells.</param>
    /// <param name="viewHeight">The height of the surface in cells.</param>
    /// <param name="totalWidth">The total width of the surface in cells.</param>
    /// <param name="totalHeight">The total height of the surface in cells.</param>
    /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
    public CellSurface(int viewWidth, int viewHeight, int totalWidth, int totalHeight, ColoredGlyphBase[]? initialCells)
    {
        if (viewWidth <= 0) throw new ArgumentOutOfRangeException(nameof(viewWidth), "Surface view width must be > 0");
        if (viewHeight <= 0) throw new ArgumentOutOfRangeException(nameof(viewHeight), "Surface view height must be > 0");
        if (totalWidth <= 0) throw new ArgumentOutOfRangeException(nameof(totalWidth), "Surface buffer width must be > 0");
        if (totalHeight <= 0) throw new ArgumentOutOfRangeException(nameof(totalHeight), "Surface buffer height must be > 0");
        if (viewWidth > totalWidth) throw new ArgumentOutOfRangeException(nameof(totalWidth), "Buffer width must be less than or equal to the width.");
        if (viewHeight > totalHeight) throw new ArgumentOutOfRangeException(nameof(totalHeight), "Buffer height must be less than or equal to the height.");


        DefaultForeground = Color.White;
        DefaultBackground = Color.Transparent;

        _viewArea = new BoundedRectangle((0, 0, viewWidth, viewHeight),
                                         (0, 0, totalWidth, totalHeight));

        if (initialCells == null)
        {
            Cells = new ColoredGlyphBase[totalWidth * totalHeight];

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new ColoredGlyph(DefaultForeground, DefaultBackground, 0);
            }
        }
        else
        {
            if (initialCells.Length != totalWidth * totalHeight)
            {
                throw new Exception("Buffer Width * Buffer Height does not match initialCells.Length");
            }

            Cells = initialCells;
        }

        Effects = new Effects.EffectsManager(this);
    }


    /// <summary>
    /// Creates a new surface from a grid view with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
    /// </summary>
    /// <param name="surface">The surface to use as the source of cells.</param>
    /// <param name="visibleWidth">Optional view width. If <c>0</c>, the view width matches the width of the surface.</param>
    /// <param name="visibleHeight">Optional view height. If <c>0</c>, the view width matches the height of the surface.</param>
    public CellSurface(IGridView<ColoredGlyphBase> surface, int visibleWidth = 0, int visibleHeight = 0)
    {
        DefaultForeground = Color.White;
        DefaultBackground = Color.Transparent;

        if (visibleWidth == 0) visibleWidth = surface.Width;
        if (visibleHeight == 0) visibleHeight = surface.Height;

        _viewArea = new BoundedRectangle((0, 0, visibleWidth, visibleHeight),
                                         (0, 0, surface.Width, surface.Height));

        Cells = new ColoredGlyphBase[surface.Count];

        for (int i = 0; i < Cells.Length; i++)
            Cells[i] = surface[i];

        Effects = new Effects.EffectsManager(this);
    }

    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private CellSurface()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        Effects = new Effects.EffectsManager(this);
    }

    /// <inheritdoc />
    public void Resize(int viewWidth, int viewHeight, bool clear) =>
        Resize(viewWidth, viewHeight, viewWidth, viewHeight, clear);

    /// <inheritdoc />
    public void Resize(int viewWidth, int viewHeight, int bufferWidth, int bufferHeight, bool clear)
    {
        if (viewWidth <= 0) throw new ArgumentOutOfRangeException(nameof(viewWidth), "Surface view width must be > 0");
        if (viewHeight <= 0) throw new ArgumentOutOfRangeException(nameof(viewHeight), "Surface view height must be > 0");
        if (bufferWidth <= 0) throw new ArgumentOutOfRangeException(nameof(bufferWidth), "Surface buffer width must be > 0");
        if (bufferHeight <= 0) throw new ArgumentOutOfRangeException(nameof(bufferHeight), "Surface buffer height must be > 0");
        if (viewWidth > bufferWidth) throw new ArgumentOutOfRangeException(nameof(bufferWidth), "Buffer width must be less than or equal to the width.");
        if (viewHeight > bufferHeight) throw new ArgumentOutOfRangeException(nameof(bufferHeight), "Buffer height must be less than or equal to the height.");

        var newCells = new ColoredGlyphBase[bufferWidth * bufferHeight];

        for (int y = 0; y < bufferHeight; y++)
        {
            for (int x = 0; x < bufferWidth; x++)
            {
                int index = new Point(x, y).ToIndex(bufferWidth);

                if (this.IsValidCell(x, y))
                {
                    newCells[index] = this[x, y];

                    if (clear)
                        this.Clear(x, y);
                }
                else
                {
                    newCells[index] = new ColoredGlyph(DefaultForeground, DefaultBackground, 0);
                }
            }
        }

        Cells = newCells;
        _viewArea = new BoundedRectangle((0, 0, viewWidth, viewHeight),
                                         (0, 0, bufferWidth, bufferHeight));
        if (clear)
            Effects.RemoveAll();
        else
            Effects.DropInvalidCells();

        IsDirty = true;
        OnCellsReset();
    }

    /// <inheritdoc />
    public void SetSurface(ICellSurface surface, Rectangle view = default)
    {
        Rectangle rect = view == Rectangle.Empty ? new Rectangle(0, 0, surface.Width, surface.Height) : view;

        if (!new Rectangle(0, 0, surface.Width, surface.Height).Contains(rect))
        {
            throw new ArgumentOutOfRangeException(nameof(view), "The view is outside the bounds of the surface.");
        }

        _viewArea = new BoundedRectangle(new Rectangle(0, 0, rect.Width, rect.Height), new Rectangle(0, 0, rect.Width, rect.Height));
        Cells = new ColoredGlyphBase[rect.Width * rect.Height];

        int index = 0;

        for (int y = 0; y < rect.Height; y++)
        {
            for (int x = 0; x < rect.Width; x++)
            {
                Cells[index] = surface[(y + rect.Y) * surface.Width + (x + rect.X)];
                index++;
            }
        }

        IsDirty = true;
        OnCellsReset();
    }

    /// <inheritdoc />
    public void SetSurface(ColoredGlyphBase[] cells, int width, int height, int bufferWidth, int bufferHeight)
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
    public IEnumerator<ColoredGlyphBase> GetEnumerator() => ((IEnumerable<ColoredGlyphBase>)Cells).GetEnumerator();

    /// <summary>
    /// Gets an enumerator for <see cref="Cells"/>.
    /// </summary>
    /// <returns>An enumerator for <see cref="Cells"/>.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Cells.GetEnumerator();
}
