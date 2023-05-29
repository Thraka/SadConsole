using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class Table
{
    /// <summary>
    /// A basic cell used in the Table control
    /// </summary>
    public sealed class Cell : IEquatable<Cell>
    {
        /// <summary>
        /// Contains the real position value when other cells are not being rendered
        /// This is used to obtain the real cell from mouse interactions.
        /// </summary>
        internal Point Position;
        /// <summary>
        /// Contains the real row value and column value when other cells are not being rendered
        /// This is used to obtain the real cell from mouse interactions.
        /// </summary>
        internal int _row, _column;

        /// <summary>
        /// The row this cell is part of
        /// </summary>
        public int Row { get; }
        /// <summary>
        /// The column this cell is part of
        /// </summary>
        public int Column { get; }
        /// <summary>
        /// The height of the row this cell is part of
        /// </summary>
        public int Height => _table.Cells.GetSizeOrDefault(Row, TableCells.Layout.LayoutType.Row);
        /// <summary>
        /// The width of the column this cell is part of
        /// </summary>
        public int Width => _table.Cells.GetSizeOrDefault(Column, TableCells.Layout.LayoutType.Column);

        private Color _foreground;
        /// <summary>
        /// The foreground color used by the cell
        /// </summary>
        public Color Foreground
        {
            get => _foreground;
            set => SetFieldValue(this, Foreground, ref _foreground, value, false);
        }

        private Color _background;
        /// <summary>
        /// The background color used by the cell
        /// </summary>
        public Color Background
        {
            get => _background;
            set => SetFieldValue(this, Background, ref _background, value, false);
        }

        /// <summary>
        /// The text shown within the cell, empty string when no value.
        /// </summary>
        public string StringValue
        {
            get => _value?.ToString() ?? string.Empty;
        }

        private object? _value;
        /// <summary>
        /// The value of the cell, .ToString() method is shown as the text result within the cell.
        /// </summary>
        public object? Value
        {
            get => _value;
            set => SetFieldValue(this, Value, ref _value, value, false);
        }

        private bool _isVisible = true;
        /// <summary>
        /// Set to false if the cell should not be rendered within the table (default IsVisible sadconsole behaviour),
        /// If an entire row or column IsVislbe is set to false in the layout, it will skip this row/column entirely (differs from default behaviour)
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => SetFieldValue(this, IsVisible, ref _isVisible, value, false);
        }

        private Options? _settings;
        /// <summary>
        /// The setting options used by the cell to define its layout
        /// </summary>
        public Options Settings => _settings ??= new Options(this);

        private readonly bool _addToTableIfModified;
        internal readonly Table _table;

        private Cell(int row, int col, Table table, object value, bool addToTableIfModified = true)
        {
            _table = table;
            _value = value;
            _row = row;
            _column = col;
            _foreground = table.DefaultForeground;
            _background = table.DefaultBackground;
            _addToTableIfModified = addToTableIfModified;

            Row = row;
            Column = col;

            // Set cell layout options
            table.Cells._columnLayout.TryGetValue(col, out TableCells.Layout? columnLayout);
            table.Cells._rowLayout.TryGetValue(row, out TableCells.Layout? rowLayout);
            TableCells.Layout?[] layoutOptions = new[] { columnLayout, rowLayout };
            foreach (TableCells.Layout? option in layoutOptions)
            {
                if (option == null) continue;

                if (option.Foreground != null)
                    _foreground = option.Foreground.Value;

                if (option.Background != null)
                    _background = option.Background.Value;

                if (option.HasCustomSettings)
                    (_settings ??= new Options(this)).CopyFrom(option.Settings);
            }
        }

        /// <summary>
        /// Internal use only. This is used by the table and the theme to create new cell instances.
        /// </summary>
        public static Cell InternalCreate(int row, int col, Table table, object value, bool addToTableIfModified = true) =>
            new(row, col, table, value, addToTableIfModified);

        internal void AddToTableIfNotExists()
        {
            if (_addToTableIfModified && _table.Cells.GetIfExists(Row, Column, true) == null)
                _table.Cells[Row, Column] = this;
        }

        internal bool IsSettingsInitialized => _settings != null;

        /// <inheritdoc/>
        public static bool operator ==(Cell? a, Cell? b)
        {
            if (a is null && b is null) return true;
            return a is not null && b is not null && a.Equals(b);
        }
        /// <inheritdoc/>
        public static bool operator !=(Cell? a, Cell? b)
        {
            return !(a == b);
        }
        /// <inheritdoc/>
        public bool Equals(Cell? cell)
        {
            return cell != null && cell.Column == Column && cell.Row == Row;
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Cell cell && Equals(cell);
        }
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Column, Row);
        }

        /// <summary>
        /// Copies the appearance of the cell passed to this method, onto the this cell.
        /// </summary>
        /// <param name="cell"></param>
        public void CopyAppearanceFrom(Cell cell)
        {
            Foreground = cell.Foreground;
            Background = cell.Background;
            if (_settings != cell._settings)
            {
                if (cell._settings == null)
                    _settings = null;
                else
                    Settings.CopyFrom(cell.Settings);
            }
        }

        /// <summary>
        /// Helper to set the underlying field value with some checks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cell"></param>
        /// <param name="previousValue"></param>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="usedForLayout"></param>
        internal static void SetFieldValue<T>(Cell? cell, T previousValue, ref T field, T newValue, bool usedForLayout)
        {
            if (!EqualityComparer<T>.Default.Equals(previousValue, newValue))
            {
                field = newValue;
                if (usedForLayout || cell is null) return;
                cell.AddToTableIfNotExists();
                cell._table.IsDirty = true;
            }
        }

        /// <summary>
        /// A collection of settings that are used by <see cref="Table.Cell"/>
        /// </summary>
        public class Options : IEquatable<Options>
        {
            private HorizontalAlign _horizontalAlignment;
            /// <summary>
            /// The horizontal text alignment setting; Default: left
            /// </summary>
            public HorizontalAlign HorizontalAlignment
            {
                get => _horizontalAlignment;
                set => SetFieldValue(_cell, HorizontalAlignment, ref _horizontalAlignment, value, _usedForLayout);
            }

            private VerticalAlign _verticalAlignment;
            /// <summary>
            /// The vertical text alignment setting; Default: left
            /// </summary>
            public VerticalAlign VerticalAlignment
            {
                get => _verticalAlignment;
                set => SetFieldValue(_cell, VerticalAlignment, ref _verticalAlignment, value, _usedForLayout);
            }

            private bool _useFakeLayout = false;
            /// <summary>
            /// Defines if the cell should also trigger the fake layout event if the option is enabled on the table; Default: false
            /// </summary>
            public bool UseFakeLayout
            {
                get => _useFakeLayout;
                set => SetFieldValue(_cell, UseFakeLayout, ref _useFakeLayout, value, _usedForLayout);
            }

            private int? _maxCharactersPerLine;
            /// <summary>
            /// The maximum characters this cell can show per line; Default: cell width
            /// </summary>
            public int? MaxCharactersPerLine
            {
                get => _maxCharactersPerLine;
                set => SetFieldValue(_cell, MaxCharactersPerLine, ref _maxCharactersPerLine, value, _usedForLayout);
            }

            private bool _interactable = true;
            /// <summary>
            /// Defines if the cell can interact with mouse events; Default: true
            /// </summary>
            public bool Interactable
            {
                get => _interactable;
                set => SetFieldValue(_cell, Interactable, ref _interactable, value, _usedForLayout);
            }

            private bool _selectable = true;
            /// <summary>
            /// Defines if the cell can be selected by the mouse; Default: true
            /// </summary>
            public bool Selectable
            {
                get => _selectable;
                set => SetFieldValue(_cell, Selectable, ref _selectable, value, _usedForLayout);
            }

            private TableCells.Layout.Mode _selectionMode;
            /// <summary>
            /// Defines the selection visual mode when the cell is selected; Default: single
            /// </summary>
            public TableCells.Layout.Mode SelectionMode
            {
                get => _selectionMode;
                set => SetFieldValue(_cell, SelectionMode, ref _selectionMode, value, _usedForLayout);
            }

            private TableCells.Layout.Mode _hoverMode;
            /// <summary>
            /// Defines the hover visual mode when the cell is hovered over by the mouse; Default: single
            /// </summary>
            public TableCells.Layout.Mode HoverMode
            {
                get => _hoverMode;
                set => SetFieldValue(_cell, HoverMode, ref _hoverMode, value, _usedForLayout);
            }

            private readonly bool _usedForLayout;
            private readonly Cell? _cell;

            internal Options(Cell cell)
            {
                _usedForLayout = false;
                _cell = cell;
                _hoverMode = _cell._table.DefaultHoverMode;
                _selectionMode = _cell._table.DefaultSelectionMode;
            }

            /// <summary>
            /// Creates new options based on the default values of the table
            /// </summary>
            /// <param name="table"></param>
            public Options(Table table)
            {
                _usedForLayout = true;
                _hoverMode = table.DefaultHoverMode;
                _selectionMode = table.DefaultSelectionMode;
            }

            /// <summary>
            /// Alignment enum for the horizontal axis
            /// </summary>
            public enum HorizontalAlign
            {
                /// <summary>
                /// Text will be aligned to the left side of the cell
                /// </summary>
                Left = 0,
                /// <summary>
                /// Text will be aligned within the center of the cell
                /// </summary>
                Center,
                /// <summary>
                /// Text will be aligned to the right side of the cell
                /// </summary>
                Right
            }

            /// <summary>
            /// Alignment enum for the vertical axis
            /// </summary>
            public enum VerticalAlign
            {
                /// <summary>
                /// Text will be aligned to the top of the cell
                /// </summary>
                Top = 0,
                /// <summary>
                /// Text will be aligned in the center of the cell
                /// </summary>
                Center,
                /// <summary>
                /// Text will be aligned to the bottom of the cell
                /// </summary>
                Bottom
            }

            /// <inheritdoc/>
            public static bool operator ==(Options? a, Options? b)
            {
                if (a is null && b is null) return true;
                return a is not null && b is not null && a.Equals(b);
            }
            /// <inheritdoc/>
            public static bool operator !=(Options? a, Options? b)
            {
                return !(a == b);
            }
            /// <inheritdoc/>
            public bool Equals(Options? other)
            {
                return other != null
&& other.HorizontalAlignment == HorizontalAlignment &&
                    other.VerticalAlignment == VerticalAlignment &&
                    other.MaxCharactersPerLine == MaxCharactersPerLine &&
                    other.Selectable == Selectable &&
                    other.SelectionMode == SelectionMode &&
                    other.HoverMode == HoverMode &&
                    other.Interactable == Interactable &&
                    other.UseFakeLayout == UseFakeLayout;
            }
            /// <inheritdoc/>
            public override bool Equals(object? obj)
            {
                return obj is Options to && Equals(to);
            }
            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return HashCode.Combine(new object?[]
                {
                    HorizontalAlignment,
                    VerticalAlignment,
                    MaxCharactersPerLine,
                    Selectable,
                    SelectionMode,
                    HoverMode,
                    Interactable,
                    UseFakeLayout
                });
            }

            internal void CopyFrom(Options settings)
            {
                HorizontalAlignment = settings.HorizontalAlignment;
                VerticalAlignment = settings.VerticalAlignment;
                MaxCharactersPerLine = settings.MaxCharactersPerLine;
                Selectable = settings.Selectable;
                SelectionMode = settings.SelectionMode;
                HoverMode = settings.HoverMode;
                Interactable = settings.Interactable;
                UseFakeLayout = settings.UseFakeLayout;
            }
        }
    }
}
