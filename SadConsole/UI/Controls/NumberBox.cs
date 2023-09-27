using System;
using System.Globalization;
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
    public long NumberMaximum { get; set; }

    /// <summary>
    /// The minimum number that can be set for this text box.
    /// </summary>
    /// <remarks>
    /// Set both <see cref="NumberMaximum"/> and <see cref="NumberMinimum"/> to 0 to disable number bounds checking.
    /// </remarks>
    [DataMember]
    public long NumberMinimum { get; set; }

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
            if (AllowDecimal && _text.Contains(GetCultureDecimalSeperator()))
            {
                if (!double.TryParse(_text, NumberStyles.Any, GetCulture(), out double value))
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
    public NumberBox(int width) : base(width) { }
    #endregion

    private void FixNumber()
    {
        if (AllowDecimal && _text.Contains(GetCultureDecimalSeperator()))
        {
            if (!double.TryParse(_text, NumberStyles.Any, GetCulture(), out double value))
                value = UseMinMax ? (NumberMinimum >= 0 ? NumberMinimum : DefaultDecimalValue) : DefaultDecimalValue;
            else
                value = UseMinMax ? Math.Clamp(value, NumberMinimum, NumberMaximum) : DefaultDecimalValue;

            Text = value.ToString(GetCulture());
        }
        else
        {
            if (!long.TryParse(_text, out long value))
                value = UseMinMax ? (NumberMinimum >= 0 ? NumberMinimum : DefaultValue) : DefaultValue;
            else
                value = UseMinMax ? Math.Clamp(value, NumberMinimum, NumberMaximum) : DefaultValue;

            Text = value.ToString(GetCulture());
        }

        IsDirty = true;
    }

    /// <inheritdoc/>
    protected override void OnFocused()
    {
        if (TextAsDouble() == 0)
            _text = string.Empty;

        base.OnFocused();
    }

    /// <inheritdoc/>
    protected override void OnUnfocused()
    {
        FixNumber();
        base.OnUnfocused();
    }

    /// <summary>
    /// Returns the <see cref="TextBox.Text"/> property as parsed by <see cref="long.TryParse(string?, NumberStyles, IFormatProvider?, out long)"/> with the current culture.
    /// </summary>
    /// <returns>The parsed text value.</returns>
    public long TextAsLong()
    {
        long.TryParse(_text, NumberStyles.Any, GetCulture(), out long value);
        return value;
    }

    /// <summary>
    /// Returns the <see cref="TextBox.Text"/> property as parsed by <see cref="double.TryParse(string?, NumberStyles, IFormatProvider?, out double)"/> with the current culture.
    /// </summary>
    /// <returns>The parsed text value.</returns>
    public double TextAsDouble()
    {
        double.TryParse(_text, NumberStyles.Any, GetCulture(), out double value);
        return value;
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

                else if (
                            (
                                (   // Decimal was pressed and we allow decimals
                                    info.KeysPressed[i].Character == GetCultureDecimalSeperator()
                                    && _allowDecimalPoint
                                    && !_text.Contains(GetCultureDecimalSeperator())
                                )
                                // Otherwise, a number was pressed
                                || char.IsDigit(info.KeysPressed[i].Character)
                            )
                            // And we've not hit max length yet
                            && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength))
                        )
                {
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

    /// <summary>
    /// Gets the current culture's decimal separator.
    /// </summary>
    /// <returns>The current culture.</returns>
    protected char GetCultureDecimalSeperator() =>
        CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

    /// <summary>
    /// Returns the current culture.
    /// </summary>
    /// <returns>The current culture.</returns>
    protected CultureInfo GetCulture() =>
        CultureInfo.CurrentCulture;

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        Text = _text;
        DetermineState();
        IsDirty = true;
    }
}
