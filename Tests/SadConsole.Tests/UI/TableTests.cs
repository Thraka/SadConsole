using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.Tests.UI;

/// <summary>
/// Tests the code structure of the table, that is accessible to the users
/// </summary>
[TestClass]
public class TableTests : TableTestsBase
{
    public TableTests()
        : base(60, 40, 10, 4)
    { }

    [TestMethod]
    public void CellsIndexer_CreatesNewCell()
    {
        Assert.AreEqual(Table.Cells.Count, 0);
        Assert.AreNotEqual(Table.Cells[0, 0], null);
        Assert.AreEqual(Table.Cells.Count, 1);
    }

    [TestMethod]
    public void Cells_GetCell_CreatesNewCell()
    {
        Assert.AreEqual(Table.Cells.Count, 0);
        Assert.AreNotEqual(Table.Cells.GetCell(0, 0), null);
        Assert.AreEqual(Table.Cells.Count, 1);
    }

    [TestMethod]
    public void Cells_Cell_SetLayout()
    {
        var settings = new Table.Cell.Options(Table)
        {
            Interactable = false
        };
        Table.Cells[0, 0].SetLayout(Color.Green, Color.Black, settings);
        Assert.AreEqual(Table.Cells.Row(0).Foreground, Color.Green);
        Assert.AreEqual(Table.Cells.Column(0).Background, Color.Black);
        Assert.AreEqual(Table.Cells.Row(0).Settings, settings);
        Assert.AreEqual(Table.Cells.Column(0).Settings, settings);
    }

    [TestMethod]
    public void ResizeCell_SetsLayout()
    {
        Table.Cells[0, 0].Resize();
        Assert.AreEqual(Table.Cells.Row(0).Size, Table.DefaultCellSize.Y);
        Assert.AreEqual(Table.Cells.Column(0).Size, Table.DefaultCellSize.X);

        Table.Cells[0, 0].Resize(2, 4);
        Assert.AreEqual(Table.Cells.Row(0).Size, 2);
        Assert.AreEqual(Table.Cells.Column(0).Size, 4);

        Table.Cells[0, 0].Resize(rowSize: 8);
        Assert.AreEqual(Table.Cells.Row(0).Size, 8);
        Assert.AreEqual(Table.Cells.Column(0).Size, 4);

        Table.Cells[0, 0].Resize(columnSize: 5);
        Assert.AreEqual(Table.Cells.Row(0).Size, 8);
        Assert.AreEqual(Table.Cells.Column(0).Size, 5);
    }

    [TestMethod]
    public void Cells_Range_ReturnsCorrectCells()
    {
        // 0 based, 0 -> 4 = 5 indexes
        Table.Cell[] cells = Table.Cells.Range(0, 0, 4, 4).ToArray();
        Assert.AreEqual(cells.Length, 25);
        int y = 0, x = 0;
        foreach (Table.Cell cell in cells)
        {
            Assert.AreEqual(cell.Row, y);
            Assert.AreEqual(cell.Column, x);
            if (y == 4)
            {
                y = 0;
                x++;
            }
            else
            {
                y++;
            }
        }
    }

    [TestMethod]
    public void Cells_Range_ForEach_AppliesActionToAll()
    {
        Table.Cell[] cells = Table.Cells.Range(0, 0, 4, 4).ToArray();
        cells.ForEach((cell) => cell.Value = "Hello!");
        Assert.IsTrue(cells.All(a => a.StringValue == "Hello!"));
    }

    [TestMethod]
    public void Cells_Select_Deselect_Cell_Correct()
    {
        Assert.AreEqual(Table.SelectedCell, null);
        Table.Cells[0, 0].Select();
        Assert.AreEqual(Table.SelectedCell, Table.Cells[0, 0]);
        Table.Cells[0, 0].Deselect();
        Assert.AreEqual(Table.SelectedCell, null);
    }

    [TestMethod]
    public void Cells_Width_Height_Correct()
    {
        Table.Cells.Row(0).Size = 11;
        Assert.AreEqual(Table.Cells[0, 0].Height, 11);

        Table.Cells.Column(0).Size = 6;
        Assert.AreEqual(Table.Cells[0, 0].Width, 6);
    }

    [TestMethod]
    public void Cells_Layout_Settings_SetCorrectly()
    {
        Cells.Layout layout = Table.Cells.Row(0);
        layout.Settings.Selectable = false;
        Assert.AreEqual(Table.Cells[0, 0].Settings.Selectable, false);
    }

