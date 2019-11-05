#if XNA
using Microsoft.Xna.Framework.Input;
#endif

using System;
using System.Runtime.Serialization;

namespace SadConsole.Controls
{
    /// <summary>
    /// InputBox control that allows text input.
    /// </summary>
    [DataContract]
    public class TextBox : ControlBase
    {
        private string _editingText;
        private bool _disableKeyboardEdit;

        /// <summary>
        /// Mask input with a certain character.
        /// </summary>
        public string PasswordChar;

        /// <summary>
        /// Indicates the caret is visible.
        /// </summary>
        public bool IsCaretVisible = false;

        /// <summary>
        /// A list of valid number characters
        /// </summary>
        protected static char[] _validNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// The alignment of the text.
        /// </summary>
        [DataMember(Name = "TextAlignment")]
        protected HorizontalAlignment _alignment = HorizontalAlignment.Left;

        /// <summary>
        /// When editing the text box, this allows the text to scroll to the right so you can see what you are typing.
        /// </summary>
        public int LeftDrawOffset { get; protected set; }

        /// <summary>
        /// The location of the caret.
        /// </summary>
        [DataMember(Name = "CaretPosition")]
        private int _caretPos;

        /// <summary>
        /// The text value of the input box.
        /// </summary>
        [DataMember(Name = "Text")]
        protected string _text = "";

        /// <summary>
        /// Indicates the input box is numeric only.
        /// </summary>
        [DataMember(Name = "IsNumeric")]
        protected bool _isNumeric;

        /// <summary>
        /// Indicates that the input box (when numeric) will accept decimal points.
        /// </summary>
        [DataMember(Name = "AllowDecimalPoint")]
        protected bool _allowDecimalPoint;

        /// <summary>
        /// The current appearance of the control.
        /// </summary>
        protected Cell _currentAppearance;

        /// <summary>
        /// Raised when the text has changed and the preview has accepted it.
        /// </summary>
        public event EventHandler TextChanged;

        /// <summary>
        /// Raised before the text has changed and allows the change to be cancelled.
        /// </summary>
        public event EventHandler<TextChangedEventArgs> TextChangedPreview;

        /// <summary>
        /// Raised when a key is pressed on the textbox.
        /// </summary>
        public event EventHandler<KeyPressEventArgs> KeyPressed;

        /// <summary>
        /// Disables mouse input.
        /// </summary>
        [DataMember(Name = "DisableMouseInput")]
        public bool DisableMouse;

        /// <summary>
        /// Disables the keyboard which turns off keyboard input and hides the cursor.
        /// </summary>
        [DataMember(Name = "DisableKeyboardInput")]
        public bool DisableKeyboard
        {
            get => _disableKeyboardEdit;
            set
            {
                _disableKeyboardEdit = value;

                if (!_disableKeyboardEdit)
                {
                    _caretPos = Text.Length;
                }
            }
        }

        /// <summary>
        /// A temp holder for the text as it's being edited.
        /// </summary>
        public string EditingText
        {
            get => _editingText;
            protected set
            {
                _editingText = value;

                if (MaxLength != 0)
                {
                    if (_editingText.Length >= MaxLength)
                    {
                        _editingText = _editingText.Substring(0, MaxLength);
                    }
                }

                ValidateCursorPosition();
                DetermineState();
                IsDirty = true;
            }
        }

        /// <summary>
        /// The alignment of the caret.
        /// </summary>
        public HorizontalAlignment TextAlignment
        {
            get => _alignment;
            set
            {
                _alignment = value;
                DetermineState();
                IsDirty = true;
            }
        }

        /// <summary>
        /// How big the text can be. Setting this to 0 will make it unlimited.
        /// </summary>
        [DataMember]
        public int MaxLength { get; set; }

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
                    var args = new TextChangedEventArgs(_text, MaxLength != 0 && value.Length > MaxLength ? value.Substring(0, MaxLength) : value);

                    TextChangedPreview?.Invoke(this, args);

                    _text = args.NewValue ?? "";
                    _text = MaxLength != 0 && _text.Length > MaxLength ? _text.Substring(0, MaxLength) : _text;

                    Validate();
                    EditingText = _text;
                    _caretPos = Text.Length;

