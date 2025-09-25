using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// A control with tabs along the edge of the control. Each tab sets the content of the control.
/// </summary>
public partial class TabControl : CompositeControl
{
    private Orientation _tabOrientation;
    private int[] _connectedLineStyle;

    /// <summary>
    /// The index used when there isn't an active tab.
    /// </summary>
    public const int InvalidActiveTabIndex = -1;

    /// <summary>
    /// Raised when the active tab changes.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<TabItem?>>? ActiveTabItemChanged;

    /// <summary>
    /// The mouse position recorded from the last time the mouse was over the control.
    /// </summary>
    protected Point CachedMousePosition;

    /// <summary>
    /// The list of tab items displayed by the control.
    /// </summary>
    [DataMember]
    protected List<TabItem> TabItems { get; set; }

    /// <summary>
    /// Gets a list of tabs in this control.
    /// </summary>
    public ListEnumerator<TabItem> Tabs => new(TabItems);

    /// <summary>
    /// The index of the active tab. If <see cref="InvalidActiveTabIndex"/> is returned, there is no active tab.
    /// </summary>
    [DataMember]
    public int ActiveTabIndex { get; protected set; } = 0;

    /// <summary>
    /// Retursn the current tab.
    /// </summary>
    public TabItem CurrentTab => TabItems[ActiveTabIndex];

    /// <summary>
    /// Sets the position of the tab strip to the top or bottom of the control.
    /// </summary>
    [DataMember]
    public Orientation TabOrientation
    {
        get => _tabOrientation;
        set { _tabOrientation = value; ThemeDetermineContentRegion(); IsDirty = true; }
    }

    /// <summary>
    /// An array of glyphs indexed by <see cref="ICellSurface.ConnectedLineIndex"/>. Defaults to <see cref="ICellSurface.ConnectedLineThin"/>.
    /// </summary>
    [DataMember]
    public int[] ConnectedLineStyle
    {
        get => _connectedLineStyle;
        set { _connectedLineStyle = value; IsDirty = true; }
    }

    /// <summary>
    /// Gets or sets the tab item at the specified index.
    /// </summary>
    /// <param name="index">The index of the tab item to get or set.</param>
    /// <returns>The tab item content.</returns>
    public new TabItem this[int index]
    {
        get => TabItems[index];
        set
        {
            TabItems[index] = value;
            IsDirty = true;
        }
    }

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
    public TabControl(IEnumerable<TabItem> tabItems, int activeTabIndex, int width, int height) : base(width, height)
    {
        _connectedLineStyle = ICellSurface.ConnectedLineThin;
        TabItems = new List<TabItem>(tabItems);
        SetActiveTab(activeTabIndex);
    }

    /// <summary>
    /// Adds a tab to the control.
    /// </summary>
    /// <param name="tab">TabItem with header and associated console for tab content</param>
    public void AddTab(TabItem tab)
    {
        TabItems.Add(tab);
    }

    /// <summary>
    /// Creates and adds a tab to the control.
    /// </summary>
    /// <param name="header">The header of the new tab.</param>
    /// <param name="content">Associated content for the new tab.</param>
    /// <returns>The new tab item.</returns>
    public TabItem AddTab(string header, Panel content)
    {
        TabItem item = new(header, content);
        AddTab(item);
        return item;
    }

    /// <summary>
    /// Removes a tab from the control.
    /// </summary>
    /// <param name="tab">TabItem that should be removed</param>
    public void RemoveTab(TabItem tab) =>
        RemoveTab(TabItems.IndexOf(tab));

    /// <summary>
    /// Removes a tab by index.
    /// </summary>
    /// <param name="index">The index of the tab to remove.</param>
    public void RemoveTab(int index)
    {
        if (index == InvalidActiveTabIndex)
            throw new Exception("The control doesn't contain the specified tab.");

        TabItems[index].Content.IsDirtyChanged -= Content_IsDirtyChanged;
        TabItems.RemoveAt(index);

        if (index == ActiveTabIndex)
            SetActiveTab(0);
    }

    /// <summary>
    /// Sets the tab specified by the <paramref name="index"/> as active.
    /// </summary>
    /// <param name="index">Index of the tab.</param>
    public void SetActiveTab(int index)
    {
        if (index > TabItems.Count)
            throw new ArgumentException("Cannot set index to out of bounds value");

        IsDirty = true;

        if (index == InvalidActiveTabIndex)
            return;

        int previousIndex = ActiveTabIndex;
        ActiveTabIndex = index;

        TabItems[previousIndex].Content.IsDirtyChanged -= Content_IsDirtyChanged;
        TabItems[ActiveTabIndex].Content.IsDirtyChanged += Content_IsDirtyChanged;

        RemoveControl(TabItems[previousIndex].Content);
        AddControl(TabItems[ActiveTabIndex].Content);
        ThemeDetermineContentRegion();
        
        OnActiveTabItem(previousIndex, index);
    }

