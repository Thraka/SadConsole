using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// Base class for all controls.
/// </summary>
[DataContract]
public abstract class ControlBase
{
    public Type ThemeType { get; set; }

    private Point _position;
    private bool _isEnabled = true;
    private IContainer? _parent;
    [DataMember(Name = "CustomTheme")]
    private Themes.ThemeBase _activeTheme;
    [DataMember(Name = "ThemeColors")]
    private Colors? _themeColors;
    private bool _isDirty;

    /// <summary>
    /// A cached value determined by <see cref="OnMouseEnter(ControlMouseState)"/>. <see langword="true"/> when the mouse is over the bounds defined by <see cref="MouseArea"/> .
    /// </summary>
    protected bool MouseState_IsMouseOver = false;

    /// <summary>
    /// A cached value determined by <see cref="OnMouseEnter(ControlMouseState)"/>. <see langword="true"/> when the mouse entered the control's bounds with the mouse button down.
    /// </summary>
    protected bool MouseState_EnteredWithButtonDown = false;

    /// <summary>
    /// A cached value determined by <see cref="OnMouseIn(ControlMouseState)"/>. <see langword="true"/> when the left mouse button is down.
    /// </summary>
    protected bool MouseState_IsMouseLeftDown;

    /// <summary>
    /// A cached value determined by <see cref="OnMouseIn(ControlMouseState)"/>. <see langword="true"/> when the right mouse button is down.
    /// </summary>
    protected bool MouseState_IsMouseRightDown;

    /// <summary>
    /// Raised when the <see cref="IsDirty"/> property changes.
    /// </summary>
    public event EventHandler<EventArgs>? IsDirtyChanged;

    /// <summary>
    /// <see langword="true"/> to allow this control to respond to keyboard interactions when focused.
    /// </summary>
    [DataMember]
    public bool UseKeyboard { get; set; }

    /// <summary>
    /// <see langword="true"/> to allow this control to respond to mouse interactions.
    /// </summary>
    [DataMember]
    public bool UseMouse { get; set; }

    /// <summary>
    /// <see langword="true"/> to indicate this control can be focused, generally by clicking on the control or tabbing with the keyboard. Otherwise <see langword="false"/>.
    /// </summary>
    [DataMember]
    public bool CanFocus { get; set; }

    /// <summary>
    /// When <see langword="true"/>, indicates that this control can be resized with the <see cref="Resize(int, int)"/> method; otherwise <see langword="false"/>.
    /// </summary>
    [DataMember]
    public bool CanResize { get; protected set; }

    /// <summary>
    /// An alternate font used to render this control.
    /// </summary>
    [DataMember]
    public IFont? AlternateFont { get; set; }

    /// <summary>
    /// The cell data to render the control. Controlled by a theme.
    /// </summary>
    [DataMember]
    public ICellSurface Surface { get; set; }

    /// <summary>
    /// The relative region the of the control used for mouse input.
    /// </summary>
    [DataMember]
    public Rectangle MouseArea { get; set; }

    /// <summary>
    /// When <see langword="true"/>, indicates the mouse button state has only been set with this control and not another; othwerise <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// This property is only set when the mouse enters the control with the buttons pressed. Once the buttons are let go, the mouse is considered clean for this control.
    /// </remarks>
    public bool IsMouseButtonStateClean => !MouseState_EnteredWithButtonDown;

    /// <summary>
    /// The relative position of this control.
    /// </summary>
    [DataMember]
    public Point Position
    {
        get => _position;
        set
        {
            _position = value;
            OnPositionChanged();
        }
    }

    /// <summary>
    /// Gets the position of this control based on the control's <see cref="Position"/> and the position of the <see cref="Parent"/>.
    /// </summary>
    public Point AbsolutePosition => Position + (Parent != null ? Parent.AbsolutePosition : Point.Zero);

    /// <summary>
    /// Indicates whether or not this control is visible.
    /// </summary>
    [DataMember]
    public bool IsVisible { get; set; }

    /// <summary>
    /// A user-definable data object.
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Indicates whether or not this control can be tabbed to.
    /// </summary>
    [DataMember]
    public bool TabStop { get; set; }

    /// <summary>
    /// Sets the tab index of this control.
    /// </summary>
    [DataMember]
    public int TabIndex { get; set; }

