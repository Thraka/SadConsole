using System;
using System.Numerics;
using Raylib_cs;
using SadRogue.Primitives;
using Color = SadRogue.Primitives.Color;
using Rectangle = SadRogue.Primitives.Rectangle;
using HostColor = Raylib_cs.Color;

namespace SadConsole.Renderers;

/// <summary>
/// Draws the entities of a <see cref="Entities.EntityManager"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Entity")]
public class EntityRenderStep : IRenderStep, IRenderStepTexture
{
    private Entities.EntityManager _entityManager;
    private Host.GameTexture _cachedTexture;

    /// <summary>
    /// The cached texture of the drawn entities.
    /// </summary>
    public RenderTexture2D BackingTexture { get; private set; }

    /// <inheritdoc/>
    public ITexture CachedTexture => _cachedTexture;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.EntityManager;

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.EntityRenderer;

    /// <summary>
    /// Sets the <see cref="Entities.EntityManager"/>.
    /// </summary>
    /// <param name="data">A <see cref="Entities.EntityManager"/> object.</param>
    public void SetData(object data)
    {
        if (data is Entities.EntityManager manager)
            _entityManager = manager;
        else
            throw new ArgumentException($"{nameof(EntityRenderStep)} must have a {nameof(Entities.EntityManager)} passed to the {nameof(SetData)} method", nameof(data));
    }

    ///  <inheritdoc/>
    public void Reset()
    {
        if (Raylib.IsRenderTextureValid(BackingTexture))
            Raylib.UnloadRenderTexture(BackingTexture);

        _cachedTexture?.Dispose();
        _cachedTexture = null;
        _entityManager = null;
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

        if (result || _entityManager.IsDirty || isForced)
        {
            Raylib.BeginTextureMode(BackingTexture);
            Raylib.ClearBackground(Color.Transparent.ToHostColor());

            Raylib.BeginBlendMode(hostRenderer.BlendState);

            IFont font = _entityManager.AlternativeFont ?? screenObject.Font;
            Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
            ColoredGlyphBase cell;
            Rectangle renderRect;

            Entities.Entity item;
            for (int i = 0; i < _entityManager.EntitiesVisible.Count; i++)
            {
                item = _entityManager.EntitiesVisible[i];

                if (!item.IsVisible) continue;

                renderRect = _entityManager.GetRenderRectangle(item.Position, item.UsePixelPositioning);
                item.IsDirty = false;

                if (item.IsSingleCell)
                {
                    cell = item.AppearanceSingle.Appearance;
                    cell.IsDirty = false;

                    if (cell.Background != Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground)
                            Raylib.DrawTexturePro(fontImage, font.SolidGlyphRectangle.ToHostRectangle(), renderRect.ToHostRectangle(), Vector2.Zero, 0f, cell.Background.ToHostColor());

                        if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                            Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Glyph).ToHostRectangle(cell.Mirror), renderRect.ToHostRectangle(), Vector2.Zero, 0f, cell.Foreground.ToHostColor());

                        if (cell.Decorators != null)
                            for (int d = 0; d < cell.Decorators.Count; d++)
                                if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                    Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToHostRectangle(cell.Decorators[d].Mirror), renderRect.ToHostRectangle(), Vector2.Zero, 0f, cell.Decorators[d].Color.ToHostColor());
                }
                else
                {
                    // Offset the top-left render rectangle by the center point of the animation.
                    Point surfaceStartPosition = new Point(renderRect.X - (item.AppearanceSurface.Animation.Center.X * renderRect.Width), renderRect.Y - (item.AppearanceSurface.Animation.Center.Y * renderRect.Height));

                    for (int y = 0; y < item.AppearanceSurface.Animation.ViewHeight; y++)
                    {
                        // local index of cell of surface we want to draw
                        int index = ((y + item.AppearanceSurface.Animation.ViewPosition.Y) * item.AppearanceSurface.Animation.Width) + item.AppearanceSurface.Animation.ViewPosition.X;

                        for (int x = 0; x < item.AppearanceSurface.Animation.ViewWidth; x++)
                        {
                            // Move the render rect by the x,y of the current cell being drawn'
                            renderRect = new Rectangle(surfaceStartPosition.X + (x * renderRect.Width), surfaceStartPosition.Y + (y * renderRect.Height), renderRect.Width, renderRect.Height);

                            cell = item.AppearanceSurface.Animation.CurrentFrame[index];
                            cell.IsDirty = false;

                            if (cell.IsVisible)
                            {
                                if (cell.Background != Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground)
                                    Raylib.DrawTexturePro(fontImage, font.SolidGlyphRectangle.ToHostRectangle(), renderRect.ToHostRectangle(), Vector2.Zero, 0f, cell.Background.ToHostColor());

                                if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                                    Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Glyph).ToHostRectangle(cell.Mirror), renderRect.ToHostRectangle(), Vector2.Zero, 0f, cell.Foreground.ToHostColor());

                                if (cell.Decorators != null)
                                    for (int d = 0; d < cell.Decorators.Count; d++)
                                        if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                            Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToHostRectangle(cell.Decorators[d].Mirror), renderRect.ToHostRectangle(), Vector2.Zero, 0f, cell.Decorators[d].Color.ToHostColor());
                            }

                            index++;
                        }
                    }
                }
            }

            Raylib.EndBlendMode();
            Raylib.EndTextureMode();

            result = true;
            _entityManager.IsDirty = false;
        }

        return result;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        Raylib.DrawTexture(BackingTexture.Texture, 0, 0, HostColor.White);
    }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
    }


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
    ~EntityRenderStep() =>
        Dispose(false);
}
