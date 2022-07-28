using System;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls;

/// <summary>
/// Represents a button that can be toggled on/off within a group of other buttons.
/// </summary>
[DataContract]
public class RadioButton : ToggleButtonBase
{
    [DataMember(Name = "GroupName")]
    private string _groupName = "";

    /// <summary>
    /// The group of the radio button. All radio buttons with the same group name will work together to keep one radio button selected at a time.
    /// </summary>
    public string GroupName
    {
        get => _groupName;
        set
        {
            _groupName = value.Trim();

            if (_groupName == null)
                _groupName = string.Empty;
        }
    }

    /// <summary>
    /// Creates a new radio button control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control.</param>
    public RadioButton(int width, int height) : base(width, height)
    {
        _textAlignment = HorizontalAlignment.Left;
    }

    /// <summary>
    /// Perfroms a click on the base button and also toggles the <see cref="ToggleButtonBase.IsSelected"/> property.
    /// </summary>
    protected override void OnClick()
    {
        base.OnClick();

        if (!IsSelected)
            IsSelected = true;
    }

    /// <summary>
    /// Raises the <see cref="ToggleButtonBase.IsSelectedChanged"/> event and when <see cref="ToggleButtonBase.IsSelected"/> is <see langword="true"/>, deselects any other <see cref="RadioButton"/> with the same <see cref="GroupName"/>.
    /// </summary>
    protected override void OnIsSelected()
    {
        if (IsSelected && Parent != null)
        {
            for (int i = 0; i < Parent.Count; i++)
            {
                ControlBase child = Parent[i];

                if (child is RadioButton button && child != this)
                {
                    if (button.GroupName.Equals(GroupName, StringComparison.OrdinalIgnoreCase))
                        button.IsSelected = false;
                }
            }
        }

        base.OnIsSelected();
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        DetermineState();
        IsDirty = true;
    }
}
