using System;
using System.Runtime.Serialization;
using System.Text;
using SadConsole.Input;

namespace SadConsole.UI.Controls;

/// <summary>
/// InputBox control that allows text input.
/// </summary>
[DataContract]
public partial class TextBox : ControlBase
{
    /// <summary>
    /// String builder used while processing text in the <see cref="ProcessKeyboard(Keyboard)"/> method.
    /// </summary>
    protected StringBuilder? _cachedBuilder;

    /// <summary>
    /// Mask input with a certain character.
    /// </summary>
    public char? Mask { get; set; }

    /// <summary>
    /// When editing the text box, this allows the text to scroll to the right so you can see what you are typing.
    /// </summary>
    public int LeftDrawOffset { get; protected set; }

    /// <summary>
    /// The location of the caret.
    /// </summary>
    protected int _caretPos;

    /// <summary>
    /// The text value of the input box.
    /// </summary>
    [DataMember(Name = "Text")]
    protected string _text = "";

    /// <summary>
    /// Raised when the text has changed and the preview has accepted it.
    /// </summary>
    public event EventHandler? TextChanged;

    /// <summary>
    /// Raised before the text has changed and allows the change to be cancelled.
    /// </summary>
    public event EventHandler<ValueChangedCancelableEventArgs<string>>? TextChangedPreview;

    /// <summary>
    /// Raised when a key is pressed on the textbox.
    /// </summary>
    public event EventHandler<KeyPressEventArgs>? KeyPressed;

    /// <summary>
    /// Raised when the <see cref="Validator"/> validates the <see cref="Text"/> property.
    /// </summary>
    public event EventHandler<StringValidation.Result>? TextValidated;

    /// <summary>
    /// Disables mouse input.
    /// </summary>
    [DataMember(Name = "DisableMouseInput")]
    public bool DisableMouse { get; set; }

    /// <summary>
    /// Disables the keyboard which turns off keyboard input and hides the cursor.
    /// </summary>
    [DataMember(Name = "DisableKeyboardInput")]
    public bool DisableKeyboard { get; set; }

    /// <summary>
    /// How big the text can be. Setting this to 0 will make it unlimited.
    /// </summary>
    [DataMember]
    public int MaxLength { get; set; }

    /// <summary>
    /// When set, validates the <see cref="Text"/> property after <see cref="TextChangedPreview"/> has allowed the result.
    /// </summary>
    public StringValidation.Validator? Validator { get; set; }

    /// <summary>
    /// Gets or sets the position of the caret in the current text.
    /// </summary>
    public int CaretPosition
    {
        get => _caretPos;
        set
        {
            _caretPos = value;
            DetermineState();
            IsDirty = true;
            ValidateCursorPosition(_text);
        }
    }

