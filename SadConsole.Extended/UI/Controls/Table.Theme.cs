using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class Table
{
    /// <inheritdoc />
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        RefreshThemeStateColors(FindThemeColors());

        // Draw the basic table surface foreground and background, and clear the glyphs
        Surface.Fill(DefaultForeground, DefaultBackground, 0);

        int maxColumnsWidth = GetMaxColumnsBasedOnColumnSizes();
        int maxRowsHeight = GetMaxRowsBasedOnRowSizes();
        HashSet<int> allRowIndexesWithContent = GetIndexesWithContent(Table.TableCells.Layout.LayoutType.Row);
        HashSet<int> allColumnIndexesWithContent = GetIndexesWithContent(Table.TableCells.Layout.LayoutType.Column);

        if (DrawFakeCells && maxColumnsWidth < Width)
            maxColumnsWidth = Width;
        if (DrawFakeCells && maxRowsHeight < Height)
            maxRowsHeight = Height;

        SetScrollBarVisibility(maxRowsHeight, maxColumnsWidth);

        int rows = 0;
        int rowIndexPos = Cells.GetIndexAtCellPosition(StartRenderYPos, Table.TableCells.Layout.LayoutType.Row, out _);
        int rowIndex = IsVerticalScrollBarVisible ? rowIndexPos : 0;
        bool offScreenY = false;
        List<((int x, int y), (int row, int col))>? fakeCells = DrawFakeCells ? new() : null;
        for (int row = 0; row <= Height; row++)
        {
            if (rows >= allRowIndexesWithContent.Count) break;
            if (allRowIndexesWithContent.Contains(row)) rows++;

            int columns = 0;

            // Check if entire row is !IsVisible, then skip this row index entirely
            bool entireRowNotVisible = IsEntireRowOrColumnNotVisible(rowIndex, Table.TableCells.Layout.LayoutType.Row);

            int colIndexPos = Cells.GetIndexAtCellPosition(StartRenderXPos, Table.TableCells.Layout.LayoutType.Column, out _);
            int colIndex = IsHorizontalScrollBarVisible ? colIndexPos : 0;
            int fullRowSize = 0;

            bool headerRow = false;
            for (int col = 0; col <= Width; col++)
            {
                if (columns >= allColumnIndexesWithContent.Count) break;
                if (allColumnIndexesWithContent.Contains(col)) columns++;

                int verticalScrollBarValue = IsVerticalScrollBarVisible ? StartRenderYPos : 0;
                int horizontalScrollBarValue = IsHorizontalScrollBarVisible ? StartRenderXPos : 0;

                // Keep header row at the top of the table
                if (row == 0 && Cells.HeaderRow && verticalScrollBarValue > 0)
                {
                    headerRow = true;
                    rowIndex = 0;
                    verticalScrollBarValue = 0;
                }

                Point cellPosition = Cells.GetCellPosition(rowIndex, colIndex, out fullRowSize, out int columnSize,
                    verticalScrollBarValue, horizontalScrollBarValue);

                col += columnSize - 1;

                fakeCells?.Add((cellPosition, (rowIndex, colIndex)));

                // Check if entire column is !IsVisible, then skip this column index entirely
                bool entireRowOrColumnNotVisible = entireRowNotVisible ||
                    IsEntireRowOrColumnNotVisible(colIndex, Table.TableCells.Layout.LayoutType.Column);
                if (!headerRow && entireRowOrColumnNotVisible)
                {
                    colIndex++;
                    continue;
                }

                // Don't attempt to render off-screen rows/columns
                if (cellPosition.X > Width || cellPosition.Y > Height)
                {
                    if (cellPosition.Y > Height)
                    {
                        offScreenY = true;
                        break;
                    }

                    if (cellPosition.X > Width)
                        break;
                }

                int oldRow = -1, oldCol = -1;
                GetOldRowAndColumnValues(fakeCells, cellPosition, ref oldRow, ref oldCol);

                Table.Cell? cell = Cells.GetIfExists(rowIndex, colIndex, true);
                if (cell == null && DrawFakeCells)
                {
                    cell = Table.Cell.InternalCreate(rowIndex, colIndex, this, string.Empty, addToTableIfModified: false);
                    cell._position = cellPosition;
                    cell._row = oldRow != -1 ? oldRow : rowIndex;
                    cell._column = oldCol != -1 ? oldCol : colIndex;
                }
                else if (cell != null)
                {
                    cell._position = cellPosition;
                    cell._row = oldRow != -1 ? oldRow : rowIndex;
                    cell._column = oldCol != -1 ? oldCol : colIndex;
                }

                if (cell == null)
                {
                    HideVisualCell( colIndex, rowIndex, cellPosition);

                    colIndex++;
                    continue;
                }

                // This method raises an event that the user can use to modify the cell layout
                if (DrawFakeCells || (cell.IsSettingsInitialized && cell.Settings.UseFakeLayout))
                    DrawFakeCell(cell);

                AdjustControlSurface( cell, GetCustomStateAppearance(cell), !entireRowOrColumnNotVisible);
                PrintText( cell);

                colIndex++;
            }

            // Make sure the next rows are properly handled if header row is enabled
            if (row == 0 && Cells.HeaderRow && StartRenderYPos > 0)
                rowIndex = rowIndexPos;

            if (offScreenY) break;

            row += fullRowSize - 1;
            rowIndex++;
        }

        IsDirty = false;
    }

    protected void GetOldRowAndColumnValues(List<((int x, int y), (int row, int col))>? fakeCells, Point cellPosition, ref int oldRow, ref int oldCol)
    {
        if (fakeCells == null)
        {
            foreach (Table.Cell cellV in Cells)
            {
                if (cellV._position == cellPosition)
                {
                    oldRow = cellV.Row;
                    oldCol = cellV.Column;
                    break;
                }
            }
        }
        else
        {
            foreach (((int x, int y), (int row, int col)) cellV in fakeCells)
            {
                if (cellV.Item1 == cellPosition)
                {
                    oldRow = cellV.Item2.row;
                    oldCol = cellV.Item2.col;
                    break;
                }
            }
        }
    }

    private void SetScrollBarPropertiesOnTable(ScrollBar scrollBar, int maxRowsHeight, int maxColumnsWidth)
    {
        if (scrollBar != null)
        {
            int total = scrollBar.Orientation == Orientation.Vertical ?
                (maxRowsHeight >= Height ? Height : maxRowsHeight) :
                (maxColumnsWidth >= Width ? Width : maxColumnsWidth);

            int max = scrollBar.Orientation == Orientation.Vertical ? Height : Width;

            if (scrollBar.Orientation == Orientation.Vertical)
            {
                VisibleRowsTotal = total;
                VisibleRowsMax = max;
            }
            else
            {
                VisibleColumnsTotal = total;
                VisibleColumnsMax = max;
            }
        }
    }

    protected void SetScrollBarVisibility(int maxRowsHeight, int maxColumnsWidth)
    {
        if (_checkScrollBarVisibility)
        {
            if (VerticalScrollBar != null)
            {
                IsVerticalScrollBarVisible = ShowHideScrollBar(VerticalScrollBar);
                SetScrollBarPropertiesOnTable(VerticalScrollBar, maxRowsHeight, maxColumnsWidth);
            }
            if (HorizontalScrollBar != null)
            {
                IsHorizontalScrollBarVisible = ShowHideScrollBar(HorizontalScrollBar);
                SetScrollBarPropertiesOnTable(HorizontalScrollBar, maxRowsHeight, maxColumnsWidth);
            }
        }
    }

    protected ColoredGlyphBase? GetCustomStateAppearance(Table.Cell cell)
    {
        if (!cell.IsVisible || (cell.IsSettingsInitialized && !cell.Settings.Interactable))
            return null;

        if ((!cell.IsSettingsInitialized || cell.Settings.Selectable) && SelectedCell != null)
        {
            Table.TableCells.Layout.Mode selectionMode = !cell.IsSettingsInitialized ? DefaultSelectionMode : cell.Settings.SelectionMode;
            switch (selectionMode)
            {
                case Table.TableCells.Layout.Mode.Single:
                    if (cell._row != SelectedCell._row ||
                        cell._column != SelectedCell._column) break;
                    return ThemeState.Selected;
                case Table.TableCells.Layout.Mode.EntireRow:
                    if (cell._row != SelectedCell._row) break;
                    return ThemeState.Selected;
                case Table.TableCells.Layout.Mode.EntireColumn:
                    if (cell._column != SelectedCell._column) break;
                    return ThemeState.Selected;
                case Table.TableCells.Layout.Mode.None:
                    break;
            }
        }

        Table.TableCells.Layout.Mode hoverMode = !cell.IsSettingsInitialized ? DefaultHoverMode : cell.Settings.HoverMode;
        switch (hoverMode)
        {
            case Table.TableCells.Layout.Mode.Single:
                if (CurrentMouseCell == null || cell._row != CurrentMouseCell._row ||
                        cell._column != CurrentMouseCell._column) break;
                return ThemeState.MouseOver;
            case Table.TableCells.Layout.Mode.EntireRow:
                if (CurrentMouseCell == null || CurrentMouseCell._row != cell._row) break;
                return ThemeState.MouseOver;
            case Table.TableCells.Layout.Mode.EntireColumn:
                if (CurrentMouseCell == null || CurrentMouseCell._column != cell._column) break;
                return ThemeState.MouseOver;
            case Table.TableCells.Layout.Mode.None:
                break;
        }
        return null;
    }

    protected void AdjustControlSurface(Table.Cell cell, ColoredGlyphBase? customStateAppearance, bool adjustVisibility)
    {
        int width = Cells.GetSizeOrDefault(cell.Column, Table.TableCells.Layout.LayoutType.Column);
        int height = Cells.GetSizeOrDefault(cell.Row, Table.TableCells.Layout.LayoutType.Row);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int colIndex = cell._position.X + x;
                int rowIndex = cell._position.Y + y;
                if (!Surface.IsValidCell(colIndex, rowIndex)) continue;
                if (adjustVisibility)
                    Surface[colIndex, rowIndex].IsVisible = cell.IsVisible;
                Surface.SetForeground(colIndex, rowIndex, customStateAppearance != null ? customStateAppearance.Foreground : cell.Foreground);
                Surface.SetBackground(colIndex, rowIndex, customStateAppearance != null ? customStateAppearance.Background : cell.Background);
            }
        }
    }

    protected void HideVisualCell(int column, int row, Point position)
    {
        int width = Cells.GetSizeOrDefault(column, Table.TableCells.Layout.LayoutType.Column);
        int height = Cells.GetSizeOrDefault(row, Table.TableCells.Layout.LayoutType.Row);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int colIndex = position.X + x;
                int rowIndex = position.Y + y;
                if (!Surface.IsValidCell(colIndex, rowIndex))
                {
                    continue;
                }

                Surface[colIndex, rowIndex].IsVisible = true;
                Surface.SetForeground(colIndex, rowIndex, DefaultForeground);
                Surface.SetBackground(colIndex, rowIndex, DefaultBackground);
            }
        }
    }

    protected void PrintText(Table.Cell cell)
    {
        if (string.IsNullOrWhiteSpace(cell.StringValue) || !cell.IsVisible) return;

        int width = Cells.GetSizeOrDefault(cell.Column, Table.TableCells.Layout.LayoutType.Column);
        int height = Cells.GetSizeOrDefault(cell.Row, Table.TableCells.Layout.LayoutType.Row);

        // Handle alignments
        Table.Cell.Options.VerticalAlign vAlign = cell.IsSettingsInitialized ? cell.Settings.VerticalAlignment : default;
        Table.Cell.Options.HorizontalAlign hAlign = cell.IsSettingsInitialized ? cell.Settings.HorizontalAlignment : default;
        GetTotalCellSize(cell, width, height, out int totalWidth, out int totalHeight);

        // Set the amount of characters to split on for wrapping
        int maxCharsPerLine = cell.IsSettingsInitialized ? (cell.Settings.MaxCharactersPerLine ?? width) : width;
        if (maxCharsPerLine > width)
            maxCharsPerLine = width;

        // Split the character array into parts based on cell width
        string[] splittedTextArray = cell.StringValue.WordWrap(maxCharsPerLine).ToArray();
        int yIndex = 0;
        for (int y = 0; y < height; y++)
        {
            // Don't go out of bounds of the cell height
            if (splittedTextArray.Length <= y)
                break;

            // Print each array to the correct y index
            // Remove spaces in the front on the newline
            char[] textArr = splittedTextArray[y].SkipWhile(a => a == ' ').ToArray();
            int startPosX = GetHorizontalAlignment(hAlign, totalWidth, textArr);
            int startPosY = GetVerticalAlignment(vAlign, totalHeight, splittedTextArray);

            int index = 0;
            foreach (char character in textArr)
            {
                if (yIndex >= height)
                {
                    y = height;
                    break;
                }

                if (character == '\n')
                {
                    yIndex++;
                    index = 0;
                    continue;
                }
                Surface.SetGlyph(startPosX + cell._position.X + index++, startPosY + cell._position.Y + yIndex, character);
            }
            if (index != 0)
                yIndex++;
        }
    }

    protected static void GetTotalCellSize(Table.Cell cell, int width, int height, out int totalWidth, out int totalHeight)
    {
        int startX = cell._position.X;
        int startY = cell._position.Y;
        int endX = cell._position.X + width;
        int endY = cell._position.Y + height;
        totalWidth = endX - startX;
        totalHeight = endY - startY;
    }

    protected static int GetHorizontalAlignment(Table.Cell.Options.HorizontalAlign hAlign, int totalWidth, char[] textArr)
    {
        int startPosX = 0;
        switch (hAlign)
        {
            case Table.Cell.Options.HorizontalAlign.Left:
                startPosX = 0;
                break;
            case Table.Cell.Options.HorizontalAlign.Center:
                startPosX = (totalWidth - textArr.Length) / 2;
                break;
            case Table.Cell.Options.HorizontalAlign.Right:
                startPosX = totalWidth - textArr.Length;
                break;
        }
        return startPosX;
    }

    private static int GetVerticalAlignment(Table.Cell.Options.VerticalAlign vAlign, int totalHeight, IEnumerable<char>[] textArrs)
    {
        int position = 0;
        switch (vAlign)
        {
            case Table.Cell.Options.VerticalAlign.Top:
                position = 0;
                break;
            case Table.Cell.Options.VerticalAlign.Center:
                position = (totalHeight - textArrs.Length) / 2;
                break;
            case Table.Cell.Options.VerticalAlign.Bottom:
                position = totalHeight - textArrs.Length;
                break;
        }
        return position;
    }
}