    [TestMethod]
    public void Cells_Layout_GetMultiple_Correct()
    {
        Cells.Layout.RangeEnumerable rowLayouts = Table.Cells.Row(0, 1, 2);
        Assert.AreEqual(rowLayouts.Count(), 3);
        Cells.Layout.RangeEnumerable columnLayouts = Table.Cells.Column(0, 1, 2);
        Assert.AreEqual(columnLayouts.Count(), 3);
    }

    [TestMethod]
    public void Cells_Layout_SetLayout_Multiple_AppliedToAll()
    {
        var settings = new Table.Cell.Options(Table)
        {
            Interactable = false
        };
        Cells.Layout.RangeEnumerable rowLayouts = Table.Cells.Row(0, 1, 2);
        rowLayouts.SetLayout(3, Color.White, Color.Orange, settings);
        foreach (Cells.Layout layout in rowLayouts)
        {
            Assert.AreEqual(layout.Size, 3);
            Assert.AreEqual(layout.Foreground, Color.White);
            Assert.AreEqual(layout.Background, Color.Orange);
            Assert.AreEqual(layout.Settings, settings);
        }
    }

    [TestMethod]
    public void Can_Enumerate_Cells()
    {
        Table.Cells[0, 0].Value = "Hello 1";
        Table.Cells[1, 0].Value = "Hello 2";
        int count = 0;
        foreach (Table.Cell cell in Table.Cells)
            count++;
        Assert.AreEqual(count, 2);
    }

    [TestMethod]
    public void Can_Enumerate_LayoutRange()
    {
        Cells.Layout.RangeEnumerable layouts = Table.Cells.Row(0, 1);
        int count = 0;
        foreach (Cells.Layout layout in layouts)
            count++;
        Assert.AreEqual(count, 2);
    }

    [TestMethod]
    public void Total_Count_IsCorrect()
    {
        Table.Cells[0, 0].Value = "Col / Row 1";
        Table.Cells[1, 0].Value = "Row 2";
        Table.Cells[2, 0].Value = "Row 3";
        Table.Cells[0, 1].Value = "Column 2";
        Assert.AreEqual(Table.Cells.MaxColumn, 1);
        Assert.AreEqual(Table.Cells.MaxRow, 2);
    }

    [TestMethod]
    public void Cells_Remove_Correct()
    {
        Table.Cells[0, 0].Value = "Hello";
        Table.Cells[1, 0].Value = "Hello";
        Assert.AreEqual(Table.Cells.Count, 2);
        Table.Cells.Remove(0, 0);
        Assert.AreEqual(Table.Cells.Count, 1);
        Table.Cells[1, 0].Remove();
        Assert.AreEqual(Table.Cells.Count, 0);
    }

    [TestMethod]
    public void Cells_Clear_Correct()
    {
        Table.Cells.Row(0).Size = 4;
        Table.Cells[0, 0].Value = "Hello";
        Table.Cells[0, 1].Value = "Hello";

        Assert.AreEqual(Table.Cells.Count, 2);

        Table.Cells.Clear(false);
        Assert.AreEqual(Table.Cells.Count, 0);
        Assert.AreEqual(Table.Cells.Row(0).Size, 4);
        Assert.AreEqual(Table.Cells.Row(0).Size, 4);

        Table.Cells.Clear(true);

        Assert.AreEqual(Table.Cells.Row(0).Size, Table.DefaultCellSize.Y);
    }

    [TestMethod]
    public void Cells_Different_Cell_NotEquals()
    {
        Table.Cell cellA = Table.Cells[0, 0];
        cellA.Value = "Hello";

        Table.Cell cellB = Table.Cells[0, 1];
        cellB.Value = "Hello";

        Assert.AreNotEqual(cellA, cellB);

        cellB = null;

        Assert.AreNotEqual(cellA, cellB);
    }

