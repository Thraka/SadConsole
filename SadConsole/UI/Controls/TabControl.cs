using System;
using System.Collections.Generic;
using System.Linq;

namespace SadConsole.UI.Controls
{
    /// <summary>
	/// Horizontal alignment modes
	/// </summary>
    public enum HorizontalTabPositions { Top, Bottom }

    /// <summary>
	/// Vertical alignment modes
	/// </summary>
    public enum VerticalTabPositions { Left, Right }

    /// <summary>
    /// Tab control.
    /// Provides showing the relevant content area on tab switch. Works on a "bring your own" principle. Content itself has to be provided by parent
    /// through a ScreenSurface (or subclass of it). Will provide options to set the view so it won't overflow, and is set to the correct position.
    /// Other components for this (eg: sliders) will have to be setup on the ScreenSurface itself
    /// </summary>
    public class TabControl : CompositeControl
    {
        internal List<TabItem> TabItems { get; private set; }
        internal int ActiveTabIndex { get; private set; } = 0;
        internal TabItem CurrentTab => TabItems[ActiveTabIndex];

        /// <summary>
        /// Makes sure the console is positioned inside the content area
        /// </summary>
        /// <value></value>
        public bool EnforceConsolePosition { get; set; } = true;

        /// <summary>
        /// Access tab by index
        /// </summary>
        /// <value></value>
        // Hiding the controls indexer, but that's ok
        public new TabItem this[int index]
        {
            get { return TabItems[index]; }
            set { TabItems[index] = value; }
        }

        /// <summary>
        /// Access tab by header
        /// </summary>
        /// <value></value>
        public TabItem this[string header]
        {
            get { return TabItems[GetTabIndex(header)]; }
            set { TabItems[GetTabIndex(header)] = value; }
        }


        /// <summary>
        /// Automatically updates the view rectangle of the active console. Note that the console will have to handle scrollbars themselves
        /// </summary>
        /// <value>If set to true, the view of the rectangle will be clamped to the tabcontrol area</value>
        public bool UpdateConsoleViewRect { get; set; } = true;

        /// <summary>
        /// Create a new TabControl
        /// </summary>
        /// <param name="tabItems">Tabs that are present on the tabcontrol</param>
        /// <param name="width">Width of the content area</param>
        /// <param name="height">Heigh of the content area</param>
        public TabControl(IEnumerable<TabItem> tabItems, int width, int height) : this(tabItems, 0, width, height) { }

        /// <summary>
        /// Create a new TabControl
        /// </summary>
        /// <param name="tabItems">Tabs that are present on the tabcontrol</param>
        /// <param name="startIndex">What tab to be active on initialization</param>
        /// <param name="width">Width of the content area</param>
        /// <param name="height">Heigh of the content area</param>
        /// <returns></returns>
        public TabControl(IEnumerable<TabItem> tabItems, int startIndex, int width, int height) : base(width, height)
        {
            TabItems = new List<TabItem>(tabItems);
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
        /// Renames a tab
        /// </summary>
        /// <param name="tab">Tab to rename</param>
        /// <param name="newName">New name</param>
        public void RenameTab(string tab, string newName)
        {
            this[tab].TabButton.Text = newName;
            this[tab].Header = newName; // Rename header last since it's also the key of the tab
        }

        /// <summary>
        /// Sets tab to specified index
        /// </summary>
        /// <param name="index">Index of the tab</param>
        public void SetActiveTab(int index)
        {
            if (index > TabItems.Count)
            {
                throw new ArgumentException("Cannot set index to out of bounds value");
            }
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
}
