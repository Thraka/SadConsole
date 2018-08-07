using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using System;
using System.Windows;
using System.Runtime.Serialization;
using SadConsole.Renderers;
using SadConsole.Input;

namespace SadConsole
{
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
        protected SadConsole.Themes.WindowTheme theme;

        [DataMember(Name = "TitleLocX")]
        protected int titleLocationX;

        [DataMember(Name = "TitleLocY")]
        protected int titleLocationY;

        [DataMember(Name = "TitleWidth")]
        protected int titleWidth;

        [DataMember(Name="TitleAlignment")]
        protected HorizontalAlignment titleAlignment;

        protected MouseConsoleState previousMouseInfo = new MouseConsoleState(null, new Input.Mouse());
        protected Point cellAtDragPosition;
        protected Point consoleAtDragAbsPos;
        protected bool prevousMouseExclusiveDrag;
        protected bool addedToParent;
        protected bool isDragging;

        /// <summary>
        /// Gets the whether or not the console is being shown as modal. 
        /// </summary>
        public bool IsModal { get { return isModal; } }

        /// <summary>
        /// When true, indiciates that the window should be redrawn.
        /// </summary>
        public bool IsDirty { get; set; }
        
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
        /// Indicates that when this window is shown by the <see cref="Show()"/> method or by setting the <see cref="_isVisible"/> property to true, the window will be shown as modal.
        /// </summary>
        [DataMember]
        public bool ModalIsDefault { get; set; }

        /// <summary>
        /// Gets or sets the alignment of the window title.
        /// </summary>
        public HorizontalAlignment TitleAlignment
        {
            get { return titleAlignment; }
            set { titleAlignment = value; Redraw(); }
        }

        /// <summary>
        /// Gets or sets the title displayed on the window.
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                Redraw();
            }
        }

        /// <summary>
        /// Gets or sets the theme of the window.
        /// </summary>
        public SadConsole.Themes.WindowTheme Theme
        {
            get
            {
                if (theme == null)
                    return SadConsole.Themes.Library.Default.WindowTheme;
                else
                    return theme;
            }
            set
            {
                theme = value;
            }
        }
        
        #region Constructors
        public Window(int width, int height)
            : base(width, height)
        {
            //_border = Shapes.Box.GetDefaultBox();
            //_border.Width = width;
            //_border.Height = height;
            IsVisible = false;

            Renderer = new SadConsole.Renderers.Window();

            IsDirty = true;
            Redraw();
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
            ((SadConsole.Renderers.Window)Renderer).ModalTint = Theme.ModalTint;

            base.Draw(drawTime);
        }

        /// <summary>
        /// Processes the mouse. If allowed, will move the console around with the mouse.
        /// </summary>
        /// <param name="state">The mouse state.</param>
        /// <returns></returns>
        public override bool ProcessMouse(Input.MouseConsoleState state)
        { 
            if (titleWidth != 0 && IsVisible)
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
                if (state.IsOnConsole && !isDragging && !previousMouseInfo.Mouse.LeftButtonDown && state.Mouse.LeftButtonDown)
                {
                    if (state.CellPosition.Y == titleLocationY && state.CellPosition.X >= titleLocationX && state.CellPosition.X < titleLocationX + titleWidth)
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
            else
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

            isModal = modal;
            IsVisible = true;
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

            this.IsExclusiveMouse = modal;
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

        /// <summary>
        /// Redraws the border and title of the window.
        /// </summary>
        public virtual void Redraw()
        {
            //DefaultForeground = Theme.FillStyle.Foreground;
            //DefaultBackground = Theme.FillStyle.Background;

            //Clear();

            //ResetBox();
            //Border.Draw(this);

            //// Draw title
            //string adjustedText = "";
            //titleWidth = 0;
            //titleLocationX = 0;
            //int adjustedWidth = Width - 2;

            //if (!string.IsNullOrEmpty(title))
            //{
            //    if (title.Length > adjustedWidth)
            //        adjustedText = title.Substring(0, title.Length - (title.Length - adjustedWidth));
            //    else
            //        adjustedText = title;
            //}

            //if (!string.IsNullOrEmpty(adjustedText))
            //{
            //    titleWidth = adjustedText.Length;

            //    if (titleAlignment == HorizontalAlignment.Left)
            //        titleLocationX = 1;

            //    else if (titleAlignment == HorizontalAlignment.Center)
            //        titleLocationX = ((adjustedWidth - adjustedText.Length) / 2) + 1;

            //    else
            //        titleLocationX = Width - 1 - adjustedText.Length;

            //    Print(titleLocationX, titleLocationY, adjustedText, Theme.TitleStyle);
            //}
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
