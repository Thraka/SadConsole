#if SFML
using SadConsole.Renderers;
using Color = SFML.Graphics.Color;
using SFML.Graphics;

namespace SadConsole.Examples;

// This class is a renderer that differs from the base class by swapping out
// SurfaceRenderStep for OffsetGlyphRenderStep.
class OffsetGlyphRenderer : SadConsole.Renderers.ScreenSurfaceRenderer
{
    protected override void AddDefaultSteps()
    {
        // Custom step here
        Steps.Add(new OffsetGlyphRenderStep());

        // Original steps from ScreenSurfaceRenderer
        Steps.Add(new OutputSurfaceRenderStep());
        Steps.Add(new TintSurfaceRenderStep());
    }
}

// This code was copied directly from the SurfaceRenderStep and modified to use the
// custom glyph type in the Refresh method.
class OffsetGlyphRenderStep : IRenderStep, IRenderStepTexture
{
    private Host.GameTexture? _cachedTexture;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public RenderTexture? BackingTexture { get; private set; }

    /// <inheritdoc/>//
    public ITexture? CachedTexture => _cachedTexture;

    /// <inheritdoc/>
    public string Name => "OffsetGlyphRenderStep";

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Renderers.Constants.RenderStepSortValues.Surface;

    /// <summary>
    /// Not used.
    /// </summary>
    public void SetData(object data) { }

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
            ColoredGlyphOffset cell; // THIS code is changed from the original
            IFont font = screenObject.Font;

            if (screenObject.Surface.DefaultBackground.A != 0)
                Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y), font.SolidGlyphRectangle.ToIntRect(), screenObject.Surface.DefaultBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)font.Image).Texture);

            for (int y = 0; y < screenObject.Surface.View.Height; y++)
            {
                int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

                for (int x = 0; x < screenObject.Surface.View.Width; x++)
                {
                    cell = (ColoredGlyphOffset)screenObject.Surface[i];

                    cell.IsDirty = false;

                    if (cell.IsVisible)
                    {
                        // THESE two lines are new, it grabs the cached render rect and offsets it
                        // 
                        IntRect originalRect = sfmlRenderer.CachedRenderRects[rectIndex];
                        IntRect renderRect = new IntRect(cell.RenderingOffset.X + originalRect.Left, cell.RenderingOffset.Y + originalRect.Top, originalRect.Width, originalRect.Height);

                        Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect, cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground, font);
                    }
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
    ~OffsetGlyphRenderStep() =>
        Dispose(false);
}

#endif
