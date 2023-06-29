using System;
using System.Collections.Generic;
using SadRogue.Primitives;
using CLI = SadConsole.ICellSurface.ConnectedLineIndex;

namespace SadConsole.UI.Controls;


public class TabControl: TabControlBase
{
    private Orientation _tabOrientation;
    private int[] _connectedLineStyle;

    /// <summary>
    /// Sets the position of the tab strip to the top or bottom of the control.
    /// </summary>
    public Orientation TabOrientation
    {
        get => _tabOrientation;
        set { _tabOrientation = value; ThemeDetermineContentRegion(); IsDirty = true; }
    }

    /// <summary>
    /// An array of glyphs indexed by <see cref="ICellSurface.ConnectedLineIndex"/>. Defaults to <see cref="ICellSurface.ConnectedLineThin"/>.
    /// </summary>
    public int[] ConnectedLineStyle { get => _connectedLineStyle; set { _connectedLineStyle = value; IsDirty = true; } }

    /// <summary>
    /// Creates a new tab control with the specified tab items.
    /// </summary>
    /// <param name="tabItems">Tabs that are present on the tabcontrol</param>
    /// <param name="width">Width of the content area</param>
    /// <param name="height">Heigh of the content area</param>
    public TabControl(IEnumerable<TabItem> tabItems, int width, int height) : this(tabItems, 0, width, height) { }