    private void Content_IsDirtyChanged(object? sender, EventArgs e) =>
        IsDirty |= ((ControlBase)sender!).IsDirty;

    /// <summary>
    /// Raises the <see cref="ActiveTabItemChanged"/> event.
    /// </summary>
    /// <param name="previousActiveIndex">The tab index of the previous item.</param>
    /// <param name="activeIndex">The index of the active tab.</param>
    protected void OnActiveTabItem(int previousActiveIndex, int activeIndex)
    {
        TabItem? previousTab = previousActiveIndex != InvalidActiveTabIndex ? TabItems[previousActiveIndex] : null;
        TabItem newTab = TabItems[activeIndex];

        ActiveTabItemChanged?.Invoke(this, new(previousTab, newTab));

        IsDirty = true;
    }

    /// <summary>
    /// Gets the index of a tab
    /// </summary>
    /// <param name="tab">Tab you need the index of</param>
    /// <returns>0 based index of the tab</returns>
    public int GetTabIndex(TabItem tab) =>
        TabItems.IndexOf(tab);

    /// <summary>
    /// Determines whether a tab is contained in this control.
    /// </summary>
    /// <param name="tab">The tab to check.</param>
    /// <returns>Returns <see langword="true"/> when the tab is in the control; otherwise, <see langword="false"/>.</returns>
    public bool ContainsTab(TabItem tab) =>
        TabItems.Contains(tab);

    /// <summary>
    /// Sets the next tab as active. Stops at the last tab.
    /// </summary>
    /// <returns>Returns <see langword="true"/> when the active tab changes; otherwise, <see langword="false"/>.</returns>
    public bool SelectNextTab()
    {
        int index = ActiveTabIndex + 1;
        if (index >= TabItems.Count)
            return false;

        SetActiveTab(index);
        return true;
    }

    /// <summary>
    /// Sets the previous tab as active. Stops at the first tab.
    /// </summary>
    /// <returns>Returns <see langword="true"/> when the active tab changes; otherwise, <see langword="false"/>.</returns>
    public bool SelectPreviousTab()
    {
        int index = ActiveTabIndex - 1;
        if (index < 0)
            return false;

        SetActiveTab(index);
        return true;
    }

    /// <inheritdoc/>
    protected override void OnLeftMouseClicked(ControlMouseState state)
    {
        base.OnLeftMouseClicked(state);

        for (int i = 0; i < TabItems.Count; i++)
        {
            TabItem tabItem = TabItems[i];
            if (ActiveTabIndex != i && tabItem.ThemeHeaderMouseArea.Contains(state.MousePosition))
            {
                SetActiveTab(i);
                break;
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnMouseIn(ControlMouseState state)
    {
        base.OnMouseIn(state);

        CachedMousePosition = state.MousePosition;

        IsDirty = true;
    }

    /// <summary>
    /// Resizes the tab item.
    /// </summary>
    protected override void OnResized() =>
        ThemeDetermineContentRegion();

    /// <summary>
    /// The region of the control where a tab item's content should be displayed.
    /// </summary>
    protected Rectangle ContentRegion;

    /// <summary>
    /// The region of the control where a tab headers should be displayed.
    /// </summary>
    protected Rectangle TabsRegion;

    /// <summary>
    /// Sets the <see cref="ContentRegion"/> rectangle to how much space the tab control should give to tab item content.
    /// </summary>
    protected void ThemeDetermineContentRegion()
    {
        if (TabOrientation == Orientation.Top)
        {
            TabsRegion = new Rectangle(1, 0, Width - 2, 3);
            ContentRegion = new(1, 3, Width - 2, Height - 4);
        }
        else if (TabOrientation == Orientation.Bottom)
        {
            TabsRegion = new Rectangle(1, Height - 3, Width - 2, 3);
            ContentRegion = new(1, 1, Width - 2, Height - 4);
        }
        else if (TabOrientation == Orientation.Left)
        {
            TabsRegion = new Rectangle(0, 1, 3, Height - 2);
            ContentRegion = new(3, 1, Width - 4, Height - 2);
        }
        else //Orientation.Right
        {
            TabsRegion = new Rectangle(Width - 3, 1, 3, Height - 2);
            ContentRegion = new(1, 1, Width - 4, Height - 2);
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
        Bottom,

        /// <summary>
        /// Tabs should be placed to the left of the control.
        /// </summary>
        Left,

        /// <summary>
        /// Tabs should be placed to the right of the control.
        /// </summary>
        Right
    }
}
