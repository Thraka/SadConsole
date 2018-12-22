using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using System;
using System.Windows;
using System.Runtime.Serialization;
using SadConsole.DrawCalls;
using SadConsole.Renderers;
using SadConsole.Input;
using SadConsole.Themes;

namespace SadConsole
{
    /// <summary>
    /// Represents a windowed controls console.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("Window")]
    public partial class Window: ControlsConsole
    {
        /// <summary>
        /// Raised when the window is closed.
        /// </summary>
        public event EventHandler Closed;

        [DataMember(Name="IsModal")]
        protected bool isModal;

        [DataMember(Name="Title")]
        protected string title;

        [DataMember(Name="Theme")]
        protected Library _theme;
        
        [DataMember(Name="TitleAlignment")]
        protected HorizontalAlignment titleAlignment;

        protected MouseConsoleState previousMouseInfo = new MouseConsoleState(null, new Input.Mouse());
        protected Point cellAtDragPosition;
        protected Point consoleAtDragAbsPos;
        protected bool prevousMouseExclusiveDrag;
        protected bool addedToParent;
        protected bool isDragging;


        /// <summary>
        /// Gets or sets the alignment of the window title.
        /// </summary>
        [DataMember]
        public HorizontalAlignment TitleAlignment
        {
            get => titleAlignment;
            set
            {
                titleAlignment = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the whether or not the console is being shown as modal. 
        /// </summary>
        public bool IsModal => isModal;

        /// <summary>
        /// Gets or sets whether or not this window can be moved with the mouse.
        /// </summary>
        [DataMember]
        public bool Dragable { get; set; }
        
        /// <summary>
        /// Gets or sets whether or not this window can be closed when the escape key is pressed.
        /// </summary>
        [DataMember]
        public bool CloseOnESC { get; set; }
        
        /// <summary>
        /// Gets or set the dialog result status of the window.
        /// </summary>
        [DataMember]
        public bool DialogResult { get; set; }
        
        /// <summary>
        /// Indicates that when this window is shown by the <see cref="Show()"/> method or by setting the <see cref="Console.IsVisible"/> property to true, the window will be shown as modal.
        /// </summary>
        [DataMember]
        public bool ModalIsDefault { get; set; }

        /// <summary>
        /// Gets or sets the title displayed on the window.
        /// </summary>
        public string Title
        {
            get => title;
            set
            {
                title = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the theme of the window.
        /// </summary>
        public new Library Theme
        {
            get => _theme ?? Library.Default;
            set
            {
                _theme = value;
                IsDirty = true;
                Invalidate();
            }
        }
        
        #region Constructors
        public Window(int width, int height)
            : base(width, height)
        {
            IsVisible = false;
            Renderer = new SadConsole.Renderers.Window() { Controls = _controls };
        }
        #endregion

        #region Base Methods
        protected override void OnVisibleChanged()
        {
            base.OnVisibleChanged();

            if (IsVisible)
                Show(ModalIsDefault);
            else
                Hide();
        }

        public override void Draw(TimeSpan drawTime)
        {
            //TODO: Perf - cache reference?
            ((SadConsole.Renderers.Window)Renderer).IsModal = isModal;
            ((SadConsole.Renderers.Window)Renderer).ModalTint = Theme.WindowTheme.ModalTint;

            if (IsModal && Theme.WindowTheme.ModalTint.A != 0)
                Global.DrawCalls.Add(new DrawCallColoredRect(new Rectangle(0, 0, Global.RenderWidth, Global.RenderHeight), Theme.WindowTheme.ModalTint));

            base.Draw(drawTime);
        }

        /// <inheritdoc />
        public new virtual void Invalidate()
        {
            Theme.WindowTheme.Refresh(Theme.Colors);
            Theme.WindowTheme.Draw(this, this);
            IsDirty = true;

            foreach (var control in _controls)
                control.IsDirty = true;
        }

        /// <summary>
        /// Processes the mouse. If allowed, will move the console around with the mouse.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        /// <returns></returns>
        public override bool ProcessMouse(Input.MouseConsoleState state)
        { 
            if (Theme.WindowTheme.TitleAreaLength != 0 && IsVisible)
            {
                if (isDragging && state.Mouse.LeftButtonDown)
                {
                    if (state.Mouse.IsOnScreen)
                    {
                        Position = state.WorldPosition - cellAtDragPosition;

                        return true;
                    }
                }

                // Stopped dragging
                if (isDragging && !state.Mouse.LeftButtonDown)
                {
                    isDragging = false;
                    IsExclusiveMouse = prevousMouseExclusiveDrag;
                    return true;
                }

                // Left button freshly down and we're not already dragging, check to see if in title
                if (CapturedControl == null && state.IsOnConsole && !isDragging && !previousMouseInfo.Mouse.LeftButtonDown && state.Mouse.LeftButtonDown)
                {
                    if (state.CellPosition.Y == Theme.WindowTheme.TitleAreaY && state.CellPosition.X >= Theme.WindowTheme.TitleAreaX && state.CellPosition.X < Theme.WindowTheme.TitleAreaX + Theme.WindowTheme.TitleAreaLength)
                    {
                        prevousMouseExclusiveDrag = IsExclusiveMouse;

                        // Mouse is in the title bar
                        IsExclusiveMouse = true;
                        isDragging = true;
                        consoleAtDragAbsPos = base.Position;

                        if (UsePixelPositioning)
                            cellAtDragPosition = state.RelativePixelPosition;
                        else
                            cellAtDragPosition = state.ConsolePosition;

                        if (this.MoveToFrontOnMouseClick)
                        {
                            IsFocused = true;
                        }
                    }
                }
            }

            return base.ProcessMouse(state);
        }

        /// <summary>
        /// Processes the keyboard looking for the ESC key press to close the console, if required. Otherwise the base ControlsConsole will process the keyboard.
        /// </summary>
        /// <param name="info">Keyboard state.</param>
        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            if (CloseOnESC && info.IsKeyReleased(Keys.Escape))
            {
                this.Hide();
                return true;
            }
            
            return base.ProcessKeyboard(info);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Displays this window using the modal value of the <see cref="ModalIsDefault"/> property.
        /// </summary>
        public void Show()
        {
            Show(ModalIsDefault);
        }

        /// <summary>
        /// Displays this window.
        /// </summary>
        /// <param name="modal">When true, the window will be displayed as modal; otherwise false.</param>
        public virtual void Show(bool modal)
        {
            if (Parent != null && IsVisible)
            {
                Parent.Children.MoveToTop(this);
                return;
            }

            if (IsVisible)
                return;
            else
                IsVisible = true;

            IsExclusiveMouse = isModal = modal;
            addedToParent = false;

            if (Parent == null)
            {
                Parent = Global.CurrentScreen;
                addedToParent = true;
            }

            Parent.Children.MoveToTop(this);

            if (modal)
            {
                Global.FocusedConsoles.Push(this);
                IsFocused = true;
            }
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public virtual void Hide()
        {
            IsVisible = false;
            IsExclusiveMouse = false;

            if (isModal)
                Global.FocusedConsoles.Pop(this);

            if (addedToParent && Parent != null)
                Parent = null;

            Closed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Centers the window within the bounds of <see cref="Global.RenderWidth"/> and <see cref="Global.RenderHeight"/>
        /// </summary>
        public void Center()
        {
            int screenWidth = Global.RenderWidth;
            int screenHeight = Global.RenderHeight;

            if (UsePixelPositioning)
                this.Position = new Point((screenWidth / 2) - ((Width * Font.Size.X) / 2), (screenHeight / 2) - ((Height * Font.Size.Y) / 2));
            else
                this.Position = new Point(((screenWidth / Font.Size.X) / 2) - (Width / 2), ((screenHeight / Font.Size.Y) / 2) - (Height / 2));
            
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            previousMouseInfo = new MouseConsoleState(null, new Input.Mouse());
            //Redraw();
        }
#endregion
    }
}
