using System.Runtime.Serialization;
using SadConsole.UI.Themes;
using SadConsole.Quick;
using System.Linq;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// Represents a button that can be toggled on/off within a group of other buttons.
/// </summary>
[DataContract]
public class ComboBox : CheckBox
{
    private ScreenSurface _dropDownContainer;
    private ListBox _listBox;

    /// <summary>
    /// Creates a new instance of the combobox control.
    /// </summary>
    /// <param name="width">The width of the control.</param>
    /// <param name="dropdownWidth">The width of the dropdown container.</param>
    /// <param name="dropdownHeight">The height of the dropdown container.</param>
    /// <param name="items">The items to seed the dropdown with.</param>
    public ComboBox(int width, int dropdownWidth, int dropdownHeight, object[] items) : base(width, 1)
    {
        _dropDownContainer = new ScreenSurface(dropdownWidth, dropdownHeight);
        _listBox = new ListBox(dropdownWidth, dropdownHeight);
        
        ControlHost listboxHost = new ControlHost();
        _dropDownContainer.SadComponents.Add(listboxHost);
        listboxHost.Add(_listBox);

        foreach (object item in items)
            _listBox.Items.Add(item);

        _listBox.SelectedItemChanged += _listBox_SelectedItemChanged;
        _listBox.SelectedItemReselected += _listBox_SelectedItemChanged;
        _listBox.SelectedIndex = 0;

        // Setup popup container console to watch the mouse
        _dropDownContainer.WithMouse((screenObject, state) =>
        {
            if (!state.IsOnScreenObject && state.Mouse.LeftClicked)
            {
                IsSelected = false;
                return true;
            }

            return false;
        });

        // Handle keyboard input. If ESC ever hit, we don't want to show the popup
        _dropDownContainer.WithKeyboard((screenObject, state) =>
        {
            if (state.IsKeyPressed(Input.Keys.Escape))
            {
                IsSelected = false;
                return true;
            }

            return false;
        });
    }

    /// <summary>
    /// Sets the items in the dropdown listbox.
    /// </summary>
    /// <param name="items">The items to set.</param>
    public void SetItems(params object[] items)
    {
        _listBox.Items.Clear();

        foreach (object item in items)
            _listBox.Items.Add(item);

        IsDirty = true;
    }

    /// <summary>
    /// Gets an array of items from the dropdown listbox.
    /// </summary>
    /// <returns></returns>
    public object[] GetItems() =>
        _listBox.Items.ToArray();

    private void _listBox_SelectedItemChanged(object? sender, ListBox.SelectedItemEventArgs e)
    {
        Text = e.Item?.ToString() ?? string.Empty;
        IsSelected = false;
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
                ComboBoxTheme theme = (ComboBoxTheme)Theme;
                _listBox.Parent!.Host!.ThemeColors = colors;
                //_listBox.SetThemeColors(colors);
                //_listBox.ScrollBar.SetThemeColors(colors);
                _listBox.Theme = ((ComboBoxTheme)Theme).ListBoxTheme;
                _dropDownContainer.Parent = Parent.Host.ParentConsole;

                Point position = AbsolutePosition;

                switch (theme.PopupHorizontal)
                {
                    case HorizontalAlignment.Left:
                        position -= theme.PopupInnerAligned ? (0, 0) : (_dropDownContainer.Width, 0);
                        break;
                    case HorizontalAlignment.Center:
                        position -= (_dropDownContainer.Width / 2 - Width / 2, 0);
                        break;
                    case HorizontalAlignment.Right:
                        position += theme.PopupInnerAligned ? (Width - _dropDownContainer.Width, 0) : (Width, 0);
                        break;
                    default:
                        break;
                }

                switch (theme.PopupVertical)
                {
                    case VerticalAlignment.Top:
                        position -= (0, _dropDownContainer.Height);
                        break;
                    case VerticalAlignment.Center:
                        position -= (0, _dropDownContainer.Height / 2);
                        break;
                    case VerticalAlignment.Bottom:
                        position += (0, 1);
                        break;
                    default:
                        break;
                }

                _listBox.ScrollToSelectedItem();
                _dropDownContainer.Position = position;
                _dropDownContainer.IsVisible = true;
                _dropDownContainer.IsExclusiveMouse = true;
                GameHost.Instance.FocusedScreenObjects.Push(_dropDownContainer);
            }
            else
            {
                _dropDownContainer.IsExclusiveMouse = false;
                _dropDownContainer.IsVisible = false;
                GameHost.Instance.FocusedScreenObjects.Pop(_dropDownContainer);
                _dropDownContainer.Parent = null;
            }
        }
    }
}