                    TextChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets weather or not this input box only allows numeric input.
        /// </summary>
        public bool IsNumeric
        {
            get => _isNumeric;
            set { _isNumeric = value; Validate(); }
        }

        /// <summary>
        /// Gets or sets weather or not this input box should restrict numeric input should allow a decimal point.
        /// </summary>
        public bool AllowDecimal
        {
            get => _allowDecimalPoint;
            set { _allowDecimalPoint = value; Validate(); }
        }

        #region Constructors
        /// <summary>
        /// Creates a new instance of the input box.
        /// </summary>
        /// <param name="width">The width of the input box.</param>
        public TextBox(int width)
            : base(width, 1)
        {
        }
        #endregion

        /// <summary>
        /// Validates that the value of the input box conforms to the settings of this control and sets the dirty flag to true.
        /// </summary>
        protected void Validate()
        {
            if (_isNumeric)
            {
                if (_allowDecimalPoint)
                {
                    if (_text != null & double.TryParse(_text, out double value))
                    {
                        _text = value.ToString();
                    }
                    else
                    {
                        _text = "0.0";
                    }

                    if (!_text.Contains("."))
                    {
                        _text = _text + ".0";
                    }
                }
                else
                {
                    if (_text != null & int.TryParse(_text, out int value))
                    {
                        _text = value.ToString();
                    }
                    else
                    {
                        _text = "0";
                    }
                }
            }
            DetermineState();
            IsDirty = true;
        }

        /// <summary>
        /// Correctly positions the cursor within the text.
        /// </summary>
        protected void ValidateCursorPosition()
        {
            if (MaxLength != 0)
            {
                if (_caretPos > EditingText.Length)
                {
                    _caretPos = EditingText.Length - 1;
                }
            }
            else if (_caretPos > EditingText.Length)
            {
                _caretPos = EditingText.Length;
            }


            // Test to see if caret is off edge of box
            if (_caretPos >= Width)
            {
                LeftDrawOffset = EditingText.Length - Width + 1;

                if (LeftDrawOffset < 0)
                {
                    LeftDrawOffset = 0;
                }
            }
            else
            {
                LeftDrawOffset = 0;
            }

            DetermineState();
            IsDirty = true;
        }


