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
    /// Draws a <see cref="SadConsole.IScreenSurface"/> object.
    /// </summary>
    public class SurfaceRenderStep : IRenderStep, IRenderStepTexture
    {
        private IScreenSurface _screen;
        private Host.GameTexture _cachedTexture;
        private ScreenSurfaceRenderer _baseRenderer;

        /// <summary>
        /// The cached texture of the drawn surface.
        /// </summary>
        public RenderTarget2D BackingTexture { get; private set; }

        /// <inheritdoc/>//
        public ITexture CachedTexture => _cachedTexture;

        /// <inheritdoc/>
        public int SortOrder { get; set; } = 50;

        ///  <inheritdoc/>
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(renderer is ScreenSurfaceRenderer))
                throw new Exception($"Renderer used with {nameof(SurfaceRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");

            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            _screen = surface;
        }

        ///  <inheritdoc/>
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            _cachedTexture?.Dispose();
            _cachedTexture = null;
            _screen = null;
        }

        ///  <inheritdoc/>
        public void OnSurfaceChanged(IRenderer renderer, IScreenSurface surface)
        {
            if (surface == null)
            {
                BackingTexture?.Dispose();
                BackingTexture = null;
                _cachedTexture?.Dispose();
                _cachedTexture = null;
                _screen = null;
            }
            else
            {
                _screen = surface;
                // BackingTexture is handled by CheckRefresh.
            }
        }

        ///  <inheritdoc/>
        public bool Refresh(IRenderer renderer, bool backingTextureChanged, bool isForced)
        {
            bool result = true;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || _screen.AbsoluteArea.Width != BackingTexture.Width || _screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, _screen.AbsoluteArea.Width, _screen.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                _cachedTexture?.Dispose();
                _cachedTexture = new Host.GameTexture(BackingTexture);
                result = true;
            }

            // Redraw is needed
            if (result || _screen.IsDirty || isForced)
            {
                Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
                Host.Global.GraphicsDevice.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, _baseRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                Font font = _screen.Font;
                Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
                ColoredGlyph cell;

                if (_screen.Surface.DefaultBackground.A != 0)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), font.SolidGlyphRectangle.ToMonoRectangle(), _screen.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                int rectIndex = 0;

                for (int y = 0; y < _screen.Surface.View.Height; y++)
                {
                    int i = ((y + _screen.Surface.ViewPosition.Y) * _screen.Surface.Width) + _screen.Surface.ViewPosition.X;

                    for (int x = 0; x < _screen.Surface.View.Width; x++)
                    {
                        cell = _screen.Surface[i];
                        cell.IsDirty = false;

                        if (cell.IsVisible)
                        {
                            if (cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != _screen.Surface.DefaultBackground)
                                Host.Global.SharedSpriteBatch.Draw(fontImage, _baseRenderer.CachedRenderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                            if (cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                                Host.Global.SharedSpriteBatch.Draw(fontImage, _baseRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                            foreach (CellDecorator decorator in cell.Decorators)
                                if (decorator.Color != SadRogue.Primitives.Color.Transparent)
                                    Host.Global.SharedSpriteBatch.Draw(fontImage, _baseRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(decorator.Glyph).ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
                        }

                        i++;
                        rectIndex++;
                    }
                }

                Host.Global.SharedSpriteBatch.End();
                Host.Global.GraphicsDevice.SetRenderTarget(null);

                result = true;
                _screen.IsDirty = false;
            }

            return result;
        }

        ///  <inheritdoc/>
        public void Composing()
        {
            if (_screen.Tint.A != 255)
                Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, Color.White);
        }

        ///  <inheritdoc/>
        public void Render() { }

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
        ~SurfaceRenderStep() =>
            Dispose(false);
    }
}
