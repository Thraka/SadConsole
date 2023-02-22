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

	public new TabItem this[int index] => TabItems[index]; // Hiding the controls indexer, but that's ok
	public TabItem this[string header] => TabItems[GetTabIndex(header)];

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
		TabItems.ForEach(t =>
		{
			t.TabButton.Click += _onTabButtonClicked;
			AddControl(t.TabButton);
		});

		SetActiveTab(startIndex);
	}

	// Tab button has been clicked
	void _onTabButtonClicked(object sender, EventArgs e)
	{
		int index = TabItems.IndexOf(TabItems.FirstOrDefault(x => x.Header == ((Button)sender).Text));
		SetActiveTab(index);
	}

	/// <summary>
	/// Adds a tab
	/// </summary>
	/// <param name="tab">TabItem with header and associated console for tab content</param>
	public void AddTab(TabItem tab)
	{
		TabItems.Add(tab);
		tab.TabButton.Click += _onTabButtonClicked;
		AddControl(tab.TabButton);
	}

	/// <summary>
	/// Adds a tab
	/// </summary>
	/// <param name="header">Name of the tab</param>
	/// <param name="console">Associated console for tab content</param>
	public void AddTab(string header, ScreenSurface console)
	{
		AddTab(new TabItem(header, console));
	}

	/// <summary>
	/// Removes a tab
	/// </summary>
	/// <param name="tab">TabItem that should be removed</param>
	public void RemoveTab(TabItem tab)
	{
		tab.TabButton.Click -= _onTabButtonClicked;
		TabItems.Remove(tab);
		RemoveControl(tab.TabButton);
	}

	/// <summary>
	/// Remove tab based on header name
	/// </summary>
	/// <param name="header">Header of the tab</param>
	public void RemoveTab(string header)
	{
		TabItems
			.Where(t => t.Header == header)
			.ToList()
			.ForEach(t => RemoveTab(t));
	}

	/// <summary>
	/// Sets tab to specified index
	/// </summary>
	/// <param name="index">Index of the tab</param>
	public void SetActiveTab(int index)
	{
		Assert(index < TabItems.Count);
		ActiveTabIndex = index;
		foreach (TabItem tab in TabItems) tab.Console.IsVisible = false;
		CurrentTab.Console.IsVisible = true;
		IsDirty = true;
	}

	/// <summary>
	/// Gets the index of a tab
	/// </summary>
	/// <param name="tab">Tab you need the index of</param>
	/// <returns>0 based index of the tab</returns>
	public int GetTabIndex(TabItem tab)
	{
		return TabItems.IndexOf(tab);
	}

	/// <summary>
	/// Gets the index of a tab based on the tab header
	/// </summary>
	/// <param name="header">Tab header</param>
	/// <returns></returns>
	public int GetTabIndex(string header)
	{
		return GetTabIndex(TabItems.FirstOrDefault(t => t.Header == header));
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