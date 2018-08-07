using Microsoft.Xna.Framework.Input;
using SadConsole.Surfaces;
using SadConsole.Themes;
using System;
using System.Runtime.Serialization;

namespace SadConsole.Controls
{
    /// <summary>
    /// Base class for creating a button type control.
    /// </summary>
    /// <typeparam name="TTheme"></typeparam>
    [DataContract]
    public abstract class ButtonBase: ControlBase
    {
        /// <summary>
        /// Raised when the button is clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// True when the mouse is down.
        /// </summary>
        protected bool isMouseDown;

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
        /// Selected part of the theme based on the state of the control.
        /// </summary>
        public Cell StateAppearance { get; protected set; }

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
        public ButtonBase(int width, int height): base(width, height) { }

        /// <summary>
        /// Raises the <see cref="Click"/> event.
        /// </summary>
        public virtual void DoClick()
        {
            Click?.Invoke(this, new EventArgs());
        }
        
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
        /// Called when the mouse is in the control area.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        protected override void OnMouseIn(Input.MouseConsoleState state)
        {
            isMouseDown = state.Mouse.LeftButtonDown;

            base.OnMouseIn(state);
        }

        /// <summary>
        /// Called when the mouse leaves the control area.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        protected override void OnMouseExit(Input.MouseConsoleState state)
        {
            isMouseDown = false;

            base.OnMouseExit(state);
        }

        /// <summary>
        /// Called when the left-mouse button is clicked.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            base.OnLeftMouseClicked(state);

            if (isEnabled)
                DoClick();
        }
    }

    /// <summary>
    /// Simple button control with a height of 1.
    /// </summary>
    [DataContract]
    public class Button: ButtonBase
    {
        private ButtonTheme theme;

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual ButtonTheme Theme
        {
            get
            {
                if (theme == null)
                    return Library.Default.ButtonTheme;
                else
                    return theme;
            }
            set
            {
                theme = value;
                DetermineAppearance();
                IsDirty = true;
            }
        }


        /// <summary>
        /// Creates an instance of the button control with the specified width.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public Button(int width, int height)
            : base(width, height)
        {
            DetermineAppearance();
        }

        /// <summary>
        /// Sets the appearance of the control depending on the current state of the control.
        /// </summary>
        public override void DetermineAppearance()
        {
            //TODO does this belong in theme?
            Cell currentappearance = StateAppearance;

            if (!isEnabled)
                StateAppearance = Theme.Disabled;

            else if (!isMouseDown && isMouseOver)
                StateAppearance = Theme.MouseOver;

            else if (!isMouseDown && !isMouseOver && IsFocused && Global.FocusedConsoles.Console == parent)
                StateAppearance = Theme.Focused;

            else if (isMouseDown && isMouseOver)
                StateAppearance = Theme.Selected;

            else
                StateAppearance = Theme.Normal;

            if (currentappearance != StateAppearance)
                IsDirty = true;
        }

        public override void Update(SurfaceBase hostSurface)
        {
            if (IsDirty)
                Theme.Render(this, hostSurface);
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            DetermineAppearance();
            IsDirty = true;
        }
    }
}
