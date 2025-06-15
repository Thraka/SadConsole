using System;
using Color = SadRogue.Primitives.Color;
using HostColor = Raylib_cs.Color;
using HostRectangle = Raylib_cs.Rectangle;
using SadRogue.Primitives;
using Raylib_cs;
using System.Numerics;

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
    public RenderTexture2D BackingTexture { get; private set; }

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
        if (Raylib.IsRenderTextureValid(BackingTexture))
            Raylib.UnloadRenderTexture(BackingTexture);

        _cachedTexture?.Dispose();
        _cachedTexture = null;
        _layers = null;
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        bool result = false;

        // Update texture if something is out of size.
        if (backingTextureChanged || !Raylib.IsRenderTextureValid(BackingTexture) || screenObject.AbsoluteArea.Width != (int)BackingTexture.Texture.Width || screenObject.AbsoluteArea.Height != (int)BackingTexture.Texture.Height)
        {
            if (Raylib.IsRenderTextureValid(BackingTexture))
                Raylib.UnloadRenderTexture(BackingTexture);

            BackingTexture = Raylib.LoadRenderTexture(screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height);
            _cachedTexture?.Dispose();
            _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
            result = true;
        }

        var hostRenderer = (ScreenSurfaceRenderer)renderer;
        Components.LayeredSurface layerObject = _layers ?? ((ILayeredData)screenObject).Layers;

        // Redraw is needed
        if (result || screenObject.IsDirty || isForced)
        {
            Raylib.BeginTextureMode(BackingTexture);
            Raylib.ClearBackground(Color.Transparent.ToHostColor());

            Raylib.BeginBlendMode(hostRenderer.BlendState);

            IFont font = screenObject.Font;
            Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
            ColoredGlyphBase cell;

            if (layerObject.DefaultBackground.A != 0)
                Raylib.DrawTexturePro(fontImage, font.SolidGlyphRectangle.ToHostRectangle(), new(0, 0, BackingTexture.Texture.Width, BackingTexture.Texture.Height), Vector2.Zero, 0f, layerObject.DefaultBackground.ToHostColor());

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
                            if (cell.Background != Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground)
                                Raylib.DrawTexturePro(fontImage, font.SolidGlyphRectangle.ToHostRectangle(), hostRenderer.CachedRenderRects[rectIndex], Vector2.Zero, 0f, cell.Background.ToHostColor());

                            if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                                Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Glyph).ToHostRectangle(cell.Mirror), hostRenderer.CachedRenderRects[rectIndex], Vector2.Zero, 0f, cell.Foreground.ToHostColor());

                            if (cell.Decorators != null)
                                for (int d = 0; d < cell.Decorators.Count; d++)
                                    if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                        Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToHostRectangle(cell.Decorators[d].Mirror), hostRenderer.CachedRenderRects[rectIndex], Vector2.Zero, 0f, cell.Decorators[d].Color.ToHostColor());
                        }

                        i++;
                        rectIndex++;
                    }
                }
            }

            Raylib.EndBlendMode();
            Raylib.EndTextureMode();

            result = true;
            screenObject.IsDirty = false;
        }

        return result;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        Raylib.DrawTexture(BackingTexture.Texture, 0, 0, HostColor.White);
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