    [TestMethod]
    public void Cells_Cell_CopyAppearanceFrom_Correct()
    {
        Table.Cell cellA = Table.Cells[0, 0];
        cellA.Background = Color.Brown;
        cellA.Settings.UseFakeLayout = true;
        cellA.Value = "Not Hello World!";

        Table.Cell cellB = Table.Cells[0, 1];
        cellB.Value = "Hello World!";
        cellB.CopyAppearanceFrom(cellA);

        Assert.AreEqual(cellA.Value, "Not Hello World!");
        Assert.AreEqual(cellB.Value, "Hello World!");
        Assert.AreEqual(cellB.Background, Color.Brown);
        Assert.AreEqual(cellB.Settings.UseFakeLayout, true);

        cellA.Settings.Selectable = false;
        cellB.CopyAppearanceFrom(cellA);

        Assert.AreEqual(cellB.Settings.UseFakeLayout, true);
        Assert.AreEqual(cellB.Settings.Selectable, false);

        // Settings is not initialized here, copy it over, everything should be default again
        Table.Cell cellC = Table.Cells[0, 2];
        cellB.CopyAppearanceFrom(cellC);

        Assert.AreEqual(cellB.Settings.UseFakeLayout, false);
        Assert.AreEqual(cellB.Settings.Selectable, true);
    }

    [TestMethod]
    public void Table_ScrollBar_Vertical_Scrolling_EqualSizes_Correct()
    {
        const int extraRowsOffScreen = 5;
        Table.SetupScrollBar(Orientation.Vertical, 5, new Point(0, 0));

        int rows = (Table.Height / Table.DefaultCellSize.Y) + extraRowsOffScreen;
        for (int row = 0; row < rows; row++)
        {
            Table.Cells[row, 0].Value = "Row " + row;
        }

        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
        Assert.AreEqual(Table.IsVerticalScrollBarVisible, true);
        Assert.AreEqual(Table.VerticalScrollBar.Maximum, extraRowsOffScreen);
        Assert.AreEqual(Table.VerticalScrollBar.Value, 0);

        // Increment
        int totalHeight = 0;
        int maximum = Table.VerticalScrollBar.Maximum;
        for (int i = 0; i < maximum; i++)
        {
            totalHeight += GetLastVisibleCellSize(Table, Orientation.Vertical, true);
            totalHeight = totalHeight < 0 ? 0 : totalHeight;
            Table.VerticalScrollBar.Value += 1;
            Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
            Assert.AreEqual(Table.StartRenderYPos, totalHeight);
            Assert.AreEqual(Table.VerticalScrollBar.Maximum, maximum);
        }

        // Decrement
        for (int i = maximum; i > 0; i--)
        {
            totalHeight += GetLastVisibleCellSize(Table, Orientation.Vertical, false);
            totalHeight = totalHeight < 0 ? 0 : totalHeight;
            Table.VerticalScrollBar.Value -= 1;
            Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
            Assert.AreEqual(Table.StartRenderYPos, totalHeight);
            Assert.AreEqual(Table.VerticalScrollBar.Maximum, maximum);
        }

        Assert.AreEqual(Table.StartRenderYPos, 0);
    }

    [TestMethod]
    public void Table_ScrollBar_Horizontal_Scrolling_EqualSizes_Correct()
    {
        const int extraColumnsOffScreen = 5;
        Table.SetupScrollBar(Orientation.Horizontal, 5, new Point(0, 0));

        int columns = (Table.Width / Table.DefaultCellSize.X) + extraColumnsOffScreen;
        for (int column = 0; column < columns; column++)
        {
            Table.Cells[0, column].Value = "Column " + column;
        }

        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
        Assert.AreEqual(Table.IsHorizontalScrollBarVisible, true);
        Assert.AreEqual(Table.HorizontalScrollBar.Maximum, extraColumnsOffScreen);
        Assert.AreEqual(Table.HorizontalScrollBar.Value, 0);

        // Increment
        int totalWidth = 0;
        int maximum = Table.HorizontalScrollBar.Maximum;
        for (int i = 0; i < maximum; i++)
        {
            totalWidth += GetLastVisibleCellSize(Table, Orientation.Horizontal, true);
            totalWidth = totalWidth < 0 ? 0 : totalWidth;
            Table.HorizontalScrollBar.Value += 1;
            Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
            Assert.AreEqual(Table.StartRenderXPos, totalWidth);
            Assert.AreEqual(Table.HorizontalScrollBar.Maximum, maximum);
        }

        // Decrement
        for (int i = maximum; i > 0; i--)
        {
            totalWidth += GetLastVisibleCellSize(Table, Orientation.Horizontal, false);
            totalWidth = totalWidth < 0 ? 0 : totalWidth;
            Table.HorizontalScrollBar.Value -= 1;
            Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
            Assert.AreEqual(Table.StartRenderXPos, totalWidth);
            Assert.AreEqual(Table.HorizontalScrollBar.Maximum, maximum);
        }

        Assert.AreEqual(Table.StartRenderXPos, 0);
    }

