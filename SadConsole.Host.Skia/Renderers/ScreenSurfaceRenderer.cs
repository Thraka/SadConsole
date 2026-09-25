using System;
using System.Collections.Generic;
using SadConsole.Host;
using SadRogue.Primitives;
using SkiaSharp;
using Color = SadRogue.Primitives.Color;

namespace SadConsole.Renderers;

/// <summary>
/// Draws an <see cref="IScreenSurface"/> by composing a collection of render steps.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Surface")]
public class ScreenSurfaceRenderer : IRenderer
{
    private GameTexture? _backingTexture;
    private Color _finalDrawColor = Color.White;
    private int _cachedViewWidth;
    private int _cachedViewHeight;

    /// <summary>
    /// Raised when the final backing texture is recreated.
    /// </summary>
    public event EventHandler? BackingTextureRecreated;

    /// <inheritdoc />
    public string Name { get; set; } = Constants.RendererNames.Default;

    /// <inheritdoc />
    public ITexture Output => BackingTexture;

    /// <inheritdoc />
    public byte Opacity
    {
        get => _finalDrawColor.A;
        set => _finalDrawColor = new Color(_finalDrawColor.R, _finalDrawColor.G, _finalDrawColor.B, value);
    }

    /// <inheritdoc />
    public bool IsForced { get; set; }

    /// <inheritdoc />
    public List<IRenderStep> Steps { get; set; } = new();

    /// <summary>
    /// Cached destination rectangles for the cells in the visible surface.
    /// </summary>
    public SKRect[] CachedRenderRects { get; protected set; } = Array.Empty<SKRect>();

    internal GameTexture BackingTexture =>
        _backingTexture ?? throw new InvalidOperationException("The renderer has not been refreshed.");

    internal SKCanvas? CompositionCanvas { get; private set; }

    internal Color FinalDrawColor => _finalDrawColor;

    /// <summary>
    /// Creates a renderer with the default surface, output, and tint steps.
    /// </summary>
    public ScreenSurfaceRenderer()
    {
        AddDefaultSteps();
        Steps.Sort(RenderStepComparer.Instance);
    }

    /// <inheritdoc />
    public virtual void Refresh(IScreenSurface screen, bool force = false)
    {
        ArgumentNullException.ThrowIfNull(screen);

        bool backingTextureChanged = false;
        IsForced = force;

        if (_backingTexture is null ||
            _backingTexture.Width != screen.AbsoluteArea.Width ||
            _backingTexture.Height != screen.AbsoluteArea.Height)
        {
            IsForced = true;
            backingTextureChanged = true;
            _backingTexture?.Dispose();
            _backingTexture = new GameTexture(screen.AbsoluteArea.Width, screen.AbsoluteArea.Height);
            BackingTextureRecreated?.Invoke(this, EventArgs.Empty);
        }

        if (CachedRenderRects.Length == 0 ||
            _cachedViewWidth != screen.Surface.View.Width ||
            _cachedViewHeight != screen.Surface.View.Height ||
            CachedRenderRects[0].Width != screen.FontSize.X ||
            CachedRenderRects[0].Height != screen.FontSize.Y)
        {
            _cachedViewWidth = screen.Surface.View.Width;
            _cachedViewHeight = screen.Surface.View.Height;
            CachedRenderRects = new SKRect[_cachedViewWidth * _cachedViewHeight];

            for (int i = 0; i < CachedRenderRects.Length; i++)
            {
                Point position = Point.FromIndex(i, _cachedViewWidth);
                CachedRenderRects[i] = SKRect.Create(
                    position.X * screen.FontSize.X,
                    position.Y * screen.FontSize.Y,
                    screen.FontSize.X,
                    screen.FontSize.Y);
            }

            IsForced = true;
        }

        bool composeRequested = IsForced;

        foreach (IRenderStep step in Steps)
            composeRequested |= step.Refresh(this, screen, backingTextureChanged, IsForced);

        if (!composeRequested)
            return;

        SKCanvas canvas = BackingTexture.BeginDraw();
        canvas.Clear(SKColors.Transparent);
        CompositionCanvas = canvas;

        try
        {
            foreach (IRenderStep step in Steps)
                step.Composing(this, screen);
        }
        finally
        {
            BackingTexture.EndDraw();
            CompositionCanvas = null;
        }
    }

    /// <inheritdoc />
    public virtual void Render(IScreenSurface screen)
    {
        foreach (IRenderStep step in Steps)
            step.Render(this, screen);
    }

    /// <summary>
    /// Adds the default render steps.
    /// </summary>
    protected virtual void AddDefaultSteps()
    {
        Steps.Add(new SurfaceRenderStep());
        Steps.Add(new OutputSurfaceRenderStep());
        Steps.Add(new TintSurfaceRenderStep());
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _backingTexture?.Dispose();
        _backingTexture = null;

        foreach (IRenderStep step in Steps)
            step.Dispose();

        Steps.Clear();
        CachedRenderRects = Array.Empty<SKRect>();
    }
}
