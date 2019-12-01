using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;
using Keyboard = SadConsole.Input.Keyboard;
using Mouse = SadConsole.Input.Mouse;


namespace SadConsole.UI
{
    /// <summary>
    /// Represents a windowed controls console.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("Window")]
    public partial class Window : ControlsConsole
    {
        /// <summary>
        /// Raised when the window is closed.
        /// </summary>
        public event EventHandler Closed;

        [DataMember(Name = "Title")]
        private string _title;

        [DataMember(Name = "TitleAlignment")]
        private HorizontalAlignment _titleAlignment;

        private bool _isVisibleProcessing;

        /// <summary>
        /// The mouse state of the previous update frame.
        /// </summary>
        protected MouseScreenObjectState PreviousMouseInfo = new MouseScreenObjectState(null, new Mouse());

        /// <summary>
        /// The position of the cell that the window drag started at.
        /// </summary>
        protected Point CellAtDragPosition;

        /// <summary>
        /// Temporary value to hold the state of <see cref="ScreenSurface.IsExclusiveMouse"/> prior to dragging.
        /// </summary>
        protected bool PreviousMouseExclusiveDrag;

        /// <summary>
        /// When <see langword="true"/>, indicates that the window has been added to a parent; otherwise <see langword="false"/>.
        /// </summary>
        protected bool AddedToParent;

        /// <summary>
        /// When <see langword="true"/>, indicates that the window is being dragged; otherwise <see langword="false"/>.
        /// </summary>
        protected bool IsDragging;