    [TestMethod]
    public void Table_ScrollBar_Vertical_Scrolling_DifferentSizes_Correct()
    {
        const int extraRowsOffScreen = 5;
        Table.SetupScrollBar(Orientation.Vertical, 5, new Point(0, 0));

        int rows = (Table.Height / Table.DefaultCellSize.Y) + extraRowsOffScreen;
        for (int row = 0; row < rows; row++)
        {
            Table.Cells[row, 0].Value = "Row " + row;
        }

        // Resize columns
        Table.Cells[1, 0].Resize(rowSize: 4);
        Table.Cells[2, 0].Resize(rowSize: 8);
        Table.Cells[3, 0].Resize(rowSize: 1);

        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());

        int maximum = GetMaximumScrollBarItems(Table, Orientation.Vertical);
        Assert.AreEqual(Table.IsVerticalScrollBarVisible, true);
        Assert.AreEqual(Table.VerticalScrollBar.Maximum, maximum);
        Assert.AreEqual(Table.VerticalScrollBar.Value, 0);

        // Increment
        int totalHeight = 0;
        for (int i = 0; i < maximum; i++)
        {
            totalHeight += GetLastVisibleCellSize(Table, Orientation.Vertical, true);
            totalHeight = totalHeight < 0 ? 0 : totalHeight;
            Table.VerticalScrollBar.Value += 1;
            Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
            Assert.AreEqual(Table.StartRenderYPos, totalHeight);
            Assert.AreEqual(Table.VerticalScrollBar.Maximum, maximum);
        }

        // Decrement
        for (int i = maximum; i > 0; i--)
        {
            totalHeight += GetLastVisibleCellSize(Table, Orientation.Vertical, false);
            totalHeight = totalHeight < 0 ? 0 : totalHeight;
            Table.VerticalScrollBar.Value -= 1;
            Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
            Assert.AreEqual(Table.StartRenderYPos, totalHeight);
            Assert.AreEqual(Table.VerticalScrollBar.Maximum, maximum);
        }

