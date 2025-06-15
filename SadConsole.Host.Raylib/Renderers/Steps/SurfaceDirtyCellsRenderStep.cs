using System;
using System.Numerics;
using Raylib_cs;
using SadRogue.Primitives;
using Color = SadRogue.Primitives.Color;
using HostColor = Raylib_cs.Color;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="SadConsole.IScreenSurface"/> object.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Surface (Dirty)")]
public class SurfaceDirtyCellsRenderStep : IRenderStep, IRenderStepTexture
{
    private Host.GameTexture _cachedTexture;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public RenderTexture2D BackingTexture { get; private set; }

    /// <inheritdoc/>//
    public ITexture CachedTexture => _cachedTexture;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.SurfaceDirtyCells;

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Surface;

    /// <summary>
    /// Not used.
    /// </summary>
    public void SetData(object data) { }

    ///  <inheritdoc/>
    public void Reset()
    {
        if (Raylib.IsRenderTextureValid(BackingTexture))
            Raylib.UnloadRenderTexture(BackingTexture);

        _cachedTexture?.Dispose();
        _cachedTexture = null;
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        bool fullRedraw = false;

        // Update texture if something is out of size.
        if (backingTextureChanged || !Raylib.IsRenderTextureValid(BackingTexture) || screenObject.AbsoluteArea.Width != (int)BackingTexture.Texture.Width || screenObject.AbsoluteArea.Height != (int)BackingTexture.Texture.Height)
        {
            if (Raylib.IsRenderTextureValid(BackingTexture))
                Raylib.UnloadRenderTexture(BackingTexture);

            BackingTexture = Raylib.LoadRenderTexture(screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height);
            _cachedTexture?.Dispose();
            _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
            fullRedraw = true;
        }

        var hostRenderer = (ScreenSurfaceRenderer)renderer;

        // Redraw is needed
        if (fullRedraw || screenObject.IsDirty || isForced)
        {
            Raylib.BeginTextureMode(BackingTexture);

            // Only cleared when full redraw needed
            if (fullRedraw)
                Raylib.ClearBackground(Color.Transparent.ToHostColor());

            Raylib.BeginBlendMode(hostRenderer.BlendState);

            int rectIndex = 0;
            IFont font = screenObject.Font;
            Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
            ColoredGlyphBase cell;

            if (fullRedraw)
                Raylib.DrawTexturePro(fontImage, font.SolidGlyphRectangle.ToHostRectangle(), new(0, 0, BackingTexture.Texture.Width, BackingTexture.Texture.Height), Vector2.Zero, 0f, screenObject.Surface.DefaultBackground.ToHostColor());

            for (int y = 0; y < screenObject.Surface.View.Height; y++)
            {
                int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

                for (int x = 0; x < screenObject.Surface.View.Width; x++)
                {
                    cell = screenObject.Surface[i];

                    if (cell.IsDirty || fullRedraw)
                    {
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
                    }

                    i++;
                    rectIndex++;
                }
            }

            Raylib.EndBlendMode();
            Raylib.EndTextureMode();

            fullRedraw = true;
            screenObject.IsDirty = false;
        }

        return fullRedraw;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        Raylib.DrawTexture(BackingTexture.Texture, 0, 0, HostColor.Blank);
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
    ~SurfaceDirtyCellsRenderStep() =>
        Dispose(false);
}
