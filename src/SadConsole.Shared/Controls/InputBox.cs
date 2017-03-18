using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Input;
using SadConsole.Themes;

using System;
using System.Runtime.Serialization;
using System.Windows;

namespace SadConsole.Controls
{




    /// <summary>
    /// InputBox control that allows text input.
    /// </summary>
    [DataContract]
    public class InputBox: ControlBase
    {
        /// <summary>
        /// Indicates the carrot is visible.
        /// </summary>
        protected bool isCarrotVisible = false;

        /// <summary>
        /// A list of valid number characters
        /// </summary>
        protected static char[] _validNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// The theme of the control.
        /// </summary>
        [DataMember(Name="Theme")]
        protected InputBoxTheme _theme;

        /// <summary>
        /// The alignment of the text.
        /// </summary>
        [DataMember(Name = "TextAlignment")]
        protected HorizontalAlignment _alignment = HorizontalAlignment.Left;

		/// <summary>
		/// When editing the text box, this allows the text to scroll to the right so you can see what you are typing.
		/// </summary>
		protected int _leftDrawOffset;

        /// <summary>
        /// The location of the carrot.
        /// </summary>
        [DataMember(Name = "CarrotPosition")]
        protected int _carrotPos;

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

        private string _editingText = "";
        private Effects.EffectsManager effects;

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual InputBoxTheme Theme
        {
            get
            {
                if (_theme == null)
                    return Library.Default.InputBoxTheme;
                else
                    return _theme;
            }
            set
            {
                _theme = value;
            }
        }

        /// <summary>
        /// The alignment of the carrot.
        /// </summary>
        public HorizontalAlignment TextAlignment
        {
            get { return _alignment; }
            set { _alignment = value; this.IsDirty = true; }
        }

