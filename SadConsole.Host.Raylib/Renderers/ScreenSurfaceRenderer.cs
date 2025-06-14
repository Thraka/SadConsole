using System;
using SadRogue.Primitives;
using System.Collections.Generic;
using Raylib_cs;
using Color = SadRogue.Primitives.Color;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="IScreenSurface"/>.
/// </summary>
/// <remarks>
/// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
/// </remarks>
public class ScreenSurfaceRenderer : IRenderer
{
    private Host.GameTexture _renderTexture;

    /// <summary>
    /// Raised when the <see cref="_backingTexture" /> is recreated.
    /// </summary>
    public event EventHandler BackingTextureRecreated;

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <summary>
    /// Quick access to backing texture.
    /// </summary>
    public RenderTexture2D _backingTexture;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public ITexture Output => _renderTexture;

    /// <summary>
    /// Color used with drawing the texture to the screen. Let's a surface become transparent.
    /// </summary>
    public Color _finalDrawColor = Color.White;

    /// <summary>
    /// Render steps to process.
    /// </summary>
    protected List<IRenderStep> RenderSteps = new();

    /// <summary>
    /// The blend state used by this renderer.
    /// </summary>
    public BlendMode BlendState { get; set; } = SadConsole.Host.Settings.SurfaceBlendMode;

    /// <summary>
    /// A 0 to 255 value representing how transparent the surface is when it's drawn to the screen. 255 represents full visibility.
    /// </summary>
    public byte Opacity
    {
        get => _finalDrawColor.A;
        set => _finalDrawColor = new Color(_finalDrawColor.R, _finalDrawColor.G, _finalDrawColor.B, value);
    }

    /// <inheritdoc/>
    public bool IsForced { get; set; }

    /// <inheritdoc/>
    public List<IRenderStep> Steps { get; set; } = new();


    /// <summary>
    /// Cached set of rectangles used in rendering each cell.
    /// </summary>
    public Raylib_cs.Rectangle[] CachedRenderRects;

    /// <summary>
    /// Creates a new instance of this renderer with the default steps.
    /// </summary>
    public ScreenSurfaceRenderer()
    {
        AddDefaultSteps();
        Steps.Sort(RenderStepComparer.Instance);
    }

    ///  <inheritdoc/>
    public virtual void Refresh(IScreenSurface screen, bool force = false)
    {
        bool backingTextureChanged = false;
        IsForced = force;

        // Update texture if something is out of size.
        if (!Raylib.IsRenderTextureValid(_backingTexture) || screen.AbsoluteArea.Width != _backingTexture.Texture.Width || screen.AbsoluteArea.Height != _backingTexture.Texture.Height)
        {
            IsForced = true;
            backingTextureChanged = true;
            if (Raylib.IsRenderTextureValid(_backingTexture))
                Raylib.UnloadRenderTexture(_backingTexture);

            _backingTexture = Raylib.LoadRenderTexture(screen.AbsoluteArea.Width, screen.AbsoluteArea.Height);
            _renderTexture?.Dispose();
            _renderTexture = new Host.GameTexture(_backingTexture.Texture);
            BackingTextureRecreated?.Invoke(this, EventArgs.Empty);
        }

        // Update cached drawing rectangles if something is out of size.
        if (CachedRenderRects == null || CachedRenderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || CachedRenderRects[0].Width != screen.FontSize.X || CachedRenderRects[0].Height != screen.FontSize.Y)
        {
            CachedRenderRects = new Raylib_cs.Rectangle[screen.Surface.View.Width * screen.Surface.View.Height];

            for (int i = 0; i < CachedRenderRects.Length; i++)
            {
                var position = Point.FromIndex(i, screen.Surface.View.Width);
                CachedRenderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToHostRectangle();
            }

            IsForced = true;
        }

        bool composeRequested = IsForced;

        // Let everything refresh before compose.
        foreach (IRenderStep step in Steps)
            composeRequested |= step.Refresh(this, screen, backingTextureChanged, IsForced);

        // If any step (or IsForced) requests a compose, process them.
        if (composeRequested)
        {
            // Setup spritebatch for compose
            Raylib.BeginTextureMode(_backingTexture);
            Raylib.ClearBackground(Color.Transparent.ToHostColor());

            // Compose each step
            foreach (IRenderStep step in Steps)
                step.Composing(this, screen);

            // End sprite batch
            Raylib.EndTextureMode();
        }
    }

    ///  <inheritdoc/>
    public virtual void Render(IScreenSurface screen)
    {
        foreach (IRenderStep step in Steps)
            step.Render(this, screen);
    }

    /// <summary>
    /// Adds the render steps this renderer uses.
    /// </summary>
    protected virtual void AddDefaultSteps()
    {
        Steps.Add(new SurfaceRenderStep());
        Steps.Add(new OutputSurfaceRenderStep());
        Steps.Add(new TintSurfaceRenderStep());
    }

    #region IDisposable Support
    /// <summary>
    /// Detects redundant calls.
    /// </summary>
    protected bool disposedValue = false;

    /// <summary>
    /// Release the backing texture and the render texture target.
    /// </summary>
    /// <param name="disposing">Indicates that the managed resources should be cleaned up.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (Raylib.IsRenderTextureValid(_backingTexture))
                Raylib.UnloadRenderTexture(_backingTexture);

            _renderTexture?.Dispose();

            foreach (IRenderStep step in Steps)
                step.Dispose();

            disposedValue = true;
        }

        if (disposing)
        {
            Steps = null;
            CachedRenderRects = null;
        }
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    ~ScreenSurfaceRenderer() =>
        Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
         GC.SuppressFinalize(this);
    }
    #endregion
}
