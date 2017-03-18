using Microsoft.Xna.Framework.Input;

using SadConsole.Themes;
using System;
using System.Runtime.Serialization;

namespace SadConsole.Controls
{
    
    /// <summary>
    /// Represents a button that can be toggled on/off within a group of other buttons.
    /// </summary>
    [DataContract]
    public class CheckBox : ControlBase
    {
        /// <summary>
        /// Raised when the selected state of the radio button is changed.
        /// </summary>
        public event EventHandler IsSelectedChanged;

        [DataMember(Name = "Group")]
        protected string _groupName = "";
        [DataMember(Name = "Theme")]
        protected CheckBoxTheme _theme;
        [DataMember(Name = "Text")]
        protected string _text;
        [DataMember(Name = "TextAlignment")]
        protected System.Windows.HorizontalAlignment _textAlignment;
        [DataMember(Name = "IsSelected")]
        protected bool _isSelected;
        protected bool _isMouseDown;
        protected Cell _currentAppearanceButton;
        protected Cell _currentAppearanceText;

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual CheckBoxTheme Theme
        {
            get
            {
                if (_theme == null)
                    return Library.Default.CheckBoxTheme;
                else
                    return _theme;
            }
            set
            {
                _theme = value;
            }
        }

        /// <summary>
        /// The text displayed on the control.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; Compose(true); }
        }

        /// <summary>
        /// The alignment of the text, left, center, or right.
        /// </summary>
        public System.Windows.HorizontalAlignment TextAlignment
        {
            get { return _textAlignment; }
            set { _textAlignment = value; Compose(true); }
        }

        /// <summary>
        /// Gets or sets the selected state of the radio button.
        /// </summary>
        /// <remarks>Radio buttons within the same group will set their IsSelected property to the opposite of this radio button when you set this property.</remarks>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    if (IsSelectedChanged != null)
                        IsSelectedChanged(this, EventArgs.Empty);

                    //if (value)
                    //    OnAction();
                    DetermineAppearance();
                    Compose(true);
                }
            }
        }

        /// <summary>
        /// Creates a new radio button control with the specified width and height.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public CheckBox(int width, int height) : base(width, height)
        {
            DetermineAppearance();
        }

        /// <summary>
        /// Determines the appearance of the control based on its current state.
        /// </summary>
        public override void DetermineAppearance()
        {
            Cell currentappearanceButton = _currentAppearanceButton;
            Cell currentappearanceText = _currentAppearanceText;

            if (!isEnabled)
            {
                _currentAppearanceButton = Theme.Button.Disabled;
                _currentAppearanceText = Theme.Disabled;
            }

            else if (!_isMouseDown && isMouseOver)
            {
                _currentAppearanceButton = Theme.Button.MouseOver;
                _currentAppearanceText = Theme.MouseOver;
            }

            else if (!_isMouseDown && !isMouseOver && IsFocused && Global.FocusedConsoles.Console == parent)
            {
                _currentAppearanceButton = Theme.Button.Focused;
                _currentAppearanceText = Theme.Focused;
            }

            else if (_isMouseDown && isMouseOver)
            {
                _currentAppearanceButton = Theme.Button.MouseClicking;
                _currentAppearanceText = Theme.MouseClicking;
            }

            else if (_isSelected)
            {
                _currentAppearanceButton = Theme.Button.Selected;
                _currentAppearanceText = Theme.Selected;
            }

            else
            {
                _currentAppearanceButton = Theme.Button.Normal;
                _currentAppearanceText = Theme.Normal;
            }

            if (currentappearanceButton != _currentAppearanceButton ||
                currentappearanceText != _currentAppearanceText)

                this.IsDirty = true;
        }

        protected override void OnMouseIn(Input.MouseConsoleState state)
        {
            isMouseOver = true;

            base.OnMouseIn(state);
        }

        protected override void OnMouseExit(Input.MouseConsoleState state)
        {
            isMouseOver = false;

            base.OnMouseExit(state);
        }

        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            base.OnLeftMouseClicked(state);

            if (isEnabled)
            {
                IsSelected = !IsSelected;
            }
        }

        /// <summary>
        /// Called when the control should process keyboard information.
        /// </summary>
        /// <param name="info">The keyboard information.</param>
        /// <returns>True if the keyboard was handled by this control.</returns>
        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            if (info.IsKeyReleased(Keys.Space) || info.IsKeyReleased(Keys.Enter))
            {
                IsSelected = !IsSelected;

                return true;
            }
            else if (Parent != null)
            {
                if (info.IsKeyReleased(Keys.Up))
                    Parent.TabPreviousControl();

                else if (info.IsKeyReleased(Keys.Down))
                    Parent.TabNextControl();

                return true;
            }

            return false;
        }

        public override void Compose()
        {
            if (this.IsDirty)
            {
                // If we are doing text, then print it otherwise we're just displaying the button part
                if (Width != 1)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        this.SetCell(x, 0, _currentAppearanceButton);
                    }
                    this.Fill(_currentAppearanceText.Foreground, _currentAppearanceText.Background, _currentAppearanceText.Glyph, null);
                    this.Print(4, 0, Text.Align(TextAlignment, this.Width - 4));
                    this.SetGlyph(0, 0, 91);
                    this.SetGlyph(2, 0, 93);

                    if (_isSelected)
                        this.SetGlyph(1, 0, Theme.CheckedIcon);
                    else
                        this.SetGlyph(1, 0, Theme.UncheckedIcon);
                }
                else
                {
                }

                OnComposed?.Invoke(this);

                this.IsDirty = false;
            }
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            DetermineAppearance();
            Compose(true);
        }
    }
}