    /// <summary>
    /// Indicates whether or not this control is dirty and should be redrawn.
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (value != _isDirty)
            {
                _isDirty = value;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Represents a name to identify a control by.
    /// </summary>
    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether or not this control will become focused when the mouse is clicked.
    /// </summary>
    [DataMember]
    public bool FocusOnClick { get; set; }

    /// <summary>
    /// The width of the control.
    /// </summary>
    public int Width { get; protected set; }

    /// <summary>
    /// The height of the control.
    /// </summary>
    public int Height { get; protected set; }

    /// <summary>
    /// Gets or sets whether or not this control is focused.
    /// </summary>
    public bool IsFocused
    {
        get
        {
            if (Parent == null || Parent.Host == null)
                return false;
            else
                return Parent.Host.FocusedControl == this;
        }
        set
        {
            if (Parent != null && Parent.Host != null)
            {
                if (value)
                {
                    if (Parent.Host.FocusedControl != this)
                        Parent.Host.FocusedControl = this;
                }
                else if (Parent.Host.FocusedControl == this)
                {
                    Parent.Host.TabNextControl();

                    if (Parent.Host.FocusedControl == this)
                        Parent.Host.FocusedControl = null;
                }

                DetermineState();
            }
        }
    }

    /// <summary>
    /// Gets or sets whether or not this control is enabled.
    /// </summary>
    [DataMember]
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            DetermineState();
        }
    }

    /// <summary>
    /// The area this control covers.
    /// </summary>
    public Rectangle Bounds => new(_position.X, _position.Y, Width, Height);

    /// <summary>
    /// Gets or sets the parent console of this control.
    /// </summary>
    public IContainer? Parent
    {
        get => _parent;
        set
        {
            if (_parent == value) return;

            if (_parent == null)
            {
                _parent = value!;

                if (_parent.IsReadOnly == false)
                    _parent.Add(this);
            }
            else
            {
                IContainer temp = _parent;
                _parent = null;
                if (!temp.IsReadOnly)
                    temp.Remove(this);

                _parent = value;

                if (_parent != null && !_parent.IsReadOnly)
                    _parent.Add(this);
            }    

            IsDirty = true;
            OnParentChanged();
        }
    }

    /// <summary>
    /// The custom theme to use with this control. If set to <see langword="null"/>, will use the theme assigned by the <see cref="Parent"/>.
    /// </summary>
    [AllowNull]
    public ThemeBase Theme
    {
        get => _activeTheme;
        set
        {
            if (value != _activeTheme)
            {
                if (value == null)
                {
                    IsCustomTheme = false;
                    _activeTheme = Library.Default.GetControlTheme(GetType());
                    if (_activeTheme == null) throw new NullReferenceException($"Theme unavailable for {GetType().FullName}. Register a theme with SadConsole.Library.Default.SetControlTheme");
                }
                else
                {
                    _activeTheme = value;
                    IsCustomTheme = true;
                }

                OnThemeChanged();
                _activeTheme.Attached(this);
                DetermineState();
                IsDirty = true;
            }
        }
    }

    /// <summary>
    /// When <see langword="true"/>, indicates the control has a custom theme assigned to it; othwerise <see langword="false"/>.
    /// </summary>
    public bool IsCustomTheme { get; protected set; }

    /// <summary>
    /// The state of the control.
    /// </summary>
    public ControlStates State { get; protected set; }

    /// <summary>
    /// Raised when the mouse enters this control.
    /// </summary>
    public event EventHandler<ControlMouseState>? MouseEnter;

    /// <summary>
    /// Raised when the mouse exits this control.
    /// </summary>
    public event EventHandler<ControlMouseState>? MouseExit;

    /// <summary>
    /// Raised when the mouse is moved over this control.
    /// </summary>
    public event EventHandler<ControlMouseState>? MouseMove;

    /// <summary>
    /// Raised when a mouse button is clicked while the mouse is over this control.
    /// </summary>
    public event EventHandler<ControlMouseState>? MouseButtonClicked;

    #region Constructors
    /// <inheritdoc />
    /// <summary>
    /// Creates a control.
    /// </summary>
    protected ControlBase(int width, int height)
    {
        Width = width;
        Height = height;
        CanResize = true;
        IsDirty = true;
        TabStop = true;
        IsVisible = true;
        FocusOnClick = true;
        CanFocus = true;
        _position = new Point();
        UseMouse = true;
        UseKeyboard = true;
        MouseArea = new Rectangle(0, 0, width, height);

        // Set theme
        Surface = null!;
        _activeTheme = Library.Default.GetControlTheme(GetType());
        if (_activeTheme == null) throw new NullReferenceException($"Theme unavalable for {GetType().FullName}. Register a theme with SadConsole.Library.Default.SetControlTheme");
        _activeTheme.Attached(this);
        DetermineState();
    }
    #endregion

    /// <summary>
    /// Called when the control loses focus. Calls DetermineAppearance.
    /// </summary>
    public virtual void FocusLost() => DetermineState();

    /// <summary>
    /// Called when the control is focused. Calls DetermineAppearance.
    /// </summary>
    public virtual void Focused() => DetermineState();


    /// <summary>
    /// Called when the <see cref="Theme"/> changes.
    /// </summary>
    protected virtual void OnThemeChanged() { }

    #region Input
    /// <summary>
    /// Called when the keyboard is used on this control.
    /// </summary>
    /// <param name="state">The state of the keyboard.</param>
    public virtual bool ProcessKeyboard(Keyboard state) => false;

    /// <summary>
    /// Checks if the mouse is the control and calls the appropriate mouse methods.
    /// </summary>
    /// <param name="state">Mouse information.</param>
    /// <returns>True when the control is enabled, set to use the mouse and the mouse is over it, otherwise false.</returns>
    public virtual bool ProcessMouse(Input.MouseScreenObjectState state)
    {
        if (IsEnabled && UseMouse)
        {
            var newState = new ControlMouseState(this, state);

            if (newState.IsMouseOver)
            {
                if (MouseState_IsMouseOver != true)
                {
                    MouseState_IsMouseOver = true;
                    OnMouseEnter(newState);
                }

                bool preventClick = MouseState_EnteredWithButtonDown;
                OnMouseIn(newState);

                if (!preventClick && state.Mouse.LeftClicked)
                    OnLeftMouseClicked(newState);

                if (!preventClick && state.Mouse.RightClicked)
                    OnRightMouseClicked(newState);

                return true;
            }
            else
            {
                if (MouseState_IsMouseOver)
                {
                    MouseState_IsMouseOver = false;
                    OnMouseExit(newState);
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Called to trigger the state of losing mouse focus.
    /// </summary>
    /// <param name="state">The mouse state.</param>
    public void LostMouse(MouseScreenObjectState state)
    {
        if (MouseState_IsMouseOver)
            OnMouseExit(new ControlMouseState(this, state));
    }
    #endregion

    /// <summary>
    /// Called when the parent property is changed.
    /// </summary>
    protected virtual void OnParentChanged() { }

    /// <summary>
    /// Called when the control changes position.
    /// </summary>
    protected virtual void OnPositionChanged() { }

    /// <summary>
    /// Places this control relative to another, taking into account the bounds of the control.
    /// </summary>
    /// <param name="control">The other control to place this one relative to.</param>
    /// <param name="direction">The direction this control should be placed.</param>
    /// <param name="padding">Additional space between the controls after placement.</param>
    /// <remarks>If this control hasn't been added to the parent of <paramref name="control"/>, it will be added.</remarks>
    public void PlaceRelativeTo(ControlBase control, SadRogue.Primitives.Direction.Types direction, int padding = 1)
    {
        if (control.Parent != null && Parent != control.Parent)
            Parent = control.Parent;

        Point calculatedDirection = (0, 0) + (Direction)direction;
        Position = control.Position + (calculatedDirection.X * padding, calculatedDirection.Y * padding);

        switch (direction)
        {
            case Direction.Types.None:
                Position = control.Position;
                break;
            case Direction.Types.Up:
                Position = (control.Position.X, control.Position.Y - Height - padding);
                break;
            case Direction.Types.UpRight:
                Position = (control.Bounds.MaxExtentX + 1 + padding, control.Position.Y - Height - padding);
                break;
            case Direction.Types.Right:
                Position = (control.Bounds.MaxExtentX + 1 + padding, control.Position.Y);
                break;
            case Direction.Types.DownRight:
                Position = (control.Bounds.MaxExtentX + 1 + padding, control.Bounds.MaxExtentY + 1 + padding);
                break;
            case Direction.Types.Down:
                Position = (control.Position.X, control.Bounds.MaxExtentY + 1 + padding);
                break;
            case Direction.Types.DownLeft:
                Position = (control.Position.X - Width - padding, control.Bounds.MaxExtentY + 1 + padding);
                break;
            case Direction.Types.Left:
                Position = (control.Position.X - Width - padding, control.Position.Y);
                break;
            case Direction.Types.UpLeft:
                Position = (control.Position.X - Width - padding, control.Position.Y - Height - padding);
                break;
        }
    }

    /// <summary>
    /// Sets the appropriate theme for the control based on the current state of the control.
    /// </summary>
    /// <remarks>Called by the control as the mouse state changes, like when the mouse is clicked on top of the control or leaves the area of the control. This method is implemented by each derived control.</remarks>
    public virtual void DetermineState()
    {
        ControlStates oldState = State;

        State = !_isEnabled
            ? (ControlStates)Helpers.SetFlag((int)State, (int)ControlStates.Disabled)
            : (ControlStates)Helpers.UnsetFlag((int)State, (int)ControlStates.Disabled);

        State = MouseState_IsMouseOver
            ? (ControlStates)Helpers.SetFlag((int)State, (int)ControlStates.MouseOver)
            : (ControlStates)Helpers.UnsetFlag((int)State, (int)ControlStates.MouseOver);

        State = IsFocused && Parent?.Host?.ParentConsole != null && Parent.Host.ParentConsole.IsFocused
            ? (ControlStates)Helpers.SetFlag((int)State, (int)ControlStates.Focused)
            : (ControlStates)Helpers.UnsetFlag((int)State, (int)ControlStates.Focused);

        State = MouseState_IsMouseLeftDown && IsMouseButtonStateClean
            ? (ControlStates)Helpers.SetFlag((int)State, (int)ControlStates.MouseLeftButtonDown)
            : (ControlStates)Helpers.UnsetFlag((int)State, (int)ControlStates.MouseLeftButtonDown);

        State = MouseState_IsMouseRightDown && IsMouseButtonStateClean
            ? (ControlStates)Helpers.SetFlag((int)State, (int)ControlStates.MouseRightButtonDown)
            : (ControlStates)Helpers.UnsetFlag((int)State, (int)ControlStates.MouseRightButtonDown);

        if (oldState != State)
        {
            OnStateChanged(oldState, State);
            IsDirty = true;
        }
    }

    /// <summary>
    /// Called when the <see cref="State"/> changes. Sets the <see cref="IsDirty"/> to true.
    /// </summary>
    /// <param name="oldState">The original state.</param>
    /// <param name="newState">The new state.</param>
    protected virtual void OnStateChanged(ControlStates oldState, ControlStates newState) => IsDirty = true;

    /// <summary>
    /// Returns the colors assigned to this control, the parent, or the library default.
    /// </summary>
    /// <returns>The found colors.</returns>
    public Colors FindThemeColors() =>
        _themeColors ?? _parent?.Host?.ThemeColors ?? Library.Default.Colors;

    /// <summary>
    /// Sets the theme colors used by this control. When <see langword="null"/>, indicates this control should read the theme colors from the parent.
    /// </summary>
    /// <param name="value">The colors to use with this control.</param>
    public void SetThemeColors(Colors? value)
    {
        _themeColors = value;
        IsDirty = true;
    }

    /// <summary>
    /// When <see langword="true"/>, indicates the control has custom theme colors assigned to it; othwerise <see langword="false"/>.
    /// </summary>
    /// <returns></returns>
    public bool HasThemeColors() =>
        _themeColors != null;

    /// <summary>
    /// Resizes the control if the <see cref="CanResize"/> property is <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// When overriding this method, to perhaps restrict the requested size, make sure to do the following:
    ///
    /// - Check if the <see cref="CanResize"/> property allows resizing. If it doesn't, throw an <see cref="InvalidOperationException"/>.
    /// - The <see cref="Width"/> and <see cref="Height"/> variables of the control are set to the values provided.
    /// - The <see cref="MouseArea"/> property is set to the same width and height as values provided. Generally the theme will adjust this if needed, but it might ignore this so you'll need to set it.
    /// - The <see cref="Theme"/>, if set, is reattached by calling <see cref="ThemeBase.Attached(ControlBase)"/>.
    /// - Set <see cref="IsDirty"/> to <see langword="true"/>.
    /// </remarks>
    /// <param name="width">The desired width of the control.</param>
    /// <param name="height">The desired height of the control.</param>
    public virtual void Resize(int width, int height)
    {
        if (!CanResize) throw new InvalidOperationException("This control can't resize.");

        Width = width;
        Height = height;
        MouseArea = new Rectangle(0, 0, width, height);
        Theme?.Attached(this);
        IsDirty = true;
    }

    /// <summary>
    /// Called when the mouse first enters the control. Raises the MouseEnter event and calls the <see cref="DetermineState"/> method.
    /// </summary>
    /// <param name="state">The current mouse data</param>
    protected virtual void OnMouseEnter(ControlMouseState state)
    {
        MouseState_IsMouseOver = true;

        if (state.OriginalMouseState.Mouse.LeftButtonDown || state.OriginalMouseState.Mouse.RightButtonDown)
            MouseState_EnteredWithButtonDown = true;

        MouseEnter?.Invoke(this, state);

        DetermineState();
    }

    /// <summary>
    /// Called when the mouse exits the area of the control. Raises the MouseExit event and calls the <see cref="DetermineState"/> method.
    /// </summary>
    /// <param name="state">The current mouse data</param>
    protected virtual void OnMouseExit(ControlMouseState state)
    {
        MouseState_IsMouseLeftDown = false;
        MouseState_IsMouseRightDown = false;
        MouseState_IsMouseOver = false;
        MouseState_EnteredWithButtonDown = false;
        MouseExit?.Invoke(this, state);

        DetermineState();
    }

    /// <summary>
    /// Called as the mouse moves around the control area. Raises the MouseMove event and calls the <see cref="DetermineState"/> method.
    /// </summary>
    /// <param name="state">The current mouse data</param>
    protected virtual void OnMouseIn(ControlMouseState state)
    {
        MouseMove?.Invoke(this, state);

        MouseState_IsMouseLeftDown = state.OriginalMouseState.Mouse.LeftButtonDown;
        MouseState_IsMouseRightDown = state.OriginalMouseState.Mouse.RightButtonDown;

        if (MouseState_EnteredWithButtonDown && !MouseState_IsMouseLeftDown && !MouseState_IsMouseRightDown)
            MouseState_EnteredWithButtonDown = false;

        DetermineState();
    }

    /// <summary>
    /// Called when the left mouse button is clicked. Raises the MouseButtonClicked event and calls the <see cref="DetermineState"/> method.
    /// </summary>
    /// <param name="state">The current mouse data</param>
    protected virtual void OnLeftMouseClicked(ControlMouseState state)
    {
        MouseButtonClicked?.Invoke(this, state);

        if (FocusOnClick)
            IsFocused = true;

        DetermineState();
    }

    /// <summary>
    /// Called when the right mouse button is clicked. Raises the MouseButtonClicked event and calls the <see cref="DetermineState"/> method.
    /// </summary>
    /// <param name="state">The current mouse data</param>
    protected virtual void OnRightMouseClicked(ControlMouseState state)
    {
        MouseButtonClicked?.Invoke(this, state);

        DetermineState();
    }

    /// <summary>
    /// Update the control appearance based on <see cref="DetermineState"/> and <see cref="IsDirty"/>.
    /// </summary>
    public virtual void Update(TimeSpan time) =>
        Theme.UpdateAndDraw(this, time);

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        IsDirty = true;
    }

    /// <summary>
    /// Mouse state based on a specific control.
    /// </summary>
    public class ControlMouseState
    {
        /// <summary>
        /// The control this mouse state is associated with.
        /// </summary>
        public ControlBase Control { get; set; }

        /// <summary>
        /// The relative position of the mouse to the control.
        /// </summary>
        public Point MousePosition { get; set; }

        /// <summary>
        /// The original mouse state used to generate the event.
        /// </summary>
        public MouseScreenObjectState OriginalMouseState { get; set; }

        /// <summary>
        /// When <see langword="true"/>, indicates the mouse is over the <see cref="Control"/>; othwerise <see langword="false"/>.
        /// </summary>
        public bool IsMouseOver { get; set; }

        /// <summary>
        /// Creates an instance of the mouse control state class.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="mousePosition">The position of the mouse relative to the control.</param>
        /// <param name="originalMouseState">The original mouse state sent to the control.</param>
        public ControlMouseState(ControlBase control, Point mousePosition, MouseScreenObjectState originalMouseState)
        {
            Control = control;
            MousePosition = mousePosition;
            OriginalMouseState = originalMouseState;
            IsMouseOver = originalMouseState.IsOnScreenObject && control.MouseArea.Contains(MousePosition);
        }


        /// <summary>
        /// Creates an instance of the mouse control state class and infers the <see cref="MousePosition"/> from the control and state.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="originalMouseState">The original mouse state sent to the control.</param>
        public ControlMouseState(ControlBase control, MouseScreenObjectState originalMouseState) : this(control, originalMouseState.CellPosition - control.AbsolutePosition, originalMouseState)
        {

        }
    }
}
