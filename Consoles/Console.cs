namespace SadConsole.Consoles
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using SadConsole.Input;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a traditional console that implements mouse and keyboard handling as well as a cursor.
    /// </summary>
    [DataContract]
    [KnownType(typeof(CellAppearance))]
    public partial class Console : CellsRenderer, IConsole
    {
        #region Events
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
        #endregion

        /// <summary>
        /// The parent console.
        /// </summary>
        protected IParentConsole _parentConsole;

        /// <summary>
        /// Indicates that the mouse is currently over this console.
        /// </summary>
        protected bool _isMouseOver = false;

        /// <summary>
        /// The private virtual curser reference.
        /// </summary>
        [DataMember(Name = "VirtualCursor")]
        protected Cursor _virtualCursor;

        /// <summary>
        /// Toggles the _virtualCursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        [DataMember]
        public bool AutoCursorOnFocus { get; set; }

        /// <summary>
        /// Represents a _virtualCursor that can be used to input information into the console.
        /// </summary>
        public Console.Cursor VirtualCursor
        {
            get { return _virtualCursor; }
            set
            {
                if (value != null)
                    _virtualCursor = value;
                else
                    throw new Exception("VirtualCursor cannot be null");
            }
        }

        /// <summary>
        /// Indicates that the mouse is currently over this console.
        /// </summary>
        public bool IsMouseOver { get { return _isMouseOver; } }

        /// <summary>
        /// Gets or sets the Parent console.
        /// </summary>
        public IParentConsole Parent
        {
            get { return _parentConsole; }
            set
            {
                if (_parentConsole != value)
                {
                    if (_parentConsole == null)
                    {
                        _parentConsole = value;
                        _parentConsole.Add(this);
                        OnParentConsoleChanged(null, _parentConsole);
                    }
                    else
                    {
                        var oldParent = _parentConsole;
                        _parentConsole = value;

                        oldParent.Remove(this);

                        if (_parentConsole != null)
                            _parentConsole.Add(this);

                        OnParentConsoleChanged(oldParent, _parentConsole);
                    }
                }

            }
        }

        /// <summary>
        /// When true, this console will move to the front of its parent console when focused.
        /// </summary>
        [DataMember]
        public bool MoveToFrontOnMouseFocus { get; set; }

        /// <summary>
        /// Allows the mouse (with a click) to focus this console.
        /// </summary>
        [DataMember]
        public bool MouseCanFocus { get; set; }

        /// <summary>
        /// Allows this console to accept keyboard input.
        /// </summary>
        [DataMember]
        public bool CanUseKeyboard { get; set; }

        /// <summary>
        /// Allows this console to accept mouse input.
        /// </summary>
        [DataMember]
        public bool CanUseMouse { get; set; }

        /// <summary>
        /// Allows this console to be focusable.
        /// </summary>
        [DataMember]
        public bool CanFocus { get; set; }

        /// <summary>
        /// Gets or sets this console as the <see cref="Engine.ActiveConsole"/> value.
        /// </summary>
        /// <remarks>If the <see cref="Engine.ActiveConsole"/> has the <see cref="Console.ExclusiveFocus"/> property set to true, you cannot use this property to set this console to focused.</remarks>
        public bool IsFocused
        {
            get { return Engine.ActiveConsole == this; }
            set
            {
                if (Engine.ActiveConsole != null)
                {
                    if (value && Engine.ActiveConsole != this && !Engine.ActiveConsole.ExclusiveFocus)
                    {
                        Engine.ActiveConsole = this;
                        OnFocused();
                    }

                    else if (value && Engine.ActiveConsole == this)
                        OnFocused();

                    else if (!value)
                    {
                        if (Engine.ActiveConsole == this)
                            Engine.ActiveConsole = null;

                        OnFocusLost();
                    }
                }
                else
                {
                    if (value)
                    {
                        Engine.ActiveConsole = this;
                        OnFocused();
                    }
                    else
                        OnFocusLost();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not this console has exclusive access to the mouse events.
        /// </summary>
        public bool ExclusiveFocus { get; set; }

        /// <summary>
        /// An alternative method handler for handling the mouse logic.
        /// </summary>
        public Func<IConsole, MouseInfo, bool> MouseHandler { get; set; }

        /// <summary>
        /// An alternative method handler for handling the keyboard logic.
        /// </summary>
        public Func<IConsole, KeyboardInfo, bool> KeyboardHandler { get; set; }

        #region Constructors
        public Console() : this(1, 1) { }
        public Console(int width, int height)
            : base(new CellSurface(width, height), new SpriteBatch(Engine.Device))
        {
            _virtualCursor = new Cursor(this);
        }

        public Console(CellSurface cellData)
            : base(cellData, new SpriteBatch(Engine.Device))
        {
            _virtualCursor = new Cursor(this);
        }
        #endregion

        protected virtual void OnMouseEnter(MouseInfo info)
        {
            if (MouseEnter != null)
                MouseEnter(this, new MouseEventArgs(info));
        }

        protected virtual void OnMouseExit(MouseInfo info)
        {
            // Force mouse off just incase
            _isMouseOver = false;

            if (MouseExit != null)
                MouseExit(this, new MouseEventArgs(info));
        }

        protected virtual void OnMouseIn(MouseInfo info)
        {
            if (MouseMove != null)
                MouseMove(this, new MouseEventArgs(info));
        }

        protected virtual void OnMouseLeftClicked(MouseInfo info)
        {
            if (MouseButtonClicked != null)
                MouseButtonClicked(this, new MouseEventArgs(info));
        }

        protected virtual void OnRightMouseClicked(MouseInfo info)
        {
            if (MouseButtonClicked != null)
                MouseButtonClicked(this, new MouseEventArgs(info));
        }

        /// <summary>
        /// Processes the mouse.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>True when the mouse is over this console.</returns>
        public virtual bool ProcessMouse(MouseInfo info)
        {
            var handlerResult = MouseHandler == null ? false : MouseHandler(this, info);

            if (!handlerResult)
            {
                if (this.IsVisible && this.CanUseMouse)
                {
                    info.Fill(this);

                    if (info.Console == this)
                    {
                        if (this.CanFocus && this.MouseCanFocus && info.LeftClicked)
                        {
                            IsFocused = true;

                            if (IsFocused && this.MoveToFrontOnMouseFocus && this.Parent != null && this.Parent.IndexOf(this) != this.Parent.Count - 1)
                                this.Parent.MoveToTop(this);
                        }

                        if (_isMouseOver != true)
                        {
                            _isMouseOver = true;
                            OnMouseEnter(info);
                        }

                        OnMouseIn(info);

                        if (info.LeftClicked)
                            OnMouseLeftClicked(info);

                        if (info.RightClicked)
                            OnRightMouseClicked(info);

                        return true;
                    }
                    else
                    {
                        if (_isMouseOver)
                        {
                            _isMouseOver = false;
                            OnMouseExit(info);
                        }
                    }
                }
            }

            return handlerResult;
        }

        /// <summary>
        /// Called by the engine to process the keyboard. If the <see cref="KeyboardHandler"/> has been set, that will be called instead of this method.
        /// </summary>
        /// <param name="info">Keyboard information.</param>
        /// <returns>True when the keyboard had data and this console did something with it.</returns>
        public virtual bool ProcessKeyboard(KeyboardInfo info)
        {
            var handlerResult = KeyboardHandler == null ? false : KeyboardHandler(this, info);

            if (!handlerResult && this.CanUseKeyboard)
            {
                bool didSomething = false;
                foreach (var key in info.KeysPressed)
                {
                    if (key.Character == '\0')
                    {
                        switch (key.XnaKey)
                        {
                            case Keys.Space:
                                this._virtualCursor.Print(key.Character.ToString());
                                didSomething = true;
                                break;
                            case Keys.Enter:
                                this._virtualCursor.CarriageReturn().LineFeed();
                                didSomething = true;
                                break;
#if !SILVERLIGHT
                            case Keys.LeftShift:
                            case Keys.RightShift:
                            case Keys.LeftAlt:
                            case Keys.RightAlt:
                            case Keys.LeftControl:
                            case Keys.RightControl:
                            case Keys.LeftWindows:
                            case Keys.RightWindows:
                            case Keys.F1:case Keys.F2:case Keys.F3:case Keys.F4:case Keys.F5:case Keys.F6:case Keys.F7:case Keys.F8:case Keys.F9:case Keys.F10:
                            case Keys.F11:case Keys.F12:case Keys.F13:case Keys.F14:case Keys.F15:case Keys.F16:case Keys.F17:case Keys.F18:case Keys.F19:case Keys.F20:
                            case Keys.F21:case Keys.F22:case Keys.F23:case Keys.F24:
                            case Keys.Pause:
                            case Keys.Escape:
#else
							case Keys.Shift:
							case Keys.Alt:
							case Keys.Ctrl:
#endif
                                //this._virtualCursor.Print(key.Character.ToString());
                                break;
                            case Keys.Up:
                                this._virtualCursor.Up(1);
                                didSomething = true;
                                break;
                            case Keys.Left:
                                this._virtualCursor.Left(1);
                                didSomething = true;
                                break;
                            case Keys.Right:
                                this._virtualCursor.Right(1);
                                didSomething = true;
                                break;
                            case Keys.Down:
                                this._virtualCursor.Down(1);
                                didSomething = true;
                                break;
                            case Keys.None:
                                break;
                            case Keys.Back:
                                this._virtualCursor.Left(1).Print(" ").Left(1);
                                didSomething = true;
                                break;
                            default:
                                this._virtualCursor.Print(key.Character.ToString());
                                didSomething = true;
                                break;
                        }
                    }
                    else
                    {
                        this._virtualCursor.Print(key.Character.ToString());
                        didSomething = true;
                    }
                }

                return didSomething;
            }

            return handlerResult;
        }

        public void FillWithRandomGarbage()
        {
            Random rand = new Random(2523);
            SadConsole.Effects.Blink pulse = new SadConsole.Effects.Blink();
            //pulse.Reset();
            int charCounter = 0;
            for (int y = 0; y < CellData.Height; y++)
            {
                for (int x = 0; x < CellData.Width; x++)
                {
                    CellData.SetCharacter(x, y, charCounter);
                    CellData.SetForeground(x, y, new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255), 255));
                    CellData.SetBackground(x, y, CellData.DefaultBackground);
                    CellData.SetBackground(x, y, new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255), 255));
                    pulse.BlinkSpeed = ((float)rand.NextDouble() * 3f);
                    
                    charCounter++;
                    if (charCounter > 255)
                        charCounter = 0;
                }
            }

            
        }
        
        /// <summary>
        /// Called when this console's focus has been lost.
        /// </summary>
        protected virtual void OnFocusLost()
        {
            if (AutoCursorOnFocus == true)
                _virtualCursor.IsVisible = false;
        }

        /// <summary>
        /// Called when this console is focused.
        /// </summary>
        protected virtual void OnFocused()
        {
            if (AutoCursorOnFocus == true)
                _virtualCursor.IsVisible = true;
        }

        protected override void OnAfterRender()
        {
            if (VirtualCursor.IsVisible)
            {
                int virtualCursorLocationIndex = CellSurface.GetIndexFromPoint(new Point(VirtualCursor.Position.X - ViewArea.Location.X, VirtualCursor.Position.Y - ViewArea.Location.Y), ViewArea.Width);

                if (virtualCursorLocationIndex >= 0 && virtualCursorLocationIndex < _renderAreaRects.Length)
                {
                    VirtualCursor.Render(Batch, Font, _renderAreaRects[virtualCursorLocationIndex]);
                }
            }

            base.OnAfterRender();
        }

        /// <summary>
        /// Updates the cell effects and cursor.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (this.DoUpdate && VirtualCursor.IsVisible)
                VirtualCursor.CursorRenderCell.UpdateAndApplyEffect(Engine.GameTimeElapsedUpdate);
        }

        /// <summary>
        /// Called when the parent console changes for this console.
        /// </summary>
        /// <param name="oldParent">The previous parent.</param>
        /// <param name="newParent">The new parent.</param>
        protected virtual void OnParentConsoleChanged(IParentConsole oldParent, IParentConsole newParent) { }

        /// <summary>
        /// Used by the console engine to properly clear the mouse over flag and call OnMouseExit. Used when mouse exits window.
        /// </summary>
        private void ExitMouse()
        {
            if (_isMouseOver)
            {
                _isMouseOver = false;

                MouseInfo info = Engine.Mouse.Clone();
                info.ConsoleLocation = new Point(-1, -1);

                OnMouseExit(info);
            }
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            _virtualCursor.AttachConsole(this);
        }
    }
}
