using System;
using System.Linq;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

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
			WriteLine("Reassigning tab button themes to horizontal");
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
			outerEdgeRect = new(0, 2, tabs.Surface.Width, tabs.Surface.Height - 2);
		}
		else if (TabPosition == HorizontalTabPositions.Bottom)
		{
			outerEdgeRect = new(0, 0, tabs.Surface.Width, tabs.Surface.Height - 2);
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
		// Top Line
		surface.DrawLine(new Point(x + 2, y), new Point(x + 1 + tab.Header.Length, y), Glyphs.ThinLineHorizontal, Color.DarkOrange);
		// Clear bottom line
		surface.DrawLine(new Point(x + 2, y + 2), new Point(x + 1 + tab.Header.Length, y + 2), 0, Color.Black);
		// Sides
		surface.SetGlyph(x + 1, y + 1, Glyphs.ThinLineVertical, Color.DarkOrange);
		surface.SetGlyph(x + 2 + tab.Header.Length, y + 1, Glyphs.ThinLineVertical, Color.DarkOrange);
		// Top Corners
		surface.SetGlyph(x + 1, y, Glyphs.ThinTopLeftCorner, Color.DarkOrange);
		surface.SetGlyph(x + 2 + tab.Header.Length, y, Glyphs.ThinTopRightCorner, Color.DarkOrange);
		// Bottom Corners
		surface.SetGlyph(x + 1, y + 2, Glyphs.ThinBottomRightCorner, Color.DarkOrange);
		surface.SetGlyph(x + 2 + tab.Header.Length, y + 2, Glyphs.ThinBottomLeftCorner, Color.DarkOrange);
		// Text
		surface.Print(x + 2, y + 1, tab.Header, Color.DarkOrange);
		// Button
		tab.TabButton.IsVisible = false;

		// Return end of tab position
		int nextX = x + 3 + tab.Header.Length;

		return nextX;
	}

	int DrawActiveTabBottom(ICellSurface surface, int x, int y, TabItem tab)
	{
		// Bottom Line
		surface.DrawLine(new Point(x + 2, y + 2), new Point(x + 1 + tab.Header.Length, y + 2), Glyphs.ThinLineHorizontal, Color.DarkOrange);
		// Clear top line
		surface.DrawLine(new Point(x + 1, y), new Point(x + 2 + tab.Header.Length, y), 0, Color.Black);
		// Sides
		surface.SetGlyph(x + 1, y + 1, Glyphs.ThinLineVertical, Color.DarkOrange);
		surface.SetGlyph(x + 2 + tab.Header.Length, y + 1, Glyphs.ThinLineVertical, Color.DarkOrange);
		// Top Corners
		surface.SetGlyph(x + 1, y, Glyphs.ThinTopRightCorner, Color.DarkOrange);
		surface.SetGlyph(x + 2 + tab.Header.Length, y, Glyphs.ThinTopLeftCorner, Color.DarkOrange);
		// Bottom Corners
		surface.SetGlyph(x + 1, y + 2, Glyphs.ThinBottomLeftCorner, Color.DarkOrange);
		surface.SetGlyph(x + 2 + tab.Header.Length, y + 2, Glyphs.ThinBottomRightCorner, Color.DarkOrange);
		// Text
		surface.Print(x + 2, y + 1, tab.Header, Color.DarkOrange);

		// Return end of tab position
		int nextX = x + 3 + tab.Header.Length;

		return nextX;
	}

	int DrawInactiveTabTop(ICellSurface surface, int x, int y, TabItem tab, int currentIndex, int activeIndex)
	{
		int nextX = 0;
		if (currentIndex > activeIndex) // Current tab is to the right of the active tab
		{
			// Top line
			surface.DrawLine(new Point(x, y), new Point(x + tab.Header.Length - 1, y), Glyphs.ThinLineHorizontal, Color.DarkGray);
			// Corner
			surface.SetGlyph(x + tab.Header.Length, y, Glyphs.ThinTopRightCorner, Color.DarkGray);
			// Side
			surface.SetGlyph(x + tab.Header.Length, y + 1, Glyphs.ThinLineVertical, Color.DarkGray);
			// Text
			surface.Print(x, y + 1, tab.Header, Color.DarkOrange);
			// Button
			tab.TabButton.Position = new(x, y + 1);

			nextX = x + tab.Header.Length + 1;
		}
		else // Current tab is to the left of the active tab
		{
			// Tab is slightlight moved to the left, account for that
			x++;
			// Top line
			surface.DrawLine(new Point(x + 1, y), new Point(x + tab.Header.Length, y), Glyphs.ThinLineHorizontal, Color.DarkGray);
			// Corner
			surface.SetGlyph(x, y, Glyphs.ThinTopLeftCorner, Color.DarkGray);
			// Side
			surface.SetGlyph(x, y + 1, Glyphs.ThinLineVertical, Color.DarkGray);
			// Text
			surface.Print(x + 1, y + 1, tab.Header, Color.DarkOrange);
			// Button
			tab.TabButton.Position = new(x + 1, y + 1);

			nextX = x + tab.Header.Length;
		}

		tab.TabButton.IsVisible = true;

		return nextX;
	}

	int DrawInactiveTabBottom(ICellSurface surface, int x, int y, TabItem tab, int currentIndex, int activeIndex)
	{
		int nextX = 0;
		if (currentIndex > activeIndex) // Current tab is to the right of the active tab
		{
			// Top line
			surface.DrawLine(new Point(x, y + 2), new Point(x + tab.Header.Length - 1, y + 2), Glyphs.ThinLineHorizontal, Color.DarkGray);
			// Corner
			surface.SetGlyph(x + tab.Header.Length, y + 2, Glyphs.ThinBottomRightCorner, Color.DarkGray);
			// Side
			surface.SetGlyph(x + tab.Header.Length, y + 1, Glyphs.ThinLineVertical, Color.DarkGray);
			// Text
			surface.Print(x, y + 1, tab.Header, Color.DarkOrange);
			// Button
			tab.TabButton.Position = new(x, y + 1);

			nextX = x + tab.Header.Length + 1;
		}
		else // Current tab is to the left of the active tab
		{
			// Tab is slightlight moved to the left, account for that
			x++;
			// Top line
			surface.DrawLine(new Point(x + 1, y + 2), new Point(x + tab.Header.Length, y + 2), Glyphs.ThinLineHorizontal, Color.DarkGray);
			// Corner
			surface.SetGlyph(x, y + 2, Glyphs.ThinBottomLeftCorner, Color.DarkGray);
			// Side
			surface.SetGlyph(x, y + 1, Glyphs.ThinLineVertical, Color.DarkGray);
			// Text
			surface.Print(x + 1, y + 1, tab.Header, Color.DarkOrange);
			// Button
			tab.TabButton.Position = new(x + 1, y + 1);

			nextX = x + tab.Header.Length;
		}

		tab.TabButton.IsVisible = true;

		return nextX;
	}

	public override ThemeBase Clone() => new HorizontalTabControlTheme()
	{
		ControlThemeState = ControlThemeState.Clone()
	};
}