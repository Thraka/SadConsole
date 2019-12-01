using System;
using SadConsole.Input;

namespace SadConsole
{
    public partial class ScreenSurface
    {

        /// <inheritdoc/>
        public event EventHandler<MouseScreenObjectState> MouseButtonClicked;

        /// <inheritdoc/>
        public event EventHandler<MouseScreenObjectState> MouseMove;

        /// <inheritdoc/>
        public event EventHandler<MouseScreenObjectState> MouseExit;

        /// <inheritdoc/>
        public event EventHandler<MouseScreenObjectState> MouseEnter;

        /// <summary>
        /// Indicates that the mouse is currently over this console.
        /// </summary>
        protected bool IsMouseOver;

        /// <inheritdoc/>
        public FocusBehavior FocusedMode { get; set; }

        /// <inheritdoc/>
        public bool IsExclusiveMouse { get; set; }

        /// <inheritdoc/>
        public bool MoveToFrontOnMouseClick { get; set; }

        /// <inheritdoc/>
        public bool FocusOnMouseClick { get; set; }

        /// <inheritdoc/>
        public bool IsFocused
        {
            get => Global.FocusedScreenObjects.ScreenObject == this;
            set
            {
                if (Global.FocusedScreenObjects.ScreenObject != null)
                {
                    if (value && Global.FocusedScreenObjects.ScreenObject != this)
                    {
                        if (FocusedMode == FocusBehavior.Push)
                            Global.FocusedScreenObjects.Push(this);
                        else
                            Global.FocusedScreenObjects.Set(this);
                    }
                    else if (!value && Global.FocusedScreenObjects.ScreenObject == this)
                        Global.FocusedScreenObjects.Pop(this);
                }
                else
                {
                    if (value)
                    {
                        if (FocusedMode == FocusBehavior.Push)
                            Global.FocusedScreenObjects.Push(this);
                        else
                            Global.FocusedScreenObjects.Set(this);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="MouseEnter"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseEnter(MouseScreenObjectState state) => MouseEnter?.Invoke(this, state);

        /// <summary>
        /// Raises the <see cref="MouseExit"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseExit(MouseScreenObjectState state)
        {
            // Force mouse off just in case
            IsMouseOver = false;

            MouseExit?.Invoke(this, state);
        }

        /// <summary>
        /// Raises the <see cref="MouseMove"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseMove(MouseScreenObjectState state)
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

            MouseMove?.Invoke(this, state);
        }

        /// <summary>
        /// Raises the <see cref="MouseButtonClicked"/> event. Possibly moves the console to the top of it's parent's children collection.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnMouseLeftClicked(MouseScreenObjectState state) => MouseButtonClicked?.Invoke(this, state);

        /// <summary>
        /// Raises the <see cref="MouseButtonClicked"/> event.
        /// </summary>
        /// <param name="state">Current mouse state in relation to this console.</param>
        protected virtual void OnRightMouseClicked(MouseScreenObjectState state) => MouseButtonClicked?.Invoke(this, state);

        /// <inheritdoc/>
        public void LostMouse(MouseScreenObjectState state)
        {
            if (IsMouseOver)
            {
                OnMouseExit(state);
            }
        }

        /// <inheritdoc/>
        public virtual bool ProcessMouse(MouseScreenObjectState state)
        {
            if (!IsVisible)
            {
                return false;
            }

            foreach (SadConsole.Components.IComponent component in ComponentsMouse.ToArray())
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

            if (state.IsOnScreenObject)
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
        /// <param name="keyboard">Keyboard information.</param>
        /// <returns>True when the keyboard had data and this console did something with it.</returns>
        public virtual bool ProcessKeyboard(Keyboard keyboard)
        {
            if (!UseKeyboard) return false;

            foreach (Components.IComponent component in ComponentsKeyboard.ToArray())
            {
                component.ProcessKeyboard(this, keyboard, out bool isHandled);

                if (isHandled)
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public virtual void OnFocusLost() { }

        /// <inheritdoc/>
        public virtual void OnFocused() { }
    }
}
