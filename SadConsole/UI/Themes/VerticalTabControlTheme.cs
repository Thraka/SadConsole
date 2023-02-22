using System;
using System.Linq;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

public class VerticalTabControlTheme : ThemeBase
{
	public VerticalTabPositions TabPosition { get; set; }

	public VerticalTabControlTheme()
	{
		TabPosition = VerticalTabPositions.Left;
	}

	public override void UpdateAndDraw(ControlBase control, TimeSpan time)
	{
		if (control.IsDirty == false) return;
		if ((control is TabControl tabs) == false) return;

		RefreshTheme(control.FindThemeColors(), control);

		// Reassign button themes
		if (tabs.CurrentTab.TabButton.Theme.GetType() != typeof(VerticalButtonTheme))
		{
			WriteLine("Reassigning tab button themes to vertical");
			tabs.TabItems.ForEach(tab =>
			{
				tab.TabButton.Theme = new VerticalButtonTheme() { ShowEnds = false };
				tab.TabButton.Resize(1, tab.TabButton.Text.Length);
			});
		}

		tabs.Surface.Clear();

		int x = 0;
		int y = 0;

		Rectangle outerEdgeRect;
		if (TabPosition == VerticalTabPositions.Left)
		{
			outerEdgeRect = new(2, 0, tabs.Surface.Width - 2, tabs.Surface.Height);
		}
		else if (TabPosition == VerticalTabPositions.Right)
		{
			outerEdgeRect = new(0, 0, tabs.Surface.Width - 2, tabs.Surface.Height);
			x = tabs.Surface.Width - 1;
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
				if (TabPosition == VerticalTabPositions.Left)
				{
					y = DrawActiveTabLeft(tabs.Surface, x, y, tab);
					if (tabs.EnforceConsolePosition) tab.Console.Position = tabs.AbsolutePosition.Translate(new Point(3, 1));
				}
				else if (TabPosition == VerticalTabPositions.Right)
				{
					y = DrawActiveTabRight(tabs.Surface, x, y, tab);
					if (tabs.EnforceConsolePosition) tab.Console.Position = tabs.AbsolutePosition.Translate(new Point(1, 1));
				}
				
				if (tabs.UpdateConsoleViewRect) ((Console)tab.Console).View = new Rectangle(0, 0, tabs.Width - 4, tabs.Height - 2);
			}
			else
			{
				if (TabPosition == VerticalTabPositions.Left)
				{
					y = DrawInactiveTabLeft(tabs.Surface, x, y, tab, indexCounter, tabs.ActiveTabIndex);
				}
				else if (TabPosition == VerticalTabPositions.Right)
				{
					y = DrawInactiveTabRight(tabs.Surface, x, y, tab, indexCounter, tabs.ActiveTabIndex);
				}
			}

			indexCounter++;
		}
	}

	int DrawActiveTabLeft(ICellSurface surface, int x, int y, TabItem tab)
	{
		// Line to the left
		surface.DrawLine(new Point(x, y + 2), new Point(x, y + tab.Header.Length + 1), Glyphs.ThinLineVertical, Color.DarkOrange);
		// Clear line to the right
		surface.DrawLine(new Point(x + 2, y + 1), new Point(x + 2, y + tab.Header.Length + 2), 0);
		// Top
		surface.SetGlyph(x, y + 1, Glyphs.ThinTopLeftCorner, Color.DarkOrange);
		surface.SetGlyph(x + 1, y + 1, Glyphs.ThinLineHorizontal, Color.DarkOrange);
		surface.SetGlyph(x + 2, y + 1, Glyphs.ThinBottomRightCorner, Color.DarkOrange);
		//	Bottom
		surface.SetGlyph(x, y + tab.Header.Length + 2, Glyphs.ThinBottomLeftCorner, Color.DarkOrange);
		surface.SetGlyph(x + 1, y + tab.Header.Length + 2, Glyphs.ThinLineHorizontal, Color.DarkOrange);
		surface.SetGlyph(x + 2, y + tab.Header.Length + 2, Glyphs.ThinTopRightCorner, Color.DarkOrange);

		// Header Text
		int headerY = y + 2;
		foreach (char letter in tab.Header)
		{
			surface.SetGlyph(x + 1, headerY, letter);
			headerY++;
		}

		tab.TabButton.IsVisible = false;

		int nextY = headerY + 1;

		return nextY;
	}

	int DrawActiveTabRight(ICellSurface surface, int x, int y, TabItem tab)
	{
		// Line to the right
		surface.DrawLine(new Point(x, y + 2), new Point(x, y + tab.Header.Length + 1), Glyphs.ThinLineVertical, Color.DarkOrange);
		// Clear line to the left
		surface.DrawLine(new Point(x - 2, y + 2), new Point(x - 2, y + tab.Header.Length + 1), 0);
		// Top
		surface.SetGlyph(x, y + 1, Glyphs.ThinTopRightCorner, Color.DarkOrange);
		surface.SetGlyph(x - 1, y + 1, Glyphs.ThinLineHorizontal, Color.DarkOrange);
		surface.SetGlyph(x - 2, y + 1, Glyphs.ThinBottomLeftCorner, Color.DarkOrange);
		// Bottom
		surface.SetGlyph(x, y + tab.Header.Length + 2, Glyphs.ThinBottomRightCorner, Color.DarkOrange);
		surface.SetGlyph(x - 2, y + tab.Header.Length + 2, Glyphs.ThinTopLeftCorner, Color.DarkOrange);
		surface.SetGlyph(x - 1, y + tab.Header.Length + 2, Glyphs.ThinLineHorizontal, Color.DarkOrange);

		// Header Text
		int headerY = y + 2;
		foreach (char letter in tab.Header)
		{
			surface.SetGlyph(x - 1, headerY, letter, Color.DarkOrange);
			headerY++;
		}

		tab.TabButton.IsVisible = false;

		int nextY = headerY + 1;

		return nextY;
	}

	int DrawInactiveTabLeft(ICellSurface surface, int x, int y, TabItem tab, int currentIndex, int activeIndex)
	{
		int nextY = 0;
		if (currentIndex > activeIndex)
		{
			surface.DrawLine(new Point(x, y), new Point(x, y + tab.Header.Length - 1), Glyphs.ThinLineVertical, Color.DarkGray);
			surface.SetGlyph(x, y + tab.Header.Length, Glyphs.ThinBottomLeftCorner, Color.DarkGray);
			surface.SetGlyph(x + 1, y + tab.Header.Length, Glyphs.ThinLineHorizontal, Color.DarkGray);

			nextY = y + tab.Header.Length + 1;
		}
		else // Current tab is to the top of the active tab)
		{
			y += 2;
			surface.DrawLine(new Point(x, y), new Point(x, y + tab.Header.Length - 1), Glyphs.ThinLineVertical, Color.DarkGray);
			surface.SetGlyph(x, y - 1, Glyphs.ThinTopLeftCorner, Color.DarkGray);
			surface.SetGlyph(x + 1, y - 1, Glyphs.ThinLineHorizontal, Color.DarkGray);

			nextY = y + tab.Header.Length - 1;
		}

		tab.TabButton.Position = new(x + 1, y);
		tab.TabButton.IsVisible = true;

		return nextY;
	}

	int DrawInactiveTabRight(ICellSurface surface, int x, int y, TabItem tab, int currentIndex, int activeIndex)
	{
		int nextY = 0;
		if (currentIndex > activeIndex)
		{
			surface.DrawLine(new Point(x, y), new Point(x, y + tab.Header.Length - 1), Glyphs.ThinLineVertical, Color.DarkGray);
			surface.SetGlyph(x, y + tab.Header.Length, Glyphs.ThinBottomRightCorner, Color.DarkGray);
			surface.SetGlyph(x - 1, y + tab.Header.Length, Glyphs.ThinLineHorizontal, Color.DarkGray);

			nextY = y + tab.Header.Length + 1;
		}
		else // Current tab is to the top of the active tab)
		{
			y += 2;
			surface.DrawLine(new Point(x, y), new Point(x, y + tab.Header.Length - 1), Glyphs.ThinLineVertical, Color.DarkGray);
			surface.SetGlyph(x, y - 1, Glyphs.ThinTopRightCorner, Color.DarkGray);
			surface.SetGlyph(x - 1, y - 1, Glyphs.ThinLineHorizontal, Color.DarkGray);


			nextY = y + tab.Header.Length - 1;
		}

		tab.TabButton.Position = new(x - 1, y);
		tab.TabButton.IsVisible = true;

		return nextY;
	}

	public override ThemeBase Clone() => new VerticalTabControlTheme()
	{
		ControlThemeState = ControlThemeState.Clone()
	};
}