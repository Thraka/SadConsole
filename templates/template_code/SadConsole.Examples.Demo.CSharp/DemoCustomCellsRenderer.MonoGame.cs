#if MONOGAME
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Renderers;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.Examples;

// This class is a renderer that differs from the base class by swapping out
// SurfaceRenderStep for OffsetGlyphRenderStep.
class OffsetGlyphRenderer: SadConsole.Renderers.ScreenSurfaceRenderer
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
    public RenderTarget2D? BackingTexture { get; private set; }

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
        if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != BackingTexture.Width || screenObject.AbsoluteArea.Height != BackingTexture.Height)
        {
            BackingTexture?.Dispose();
            BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            _cachedTexture?.Dispose();
            _cachedTexture = new Host.GameTexture(BackingTexture);
            result = true;
        }

        var monoRenderer = (IRendererMonoGame)renderer;

        // Redraw is needed
        if (result || screenObject.IsDirty || isForced)
        {
            Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            Host.Global.GraphicsDevice.Clear(XnaColor.Transparent);
            Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, monoRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

            IFont font = screenObject.Font;
            Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
            ColoredGlyphOffset cell; // THIS code is changed from the original

            if (screenObject.Surface.DefaultBackground.A != 0)
                Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), font.SolidGlyphRectangle.ToMonoRectangle(), screenObject.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

            int rectIndex = 0;

            for (int y = 0; y < screenObject.Surface.View.Height; y++)
            {
                int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

                for (int x = 0; x < screenObject.Surface.View.Width; x++)
                {
                    cell = (ColoredGlyphOffset)screenObject.Surface[i];
                    cell.IsDirty = false;

                    if (cell.IsVisible)
                    {
                        // THESE two lines are new, it caches the render rect and then offsets it. Each SharedSpriteBatch.Draw call below uses the new rect now
                        XnaRectangle renderRect = monoRenderer.CachedRenderRects[rectIndex];
                        renderRect.Offset(cell.RenderingOffset.ToMonoPoint());

                        if (cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground)
                            Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                        if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                            Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                        if (cell.Decorators != null)
                            for (int d = 0; d < cell.Decorators.Count; d++)
                                if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                    Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), cell.Decorators[d].Color.ToMonoColor(), 0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
                    }

                    i++;
                    rectIndex++;
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
        Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, XnaColor.White);
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
