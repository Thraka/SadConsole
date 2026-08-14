using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// A scrollable table control.
/// </summary>
public partial class Table : CompositeControl
{
    private bool _useMouse = true;

    /// <summary>
    /// The cells collection used to modify the table cells
    /// </summary>
    public TableCells Cells { get; }

    /// <summary>
    /// The default foreground color used for the table foreground and newly created cells
    /// </summary>
    public Color DefaultForeground { get; set; }

    /// <summary>
    /// The default background color used for the table background and newly created cells
    /// </summary>
    public Color DefaultBackground { get; set; }

    /// <summary>
    /// The default size a cell gets when it is newly created
    /// </summary>
    public Point DefaultCellSize
    {
        get => _defaultCellSize;
        set
        {
            if (_defaultCellSize == value) return;

            _defaultCellSize = value;
            Cells.AdjustCellPositionsAfterResize();
            SyncScrollAmountOnResize();
            _checkScrollBarVisibility = true;
            IsDirty = true;
        }
    }
    private Point _defaultCellSize;

    /// <summary>
    /// The default visual hovering mode when hovering over cells
    /// </summary>
    public TableCells.Layout.Mode DefaultHoverMode { get; set; }

    /// <summary>
    /// The default visual selection mode when selecting a cell
    /// </summary>
    public TableCells.Layout.Mode DefaultSelectionMode { get; set; }


    /// <summary>
    /// When <see langword="true"/>, this object will use the mouse; otherwise <see langword="false"/>.
    /// </summary>
    public new bool UseMouse
    {
        get => _useMouse;
        set
        {
            _useMouse = value;
            if (!_useMouse)
            {
                SelectedCell = null;
                CurrentMouseCell = null;
                IsDirty = true;
            }
        }
    }

    /// <summary>
    /// Returns the cell the mouse is over, if <see cref="UseMouse"/> is <see langword="true"/>.
    /// </summary>
    public Cell? CurrentMouseCell { get; private set; }

    private Cell? _selectedCell;

    /// <summary>
    /// Returns the current selected cell
    /// </summary>
    public Cell? SelectedCell
    {
        get => _selectedCell;
        internal set
        {
            Cell? prev = _selectedCell;
            _selectedCell = value;
            if (prev != _selectedCell)
            {
                SelectedCellChanged?.Invoke(this, new CellChangedEventArgs(prev, _selectedCell));
                IsDirty = true;
            }
        }
    }

    /// <summary>
    /// By default, only cells that have been modified in anyway will be rendered on the table control.
    /// Turn this off, if the whole table should draw as many cells as it fits with their default layout.
    /// </summary>
    public bool DrawFakeCells { get; set; } = false;

    /// <summary>
    /// Fires an event when a cell is entered by the mouse.
    /// </summary>
    public event EventHandler<CellEventArgs>? OnCellEnter;

    /// <summary>
    /// Fires an event when a cell is exited by the mouse.
    /// </summary>
    public event EventHandler<CellEventArgs>? OnCellExit;

    /// <summary>
    /// Fires an event when the selected cell has changed.
    /// </summary>
    public event EventHandler<CellChangedEventArgs>? SelectedCellChanged;

    /// <summary>
    /// Fires an event when a cell is left clicked.
    /// </summary>
    public event EventHandler<CellEventArgs>? OnCellLeftClick;

    /// <summary>
    /// Fires an event when a cell is right clicked.
    /// </summary>
    public event EventHandler<CellEventArgs>? OnCellRightClick;

    /// <summary>
    /// Fires an event when a cell is double clicked.
    /// </summary>
    public event EventHandler<CellEventArgs>? OnCellDoubleClick;

    /// <summary>
    /// Called when a fake cells is being drawn, you can use this to modify the cell layout.
    /// </summary>
    public event EventHandler<CellEventArgs>? OnDrawFakeCell;

    /// <summary>
    /// The vertical scrollbar, automatically configured by the table.
    /// </summary>
    public ScrollBar? VerticalScrollBar { get; private set; }

    /// <summary>
    /// The horizontal scrollbar, automatically configured by the table.
    /// </summary>
    public ScrollBar? HorizontalScrollBar { get; private set; }

    /// <summary>
    /// Returns true if the vertical scroll bar is currently visible.
    /// </summary>
    public bool IsVerticalScrollBarVisible
    {
        get => VerticalScrollBar != null && VerticalScrollBar.IsVisible;
        internal set
        {
            if (VerticalScrollBar == null) return;
            VerticalScrollBar.IsVisible = value;
        }
    }
    /// <summary>
    /// Returns true if the horizontal scroll bar is currently visible.
    /// </summary>
    public bool IsHorizontalScrollBarVisible
    {
        get => HorizontalScrollBar != null && HorizontalScrollBar.IsVisible;
        internal set
        {
            if (HorizontalScrollBar == null) return;
            HorizontalScrollBar.IsVisible = value;
        }
    }

