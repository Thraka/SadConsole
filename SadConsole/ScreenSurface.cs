using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole.Components;
using SadConsole.Renderers;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace SadConsole;

/// <summary>
/// An object that renders a <see cref="ICellSurface"/>.
/// </summary>
[DataContract]
[JsonObject(memberSerialization: MemberSerialization.OptIn)]
public partial class ScreenSurface : ScreenObject, IDisposable, IScreenSurface, ISurfaceSettable
{
    [DataMember(Name = "Font")]
    [JsonConverter(typeof(SerializedTypes.FontJsonConverter))]
    private IFont _font;
    [DataMember(Name = "FontSize")]
    private Point _fontSize;
    [DataMember(Name = "Tint")]
    private Color _tint = Color.Transparent;
    [DataMember(Name = "UsePixelPositioning")]
    private bool _usePixelPositioning;
    [DataMember(Name = "Surface")]
    private ICellSurface _surface;

    private bool _quietSurface;

    private Renderers.IRenderer? _renderer;

    /// <inheritdoc/>
    public bool ForceRendererRefresh { get; set; }

    /// <inheritdoc/>
    public virtual string DefaultRendererName { get; } = Renderers.Constants.RendererNames.Default;

    /// <inheritdoc/>
    public Renderers.IRenderer? Renderer
    {
        get => _renderer;
        protected set
        {
            if (_renderer == value) return;

            _renderer = value;

            OnRendererChanged();
            IsDirty = true;
        }
    }

    /// <summary>
    /// When <see langword="true"/>, prevents the <see cref="Surface"/> property from raising events and virtual methods when the surface changes.
    /// </summary>
    public bool QuietSurfaceHandling
    {
        get => _quietSurface;
        set
        {
            _quietSurface = value;

            if (_surface != null)
            {
                if (value)
                    _surface.IsDirtyChanged -= _isDirtyChangedEventHadler;
                else
                    _surface.IsDirtyChanged += _isDirtyChangedEventHadler;
            }
        }
    }

    /// <inheritdoc/>
    public List<IRenderStep> RenderSteps { get; } = new List<IRenderStep>();

    /// <summary>
    /// The surface this screen object represents.
    /// </summary>
    public ICellSurface Surface
    {
        get => _surface;
        set
        {
            ICellSurface old = _surface;

            _surface = value ?? throw new NullReferenceException("Surface cannot be set to null.");

            if (!_quietSurface)
            {
                old.IsDirtyChanged -= _isDirtyChangedEventHadler;
                _surface.IsDirtyChanged += _isDirtyChangedEventHadler;

                OnSurfaceChanged(old);
                CallOnHostUpdated();
            }
        }
    }

    /// <summary>
    /// When <see langword="true"/>, indicates that the <see cref="Surface"/> needs to be redrawn; otherwise <see langword="false"/>.
    /// </summary>
    public bool IsDirty
    {
        get => _surface.IsDirty;
        set => _surface.IsDirty = value;
    }

    /// <inheritdoc/>
    public IFont Font
    {
        get => _font;
        set
        {
            if (_font == value) return;

            _font = value;
            FontSize = _font.GetFontSize(IFont.Sizes.One);
            IsDirty = true;
        }
    }

    /// <inheritdoc/>
    public Point FontSize
    {
        get => _fontSize;
        set
        {
            if (_fontSize == value) return;

            _fontSize = value;
            IsDirty = true;
        }
    }

    /// <inheritdoc/>
    public Color Tint
    {
        get => _tint;
        set
        {
            _tint = value;
            IsDirty = true;
        }
    }

    /// <inheritdoc/>
    public Rectangle AbsoluteArea => new Rectangle(AbsolutePosition.X, AbsolutePosition.Y, WidthPixels, HeightPixels);

    /// <inheritdoc/>
    public bool UsePixelPositioning
    {
        get => _usePixelPositioning;
        set
        {
            _usePixelPositioning = value;
            IsDirty = true;
            UpdateAbsolutePosition();
        }
    }

    /// <inheritdoc/>
    public int WidthPixels => Surface.View.Width * FontSize.X;


    /// <inheritdoc/>
    public int HeightPixels => Surface.View.Height * FontSize.Y;

    /// <summary>
    /// The total width of the console.
    /// </summary>
    public int Width => Surface.Width;


    /// <summary>
    /// The total height of the console.
    /// </summary>
    public int Height => Surface.Height;

    /// <summary>
    /// Gets or sets the visible width of the surface in cells.
    /// </summary>
    public int ViewWidth { get => Surface.ViewWidth; set => Surface.ViewWidth = value; }

