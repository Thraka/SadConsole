using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="SadConsole.IScreenSurface"/> object.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Surface (Dirty)")]
    public class SurfaceDirtyCellsRenderStep : IRenderStep, IRenderStepTexture
    {
        private Host.GameTexture _cachedTexture;

        /// <summary>
        /// The cached texture of the drawn surface.
        /// </summary>
        public RenderTarget2D BackingTexture { get; private set; }

        /// <inheritdoc/>//
        public ITexture CachedTexture => _cachedTexture;

        /// <inheritdoc/>
        public string Name => Constants.RenderStepNames.SurfaceDirtyCells;

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
            bool fullRedraw = false;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != BackingTexture.Width || screenObject.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
                _cachedTexture?.Dispose();
                _cachedTexture = new Host.GameTexture(BackingTexture);
                fullRedraw = true;
            }

            var monoRenderer = (ScreenSurfaceRenderer)renderer;
            
            // Redraw is needed
            if (fullRedraw || screenObject.IsDirty || isForced)
            {
                Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);

                // Only cleared when full redraw needed
                if (fullRedraw)
                    Host.Global.GraphicsDevice.Clear(Color.Transparent);

                Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, monoRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                IFont font = screenObject.Font;
                Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
                ColoredGlyph cell;

                if (fullRedraw)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), font.SolidGlyphRectangle.ToMonoRectangle(), screenObject.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                int rectIndex = 0;

                for (int y = 0; y < screenObject.Surface.View.Height; y++)
                {
                    int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

                    for (int x = 0; x < screenObject.Surface.View.Width; x++)
                    {
                        cell = screenObject.Surface[i];

                        if (cell.IsDirty || fullRedraw)
                        {
                            cell.IsDirty = false;

                            if (cell.IsVisible)
                            {
                                Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                                if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                                    Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                                for (int d = 0; d < cell.Decorators.Length; d++)
                                    if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                        Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), cell.Decorators[d].Color.ToMonoColor(), 0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
                            }
                            else
                                // Draw the default background
                                Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), screenObject.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
                        }
                        i++;
                        rectIndex++;
                    }
                }

                Host.Global.SharedSpriteBatch.End();
                Host.Global.GraphicsDevice.SetRenderTarget(null);

                fullRedraw = true;
                screenObject.IsDirty = false;
            }

            return fullRedraw;
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
        ~SurfaceDirtyCellsRenderStep() =>
            Dispose(false);
    }
}
