using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class Table
{
    /// <summary>
    /// A collection class that contains all the cells of the <see cref="Table"/> and methods to modify them.
    /// </summary>
    public sealed class TableCells : IEnumerable<Cell>
    {
        private readonly Table _table;
        private readonly Dictionary<Point, Cell> _cells = new();
        internal readonly Dictionary<int, Layout> _columnLayout = new();
        internal readonly Dictionary<int, Layout> _rowLayout = new();
        /// <summary>
        /// Contains all rows and columns that aren't rendered
        /// </summary>
        internal readonly Dictionary<Layout.LayoutType, HashSet<int>> _hiddenIndexes = new();

        /// <summary>
        /// Gets or creates a new cell on the specified row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Cell this[int row, int col]
        {
            get => GetOrCreateCell(row, col);
            internal set => SetCell(row, col, value);
        }

        /// <summary>
        /// The maximum row in the table.
        /// </summary>
        public int MaxRow { get; private set; }

        /// <summary>
        /// The maximum column in the table.
        /// </summary>
        public int MaxColumn { get; private set; }

        private bool _headerRow;
        /// <summary>
        /// True if row 0 should be the header of the table, and remain at the top when vertical scrolling.
        /// </summary>
        public bool HeaderRow
        {
            get => _headerRow;
            set
            {
                _headerRow = value;
                _table._checkScrollBarVisibility = true;
                _table.IsDirty = true;
            }
        }

        /// <summary>
        /// The amount of cells currently in the table.
        /// </summary>
        public int Count => _cells.Count;

        internal TableCells(Table table)
        {
            _table = table;
        }

        #region Public Methods
        /// <summary>
        /// Sets the visibility of the entire row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="visible"></param>
        public void Row(int row, bool visible)
        {
            if (!_hiddenIndexes.TryGetValue(Layout.LayoutType.Row, out HashSet<int>? indexes))
            {
                if (visible) return;

                indexes = new HashSet<int>();
                _hiddenIndexes.Add(Layout.LayoutType.Row, indexes);
            }

            for (int col = 0; col <= _table.Cells.MaxColumn; col++)
                _table.Cells[row, col].IsVisible = visible;

            if (visible)
            {
                indexes.Remove(row);
                if (indexes.Count == 0)
                    _hiddenIndexes.Remove(Layout.LayoutType.Row);
            }
            else
            {
                indexes.Add(row);
            }

            if (_table.HorizontalScrollBar != null)
                _table.HorizontalScrollBar.Value = 0;

            if (_table.VerticalScrollBar != null)
                _table.VerticalScrollBar.Value = 0;

            _table.IsDirty = true;
            _table._checkScrollBarVisibility = true;
        }

        /// <summary>
        /// Sets the visibility of the entire column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="visible"></param>
        public void Column(int column, bool visible)
        {
            if (!_hiddenIndexes.TryGetValue(Layout.LayoutType.Column, out HashSet<int>? indexes))
            {
                if (visible) return;

                indexes = new HashSet<int>();
                _hiddenIndexes.Add(Layout.LayoutType.Column, indexes);
            }

            for (int row = 0; row <= _table.Cells.MaxRow; row++)
                _table.Cells[row, column].IsVisible = visible;

            if (visible)
            {
                indexes.Remove(column);
                if (indexes.Count == 0)
                    _hiddenIndexes.Remove(Layout.LayoutType.Column);
            }
            else
            {
                indexes.Add(column);
            }

            if (_table.HorizontalScrollBar != null)
                _table.HorizontalScrollBar.Value = 0;
            if (_table.VerticalScrollBar != null)
                _table.VerticalScrollBar.Value = 0;

            _table.IsDirty = true;
            _table._checkScrollBarVisibility = true;
        }

        /// <summary>
        /// Get the layout for the given column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Layout Column(int column)
        {
            _columnLayout.TryGetValue(column, out Layout? layout);
            layout ??= _columnLayout[column] = new Layout(_table, column, Layout.LayoutType.Column);
            return layout;
        }

        /// <summary>
        /// Get the layout for the given row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public Layout Row(int row)
        {
            _rowLayout.TryGetValue(row, out Layout? layout);
            layout ??= _rowLayout[row] = new Layout(_table, row, Layout.LayoutType.Row);
            return layout;
        }

        /// <summary>
        /// Gets the cell at the given row and col
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Cell GetCell(int row, int col) =>
            this[row, col];

        /// <summary>
        /// Sets the specified cell as the selected cell if it exists.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void Select(int row, int column)
        {
            // Set existing cell, or a fake one if it does not yet exists, but modifying this fake cell with add it to the table
            _table.SelectedCell = GetIfExists(row, column, false) ?? Cell.InternalCreate(row, column, _table, string.Empty);
            _table.SelectedCell.Position = GetCellPosition(row, column, out _, out _,
                    _table.IsVerticalScrollBarVisible ? _table.StartRenderYPos : 0,
                    _table.IsHorizontalScrollBarVisible ? _table.StartRenderXPos : 0);
        }

        /// <summary>
        /// Deselects the current selected cell.
        /// </summary>
        public void Deselect()
        {
            _table.SelectedCell = null;
        }

        /// <summary>
        /// Removes a cell from the table.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void Remove(int row, int column)
        {
            Remove(row, column, true);
        }

        /// <summary>
        /// Resets all the cells data
        /// </summary>
        /// <param name="clearLayoutOptions"></param>
        public void Clear(bool clearLayoutOptions = true)
        {
            if (clearLayoutOptions)
            {
                _rowLayout.Clear();
                _columnLayout.Clear();
            }
            MaxRow = 0;
            MaxColumn = 0;
            _cells.Clear();
            _table.IsDirty = true;
        }

        /// <summary>
        /// Removes all rows and columns except the header if applicable
        /// </summary>
        /// <param name="clearLayoutOptionsForContent"></param>
        public void ClearContent(bool clearLayoutOptionsForContent = true)
        {
            // X = row, Y = column
            int maxColumn = HeaderRow ? _cells.Where(a => a.Key.X == 0).Count() : 0;
            foreach (KeyValuePair<Point, Cell> cell in _cells.ToArray())
            {
                if (HeaderRow && cell.Key.X == 0) continue;

                Remove(cell.Key.X, cell.Key.Y, false);

                if (clearLayoutOptionsForContent)
                {
                    _rowLayout.Remove(cell.Key.X);
                    _columnLayout.Remove(cell.Key.Y);
                }
            }

            // Adjust maxes
            MaxRow = HeaderRow ? 1 : 0;
            MaxColumn = maxColumn;

            AdjustCellPositionsAfterResize();
            _table.SyncScrollAmountOnResize();
            _table.IsDirty = true;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Get's the cell position on the control based on the row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="rowSize"></param>
        /// <param name="columnSize"></param>
        /// <param name="verticalScrollBarValue"></param>
        /// <param name="horizontalScrollbarValue"></param>
        /// <returns></returns>
        internal Point GetCellPosition(int row, int col, out int rowSize, out int columnSize, int verticalScrollBarValue = 0, int horizontalScrollbarValue = 0)
        {
            int columnIndex = GetControlIndex(col, horizontalScrollbarValue, Layout.LayoutType.Column, out columnSize);
            int rowIndex = GetControlIndex(row, verticalScrollBarValue, Layout.LayoutType.Row, out rowSize);
            return new Point(columnIndex, rowIndex);
        }

        /// <summary>
        /// Get the size of the column or row or the default if no layout exists, without allocating a new layout object.
        /// </summary>
        /// <returns></returns>
        internal int GetSizeOrDefault(int index, Layout.LayoutType type)
        {
            return type switch
            {
                Layout.LayoutType.Column => _columnLayout.TryGetValue(index, out Layout? layout) ? layout.Size : _table.DefaultCellSize.X,
                Layout.LayoutType.Row => _rowLayout.TryGetValue(index, out Layout? layout) ? layout.Size : _table.DefaultCellSize.Y,
                _ => throw new NotSupportedException("Invalid layout type."),
            };
        }

        internal Cell? GetIfExists(int row, int col, bool useRealRowAndCols)
        {
            Cell? chosenCell = null;
            foreach (KeyValuePair<Point, Cell> cell in _cells)
            {
                if (useRealRowAndCols)
                {
                    if (cell.Value.Row == row && cell.Value.Column == col)
                        chosenCell = cell.Value;
                    continue;
                }

                if (cell.Value._row == row && cell.Value._column == col)
                {
                    chosenCell = cell.Value;
                }
            }
            return chosenCell;
        }

        private Cell GetOrCreateCell(int row, int col)
        {
            if (!_cells.TryGetValue((row, col), out Cell? cell))
            {
                cell = Cell.InternalCreate(row, col, _table, string.Empty);
                cell.Position = GetCellPosition(row, col, out _, out _,
                        _table.IsVerticalScrollBarVisible ? _table.StartRenderYPos : 0,
                        _table.IsHorizontalScrollBarVisible ? _table.StartRenderXPos : 0);

                _cells[(row, col)] = cell;
                if (MaxRow < row)
                    MaxRow = row;

                if (MaxColumn < col)
                    MaxColumn = col;

                _table._checkScrollBarVisibility = true;
            }

            return cell;
        }

        private int GetControlIndex(int index, int startPos, Layout.LayoutType type, out int indexSize)
        {
            // Matches the right cell we should start at, but it could be we need to start somewhere within this cell.
            int startIndex = GetIndexAtCellPosition(startPos, type, out int indexPos);
            int controlIndex = 0;

            if (indexPos < startPos)
                controlIndex = indexPos - startPos;

            Dictionary<int, Layout> layoutDict = type == Layout.LayoutType.Column ? _columnLayout : _rowLayout;
            int defaultSize = type == Layout.LayoutType.Column ? _table.DefaultCellSize.X : _table.DefaultCellSize.Y;

            indexSize = layoutDict.TryGetValue(startIndex, out Layout? layout) ? layout.Size : defaultSize;

            // If entire row or column is hidden then skip it
            _hiddenIndexes.TryGetValue(type, out HashSet<int>? indexes);
            if (indexes != null && indexes.Contains(startIndex))
                indexSize = 0;

            while (startIndex < index)
            {
                controlIndex += indexSize;
                startIndex++;

                indexSize = layoutDict.TryGetValue(startIndex, out layout) ? layout.Size : defaultSize;
                // If entire row or column is hidden then skip it
                _hiddenIndexes.TryGetValue(type, out indexes);
                if (indexes != null && indexes.Contains(startIndex) && startIndex < index)
                    indexSize = 0;
            }
            return controlIndex;
        }

        internal int GetIndexAtCellPosition(int pos, Layout.LayoutType type, out int indexPos)
        {
            int total = type == Layout.LayoutType.Row ? (_table.Cells.MaxRow + 1) : (_table.Cells.MaxColumn + 1);
            Dictionary<int, Layout> layoutDict = type == Layout.LayoutType.Row ? _rowLayout : _columnLayout;
            int defaultSize = type == Layout.LayoutType.Row ? _table.DefaultCellSize.Y : _table.DefaultCellSize.X;
            int totalSize = 0;
            for (int i = 0; i < total; i++)
            {
                int indexSize = layoutDict.TryGetValue(i, out Layout? layout) ? layout.Size : defaultSize;
                _hiddenIndexes.TryGetValue(type, out HashSet<int>? indexes);
                if (indexes != null && indexes.Contains(i))
                    indexSize = 0;
                totalSize += indexSize;
                if (pos < totalSize)
                {
                    indexPos = totalSize - indexSize;
                    return i;
                }
            }
            indexPos = 0;
            return 0;
        }

        private void SetCell(int row, int col, Cell? cell)
        {
            if (cell == null)
            {
                if (_cells.Remove((row, col)))
                {
                    MaxRow = _cells.Count == 0 ? 0 : _cells.Values.Max(a => a.Row);
                    MaxColumn = _cells.Count == 0 ? 0 : _cells.Values.Max(a => a.Column);
                    _table._checkScrollBarVisibility = true;
                    _table.IsDirty = true;
                }
                return;
            }

            _cells[(row, col)] = cell;
            if (MaxRow < row)
                MaxRow = row;
            if (MaxColumn < col)
                MaxColumn = col;
            _table._checkScrollBarVisibility = true;
            _table.IsDirty = true;
        }

        internal void AdjustCellPositionsAfterResize()
        {
            foreach (KeyValuePair<Point, Cell> cell in _cells)
                cell.Value.Position = GetCellPosition(cell.Value.Row, cell.Value.Column, out _, out _,
                    _table.IsVerticalScrollBarVisible ? _table.StartRenderYPos : 0, _table.IsHorizontalScrollBarVisible ? _table.StartRenderXPos : 0);
            _table._checkScrollBarVisibility = true;
            _table.IsDirty = true;
        }

        /// <inheritdoc/>
        public IEnumerator<Cell> GetEnumerator() =>
            _cells.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        private void Remove(int row, int column, bool adjustMaxRowColumns)
        {
            int prev = _cells.Count;
            _cells.Remove((row, column));
            if (prev != _cells.Count)
            {
                if (adjustMaxRowColumns)
                {
                    MaxRow = _cells.Count == 0 ? 0 : _cells.Values.Max(a => a.Row);
                    MaxColumn = _cells.Count == 0 ? 0 : _cells.Values.Max(a => a.Column);
                }

                AdjustCellPositionsAfterResize();
                _table.SyncScrollAmountOnResize();
                _table.IsDirty = true;
            }
        }

        /// <summary>
        /// Defines the layout for a row or a column defined in <see cref="Cells"/>
        /// </summary>
        public class Layout
        {
            private int _size;
            /// <summary>
            /// The size of the row or column
            /// </summary>
            public int Size
            {
                get => _size;
                set
                {
                    if (_size != value)
                    {
                        _size = value;
                        _table.Cells.AdjustCellPositionsAfterResize();
                        _table.SyncScrollAmountOnResize();
                        _table.IsDirty = true;
                    }
                }
            }

            /// <summary>
            /// The foreground color used by the row or column
            /// </summary>
            public Color? Foreground { get; set; }
            /// <summary>
            /// The background color used by the row or column
            /// </summary>
            public Color? Background { get; set; }

            private Cell.Options? _settings;
            /// <summary>
            /// The setting options used by the row or column
            /// </summary>
            public Cell.Options Settings
            {
                get => _settings ??= new Cell.Options(_table);
                set
                {
                    if (value == null) return;
                    (_settings ??= new Cell.Options(_table)).CopyFrom(value);
                }
            }

            /// <summary>
            /// True if the Settings property has been accessed before.
            /// </summary>
            internal bool HasCustomSettings => _settings != null;

            private readonly Table _table;
            private readonly LayoutType _layoutType;
            private readonly int _index;

            internal Layout(Table table, int index, LayoutType type)
            {
                _table = table;
                _layoutType = type;
                _index = index;
                Size = type == LayoutType.Column ? table.DefaultCellSize.X : table.DefaultCellSize.Y;
            }


            /// <summary>
            /// Removes this entire layout from the table.
            /// </summary>
            public void Remove()
            {
                Dictionary<int, Layout> layoutDict = _layoutType == LayoutType.Row ? _table.Cells._rowLayout : _table.Cells._columnLayout;
                layoutDict.Remove(_index);
                _table.IsDirty = true;
            }

            /// <summary>
            /// Set a default layout to be used for each new cell
            /// </summary>
            /// <param name="size"></param>
            /// <param name="foreground"></param>
            /// <param name="background"></param>
            /// <param name="settings"></param>
            public void SetLayout(int? size = null, Color? foreground = null, Color? background = null, Cell.Options? settings = null)
            {
                int prevSize = _size;
                SetLayoutInternal(size, foreground, background, settings);
                if (prevSize != _size)
                {
                    _table.Cells.AdjustCellPositionsAfterResize();
                    _table.SyncScrollAmountOnResize();
                    _table.IsDirty = true;
                }
            }

            /// <summary>
            /// Sets the layout without adjusting cells or setting the table dirty
            /// </summary>
            /// <param name="size"></param>
            /// <param name="foreground"></param>
            /// <param name="background"></param>
            /// <param name="settings"></param>
            internal void SetLayoutInternal(int? size = null, Color? foreground = null, Color? background = null, Cell.Options? settings = null)
            {
                if (size != null)
                    _size = size.Value;
                if (foreground != null)
                    Foreground = foreground;
                if (background != null)
                    Background = background;
                if (settings != null)
                    Settings = settings;
            }

            internal enum LayoutType
            {
                Column,
                Row
            }

            /// <summary>
            /// Defines several visual modes
            /// </summary>
            public enum Mode
            {
                /// <summary>
                /// Only a single cell will be visualized
                /// </summary>
                Single = 0,

                /// <summary>
                /// Nothing will be visualized
                /// </summary>
                None,

                /// <summary>
                /// The entire row of the cell will be visualized
                /// </summary>
                EntireRow,

                /// <summary>
                /// The entire column of the cell will be visualized
                /// </summary>
                EntireColumn
            }

            /// <summary>
            /// An enumerable range that contains the layouts of all the rows and columns defined by the size of the range method within <see cref="Cells"/>
            /// </summary>
            public class RangeEnumerable : IEnumerable<Layout>
            {
                private readonly IEnumerable<Layout> _layouts;

                internal RangeEnumerable(IEnumerable<Layout> layouts) =>
                    _layouts = layouts;

                /// <summary>
                /// Sets the layout of all the columns and rows for the given params
                /// </summary>
                /// <param name="size"></param>
                /// <param name="foreground"></param>
                /// <param name="background"></param>
                /// <param name="settings"></param>
                public void SetLayout(int? size = null, Color? foreground = null, Color? background = null, Cell.Options? settings = null)
                {
                    foreach (Layout layout in _layouts)
                        layout.SetLayout(size, foreground, background, settings);
                }

                /// <inheritdoc/>
                public IEnumerator<Layout> GetEnumerator() =>
                    _layouts.GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() =>
                    GetEnumerator();
            }
        }
    }
}