    /// <summary>
    /// Gets or sets the visible height of the surface in cells.
    /// </summary>
    public int ViewHeight { get => Surface.ViewHeight; set => Surface.ViewHeight = value; }

    /// <summary>
    /// The position of the view within the console.
    /// </summary>
    public Point ViewPosition { get => Surface.ViewPosition; set => Surface.ViewPosition = value; }


    /// <summary>
    /// Creates a new screen object that can render a surface.
    /// </summary>
    /// <param name="width">The width in cells of the surface.</param>
    /// <param name="height">The height in cells of the surface.</param>
    public ScreenSurface(int width, int height) : this(width, height, width, height, null)
    {

    }

    /// <summary>
    /// Creates a new screen object that can render a surface. Uses the specified cells to generate the surface.
    /// </summary>
    /// <param name="width">The width in cells of the surface.</param>
    /// <param name="height">The height in cells of the surface.</param>
    /// <param name="initialCells">The initial cells to seed the surface.</param>
    public ScreenSurface(int width, int height, ColoredGlyph[] initialCells) : this(width, height, width, height, initialCells)
    {

    }

    /// <summary>
    /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
    /// </summary>
    /// <param name="viewWidth">The visible width of the surface in cells.</param>
    /// <param name="viewHeight">The visible height of the surface in cells.</param>
    /// <param name="totalWidth">The total width of the surface in cells.</param>
    /// <param name="totalHeight">The total height of the surface in cells.</param>
    public ScreenSurface(int viewWidth, int viewHeight, int totalWidth, int totalHeight) : this(viewWidth, viewHeight, totalWidth, totalHeight, null)
    {

    }

    /// <summary>
    /// Creates a new surface from a grid view. The cells between this object and the grid view are shared.
    /// </summary>
    /// <param name="surface">The surface to use as the source of cells.</param>
    /// <param name="visibleWidth">Optional view width. If <c>0</c>, the view width matches the width of the surface.</param>
    /// <param name="visibleHeight">Optional view height. If <c>0</c>, the view width matches the height of the surface.</param>
    public ScreenSurface(IGridView<ColoredGlyph> surface, int visibleWidth = 0, int visibleHeight = 0) :
        this(new CellSurface(surface, visibleWidth, visibleHeight))
    {

    }



