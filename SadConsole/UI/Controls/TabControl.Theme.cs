using System;
using System.Collections.Generic;
using SadRogue.Primitives;
using CLI = SadConsole.ICellSurface.ConnectedLineIndex;

namespace SadConsole.UI.Controls;

public partial class TabControl
{
    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (IsDirty)
        {
            Colors colors = FindThemeColors();

            RefreshThemeStateColors(colors);

            Surface.Fill(colors.ControlHostForeground, colors.ControlHostBackground, 0, Mirror.None);
            Surface.DrawBox(ContentRegion.Expand(1, 1), ShapeParameters.CreateStyledBox(ConnectedLineStyle, new ColoredGlyph(colors.Lines), false, true));

            (int x, int y) = TabsRegion.Position;
            bool handledActiveTab = false;

            // Top/Bottom tab mode
            if (TabOrientation == Orientation.Top || TabOrientation == Orientation.Bottom)
            {
                bool isTop = TabOrientation == Orientation.Top;

                for (int i = 0; i < TabItems.Count; i++)
                {
                    TabItem tabItem = TabItems[i];

                    // Figure out the space the header text takes up
                    string header;
                    bool isMouseOver = false;
                    ColoredGlyph linesGlyph = new ColoredGlyph(colors.Lines);

                    if (tabItem.TabSize == -1)
                        header = $"{new string(' ', tabItem.AutomaticPadding)}{tabItem.Header}{new string(' ', tabItem.AutomaticPadding)}";
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
                        {
                            Surface.SetGlyph(lineX, y + (isTop ? 2 : 0), 0);
                            Surface.SetGlyph(lineX, y + 1, header[lineX - (x + 1)], ThemeState.Selected.Foreground, ThemeState.Selected.Background);
                        }
                        Surface.SetGlyph(tabItem.ThemeHeaderArea.MaxExtentX, y + (isTop ? 2 : 0), ConnectedLineStyle[(int)(isTop ? CLI.BottomLeft : CLI.TopLeft)]);

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

                        for (int counter = 0; counter < header.Length; counter++)
                        {
                            Surface.SetCellAppearance(x + 1 + counter, y + 1, isMouseOver ? ThemeState.MouseOver : ThemeState.Normal);
                            Surface.SetGlyph(x + 1 + counter, y + 1, header[counter]);
                        }
                    }

                    // Move the header printing area
                    x += tabItem.ThemeHeaderArea.Width;

                    if (x >= Width)
                        break;
                }
            }

            // Left/Right tab mode
            else
            {
                bool isLeft = TabOrientation == Orientation.Left;

                for (int i = 0; i < TabItems.Count; i++)
                {
                    TabItem tabItem = TabItems[i];

                    // Figure out the space the header text takes up
                    string header;
                    bool isMouseOver = false;
                    ColoredGlyph linesGlyph = new ColoredGlyph(colors.Lines);

                    if (tabItem.TabSize == -1)
                        header = $"{new string(' ', tabItem.AutomaticPadding)}{tabItem.Header}{new string(' ', tabItem.AutomaticPadding)}";
                    else
                        header = tabItem.Header.Align(tabItem.TextAlignment, tabItem.TabSize);

                    if (header.Length + y >= Height)
                        header = header.Substring(0, Height - y - 1);

                    // Cache the headers area
                    tabItem.ThemeHeaderArea = new Rectangle(x, y, 3, header.Length + 2);

                    // Flag mouse over
                    if (MouseState_IsMouseOver)
                    {
                        if (isLeft)
                            tabItem.ThemeHeaderMouseArea = tabItem.ThemeHeaderArea.ChangeWidth(-1);
                        else
                            tabItem.ThemeHeaderMouseArea = tabItem.ThemeHeaderArea.ChangeX(1);

                        if (tabItem.ThemeHeaderMouseArea.Contains(CachedMousePosition))
                            isMouseOver = true;
                    }

                    // Draw the header
                    if (i == ActiveTabIndex)
                    {
                        Surface.DrawBox(tabItem.ThemeHeaderArea, ShapeParameters.CreateStyledBox(ConnectedLineStyle, linesGlyph, false, true));
                        // Left/Right line
                        Surface.SetGlyph(x + (isLeft ? 2 : 0), y, ConnectedLineStyle[(int)(isLeft ? CLI.BottomRight : CLI.BottomLeft)]);
                        for (int lineY = y + 1; lineY < tabItem.ThemeHeaderArea.MaxExtentY; lineY++)
                        {
                            Surface.SetGlyph(x + (isLeft ? 2 : 0), lineY, 0);
                            Surface.SetGlyph(x + 1, lineY, header[lineY - (y + 1)], ThemeState.Selected.Foreground, ThemeState.Selected.Background);
                        }
                        Surface.SetGlyph(x + (isLeft ? 2 : 0), tabItem.ThemeHeaderArea.MaxExtentY, ConnectedLineStyle[(int)(isLeft ? CLI.TopRight : CLI.TopLeft)]);

                        handledActiveTab = true;
                    }
                    else
                    {
                        if (handledActiveTab)
                        {
                            // Cull the top lines, only draw the bottom side of the box
                            Surface.DrawLine(tabItem.ThemeHeaderArea.Position + new Point(isLeft ? 0 : 2, 0), tabItem.ThemeHeaderArea.Position.Translate(isLeft ? 0 : 2, tabItem.ThemeHeaderArea.Height - 1), ConnectedLineStyle[(int)(isLeft ? CLI.Left : CLI.Right)], ThemeState.Disabled.Foreground);
                            Surface.SetGlyph(isLeft ? x : x + 2, tabItem.ThemeHeaderArea.MaxExtentY, ConnectedLineStyle[(int)(isLeft ? CLI.BottomLeft : CLI.BottomRight)], ThemeState.Disabled.Foreground);
                            Surface.SetGlyph(x + 1, tabItem.ThemeHeaderArea.MaxExtentY, ConnectedLineStyle[(int)CLI.Bottom], ThemeState.Disabled.Foreground);
                        }
                        else
                        {
                            // Cull the right lines, only draw the left side of the box
                            Surface.DrawLine(tabItem.ThemeHeaderArea.Position + new Point(isLeft ? 0 : 2, 0), tabItem.ThemeHeaderArea.Position.Translate(isLeft ? 0 : 2, tabItem.ThemeHeaderArea.Height - 1), ConnectedLineStyle[(int)(isLeft ? CLI.Left : CLI.Right)], ThemeState.Disabled.Foreground);
                            Surface.SetGlyph(isLeft ? x : x + 2, tabItem.ThemeHeaderArea.Y, ConnectedLineStyle[(int)(isLeft ? CLI.TopLeft : CLI.TopRight)], ThemeState.Disabled.Foreground);
                            Surface.SetGlyph(x + 1, tabItem.ThemeHeaderArea.Y, ConnectedLineStyle[(int)CLI.Bottom], ThemeState.Disabled.Foreground);
                        }

                        for (int counter = 0; counter < header.Length; counter++)
                        {
                            Surface.SetCellAppearance(x + 1, y + 1 + counter, isMouseOver ? ThemeState.MouseOver : ThemeState.Normal);
                            Surface.SetGlyph(x + 1, y + 1 + counter, header[counter]);
                        }
                    }

                    // Move the header printing area
                    y += header.Length + 2;

                    if (y >= Height)
                        break;
                }
            }

            IsDirty = false;
        }

        base.UpdateAndRedraw(time);
    }
}