    /// <summary>
    /// The updated maximum value, incase the scrollbar object's maximum value is not yet updated by the theme.
    /// </summary>
    public int VerticalScrollBarMaximum
    {
        get
        {
            if (VerticalScrollBar == null) return 0;
            UpdateScrollBarMaximum(Orientation.Vertical);
            return VerticalScrollBar.MaximumValue;
        }
    }

    /// <summary>
    /// The updated maximum value, incase the scrollbar object's maximum value is not yet updated by the theme.
    /// </summary>
    public int HorizontalScrollBarMaximum
    {
        get
        {
            if (HorizontalScrollBar == null) return 0;
            UpdateScrollBarMaximum(Orientation.Horizontal);
            return HorizontalScrollBar.MaximumValue;
        }
    }

    /// <summary>
    /// By default the table will automatically scroll to the selected cell if possible.
    /// </summary>
    public bool AutoScrollOnCellSelection { get; set; } = true;

    /// <summary>
    /// The total rows visible in the table.
    /// </summary>
    internal int VisibleRowsTotal { get; set; }

    /// <summary>
    /// The maximum amount of rows that can be shown in the table.
    /// </summary>
    internal int VisibleRowsMax { get; set; }

    /// <summary>
    /// The total columns visible in the table.
    /// </summary>
    internal int VisibleColumnsTotal { get; set; }

    /// <summary>
    /// The maximum amount of columns that can be shown in the table.
    /// </summary>
    internal int VisibleColumnsMax { get; set; }

    private DateTime _leftMouseLastClick = DateTime.Now;
    private Point? _leftMouseLastClickPosition;
    internal bool _checkScrollBarVisibility;
    private bool _automaticallyManageHorizontalScrollBar = true;
    private bool _automaticallyManageVerticalScrollBar = true;

    private int EffectiveViewportWidth =>
        Math.Max(Width - (_automaticallyManageVerticalScrollBar && IsVerticalScrollBarVisible ? 1 : 0), 0);

    private int EffectiveViewportHeight =>
        Math.Max(Height - (_automaticallyManageHorizontalScrollBar && IsHorizontalScrollBarVisible ? 1 : 0), 0);

    private int _previousScrollValueVertical, _previousScrollValueHorizontal;
    private void ScrollBar_ValueChanged(object? sender, EventArgs e)
    {
        var scrollBar = (ScrollBar?)sender;
        if (scrollBar == null) return;
        int previousScrollValue = scrollBar.Orientation == Orientation.Vertical ? _previousScrollValueVertical : _previousScrollValueHorizontal;
        bool increment = previousScrollValue < scrollBar.Value;

        int diff = Math.Abs(scrollBar.Value - previousScrollValue);
        for (int i = 0; i < diff; i++)
        {
            SetScrollAmount(scrollBar.Orientation, increment);
            Cells.AdjustCellPositionsAfterResize();
        }

        if (scrollBar.Orientation == Orientation.Vertical)
            _previousScrollValueVertical = scrollBar.Value;
        else
            _previousScrollValueHorizontal = scrollBar.Value;

        IsDirty = true;
    }

    /// <summary>
    /// Creates a new table with the default SadConsole colors, and cell size of (1 width, 1 height)
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public Table(int width, int height) : base(width, height)
    {
        Cells = new TableCells(this);
        DefaultForeground = Color.White;
        DefaultBackground = Color.TransparentBlack;
        DefaultCellSize = new Point(1, 1);
        EnsureAutomaticScrollBars();
        UpdateAutomaticScrollBarGeometry();
    }

    /// <summary>
    /// Creates a new table with custom cell width and cell height params; Uses the default SadConsole colors
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="cellWidth"></param>
    /// <param name="cellHeight"></param>
    public Table(int width, int height, int cellWidth, int cellHeight = 1)
        : this(width, height)
    {
        DefaultCellSize = new Point(cellWidth, cellHeight);
    }

    /// <summary>
    /// Creates a new table with extra params to set the base default values of the table
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="cellWidth"></param>
    /// <param name="defaultForeground"></param>
    /// <param name="defaultBackground"></param>
    /// <param name="cellHeight"></param>
    public Table(int width, int height, int cellWidth, Color defaultForeground, Color defaultBackground, int cellHeight = 1)
        : this(width, height, cellWidth, cellHeight)
    {
        DefaultForeground = defaultForeground;
        DefaultBackground = defaultBackground;
    }