    /// <summary>
    /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
    /// </summary>
    /// <param name="viewWidth">The width of the surface in cells to show.</param>
    /// <param name="viewHeight">The height of the surface in cells to show.</param>
    /// <param name="totalWidth">The total width of the surface in cells.</param>
    /// <param name="totalHeight">The total height of the surface in cells.</param>
    /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
    public ScreenSurface(int viewWidth, int viewHeight, int totalWidth, int totalHeight, ColoredGlyph[]? initialCells):
        this(new CellSurface(viewWidth, viewHeight, totalWidth, totalHeight, initialCells), GameHost.Instance.DefaultFont, GameHost.Instance.DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize))
    {
    }

    /// <summary>
    /// Creates a new screen object wrapping an existing surface.
    /// </summary>
    /// <param name="surface">The surface.</param>
    /// <param name="font">The font to use with the surface.</param>
    /// <param name="fontSize">The font size.</param>
    [JsonConstructor]
    public ScreenSurface(ICellSurface surface, IFont? font = null, Point? fontSize = null)
    {
        _surface = surface;
        _surface.IsDirtyChanged += _isDirtyChangedEventHadler;

        _font = font ?? GameHost.Instance.DefaultFont;
        FontSize = fontSize ?? _font.GetFontSize(GameHost.Instance.DefaultFontSize);

        Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
        RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Surface));
        RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Output));
        RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Tint));
        RenderSteps.Sort(RenderStepComparer.Instance);
    }

    /// <summary>
    /// Resizes the surface to the specified width and height.
    /// </summary>
    /// <param name="width">The viewable width of the surface.</param>
    /// <param name="height">The viewable height of the surface.</param>
    /// <param name="bufferWidth">The maximum width of the surface.</param>
    /// <param name="bufferHeight">The maximum height of the surface.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="ICellSurface.DefaultForeground"/>, <see cref="ICellSurface.DefaultBackground"/> and glyph 0.</param>
    public void Resize(int width, int height, int bufferWidth, int bufferHeight, bool clear)
    {
        if (Surface is ICellSurfaceResize surface)
            surface.Resize(width, height, bufferWidth, bufferHeight, clear);
        else
            throw new Exception("Surface doesn't support resize.");
    }

    /// <summary>
    /// Resizes the surface and view to the specified width and height.
    /// </summary>
    /// <param name="width">The viewable width of the surface.</param>
    /// <param name="height">The viewable height of the surface.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="ICellSurface.DefaultForeground"/>, <see cref="ICellSurface.DefaultBackground"/> and glyph 0.</param>
    public void Resize(int width, int height, bool clear)
    {
        if (Surface is ICellSurfaceResize surface)
            surface.Resize(width, height, clear);
        else
            throw new Exception("Surface doesn't support resize.");
    }

    /// <inheritdoc />
    public override void UpdateAbsolutePosition()
    {
        if (UsePixelPositioning)
            AbsolutePosition = Position + (Parent?.AbsolutePosition ?? Point.Zero);
        else
            AbsolutePosition = (FontSize * Position) + (Parent?.AbsolutePosition ?? Point.Zero);

        int count = Children.Count;
        for (int i = 0; i < count; i++)
            Children[i].UpdateAbsolutePosition();
    }

    /// <summary>
    /// Draws the <see cref="Surface"/> and all <see cref="ScreenObject.SadComponents"/> and <see cref="ScreenObject.Children"/>.
    /// </summary>
    /// <param name="delta">The time that has elapsed since the last call.</param>
    /// <remarks>Only processes if <see cref="ScreenObject.IsVisible"/> is <see langword="true"/>.</remarks>
    public override void Render(TimeSpan delta)
    {
        if (!IsVisible) return;

        if (_renderer != null)
        {
            _renderer.Refresh(this, ForceRendererRefresh);
            _renderer.Render(this);
            ForceRendererRefresh = false;
        }

        int count = ComponentsRender.Count;
        for (int i = 0; i < count; i++)
            ComponentsRender[i].Render(this, delta);

        Children.IsLocked = true;
        count = Children.Count;
        for (int i = 0; i < count; i++)
            Children[i].Render(delta);
        Children.IsLocked = false;
    }

    /// <summary>
    /// Updates the <see cref="Surface"/> effects and all <see cref="ScreenObject.SadComponents"/> and <see cref="ScreenObject.Children"/>.
    /// </summary>
    /// <param name="delta">The time that has elapsed since this method was last called.</param>
    /// <remarks>Only processes if <see cref="ScreenObject.IsEnabled"/> is <see langword="true"/>.</remarks>
    public override void Update(TimeSpan delta)
    {
        if (!IsEnabled) return;

        Surface.Effects.UpdateEffects(delta);

        if (ComponentsUpdate.Count > 0)
        {
            IComponent[] array = ComponentsUpdate.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                IComponent component = array[i];
                component.Update(this, delta);
            }
        }

        IScreenObject[] tempChildren = Children.ToArray();
        for (int i1 = 0; i1 < tempChildren.Length; i1++)
        {
            IScreenObject child = tempChildren[i1];
            child.Update(delta);
        }
    }

    private void _isDirtyChangedEventHadler(object? sender, EventArgs e) =>
        OnIsDirtyChanged();

    /// <summary>
    /// Called when the <see cref="IsDirty"/> property changes.
    /// </summary>
    protected virtual void OnIsDirtyChanged() { }

    /// <summary>
    /// Called when the <see cref="Font"/> or <see cref="FontSize"/> property changes.
    /// </summary>
    /// <param name="oldFont">The font prior to the change.</param>
    /// <param name="oldFontSize">The font size prior to the change.</param>
    protected virtual void OnFontChanged(IFont oldFont, Point oldFontSize) { }


    /// <summary>
    /// Called when the <see cref="Surface"/> property is changed.
    /// </summary>
    /// <param name="oldSurface">The previous surface.</param>
    protected virtual void OnSurfaceChanged(ICellSurface oldSurface) { }


    /// <summary>
    /// Called when the <see cref="Renderer"/> property is changed.
    /// </summary>
    protected virtual void OnRendererChanged() { }

    /// <summary>
    /// Returns the value "ScreenSurface".
    /// </summary>
    /// <returns>The string "ScreenSurface".</returns>
    public override string ToString() =>
        "ScreenSurface";

    /// <summary>
    /// Calls the OnHostUpdated method on components, renderer, and rendersteps.
    /// </summary>
    public void CallOnHostUpdated()
    {
        int count = SadComponents.Count;
        for (int i = 0; i < count; i++)
            SadComponents[i].OnHostUpdated(this);

        Renderer?.OnHostUpdated(this);

        foreach (IRenderStep step in RenderSteps)
            step.OnHostUpdated(this);
    }

    /// <inheritdoc/>
    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
        UpdateAbsolutePosition();
    }

    #region IDisposable Support
    private bool _disposedValue = false; // To detect redundant calls

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects).
            }

            _renderer?.Dispose();
            _renderer = null;

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes <see cref="Renderer"/>.
    /// </summary>
    ~ScreenSurface() =>
        Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
