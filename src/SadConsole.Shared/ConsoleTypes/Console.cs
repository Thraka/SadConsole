using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Mouse = SadConsole.Input.Mouse;

using SadConsole.Surfaces;
using SadConsole.Renderers;
using SadConsole.Input;
using System;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace SadConsole
{
    /// <summary>
    /// Represents a traditional console that implements mouse and keyboard handling as well as a cursor.
    /// </summary>
    public partial class Console : SurfaceEditor, IConsole
    {
        /// <summary>
        /// How the console handles becoming <see cref="Global.InputTargets.Console"/>.
        /// </summary>
        [DataContract]
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

        /// <summary>
        /// The position the console will draw the text surface.
        /// </summary>
        protected Point calculatedPosition;
        
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
        /// The renderer used to draw the <see cref="TextSurface"/>.
        /// </summary>
        protected ISurfaceRenderer _renderer;

        /// <summary>
        /// Where the console should be located on the screen.
        /// </summary>
        protected Point position;

        /// <summary>
        /// Indicates the console is visible.
        /// </summary>
        protected bool isVisible = true;

        /// <summary>
        /// The parent console.
        /// </summary>
        protected IScreen parentConsole;

        /// <summary>
        /// Indicates that the mouse is currently over this console.
        /// </summary>
        protected bool isMouseOver = false;

        /// <summary>
        /// The private virtual curser reference.
        /// </summary>
        protected Cursor virtualCursor;

        /// <summary>
        /// Toggles the VirtualCursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        public bool AutoCursorOnFocus { get; set; }

        /// <summary>
        /// Represents a _virtualCursor that can be used to input information into the console.
        /// </summary>
        public Cursor VirtualCursor
        {
            get { return virtualCursor; }
            //set
            //{
            //    if (value != null)
            //        _virtualCursor = value;
            //    else
            //        throw new Exception("VirtualCursor cannot be null");
            //}
        }

        /// <summary>
        /// Children objects related to this one.
        /// </summary>
        public ScreenCollection Children { get; }
        
        /// <summary>
        /// Indicates that the mouse is currently over this console.
        /// </summary>
        public bool IsMouseOver { get { return isMouseOver; } }

        /// <summary>
        /// Gets or sets the Parent console.
        /// </summary>
        public IScreen Parent
        {
            get { return parentConsole; }
            set
            {
                if (parentConsole != value)
                {
                    if (parentConsole == null)
                    {
                        parentConsole = value;
                        parentConsole.Children.Add(this);
                        OnParentConsoleChanged(null, parentConsole);
                        OnCalculateRenderPosition();
                    }
                    else
                    {
                        var oldParent = parentConsole;
                        parentConsole = value;

                        oldParent.Children.Remove(this);

                        if (parentConsole != null)
                            parentConsole.Children.Add(this);

                        OnParentConsoleChanged(oldParent, parentConsole);
                        OnCalculateRenderPosition();
                    }
                }
            }
        }

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
        /// Indicates whether or not this console is visible.
        /// </summary>
        public bool IsVisible { get { return isVisible; } set { isVisible = value; OnVisibleChanged(); } }

        /// <summary>
        /// Indicates the screen object should not process <see cref="Update(TimeSpan)"/>.
        /// </summary>
        public bool IsPaused { get; set; }


        /// <summary>
        /// The renderer used to draw <see cref="TextSurface"/>.
        /// </summary>
        public ISurfaceRenderer Renderer
        {
            get { return _renderer; }
            set
            {
                if (_renderer != null)
                {
                    _renderer.BeforeRenderCallback = null;
                    _renderer.BeforeRenderTintCallback = null;
                    _renderer.AfterRenderCallback = null;
                }

                _renderer = value;
                _renderer.BeforeRenderCallback = this.OnBeforeRender;
                _renderer.BeforeRenderTintCallback = this.OnBeforeRenderTint;
                _renderer.AfterRenderCallback = this.OnAfterRender;
            }
        }

        /// <summary>
        /// The text surface to be rendered or changed.
        /// </summary>
        public new ISurface TextSurface
        {
            get { return base.textSurface; }
            set { textSurface = value; base.TextSurface = value; }
        }

        /// <summary>
        /// Gets or sets the position to render the cells.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set
            {
                Point previousPosition = position;
                position = value;
                OnPositionChanged(previousPosition);
                OnCalculateRenderPosition();
            }
        }

        /// <summary>
        /// The position of this screen relative to the parents.
        /// </summary>
        public Point CalculatedPosition { get { return calculatedPosition; } }

        /// <summary>
        /// How the console should handle becoming active.
        /// </summary>
        public ActiveBehavior FocusedMode { get; set; }

        /// <summary>
        /// Gets or sets this console as the <see cref="Global.InputTargets.Console"/> value.
        /// </summary>
        /// <remarks>If the <see cref="Console.ActiveConsoles.Console"/> has the <see cref="Console.ExclusiveFocus"/> property set to true, you cannot use this property to set this console to focused.</remarks>
        [DataMember]
        public bool IsFocused
        {
            get { return Global.FocusedConsoles.Console == this; }
            set
            {
                if (Global.FocusedConsoles.Console != null)
                {
                    if (value && Global.FocusedConsoles.Console != this)
                    {
                        if (FocusedMode == ActiveBehavior.Push)
                            Global.FocusedConsoles.Push(this);
                        else
                            Global.FocusedConsoles.Set(this);

                        OnFocused();
                    }

                    else if (value && Global.FocusedConsoles.Console == this)
                        OnFocused();

                    else if (!value)
                    {
                        if (Global.FocusedConsoles.Console == this)
                            Global.FocusedConsoles.Pop(this);

                        OnFocusLost();
                    }
                }
                else
                {
                    if (value)
                    {
                        if (FocusedMode == ActiveBehavior.Push)
                            Global.FocusedConsoles.Push(this);
                        else
                            Global.FocusedConsoles.Set(this);

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
        [DataMember]
        public bool IsExclusiveMouse { get; set; }

        /// <summary>
        /// An alternative method handler for handling the mouse logic.
        /// </summary>
        public Func<IConsole, SadConsole.Input.MouseConsoleState, bool> MouseHandler { get; set; }

        /// <summary>
        /// An alternative method handler for handling the keyboard logic.
        /// </summary>
        public Func<IConsole, Input.Keyboard, bool> KeyboardHandler { get; set; }

        /// <summary>
        /// Treats the <see cref="Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        [DataMember]
        public bool UsePixelPositioning { get; set; } = false;

        #region Constructors

        /// <summary>
        /// Creates a new console with the specified width and height, using the <see cref="Engine.DefaultFont"/>.
        /// </summary>
        /// <param name="width">The width of the <see cref="SadConsole.Surface.TextSurface"/> that will back this console.</param>
        /// <param name="height">The height of the <see cref="SadConsole.Surface.TextSurface"/> that will back this console.</param>
        public Console(int width, int height) : this(width, height, Global.FontDefault) { }

        /// <summary>
        /// Creates a new console with the specified width and height, using the specified font.
        /// </summary>
        /// <param name="width">The width of the <see cref="SadConsole.Surface.TextSurface"/> that will back this console.</param>
        /// <param name="height">The height of the <see cref="SadConsole.Surface.TextSurface"/> that will back this console.</param>
        /// <param name="font">The font to use.</param>
        public Console(int width, int height, Font font) : this(new BasicSurface(width, height, font)) { }

        /// <summary>
        /// Wraps an existing text surface using a <see cref="TextSurfaceRenderer"/> to render.
        /// </summary>
        /// <param name="textData">The backing text surface.</param>
        public Console(ISurface textData) : base(textData)
        {
            Children = new ScreenCollection(this);
            virtualCursor = new Cursor(this);
            Renderer = new SurfaceRenderer();
            textSurface = textData;
        }
        #endregion

        protected virtual void OnMouseEnter(MouseConsoleState state)
        {
            MouseEnter?.Invoke(this, new MouseEventArgs(state));
        }

        protected virtual void OnMouseExit(MouseConsoleState state)
        {
            // Force mouse off just incase
            isMouseOver = false;

            MouseExit?.Invoke(this, new MouseEventArgs(state));
        }

        protected virtual void OnMouseMove(MouseConsoleState state)
        {
            MouseMove?.Invoke(this, new MouseEventArgs(state));
        }

        protected virtual void OnMouseLeftClicked(MouseConsoleState state)
        {
            if (MoveToFrontOnMouseClick && Parent != null && Parent.Children.IndexOf(this) != Parent.Children.Count - 1)
                Parent.Children.MoveToTop(this);

            if (FocusOnMouseClick && !IsFocused)
                IsFocused = true;

            MouseButtonClicked?.Invoke(this, new MouseEventArgs(state));
        }

        protected virtual void OnRightMouseClicked(MouseConsoleState state)
        {
            MouseButtonClicked?.Invoke(this, new MouseEventArgs(state));
        }

        public void LostMouse(MouseConsoleState state)
        {
            if (isMouseOver)
                OnMouseExit(state);
        }

        /// <summary>
        /// Processes the mouse.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>True when the mouse is over this console.</returns>
        public virtual bool ProcessMouse(MouseConsoleState state)
        {
            var handlerResult = MouseHandler?.Invoke(this, state) ?? false;

            if (!handlerResult)
            {
                if (this.IsVisible && this.UseMouse)
                {
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
                    else
                    {
                        if (isMouseOver)
                        {
                            isMouseOver = false;
                            OnMouseExit(state);
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
        public virtual bool ProcessKeyboard(Input.Keyboard info)
        {
            var handlerResult = KeyboardHandler == null ? false : KeyboardHandler(this, info);

            if (!handlerResult && this.UseKeyboard)
            {
                bool didSomething = false;
                foreach (var key in info.KeysPressed)
                {
                    if (key.Character == '\0')
                    {
                        switch (key.Key)
                        {
                            case Keys.Space:
                                this.virtualCursor.Print(key.Character.ToString());
                                didSomething = true;
                                break;
                            case Keys.Enter:
                                this.virtualCursor.CarriageReturn().LineFeed();
                                didSomething = true;
                                break;

                            case Keys.Pause:
                            case Keys.Escape:
                            case Keys.F1: case Keys.F2: case Keys.F3: case Keys.F4: case Keys.F5: case Keys.F6: case Keys.F7: case Keys.F8: case Keys.F9: case Keys.F10: case Keys.F11: case Keys.F12:
                            case Keys.LeftShift:
                            case Keys.RightShift:
                            case Keys.LeftAlt:
                            case Keys.RightAlt:
                            case Keys.LeftControl:
                            case Keys.RightControl:
                            case Keys.LeftWindows:
                            case Keys.RightWindows:
                            case Keys.F13: case Keys.F14: case Keys.F15: case Keys.F16: case Keys.F17: case Keys.F18: case Keys.F19: case Keys.F20: case Keys.F21: case Keys.F22: case Keys.F23: case Keys.F24:
                                //this._virtualCursor.Print(key.Character.ToString());
                                break;
                            case Keys.Up:
                                this.virtualCursor.Up(1);
                                didSomething = true;
                                break;
                            case Keys.Left:
                                this.virtualCursor.Left(1);
                                didSomething = true;
                                break;
                            case Keys.Right:
                                this.virtualCursor.Right(1);
                                didSomething = true;
                                break;
                            case Keys.Down:
                                this.virtualCursor.Down(1);
                                didSomething = true;
                                break;
                            case Keys.None:
                                break;
                            case Keys.Back:
                                this.virtualCursor.Left(1).Print(" ").Left(1);
                                didSomething = true;
                                break;
                            default:
                                this.virtualCursor.Print(key.Character.ToString());
                                didSomething = true;
                                break;
                        }
                    }
                    else
                    {
                        this.virtualCursor.Print(key.Character.ToString());
                        didSomething = true;
                    }
                }

                return didSomething;
            }

            return handlerResult;
        }



        /// <summary>
        /// Called when the visibility of the console changes.
        /// </summary>
        protected virtual void OnVisibleChanged() { }

        /// <summary>
        /// Called when this console's focus has been lost.
        /// </summary>
        protected virtual void OnFocusLost()
        {
            if (AutoCursorOnFocus == true)
                virtualCursor.IsVisible = false;
        }

        /// <summary>
        /// Called when this console is focused.
        /// </summary>
        protected virtual void OnFocused()
        {
            if (AutoCursorOnFocus == true)
                virtualCursor.IsVisible = true;
        }

        /// <summary>
        /// Called when the <see cref="Position" /> property changes.
        /// </summary>
        /// <param name="oldLocation">The location before the change.</param>
        protected virtual void OnPositionChanged(Point oldLocation) { }

        /// <summary>
        /// Called before the renderer applies a tint color.
        /// </summary>
        /// <param name="batch">The batch used in renderering.</param>
        protected virtual void OnBeforeRenderTint(SpriteBatch batch)
        {
            if (VirtualCursor.IsVisible)
            {
                // Bug - Virtual cursor position index is incorrectly positioned in the render area when the render area
                //       is smaller than width.
                //       Render 

                int virtualCursorLocationIndex = BasicSurface.GetIndexFromPoint(
                    new Point(VirtualCursor.Position.X - TextSurface.RenderArea.Left,
                              VirtualCursor.Position.Y - TextSurface.RenderArea.Top), TextSurface.RenderArea.Width);

                if (virtualCursorLocationIndex >= 0 && virtualCursorLocationIndex < textSurface.RenderRects.Length)
                {
                    VirtualCursor.Render(batch, textSurface.Font, textSurface.RenderRects[virtualCursorLocationIndex]);
                }
            }
        }

        /// <summary>
        /// Called when the renderer renders the text view.
        /// </summary>
        /// <param name="batch">The batch used in renderering.</param>
        protected virtual void OnAfterRender(SpriteBatch batch)
        {
            //if (VirtualCursor.IsVisible)
            //{
            //    int virtualCursorLocationIndex = BasicSurface.GetIndexFromPoint(
            //        new Point(VirtualCursor.Position.X - TextSurface.RenderArea.Left,
            //                  VirtualCursor.Position.Y - TextSurface.RenderArea.Top), TextSurface.RenderArea.Width);

            //    if (virtualCursorLocationIndex >= 0 && virtualCursorLocationIndex < textSurface.RenderRects.Length)
            //    {
            //        VirtualCursor.Render(batch, textSurface.Font, textSurface.RenderRects[virtualCursorLocationIndex]);
            //    }
            //}
        }

        

        /// <summary>
        /// Called when the renderer renders the text view.
        /// </summary>
        /// <param name="batch">The batch used in renderering.</param>
        protected virtual void OnBeforeRender(SpriteBatch batch) { }

        /// <summary>
        /// Updates the cell effects and cursor. Calls Update on <see cref="Children"/>.
        /// </summary>
        /// <param name="delta">Time difference for this frame (if update was called last frame).</param>
        public virtual void Update(TimeSpan delta)
        {
            if (!IsPaused)
            {
                Effects.UpdateEffects(delta.TotalSeconds);

                if (VirtualCursor.IsVisible)
                    VirtualCursor.Update(delta);

                var copyList = new List<IScreen>(Children);

                foreach (var child in copyList)
                    child.Update(delta);
            }
        }

        /// <summary>
        /// The <see cref="Renderer"/> will draw the <see cref="TextSurface"/> and then Add a draw call to <see cref="Global.DrawCalls"/>. Calls Draw on <see cref="Children"/>.
        /// </summary>
        /// <param name="delta">Time difference for this frame (if draw was called last frame).</param>
        public virtual void Draw(TimeSpan delta)
        {
            if (isVisible)
            {
                Renderer.Render(textSurface);

                Global.DrawCalls.Add(new DrawCallSurface(textSurface, calculatedPosition, UsePixelPositioning));

                var copyList = new List<IScreen>(Children);

                foreach (var child in copyList)
                    child.Draw(delta);
            }
        }

        /// <summary>
        /// Called when the parent console changes for this console.
        /// </summary>
        /// <param name="oldParent">The previous parent.</param>
        /// <param name="newParent">The new parent.</param>
        protected virtual void OnParentConsoleChanged(IScreen oldParent, IScreen newParent) { }

        /// <summary>
        /// Called when the parent position changes.
        /// </summary>
        public virtual void OnCalculateRenderPosition()
        {
            calculatedPosition = position;
            IScreen parent = parentConsole;

            while (parent != null)
            {
                calculatedPosition += parent.Position;
                parent = parent.Parent;
            }
            
            foreach (var child in Children)
            {
                child.OnCalculateRenderPosition();
            }
        }

        //#region Serialization
        ///// <summary>
        ///// Saves the <see cref="Console"/> to a file.
        ///// </summary>
        ///// <param name="file">The destination file.</param>
        ///// <param name="saveTextSurface">When false the <see cref="IConsole.TextSurface"/> property will not be serialized.</param>
        ///// <param name="knownTypes">Types to provide if the <see cref="SurfaceEditor.TextSurface"/> and <see cref="Renderer" /> types are custom and unknown to the serializer.</param>
        //public void Save(string file, bool saveTextSurface, params Type[] knownTypes)
        //{
        //    //new Serialized(this, saveTextSurface).Save(file, knownTypes.Union(Serializer.ConsoleTypes).ToArray());
        //    //Serializer.Save(this, file, new Type[] { typeof(Cell) });
        //    serializeTextSurface = saveTextSurface;
        //    Serializer.Save(this, file, knownTypes.Union(Serializer.ConsoleTypes));
        //}

        ///// <summary>
        ///// Loads a <see cref="Console"/> from a file.
        ///// </summary>
        ///// <param name="file">The source file.</param>
        ///// <param name="knownTypes">Types to provide if the <see cref="SurfaceEditor.TextSurface"/> and <see cref="Renderer" /> types are custom and unknown to the serializer.</param>
        ///// <returns>The <see cref="Console"/>.</returns>
        //public static Console Load(string file, params Type[] knownTypes)
        //{
        //    //return Serializer.Load<Console>(file, new Type[] { typeof(Cell) });
        //    //return Serialized.Load(file, knownTypes.Union(Serializer.ConsoleTypes).ToArray());
        //    return Serializer.Load<Console>(file, knownTypes.Union(Serializer.ConsoleTypes));
        //}
        //#endregion

    }
}
