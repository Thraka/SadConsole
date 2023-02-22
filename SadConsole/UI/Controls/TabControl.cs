using System;
using System.Linq;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public enum HorizontalTabPositions { Top, Bottom }
public enum VerticalTabPositions { Left, Right }

public class TabControl : CompositeControl
{
	internal List<TabItem> TabItems { get; private set; }
	internal int ActiveTabIndex { get; private set; } = 0;
	internal TabItem CurrentTab => TabItems[ActiveTabIndex];

	// Makes sure the topleft of the consol is in the correct position
	public bool EnforceConsolePosition { get; set; } = true;

	/// <summary>
	/// Automatically updates the view rectangle of the active console. Note that the console will have to handle scrollbars themselves
	/// </summary>
	/// <value>If set to true, the view of the rectangle will be clamped to the tabcontrol area</value>
	public bool UpdateConsoleViewRect { get; set; } = true;

	public TabControl(IEnumerable<TabItem> tabItems, int width, int height) : this(tabItems, 0, width, height) { }
	public TabControl(IEnumerable<TabItem> tabItems, int startIndex, int width, int height) : base(width, height)
	{
		Assert(tabItems.Count() > 0);
		TabItems = new(tabItems);
		TabItems.ForEach(t => AddTab(t));

		SetActiveTab(startIndex);
	}

	// Tab button has been clicked
	void _onTabButtonClicked(object sender, EventArgs e)
	{
		int index = TabItems.IndexOf(TabItems.FirstOrDefault(x => x.Header == ((Button)sender).Text));
		SetActiveTab(index);
	}

	// Adds a tab and associated events
	public void AddTab(TabItem tab)
	{
		tab.TabButton.Click += _onTabButtonClicked;
		AddControl(tab.TabButton);
	}

	// Removes tab and associated events
	public void RemoveTab(TabItem tab)
	{
		tab.TabButton.Click -= _onTabButtonClicked;
		TabItems.Remove(tab);
		RemoveControl(tab.TabButton);
	}

	// Switches to specified tabindex
	public void SetActiveTab(int index)
	{
		Assert(index < TabItems.Count);
		ActiveTabIndex = index;
		foreach (TabItem tab in TabItems) tab.Console.IsVisible = false;
		CurrentTab.Console.IsVisible = true;
		IsDirty = true;
	}

	/// <summary>
	/// Shifts to the next tab. Stopping at the last tab
	/// </summary>
	public void NextTab()
	{
		ActiveTabIndex++;
		if (ActiveTabIndex >= TabItems.Count) ActiveTabIndex = TabItems.Count - 1;
		SetActiveTab(ActiveTabIndex);
	}

	/// <summary>
	/// Shifts to the previous tab. Stopping at the first tab
	/// </summary>
	public void PreviousTab()
	{
		ActiveTabIndex--;
		if (ActiveTabIndex < 0) ActiveTabIndex = 0;
		SetActiveTab(ActiveTabIndex);
	}
}