    /// <summary>
    /// Called when a fake cell is being drawn, this fake cell cannot be added to the table when it is modified.
    /// This method can only be used to modify the cell layout when drawn, and thus will not count as a new cell within the table.
    /// </summary>
    /// <param name="cell"></param>
    internal void DrawFakeCell(Cell cell)
    {
        OnDrawFakeCell?.Invoke(this, new CellEventArgs(cell));
    }

    /// <summary>
    /// Reconfigures the scrollbar for the specified orientation.
    /// </summary>
    /// <param name="orientation">The orientation of the scrollbar.</param>
    /// <param name="size"></param>
    /// <param name="position">The position of the scrollbar.</param>
    public void SetupScrollBar(Orientation orientation, int size, Point position) =>
        SetupScrollBar(orientation, size, position, false);

    private void SetupScrollBar(Orientation orientation, int size, Point position, bool automaticallyManageGeometry)
    {
        bool scrollBarExists = false;
        int value = 0;
        int max = 0;

        ScrollBar? existingScrollBar = orientation == Orientation.Vertical ? VerticalScrollBar : HorizontalScrollBar;
        if (existingScrollBar != null)
        {
            existingScrollBar.ValueChanged -= ScrollBar_ValueChanged;
            value = existingScrollBar.Value;
            max = existingScrollBar.MaximumValue;
            RemoveControl(existingScrollBar);
            scrollBarExists = true;
        }

        existingScrollBar = new ScrollBar(orientation, size);

        if (automaticallyManageGeometry)
            existingScrollBar.IsVisible = false;

        if (scrollBarExists)
        {
            existingScrollBar.MaximumValue = max;
            existingScrollBar.Value = value;
        }

        existingScrollBar.ValueChanged += ScrollBar_ValueChanged;
        existingScrollBar.Position = position;
        AddControl(existingScrollBar);

        if (orientation == Orientation.Vertical)
        {
            VerticalScrollBar = existingScrollBar;
            _automaticallyManageVerticalScrollBar = automaticallyManageGeometry;
        }
        else
        {
            HorizontalScrollBar = existingScrollBar;
            _automaticallyManageHorizontalScrollBar = automaticallyManageGeometry;
        }

        _checkScrollBarVisibility = true;

        DetermineState();
        UpdateAutomaticScrollBarGeometry();
    }

    private void EnsureAutomaticScrollBars()
    {
        if (_automaticallyManageHorizontalScrollBar && HorizontalScrollBar == null && Width >= 2)
            SetupScrollBar(Orientation.Horizontal, Width, new Point(0, Height - 1), true);

        if (_automaticallyManageVerticalScrollBar && VerticalScrollBar == null && Height >= 2)
            SetupScrollBar(Orientation.Vertical, Height, new Point(Width - 1, 0), true);
    }

    private void RemoveAutomaticScrollBar(Orientation orientation)
    {
        ScrollBar? scrollBar = orientation == Orientation.Horizontal ? HorizontalScrollBar : VerticalScrollBar;
        if (scrollBar == null)
            return;

        scrollBar.ValueChanged -= ScrollBar_ValueChanged;
        RemoveControl(scrollBar);

        if (orientation == Orientation.Horizontal)
        {
            HorizontalScrollBar = null;
            StartRenderXPos = 0;
            _previousScrollValueHorizontal = 0;
        }
        else
        {
            VerticalScrollBar = null;
            StartRenderYPos = 0;
            _previousScrollValueVertical = 0;
        }
    }

    private void UpdateAutomaticScrollBarGeometry()
    {
        bool reserveCorner = IsHorizontalScrollBarVisible && IsVerticalScrollBarVisible;
        int horizontalWidth = reserveCorner ? Width - 1 : Width;
        int verticalHeight = reserveCorner ? Height - 1 : Height;

        if (_automaticallyManageHorizontalScrollBar && HorizontalScrollBar != null && horizontalWidth < 2)
            RemoveAutomaticScrollBar(Orientation.Horizontal);

        if (_automaticallyManageVerticalScrollBar && VerticalScrollBar != null && verticalHeight < 2)
            RemoveAutomaticScrollBar(Orientation.Vertical);

        reserveCorner = IsHorizontalScrollBarVisible && IsVerticalScrollBarVisible;

        if (_automaticallyManageHorizontalScrollBar && HorizontalScrollBar != null)
        {
            horizontalWidth = reserveCorner ? Width - 1 : Width;
            if (HorizontalScrollBar.Width != horizontalWidth)
                HorizontalScrollBar.Resize(horizontalWidth, 1);

            HorizontalScrollBar.Position = new Point(0, Height - 1);
        }

        if (_automaticallyManageVerticalScrollBar && VerticalScrollBar != null)
        {
            verticalHeight = reserveCorner ? Height - 1 : Height;
            if (VerticalScrollBar.Height != verticalHeight)
                VerticalScrollBar.Resize(1, verticalHeight);

            VerticalScrollBar.Position = new Point(Width - 1, 0);
        }
    }