        private bool TriggerKeyPressEvent(Input.AsciiKey key)
        {
            if (KeyPressed == null)
            {
                return false;
            }

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
            if (info.KeysPressed.Count != 0)
            {
                if (DisableKeyboard)
                {
                    for (int i = 0; i < info.KeysPressed.Count; i++)
                    {
                        if (info.KeysPressed[i].Key == Keys.Enter)
                        {
                            if (TriggerKeyPressEvent(info.KeysPressed[i]))
                            {
                                return false;
                            }

                            IsDirty = true;
                            DisableKeyboard = false;
                            Text = EditingText;
                        }
                    }
                    return true;
                }
                else
                {
                    var newText = new System.Text.StringBuilder(EditingText, Width - 1);

                    IsDirty = true;

                    for (int i = 0; i < info.KeysPressed.Count; i++)
                    {
                        if (TriggerKeyPressEvent(info.KeysPressed[i]))
                        {
                            return false;
                        }

                        if (_isNumeric)
                        {
                            if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0)
                            {
                                newText.Remove(newText.Length - 1, 1);
                                _caretPos -= 1;

                                if (_caretPos == -1)
                                {
                                    _caretPos = 0;
                                }
                            }
                            else if (info.KeysPressed[i].Key == Keys.Enter)
                            {
                                DisableKeyboard = true;
                                Text = EditingText;
                                return true;
                            }
                            else if (info.KeysPressed[i].Key == Keys.Escape)
                            {
                                DisableKeyboard = true;
                                return true;
                            }

                            else if (char.IsDigit(info.KeysPressed[i].Character) || (_allowDecimalPoint && info.KeysPressed[i].Character == '.'))
                            {
                                newText.Append(info.KeysPressed[i].Character);
                                _caretPos += 1;
                            }

                        }

                        else
                        {
                            if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0 && _caretPos != 0)
                            {
                                if (_caretPos == newText.Length)
                                {
                                    newText.Remove(newText.Length - 1, 1);
                                }
                                else
                                {
                                    newText.Remove(_caretPos - 1, 1);
                                }

                                _caretPos -= 1;

                                if (_caretPos == -1)
                                {
                                    _caretPos = 0;
                                }
                            }
                            else if (info.KeysPressed[i].Key == Keys.Space && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
                            {
                                newText.Insert(_caretPos, ' ');
                                _caretPos++;

                                if (_caretPos > newText.Length)
                                {
                                    _caretPos = newText.Length;
                                }
                            }

                            else if (info.KeysPressed[i].Key == Keys.Delete && _caretPos != newText.Length)
                            {
                                newText.Remove(_caretPos, 1);

                                if (_caretPos > newText.Length)
                                {
                                    _caretPos = newText.Length;
                                }
                            }

                            else if (info.KeysPressed[i].Key == Keys.Enter)
                            {
                                Text = EditingText;
                                DisableKeyboard = true;
                                return true;
                            }
                            else if (info.KeysPressed[i].Key == Keys.Escape)
                            {
                                DisableKeyboard = true;
                                return true;
                            }
                            else if (info.KeysPressed[i].Key == Keys.Left)
                            {
                                _caretPos -= 1;

                                if (_caretPos == -1)
                                {
                                    _caretPos = 0;
                                }
                            }
                            else if (info.KeysPressed[i].Key == Keys.Right)
                            {
                                _caretPos += 1;

                                if (_caretPos > newText.Length)
                                {
                                    _caretPos = newText.Length;
                                }
                            }

                            else if (info.KeysPressed[i].Key == Keys.Home)
                            {
                                _caretPos = 0;
                            }

                            else if (info.KeysPressed[i].Key == Keys.End)
                            {
                                _caretPos = newText.Length;
                            }

                            else if (info.KeysPressed[i].Character != 0 && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
                            {
                                newText.Insert(_caretPos, info.KeysPressed[i].Character);
                                _caretPos++;

                                if (_caretPos > newText.Length)
                                {
                                    _caretPos = newText.Length;
                                }
                            }
                        }
                    }

                    EditingText = newText.ToString();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the control loses focus.
        /// </summary>
        public override void FocusLost()
        {
            base.FocusLost();
            DisableKeyboard = true;
            Text = EditingText;
            IsDirty = true;
        }

        /// <summary>
        /// Called when the control is focused.
        /// </summary>
        public override void Focused()
        {
            base.Focused();
            DisableKeyboard = false;
            EditingText = _text;
            IsDirty = true;
            ValidateCursorPosition();
        }

        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            if (!DisableMouse)
            {
                base.OnLeftMouseClicked(state);

                DisableKeyboard = false;

                if (!IsFocused)
                {
                    Parent.FocusedControl = this;
                }

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

        /// <summary>
        /// Event arguments that indicate the change in text for a textbox control.
        /// </summary>
        public class TextChangedEventArgs : EventArgs
        {
            /// <summary>
            /// The original text value.
            /// </summary>
            public readonly string OldValue;

            /// <summary>
            /// The new text of the textbox.
            /// </summary>
            public string NewValue { get; set; }

            /// <summary>
            /// Creates a new event args object.
            /// </summary>
            /// <param name="oldValue">The original value of the text.</param>
            /// <param name="newValue">The value the text is chaning to.</param>
            public TextChangedEventArgs(string oldValue, string newValue)
            {
                OldValue = oldValue;
                NewValue = newValue;
            }

        }

        /// <summary>
        /// Event arguments to indicate that a key is being pressed on the textbox.
        /// </summary>
        public class KeyPressEventArgs : EventArgs
        {
            /// <summary>
            /// The key being pressed by the textbox.
            /// </summary>
            public readonly Input.AsciiKey Key;

            /// <summary>
            /// When set to <see langword="true"/>, causes the textbox to cancel the key press.
            /// </summary>
            public bool IsCancelled { get; set; }

            /// <summary>
            /// Creates a new event args object.
            /// </summary>
            /// <param name="key">The key being pressed.</param>
            public KeyPressEventArgs(Input.AsciiKey key) =>
                Key = key;
        }
    }
}