        Assert.AreEqual(Table.StartRenderYPos, 0);
    }

    [TestMethod]
    public void Table_ScrollBar_Horizontal_Scrolling_DifferentSizes_Correct()
    {
        const int extraColumnsOffScreen = 5;
        Table.SetupScrollBar(SadConsole.Orientation.Horizontal, 5, new Point(0, 0));

        int columns = (Table.Width / Table.DefaultCellSize.X) + extraColumnsOffScreen;
        for (int column = 0; column < columns; column++)
        {
            Table.Cells[0, column].Value = "Column " + column;
        }

        // Resize columns
        Table.Cells[0, 1].Resize(columnSize: 4);
        Table.Cells[0, 2].Resize(columnSize: 8);
        Table.Cells[0, 3].Resize(columnSize: 1);

        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());

        int maximum = GetMaximumScrollBarItems(Table, Orientation.Horizontal);
        Assert.AreEqual(Table.IsHorizontalScrollBarVisible, true);
        Assert.AreEqual(Table.HorizontalScrollBar.Maximum, maximum);
        Assert.AreEqual(Table.HorizontalScrollBar.Value, 0);

        // Increment
        int totalWidth = 0;
        for (int i = 0; i < maximum; i++)
        {
            totalWidth += GetLastVisibleCellSize(Table, Orientation.Horizontal, true);
            totalWidth = totalWidth < 0 ? 0 : totalWidth;
            Table.HorizontalScrollBar.Value += 1;
            Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
            Assert.AreEqual(Table.StartRenderXPos, totalWidth);
            Assert.AreEqual(Table.HorizontalScrollBar.Maximum, maximum);
        }

        for (int i = maximum; i > 0; i--)
        {
            totalWidth += GetLastVisibleCellSize(Table, Orientation.Horizontal, false);
            totalWidth = totalWidth < 0 ? 0 : totalWidth;
            Table.HorizontalScrollBar.Value -= 1;
            Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
            Assert.AreEqual(Table.StartRenderXPos, totalWidth);
            Assert.AreEqual(Table.HorizontalScrollBar.Maximum, maximum);
        }

        Assert.AreEqual(Table.StartRenderXPos, 0);
    }

    [TestMethod]
    public void Table_ScrollBar_Horizontal_ChangeScrollMaximum_OnResize_Correct()
    {
        const int extraColumnsOffScreen = 5;
        Table.SetupScrollBar(Orientation.Horizontal, 5, new Point(0, 0));

        int columns = (Table.Width / Table.DefaultCellSize.X) + extraColumnsOffScreen;
        for (int column = 0; column < columns; column++)
        {
            Table.Cells[0, column].Value = "Column " + column;
        }

        // Resize columns
        Table.Cells[0, 1].Resize(columnSize: 4);
        Table.Cells[0, 2].Resize(columnSize: 8);
        Table.Cells[0, 3].Resize(columnSize: 1);

        Table.HorizontalScrollBar.Value = 1;
        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());

        int maximum = GetMaximumScrollBarItems(Table, Orientation.Horizontal);
        Assert.AreEqual(Table.IsHorizontalScrollBarVisible, true);
        Assert.AreEqual(Table.HorizontalScrollBar.Maximum, maximum);
        Assert.AreEqual(Table.HorizontalScrollBar.Value, 1);

        // Resize existing cell
        Table.Cells[0, 1].Resize(columnSize: 9);
        Table.Cells[0, 2].Resize(columnSize: 16);
        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
        Assert.AreNotEqual(Table.HorizontalScrollBar.Maximum, maximum);

        // Update max
        maximum = GetMaximumScrollBarItems(Table, Orientation.Horizontal);
        Assert.AreEqual(Table.HorizontalScrollBar.Maximum, maximum);
    }

    [TestMethod]
    public void Table_ScrollBar_Vertical_ChangeScrollMaximum_OnResize_Correct()
    {
        const int extraRowsOffScreen = 5;
        Table.SetupScrollBar(Orientation.Vertical, 5, new Point(0, 0));

        int rows = (Table.Height / Table.DefaultCellSize.Y) + extraRowsOffScreen;
        for (int row = 0; row < rows; row++)
        {
            Table.Cells[row, 0].Value = "Row " + row;
        }

        // Resize columns
        Table.Cells[1, 0].Resize(rowSize: 4);
        Table.Cells[2, 0].Resize(rowSize: 8);
        Table.Cells[3, 0].Resize(rowSize: 1);

        Table.VerticalScrollBar.Value = 1;
        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());

        int maximum = GetMaximumScrollBarItems(Table, Orientation.Vertical);
        Assert.AreEqual(Table.IsVerticalScrollBarVisible, true);
        Assert.AreEqual(Table.VerticalScrollBar.Maximum, maximum);
        Assert.AreEqual(Table.VerticalScrollBar.Value, 1);

        // Resize existing cell
        Table.Cells[1, 0].Resize(rowSize: 9);
        Table.Cells[2, 0].Resize(rowSize: 16);
        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());
        Assert.AreNotEqual(Table.VerticalScrollBar.Maximum, maximum);

        // Update max
        maximum = GetMaximumScrollBarItems(Table, Orientation.Vertical);
        Assert.AreEqual(Table.VerticalScrollBar.Maximum, maximum);
    }

    [TestMethod]
    public void Table_ScrollBar_ScrollToSelectedItem_Vertical_Works()
    {
        const int extraRowsOffScreen = 5;
        Table.SetupScrollBar(Orientation.Vertical, 5, new Point(0, 0));

        int rows = (Table.Height / Table.DefaultCellSize.Y) + extraRowsOffScreen;
        for (int row = 0; row < rows; row++)
        {
            Table.Cells[row, 0].Value = "Row " + row;
        }

        // Resize columns
        Table.Cells[1, 0].Resize(rowSize: 4);
        Table.Cells[2, 0].Resize(rowSize: 8);
        Table.Cells[3, 0].Resize(rowSize: 1);

        Table.VerticalScrollBar.Value = 0;
        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());

        Table.Cells.Select(rows - 1, 0);
        Table.ScrollToSelectedItem();

        Assert.AreNotEqual(Table.VerticalScrollBar.Value, 0);
    }

    [TestMethod]
    public void Table_ScrollBar_ScrollToSelectedItem_Horizontal_Works()
    {
        const int extraColumnsOffScreen = 5;
        Table.SetupScrollBar(Orientation.Horizontal, 5, new Point(0, 0));

        int columns = (Table.Width / Table.DefaultCellSize.X) + extraColumnsOffScreen;
        for (int col = 0; col < columns; col++)
        {
            Table.Cells[0, col].Value = "Col " + col;
        }

        // Resize columns
        Table.Cells[0, 1].Resize(columnSize: 4);
        Table.Cells[0, 2].Resize(columnSize: 8);
        Table.Cells[0, 3].Resize(columnSize: 1);

        Table.HorizontalScrollBar.Value = 0;
        Table.Theme.UpdateAndDraw(Table, new System.TimeSpan());

        Table.Cells.Select(0, columns - 1);
        Table.ScrollToSelectedItem();

        Assert.AreNotEqual(Table.HorizontalScrollBar.Value, 0);
    }
}

