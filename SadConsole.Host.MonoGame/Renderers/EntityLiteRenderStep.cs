using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadRectangle = SadRogue.Primitives.Rectangle;
using SadConsole.Host.MonoGame;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws the entities of a <see cref="Entities.EntityLiteManager"/>.
    /// </summary>
    public class EntityLiteRenderStep : IRenderStep, IRenderStepTexture
    {
        private Entities.EntityLiteManager _entityManager;
        private ScreenSurfaceRenderer _baseRenderer;
        private IScreenSurface _screen;

        /// <summary>
        /// The cached texture of the drawn entities.
        /// </summary>
        public RenderTarget2D BackingTexture { get; private set; }

        /// <inheritdoc/>
        public int SortOrder { get; set; } = 5;

        ///  <inheritdoc/>
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(renderer is ScreenSurfaceRenderer)) throw new Exception($"Renderer used with {nameof(EntityLiteRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");
            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            _screen = surface;

            OnSurfaceChanged(renderer, surface);
        }

        ///  <inheritdoc/>
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            _screen = null;
            _baseRenderer = null;
            _entityManager = null;
        }

        ///  <inheritdoc/>
        public void OnSurfaceChanged(IRenderer renderer, IScreenSurface surface)
        {
            if (surface == null)
            {
                BackingTexture?.Dispose();
                BackingTexture = null;
                _screen = null;
                _entityManager = null;
            }
            else
            {
                if (!_screen.HasSadComponent(out Entities.EntityLiteManager host))
                    throw new Exception("EntityLiteManager is being run on object without a control host component.");
                _screen = surface;
                _entityManager = host;
                // BackingTexture is handled by prestart.
            }
        }

        ///  <inheritdoc/>
        public void RenderStart()
        {
            if (_screen.Tint.A != 255)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(_screen.AbsoluteArea.Position.X, _screen.AbsoluteArea.Position.Y), _baseRenderer._finalDrawColor));
        }

        ///  <inheritdoc/>
        public void RenderEnd() { }

        ///  <inheritdoc/>
        public bool RefreshPreStart()
        {
            // Update texture if something is out of size.
            if (BackingTexture == null || _screen.AbsoluteArea.Width != BackingTexture.Width || _screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, _screen.AbsoluteArea.Width, _screen.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                return true;
            }

            return false;
        }

        ///  <inheritdoc/>
        public void Refresh()
        {
            if (_baseRenderer.IsForced || _entityManager.IsDirty)
            {
                Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
                Host.Global.GraphicsDevice.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, _baseRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                Font font;
                SadRogue.Primitives.Point fontSize;
                Texture2D fontImage;
                ColoredGlyph cell;
                SadRectangle offsetArea = _screen.AbsoluteArea.WithPosition(_screen.Surface.ViewPosition * _screen.FontSize);
                SadRogue.Primitives.Point cellRenderPosition;
                XnaRectangle renderRect;

                foreach (Entities.EntityLite item in _entityManager.Entities)
                {
                    if (!item.IsVisible) continue;


                    if (item.UsePixelPositioning)
                    {
                        if (!offsetArea.Expand(item.FontSize.X, item.FontSize.Y).Contains(item.Position)) continue;

                        cellRenderPosition = item.Position - offsetArea.Position;
                        renderRect = new XnaRectangle(cellRenderPosition.X, cellRenderPosition.Y, item.FontSize.X, item.FontSize.Y);
                    }
                    else
                    {
                        if (!_screen.Surface.View.Contains(item.Position)) continue;

                        renderRect = _baseRenderer.CachedRenderRects[(item.Position - _screen.Surface.View.Position).ToIndex(_screen.Surface.ViewWidth)];
                    }

                    font = item.Font;
                    fontSize = item.FontSize;
                    fontImage = ((Host.GameTexture)font.Image).Texture;

                    cell = item.Appearance;

                    cell.IsDirty = false;

                    if (cell.Background != SadRogue.Primitives.Color.Transparent)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(font.SolidGlyphIndex).ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                    if (cell.Foreground != SadRogue.Primitives.Color.Transparent)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                    foreach (CellDecorator decorator in cell.Decorators)
                        if (decorator.Color != SadRogue.Primitives.Color.Transparent)
                            Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(decorator.Glyph).ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
                }

                Host.Global.SharedSpriteBatch.End();
                Host.Global.GraphicsDevice.SetRenderTarget(null);
            }

            _entityManager.IsDirty = false;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
        protected void Dispose(bool disposing)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
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
