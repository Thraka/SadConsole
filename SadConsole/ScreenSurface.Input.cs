﻿using System;
using System.Runtime.Serialization;
using SadConsole.Input;

namespace SadConsole;

public partial class ScreenSurface
{
    /// <summary>
    /// A cached value determined by <see cref="OnMouseEnter(MouseScreenObjectState)"/>. <see langword="true"/> when the mouse entered the surface bounds with the mouse button down.
    /// </summary>
    protected internal bool MouseState_EnteredWithButtonDown = false;

    /// <inheritdoc/>
    public event EventHandler<MouseScreenObjectState>? MouseButtonClicked;

    /// <inheritdoc/>
    public event EventHandler<MouseScreenObjectState>? MouseMove;

    /// <inheritdoc/>
    public event EventHandler<MouseScreenObjectState>? MouseExit;

    /// <inheritdoc/>
    public event EventHandler<MouseScreenObjectState>? MouseEnter;

    /// <summary>
    /// Indicates that the mouse is currently over this console.
    /// </summary>
    protected bool IsMouseOver;

    /// <inheritdoc/>
    [DataMember]
    public bool MoveToFrontOnMouseClick { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public bool FocusOnMouseClick { get; set; }

    /// <summary>
    /// Raises the <see cref="MouseEnter"/> event.
    /// </summary>
    /// <param name="state">Current mouse state in relation to this console.</param>
    protected virtual void OnMouseEnter(MouseScreenObjectState state)
    {
        if (state.Mouse.LeftButtonDown)
            MouseState_EnteredWithButtonDown = true;

        MouseEnter?.Invoke(this, state);
    }

    /// <summary>
    /// Raises the <see cref="MouseExit"/> event.
    /// </summary>
    /// <param name="state">Current mouse state in relation to this console.</param>
    protected virtual void OnMouseExit(MouseScreenObjectState state)
    {
        // Force mouse off just in case
        IsMouseOver = false;
        MouseState_EnteredWithButtonDown = false;

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
            if (MoveToFrontOnMouseClick && !MouseState_EnteredWithButtonDown && Parent != null && Parent.Children.IndexOf(this) != Parent.Children.Count - 1)
            {
                Parent.Children.MoveToTop(this);
            }

            if (FocusOnMouseClick && !IsFocused)
            {
                IsFocused = true;
            }
        }
        else
            MouseState_EnteredWithButtonDown = false;

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
    public override void LostMouse(MouseScreenObjectState state)
    {
        if (IsMouseOver)
            OnMouseExit(state);
    }

    /// <inheritdoc/>
    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (!IsVisible) return false;

        int count = ComponentsMouse.Count;
        for (int i = 0; i < count; i++)
        {
            ComponentsMouse[i].ProcessMouse(this, state, out bool isHandled);

            if (isHandled)
                return true;
        }

        if (!UseMouse) return false;

        if (state.IsOnScreenObject)
        {
            if (IsMouseOver != true)
            {
                IsMouseOver = true;
                OnMouseEnter(state);
            }

            OnMouseMove(state);

            if (state.Mouse.LeftClicked)
                OnMouseLeftClicked(state);

            if (state.Mouse.RightClicked)
                OnRightMouseClicked(state);

            return true;
        }

        if (IsMouseOver)
        {
            IsMouseOver = false;
            OnMouseExit(state);
        }

        return false;
    }

}
