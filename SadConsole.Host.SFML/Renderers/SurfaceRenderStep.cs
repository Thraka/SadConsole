using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;
using System.Collections.Generic;

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
        public RenderTexture BackingTexture { get; private set; }

        /// <inheritdoc/>
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
            _baseRenderer = null;
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
                // BackingTexture is handled by prestart.
            }
        }

        ///  <inheritdoc/>
        public bool Refresh(IRenderer renderer, bool backingTextureChanged, bool isForced)
        {
            bool result = true;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || _screen.AbsoluteArea.Width != (int)BackingTexture.Size.X || _screen.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTexture((uint)_screen.AbsoluteArea.Width, (uint)_screen.AbsoluteArea.Height);
                _cachedTexture?.Dispose();
                _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
                result = true;
            }

            // Redraw is needed
            if (result || _screen.IsDirty || isForced)
            {
                BackingTexture.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(BackingTexture, _baseRenderer.SFMLBlendState, Transform.Identity);

                int rectIndex = 0;
                ColoredGlyph cell;
                Font font = _screen.Font;

                if (_screen.Surface.DefaultBackground.A != 0)
                    Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y), font.SolidGlyphRectangle.ToIntRect(), _screen.Surface.DefaultBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)font.Image).Texture);

                for (int y = 0; y < _screen.Surface.View.Height; y++)
                {
                    int i = ((y + _screen.Surface.ViewPosition.Y) * _screen.Surface.Width) + _screen.Surface.ViewPosition.X;

                    for (int x = 0; x < _screen.Surface.View.Width; x++)
                    {
                        cell = _screen.Surface[i];

                        cell.IsDirty = false;

                        if (cell.IsVisible)
                            Host.Global.SharedSpriteBatch.DrawCell(cell, _baseRenderer.CachedRenderRects[rectIndex], cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != _screen.Surface.DefaultBackground, font);

                        i++;
                        rectIndex++;
                    }
                }

                Host.Global.SharedSpriteBatch.End();
                BackingTexture.Display();

                result = true;
                _screen.IsDirty = false;
            }

            return result;
        }

        ///  <inheritdoc/>
        public void Composing()
        {
            if (_screen.Tint.A != 255)
            {
                IntRect outputArea = new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y);
                Host.Global.SharedSpriteBatch.DrawQuad(outputArea, outputArea, Color.White, BackingTexture.Texture);
            }
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
