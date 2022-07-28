using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadRogue.Primitives;
using Keyboard = SadConsole.Input.Keyboard;
using Mouse = SadConsole.Input.Mouse;


namespace SadConsole.UI;

/// <summary>
/// Represents a windowed controls console.
/// </summary>
[DataContract]
[System.Diagnostics.DebuggerDisplay("Window")]
public partial class Window : Console
{
    /// <summary>
    /// Raised when the window is closed.
    /// </summary>
    public event EventHandler Closed;

    /// <summary>
    /// Raised when the window is shown.
    /// </summary>
    public event EventHandler Shown;

    /// <summary>
    /// The controls host holding all the controls.
    /// </summary>
    public ControlHost Controls { get; }

    [DataMember(Name = "Title")]
    private string _title = "Window";

    [DataMember(Name = "TitleAlignment")]
    private HorizontalAlignment _titleAlignment = HorizontalAlignment.Center;

    [DataMember(Name = "BorderLineStyle")]
    private int[] _borderLineStyle = ICellSurface.ConnectedLineThin;

    private bool _isVisibleProcessing;

    #region Theme related
    /// <summary>
    /// The Y coordinate of the title drawing area. This can be set to any value > 0 and &lt; the height.
    /// </summary>
    [DataMember]
    public int TitleAreaY { get; set; }

    /// <summary>
    /// The X coordinate of the title drawing area. This is automatically set by the theme.
    /// </summary>
    [DataMember]
    public int TitleAreaX { get; set; }

    /// <summary>
    /// The width of the title drawing area. This is automatically set by the theme.
    /// </summary>
    [DataMember]
    public int TitleAreaLength { get; set; }

    /// <summary>
    /// The line sytle for the border.
    /// </summary>
    [DataMember]
    public int[] BorderLineStyle
    {
        get => _borderLineStyle;
        set
        {
            _borderLineStyle = value;
            DrawBorder();
        }
    }
    #endregion

    /// <summary>
    /// The mouse state of the previous update frame.
    /// </summary>
    protected MouseScreenObjectState PreviousMouseInfo = new MouseScreenObjectState(null, new Mouse());

    /// <summary>
    /// The position of the cell that the window drag started at.
    /// </summary>
    [IgnoreDataMember]
    protected Point CellAtDragPosition { get; set; }

    /// <summary>
    /// Temporary value to hold the state of <see cref="IScreenObject.IsExclusiveMouse"/> prior to dragging.
    /// </summary>
    [IgnoreDataMember]
    protected bool PreviousMouseExclusiveDrag { get; set; }

    /// <summary>
    /// When <see langword="true"/>, indicates that the window has been added to a parent; otherwise <see langword="false"/>.
    /// </summary>
    [IgnoreDataMember]
    protected bool AddedToParent { get; set; }