    /// <inheritdoc />
    protected override void OnResized()
    {
        base.OnResized();
        EnsureAutomaticScrollBars();
        UpdateAutomaticScrollBarGeometry();
        _checkScrollBarVisibility = true;
    }

    internal HashSet<int> GetIndexesWithContent(TableCells.Layout.LayoutType indexType)
    {
        if (Cells.Count == 0)
            return new HashSet<int>();

        HashSet<int> indexes = new();

        if (indexType == TableCells.Layout.LayoutType.Row)
            foreach (Cell item in Cells)
                indexes.Add(item.Row);
        else
            foreach (Cell item in Cells)
                indexes.Add(item.Column);

        return indexes;
    }

    internal int GetMaxRowsBasedOnRowSizes()
    {
        if (Cells.Count == 0) return 0;

        return Cells
            .GroupBy(a => a.Row)
            .Select(a => Cells.GetSizeOrDefault(a.Key, TableCells.Layout.LayoutType.Row))
            .Sum();
    }

    internal int GetMaxColumnsBasedOnColumnSizes()
    {
        if (Cells.Count == 0) return 0;

        return Cells
            .GroupBy(a => a.Column)
            .Select(a => Cells.GetSizeOrDefault(a.Key, TableCells.Layout.LayoutType.Column))
            .Sum();
    }

    private void UpdateScrollBarMaximum(Orientation orientation)
    {
        ScrollBar? scrollBar = orientation == Orientation.Horizontal ? HorizontalScrollBar : VerticalScrollBar;
        if (scrollBar != null)
        {
            int scrollItems = GetScrollBarItems(orientation);
            if (ReconcileScrollBarState(scrollBar, scrollBar.IsVisible, scrollItems))
                SynchronizeScrollOffsetsFromValues();
        }
    }

    internal int GetScrollBarItems(Orientation orientation)
    {
        int maxSize = orientation == Orientation.Vertical ? EffectiveViewportHeight : EffectiveViewportWidth;
        return GetScrollBarItems(orientation, maxSize);
    }

    private int GetScrollBarItems(Orientation orientation, int maxSize)
    {
        IEnumerable<IGrouping<int, Cell>> indexes = orientation == Orientation.Vertical ?
            Cells.GroupBy(a => a.Row) : Cells.GroupBy(a => a.Column);
        IOrderedEnumerable<IGrouping<int, Cell>> orderedIndex = indexes.OrderBy(a => a.Key);

        TableCells.Layout.LayoutType layoutType = orientation == Orientation.Vertical ? TableCells.Layout.LayoutType.Row : TableCells.Layout.LayoutType.Column;
        int totalSize = 0;
        int items = 0;
        foreach (IGrouping<int, Cell> index in orderedIndex)
        {
            int size = Cells.GetSizeOrDefault(index.Key, layoutType);
            if (IsEntireRowOrColumnNotVisible(index.Key, layoutType))
                continue;

            totalSize += size;

            if (totalSize > maxSize)
            {
                items++;
            }
        }
        return items;
    }

    /// <summary>
    /// Shows the scroll bar when there are too many items to display; otherwise, hides it.
    /// </summary>
    /// <param name="scrollBar"></param>
    internal bool ShowHideScrollBar(ScrollBar scrollBar)
    {
        int scrollbarItems = GetScrollBarItems(scrollBar.Orientation);
        bool isVisible = scrollbarItems > 0;
        ReconcileScrollBarState(scrollBar, isVisible, scrollbarItems);
        return isVisible;
    }

    private bool ReconcileScrollBarState(ScrollBar scrollBar, bool isVisible, int scrollItems)
    {
        int oldMaximum = scrollBar.MaximumValue;
        int oldValue = scrollBar.Value;
        int maximum = Math.Max(scrollItems, 0);
        int value = isVisible ? Math.Min(oldValue, maximum) : 0;

        scrollBar.ValueChanged -= ScrollBar_ValueChanged;
        scrollBar.MaximumValue = maximum;
        scrollBar.Value = value;
        scrollBar.ValueChanged += ScrollBar_ValueChanged;

        if (scrollBar.Orientation == Orientation.Vertical)
            _previousScrollValueVertical = scrollBar.Value;
        else
            _previousScrollValueHorizontal = scrollBar.Value;

        return oldMaximum != scrollBar.MaximumValue || oldValue != scrollBar.Value;
    }

