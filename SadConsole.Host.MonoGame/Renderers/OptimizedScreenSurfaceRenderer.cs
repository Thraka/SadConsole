using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using System.Collections.Generic;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="IScreenSurface"/> with tint. Doesn't allow render steps.
/// </summary>
/// <remarks>
/// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
/// </remarks>
[System.Diagnostics.DebuggerDisplay("Surface")]
public sealed class OptimizedScreenSurfaceRenderer : IRenderer, IRendererMonoGame
{
    /// <summary>
    /// The final texture steps are drawing on.
    /// </summary>
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
    public RenderTarget2D _backingTexture;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public ITexture Output => _renderTexture;

    /// <summary>
    /// Color used with drawing the texture to the screen. Let's a surface become transparent.
    /// </summary>
    public Color _finalDrawColor = SadRogue.Primitives.Color.White.ToMonoColor();

    /// <summary>
    /// The blend state used by this renderer.
    /// </summary>
    public BlendState MonoGameBlendState { get; set; } = SadConsole.Host.Settings.MonoGameSurfaceBlendState;

    /// <summary>
    /// Used when creating the <see cref="_backingTexture"/> variable.
    /// </summary>
    public RenderTargetUsage BackingTextureUsageMode { get; set; } = RenderTargetUsage.DiscardContents;

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

    List<IRenderStep> IRenderer.Steps { get; set; } = new();

    /// <inheritdoc/>
    public XnaRectangle[] CachedRenderRects { get; private set; }

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
        if (_backingTexture == null || screen.AbsoluteArea.Width != _backingTexture.Width || screen.AbsoluteArea.Height != _backingTexture.Height)
        {
            IsForced = true;
            _backingTexture?.Dispose();
            _backingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, BackingTextureUsageMode);
            _renderTexture?.Dispose();
            _renderTexture = new Host.GameTexture(_backingTexture);
            BackingTextureRecreated?.Invoke(this, EventArgs.Empty);
        }
        
        // Update cached drawing rectangles if something is out of size.
        if (CachedRenderRects == null || CachedRenderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || CachedRenderRects[0].Width != screen.FontSize.X || CachedRenderRects[0].Height != screen.FontSize.Y)
        {
            CachedRenderRects = new XnaRectangle[screen.Surface.View.Width * screen.Surface.View.Height];

            for (int i = 0; i < CachedRenderRects.Length; i++)
            {
                var position = SadRogue.Primitives.Point.FromIndex(i, screen.Surface.View.Width);
                CachedRenderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToMonoRectangle();
            }

            IsForced = true;
        }

        // Let everything refresh before compose.
        if (screen.IsDirty || IsForced)
            RedrawSurface(screen);
    }

    ///  <inheritdoc/>
    public void Render(IScreenSurface screen) =>
        GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(_backingTexture, new Vector2(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y), _finalDrawColor));

    void RedrawSurface(IScreenSurface screenObject)
    {
        IFont font = screenObject.Font;
        Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
        ColoredGlyphBase cell;

        Host.Global.GraphicsDevice.SetRenderTarget(_backingTexture);
        Host.Global.GraphicsDevice.Clear(Color.Transparent);
        Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

        if (screenObject.Surface.DefaultBackground.A != 0)
            Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, _backingTexture.Width, _backingTexture.Height), font.SolidGlyphRectangle.ToMonoRectangle(), screenObject.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

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
                    if (cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, CachedRenderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                    if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                    if (cell.Decorators != null)
                        for (int d = 0; d < cell.Decorators.Count; d++)
                            if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                Host.Global.SharedSpriteBatch.Draw(fontImage, CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), cell.Decorators[d].Color.ToMonoColor(), 0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
                }

                i++;
                rectIndex++;
            }
        }

        // Tint
        if (screenObject.Tint.A!= SadRogue.Primitives.Color.Transparent.A)
            Host.Global.SharedSpriteBatch.Draw(fontImage, screenObject.AbsoluteArea.ToMonoRectangle(), font.SolidGlyphRectangle.ToMonoRectangle(), screenObject.Tint.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

        Host.Global.SharedSpriteBatch.End();
        Host.Global.GraphicsDevice.SetRenderTarget(null);

        screenObject.IsDirty = false;
    }

    #region IDisposable Support
    /// <summary>
    /// Detects redundant calls.
    /// </summary>
    bool disposedValue;

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
