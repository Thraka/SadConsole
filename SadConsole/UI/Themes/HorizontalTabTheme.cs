using System;
using System.Linq;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    public class HorizontalTabControlTheme : ThemeBase
    {
        public HorizontalTabPositions TabPosition { get; set; }

        public HorizontalTabControlTheme()
        {
            TabPosition = HorizontalTabPositions.Top;
        }

        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (control.IsDirty == false) return;
            if ((control is TabControl tabs) == false) return;

            RefreshTheme(control.FindThemeColors(), control);

            if (tabs.CurrentTab.TabButton.Theme.GetType() != typeof(ButtonTheme))
            {
                tabs.TabItems.ForEach(tab =>
                {
                    tab.TabButton.Theme = new ButtonTheme() { ShowEnds = false };
                    tab.TabButton.Resize(tab.TabButton.Text.Length, 1);
                });
            }

            tabs.Surface.Clear();

            int x = 0;
            int y = 0;

            Rectangle outerEdgeRect;
            if (TabPosition == HorizontalTabPositions.Top)
            {
                outerEdgeRect = new Rectangle(0, 2, tabs.Surface.Width, tabs.Surface.Height - 2);
            }
            else if (TabPosition == HorizontalTabPositions.Bottom)
            {
                outerEdgeRect = new Rectangle(0, 0, tabs.Surface.Width, tabs.Surface.Height - 2);
                y = tabs.Surface.Height - 3;

            }
            else throw new Exception();

            tabs.Surface.DrawBox(outerEdgeRect,
                ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
                    new ColoredGlyph(Color.DarkOrange, Color.Black)));

            int indexCounter = 0;
            foreach (TabItem tab in tabs.TabItems)
            {
                // Active tab
                if (indexCounter == tabs.ActiveTabIndex)
                {
                    if (TabPosition == HorizontalTabPositions.Top)
                    {
                        x = DrawActiveTabTop(tabs.Surface, x, y, tab);
                        if (tabs.EnforceConsolePosition) tab.Console.Position = tabs.AbsolutePosition.Translate(new Point(1, 3));
                    }
                    else if (TabPosition == HorizontalTabPositions.Bottom)
                    {
                        x = DrawActiveTabBottom(tabs.Surface, x, y, tab);
                        if (tabs.EnforceConsolePosition) tab.Console.Position = tabs.AbsolutePosition.Translate(new Point(1, 1));
                    }

                    if (tabs.UpdateConsoleViewRect) ((Console)tab.Console).View = new Rectangle(0, 0, tabs.Width - 2, tabs.Height - 4);
                }
                else
                {
                    if (TabPosition == HorizontalTabPositions.Top)
                    {
                        x = DrawInactiveTabTop(tabs.Surface, x, y, tab, indexCounter, tabs.ActiveTabIndex);
                    }
                    else if (TabPosition == HorizontalTabPositions.Bottom)
                    {
                        x = DrawInactiveTabBottom(tabs.Surface, x, y, tab, indexCounter, tabs.ActiveTabIndex);
                    }
                }

                indexCounter++;
            }
        }

        int DrawActiveTabTop(ICellSurface surface, int x, int y, TabItem tab)
        {
            int headerLength = tab.Header.Length > tab.TabSize ? tab.Header.Length : tab.TabSize;

            // Top Line
            surface.DrawLine(new Point(x + 2, y), new Point(x + 1 + headerLength, y), 196, Color.DarkOrange);
            // Clear bottom line
            surface.DrawLine(new Point(x + 2, y + 2), new Point(x + 1 + headerLength, y + 2), 0, Color.Black);
            // Sides
            surface.SetGlyph(x + 1, y + 1, 179, Color.DarkOrange);
            surface.SetGlyph(x + 2 + headerLength, y + 1, 179, Color.DarkOrange);
            // Top Corners
            surface.SetGlyph(x + 1, y, 218, Color.DarkOrange);
            surface.SetGlyph(x + 2 + headerLength, y, 191, Color.DarkOrange);
            // Bottom Corners
            surface.SetGlyph(x + 1, y + 2, 217, Color.DarkOrange);
            surface.SetGlyph(x + 2 + headerLength, y + 2, 192, Color.DarkOrange);
            // Text
            int padding = 0;
            if (tab.TextAlignment == HorizontalAlignment.Center)
            {
                padding = (headerLength - tab.Header.Length) / 2;
            }
            else if (tab.TextAlignment == HorizontalAlignment.Right)
            {
                padding = headerLength - tab.Header.Length;
            }
            if (padding < 0) padding = 0;

            surface.Print(x + 2 + padding, y + 1, tab.Header, Color.DarkOrange);
            // Button
            tab.TabButton.IsVisible = false;

            // Return end of tab position
            int nextX = x + 3 + headerLength;

            return nextX;
        }

        int DrawActiveTabBottom(ICellSurface surface, int x, int y, TabItem tab)
        {
            int headerLength = tab.Header.Length > tab.TabSize ? tab.Header.Length : tab.TabSize;

            // Bottom Line
            surface.DrawLine(new Point(x + 2, y + 2), new Point(x + 1 + headerLength, y + 2), 196, Color.DarkOrange);
            // Clear top line
            surface.DrawLine(new Point(x + 1, y), new Point(x + 2 + headerLength, y), 0, Color.Black);
            // Sides
            surface.SetGlyph(x + 1, y + 1, 179, Color.DarkOrange);
            surface.SetGlyph(x + 2 + headerLength, y + 1, 179, Color.DarkOrange);
            // Top Corners
            surface.SetGlyph(x + 1, y, 191, Color.DarkOrange);
            surface.SetGlyph(x + 2 + headerLength, y, 218, Color.DarkOrange);
            // Bottom Corners
            surface.SetGlyph(x + 1, y + 2, 192, Color.DarkOrange);
            surface.SetGlyph(x + 2 + headerLength, y + 2, 217, Color.DarkOrange);
            // Text
            int padding = 0;
            if (tab.TextAlignment == HorizontalAlignment.Center)
            {
                padding = (headerLength - tab.Header.Length) / 2;
            }
            else if (tab.TextAlignment == HorizontalAlignment.Right)
            {
                padding = headerLength - tab.Header.Length;
            }
            if (padding < 0) padding = 0;
            surface.Print(x + 2 + padding, y + 1, tab.Header, Color.DarkOrange);

            // Return end of tab position
            int nextX = x + 3 + headerLength;

            return nextX;
        }

        int DrawInactiveTabTop(ICellSurface surface, int x, int y, TabItem tab, int currentIndex, int activeIndex)
        {
            int headerLength = tab.Header.Length > tab.TabSize ? tab.Header.Length : tab.TabSize;

            int nextX = 0;
            if (currentIndex > activeIndex) // Current tab is to the right of the active tab
            {
                // Top line
                surface.DrawLine(new Point(x, y), new Point(x + headerLength - 1, y), 196, Color.DarkGray);
                // Corner
                surface.SetGlyph(x + headerLength, y, 191, Color.DarkGray);
                // Side
                surface.SetGlyph(x + headerLength, y + 1, 179, Color.DarkGray);
                // Button
                tab.TabButton.Position = new Point(x, y + 1);
                nextX = x + headerLength + 1;
            }
            else // Current tab is to the left of the active tab
            {
                // Tab is slightlight moved to the left, account for that
                x++;
                // Top line
                surface.DrawLine(new Point(x + 1, y), new Point(x + headerLength, y), 196, Color.DarkGray);
                // Corner
                surface.SetGlyph(x, y, 218, Color.DarkGray);
                // Side
                surface.SetGlyph(x, y + 1, 179, Color.DarkGray);
                // Button
                tab.TabButton.Position = new Point(x + 1, y + 1);
                nextX = x + headerLength;
            }

            tab.TabButton.Resize(headerLength, 1);
            tab.TabButton.TextAlignment = tab.TextAlignment;
            tab.TabButton.IsVisible = true;

            return nextX;
        }

        int DrawInactiveTabBottom(ICellSurface surface, int x, int y, TabItem tab, int currentIndex, int activeIndex)
        {
            int headerLength = tab.Header.Length > tab.TabSize ? tab.Header.Length : tab.TabSize;

            int nextX = 0;
            if (currentIndex > activeIndex) // Current tab is to the right of the active tab
            {
                // Top line
                surface.DrawLine(new Point(x, y + 2), new Point(x + headerLength - 1, y + 2), 196, Color.DarkGray);
                // Corner
                surface.SetGlyph(x + headerLength, y + 2, 217, Color.DarkGray);
                // Side
                surface.SetGlyph(x + headerLength, y + 1, 179, Color.DarkGray);
                // Button
                tab.TabButton.Position = new Point(x, y + 1);

                nextX = x + headerLength + 1;
            }
            else // Current tab is to the left of the active tab
            {
                // Tab is slightlight moved to the left, account for that
                x++;
                // Top line
                surface.DrawLine(new Point(x + 1, y + 2), new Point(x + headerLength, y + 2), 196, Color.DarkGray);
                // Corner
                surface.SetGlyph(x, y + 2, 192, Color.DarkGray);
                // Side
                surface.SetGlyph(x, y + 1, 179, Color.DarkGray);
                // Button
                tab.TabButton.Position = new Point(x + 1, y + 1);

                nextX = x + headerLength;
            }

            tab.TabButton.IsVisible = true;

            return nextX;
        }

        public override ThemeBase Clone() => new HorizontalTabControlTheme()
        {
            ControlThemeState = ControlThemeState.Clone()
        };
    }
}
