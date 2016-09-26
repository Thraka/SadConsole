#if SFML
using Point = SFML.System.Vector2i;
using Rectangle = SFML.Graphics.IntRect;
#elif MONOGAME
using Microsoft.Xna.Framework;
#endif

using SadConsole.Consoles;
using SadConsole.Input;
using System.Runtime.Serialization;

namespace SadConsole.Controls
{
    /// <summary>
    /// Base class for all controls.
    /// </summary>
    [DataContract]
    public abstract class ControlBase: SurfaceEditor, IInput
    {
        protected Point position;
        protected Rectangle bounds;
        protected bool isMouseOver = false;
        protected bool isEnabled = true;
        protected Consoles.ControlsConsole parent;

        [DataMember]
        public bool CanUseKeyboard { get; set; }

        [DataMember]
        public bool CanUseMouse { get; set; }

        [DataMember]
        public bool CanFocus { get; set; }

        [DataMember]
        public bool ExclusiveFocus { get; set; }

        [DataMember]
        public Font AlternateFont { get; set; }

        /// <summary>
        /// Indicates he rendering location of this control.
        /// </summary>
        [DataMember]
        public Point Position { get { return position; } set { position = value; bounds = new Rectangle(position.X, position.Y, Width, Height); OnPositionChanged(); } }

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
        public bool IsDirty { get; set; }

        /// <summary>
        /// Represents a name to identify a control by.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets weather or not this control will become focused when the mosue is clicked.
        /// </summary>
        [DataMember]
        public bool FocusOnClick { get; set; }

        /// <summary>
        /// Gets or sets weather or not this control is focused.
        /// </summary>
        public bool IsFocused
        {
            get
            {
                if (Parent == null)
                    return false;
                else
                    return Parent.FocusedControl == this;
            }
            set
            {
                if (Parent != null)
                {
                    if (value)
                    {
                        if (Parent.FocusedControl != this)
                            Parent.FocusedControl = this;
                    }
                    else if (Parent.FocusedControl == this)
                    {
                        Parent.TabNextControl();

                        if (Parent.FocusedControl == this)
                            Parent.FocusedControl = null;
                    }

                    DetermineAppearance();
                }

            }
        }