        /// <summary>
        /// Gets or sets the alignment of the window title.
        /// </summary>
        [DataMember]
        public HorizontalAlignment TitleAlignment
        {
            get => _titleAlignment;
            set
            {
                _titleAlignment = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Gets the whether or not the window is being shown as modal. 
        /// </summary>
        public bool IsModal { get; private set; }

        /// <summary>
        /// Gets or sets whether or not this window can be moved with the mouse.
        /// </summary>
        [DataMember]
        public bool CanDrag { get; set; }

        /// <summary>
        /// Gets or sets whether or not this window can be closed when the escape key is pressed.
        /// </summary>
        [DataMember]
        public bool CloseOnEscKey { get; set; }

        /// <summary>
        /// Gets or set the dialog result status of the window.
        /// </summary>
        [DataMember]
        public bool DialogResult { get; set; }

        /// <summary>
        /// Indicates that when this window is shown by the <see cref="Show()"/> method or by setting the <see cref="Console.IsVisible"/> property to true, the window will be shown as modal.
        /// </summary>
        [DataMember]
        public bool IsModalDefault { get; set; }

        /// <summary>
        /// Gets or sets the title displayed on the window.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Creates a new window.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        public Window(int width, int height) : this(width, height, width, height, null) { }

        /// <summary>
        /// Creates a new screen object that can render a surface. Uses the specified cells to generate the surface.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        /// <param name="initialCells">The initial cells to seed the surface.</param>
        public Window(int width, int height, ColoredGlyph[] initialCells) : this(width, height, width, height, initialCells) { }

        /// <summary>
        /// Creates a new window with the specified width and height, with <see cref="SadRogue.Primitives.Color.Transparent"/> for the background and <see cref="SadRogue.Primitives.Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The visible width of the window in cells.</param>
        /// <param name="height">The visible height of the window in cells.</param>
        /// <param name="bufferWidth">The total width of the window in cells.</param>
        /// <param name="bufferHeight">The total height of the window in cells.</param>
        public Window(int width, int height, int bufferWidth, int bufferHeight) : this(width, height, bufferWidth, bufferHeight, null) { }

        /// <summary>
        /// Creates a new window using the specified surface's cells.
        /// </summary>
        /// <param name="surface">The surface.</param>
        public Window(CellSurface surface) : this(surface.View.Width, surface.View.Height, surface.BufferWidth, surface.BufferHeight, surface.Cells) { }

        /// <summary>
        /// Creates a window with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the window in cells.</param>
        /// <param name="height">The height of the window in cells.</param>
        /// <param name="bufferWidth">The total width of the window in cells.</param>
        /// <param name="bufferHeight">The total height of the window in cells.</param>
        /// <param name="initialCells">The cells to seed the window with. If <see langword="null"/>, creates the cells for you.</param>
        public Window(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[] initialCells) : base(width, height, bufferWidth, bufferHeight, initialCells)
        {
            _isVisibleProcessing = true;
            IsVisible = false;
            _isVisibleProcessing = false;
            CanDrag = true;
            MoveToFrontOnMouseClick = true;
        }

        /// <summary>
        /// Causes the window to be redrawn with the selected <see cref="Theme"/>. When overridden, call this method first.
        /// </summary>
        public override void RedrawTheme()
        {
            Theme?.Draw(this);
            OnThemeDrawn();

            foreach (ControlBase control in ControlsList)
                control.IsDirty = true;
        }

        /// <inheritdoc />
        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            if (!IsVisible)
            {
                return false;
            }

            var theme = Theme as Themes.Window;

            if (theme == null)
                return false;

            if (!CanDrag || theme.TitleAreaLength == 0)
            {
                PreviousMouseInfo = state;
                return base.ProcessMouse(state);
            }

            if (IsDragging && state.Mouse.LeftButtonDown)
            {
                if (state.Mouse.IsOnScreen)
                {
                    if (UsePixelPositioning)
                    {
                        Position = state.Mouse.ScreenPosition - CellAtDragPosition;
                    }
                    else
                    {
                        Position = state.WorldCellPosition - CellAtDragPosition;
                    }

                    PreviousMouseInfo = state;
                    return true;
                }
            }

            // Stopped dragging
            if (IsDragging && !state.Mouse.LeftButtonDown)
            {
                IsDragging = false;
                IsExclusiveMouse = PreviousMouseExclusiveDrag;
                PreviousMouseInfo = state;
                return true;
            }

            // Left button freshly down and we're not already dragging, check to see if in title
            if (CapturedControl == null && state.IsOnScreenObject && !IsDragging && !PreviousMouseInfo.Mouse.LeftButtonDown && state.Mouse.LeftButtonDown)
            {
                if (state.CellPosition.Y == theme.TitleAreaY && state.CellPosition.X >= theme.TitleAreaX && state.CellPosition.X < theme.TitleAreaX + theme.TitleAreaLength)
                {
                    PreviousMouseExclusiveDrag = IsExclusiveMouse;

                    // Mouse is in the title bar
                    IsExclusiveMouse = true;
                    IsDragging = true;

                    if (UsePixelPositioning)
                    {
                        CellAtDragPosition = state.SurfacePixelPosition;
                    }
                    else
                    {
                        CellAtDragPosition = state.SurfaceCellPosition;
                    }

                    if (MoveToFrontOnMouseClick)
                    {
                        IsFocused = true;
                    }
                }
            }

            PreviousMouseInfo = state;
            return base.ProcessMouse(state);
        }

        /// <summary>
        /// Processes the keyboard looking for the ESC key press to close the window, if required. Otherwise the base ControlsConsole will process the keyboard.
        /// </summary>
        /// <param name="info">Keyboard state.</param>
        public override bool ProcessKeyboard(Keyboard info)
        {
            if (CloseOnEscKey && info.IsKeyReleased(Keys.Escape))
            {
                Hide();
                return true;
            }

            return base.ProcessKeyboard(info);
        }

        /// <summary>
        /// Depending on if the window is visible, calls <see cref="Show(bool)"/> or <see cref="Hide"/>.
        /// </summary>
        protected override void OnVisibleChanged()
        {
            base.OnVisibleChanged();

            if (_isVisibleProcessing)
            {
                return;
            }

            _isVisibleProcessing = true;
            if (IsVisible)
            {
                Show();
            }
            else
            {
                Hide();
            }

            _isVisibleProcessing = false;
        }

        /// <summary>
        /// Displays this window using the modal value of the <see cref="IsModalDefault"/> property.
        /// </summary>
        public void Show() => Show(IsModalDefault);

        /// <summary>
        /// Displays this window.
        /// </summary>
        /// <param name="modal">When true, the window will be displayed as modal; otherwise false.</param>
        public virtual void Show(bool modal)
        {
            IsDirty = true;

            if (Parent != null && IsVisible)
            {
                Parent.Children.MoveToTop(this);
                return;
            }

            if (IsVisible && !_isVisibleProcessing)
            {
                return;
            }

            _isVisibleProcessing = true;
            IsVisible = true;

            IsExclusiveMouse = IsModal = modal;
            AddedToParent = false;

            if (Parent == null)
            {
                Parent = Global.Screen;
                AddedToParent = true;
            }

            Parent.Children.MoveToTop(this);

            if (modal)
            {
                Global.FocusedScreenObjects.Push(this);
                IsFocused = true;
                Global.Mouse.ClearLastMouseScreenObject();
            }
            _isVisibleProcessing = false;
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public virtual void Hide()
        {
            if (!IsVisible && !_isVisibleProcessing)
            {
                return;
            }

            _isVisibleProcessing = true;
            IsVisible = false;
            IsExclusiveMouse = false;

            if (IsModal)
            {
                Global.FocusedScreenObjects.Pop(this);
            }

            if (AddedToParent && Parent != null)
            {
                Parent = null;
            }

            Closed?.Invoke(this, new EventArgs());
            _isVisibleProcessing = false;
        }

        /// <summary>
        /// Centers the window within the bounds of <see cref="Global.RenderWidth"/> and <see cref="Global.RenderHeight"/>
        /// </summary>
        public void Center()
        {
            int screenWidth = Settings.Rendering.RenderWidth;
            int screenHeight = Settings.Rendering.RenderHeight;

            if (UsePixelPositioning)
            {
                Position = new Point((screenWidth / 2) - ((Width * FontSize.X) / 2), (screenHeight / 2) - ((Height * FontSize.Y) / 2));
            }
            else
            {
                Position = new Point(((screenWidth / FontSize.X) / 2) - (Width / 2), ((screenHeight / FontSize.Y) / 2) - (Height / 2));
            }
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context) =>
            PreviousMouseInfo = new MouseScreenObjectState(null, new Mouse());//Redraw();
    }
}
