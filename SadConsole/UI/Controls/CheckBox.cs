using System;
using SadConsole.Input;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls;

/// <summary>
/// Represents a button that can be toggled on/off within a group of other buttons.
/// </summary>
[DataContract]
public class CheckBox : ToggleButtonBase
{
    /// <summary>
    /// Creates a new checkbox control with the specified width and height.
    /// </summary>
    /// <param name="width">The width of the control.</param>
    /// <param name="height">The height of the control.</param>
    public CheckBox(int width, int height) : base(width, height)
    {
        _textAlignment = HorizontalAlignment.Left;
    }

    /// <summary>
    /// Perfroms a click on the base button and also toggles the <see cref="ToggleButtonBase.IsSelected"/> property.
    /// </summary>
    protected override void OnClick()
    {
        base.OnClick();
        IsSelected = !IsSelected;
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        DetermineState();
        IsDirty = true;
    }
}
