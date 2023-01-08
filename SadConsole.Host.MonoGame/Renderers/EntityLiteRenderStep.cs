using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaPoint = Microsoft.Xna.Framework.Point;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadRectangle = SadRogue.Primitives.Rectangle;
using SadConsole.Host.MonoGame;

namespace SadConsole.Renderers
{
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
        public RenderTarget2D BackingTexture { get; private set; }

        /// <inheritdoc/>
        public ITexture CachedTexture => _cachedTexture;

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
            if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != BackingTexture.Width || screenObject.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                _cachedTexture?.Dispose();
                _cachedTexture = new Host.GameTexture(BackingTexture);
                result = true;
            }

            if (result || _entityManager.IsDirty || isForced)
            {
                Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
                Host.Global.GraphicsDevice.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, ((ScreenSurfaceRenderer)renderer).MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                Texture2D fontImage = ((Host.GameTexture)screenObject.Font.Image).Texture;
                IFont font = screenObject.Font;
                ColoredGlyph cell;
                XnaRectangle renderRect;

                Entities.Entity item;
                for (int i = 0; i < _entityManager.EntitiesVisible.Count; i++)
                {
                    item = _entityManager.EntitiesVisible[i];

                    if (!item.IsVisible) continue;

                    renderRect = _entityManager.GetRenderRectangle(item.Position, item.UsePixelPositioning).ToMonoRectangle();
                    item.IsDirty = false;

                    if (item.IsSingleCell)
                    {
                        cell = item.AppearanceSingle.Appearance;
                        cell.IsDirty = false;

                        if (cell.Background != SadRogue.Primitives.Color.Transparent)
                            Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                        if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent)
                            Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                        for (int d = 0; d < cell.Decorators.Length; d++)
                            if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), cell.Decorators[d].Color.ToMonoColor(), 0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
                    }
                    else
                    {
                        // Offset the top-left render rectangle by the center point of the animation.
                        XnaPoint surfaceStartPosition = new XnaPoint(renderRect.X - (item.AppearanceSurface.Animation.Center.X * renderRect.Width), renderRect.Y - (item.AppearanceSurface.Animation.Center.Y * renderRect.Height));

                        for (int y = 0; y < item.AppearanceSurface.Animation.CurrentFrame.View.Height; y++)
                        {
                            // local index of cell of surface we want to draw
                            int index = ((y + item.AppearanceSurface.Animation.CurrentFrame.ViewPosition.Y) * item.AppearanceSurface.Animation.CurrentFrame.Width) + item.AppearanceSurface.Animation.CurrentFrame.ViewPosition.X;

                            for (int x = 0; x < item.AppearanceSurface.Animation.CurrentFrame.View.Width; x++)
                            {
                                // Move the render rect by the x,y of the current cell being drawn'
                                renderRect = new XnaRectangle(surfaceStartPosition.X + (x * renderRect.Width), surfaceStartPosition.Y + (y * renderRect.Height), renderRect.Width, renderRect.Height);

                                cell = item.AppearanceSurface.Animation.CurrentFrame[index];
                                cell.IsDirty = false;

                                if (cell.IsVisible)
                                {
                                    if (cell.Background != SadRogue.Primitives.Color.Transparent)
                                        Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                                    if (cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                                        Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                                    for (int d = 0; d < cell.Decorators.Length; d++)
                                        if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                            Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), cell.Decorators[d].Color.ToMonoColor(), 0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
                                }

                                index++;
                            }
                        }
                    }
                }

                Host.Global.SharedSpriteBatch.End();
                Host.Global.GraphicsDevice.SetRenderTarget(null);

                result = true;
                _entityManager.IsDirty = false;
            }

            return result;
        }

        ///  <inheritdoc/>
        public void Composing(IRenderer renderer, IScreenSurface screenObject)
        {
            Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, Color.White);
        }

        ///  <inheritdoc/>
        public void Render(IRenderer renderer, IScreenSurface screenObject)
        {
        }


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
        protected void Dispose(bool disposing)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            _cachedTexture?.Dispose();
            _cachedTexture = null;
        }

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
}
