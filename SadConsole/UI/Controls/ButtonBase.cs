using System;
using System.Runtime.Serialization;
using SadConsole.Input;

namespace SadConsole.UI.Controls;

/// <summary>
/// Base class for creating a button type control.
/// </summary>
[DataContract]
public abstract class ButtonBase : ControlBase
{
    private bool _mouseDownForClick = false;
    private string _text = string.Empty;
    private bool _autoSize = false;

    /// <summary>
    /// Raised when the button is clicked.
    /// </summary>
    public event EventHandler? Click;

    /// <summary>
    /// The alignment of the <see cref="_text"/>.
    /// </summary>
    protected HorizontalAlignment _textAlignment = HorizontalAlignment.Center;

    /// <summary>
    /// The text displayed on the control.
    /// </summary>
    [DataMember]
    public string Text
    {
        get => _text;
        set { _text = value ?? string.Empty; IsDirty = true; }
    }

    /// <summary>
    /// When <see langword="true"/>, automatically resizes the surface to fit the <see cref="Text"/>.
    /// </summary>
    [DataMember]
    public bool AutoSize
    {
        get => _autoSize;
        set { _autoSize = value; CanResize = !value; IsDirty = true; }
    }

    /// <summary>
    /// The alignment of the text, left, center, or right.
    /// </summary>
    [DataMember]
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
    protected ButtonBase(int width, int height) : base(width, height) { }

    /// <summary>
    /// Creates a new button control with <see cref="AutoSize"/> set to <see langword="true"/>.
    /// </summary>
    protected ButtonBase(): base(1, 1)
    {
        AutoSize = true;
    }

    /// <summary>
    /// Raises the <see cref="Click"/> event.
    /// </summary>
    protected virtual void OnClick() =>
        Click?.Invoke(this, new EventArgs());

    /// <summary>
    /// Simulates a mouse click on the button. Optionally, focuses the button prior to simulating the click.
    /// </summary>
    /// <param name="focus">When <see langword="true"/>, focuses the button before clicking.</param>
    public void InvokeClick(bool focus = false)
    {
        if (focus) IsFocused = true;
        OnClick();
    }

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

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        DetermineState();
        IsDirty = true;
    }
}
