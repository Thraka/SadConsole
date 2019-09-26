#if XNA
using Microsoft.Xna.Framework.Input;
#endif 

using System;
using System.Runtime.Serialization;

namespace SadConsole.Controls
{
    /// <summary>
    /// Base class for creating a button type control.
    /// </summary>
    [DataContract]
    public abstract class ButtonBase : ControlBase
    {
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
        public virtual void DoClick() => Click?.Invoke(this, new EventArgs());

        /// <summary>
        /// Detects if the SPACE and ENTER keys are pressed and calls the <see cref="Click"/> method.
        /// </summary>
        /// <param name="info"></param>
        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            if (info.IsKeyReleased(Keys.Space) || info.IsKeyReleased(Keys.Enter))
            {
                DoClick();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the left-mouse button is clicked.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            base.OnLeftMouseClicked(state);

            if (isEnabled)
            {
                DoClick();
            }
        }
    }

    /// <summary>
    /// Simple button control with a height of 1.
    /// </summary>
    [DataContract]
    public class Button : ButtonBase
    {
        /// <summary>
        /// Creates an instance of the button control with the specified width and height.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control (default is 1).</param>
        public Button(int width, int height = 1)
            : base(width, height)
        {
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            DetermineState();
            IsDirty = true;
        }
    }
}
