using System;
using System.Runtime.Serialization;
using System.Text;
using SadConsole.Input;

namespace SadConsole.UI.Controls;

/// <summary>
/// InputBox control that allows text input.
/// </summary>
[DataContract]
public partial class NumberBox : TextBox
{
    /// <summary>
    /// Indicates that the input box (when numeric) will accept decimal points.
    /// </summary>
    [DataMember(Name = "AllowDecimalPoint")]
    protected bool _allowDecimalPoint;

    /// <summary>
    /// Gets or sets whether or not this input box should restrict numeric input should allow a decimal point.
    /// </summary>
    public bool AllowDecimal
    {
        get => _allowDecimalPoint;
        set { _allowDecimalPoint = value; ValidateCursorPosition(Text); }
    }

    /// <summary>
    /// The maximum number that can be set for this text box.
    /// </summary>
    /// <remarks>
    /// Set both <see cref="NumberMaximum"/> and <see cref="NumberMinimum"/> to 0 to disable number bounds checking.
    /// </remarks>
    [DataMember]
    public int NumberMaximum { get; set; }

    /// <summary>
    /// The minimum number that can be set for this text box.
    /// </summary>
    /// <remarks>
    /// Set both <see cref="NumberMaximum"/> and <see cref="NumberMinimum"/> to 0 to disable number bounds checking.
    /// </remarks>
    [DataMember]
    public int NumberMinimum { get; set; }

    /// <summary>
    /// The default value to use when the current value is invalid and <see cref="AllowDecimal"/> is <see langword="false"/>.
    /// </summary>
    [DataMember]
    public long DefaultValue { get; set; }

    /// <summary>
    /// The default value to use when the current value is invalid and <see cref="AllowDecimal"/> is <see langword="true"/>.
    /// </summary>
    [DataMember]
    public double DefaultDecimalValue { get; set; }

    private bool UseMinMax => NumberMaximum != 0 || NumberMinimum != 0;

    /// <summary>
    /// When <see langword="true"/>, indicates that the number is either out of bounds of the <see cref="NumberMinimum"/> and <see cref="NumberMaximum"/> range, or it can't be property parsed. Otherwise, <see langword="false"/>
    /// </summary>
    /// <remarks>
    /// Used by the theme system.
    /// </remarks>
    public bool IsEditingNumberInvalid
    {
        get
        {
            if (AllowDecimal && _text.Contains('.'))
            {
                if (!double.TryParse(_text, out double value))
                    return true;
                else
                    return UseMinMax && (value < NumberMinimum || value > NumberMaximum);
            }
            else
            {
                if (!long.TryParse(_text, out long value))
                    return true;
                else
                    return UseMinMax && (value < NumberMinimum || value > NumberMaximum);
            }
        }
    }

    #region Constructors
    /// <summary>
    /// Creates a new instance of the input box.
    /// </summary>
    /// <param name="width">The width of the input box.</param>
    public NumberBox(int width)
        : base(width)
    {
    }
    #endregion

    private void FixNumber()
    {
        if (AllowDecimal && _text.Contains('.'))
        {
            if (!double.TryParse(_text, out double value))
                value = UseMinMax ? (NumberMinimum >= 0 ? NumberMinimum : DefaultDecimalValue) : DefaultDecimalValue;
            else
                value = UseMinMax ? Math.Clamp(value, NumberMinimum, NumberMaximum) : DefaultDecimalValue;

            Text = value.ToString();
        }
        else
        {
            if (!long.TryParse(_text, out long value))
                value = UseMinMax ? (NumberMinimum >= 0 ? NumberMinimum : DefaultValue) : DefaultValue;
            else
                value = UseMinMax ? Math.Clamp(value, NumberMinimum, NumberMaximum) : DefaultValue;

            Text = value.ToString();
        }

        IsDirty = true;
    }

    /// <inheritdoc/>
    protected override void OnUnfocused()
    {
        FixNumber();
        base.OnUnfocused();
    }

    /// <summary>
    /// Called when the control should process keyboard information.
    /// </summary>
    /// <param name="info">The keyboard information.</param>
    /// <returns>True if the keyboard was handled by this control.</returns>
    public override bool ProcessKeyboard(Input.Keyboard info)
    {
        if (!DisableKeyboard)
        {
            StringBuilder newText = _cachedBuilder?.Clear().Append(_text) ?? (_cachedBuilder = new StringBuilder(_text));

            IsDirty = true;

            if (info.IsKeyReleased(Keys.Tab) || info.IsKeyDown(Keys.Tab))
                return false;

            for (int i = 0; i < info.KeysPressed.Count; i++)
            {
                if (CheckKeyPressCancel(info.KeysPressed[i]))
                    return true;

                if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0)
                {
                    newText.Remove(newText.Length - 1, 1);
                    _caretPos -= 1;

                    if (_caretPos == -1)
                        _caretPos = 0;

                    ValidateCursorPosition(newText.ToString());
                }

                else if (_caretPos == 0 && info.KeysPressed[i].Character == '-')
                {
                    newText.Append(info.KeysPressed[i].Character);
                    _caretPos += 1;
                }

                else if ((char.IsDigit(info.KeysPressed[i].Character)
                         || (_allowDecimalPoint && info.KeysPressed[i].Character == '.' && !_text.Contains('.')))
                         && MaxLength != 0 && newText.Length < MaxLength)
                {
                    //if ((_allowDecimalPoint && info.KeysPressed[i].Character != '.') )
                    //{

                    //}

                    newText.Append(info.KeysPressed[i].Character);
                    _caretPos += 1;
                    ValidateCursorPosition(newText.ToString());
                }
            }

            Text = newText.ToString();

            return true;
        }

        return false;
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        Text = _text;
        DetermineState();
        IsDirty = true;
    }
}