    internal bool IsEntireRowOrColumnNotVisible(int index, TableCells.Layout.LayoutType type)
    {
        Cells._hiddenIndexes.TryGetValue(type, out HashSet<int>? indexes);
        return indexes != null && indexes.Contains(index);
    }

    /// <summary>
    /// Scrolls the list to the item currently selected.
    /// </summary>
    public void ScrollToSelectedItem()
    {
        if (!AutoScrollOnCellSelection) return;
        if (IsVerticalScrollBarVisible || IsHorizontalScrollBarVisible)
        {
            ScrollBar?[] scrollBars = new[] { VerticalScrollBar, HorizontalScrollBar };
            foreach (ScrollBar? scrollBar in scrollBars)
            {
                if (scrollBar == null) continue;

                Orientation orientation = scrollBar.Orientation;
                int selectedIndex = SelectedCell != null ? (orientation == Orientation.Vertical ? SelectedCell.Row : SelectedCell.Column) : 0;

                bool isRowType = orientation == Orientation.Vertical;
                IOrderedEnumerable<int> indexes = Cells
                    .GroupBy(a => isRowType ? a.Row : a.Column)
                    .Select(a => a.Key)
                    .OrderBy(a => a);
                int totalIndexSize = 0;
                foreach (int index in indexes)
                {
                    int cellSize = Cells.GetSizeOrDefault(index, isRowType ?
                        TableCells.Layout.LayoutType.Row : TableCells.Layout.LayoutType.Column);
                    totalIndexSize += cellSize;

                    if (index > selectedIndex)
                        break;
                }

                int maxIndexSize = orientation == Orientation.Vertical ? GetMaxRowsBasedOnRowSizes() : GetMaxColumnsBasedOnColumnSizes();
                int max = orientation == Orientation.Vertical ? VisibleRowsMax : VisibleColumnsMax;
                int total = orientation == Orientation.Vertical ? VisibleRowsTotal : VisibleColumnsTotal;
                int defaultIndexSize = orientation == Orientation.Vertical ? DefaultCellSize.Y : DefaultCellSize.X;

                int indexSize = (totalIndexSize - total) / defaultIndexSize;
                scrollBar.Value = totalIndexSize < max ? 0 : indexSize > scrollBar.MaximumValue ? scrollBar.MaximumValue : indexSize;
            }
        }
    }

    /// <summary>
    /// When a cell is resized after the bar has been scrolled, it must be updated with the new values for the rendering.
    /// </summary>
    internal void SyncScrollAmountOnResize()
    {
        SynchronizeScrollOffsetsFromValues();
    }

    private void SynchronizeScrollOffsetsFromValues()
    {
        StartRenderYPos = 0;
        StartRenderXPos = 0;

        Cells.AdjustCellPositionsAfterResize();

        int amountVertical = IsVerticalScrollBarVisible && VerticalScrollBar != null ? VerticalScrollBar.Value : 0;
        int amountHorizontal = IsHorizontalScrollBarVisible && HorizontalScrollBar != null ? HorizontalScrollBar.Value : 0;
        int max = amountVertical > amountHorizontal ? amountVertical : amountHorizontal;

        for (int i = 0; i < max; i++)
        {
            if (i < amountVertical)
                SetScrollAmount(Orientation.Vertical, true);
            if (i < amountHorizontal)
                SetScrollAmount(Orientation.Horizontal, true);
            Cells.AdjustCellPositionsAfterResize();
        }

        _previousScrollValueVertical = amountVertical;
        _previousScrollValueHorizontal = amountHorizontal;
    }

    /// <summary>
    /// The row the rendering should start at
    /// </summary>
    internal int StartRenderYPos { get; private set; }
    /// <summary>
    /// The column the rendering should start at
    /// </summary>
    internal int StartRenderXPos { get; private set; }

    private void SetScrollAmount(Orientation orientation, bool increment)
    {
        int scrollPos = GetNextScrollPos(increment, orientation);

        if (orientation == Orientation.Vertical)
            StartRenderYPos += scrollPos;
        else
            StartRenderXPos += scrollPos;

        StartRenderYPos = StartRenderYPos < 0 ? 0 : StartRenderYPos;
        StartRenderXPos = StartRenderXPos < 0 ? 0 : StartRenderXPos;
    }

