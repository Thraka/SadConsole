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
        protected string _text;

        /// <summary>
        /// The alignment of the <see cref="_text"/>.
        /// </summary>
        [DataMember(Name = "TextAlignment")]
        protected HorizontalAlignment _textAlignment = HorizontalAlignment.Center;

        /// <summary>
        /// The text displayed on the control.
        /// </summary>
        public string Text
        {
            get => _text;
            set { _text = value; IsDirty = true; }
        }

        /// <summary>
        /// The alignment of the text, left, center, or right.
        /// </summary>
        public HorizontalAlignment TextAlignment
        {
            get => _textAlignment;
            set { _textAlignment = value; IsDirty = true; }
        }

        /// <summary>
        /// Creates a new button control.
        /// </summary>
        /// <param name="width">Width of the button.</param>
        /// <param name="height">Height of the button.</param>
        public ButtonBase(int width, int height) : base(width, height) {  }

        /// <summary>
        /// Raises the <see cref="Click"/> event.
        /// </summary>
        protected virtual void OnClick() => Click?.Invoke(this, new EventArgs());

        /// <summary>
        /// Simulates a mouse click on the button.
        /// </summary>
        public void InvokeClick() => OnClick();

        /// <summary>
        /// Detects if the SPACE or ENTER keys are pressed and calls the <see cref="Click"/> method.
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

        /// <inheritdoc />
        protected override void OnMouseIn(ControlMouseState state)
        {
            base.OnMouseIn(state);

            if (IsEnabled && _mouseDownForClick && !state.OriginalMouseState.Mouse.LeftButtonDown && !state.OriginalMouseState.Mouse.LeftClicked)
            {
                OnLeftMouseClicked(state);
                _mouseDownForClick = false;
            }
            else if (!MouseState_EnteredWithButtonDown && state.OriginalMouseState.Mouse.LeftButtonDown)
                _mouseDownForClick = true;

        }

        /// <inheritdoc />
        protected override void OnLeftMouseClicked(ControlMouseState state)
        {
            _mouseDownForClick = false;
            base.OnLeftMouseClicked(state);
            OnClick();
        }

        /// <inheritdoc />
        protected override void OnMouseExit(ControlMouseState state)
        {
            base.OnMouseExit(state);
            _mouseDownForClick = false;
        }
    }
}
