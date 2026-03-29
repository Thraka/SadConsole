#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="RowFontSurface"/> object.
/// </summary>
/// <remarks>
/// This render step handles surfaces where each row can use a different font with different glyph dimensions.
/// Destination rectangles are computed on the fly using each row's font size and Y offset, rather than using
/// cached rectangles like <see cref="SurfaceRenderStep"/>.
/// </remarks>
[System.Diagnostics.DebuggerDisplay("RowFontSurface")]
public class RowFontSurfaceRenderStep : IRenderStep, IRenderStepTexture
{
    private Host.GameTexture? _cachedTexture;
    private IScreenSurface? _screenSurface;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public RenderTarget2D? BackingTexture { get; private set; }

    /// <inheritdoc/>
    public ITexture? CachedTexture => _cachedTexture;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.RowFontSurface;

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.RowFontSurface;

    /// <summary>
    /// The color applied when the sprite batch draws this surface on the renderer.
    /// </summary>
    public Color ComposeTint { get; set; } = Color.White;

    /// <summary>
    /// Sets an alternative <see cref="IScreenSurface"/> to render. If null, the surface associated with the parent renderer is drawn.
    /// </summary>
    public void SetData(object data)
    {
        if (data is null)
            _screenSurface = null;
        else if (data is IScreenSurface surface)
            _screenSurface = surface;
        else
            throw new InvalidCastException($"Data must be {nameof(IScreenSurface)}");
    }

    ///  <inheritdoc/>
    public void Reset()
    {
        BackingTexture?.Dispose();
        BackingTexture = null;
        _cachedTexture?.Dispose();
        _cachedTexture = null;
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        bool result = false;

        // Swap references if data was set
        screenObject = _screenSurface ?? screenObject;

        // Cast to RowFontSurface to access row font methods
        var rowFontSurface = screenObject as RowFontSurface;
        if (rowFontSurface == null)
            throw new InvalidOperationException("RowFontSurfaceRenderStep requires a RowFontSurface");

        // Update texture if something is out of size.
        if (backingTextureChanged || BackingTexture == null || 
            screenObject.AbsoluteArea.Width != BackingTexture.Width || 
            screenObject.AbsoluteArea.Height != BackingTexture.Height)
        {
            BackingTexture?.Dispose();
            BackingTexture = new RenderTarget2D(
                Host.Global.GraphicsDevice, 
                screenObject.AbsoluteArea.Width, 
                screenObject.AbsoluteArea.Height, 
                false, 
                Host.Global.GraphicsDevice.DisplayMode.Format, 
                DepthFormat.Depth24, 
                0, 
                RenderTargetUsage.DiscardContents);
            _cachedTexture?.Dispose();
            _cachedTexture = new Host.GameTexture(BackingTexture);
            result = true;
        }

        var monoRenderer = (IRendererMonoGame)renderer;

        // Redraw is needed
        if (result || screenObject.IsDirty || isForced)
        {
            Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            Host.Global.GraphicsDevice.Clear(Color.Transparent);
            monoRenderer.LocalSpriteBatch.Begin(
                SpriteSortMode.Deferred, 
                monoRenderer.MonoGameBlendState, 
                SamplerState.PointClamp, 
                DepthStencilState.DepthRead, 
                RasterizerState.CullNone);

            // Draw default background if needed
            if (screenObject.Surface.DefaultBackground.A != 0)
            {
                IFont defaultFont = screenObject.Font;
                Texture2D fontImage = ((Host.GameTexture)defaultFont.Image).Texture;
                monoRenderer.LocalSpriteBatch.Draw(
                    fontImage, 
                    new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), 
                    defaultFont.SolidGlyphRectangle.ToMonoRectangle(), 
                    screenObject.Surface.DefaultBackground.ToMonoColor(), 
                    0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            }

            // Draw each row with its specific font
            int yOffset = 0;
            for (int y = 0; y < screenObject.Surface.View.Height; y++)
            {
                IFont rowFont = rowFontSurface.GetRowFont(y);
                SadRogue.Primitives.Point rowFontSize = rowFontSurface.GetRowFontSize(y);
                Texture2D fontImage = ((Host.GameTexture)rowFont.Image).Texture;

                int cellIndex = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) 
                              + screenObject.Surface.ViewPosition.X;

                for (int x = 0; x < screenObject.Surface.View.Width; x++)
                {
                    ColoredGlyphBase cell = screenObject.Surface[cellIndex];
                    cell.IsDirty = false;

                    if (cell.IsVisible)
                    {
                        // Calculate destination rect on the fly (no cached rects)
                        XnaRectangle destRect = new XnaRectangle(
                            x * rowFontSize.X,
                            yOffset,
                            rowFontSize.X,
                            rowFontSize.Y);

                        // Background
                        if (cell.Background != SadRogue.Primitives.Color.Transparent && 
                            cell.Background != screenObject.Surface.DefaultBackground)
                        {
                            monoRenderer.LocalSpriteBatch.Draw(
                                fontImage, 
                                destRect, 
                                rowFont.SolidGlyphRectangle.ToMonoRectangle(), 
                                cell.Background.ToMonoColor(), 
                                0f, Vector2.Zero, SpriteEffects.None, 0.3f);
                        }

                        // Foreground glyph
                        if (cell.Glyph != 0 && 
                            cell.Foreground != SadRogue.Primitives.Color.Transparent && 
                            cell.Foreground != cell.Background)
                        {
                            monoRenderer.LocalSpriteBatch.Draw(
                                fontImage, 
                                destRect, 
                                rowFont.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), 
                                cell.Foreground.ToMonoColor(), 
                                0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);
                        }

                        // Decorators
                        if (cell.Decorators != null)
                        {
                            for (int d = 0; d < cell.Decorators.Count; d++)
                            {
                                if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                {
                                    monoRenderer.LocalSpriteBatch.Draw(
                                        fontImage, 
                                        destRect, 
                                        rowFont.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), 
                                        cell.Decorators[d].Color.ToMonoColor(), 
                                        0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
                                }
                            }
                        }
                    }

                    cellIndex++;
                }
                yOffset += rowFontSize.Y;
            }

            monoRenderer.LocalSpriteBatch.End();
            Host.Global.GraphicsDevice.SetRenderTarget(null);

            result = true;
            screenObject.IsDirty = false;
        }

        return result;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, ComposeTint);
    }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject) { }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
    protected void Dispose(bool disposing) =>
        Reset();

    ///  <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizes the object for collection.
    /// </summary>
    ~RowFontSurfaceRenderStep() =>
        Dispose(false);
}