    internal int GetNextScrollPos(bool increment, Orientation orientation)
    {
        TableCells.Layout.LayoutType type =
            orientation == Orientation.Vertical
            ? TableCells.Layout.LayoutType.Row
            : TableCells.Layout.LayoutType.Column;

        bool isRowType = type == TableCells.Layout.LayoutType.Row;
        int viewportSize = isRowType ? EffectiveViewportHeight : EffectiveViewportWidth;

        IEnumerable<IGrouping<int, Cell>> cellGroups = Cells.GroupBy(a => isRowType ? a.Row : a.Column);

        IOrderedEnumerable<IGrouping<int, Cell>> orderedCells =
            increment
            ? cellGroups.OrderBy(a => a.Key)
            : cellGroups.OrderByDescending(a => a.Key);

        foreach (IGrouping<int, Cell> group in orderedCells)
        {
            foreach (Cell cell in group)
            {
                bool partialOverlap = false;
                int indexSizeCell = isRowType ? cell._position.Y : cell._position.X;
                if (!increment)
                {
                    // Check if cell position is the last cell on screen
                    if (indexSizeCell >= viewportSize)
                        break;
                }
                else
                {
                    // Check if cell position is the next off-screen
                    // >= because it assumes the cell starts at Height, and thats off screen
                    bool isPositionOfScreen = indexSizeCell >= viewportSize;
                    if (!isPositionOfScreen)
                    {
                        // Here it is only > because if the real cell pos is 20 its the ending, so where the next cell starts
                        // which means its not off screen
                        int realCellPosition = isRowType ? (cell._position.Y + cell.Height) : (cell._position.X + cell.Width);

                        if (realCellPosition > viewportSize)
                            partialOverlap = true;
                        else
                            break;
                    }
                }

                // Get size of current cell
                Dictionary<int, TableCells.Layout> layoutDict = isRowType ? Cells._rowLayout : Cells._columnLayout;
                int defaultSize = isRowType ? DefaultCellSize.Y : DefaultCellSize.X;
                int offScreenIndex = isRowType ? cell.Row : cell.Column;
                int cellSize = layoutDict.TryGetValue(offScreenIndex, out TableCells.Layout? layout) ?
                    layout.Size : defaultSize;

                // Calculate the overlap amount
                if (partialOverlap)
                {
                    int overlapAmount = indexSizeCell + cellSize - viewportSize;
                    cellSize = overlapAmount;
                }

                return increment ? cellSize : -cellSize;
            }
        }

        int defaultCellSize = isRowType ? DefaultCellSize.Y : DefaultCellSize.X;

        return increment ? defaultCellSize : -defaultCellSize;
    }

