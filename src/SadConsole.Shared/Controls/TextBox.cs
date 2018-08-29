using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Input;
using SadConsole.Themes;

using System;
using System.Runtime.Serialization;
using System.Windows;
using SadConsole.Surfaces;

namespace SadConsole.Controls
{
    /// <summary>
    /// InputBox control that allows text input.
    /// </summary>
    [DataContract]
    public class TextBox: ControlBase
    {
        /// <summary>
        /// Indicates the caret is visible.
        /// </summary>
        public bool IsCaretVisible = false;

        /// <summary>
        /// A list of valid number characters
        /// </summary>
        protected static char[] _validNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// The theme of the control.
        /// </summary>
        [DataMember(Name="Theme")]
        protected TextBoxTheme _theme;

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
        protected int _caretPos;

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
        /// Disables mouse input.
        /// </summary>
        [DataMember(Name = "DisableMouseInput")]
        public bool DisableMouse;

        /// <summary>
        /// Disables the keyboard which turns off keyboard input and hides the cursor.
        /// </summary>
        [DataMember(Name = "DisableKeyboardInput")]
        public bool DisableKeyboard;

        /// <summary>
        /// A temp holder for the text as it's being edited.
        /// </summary>
        public string EditingText { get; protected set; }

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public  TextBoxTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                _theme.Attached(this);
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
                    TextChangedEventArgs args = new TextChangedEventArgs
                    {
                        NewValue = MaxLength != 0 && value.Length > MaxLength ? value.Substring(0, MaxLength) : value,
                        OldValue = _text
                    };

                    TextChangedPreview?.Invoke(this, args);

                    _text = args.NewValue ?? "";
					_text = MaxLength != 0 && _text.Length > MaxLength ? _text.Substring(0, MaxLength) : _text;

					Validate();
                    EditingText = _text;
					PositionCursor();

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
            Theme = (TextBoxTheme) Library.Default.TextBoxTheme.Clone();
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
                    float value;
                    if (_text != null & float.TryParse(_text, out value))
                        _text = value.ToString();
                    else
                        _text = "0.0";

                    if (!_text.Contains("."))
                        _text = _text + ".0";
                }
                else
                {
                    int value;
                    if (_text != null & int.TryParse(_text, out value))
                        _text = value.ToString();
                    else
                        _text = "0";
                }
            }

            PositionCursor();

            DetermineState();
            IsDirty = true;
        }

        protected void ValidateEdit()
        {
            PositionCursor();

            DetermineState();
            IsDirty = true;
        }

        /// <summary>
        /// Correctly positions the cursor within the text.
        /// </summary>
        protected void PositionCursor()
        {
            if (MaxLength != 0 && EditingText.Length > MaxLength)
            {
                EditingText = EditingText.Substring(0, MaxLength);

                if (EditingText.Length == MaxLength)
                    _caretPos = EditingText.Length - 1;
                else
                    _caretPos = EditingText.Length;
            }
            else
                _caretPos = EditingText.Length;

			// Test to see if caret is off edge of box
			if (_caretPos >= Width)
			{
				LeftDrawOffset = EditingText.Length - Width + 1;

				if (LeftDrawOffset < 0)
				    LeftDrawOffset = 0;
			}
			else
			{
			    LeftDrawOffset = 0;
			}
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
                            this.IsDirty = true;
                            DisableKeyboard = false;
                            Text = EditingText;
                        }
                    }
                    return true;
                }
                else
                {
                    System.Text.StringBuilder newText = new System.Text.StringBuilder(EditingText, Width - 1);

                    this.IsDirty = true;

					for (int i = 0; i < info.KeysPressed.Count; i++)
					{
						if (_isNumeric)
						{
                            if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0)
								newText.Remove(newText.Length - 1, 1);

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
							}

							PositionCursor();
						}

						else
						{
                            if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0 && _caretPos != 0)
                            {
                                if (_caretPos == newText.Length)
									newText.Remove(newText.Length - 1, 1);
								else
									newText.Remove(_caretPos - 1, 1);

								_caretPos -= 1;

								if (_caretPos == -1)
									_caretPos = 0;
							}
							else if (info.KeysPressed[i].Key == Keys.Space && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
							{
								newText.Insert(_caretPos, ' ');
								_caretPos++;

								if (_caretPos > newText.Length)
									_caretPos = newText.Length;
							}

							else if (info.KeysPressed[i].Key == Keys.Delete && _caretPos != newText.Length)
							{
								newText.Remove(_caretPos, 1);

								if (_caretPos > newText.Length)
									_caretPos = newText.Length;
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
									_caretPos = 0;
							}
							else if (info.KeysPressed[i].Key == Keys.Right)
							{
								_caretPos += 1;

								if (_caretPos > newText.Length)
									_caretPos = newText.Length;
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
									_caretPos = newText.Length;
							}

							// Test to see if caret is off edge of box
							if (_caretPos >= Width)
							{
							    LeftDrawOffset = newText.Length - Width + 1;

								if (LeftDrawOffset < 0)
								    LeftDrawOffset = 0;
							}
							else
							{
							    LeftDrawOffset = 0;
							}
						}

					}

                    string newString = newText.ToString();
                    if (newString != EditingText)
                        EditingText = newString;

                    ValidateEdit();
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
            PositionCursor();
        }

        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            if (!DisableMouse)
            {
                base.OnLeftMouseClicked(state);

                DisableKeyboard = false;

                if (!IsFocused)
                    Parent.FocusedControl = this;

                IsDirty = true;    
            }
        }

        /// <inheritdoc />
        public override void Update(TimeSpan time)
        {
            Theme.UpdateAndDraw(this, time);
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            Text = _text;
            DetermineState();
            IsDirty = true;
        }

        public class TextChangedEventArgs : EventArgs
        {
            public string OldValue;
            public string NewValue;
        }
    }
}
