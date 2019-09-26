namespace SadConsole
{
    using System;
    using SadConsole.Input;

    public partial class Console
    {
        /// <summary>
        /// Raised when the a mouse button is clicked on this console.
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
        protected bool IsMouseOver;

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
        public bool UseKeyboard { get; set; } = Settings.DefaultConsoleUseKeyboard;

        /// <summary>
        /// Allows this console to accept mouse input.
        /// </summary>
        public bool UseMouse { get; set; } = true;

        /// <summary>
        /// Raises the <see cref="MouseEnter"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseEnter(MouseConsoleState state) => MouseEnter?.Invoke(this, new MouseEventArgs(state));

        /// <summary>
        /// Raises the <see cref="MouseExit"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseExit(MouseConsoleState state)
        {
            // Force mouse off just in case
            IsMouseOver = false;

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
                {
                    Parent.Children.MoveToTop(this);
                }

                if (FocusOnMouseClick && !IsFocused)
                {
                    IsFocused = true;
                }
            }

            MouseMove?.Invoke(this, new MouseEventArgs(state));
        }

        /// <summary>
        /// Raises the <see cref="MouseButtonClicked"/> event. Possibly moves the console to the top of it's parent's children collection.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseLeftClicked(MouseConsoleState state) => MouseButtonClicked?.Invoke(this, new MouseEventArgs(state));

        /// <summary>
        /// Raises the <see cref="MouseButtonClicked"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnRightMouseClicked(MouseConsoleState state) => MouseButtonClicked?.Invoke(this, new MouseEventArgs(state));

        /// <summary>
        /// If the mouse is not over the console, causes the protected <see cref="OnMouseExit"/> method to run which raises the <see cref="MouseExit"/> event.
        /// </summary>
        /// <param name="state"></param>
        public void LostMouse(MouseConsoleState state)
        {
            if (IsMouseOver)
            {
                OnMouseExit(state);
            }
        }

        /// <summary>
        /// Processes the mouse.
        /// </summary>
        /// <param name="state">The mouse state related to this console.</param>
        /// <returns>True when the mouse is over this console and processing should stop.</returns>
        public virtual bool ProcessMouse(MouseConsoleState state)
        {
            if (!IsVisible)
            {
                return false;
            }

            foreach (Components.IConsoleComponent component in ComponentsMouse.ToArray())
            {
                component.ProcessMouse(this, state, out bool isHandled);

                if (isHandled)
                {
                    return true;
                }
            }

            if (!UseMouse)
            {
                return false;
            }

            if (state.IsOnConsole)
            {
                if (IsMouseOver != true)
                {
                    IsMouseOver = true;
                    OnMouseEnter(state);
                }

                OnMouseMove(state);

                if (state.Mouse.LeftClicked)
                {
                    OnMouseLeftClicked(state);
                }

                if (state.Mouse.RightClicked)
                {
                    OnRightMouseClicked(state);
                }

                return true;
            }

            if (IsMouseOver)
            {
                IsMouseOver = false;
                OnMouseExit(state);
            }

            return false;
        }

        /// <summary>
        /// Called by the engine to process the keyboard.
        /// </summary>
        /// <param name="info">Keyboard information.</param>
        /// <returns>True when the keyboard had data and this console did something with it.</returns>
        public virtual bool ProcessKeyboard(Keyboard info)
        {
            foreach (Components.IConsoleComponent component in ComponentsKeyboard.ToArray())
            {
                component.ProcessKeyboard(this, info, out bool isHandled);

                if (isHandled)
                {
                    return true;
                }
            }

            if (!UseKeyboard)
            {
                return false;
            }

            return !IsCursorDisabled && Cursor.IsEnabled && Cursor.ProcessKeyboard(info);
        }

        /// <summary>
        /// How the console handles becoming focused and added to the <see cref="Global.FocusedConsoles"/> collection.
        /// </summary>
        public enum ActiveBehavior
        {
            /// <summary>
            /// Becomes the only active input object when focused.
            /// </summary>
            Set,

            /// <summary>
            /// Pushes to the top of the stack when it becomes the active input object.
            /// </summary>
            Push
        }
    }
}
