using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;

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
    public RenderTexture BackingTexture { get; private set; }

    /// <inheritdoc/>//
    public ITexture CachedTexture => _cachedTexture;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.Surface;

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
        _layers = null;
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        bool result = false;

        // Update texture if something is out of size.
        if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != (int)BackingTexture.Size.X || screenObject.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
        {
            BackingTexture?.Dispose();
            BackingTexture = new RenderTexture((uint)screenObject.AbsoluteArea.Width, (uint)screenObject.AbsoluteArea.Height);
            _cachedTexture?.Dispose();
            _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
            result = true;
        }

        var sfmlRenderer = (ScreenSurfaceRenderer)renderer;
        Components.LayeredSurface layerObject = _layers ?? ((ILayeredData)screenObject).Layers;

        // Redraw is needed
        if (result || screenObject.IsDirty || isForced)
        {
            BackingTexture.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Reset(BackingTexture, sfmlRenderer.SFMLBlendState, Transform.Identity);

            ColoredGlyphBase cell;
            IFont font = screenObject.Font;

            if (layerObject.DefaultBackground.A != 0)
                Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y), font.SolidGlyphRectangle.ToIntRect(), layerObject.DefaultBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)font.Image).Texture);

            foreach (ICellSurface layer in layerObject)
            {
                int rectIndex = 0;

                for (int y = 0; y < layer.Surface.View.Height; y++)
                {
                    int i = ((y + layer.Surface.ViewPosition.Y) * layer.Surface.Width) + layer.Surface.ViewPosition.X;

                    for (int x = 0; x < layer.Surface.View.Width; x++)
                    {
                        cell = layer.Surface[i];
                        cell.IsDirty = false;

                        if (cell.IsVisible)
                            Host.Global.SharedSpriteBatch.DrawCell(cell, sfmlRenderer.CachedRenderRects[rectIndex], cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != layer.Surface.DefaultBackground, font);

                        i++;
                        rectIndex++;
                    }
                }
            }

            Host.Global.SharedSpriteBatch.End();
            BackingTexture.Display();

            result = true;
            screenObject.IsDirty = false;
        }

        return result;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        IntRect outputArea = new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y);
        Host.Global.SharedSpriteBatch.DrawQuad(outputArea, outputArea, Color.White, BackingTexture.Texture);
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
