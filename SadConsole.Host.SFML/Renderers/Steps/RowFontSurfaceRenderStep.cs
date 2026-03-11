#nullable enable
using System;
using SadRogue.Primitives;
using SFML.Graphics;
using static System.Net.Mime.MediaTypeNames;
using Color = SFML.Graphics.Color;

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
    public RenderTexture? BackingTexture { get; private set; }

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
            screenObject.AbsoluteArea.Width != (int)BackingTexture.Size.X || 
            screenObject.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
        {
            BackingTexture?.Dispose();
            BackingTexture = new RenderTexture(new((uint)screenObject.AbsoluteArea.Width, (uint)screenObject.AbsoluteArea.Height));
            _cachedTexture?.Dispose();
            _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
            result = true;
        }

        var sfmlRenderer = (ScreenSurfaceRenderer)renderer;

        // Redraw is needed
        if (result || screenObject.IsDirty || isForced)
        {
            BackingTexture.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Reset(BackingTexture, sfmlRenderer.SFMLBlendState, Transform.Identity);

            // Draw default background if needed
            if (screenObject.Surface.DefaultBackground.A != 0)
            {
                IFont defaultFont = screenObject.Font;
                Host.Global.SharedSpriteBatch.DrawQuad(
                    new IntRect(new(0, 0), new((int)BackingTexture.Size.X, (int)BackingTexture.Size.Y)), 
                    defaultFont.SolidGlyphRectangle.ToIntRect(), 
                    screenObject.Surface.DefaultBackground.ToSFMLColor(), 
                    ((SadConsole.Host.GameTexture)defaultFont.Image).Texture);
            }

            // Draw each row with its specific font
            int yOffset = 0;
            for (int y = 0; y < screenObject.Surface.View.Height; y++)
            {
                IFont rowFont = rowFontSurface.GetRowFont(y);
                SadRogue.Primitives.Point rowFontSize = rowFontSurface.GetRowFontSize(y);
                SFML.Graphics.Texture fontTexture = ((SadConsole.Host.GameTexture)rowFont.Image).Texture;

                int cellIndex = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) 
                              + screenObject.Surface.ViewPosition.X;

                for (int x = 0; x < screenObject.Surface.View.Width; x++)
                {
                    ColoredGlyphBase cell = screenObject.Surface[cellIndex];
                    cell.IsDirty = false;

                    if (cell.IsVisible)
                    {
                        // Calculate destination rect on the fly (no cached rects)
                        IntRect destRect = new IntRect(
                            new (x * rowFontSize.X, yOffset),
                            new (rowFontSize.X, rowFontSize.Y));

                        Host.Global.SharedSpriteBatch.DrawCell(cell, destRect, cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground, rowFont);
                    }

                    cellIndex++;
                }
                yOffset += rowFontSize.Y;
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
        IntRect outputArea = new(new (0, 0), new((int)BackingTexture!.Size.X, (int)BackingTexture.Size.Y));
        Host.Global.SharedSpriteBatch.DrawQuad(outputArea, outputArea, ComposeTint, BackingTexture.Texture);
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
