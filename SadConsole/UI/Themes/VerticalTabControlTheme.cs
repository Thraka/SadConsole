using System;
using System.Linq;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
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
			outerEdgeRect = new Rectangle(2, 0, tabs.Surface.Width - 2, tabs.Surface.Height);
		}
		else if (TabPosition == VerticalTabPositions.Right)
		{
			outerEdgeRect = new Rectangle(0, 0, tabs.Surface.Width - 2, tabs.Surface.Height);
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
		int headerLength = tab.Header.Length > tab.TabSize ? tab.Header.Length : tab.TabSize;

		// Line to the left
		surface.DrawLine(new Point(x, y + 2), new Point(x, y + headerLength + 1), 179, Color.DarkOrange);
		// Clear line to the right
		surface.DrawLine(new Point(x + 2, y + 1), new Point(x + 2, y + headerLength + 2), 0);
		// Top
		surface.SetGlyph(x, y + 1, 218, Color.DarkOrange);
		surface.SetGlyph(x + 1, y + 1, 196, Color.DarkOrange);
		surface.SetGlyph(x + 2, y + 1, 217, Color.DarkOrange);
		//	Bottom
		surface.SetGlyph(x, y + headerLength + 2, 192, Color.DarkOrange);
		surface.SetGlyph(x + 1, y + headerLength + 2, 196, Color.DarkOrange);
		surface.SetGlyph(x + 2, y + headerLength + 2, 191, Color.DarkOrange);

		// Header Text
		int headerY = y + 2;
		int padding = 0;
		if (tab.TextAlignment == HorizontalAlignment.Center)
		{
			padding = (headerLength - tab.Header.Length) / 2;
		}
		else if (tab.TextAlignment == HorizontalAlignment.Right)
		{
			padding = headerLength - tab.Header.Length;
		}

		foreach (char letter in tab.Header)
		{
			surface.SetGlyph(x + 1, headerY + padding, letter);
			headerY++;
		}

		tab.TabButton.IsVisible = false;

		int nextY = y + headerLength + 3;

		return nextY;
	}

	int DrawActiveTabRight(ICellSurface surface, int x, int y, TabItem tab)
	{
		int headerLength = tab.Header.Length > tab.TabSize ? tab.Header.Length : tab.TabSize;

		// Line to the right
		surface.DrawLine(new Point(x, y + 2), new Point(x, y + headerLength + 1), 179, Color.DarkOrange);
		// Clear line to the left
		surface.DrawLine(new Point(x - 2, y + 2), new Point(x - 2, y + headerLength + 1), 0);
		// Top
		surface.SetGlyph(x, y + 1, 191, Color.DarkOrange);
		surface.SetGlyph(x - 1, y + 1, 196, Color.DarkOrange);
		surface.SetGlyph(x - 2, y + 1, 192, Color.DarkOrange);
		// Bottom
		surface.SetGlyph(x, y + headerLength + 2, 217, Color.DarkOrange);
		surface.SetGlyph(x - 2, y + headerLength + 2, 218, Color.DarkOrange);
		surface.SetGlyph(x - 1, y + headerLength + 2, 196, Color.DarkOrange);

		// Header Text
		int headerY = y + 2;
		int padding = 0;
		if (tab.TextAlignment == HorizontalAlignment.Center)
		{
			padding = (headerLength - tab.Header.Length) / 2;
		}
		else if (tab.TextAlignment == HorizontalAlignment.Right)
		{
			padding = headerLength - tab.Header.Length;
		}

		foreach (char letter in tab.Header)
		{
			surface.SetGlyph(x - 1, headerY + padding, letter, Color.DarkOrange);
			headerY++;
		}

		tab.TabButton.IsVisible = false;

		int nextY = y + headerLength + 3;

		return nextY;
	}

	int DrawInactiveTabLeft(ICellSurface surface, int x, int y, TabItem tab, int currentIndex, int activeIndex)
	{
		int headerLength = tab.Header.Length > tab.TabSize ? tab.Header.Length : tab.TabSize;

		int nextY = 0;
		if (currentIndex > activeIndex)
		{
			surface.DrawLine(new Point(x, y), new Point(x, y + headerLength - 1), 179, Color.DarkGray);
			surface.SetGlyph(x, y + headerLength, 192, Color.DarkGray);
			surface.SetGlyph(x + 1, y + headerLength, 196, Color.DarkGray);

			nextY = y + headerLength + 1;
		}
		else // Current tab is to the top of the active tab)
		{
			y += 2;
			surface.DrawLine(new Point(x, y), new Point(x, y + headerLength - 1), 179, Color.DarkGray);
			surface.SetGlyph(x, y - 1, 218, Color.DarkGray);
			surface.SetGlyph(x + 1, y - 1, 196, Color.DarkGray);

			nextY = y + headerLength - 1;
		}

		tab.TabButton.Position = new Point(x + 1, y);
		tab.TabButton.IsVisible = true;
		tab.TabButton.Resize(1, headerLength);
		tab.TabButton.TextAlignment = tab.TextAlignment;

		return nextY;
	}

	int DrawInactiveTabRight(ICellSurface surface, int x, int y, TabItem tab, int currentIndex, int activeIndex)
	{
		int headerLength = tab.Header.Length > tab.TabSize ? tab.Header.Length : tab.TabSize;

		int nextY = 0;
		if (currentIndex > activeIndex)
		{
			surface.DrawLine(new Point(x, y), new Point(x, y + headerLength - 1), 179, Color.DarkGray);
			surface.SetGlyph(x, y + headerLength, 217, Color.DarkGray);
			surface.SetGlyph(x - 1, y + headerLength, 196, Color.DarkGray);

			nextY = y + headerLength + 1;
		}
		else // Current tab is to the top of the active tab)
		{
			y += 2;
			surface.DrawLine(new Point(x, y), new Point(x, y + headerLength - 1), 179, Color.DarkGray);
			surface.SetGlyph(x, y - 1, 191, Color.DarkGray);
			surface.SetGlyph(x - 1, y - 1, 196, Color.DarkGray);


			nextY = y + headerLength - 1;
		}

		tab.TabButton.Position = new Point(x - 1, y);
		tab.TabButton.IsVisible = true;
		tab.TabButton.Resize(1, headerLength);
		tab.TabButton.TextAlignment = tab.TextAlignment;

		return nextY;
	}

	public override ThemeBase Clone() => new VerticalTabControlTheme()
	{
		ControlThemeState = ControlThemeState.Clone()
	};
}
}
