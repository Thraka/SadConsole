using System;
using System.Runtime.Serialization;
using SadConsole.Input;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// Base class for creating a button type control.
    /// </summary>
    [DataContract]
    public abstract class ButtonBase : ControlBase
    {
        bool _mouseDownForClick = false;

        /// <summary>
        /// Raised when the button is clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// The display text of the button.
        /// </summary>
        [DataMember(Name = "Text")]
        protected string text;

        /// <summary>
        /// The alignment of the <see cref="text"/>.
        /// </summary>
        [DataMember(Name = "TextAlignment")]
        protected HorizontalAlignment textAlignment = HorizontalAlignment.Center;

        /// <summary>
        /// The text displayed on the control.
        /// </summary>
        public string Text
        {
            get => text;
            set { text = value; IsDirty = true; }
        }

        /// <summary>
        /// The alignment of the text, left, center, or right.
        /// </summary>
        public HorizontalAlignment TextAlignment
        {
            get => textAlignment;
            set { textAlignment = value; IsDirty = true; }
        }

        /// <summary>
        /// Creates a new button control.
        /// </summary>
        /// <param name="width">Width of the button.</param>
        /// <param name="height">Height of the button.</param>
        public ButtonBase(int width, int height) : base(width, height) { }

        /// <summary>
        /// Raises the <see cref="Click"/> event.
        /// </summary>
        protected virtual void OnClick() => Click?.Invoke(this, new EventArgs());

        /// <summary>
        /// Simulates a mouse click on the button.
        /// </summary>
        public void InvokeClick() => OnClick();

        /// <summary>
        /// Detects if the SPACE and ENTER keys are pressed and calls the <see cref="Click"/> method.
        /// </summary>
        /// <param name="info"></param>
        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            if (info.IsKeyReleased(Keys.Space) || info.IsKeyReleased(Keys.Enter))
            {
                OnClick();
                return true;
            }

            return false;
        }

        protected override void OnMouseIn(ControlMouseState state)
        {
            base.OnMouseIn(state);

            if (_isEnabled && _mouseDownForClick && !state.OriginalMouseState.Mouse.LeftButtonDown && !state.OriginalMouseState.Mouse.LeftClicked)
            {
                OnLeftMouseClicked(state);
                _mouseDownForClick = false;
            }
            else if (!_mouseEnteredWithButtonDown && state.OriginalMouseState.Mouse.LeftButtonDown)
                _mouseDownForClick = true;

        }

        protected override void OnLeftMouseClicked(ControlMouseState state)
        {
            _mouseDownForClick = false;
            base.OnLeftMouseClicked(state);
            OnClick();
        }

        protected override void OnMouseExit(ControlMouseState state)
        {
            base.OnMouseExit(state);
            _mouseDownForClick = false;
        }
    }
}