    /// <inheritdoc/>
    protected override void OnMouseIn(ControlMouseState state)
    {
        base.OnMouseIn(state);

        // Handle mouse hovering over cell
        Point? mousePosCellIndex = GetCellIndexByMousePosition(state.MousePosition);
        Point? currentPosition = CurrentMouseCell == null ? (Point?)null : (CurrentMouseCell.Column, CurrentMouseCell.Row);

        if (!Equals(mousePosCellIndex, currentPosition))
        {
            if (mousePosCellIndex != null)
            {
                Cell? cell = Cells.GetIfExists(mousePosCellIndex.Value.Y, mousePosCellIndex.Value.X, true);

                if (cell == null && DrawFakeCells)
                {
                    // A fake cell doesn't know if it should be selected if the row is hidden
                    cell = Cell.InternalCreate(mousePosCellIndex.Value.Y, mousePosCellIndex.Value.X, this, string.Empty);
                    cell._position = Cells.GetCellPosition(mousePosCellIndex.Value.Y, mousePosCellIndex.Value.X, out _, out _,
                                                IsVerticalScrollBarVisible ? StartRenderYPos : 0,
                                                IsHorizontalScrollBarVisible ? StartRenderXPos : 0);
                }

                if (CurrentMouseCell != cell)
                    IsDirty = true;

                CurrentMouseCell = cell;

                if (CurrentMouseCell != null && CurrentMouseCell.IsVisible && (!CurrentMouseCell.IsSettingsInitialized || CurrentMouseCell.Settings.Interactable))
                    OnCellEnter?.Invoke(this, new CellEventArgs(CurrentMouseCell));
            }
            else
            {
                if (CurrentMouseCell != null && CurrentMouseCell.IsVisible && (!CurrentMouseCell.IsSettingsInitialized || CurrentMouseCell.Settings.Interactable))
                {
                    OnCellExit?.Invoke(this, new CellEventArgs(CurrentMouseCell));
                    CurrentMouseCell = null;
                    IsDirty = true;
                }
            }
        }

        if (state.OriginalMouseState.Mouse.ScrollWheelValueChange != 0)
        {
            ScrollBar? scrollBar = null;

            if (IsVerticalScrollBarVisible && !IsHorizontalScrollBarVisible)
                scrollBar = VerticalScrollBar;

            else if (!IsVerticalScrollBarVisible && IsHorizontalScrollBarVisible)
                scrollBar = HorizontalScrollBar;

            // If both scroll bars are not null, we only wanna scroll on the vertical scrollbar with the mousewheel
            else if (IsVerticalScrollBarVisible && IsHorizontalScrollBarVisible)
                scrollBar = VerticalScrollBar;

            if (scrollBar != null)
            {
                int prev = scrollBar.Value;

                if (state.OriginalMouseState.Mouse.ScrollWheelValueChange < 0)
                    scrollBar.Value -= 1;
                else
                    scrollBar.Value += 1;

                if (prev != scrollBar.Value)
                    IsDirty = true;
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnLeftMouseClicked(ControlMouseState state)
    {
        base.OnLeftMouseClicked(state);

        if (CurrentMouseCell != null)
        {
            if (SelectedCell != CurrentMouseCell && CurrentMouseCell.IsVisible && (!CurrentMouseCell.IsSettingsInitialized || (CurrentMouseCell.Settings.Interactable && CurrentMouseCell.Settings.Selectable)))
            {
                SelectedCell = CurrentMouseCell;
                ScrollToSelectedItem();
            }
            else
            {
                // Unselect after clicking the selected cell again
                SelectedCell = null;
            }

            if (CurrentMouseCell.IsVisible && (!CurrentMouseCell.IsSettingsInitialized || CurrentMouseCell.Settings.Interactable))
                OnCellLeftClick?.Invoke(this, new CellEventArgs(CurrentMouseCell));

            DateTime click = DateTime.Now;
            bool doubleClicked = (click - _leftMouseLastClick).TotalSeconds <= 0.25 && state.MousePosition == _leftMouseLastClickPosition;
            _leftMouseLastClick = click;
            _leftMouseLastClickPosition = state.MousePosition;

            if (doubleClicked)
            {
                _leftMouseLastClick = DateTime.MinValue;
                _leftMouseLastClickPosition = null;
                if (CurrentMouseCell.IsVisible && (!CurrentMouseCell.IsSettingsInitialized || CurrentMouseCell.Settings.Interactable))
                    OnCellDoubleClick?.Invoke(this, new CellEventArgs(CurrentMouseCell));
            }
        }
        else
            SelectedCell = null;
    }

    /// <inheritdoc/>
    protected override void OnRightMouseClicked(ControlMouseState state)
    {
        base.OnRightMouseClicked(state);

        if (CurrentMouseCell != null && CurrentMouseCell.IsVisible && (!CurrentMouseCell.IsSettingsInitialized || CurrentMouseCell.Settings.Interactable))
            OnCellRightClick?.Invoke(this, new CellEventArgs(CurrentMouseCell));
    }

    /// <inheritdoc/>
    protected override void OnMouseExit(ControlMouseState state)
    {
        base.OnMouseExit(state);

        if (CurrentMouseCell != null)
        {
            if (CurrentMouseCell.IsVisible && (!CurrentMouseCell.IsSettingsInitialized || CurrentMouseCell.Settings.Interactable))
                OnCellExit?.Invoke(this, new CellEventArgs(CurrentMouseCell));

            CurrentMouseCell = null;
            IsDirty = true;
        }
    }

    private Point? GetCellIndexByMousePosition(Point mousePosition)
    {
        for (int col = 0; col <= Cells.MaxColumn; col++)
        {
            for (int row = 0; row <= Cells.MaxRow; row++)
            {
                int rowValue = row + (IsVerticalScrollBarVisible && VerticalScrollBar != null ? VerticalScrollBar.Value : 0);
                int colValue = col + (IsHorizontalScrollBarVisible && HorizontalScrollBar != null ? HorizontalScrollBar.Value : 0);

                Point position = Cells.GetCellPosition(rowValue, colValue, out int rowSize, out int columnSize,
                    IsVerticalScrollBarVisible ? StartRenderYPos : 0, IsHorizontalScrollBarVisible ? StartRenderXPos : 0);
                if (IsMouseWithinCell(mousePosition, position.Y, position.X, columnSize, rowSize))
                {
                    Cell? cell = Cells.GetIfExists(rowValue, colValue, false);
                    if (cell == null || (cell._row == rowValue && cell._column == colValue && cell.IsVisible))
                        return cell != null ? (cell.Column, cell.Row) : (colValue, rowValue);
                }
            }
        }
        return null;
    }

    private static bool IsMouseWithinCell(Point mousePosition, int row, int column, int width, int height)
    {
        int maxX = column + width;
        int maxY = row + height;

        return mousePosition.X >= column && mousePosition.X < maxX &&
               mousePosition.Y >= row && mousePosition.Y < maxY;
    }

    #region Event Args
    /// <summary>
    /// Cell args for a table event
    /// </summary>
    public class CellEventArgs : EventArgs
    {
        /// <summary>
        /// The cell that triggered this event
        /// </summary>
        public readonly Cell? Cell;

        internal CellEventArgs(Cell? cell) =>
            Cell = cell;
    }

    /// <inheritdoc/>
    public sealed class CellChangedEventArgs : CellEventArgs
    {
        /// <summary>
        /// The original cell before the event was triggered
        /// </summary>
        public readonly Cell? PreviousCell;

        internal CellChangedEventArgs(Cell? previousCell, Cell? cell): base(cell) =>
            PreviousCell = previousCell;
    }
    #endregion
}

/// <summary>
/// Extensions used for the <see cref="Table"/>
/// </summary>
public static class TableExtensions
{
    /// <summary>
    /// Returns a range of cells that fits the given parameter values.
    /// </summary>
    /// <param name="cells"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    /// <param name="endRow"></param>
    /// <param name="endCol"></param>
    /// <returns></returns>
    public static IEnumerable<Table.Cell> Range(this Table.TableCells cells, int startRow, int startCol, int endRow, int endCol)
    {
        int width = endCol - startCol + 1;
        int height = endRow - startRow + 1;
        for (int x = startCol; x < startCol + width; x++)
        {
            for (int y = startRow; y < startRow + height; y++)
            {
                yield return cells[y, x];
            }
        }
    }

    /// <summary>
    /// Executes an action on each cell.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="action"></param>
    public static void ForEach(this IEnumerable<Table.Cell> range, Action<Table.Cell> action)
    {
        foreach (Table.Cell cell in range)
            action(cell);
    }

    /// <summary>
    /// Sets the layout for the cell.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="foreground"></param>
    /// <param name="background"></param>
    /// <param name="settings"></param>
    public static void SetLayout(this Table.Cell cell, Color? foreground = null, Color? background = null, Table.Cell.Options? settings = null)
    {
        cell._table.Cells.Column(cell.Column).SetLayout(null, foreground, background, settings);
        cell._table.Cells.Row(cell.Row).SetLayout(null, foreground, background, settings);
        cell._table.IsDirty = true;
    }

    /// <summary>
    /// Resizes the entire column and row to the specified sizes.
    /// If no sizes are specified for both row and column, the cell will be reset to the default size.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="rowSize"></param>
    /// <param name="columnSize"></param>
    public static void Resize(this Table.Cell cell, int? rowSize = null, int? columnSize = null)
    {
        if (rowSize == null && columnSize == null)
        {
            rowSize = cell._table.DefaultCellSize.Y;
            columnSize = cell._table.DefaultCellSize.X;
        }

        cell._table.Cells.Column(cell.Column).SetLayoutInternal(columnSize);
        cell._table.Cells.Row(cell.Row).SetLayoutInternal(rowSize);
        cell._table.Cells.AdjustCellPositionsAfterResize();
        cell._table.SyncScrollAmountOnResize();
        cell._table.IsDirty = true;
    }

    /// <summary>
    /// Sets the cell as the selected cell.
    /// </summary>
    /// <param name="cell"></param>
    public static void Select(this Table.Cell cell)
    {
        cell._table.Cells.Select(cell.Row, cell.Column);
    }

    /// <summary>
    /// Incase this cell is the selected cell, it will unselect it.
    /// </summary>
    /// <param name="cell"></param>
    public static void Deselect(this Table.Cell cell)
    {
        cell._table.Cells.Deselect();
    }

    /// <summary>
    /// Get the layout for the given columns.
    /// </summary>
    /// <param name="cells"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static Table.TableCells.Layout.RangeEnumerable Column(this Table.TableCells cells, params int[] columns)
    {
        IEnumerable<Table.TableCells.Layout> layouts = columns.Select(a => cells.Column(a));
        return new Table.TableCells.Layout.RangeEnumerable(layouts);
    }

    /// <summary>
    /// Get the layout for the given rows.
    /// </summary>
    /// <param name="cells"></param>
    /// <param name="rows"></param>
    /// <returns></returns>
    public static Table.TableCells.Layout.RangeEnumerable Row(this Table.TableCells cells, params int[] rows)
    {
        IEnumerable<Table.TableCells.Layout> layouts = rows.Select(a => cells.Row(a));
        return new Table.TableCells.Layout.RangeEnumerable(layouts);
    }

    /// <summary>
    /// Removes the cell from its table.
    /// </summary>
    /// <param name="cell"></param>
    public static void Remove(this Table.Cell cell)
    {
        cell._table.Cells.Remove(cell.Row, cell.Column);
    }
}