    /// <summary>
    /// Gets or sets the text of the input box.
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            if (value != _text)
            {
                var args = new ValueChangedCancelableEventArgs<string>(_text, MaxLength != 0 && value.Length > MaxLength ? value.Substring(0, MaxLength) : value);

                TextChangedPreview?.Invoke(this, args);

                if (!args.IsCancelled)
                {
                    _text = args.NewValue ?? "";
                    _text = MaxLength != 0 && _text.Length > MaxLength ? _text.Substring(0, MaxLength) : _text;

                    if (Validator != null)
                        TextValidated?.Invoke(this, Validator.Invoke(_text));

                    TextChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    #region Constructors
    /// <summary>
    /// Creates a new instance of the input box.
    /// </summary>
    /// <param name="width">The width of the input box.</param>
    public TextBox(int width): base(width, 1)
    {
        CaretEffect = new Effects.BlinkGlyph()
        {
            GlyphIndex = 95,
            BlinkSpeed = System.TimeSpan.FromSeconds(0.4d)
        };
    }
    #endregion

    /// <inheritdoc/>
    public override void Resize(int width, int height)
    {
        base.Resize(width, 1);
    }

    /// <summary>
    /// Correctly positions the cursor within the text.
    /// </summary>
    protected void ValidateCursorPosition(string text)
    {
        if (_caretPos > text.Length)
            _caretPos = text.Length;

        // Test to see if caret is off edge of box
        if (_caretPos >= Width)
            LeftDrawOffset = _caretPos - Width + 1;
        else
            LeftDrawOffset = 0;

        if (LeftDrawOffset < 0)
            LeftDrawOffset = 0;

        if (_caretPos < LeftDrawOffset)
            LeftDrawOffset = _caretPos;

        DetermineState();
        IsDirty = true;
    }


    /// <summary>
    /// Raises the <see cref="KeyPressed"/> event and returns <see langword="true"/> if the keypress was cancelled.
    /// </summary>
    /// <param name="key">The key to use with the event.</param>
    /// <returns><see langword="true"/> to indicate that the keypress should be considered cancelled.</returns>
    protected bool CheckKeyPressCancel(Input.AsciiKey key)
    {
        if (KeyPressed == null)
            return false;

        var args = new KeyPressEventArgs(key);
        KeyPressed(this, args);

        return args.IsCancelled;
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

                if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0 && _caretPos != 0)
                {
                    if (_caretPos == newText.Length)
                        newText.Remove(newText.Length - 1, 1);
                    else
                        newText.Remove(_caretPos - 1, 1);

                    _caretPos = Math.Clamp(_caretPos - 1, 0, newText.Length);
                    ValidateCursorPosition(newText.ToString());
                }
                else if (info.KeysPressed[i].Key == Keys.Space && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
                {
                    newText.Insert(_caretPos, ' ');
                    _caretPos = Math.Clamp(_caretPos + 1, 0, newText.Length);
                    ValidateCursorPosition(newText.ToString());
                }

                else if (info.KeysPressed[i].Key == Keys.Delete && _caretPos != newText.Length)
                {
                    newText.Remove(_caretPos, 1);

                    _caretPos = Math.Clamp(_caretPos, 0, newText.Length);
                    ValidateCursorPosition(newText.ToString());
                }

                else if (info.KeysPressed[i].Key == Keys.Left)
                {
                    _caretPos = Math.Clamp(_caretPos - 1, 0, newText.Length);
                    ValidateCursorPosition(newText.ToString());
                }
                else if (info.KeysPressed[i].Key == Keys.Right)
                {
                    _caretPos = Math.Clamp(_caretPos + 1, 0, newText.Length);
                    ValidateCursorPosition(newText.ToString());
                }

                else if (info.KeysPressed[i].Key == Keys.Home)
                {
                    _caretPos = 0;
                    ValidateCursorPosition(newText.ToString());
                }

                else if (info.KeysPressed[i].Key == Keys.End)
                {
                    _caretPos = newText.Length;
                    ValidateCursorPosition(newText.ToString());
                }

                else if (info.KeysPressed[i].Character != 0 && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
                {
                    newText.Insert(_caretPos, info.KeysPressed[i].Character);
                    _caretPos++;
                    ValidateCursorPosition(newText.ToString());
                }
            }

            Text = newText.ToString();

            return true;
        }

        return false;
    }

    /// <summary>
    /// Called when the control loses focus.
    /// </summary>
    protected override void OnUnfocused()
    {
        base.OnUnfocused();
        IsDirty = true;
    }

    /// <summary>
    /// Called when the control is focused.
    /// </summary>
    protected override void OnFocused()
    {
        base.OnFocused();
        ValidateCursorPosition(_text);
        IsDirty = true;
    }

    /// <summary>
    /// Focuses the control and enters typing mode.
    /// </summary>
    /// <param name="state">The mouse state.</param>
    protected override void OnLeftMouseClicked(ControlMouseState state)
    {
        if (!DisableMouse)
        {
            base.OnLeftMouseClicked(state);

            if (!IsFocused && Parent?.Host != null)
                Parent.Host.FocusedControl = this;

            IsDirty = true;
        }
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        Text = _text;
        DetermineState();
        IsDirty = true;
    }
}
