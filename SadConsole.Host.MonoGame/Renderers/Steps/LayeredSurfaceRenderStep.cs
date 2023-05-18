using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="SadConsole.IScreenSurface"/> object.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Surface (Layered)")]
public class LayeredSurfaceRenderStep : IRenderStep, IRenderStepTexture
{
    private Host.GameTexture _cachedTexture;

    private Components.LayeredSurface _layers;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public RenderTarget2D BackingTexture { get; private set; }

    /// <inheritdoc/>//
    public ITexture CachedTexture => _cachedTexture;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.SurfaceLayered;

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Surface;

    /// <summary>
    /// Sets the <see cref="Components.LayeredSurface"/>.
    /// </summary>
    /// <param name="data">A <see cref="Components.LayeredSurface"/> object.</param>
    public void SetData(object data)
    {
        if (data is Components.LayeredSurface layers)
            _layers = layers;
        else
            throw new ArgumentException($"{nameof(LayeredSurfaceRenderStep)} must have a {nameof(Components.LayeredSurface)} passed to the {nameof(SetData)} method", nameof(data));
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

        // Update texture if something is out of size.
        if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != BackingTexture.Width || screenObject.AbsoluteArea.Height != BackingTexture.Height)
        {
            BackingTexture?.Dispose();
            BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            _cachedTexture?.Dispose();
            _cachedTexture = new Host.GameTexture(BackingTexture);
            result = true;
        }

        var monoRenderer = (IRendererMonoGame)renderer;
        Components.LayeredSurface layerObject = _layers ?? ((ILayeredData)screenObject).Layers;

        // Redraw is needed
        if (result || screenObject.IsDirty || isForced)
        {
            Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            Host.Global.GraphicsDevice.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, monoRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

            IFont font = screenObject.Font;
            Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
            ColoredGlyph cell;

            if (layerObject.DefaultBackground.A != 0)
                Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), font.SolidGlyphRectangle.ToMonoRectangle(), layerObject.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

            foreach (ICellSurface layer in layerObject)
            {
                int rectIndex = 0;

                for (int y = 0; y < layer.View.Height; y++)
                {
                    int i = ((y + layer.ViewPosition.Y) * layer.Width) + layer.ViewPosition.X;

                    for (int x = 0; x < layer.View.Width; x++)
                    {
                        cell = layer[i];
                        cell.IsDirty = false;

                        if (cell.IsVisible)
                        {
                            if (cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != layer.DefaultBackground)
                                Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                            if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                                Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                            for (int d = 0; d < cell.Decorators.Length; d++)
                                if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                    Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), cell.Decorators[d].Color.ToMonoColor(), 0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
                        }

                        i++;
                        rectIndex++;
                    }
                }
            }

            Host.Global.SharedSpriteBatch.End();
            Host.Global.GraphicsDevice.SetRenderTarget(null);

            result = true;
            screenObject.IsDirty = false;
        }

        return result;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, Color.White);
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
    ~LayeredSurfaceRenderStep() =>
        Dispose(false);
}