public abstract class TableTestsBase
{
    protected Table Table { get; set; }
    protected readonly int Width, Height, CellWidth, CellHeight;

    protected TableTestsBase(int width, int height, int cellWidth, int cellHeight)
    {
        Width = width;
        Height = height;
        CellWidth = cellWidth;
        CellHeight = cellHeight;

        Library.Default.SetControlTheme(typeof(Table), typeof(TableTheme));
    }

    [TestInitialize]
    public virtual void Setup()
    {
        Table = new Table(Width, Height, CellWidth, CellHeight);
    }

    protected static int GetLastVisibleCellSize(Table table, Orientation orientation, bool increment)
    {
        Cells.Layout.LayoutType type = orientation == Orientation.Vertical ?
            Cells.Layout.LayoutType.Row : Cells.Layout.LayoutType.Column;
        bool isRowType = type == Cells.Layout.LayoutType.Row;
        System.Collections.Generic.IEnumerable<IGrouping<int, Table.Cell>> cellGroups = table.Cells.GroupBy(a => isRowType ? a.Row : a.Column);
        IOrderedEnumerable<IGrouping<int, Table.Cell>> orderedCells = increment ? cellGroups.OrderBy(a => a.Key) :
            cellGroups.OrderByDescending(a => a.Key);

        foreach (IGrouping<int, Table.Cell> group in orderedCells)
        {
            foreach (Table.Cell cell in group)
            {
                bool partialOverlap = false;
                int indexSizeCell = isRowType ? cell.Position.Y : cell.Position.X;
                if (!increment)
                {
                    // Check if cell position is the last cell on screen
                    if (indexSizeCell >= (isRowType ? table.Height : table.Width))
                        break;
                }
                else
                {
                    // Check if cell position is the next off-screen
                    // >= because it assumes the cell starts at Height, and thats off screen
                    bool isPositionOfScreen = isRowType ? indexSizeCell >= table.Height : indexSizeCell >= table.Width;
                    if (!isPositionOfScreen)
                    {
                        // Here it is only > because if the real cell pos is 20 its the ending, so where the next cell starts
                        // which means its not off screen
                        int realCellPosition = isRowType ? (cell.Position.Y + cell.Height) : (cell.Position.X + cell.Width);
                        if (realCellPosition > (isRowType ? table.Height : table.Width))
                        {
                            partialOverlap = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                int size = table.Cells.GetSizeOrDefault(indexSizeCell, type);
                if (partialOverlap)
                {
                    int overlapAmount = indexSizeCell + size - (isRowType ? table.Height : table.Width);
                    size = overlapAmount;
                }
                return increment ? size : -size;
            }
        }
        int defaultSize = type == Cells.Layout.LayoutType.Row ? table.DefaultCellSize.Y : table.DefaultCellSize.X;
        return increment ? defaultSize : -defaultSize;
    }

    protected static int GetMaximumScrollBarItems(Table table, Orientation orientation)
    {
        System.Collections.Generic.IEnumerable<IGrouping<int, Table.Cell>> indexes = orientation == Orientation.Vertical ?
            table.Cells.GroupBy(a => a.Row) : table.Cells.GroupBy(a => a.Column);
        IOrderedEnumerable<IGrouping<int, Table.Cell>> orderedIndex = indexes.OrderBy(a => a.Key);

        Cells.Layout.LayoutType layoutType = orientation == Orientation.Vertical ? Cells.Layout.LayoutType.Row : Cells.Layout.LayoutType.Column;
        int maxSize = orientation == Orientation.Vertical ? table.Height : table.Width;
        int totalSize = 0;
        int items = 0;
        foreach (IGrouping<int, Table.Cell> index in orderedIndex)
        {
            int size = table.Cells.GetSizeOrDefault(index.Key, layoutType);
            totalSize += size;

            if (totalSize > maxSize)
            {
                items++;
            }
        }
        return items;
    }
}
