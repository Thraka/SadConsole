using System.Runtime.Serialization;
using SadConsole.Quick;
using System.Linq;
using SadRogue.Primitives;
using static SadConsole.UI.Controls.ListBox;
using System;

namespace SadConsole.UI.Controls;

/// <summary>
/// Represents a button that can be toggled on/off within a group of other buttons.
/// </summary>
[DataContract]
public partial class ComboBox : CheckBox
{
    /// <summary>
    /// Surface that contains the listbox
    /// </summary>
    protected readonly ScreenSurface DropdownContainer;

    /// <summary>
    /// Listbox used to control the items
    /// </summary>
    protected readonly ListBox ListBox;

    /// <summary>
    /// An event that triggers when the <see cref="SelectedItem"/> property changes.
    /// </summary>
    public event EventHandler<SelectedItemEventArgs>? SelectedItemChanged;

    /// <summary>
    /// Gets or sets the index of the selected item.
    /// </summary>
    public int SelectedIndex
    {
        get => ListBox.SelectedIndex;
        set => ListBox.SelectedIndex = value;
    }

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    public object? SelectedItem
    {
        get => ListBox.SelectedItem;
        set => ListBox.SelectedItem = value;
    }

    /// <summary>
    /// Creates a new instance of the combobox control.
    /// </summary>
    /// <param name="width">The width of the control.</param>
    /// <param name="dropdownWidth">The width of the dropdown container.</param>
    /// <param name="dropdownHeight">The height of the dropdown container.</param>
    /// <param name="items">The items to seed the dropdown with.</param>
    public ComboBox(int width, int dropdownWidth, int dropdownHeight, object[] items) : base(width, 1)
    {
        DropdownContainer = new ScreenSurface(dropdownWidth, dropdownHeight);
        ListBox = new ListBox(dropdownWidth, dropdownHeight);

        ControlHost listboxHost = new ControlHost();
        DropdownContainer.SadComponents.Add(listboxHost);
        listboxHost.Add(ListBox);

        foreach (object item in items)
            ListBox.Items.Add(item);

        ListBox.SelectedItemChanged += _listBox_SelectedItemChanged;
        ListBox.SelectedItemReselected += _listBox_SelectedItemReselected; ;
        ListBox.SelectedIndex = 0;

        // Setup popup container console to watch the mouse
        DropdownContainer.WithMouse((screenObject, state) =>
        {
            if (!state.IsOnScreenObject && state.Mouse.LeftClicked)
            {
                IsSelected = false;
                return true;
            }

            return false;
        });

        // Handle keyboard input. If ESC ever hit, we don't want to show the popup
        DropdownContainer.WithKeyboard((screenObject, state) =>
        {
            if (state.IsKeyPressed(Input.Keys.Escape))
            {
                IsSelected = false;
                return true;
            }

            return false;
        });

        // Theme settings
        IconThemeState = new ThemeStates();

        CollapsedButtonGlyph = 16; // ▶
        ExpandedButtonGlyph = 31; // ▼

        ListBox.DrawBorder = true;

        PopupInnerAligned = true;
        PopupHorizontal = HorizontalAlignment.Left;
        PopupVertical = VerticalAlignment.Bottom;
    }

    /// <summary>
    /// Resizes the dropdown container to the given width/height
    /// </summary>
    /// <param name="width">Width of the dropdown</param>
    /// <param name="height">Height of the dropdown</param>
    public void ResizeDropDown(int? width = null, int? height = null)
    {
        if (width == null && height == null) return;
        int newWidth = width ?? DropdownContainer.Surface.Width;
        int newHeight = height ?? DropdownContainer.Surface.Height;
        DropdownContainer.Surface = new CellSurface(newWidth, newHeight);
        ListBox.Resize(newWidth, newHeight);
    }

    /// <summary>
    /// Sets the items in the dropdown listbox.
    /// </summary>
    /// <param name="items">The items to set.</param>
    public void SetItems(params object[] items)
    {
        ListBox.Items.Clear();

        foreach (object item in items)
            ListBox.Items.Add(item);

        IsDirty = true;
    }

