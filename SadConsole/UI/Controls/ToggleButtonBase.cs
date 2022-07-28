using System;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls;

/// <summary>
/// Base class for toggle button controls, such as <see cref="CheckBox"/> and <see cref="RadioButton"/>.
/// </summary>
[DataContract]
public abstract class ToggleButtonBase : ButtonBase
{
    [DataMember(Name = "IsSelected")]
    private bool _isSelected;

    /// <summary>
    /// Raised when the selected state of the radio button is changed.
    /// </summary>
    public event EventHandler IsSelectedChanged;

    /// <summary>
    /// Gets or sets the selected state of the radio button.
    /// </summary>
    /// <remarks>Radio buttons within the same group will set their IsSelected property to the opposite of this radio button when you set this property.</remarks>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                DetermineState();
                IsDirty = true;
                OnIsSelected();
            }
        }
    }

    /// <summary>
    /// Raises the <see cref="IsSelectedChanged"/> event.
    /// </summary>
    protected virtual void OnIsSelected()
    {
        IsSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Creates an instance of the button control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control (default is 1).</param>
    public ToggleButtonBase(int width, int height)
        : base(width, height)
    {
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        DetermineState();
        IsDirty = true;
    }
}
