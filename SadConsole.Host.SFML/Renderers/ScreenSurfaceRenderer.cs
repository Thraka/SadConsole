using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;
using System.Collections.Generic;

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
    public RenderTexture _backingTexture;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public ITexture Output => _renderTexture;

    /// <summary>
    /// Color used with drawing the texture to the screen. Let's a surface become transparent.
    /// </summary>
    public Color _finalDrawColor = SadRogue.Primitives.Color.White.ToSFMLColor();

    /// <summary>
    /// Render steps to process.
    /// </summary>
    protected List<IRenderStep> RenderSteps = new List<IRenderStep>();

    /// <summary>
    /// The blend state used by this renderer.
    /// </summary>
    public BlendMode SFMLBlendState { get; set; } = SadConsole.Host.Settings.SFMLSurfaceBlendMode;

    /// <summary>
    /// A 0 to 255 value represening how transparent the surface is when it's drawn to the screen. 255 represents full visibility.
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
    public IntRect[] CachedRenderRects;

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
        if (_backingTexture == null || screen.AbsoluteArea.Width != (int)_backingTexture.Size.X || screen.AbsoluteArea.Height != (int)_backingTexture.Size.Y)
        {
            IsForced = true;
            backingTextureChanged = true;
            _backingTexture?.Dispose();
            _backingTexture = new RenderTexture((uint)screen.AbsoluteArea.Width, (uint)screen.AbsoluteArea.Height);
            _renderTexture?.Dispose();
            _renderTexture = new Host.GameTexture(_backingTexture.Texture);
            BackingTextureRecreated?.Invoke(this, EventArgs.Empty);
        }

        // Update cached drawing rectangles if something is out of size.
        if (CachedRenderRects == null || CachedRenderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || CachedRenderRects[0].Width != screen.FontSize.X || CachedRenderRects[0].Height != screen.FontSize.Y)
        {
            CachedRenderRects = new IntRect[screen.Surface.View.Width * screen.Surface.View.Height];

            for (int i = 0; i < CachedRenderRects.Length; i++)
            {
                var position = Point.FromIndex(i, screen.Surface.View.Width);
                CachedRenderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToIntRect();
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
            _backingTexture.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Reset(_backingTexture, SFMLBlendState, Transform.Identity);

            // Compose each step
            foreach (IRenderStep step in Steps)
                step.Composing(this, screen);

            // End sprite batch
            Host.Global.SharedSpriteBatch.End();
            _backingTexture.Display();
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
    protected bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            foreach (var item in RenderSteps)
                item.Dispose();

            disposedValue = true;
        }
    }

    ~ScreenSurfaceRenderer() =>
        Dispose(false);

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        Dispose(true);
         GC.SuppressFinalize(this);
    }
    #endregion
}