    /// <summary>
    /// Creates a new tab control with the specified tab items.
    /// </summary>
    /// <param name="tabItems">Tabs that are present on the tabcontrol</param>
    /// <param name="activeTabIndex">What tab to be active on initialization</param>
    /// <param name="width">Width of the content area</param>
    /// <param name="height">Heigh of the content area</param>
    public TabControl(IEnumerable<TabItem> tabItems, int activeTabIndex, int width, int height) : base(tabItems, activeTabIndex, width, height)
    {
        ConnectedLineStyle = ICellSurface.ConnectedLineThin;
        TabItems = new List<TabItem>(tabItems);
        SetActiveTab(activeTabIndex);
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        Surface.Fill(colors.ControlHostForeground, colors.ControlHostBackground, 0, Mirror.None);
        Surface.DrawBox(ContentRegion.Expand(1, 1), ShapeParameters.CreateStyledBox(ConnectedLineStyle, new ColoredGlyph(colors.Lines), false, true));

        int x = TabsRegion.X;
        int y = TabsRegion.Y;
        bool handledActiveTab = false;
        bool isTop = TabOrientation == Orientation.Top;

        for (int i = 0; i < TabItems.Count; i++)
        {
            TabItem tabItem = TabItems[i];

            // Figure out the space the header text takes up
            string header;
            bool isMouseOver = false;
            ColoredGlyph linesGlyph = new ColoredGlyph(colors.Lines);

            if (tabItem.TabSize == -1)
                header = $" {tabItem.Header} ";
            else
                header = tabItem.Header.Align(tabItem.TextAlignment, tabItem.TabSize);

            if (header.Length + x >= Width)
                header = header.Substring(0, header.Length - Width + x - 1);

            // Cache the headers area
            tabItem.ThemeHeaderArea = new Rectangle(x, y, header.Length + 2, 3);

            // Flag mouse over
            if (MouseState_IsMouseOver)
            {
                if (isTop)
                    tabItem.ThemeHeaderMouseArea = tabItem.ThemeHeaderArea.ChangeHeight(-1);
                else
                    tabItem.ThemeHeaderMouseArea = tabItem.ThemeHeaderArea.ChangeY(1);

                if (tabItem.ThemeHeaderMouseArea.Contains(CachedMousePosition))
                    isMouseOver = true;
            }

            // Draw the header
            if (i == ActiveTabIndex)
            {
                Surface.DrawBox(tabItem.ThemeHeaderArea, ShapeParameters.CreateStyledBox(ConnectedLineStyle, linesGlyph, false, true));

                // Bottom/Top line
                Surface.SetGlyph(x, y + (isTop ? 2 : 0), ConnectedLineStyle[(int)(isTop ? CLI.BottomRight : CLI.TopRight)]);
                for (int lineX = x + 1; lineX < tabItem.ThemeHeaderArea.MaxExtentX; lineX++)
                    Surface.SetGlyph(lineX, y + (isTop ? 2 : 0), 0);
                Surface.SetGlyph(tabItem.ThemeHeaderArea.MaxExtentX, y + (isTop ? 2 : 0), ConnectedLineStyle[(int)(isTop ? CLI.BottomLeft : CLI.TopLeft)]);

                Surface.Print(x + 1, y + 1, header, ThemeState.Selected);

                handledActiveTab = true;
            }
            else
            {
                if (handledActiveTab)
                {
                    // Cull the left lines, only draw the right side of the box
                    Surface.DrawLine(tabItem.ThemeHeaderArea.Position + new Point(0, isTop ? 0 : 2), tabItem.ThemeHeaderArea.Position.Translate((tabItem.ThemeHeaderArea.Width - 1, isTop ? 0 : 2)), ConnectedLineStyle[(int)(isTop ? CLI.Top : CLI.Bottom)], ThemeState.Disabled.Foreground);
                    Surface.SetGlyph(tabItem.ThemeHeaderArea.MaxExtentX, isTop ? y : y + 2, ConnectedLineStyle[(int)(isTop ? CLI.TopRight : CLI.BottomRight)], ThemeState.Disabled.Foreground);
                    Surface.SetGlyph(tabItem.ThemeHeaderArea.MaxExtentX, y + 1, ConnectedLineStyle[(int)CLI.Right], ThemeState.Disabled.Foreground);
                }
                else
                {
                    // Cull the right lines, only draw the left side of the box
                    Surface.DrawLine(tabItem.ThemeHeaderArea.Position + new Point(0, isTop ? 0 : 2), tabItem.ThemeHeaderArea.Position.Translate((tabItem.ThemeHeaderArea.Width - 1, isTop ? 0 : 2)), ConnectedLineStyle[(int)(isTop ? CLI.Top : CLI.Bottom)], ThemeState.Disabled.Foreground);
                    Surface.SetGlyph(tabItem.ThemeHeaderArea.X, isTop ? y : y + 2, ConnectedLineStyle[(int)(isTop ? CLI.TopLeft : CLI.BottomLeft)], ThemeState.Disabled.Foreground);
                    Surface.SetGlyph(tabItem.ThemeHeaderArea.X, y + 1, ConnectedLineStyle[(int)CLI.Left], ThemeState.Disabled.Foreground);
                }

                Surface.Print(x + 1, y + 1, header, isMouseOver ? ThemeState.MouseOver : ThemeState.Normal);
            }

            // Move the header printing area
            x += header.Length + 2;

            if (x >= Width)
                break;
        }

        base.UpdateAndRedraw(time);

        IsDirty = false;
    }

    /// <inheritdoc/>
    protected override void ThemeDetermineContentRegion()
    {
        if (TabOrientation == Orientation.Top)
        {
            TabsRegion = new Rectangle(1, 0, Width - 2, 3);
            ContentRegion = new(1, 3, Width - 2, Height - 4);
        }
        else
        {
            TabsRegion = new Rectangle(1, Height - 3, Width - 2, 3);
            ContentRegion = new(1, 1, Width - 2, Height - 4);
        }

        if (ActiveTabIndex != InvalidActiveTabIndex)
        {
            TabItems[ActiveTabIndex].Content.Position = ContentRegion.Position;
            TabItems[ActiveTabIndex].Content.Resize(ContentRegion.Width, ContentRegion.Height);
        }
    }

    /// <summary>
    /// Horizontal alignment modes
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// Tabs should be placed at the top of the control.
        /// </summary>
        Top,

        /// <summary>
        /// Tabs should be placed at the bottom of the control.
        /// </summary>
        Bottom
    }
}
