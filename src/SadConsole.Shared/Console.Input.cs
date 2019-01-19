using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using SadConsole.Input;

namespace SadConsole
{
    public partial class Console
    {
        /// <summary>
        /// Raised when the a mosue button is clicked on this console.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseButtonClicked;

        /// <summary>
        /// Raised when the mouse moves around the this console.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseMove;

        /// <summary>
        /// Raised when the mouse exits this console.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseExit;

        /// <summary>
        /// Raised when the mouse enters this console.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseEnter;

        /// <summary>
        /// Indicates that the mouse is currently over this console.
        /// </summary>
        protected bool isMouseOver = false;

        /// <summary>
        /// When true, this console will move to the front of its parent console when the mouse is clicked.
        /// </summary>
        public bool MoveToFrontOnMouseClick { get; set; }

        /// <summary>
        /// When true, this console will set <see cref="IsFocused"/> to true when the mouse is clicked.
        /// </summary>
        public bool FocusOnMouseClick { get; set; }

        /// <summary>
        /// Allows this console to accept keyboard input.
        /// </summary>
        public bool UseKeyboard { get; set; } = true;

        /// <summary>
        /// Allows this console to accept mouse input.
        /// </summary>
        public bool UseMouse { get; set; } = true;

        /// <summary>
        /// An alternative method handler for handling the mouse logic.
        /// </summary>
        public Func<Console, SadConsole.Input.MouseConsoleState, bool> MouseHandler { get; set; }

        /// <summary>
        /// An alternative method handler for handling the keyboard logic.
        /// </summary>
        public Func<Console, Input.Keyboard, bool> KeyboardHandler { get; set; }

        /// <summary>
        /// Raises the <see cref="MouseEnter"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseEnter(MouseConsoleState state)
        {
            MouseEnter?.Invoke(this, new MouseEventArgs(state));
        }

        /// <summary>
        /// Raises the <see cref="MouseExit"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseExit(MouseConsoleState state)
        {
            // Force mouse off just incase
            isMouseOver = false;

            MouseExit?.Invoke(this, new MouseEventArgs(state));
        }

        /// <summary>
        /// Raises the <see cref="MouseMove"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseMove(MouseConsoleState state)
        {
            if (state.Mouse.LeftButtonDown)
            {
                if (MoveToFrontOnMouseClick && Parent != null && Parent.Children.IndexOf(this) != Parent.Children.Count - 1)
                    Parent.Children.MoveToTop(this);

                if (FocusOnMouseClick && !IsFocused)
                    IsFocused = true;
            }

            MouseMove?.Invoke(this, new MouseEventArgs(state));
        }

        /// <summary>
        /// Raises the <see cref="MouseButtonClicked"/> event. Possibly moves the console to the top of it's parent's children collection.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseLeftClicked(MouseConsoleState state)
        {
            MouseButtonClicked?.Invoke(this, new MouseEventArgs(state));
        }

        /// <summary>
        /// Raises the <see cref="MouseButtonClicked"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnRightMouseClicked(MouseConsoleState state)
        {
            MouseButtonClicked?.Invoke(this, new MouseEventArgs(state));
        }

        /// <summary>
        /// If the mouse is not over the console, causes the protected <see cref="OnMouseExit"/> method to run which raises the <see cref="MouseExit"/> event.
        /// </summary>
        /// <param name="state"></param>
        public void LostMouse(MouseConsoleState state)
        {
            if (isMouseOver)
                OnMouseExit(state);
        }

        /// <summary>
        /// Processes the mouse.
        /// </summary>
        /// <param name="state">The mouse state related to this console.</param>
        /// <returns>True when the mouse is over this console and processing should stop.</returns>
        public virtual bool ProcessMouse(MouseConsoleState state)
        {
            return MouseHandler?.Invoke(this, state) ?? ProcessMouseNonHandler(state);
        }

        /// <summary>
        /// Processing the mouse ignoring the attached <see cref="MouseHandler"/>.
        /// </summary>
        /// <param name="state">The mouse state related to this console.</param>
        /// <returns>True when the mouse is over this console and processing should stop.</returns>
        public bool ProcessMouseNonHandler(MouseConsoleState state)
        {
            var handlerResult = MouseHandler?.Invoke(this, state) ?? false;

            if (handlerResult || !IsVisible || !UseMouse) return false;

            if (state.IsOnConsole)
            {
                if (isMouseOver != true)
                {
                    isMouseOver = true;
                    OnMouseEnter(state);
                }

                OnMouseMove(state);

                if (state.Mouse.LeftClicked)
                    OnMouseLeftClicked(state);

                if (state.Mouse.RightClicked)
                    OnRightMouseClicked(state);

                return true;
            }

            if (isMouseOver)
            {
                isMouseOver = false;
                OnMouseExit(state);
            }

            return false;
        }

        /// <summary>
    /// Called by the engine to process the keyboard. If the <see cref="KeyboardHandler"/> has been set, that will be called instead of this method.
    /// </summary>
    /// <param name="info">Keyboard information.</param>
    /// <returns>True when the keyboard had data and this console did something with it.</returns>
    public virtual bool ProcessKeyboard(Input.Keyboard info)
        {
            var handlerResult = KeyboardHandler?.Invoke(this, info) ?? false;

            if (!handlerResult && this.UseKeyboard)
            {
                return Cursor.ProcessKeyboard(info);
            }

            return handlerResult;
        }

    }
}