        /// <summary>
        /// How big the text can be. Setting this to 0 will make it unlimited.
        /// </summary>
        [DataMember]
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the position of the carrot in the current text.
        /// </summary>
        public int CarrotPosition
        {
            get { return _carrotPos; }
            set
            {
                _carrotPos = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the text of the input box.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    TextChangedEventArgs args = new TextChangedEventArgs();
                    args.NewValue = MaxLength != 0 && value.Length > MaxLength ? value.Substring(0, MaxLength) : value;
                    args.OldValue = _text;

                    if (TextChangedPreview != null)
                        TextChangedPreview(this, args);

                    _text = args.NewValue ?? "";
					_text = MaxLength != 0 && _text.Length > MaxLength ? _text.Substring(0, MaxLength) : _text;

					Validate();
                    _editingText = _text;
					PositionCursor();
					
                    if (TextChanged != null)
                        TextChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets weather or not this input box only allows numeric input.
        /// </summary>
        public bool IsNumeric
        {
            get { return _isNumeric; }
            set { _isNumeric = value; Validate(); }
        }

        /// <summary>
        /// Gets or sets weather or not this input box should restrict numeric input should allow a decimal point.
        /// </summary>
        public bool AllowDecimal
        {
            get { return _allowDecimalPoint; }
            set { _allowDecimalPoint = value; Validate(); }
        }

        #region Constructors
        /// <summary>
        /// Creates a new instance of the input box.
        /// </summary>
        /// <param name="width">The width of the input box.</param>
        public InputBox(int width)
            : base(width, 1)
        {
            effects = new Effects.EffectsManager(textSurface);
            DetermineAppearance();
        }
        #endregion

        /// <summary>
        /// Draws the control.
        /// </summary>
        public override void Compose()
        {
            if (this.IsDirty)
            {
                this.Fill(_currentAppearance.Foreground, _currentAppearance.Background, _currentAppearance.Glyph, null);

                effects.RemoveAll();

                if (base.IsFocused && !DisableKeyboard)
                {
                    this.Print(0, 0, _editingText.Substring(_leftDrawOffset));
                    effects.SetEffect(this[this._carrotPos - _leftDrawOffset, 0], Theme.CarrotEffect);
                    isCarrotVisible = true;
                }
                else
                {
                    isCarrotVisible = false;
                    this.Print(0, 0, _text.Align(TextAlignment, this.Width));
                }

                OnComposed?.Invoke(this);
                this.IsDirty = false;
            }
        }

        /// <summary>
        /// Determines the appearance of the control based on its current state.
        /// </summary>
        public override void DetermineAppearance()
        {
            Cell currentappearance = _currentAppearance;

            if (isMouseOver)
                _currentAppearance = Theme.MouseOver;

            else if (base.IsFocused && Global.FocusedConsoles.Console == parent)
                _currentAppearance = Theme.Focused;
            else
                _currentAppearance = Theme.Normal;

            if (currentappearance != _currentAppearance)
                IsDirty = true;
        }

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

            this.IsDirty = true;
        }

        protected void ValidateEdit()
        {
            PositionCursor();

            this.IsDirty = true;
        }

        /// <summary>
        /// Correctly positions the cursor within the text.
        /// </summary>
        protected void PositionCursor()
        {
            if (MaxLength != 0 && _editingText.Length > MaxLength)
            {
                _editingText = _editingText.Substring(0, MaxLength);

                if (_editingText.Length == MaxLength)
                    _carrotPos = _editingText.Length - 1;
                else
                    _carrotPos = _editingText.Length;
            }
            else
                _carrotPos = _editingText.Length;

			// Test to see if carrot is off edge of box
			if (_carrotPos >= Width)
			{
				_leftDrawOffset = _editingText.Length - Width + 1;

				if (_leftDrawOffset < 0)
					_leftDrawOffset = 0;
			}
			else
			{
				_leftDrawOffset = 0;
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
                            Text = _editingText;
                        }
                    }
                    return true;
                }
                else
                {
                    System.Text.StringBuilder newText = new System.Text.StringBuilder(_editingText, textSurface.Width - 1);

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

								Text = _editingText;

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
                            if (info.KeysPressed[i].Key == Keys.Back && newText.Length != 0 && _carrotPos != 0)
                            {
                                if (_carrotPos == newText.Length)
									newText.Remove(newText.Length - 1, 1);
								else
									newText.Remove(_carrotPos - 1, 1);

								_carrotPos -= 1;

								if (_carrotPos == -1)
									_carrotPos = 0;
							}
							else if (info.KeysPressed[i].Key == Keys.Space && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
							{
								newText.Insert(_carrotPos, ' ');
								_carrotPos++;

								if (_carrotPos > newText.Length)
									_carrotPos = newText.Length;
							}

							else if (info.KeysPressed[i].Key == Keys.Delete && _carrotPos != newText.Length)
							{
								newText.Remove(_carrotPos, 1);

								if (_carrotPos > newText.Length)
									_carrotPos = newText.Length;
							}

							else if (info.KeysPressed[i].Key == Keys.Enter)
                            {
                                Text = _editingText;
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
								_carrotPos -= 1;

								if (_carrotPos == -1)
									_carrotPos = 0;
							}
							else if (info.KeysPressed[i].Key == Keys.Right)
							{
								_carrotPos += 1;

								if (_carrotPos > newText.Length)
									_carrotPos = newText.Length;
							}

							else if (info.KeysPressed[i].Key == Keys.Home)
							{
								_carrotPos = 0;
							}

							else if (info.KeysPressed[i].Key == Keys.End)
							{
									_carrotPos = newText.Length;
							}

							else if (info.KeysPressed[i].Character != 0 && (MaxLength == 0 || (MaxLength != 0 && newText.Length < MaxLength)))
							{
								newText.Insert(_carrotPos, info.KeysPressed[i].Character);
								_carrotPos++;

								if (_carrotPos > newText.Length)
									_carrotPos = newText.Length;
							}

							// Test to see if carrot is off edge of box
							if (_carrotPos >= Width)
							{
								_leftDrawOffset = newText.Length - Width + 1;

								if (_leftDrawOffset < 0)
									_leftDrawOffset = 0;
							}
							else
							{
								_leftDrawOffset = 0;
							}
						}

					}

                    string newString = newText.ToString();
                    if (newString != _editingText)
                        _editingText = newString;

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
            Text = _editingText;
            IsDirty = true;
        }

        /// <summary>
        /// Called when the control is focused.
        /// </summary>
        public override void Focused()
        {
            base.Focused();
            DisableKeyboard = false;
            _editingText = _text;
            IsDirty = true;
            PositionCursor();
        }

        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            if (!DisableMouse)
            {
                base.OnLeftMouseClicked(state);

                DisableKeyboard = false;
                _editingText = Text;

                if (!IsFocused)
                    Parent.FocusedControl = this;

                IsDirty = true;    
            }
        }

        public override void Update()
        {
            if (isCarrotVisible)
            {
                effects.UpdateEffects(Global.GameTimeElapsedUpdate);
                OnComposed?.Invoke(this);
            }

            base.Update();
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            Text = _text;
            DetermineAppearance();
            Compose(true);
        }

        public class TextChangedEventArgs : EventArgs
        {
            public string OldValue;
            public string NewValue;
        }
    }
}
