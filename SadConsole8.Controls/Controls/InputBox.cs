namespace SadConsole.Controls
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using SadConsole.Themes;
    using System;
    using System.Runtime.Serialization;
    using System.Windows;

    /// <summary>
    /// InputBox control that allows text input.
    /// </summary>
    [DataContract]
    public class InputBox: ControlBase
    {
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
        protected CellAppearance _currentAppearance;

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
                TextChangedEventArgs args = new TextChangedEventArgs();
                args.NewValue = value;
                args.OldValue = _text;

                if (TextChangedPreview != null)
                    TextChangedPreview(this, args);

                _text = args.NewValue ?? "";

                Validate();

                if (TextChanged != null)
                    TextChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets weather or not this input box only allows numeric input.
        /// </summary>
        public bool IsNumeric
        {
            get { return _isNumeric; }
            set { _isNumeric = value; Text = Text; }
        }

        /// <summary>
        /// Gets or sets weather or not this input box should restrict numeric input should allow a decimal point.
        /// </summary>
        public bool AllowDecimal
        {
            get { return _allowDecimalPoint; }
            set { _allowDecimalPoint = value; Text = Text; }
        }

        #region Constructors
        /// <summary>
        /// Creates a new instance of the input box.
        /// </summary>
        /// <param name="width">The width of the input box.</param>
        public InputBox(int width)
            : base()
        {
            base.Resize(width, 1);

            DetermineAppearance();
        }
        #endregion

        /// <summary>
        /// Repositions the cursor after a resize.
        /// </summary>
        protected override void OnResize()
        {
            PositionCursor();
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        public override void Compose()
        {
            if (this.IsDirty)
            {
                this.Fill(_currentAppearance.Foreground, _currentAppearance.Background, _currentAppearance.CharacterIndex, null);

                this.Print(0, 0, Text.Align(TextAlignment, this.Width));

                if (base.IsFocused && !DisableKeyboard)
                {
                    SetEffect(this[this._carrotPos, 0], Theme.CarrotEffect.Clone());
                }
                else
                {

                }

                this.IsDirty = false;
            }
        }

        /// <summary>
        /// Determines the appearance of the control based on its current state.
        /// </summary>
        public override void DetermineAppearance()
        {
            CellAppearance currentappearance = _currentAppearance;

            if (_isMouseOver)
                _currentAppearance = Theme.MouseOver;

            else if (base.IsFocused && Engine.ActiveConsole == _parent)
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

        /// <summary>
        /// Correctly positions the cursor within the text.
        /// </summary>
        protected void PositionCursor()
        {
            if (MaxLength != 0 && _text.Length > MaxLength)
            {
                _text = _text.Substring(0, MaxLength);

                if (_text.Length == MaxLength)
                    _carrotPos = _text.Length - 1;
                else
                    _carrotPos = _text.Length;
            }
            else
                _carrotPos = _text.Length;

            if (_carrotPos >= this.Width)
                _carrotPos = this.Width - 1;
        }

        /// <summary>
        /// Called when the control should process keyboard information.
        /// </summary>
        /// <param name="info">The keyboard information.</param>
        /// <returns>True if the keyboard was handled by this control.</returns>
        public override bool ProcessKeyboard(Input.KeyboardInfo info)
        {
            if (info.KeysPressed.Count != 0)
            {
                if (DisableKeyboard)
                {
                    for (int i = 0; i < info.KeysPressed.Count; i++)
                    {
                        if (info.KeysPressed[i].XnaKey == Keys.Enter)
                        {
                            this.IsDirty = true;
                            DisableKeyboard = false;
                        }
                    }
                    return true;
                }
                else
                {
                    System.Text.StringBuilder newText = new System.Text.StringBuilder(_text, this.Width - 1);

                    this.IsDirty = true;

                    for (int i = 0; i < info.KeysPressed.Count; i++)
                    {
                        if (_isNumeric)
                        {
                            if (info.KeysPressed[i].XnaKey == Keys.Back && newText.Length != 0)
                                newText.Remove(newText.Length - 1, 1);

                            else if (info.KeysPressed[i].XnaKey == Keys.Enter || info.KeysPressed[i].XnaKey == Keys.Escape)
                            {
                                DisableKeyboard = true;

                                Validate();
                                Text = _text;

                                return true;
                            }

                            else if (char.IsDigit(info.KeysPressed[i].Character) || (_allowDecimalPoint && info.KeysPressed[i].Character == '.'))
                            {
                                newText.Append(info.KeysPressed[i].Character);
                            }
                        }

                        else
                        {
                            if (info.KeysPressed[i].XnaKey == Keys.Back && newText.Length != 0)
                                newText.Remove(newText.Length - 1, 1);
                            else if (info.KeysPressed[i].XnaKey == Keys.Space)
                                newText.Append(' ');
                            else if (info.KeysPressed[i].Character != 0)
                                newText.Append(info.KeysPressed[i].Character);
                            else if (info.KeysPressed[i].XnaKey == Keys.Enter || info.KeysPressed[i].XnaKey == Keys.Escape)
                                DisableKeyboard = true;
                        }

                    }

                    string newString = newText.ToString();
                    if (newString != _text)
                        _text = newString;

                    PositionCursor();
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
            Validate();
            DisableKeyboard = true;
            Text = _text;
        }

        /// <summary>
        /// Called when the control is focused.
        /// </summary>
        public override void Focused()
        {
            base.Focused();
            DisableKeyboard = false;
            IsDirty = true;
        }

        protected override void OnLeftMouseClicked(Input.MouseInfo info)
        {
            if (!DisableMouse)
            {
                base.OnLeftMouseClicked(info);

                DisableKeyboard = false;

                if (!IsFocused)
                    Parent.FocusedControl = this;
            }
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
