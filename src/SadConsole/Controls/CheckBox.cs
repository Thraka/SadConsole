#if XNA
using Microsoft.Xna.Framework.Input;
#endif

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
        [DataMember(Name = "Text")]
        protected string _text;
        [DataMember(Name = "TextAlignment")]
        protected HorizontalAlignment _textAlignment;
        [DataMember(Name = "IsSelected")]
        protected bool _isSelected;
        protected bool _isMouseDown;
        protected Cell _currentAppearanceButton;
        protected Cell _currentAppearanceText;

        /// <summary>
        /// The text displayed on the control.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                IsDirty = true;
                DetermineState();
            }
        }

        /// <summary>
        /// The alignment of the text, left, center, or right.
        /// </summary>
        public HorizontalAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                IsDirty = true;
                DetermineState();
            }
        }

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

                    IsSelectedChanged?.Invoke(this, EventArgs.Empty);

                    //if (value)
                    //    OnAction();
                    IsDirty = true;
                    DetermineState();
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
                {
                    Parent.TabPreviousControl();
                }
                else if (info.IsKeyReleased(Keys.Down))
                {
                    Parent.TabNextControl();
                }

                return true;
            }

            return false;
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            DetermineState();
            IsDirty = true;
        }
    }
}
