using System;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using SadConsole.UI.Themes;
using SadConsole.Quick;

namespace SadConsole.UI.Controls;

/// <summary>
/// Represents a button that can be toggled on/off within a group of other buttons.
/// </summary>
[DataContract]
public class ComboBox : CheckBox
{
    private int _dropDownheight;
    private ScreenSurface _dropDownContainer;
    private ListBox _listBox;

    public ListBoxItemTheme Theme_ListBoxItemTheme;

    public ComboBox(int width, int dropdownHeight, Object[] items) : base(width, 1)
    {
        //DropDownHeight = dropdownHeight;

        _dropDownContainer = new ScreenSurface(width, dropdownHeight);
        _listBox = new ListBox(width, dropdownHeight);
        
        ControlHost listboxHost = new ControlHost();
        _dropDownContainer.SadComponents.Add(listboxHost);
        listboxHost.Add(_listBox);

        foreach (var item in items)
            _listBox.Items.Add(item);

        _listBox.SelectedItemChanged += _listBox_SelectedItemChanged;
        _listBox.MouseButtonClicked += _listBox_MouseButtonClicked;
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

    private void _listBox_MouseButtonClicked(object? sender, ControlMouseState e)
    {
        //if (_listBox.MouseArea.Contains(e.MousePosition))
        //    IsSelected = false;
    }

    private void _listBox_SelectedItemChanged(object? sender, ListBox.SelectedItemEventArgs e)
    {
        Text = e.Item?.ToString() ?? string.Empty;
        IsSelected = false;
    }

    public void Theme_SetDropDownHeight(int height)
    {
        //_dropDownContainer.Resize(Width, )
    }

    protected override void OnIsSelected()
    {
        base.OnIsSelected();

        if (Parent?.Host?.ParentConsole is not null)
        {
            if (IsSelected)
            {
                _dropDownContainer.Parent = Parent.Host.ParentConsole;
                _dropDownContainer.Position = AbsolutePosition + (0, 1);
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
