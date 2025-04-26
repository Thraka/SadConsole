using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="IScreenSurface"/> with tint. Doesn't allow render steps.
/// </summary>
/// <remarks>
/// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
/// </remarks>
public sealed class OptimizedScreenSurfaceRenderer : IRenderer
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
    /// The blend state used by this renderer.
    /// </summary>
    public BlendMode SFMLBlendState { get; set; } = SadConsole.Host.Settings.SFMLSurfaceBlendMode;

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
    List<IRenderStep> IRenderer.Steps { get; set; } = new();

    /// <summary>
    /// Cached set of rectangles used in rendering each cell.
    /// </summary>
    public IntRect[] CachedRenderRects;

    /// <summary>
    /// Creates a new instance of this renderer with the default steps.
    /// </summary>
    public OptimizedScreenSurfaceRenderer()
    {
    }

    void IRenderer.OnHostUpdated(IScreenObject host)
    {
    }

    ///  <inheritdoc/>
    public void Refresh(IScreenSurface screen, bool force = false)
    {
        IsForced = force;

        // Update texture if something is out of size.
        if (_backingTexture == null || screen.AbsoluteArea.Width != (int)_backingTexture.Size.X || screen.AbsoluteArea.Height != (int)_backingTexture.Size.Y)
        {
            IsForced = true;
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

        // Let everything refresh before compose.
        if (screen.IsDirty || IsForced)
            RedrawSurface(screen);
    }

    ///  <inheritdoc/>
    public void Render(IScreenSurface screenObject) =>
        GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(_backingTexture.Texture, new SFML.System.Vector2i(screenObject.AbsoluteArea.Position.X, screenObject.AbsoluteArea.Position.Y), _finalDrawColor));

    void RedrawSurface(IScreenSurface screenObject)
    {
        _backingTexture.Clear(Color.Transparent);
        Host.Global.SharedSpriteBatch.Reset(_backingTexture, SFMLBlendState, Transform.Identity);

        int rectIndex = 0;
        ColoredGlyphBase cell;
        IFont font = screenObject.Font;

        if (screenObject.Surface.DefaultBackground.A != 0)
            Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(0, 0, (int)_backingTexture.Size.X, (int)_backingTexture.Size.Y), font.SolidGlyphRectangle.ToIntRect(), screenObject.Surface.DefaultBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)font.Image).Texture);

        for (int y = 0; y < screenObject.Surface.View.Height; y++)
        {
            int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

            for (int x = 0; x < screenObject.Surface.View.Width; x++)
            {
                cell = screenObject.Surface[i];

                cell.IsDirty = false;

                if (cell.IsVisible)
                    Host.Global.SharedSpriteBatch.DrawCell(cell, CachedRenderRects[rectIndex], cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground, font);

                i++;
                rectIndex++;
            }
        }

        if (screenObject.Tint.A != 0)
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screenObject.Tint.ToSFMLColor(), ((SadConsole.Host.GameTexture)screenObject.Font.Image).Texture, screenObject.AbsoluteArea.ToIntRect(), screenObject.Font.SolidGlyphRectangle.ToIntRect()));

        Host.Global.SharedSpriteBatch.End();
        _backingTexture.Display();

        screenObject.IsDirty = false;
    }

    #region IDisposable Support
    /// <summary>
    /// Detects redundant calls.
    /// </summary>
    bool disposedValue = false;

    /// <summary>
    /// Release the backing texture and the render texture target.
    /// </summary>
    /// <param name="disposing">Indicates that the managed resources should be cleaned up.</param>
    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            _backingTexture?.Dispose();
            _renderTexture?.Dispose();

            disposedValue = true;
        }

        if (disposing)
        {
            ((IRenderer)this).Steps = null;
            CachedRenderRects = null;
        }
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    ~OptimizedScreenSurfaceRenderer() =>
        Dispose(false);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
         GC.SuppressFinalize(this);
    }
    #endregion
}