    /// <summary>
    /// When <see langword="true"/>, indicates that the window is being dragged; otherwise <see langword="false"/>.
    /// </summary>
    [IgnoreDataMember]
    protected bool IsDragging { get; set; }

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
            DrawBorder();
        }
    }

    /// <summary>
    /// Gets the whether or not the window is being shown as modal. 
    /// </summary>
    [IgnoreDataMember]
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
    /// Indicates that when this window is shown by the <see cref="Show()"/> method or by setting the <see cref="IScreenObject.IsVisible"/> property to true, the window will be shown as modal.
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
            DrawBorder();
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
        Controls = new ControlHost();
        SadComponents.Add(Controls);
        RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Window));
        //Renderer = GameHost.Instance.GetRenderer("window");

        // todo: Perhaps a new design with windows.
        // A border surface so that the surface of the window contains just the controls and print code.
        //DrawingArea = Surface.GetSubSurface(Surface.Buffer.WithPosition((1, 1)).Expand(-1, -1));
        DrawBorder();
    }

    /// <summary>
    /// Creates a new window using the existing surface.
    /// </summary>
    /// <param name="surface">The surface.</param>
    /// <param name="font">The font to use with the surface.</param>
    /// <param name="fontSize">The font size.</param>
    public Window(ICellSurface surface, IFont font = null, Point? fontSize = null) : base(surface, font, fontSize)
    {
        _isVisibleProcessing = true;
        IsVisible = false;
        _isVisibleProcessing = false;
        CanDrag = true;
        MoveToFrontOnMouseClick = true;
        Controls = new ControlHost();
        SadComponents.Add(Controls);
        RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Window));
        DrawBorder();
    }


    /// <inheritdoc />
    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (!IsVisible)
            return false;

        if (!CanDrag || TitleAreaLength == 0)
        {
            PreviousMouseInfo = state;
            return base.ProcessMouse(state);
        }

        if (IsDragging && state.Mouse.LeftButtonDown)
        {
            if (state.Mouse.IsOnScreen)
            {
                if (UsePixelPositioning)
                    Position = state.Mouse.ScreenPosition - CellAtDragPosition;
                else
                    Position = state.WorldCellPosition - CellAtDragPosition;

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
        if (Controls.CapturedControl == null && state.IsOnScreenObject && !IsDragging && !PreviousMouseInfo.Mouse.LeftButtonDown && state.Mouse.LeftButtonDown)
        {
            if (state.CellPosition.Y == TitleAreaY && state.CellPosition.X >= TitleAreaX && state.CellPosition.X < TitleAreaX + TitleAreaLength)
            {
                PreviousMouseExclusiveDrag = IsExclusiveMouse;

                // Mouse is in the title bar
                IsExclusiveMouse = true;
                IsDragging = true;

                if (UsePixelPositioning)
                    CellAtDragPosition = state.SurfacePixelPosition;
                else
                    CellAtDragPosition = state.SurfaceCellPosition;

                if (MoveToFrontOnMouseClick)
                    IsFocused = true;
            }
        }

        PreviousMouseInfo = state;
        return base.ProcessMouse(state);
    }

    /// <inheritdoc/>
    protected virtual void DrawBorder()
    {
        var themeColors = Controls.GetThemeColors();

        var fillStyle = new ColoredGlyph(themeColors.ControlHostForeground, themeColors.ControlHostBackground);
        var titleStyle = new ColoredGlyph(themeColors.Title, fillStyle.Background, fillStyle.Glyph);
        var borderStyle = new ColoredGlyph(themeColors.Lines, fillStyle.Background, 0);

        if (BorderLineStyle != null)
            Surface.DrawBox(new Rectangle(0, 0, Width, Height), ShapeParameters.CreateStyledBox(BorderLineStyle, borderStyle));

        // Draw title
        string adjustedText = "";
        int adjustedWidth = Width - 2;
        TitleAreaLength = 0;
        TitleAreaX = 0;

        if (!string.IsNullOrEmpty(Title))
        {
            if (Title.Length > adjustedWidth)
                adjustedText = Title.Substring(0, Title.Length - (Title.Length - adjustedWidth));
            else
                adjustedText = Title;
        }

        if (!string.IsNullOrEmpty(adjustedText))
        {
            TitleAreaLength = adjustedText.Length;

            if (TitleAlignment == HorizontalAlignment.Left)
                TitleAreaX = 1;
            else if (TitleAlignment == HorizontalAlignment.Center)
                TitleAreaX = ((adjustedWidth - adjustedText.Length) / 2) + 1;
            else
                TitleAreaX = Width - 1 - adjustedText.Length;

            Surface.Print(TitleAreaX, TitleAreaY, adjustedText, titleStyle);
        }

        IsDirty = true;
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
            return;

        _isVisibleProcessing = true;

        if (IsVisible)
            Show();
        else
            Hide();

        _isVisibleProcessing = false;
    }

    /// <summary>
    /// User-definable code called when the window is shown.
    /// </summary>
    protected virtual void OnShown() { }

    /// <summary>
    /// User-definable code called when the window is hidden.
    /// </summary>
    protected virtual void OnHidden() { }

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
            return;

        _isVisibleProcessing = true;
        IsVisible = true;

        IsExclusiveMouse = IsModal = modal;
        AddedToParent = false;

        if (Parent == null)
        {
            Parent = GameHost.Instance.Screen;
            AddedToParent = true;
        }

        Parent.Children.MoveToTop(this);

        if (modal)
        {
            GameHost.Instance.FocusedScreenObjects.Push(this);
            IsFocused = true;
            GameHost.Instance.Mouse.ClearLastMouseScreenObject();
        }
        Shown?.Invoke(this, new EventArgs());
        OnShown();
        _isVisibleProcessing = false;
    }

    /// <summary>
    /// Hides the window.
    /// </summary>
    public virtual void Hide()
    {
        if (!IsVisible && !_isVisibleProcessing)
            return;

        _isVisibleProcessing = true;
        IsVisible = false;
        IsExclusiveMouse = false;

        if (IsModal)
            GameHost.Instance.FocusedScreenObjects.Pop(this);

        if (AddedToParent && Parent != null)
            Parent = null;

        Closed?.Invoke(this, new EventArgs());
        OnHidden();
        _isVisibleProcessing = false;
    }

    /// <summary>
    /// Centers the window within the bounds of <see cref="Settings.Rendering.RenderWidth"/> and <see cref="Settings.Rendering.RenderHeight"/>
    /// </summary>
    public void Center()
    {
        int screenWidth = Settings.Rendering.RenderWidth;
        int screenHeight = Settings.Rendering.RenderHeight;

        if (UsePixelPositioning)
            Position = new Point((screenWidth / 2) - ((Width * FontSize.X) / 2), (screenHeight / 2) - ((Height * FontSize.Y) / 2));
        else
            Position = new Point(((screenWidth / FontSize.X) / 2) - (Width / 2), ((screenHeight / FontSize.Y) / 2) - (Height / 2));
    }

    /// <summary>
    /// Returns the value "Window".
    /// </summary>
    /// <returns>The string "Window".</returns>
    public override string ToString() =>
        "Window";

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context) =>
        PreviousMouseInfo = new MouseScreenObjectState(null, new Mouse());//Redraw();
}
