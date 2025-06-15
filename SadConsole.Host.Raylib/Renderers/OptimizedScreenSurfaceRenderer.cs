using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using SadRogue.Primitives;
using HostColor = Raylib_cs.Color;
using Color = SadRogue.Primitives.Color;

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
    public RenderTexture2D _backingTexture;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public ITexture Output => _renderTexture;

    /// <summary>
    /// Color used with drawing the texture to the screen. Let's a surface become transparent.
    /// </summary>
    public HostColor _finalDrawColor = SadRogue.Primitives.Color.White.ToHostColor();

    /// <summary>
    /// The blend state used by this renderer.
    /// </summary>
    public BlendMode HostBlendState { get; set; } = SadConsole.Host.Settings.SurfaceBlendMode;

    /// <summary>
    /// A 0 to 255 value representing how transparent the surface is when it's drawn to the screen. 255 represents full visibility.
    /// </summary>
    public byte Opacity
    {
        get => _finalDrawColor.A;
        set => _finalDrawColor = new HostColor(_finalDrawColor.R, _finalDrawColor.G, _finalDrawColor.B, value);
    }

    /// <inheritdoc/>
    public bool IsForced { get; set; }

    /// <inheritdoc/>
    List<IRenderStep> IRenderer.Steps { get; set; } = new();

    /// <summary>
    /// Cached set of rectangles used in rendering each cell.
    /// </summary>
    public Raylib_cs.Rectangle[] CachedRenderRects;

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
        if (!Raylib.IsRenderTextureValid(_backingTexture) || screen.AbsoluteArea.Width != _backingTexture.Texture.Width || screen.AbsoluteArea.Height != _backingTexture.Texture.Height)
        {
            IsForced = true;
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

        // Let everything refresh before compose.
        if (screen.IsDirty || IsForced)
            RedrawSurface(screen);
    }

    ///  <inheritdoc/>
    public void Render(IScreenSurface screenObject) =>
        GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(_backingTexture.Texture, screenObject.AbsoluteArea.Position, _finalDrawColor));

    void RedrawSurface(IScreenSurface screenObject)
    {
        Raylib.BeginTextureMode(_backingTexture);
        Raylib.ClearBackground(Color.Transparent.ToHostColor());

        Raylib.BeginBlendMode(HostBlendState);

        IFont font = screenObject.Font;
        Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
        ColoredGlyphBase cell;

        if (screenObject.Surface.DefaultBackground.A != 0)
            Raylib.DrawTexturePro(fontImage, font.SolidGlyphRectangle.ToHostRectangle(), new(0, 0, _backingTexture.Texture.Width, _backingTexture.Texture.Height), Vector2.Zero, 0f, screenObject.Surface.DefaultBackground.ToHostColor());

        int rectIndex = 0;

        for (int y = 0; y < screenObject.Surface.View.Height; y++)
        {
            int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

            for (int x = 0; x < screenObject.Surface.View.Width; x++)
            {
                cell = screenObject.Surface[i];
                cell.IsDirty = false;

                if (cell.IsVisible)
                {
                    if (cell.Background != Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground)
                        Raylib.DrawTexturePro(fontImage, font.SolidGlyphRectangle.ToHostRectangle(), CachedRenderRects[rectIndex], Vector2.Zero, 0f, cell.Background.ToHostColor());

                    if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                        Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Glyph).ToHostRectangle(cell.Mirror), CachedRenderRects[rectIndex], Vector2.Zero, 0f, cell.Foreground.ToHostColor());

                    if (cell.Decorators != null)
                        for (int d = 0; d < cell.Decorators.Count; d++)
                            if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToHostRectangle(cell.Decorators[d].Mirror), CachedRenderRects[rectIndex], Vector2.Zero, 0f, cell.Decorators[d].Color.ToHostColor());
                }

                i++;
                rectIndex++;
            }
        }
        Raylib.EndBlendMode();
        Raylib.EndTextureMode();

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
            if (Raylib.IsRenderTextureValid(_backingTexture))
                Raylib.UnloadRenderTexture(_backingTexture);

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
