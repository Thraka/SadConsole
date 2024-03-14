#nullable enable
using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="SadConsole.IScreenSurface"/> object.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Surface")]
public class SurfaceRenderStep : IRenderStep, IRenderStepTexture
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
    public string Name => Constants.RenderStepNames.Surface;

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Surface;

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

        // Redraw is needed
        if (result || screenObject.IsDirty || isForced)
        {
            BackingTexture.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Reset(BackingTexture, sfmlRenderer.SFMLBlendState, Transform.Identity);

            int rectIndex = 0;
            ColoredGlyphBase cell;
            IFont font = screenObject.Font;

            if (screenObject.Surface.DefaultBackground.A != 0)
                Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y), font.SolidGlyphRectangle.ToIntRect(), screenObject.Surface.DefaultBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)font.Image).Texture);

            for (int y = 0; y < screenObject.Surface.View.Height; y++)
            {
                int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

                for (int x = 0; x < screenObject.Surface.View.Width; x++)
                {
                    cell = screenObject.Surface[i];

                    cell.IsDirty = false;

                    if (cell.IsVisible)
                        Host.Global.SharedSpriteBatch.DrawCell(cell, sfmlRenderer.CachedRenderRects[rectIndex], cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground, font);

                    i++;
                    rectIndex++;
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
        IntRect outputArea = new IntRect(0, 0, (int)BackingTexture!.Size.X, (int)BackingTexture.Size.Y);
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
    ~SurfaceRenderStep() =>
        Dispose(false);
}
