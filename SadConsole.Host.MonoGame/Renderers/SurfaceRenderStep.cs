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
        private Host.GameTexture _cachedTexture;

        /// <summary>
        /// The cached texture of the drawn surface.
        /// </summary>
        public RenderTarget2D BackingTexture { get; private set; }

        /// <inheritdoc/>//
        public ITexture CachedTexture => _cachedTexture;

        /// <inheritdoc/>
        public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Surface;

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
            bool result = true;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != BackingTexture.Width || screenObject.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                _cachedTexture?.Dispose();
                _cachedTexture = new Host.GameTexture(BackingTexture);
                result = true;
            }

            var monoRenderer = (ScreenSurfaceRenderer)renderer;

            // Redraw is needed
            if (result || screenObject.IsDirty || isForced)
            {
                Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
                Host.Global.GraphicsDevice.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, monoRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                IFont font = screenObject.Font;
                Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
                ColoredGlyph cell;

                if (screenObject.Surface.DefaultBackground.A != 0)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), font.SolidGlyphRectangle.ToMonoRectangle(), screenObject.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                int rectIndex = 0;

                for (int y = 0; y < screenObject.Surface.View.Height; y++)
                {
                    int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

                    for (int x = 0; x < screenObject.Surface.View.Width; x++)
                    {
                        cell = screenObject.Surface[i];
                        cell.IsDirty = false;

                        if (cell.IsVisible)
                        {
                            if (cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != screenObject.Surface.DefaultBackground)
                                Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                            if (cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                                Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                            foreach (CellDecorator decorator in cell.Decorators)
                                if (decorator.Color != SadRogue.Primitives.Color.Transparent)
                                    Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(decorator.Glyph).ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
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
            Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, Color.White);
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
}