    /// <summary>
    /// Gets an array of items from the dropdown listbox.
    /// </summary>
    /// <returns></returns>
    public object[] GetItems() =>
        ListBox.Items.ToArray();

    private void _listBox_SelectedItemChanged(object? sender, SelectedItemEventArgs e)
    {
        Text = e.Item?.ToString() ?? string.Empty;
        IsDirty = true;
        IsSelected = false;
        SelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(SelectedItem));
    }

    private void _listBox_SelectedItemReselected(object? sender, SelectedItemEventArgs e)
    {
        IsSelected = false;
    }

    /// <summary>
    /// Checks if the dropdown container is off-screen, and pushes it back in.
    /// </summary>
    protected void RepositionOffScreenContainer()
    {
        if (Parent?.Host?.ParentConsole is not null)
        {
            // Calculate based on the fontsize
            IScreenSurface console = Parent.Host.ParentConsole;
            int screenBoundsX = console.Surface.Width;
            int screenBoundsY = console.Surface.Height;
            int containerBoundsX = (DropdownContainer.Position.X + DropdownContainer.Width);
            int containerBoundsY = (DropdownContainer.Position.Y + DropdownContainer.Height);

            // We are going off the screen horizontally
            if (containerBoundsX >= screenBoundsX)
            {
                int diff = containerBoundsX - screenBoundsX;
                DropdownContainer.Position = new Point(DropdownContainer.Position.X - (int)Math.Round((decimal)diff / (console.FontSize.X / console.Font.GlyphWidth)), DropdownContainer.Position.Y);
            }

            // We are going off the screen vertically
            if (containerBoundsY >= screenBoundsY)
            {
                int diff = containerBoundsY - screenBoundsY;
                DropdownContainer.Position = new Point(DropdownContainer.Position.X, DropdownContainer.Position.Y - (int)Math.Round((decimal)diff / (console.FontSize.Y / console.Font.GlyphHeight)));
            }
        }
    }

    /// <summary>
    /// When <see cref="ToggleButtonBase.IsSelected"/> is <see langword="true"/>, displays the popup container. When <see langword="false"/>, hides the popup container.
    /// </summary>
    protected override void OnIsSelected()
    {
        base.OnIsSelected();

        if (Parent?.Host?.ParentConsole is not null)
        {
            if (IsSelected)
            {
                Colors colors = FindThemeColors();
                ListBox.Parent!.Host!.ThemeColors = colors;
                DropdownContainer.Font = Parent.Host.ParentConsole.Font;
                DropdownContainer.FontSize = Parent.Host.ParentConsole.FontSize;
                DropdownContainer.Parent = Parent.Host.ParentConsole;

                Point position = AbsolutePosition;

                switch (PopupHorizontal)
                {
                    case HorizontalAlignment.Left:
                        position -= PopupInnerAligned ? (0, 0) : (DropdownContainer.Width, 0);
                        break;
                    case HorizontalAlignment.Center:
                        position -= (DropdownContainer.Width / 2 - Width / 2, 0);
                        break;
                    case HorizontalAlignment.Right:
                        position += PopupInnerAligned ? (Width - DropdownContainer.Width, 0) : (Width, 0);
                        break;
                    default:
                        break;
                }

                switch (PopupVertical)
                {
                    case VerticalAlignment.Top:
                        position -= (0, DropdownContainer.Height);
                        break;
                    case VerticalAlignment.Center:
                        position -= (0, DropdownContainer.Height / 2);
                        break;
                    case VerticalAlignment.Bottom:
                        position += (0, 1);
                        break;
                    default:
                        break;
                }

                ListBox.ScrollToSelectedItem();
                DropdownContainer.Position = position;
                DropdownContainer.IsVisible = true;
                DropdownContainer.IsExclusiveMouse = true;

                // If the dropdown container goes offscreen, this will push it back in
                RepositionOffScreenContainer();

                GameHost.Instance.FocusedScreenObjects.Push(DropdownContainer);
            }
            else
            {
                DropdownContainer.IsExclusiveMouse = false;
                DropdownContainer.IsVisible = false;
                GameHost.Instance.FocusedScreenObjects.Pop(DropdownContainer);
                DropdownContainer.Parent = null;
            }
        }
    }
}
