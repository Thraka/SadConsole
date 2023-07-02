﻿using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;

namespace SadConsole.Renderers;

/// <summary>
/// Draws the entities of a <see cref="Entities.Renderer"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Entity lite")]
public class EntityLiteRenderStep : IRenderStep, IRenderStepTexture
{
    private Entities.Renderer _entityManager;
    private Host.GameTexture _cachedTexture;

    /// <summary>
    /// The cached texture of the drawn entities.
    /// </summary>
    public RenderTexture BackingTexture { get; private set; }

    /// <inheritdoc/>
    public ITexture CachedTexture => _cachedTexture;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.EntityRenderer;

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.EntityRenderer;

    /// <summary>
    /// Sets the <see cref="Entities.Renderer"/>.
    /// </summary>
    /// <param name="data">A <see cref="Entities.Renderer"/> object.</param>
    public void SetData(object data)
    {
        if (data is Entities.Renderer manager)
            _entityManager = manager;
        else
            throw new ArgumentException($"{nameof(EntityLiteRenderStep)} must have a {nameof(Entities.Renderer)} passed to the {nameof(SetData)} method", nameof(data));
    }

    ///  <inheritdoc/>
    public void Reset()
    {
        BackingTexture?.Dispose();
        BackingTexture = null;
        _cachedTexture?.Dispose();
        _cachedTexture = null;
        _entityManager = null;
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

        // Redraw is needed
        if (result || _entityManager.IsDirty || isForced)
        {
            BackingTexture.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Reset(BackingTexture, ((ScreenSurfaceRenderer)renderer).SFMLBlendState, Transform.Identity);

            ColoredGlyph cell;
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

                    Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect.ToIntRect(), true, screenObject.Font);
                }
                else
                {
                    // Offset the top-left render rectangle by the center point of the animation.
                    var surfaceStartPosition = new Point(renderRect.X - (item.AppearanceSurface.Animation.Center.X * renderRect.Width), renderRect.Y - (item.AppearanceSurface.Animation.Center.Y * renderRect.Height));

                    for (int y = 0; y < item.AppearanceSurface.Animation.CurrentFrame.View.Height; y++)
                    {
                        // local index of cell of surface we want to draw
                        int index = ((y + item.AppearanceSurface.Animation.CurrentFrame.ViewPosition.Y) * item.AppearanceSurface.Animation.CurrentFrame.Width) + item.AppearanceSurface.Animation.CurrentFrame.ViewPosition.X;

                        for (int x = 0; x < item.AppearanceSurface.Animation.CurrentFrame.View.Width; x++)
                        {
                            // Move the render rect by the x,y of the current cell being drawn'
                            renderRect = new Rectangle(surfaceStartPosition.X + (x * renderRect.Width), surfaceStartPosition.Y + (y * renderRect.Height), renderRect.Width, renderRect.Height);

                            cell = item.AppearanceSurface.Animation.CurrentFrame[index];
                            cell.IsDirty = false;

                            if (cell.IsVisible)
                                Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect.ToIntRect(), true, screenObject.Font);

                            index++;
                        }
                    }
                }
            }

            Host.Global.SharedSpriteBatch.End();
            BackingTexture.Display();

            result = true;
            _entityManager.IsDirty = false;
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
    ~EntityLiteRenderStep() =>
        Dispose(false);
}