        /// <summary>
        /// Gets or sets weather or not this control is enabled.
        /// </summary>
        [DataMember]
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                DetermineAppearance();
            }
        }

        /// <summary>
        /// The width of the control.
        /// </summary>
        public int Width { get { return textSurface.Width; } }

        /// <summary>
        /// The height of the control.
        /// </summary>
        public int Height { get { return textSurface.Height; } }

        /// <summary>
        /// The area this control covers.
        /// </summary>
        public Rectangle Bounds { get { return bounds; } }
        
        /// <summary>
        /// Gets or sets the parent console of this control.
        /// </summary>
        public Consoles.ControlsConsole Parent
        {
            get { return parent; }
            set { parent = value; OnParentChanged(); }
        }

        /// <summary>
        /// Raised when the mouse enters this control.
        /// </summary>
        public event System.EventHandler<MouseEventArgs> MouseEnter;

        /// <summary>
        /// Raised when the mouse exits this control.
        /// </summary>
        public event System.EventHandler<MouseEventArgs> MouseExit;

        /// <summary>
        /// Raised when the mouse is moved over this control.
        /// </summary>
        public event System.EventHandler<MouseEventArgs> MouseMove;

        /// <summary>
        /// Raised when a mouse button is clicked while the mouse is over this control.
        /// </summary>
        public event System.EventHandler<MouseEventArgs> MouseButtonClicked;

        #region Constructors
        /// <summary>
        /// Default constructor of the control.
        /// </summary>
        public ControlBase(int width, int height)
            : base(new TextSurface(width, height, Engine.DefaultFont))
        {
            IsDirty = true;
            TabStop = true;
            IsVisible = true;
            FocusOnClick = true;
            CanFocus = false;
            position = new Point();
            CanUseMouse = true;
            CanUseKeyboard = true;
            bounds = new Rectangle(0, 0, width, height);
        }
        #endregion

        /// <summary>
        /// Called when the control loses focus. Calls DetermineAppearance.
        /// </summary>
        public virtual void FocusLost() { DetermineAppearance(); }

        /// <summary>
        /// Called when the control is focused. Calls DetermineAppearance.
        /// </summary>
        public virtual void Focused() { DetermineAppearance(); }

        #region Input
        /// <summary>
        /// Called when the keyboard is used on this control.
        /// </summary>
        /// <param name="info">The state of the keyboard.</param>
        public virtual bool ProcessKeyboard(KeyboardInfo info) { return false; }

        /// <summary>
        /// Checks if the mouse is the control and calls the appropriate mouse methods.
        /// </summary>
        /// <param name="info">Mouse information.</param>
        /// <returns>Always returns false.</returns>
        public virtual bool ProcessMouse(Input.MouseInfo info)
        {
            if (IsEnabled && CanUseMouse)
            {
                if (info.ConsoleLocation.X >= Position.X && info.ConsoleLocation.X < Position.X + Width &&
                    info.ConsoleLocation.Y >= Position.Y && info.ConsoleLocation.Y < Position.Y + Height)
                {
                    if (isMouseOver != true)
                    {
                        isMouseOver = true;
                        OnMouseEnter(info);
                    }

                    OnMouseIn(info);

                    if (info.LeftClicked)
                        OnLeftMouseClicked(info);

                    if (info.RightClicked)
                        OnRightMouseClicked(info);
                }
                else
                {
                    if (isMouseOver)
                    {
                        isMouseOver = false;
                        OnMouseExit(info);
                    }
                }
            }

            return false;
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
        /// Sets the appropriate theme for the control based on the current state of the control.
        /// </summary>
        /// <remarks>Called by the control as the mouse state changes, like when the mouse is clicked on top of the control or leaves the area of the control. This method is implemented by each derived control.</remarks>
        public abstract void DetermineAppearance();

        /// <summary>
        /// Called when the mouse first enters the control. Raises the MouseEnter event and calls the <see cref="DetermineAppearance"/> method.
        /// </summary>
        /// <param name="info">The current mouse data</param>
        protected virtual void OnMouseEnter(Input.MouseInfo info)
        {
            if (MouseEnter != null)
                MouseEnter(this, new MouseEventArgs(info));

            DetermineAppearance();
        }

        /// <summary>
        /// Called when the mouse exits the area of the control. Raises the MouseExit event and calls the <see cref="DetermineAppearance"/> method.
        /// </summary>
        /// <param name="info">The current mouse data</param>
        protected virtual void OnMouseExit(Input.MouseInfo info)
        {
            if (MouseExit != null)
                MouseExit(this, new MouseEventArgs(info));

            DetermineAppearance();
        }

        /// <summary>
        /// Called as the mouse moves around the control area. Raises the MouseMove event and calls the <see cref="DetermineAppearance"/> method.
        /// </summary>
        /// <param name="info">The current mouse data</param>
        protected virtual void OnMouseIn(Input.MouseInfo info)
        {
            if (MouseMove != null)
                MouseMove(this, new MouseEventArgs(info));

            DetermineAppearance();
        }

        /// <summary>
        /// Called when the left mouse button is clicked. Raises the MouseButtonClicked event and calls the <see cref="DetermineAppearance"/> method.
        /// </summary>
        /// <param name="info">The current mouse data</param>
        protected virtual void OnLeftMouseClicked(Input.MouseInfo info)
        {
            if (MouseButtonClicked != null)
                MouseButtonClicked(this, new MouseEventArgs(info));

            if (FocusOnClick)
                this.IsFocused = true;

            DetermineAppearance();
        }

        /// <summary>
        /// Called when the right mouse button is clicked. Raises the MouseButtonClicked event and calls the <see cref="DetermineAppearance"/> method.
        /// </summary>
        /// <param name="info">The current mouse data</param>
        protected virtual void OnRightMouseClicked(Input.MouseInfo info)
        {
            if (MouseButtonClicked != null)
                MouseButtonClicked(this, new MouseEventArgs(info));

            DetermineAppearance();
        }

        /// <summary>
        /// Helper method that returns the mouse x,y position for the control.
        /// </summary>
        /// <param name="info">The mouse information as used by a mouse event.</param>
        /// <returns>The x,y position of the mouse over the control.</returns>
        protected Point TransformConsolePositionByControlPosition(Input.MouseInfo info)
        {
            return new Point(info.ConsoleLocation.X - this.Position.X, info.ConsoleLocation.Y - this.Position.Y);
        }

        /// <summary>
        /// Redraw the lastest appearance of the control.
        /// </summary>
        /// <remarks>This method is implemented by each derived control.</remarks>
        public abstract void Compose();

        /// <summary>
        /// Redraw the latest appearance of the control if <see cref="IsDirty"/> is set to true.
        /// </summary>
        /// <param name="force">Force the draw to happen by setting IsDirty as true.</param>
        public virtual void Compose(bool force)
        {
            if (force)
            {
                IsDirty = true;
                Compose();
            }
            else
                Compose();
        }

        /// <summary>
        /// Update the control. Calls Compose() and then updates each cell effect if needed.
        /// </summary>
        public virtual void Update()
        {
            Compose();
            
            //TODO: Effects
            //UpdateEffects(Engine.GameTimeElapsedUpdate);
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            IsDirty = true;
            bounds = new Rectangle(position.X, position.Y, Width, Height);
        }
    }
}
