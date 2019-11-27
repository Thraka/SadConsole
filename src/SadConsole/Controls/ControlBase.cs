#if XNA
using Microsoft.Xna.Framework;
#endif

using SadConsole.Input;
using System.Runtime.Serialization;
using System;

namespace SadConsole.Controls
{
    /// <summary>
    /// Base class for all controls.
    /// </summary>
    [DataContract]
    public abstract class ControlBase
    {
        protected Point position;
        protected bool isMouseOver = false;
        protected bool isEnabled = true;
        protected ControlsConsole parent;
        protected ControlStates state;

        protected Themes.ThemeBase ActiveTheme;
        protected bool IsCustomTheme;
        protected Themes.Colors _themeColors;

        /// <summary>
        /// <see langword="true"/> when the left mouse button is down.
        /// </summary>
        protected bool isMouseLeftDown;

        /// <summary>
        /// <see langword="true"/> when the right mouse button is down.
        /// </summary>
        protected bool isMouseRightDown;

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
        public CellSurface Surface { get; set; }

        /// <summary>
        /// The region the of the control used for mouse input.
        /// </summary>
        [DataMember]
        public Rectangle MouseBounds { get; set; }

        /// <summary>
        /// Indicates the rendering location of this control.
        /// </summary>
        [DataMember]
        public Point Position
        {
            get => position;
            set
            {
                Point oldPosition = position;
                position = value;
                Bounds = new Rectangle(position.X, position.Y, Width, Height);
                Point mouseBoundsPosition = MouseBounds.Location + value - oldPosition;
                MouseBounds = new Rectangle(mouseBoundsPosition.X, mouseBoundsPosition.Y, MouseBounds.Width, MouseBounds.Height);
                OnPositionChanged();
            }
        }

        /// <summary>
        /// Indicates weather or not this control is visible.
        /// </summary>
        [DataMember]
        public bool IsVisible { get; set; }

        /// <summary>
        /// Indicates weather or not this control can be tabbed to.
        /// </summary>
        [DataMember]
        public bool TabStop { get; set; }

        /// <summary>
        /// Sets the tab index of this control.
        /// </summary>
        [DataMember]
        public int TabIndex { get; set; }

        /// <summary>
        /// Indicates weather or not this control is dirty and should be redrawn.
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
        /// Gets or sets weather or not this control will become focused when the mouse is clicked.
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
        /// Gets or sets weather or not this control is focused.
        /// </summary>
        public bool IsFocused
        {
            get => (Parent == null) ? false : Parent.FocusedControl == this;
            set
            {
                if (Parent != null)
                {
                    if (value)
                    {
                        if (Parent.FocusedControl != this)
                        {
                            Parent.FocusedControl = this;
                        }
                    }
                    else if (Parent.FocusedControl == this)
                    {
                        Parent.TabNextControl();

                        if (Parent.FocusedControl == this)
                        {
                            Parent.FocusedControl = null;
                        }
                    }

                    DetermineState();
                }
            }
        }

