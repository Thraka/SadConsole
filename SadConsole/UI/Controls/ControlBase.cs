using SadConsole.Input;
using System.Runtime.Serialization;
using System;
using SadRogue.Primitives;
using SadConsole.UI.Themes;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// Base class for all controls.
    /// </summary>
    [DataContract]
    public abstract class ControlBase
    {
        protected Point _position;
        protected bool _isMouseOver = false;
        protected bool _mouseEnteredWithButtonDown = false;
        protected bool _isEnabled = true;
        protected IContainer _parent;
        protected ControlStates _state;

        protected Themes.ThemeBase ActiveTheme;
        protected bool IsCustomTheme;
        protected Colors _themeColors;

        /// <summary>
        /// <see langword="true"/> when the left mouse button is down.
        /// </summary>
        protected bool _isMouseLeftDown;

        /// <summary>
        /// <see langword="true"/> when the right mouse button is down.
        /// </summary>
        protected bool _isMouseRightDown;

        private bool _isDirty;

        public event EventHandler<EventArgs> IsDirtyChanged;

        [DataMember]
        public bool UseKeyboard { get; set; }

        [DataMember]
        public bool UseMouse { get; set; }

        [DataMember]
        public bool CanFocus { get; set; }

        [DataMember]
        public bool ExclusiveFocus { get; set; }

        [DataMember]
        public Font AlternateFont { get; set; }

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
        /// When <see langword="true"/>, indicates the mouse button state can be relied on; othwerise <see langword="false"/>.
        /// </summary>
        /// <remarks>
        /// This property is only set when the mouse enters the control with the buttons pressed. Once the buttons are let go, the mouse is considered clean for this control.
        /// </remarks>
        public bool IsMouseButtonStateClean => !_mouseEnteredWithButtonDown;

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
        public Point AbsolutePosition => Position + (Parent != null ? Parent.AbsolutePosition : new Point(0, 0));

        /// <summary>
        /// Indicates whether or not this control is visible.
        /// </summary>
        [DataMember]
        public bool IsVisible { get; set; }

        /// <summary>
        /// A user-definable data object.
        /// </summary>
        public object Tag { get; set; }

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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether or not this control will become focused when the mouse is clicked.
        /// </summary>
        [DataMember]
        public bool FocusOnClick { get; set; }

        /// <summary>
        /// The width of the control.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the control.
        /// </summary>
        public int Height { get; }


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
                if (Parent != null && Parent.Host != null )
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
        public Rectangle Bounds => new Rectangle(_position.X, _position.Y, Width, Height);

        /// <summary>
        /// Gets or sets the parent console of this control.
        /// </summary>
        public IContainer Parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;
                IContainer temp = _parent;
                _parent = null;
                temp?.Remove(this);
                _parent = value;
                _parent?.Add(this);

                OnParentChanged();
            }
        }

        /// <summary>
        /// Gets or sets the colors to use with the <see cref="Theme"/>.
        /// </summary>
        public Colors ThemeColors
        {
            //get => _themeColors;
            set => _themeColors = value;
        }

        /// <summary>
        /// The custom theme to use with this control. If set to <see langword="null"/>, will use the theme assigned by the <see cref="Parent"/>.
        /// </summary>
        public ThemeBase Theme
        {
            get => ActiveTheme;
            set
            {
                if (value != ActiveTheme)
                {
                    if (value == null)
                    {
                        IsCustomTheme = false;
                        ActiveTheme = Library.Default.GetControlTheme(GetType());
                        if (ActiveTheme == null) throw new NullReferenceException($"Theme unavalable for {GetType().FullName}. Register a theme with SadConsole.Library.Default.SetControlTheme");
                    }
                    else
                    {
                        ActiveTheme = value;
                        IsCustomTheme = true;
                    }

                    OnThemeChanged();
                    ActiveTheme.Attached(this);
                    DetermineState();
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// The state of the control.
        /// </summary>
        public ControlStates State => _state;

        /// <summary>
        /// Raised when the mouse enters this control.
        /// </summary>
        public event EventHandler<ControlMouseState> MouseEnter;

        /// <summary>
        /// Raised when the mouse exits this control.
        /// </summary>
        public event EventHandler<ControlMouseState> MouseExit;

        /// <summary>
        /// Raised when the mouse is moved over this control.
        /// </summary>
        public event EventHandler<ControlMouseState> MouseMove;

        /// <summary>
        /// Raised when a mouse button is clicked while the mouse is over this control.
        /// </summary>
        public event EventHandler<ControlMouseState> MouseButtonClicked;

        #region Constructors
        /// <inheritdoc />
        /// <summary>
        /// Creates a control.
        /// </summary>
        protected ControlBase(int width, int height)
        {
            Width = width;
            Height = height;
            IsDirty = true;
            TabStop = true;
            IsVisible = true;
            FocusOnClick = true;
            CanFocus = true;
            _position = new Point();
            UseMouse = true;
            UseKeyboard = true;
            MouseArea = new Rectangle(0, 0, width, height);
            Theme = Library.Default.GetControlTheme(GetType());
            if (Theme == null) throw new NullReferenceException($"Theme unavalable for {GetType().FullName}. Register a theme with SadConsole.Library.Default.SetControlTheme");
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
        /// <returns>Always returns false.</returns>
        public virtual bool ProcessMouse(Input.MouseScreenObjectState state)
        {
            if (IsEnabled && UseMouse)
            {
                var newState = new ControlMouseState(this, state);
                
                if (newState.IsMouseOver)
                {
                    if (_isMouseOver != true)
                    {
                        _isMouseOver = true;
                        OnMouseEnter(newState);
                    }

                    bool preventClick = _mouseEnteredWithButtonDown;
                    OnMouseIn(newState);

                    if (!preventClick && state.Mouse.LeftClicked)
                        OnLeftMouseClicked(newState);

                    if (!preventClick && state.Mouse.RightClicked)
                        OnRightMouseClicked(newState);

                    return true;
                }
                else
                {
                    if (_isMouseOver)
                    {
                        _isMouseOver = false;
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
            if (_isMouseOver)
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
        /// Plases this control relative to another, taking into account the bounds of the control.
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
            ControlStates oldState = _state;

            _state = !_isEnabled
                ? (ControlStates)Helpers.SetFlag((int)_state, (int)ControlStates.Disabled)
                : (ControlStates)Helpers.UnsetFlag((int)_state, (int)ControlStates.Disabled);

            _state = _isMouseOver
                ? (ControlStates)Helpers.SetFlag((int)_state, (int)ControlStates.MouseOver)
                : (ControlStates)Helpers.UnsetFlag((int)_state, (int)ControlStates.MouseOver);

            _state = IsFocused && Parent.Host.ParentConsole != null && Parent.Host.ParentConsole.IsFocused
                ? (ControlStates)Helpers.SetFlag((int)_state, (int)ControlStates.Focused)
                : (ControlStates)Helpers.UnsetFlag((int)_state, (int)ControlStates.Focused);

            _state = _isMouseLeftDown && IsMouseButtonStateClean
                ? (ControlStates)Helpers.SetFlag((int)_state, (int)ControlStates.MouseLeftButtonDown)
                : (ControlStates)Helpers.UnsetFlag((int)_state, (int)ControlStates.MouseLeftButtonDown);

            _state = _isMouseRightDown && IsMouseButtonStateClean
                ? (ControlStates)Helpers.SetFlag((int)_state, (int)ControlStates.MouseRightButtonDown)
                : (ControlStates)Helpers.UnsetFlag((int)_state, (int)ControlStates.MouseRightButtonDown);

            if (oldState != _state)
            {
                OnStateChanged(oldState, _state);
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
            _themeColors ?? _parent?.Host.ThemeColors ?? Library.Default.Colors;

        /// <summary>
        /// Called when the mouse first enters the control. Raises the MouseEnter event and calls the <see cref="DetermineState"/> method.
        /// </summary>
        /// <param name="state">The current mouse data</param>
        protected virtual void OnMouseEnter(ControlMouseState state)
        {
            _isMouseOver = true;

            if (state.OriginalMouseState.Mouse.LeftButtonDown || state.OriginalMouseState.Mouse.RightButtonDown)
                _mouseEnteredWithButtonDown = true;

            MouseEnter?.Invoke(this, state);

            DetermineState();
        }

        /// <summary>
        /// Called when the mouse exits the area of the control. Raises the MouseExit event and calls the <see cref="DetermineState"/> method.
        /// </summary>
        /// <param name="state">The current mouse data</param>
        protected virtual void OnMouseExit(ControlMouseState state)
        {
            _isMouseLeftDown = false;
            _isMouseRightDown = false;
            _isMouseOver = false;
            _mouseEnteredWithButtonDown = false;
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

            _isMouseLeftDown = state.OriginalMouseState.Mouse.LeftButtonDown;
            _isMouseRightDown = state.OriginalMouseState.Mouse.RightButtonDown;

            if (_mouseEnteredWithButtonDown && !_isMouseLeftDown && !_isMouseRightDown)
                _mouseEnteredWithButtonDown = false;

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
                IsMouseOver = control.MouseArea.Contains(MousePosition);
            }


            /// <summary>
            /// Creates an instance of the mouse control state class and infers the <see cref="MousePosition"/> from the control and state.
            /// </summary>
            /// <param name="control">The control.</param>
            /// <param name="originalMouseState">The original mouse state sent to the control.</param>
            public ControlMouseState(ControlBase control, MouseScreenObjectState originalMouseState): this(control, originalMouseState.SurfaceCellPosition - control.AbsolutePosition, originalMouseState)
            {

            }
        }
    }
}