        /// <summary>
        /// Gets or sets weather or not this control is enabled.
        /// </summary>
        [DataMember]
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                DetermineState();
            }
        }

        /// <summary>
        /// The area this control covers.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Gets or sets the parent console of this control.
        /// </summary>
        public ControlsConsole Parent
        {
            get => parent;
            set
            {
                parent = value;
                OnParentChanged();
            }
        }

        /// <summary>
        /// Gets or sets the colors to use with the <see cref="Theme"/>.
        /// </summary>
        public Themes.Colors ThemeColors
        {
            get => _themeColors ?? parent?.ThemeColors;
            set => _themeColors = value;
        }

        /// <summary>
        /// The custom theme to use with this control. If set to <see langword="null"/>, will use the theme assigned by the <see cref="Themes.Library"/>.
        /// </summary>
        public Themes.ThemeBase Theme
        {
            get => ActiveTheme;
            set
            {
                if (value != ActiveTheme)
                {
                    if (value == null)
                    {
                        IsCustomTheme = false;
                        ActiveTheme = Themes.Library.Default.GetControlTheme(GetType());
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
        public ControlStates State => state;

        /// <summary>
        /// Raised when the mouse enters this control.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseEnter;

        /// <summary>
        /// Raised when the mouse exits this control.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseExit;

        /// <summary>
        /// Raised when the mouse is moved over this control.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseMove;

        /// <summary>
        /// Raised when a mouse button is clicked while the mouse is over this control.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseButtonClicked;

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
            position = new Point();
            UseMouse = true;
            UseKeyboard = true;
            Bounds = new Rectangle(0, 0, width, height);
            MouseBounds = new Rectangle(0, 0, width, height);
            Theme = SadConsole.Themes.Library.Default.GetControlTheme(GetType());
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
        public virtual bool ProcessMouse(Input.MouseConsoleState state)
        {
            if (IsEnabled && UseMouse)
            {
                if (state.Console == parent && state.IsOnConsole && MouseBounds.Contains(state.CellPosition))
                {
                    if (isMouseOver != true)
                    {
                        isMouseOver = true;
                        OnMouseEnter(state);
                    }

                    OnMouseIn(state);

                    if (state.Mouse.LeftClicked)
                    {
                        OnLeftMouseClicked(state);
                    }

                    if (state.Mouse.RightClicked)
                    {
                        OnRightMouseClicked(state);
                    }
                }
                else
                {
                    if (isMouseOver)
                    {
                        isMouseOver = false;
                        OnMouseExit(state);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Called to trigger the state of losing mouse focus.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        public void LostMouse(MouseConsoleState state) => OnMouseExit(state);
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
        /// Sets the appropriate theme for the control based on the current state of the control.
        /// </summary>
        /// <remarks>Called by the control as the mouse state changes, like when the mouse is clicked on top of the control or leaves the area of the control. This method is implemented by each derived control.</remarks>
        public virtual void DetermineState()
        {
            ControlStates oldState = state;

            if (!isEnabled)
            {
                Helpers.SetFlag(ref state, ControlStates.Disabled);
            }
            else
            {
                Helpers.UnsetFlag(ref state, ControlStates.Disabled);
            }

            if (isMouseOver)
            {
                Helpers.SetFlag(ref state, ControlStates.MouseOver);
            }
            else
            {
                Helpers.UnsetFlag(ref state, ControlStates.MouseOver);
            }

            if (IsFocused)
            {
                Helpers.SetFlag(ref state, ControlStates.Focused);
            }
            else
            {
                Helpers.UnsetFlag(ref state, ControlStates.Focused);
            }

            if (isMouseLeftDown)
            {
                Helpers.SetFlag(ref state, ControlStates.MouseLeftButtonDown);
            }
            else
            {
                Helpers.UnsetFlag(ref state, ControlStates.MouseLeftButtonDown);
            }

            if (isMouseRightDown)
            {
                Helpers.SetFlag(ref state, ControlStates.MouseRightButtonDown);
            }
            else
            {
                Helpers.UnsetFlag(ref state, ControlStates.MouseRightButtonDown);
            }

            if (oldState != state)
            {
                OnStateChanged(oldState, state);
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
        /// Called when the mouse first enters the control. Raises the MouseEnter event and calls the <see cref="DetermineState"/> method.
        /// </summary>
        /// <param name="state">The current mouse data</param>
        protected virtual void OnMouseEnter(Input.MouseConsoleState state)
        {
            isMouseOver = true;
            MouseEnter?.Invoke(this, new MouseEventArgs(state));

            DetermineState();
        }

        /// <summary>
        /// Called when the mouse exits the area of the control. Raises the MouseExit event and calls the <see cref="DetermineState"/> method.
        /// </summary>
        /// <param name="state">The current mouse data</param>
        protected virtual void OnMouseExit(Input.MouseConsoleState state)
        {
            isMouseLeftDown = false;
            isMouseRightDown = false;
            isMouseOver = false;
            MouseExit?.Invoke(this, new MouseEventArgs(state));

            DetermineState();
        }

        /// <summary>
        /// Called as the mouse moves around the control area. Raises the MouseMove event and calls the <see cref="DetermineState"/> method.
        /// </summary>
        /// <param name="state">The current mouse data</param>
        protected virtual void OnMouseIn(Input.MouseConsoleState state)
        {
            MouseMove?.Invoke(this, new MouseEventArgs(state));

            isMouseLeftDown = state.Mouse.LeftButtonDown;
            isMouseRightDown = state.Mouse.RightButtonDown;

            DetermineState();
        }

        /// <summary>
        /// Called when the left mouse button is clicked. Raises the MouseButtonClicked event and calls the <see cref="DetermineState"/> method.
        /// </summary>
        /// <param name="state">The current mouse data</param>
        protected virtual void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            MouseButtonClicked?.Invoke(this, new MouseEventArgs(state));

            if (FocusOnClick)
            {
                IsFocused = true;
            }

            DetermineState();
        }

        /// <summary>
        /// Called when the right mouse button is clicked. Raises the MouseButtonClicked event and calls the <see cref="DetermineState"/> method.
        /// </summary>
        /// <param name="state">The current mouse data</param>
        protected virtual void OnRightMouseClicked(Input.MouseConsoleState state)
        {
            MouseButtonClicked?.Invoke(this, new MouseEventArgs(state));

            DetermineState();
        }

        /// <summary>
        /// Helper method that returns the mouse x,y position for the control.
        /// </summary>
        /// <param name="consolePosition">Position of the console to get the relative control position from.</param>
        /// <returns>The x,y position of the mouse over the control.</returns>
        protected Point TransformConsolePositionByControlPosition(Point consolePosition) => consolePosition - position;

        /// <summary>
        /// Update the control appearance based on <see cref="DetermineState"/> and <see cref="IsDirty"/>.
        /// </summary>
        public virtual void Update(TimeSpan time) => Theme.UpdateAndDraw(this, time);

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            IsDirty = true;
            Bounds = new Rectangle(position.X, position.Y, Width, Height);
        }
    }
}
