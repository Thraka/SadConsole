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
    /// Draws a <see cref="ScreenObjectSurface"/>.
    /// </summary>
    /// <remarks>
    /// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
    /// </remarks>
    public class ScreenObjectRenderer : IRenderer
    {
        public RenderTarget2D BackingTexture;

        private XnaRectangle[] _renderRects;

        public void Attach(ScreenObjectSurface surface)
        {
        }

        public void Detatch(ScreenObjectSurface surface)
        {
            BackingTexture.Dispose();
            BackingTexture = null;
        }

        public void Render(ScreenObjectSurface surface)
        {
            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(surface.AbsoluteArea.Position.X, surface.AbsoluteArea.Position.Y)));
        }

        public void Refresh(ScreenObjectSurface surface)
        {
            if (!surface.IsDirty) return;

            // Update texture if something is out of size.
            if (BackingTexture == null || surface.AbsoluteArea.Width != BackingTexture.Width || surface.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(MonoGame.Global.GraphicsDevice, surface.AbsoluteArea.Width, surface.AbsoluteArea.Height, false, MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != surface.Surface.ViewWidth * surface.Surface.ViewHeight || _renderRects[0].Width != surface.FontSize.X || _renderRects[0].Height != surface.FontSize.Y)
            {
                _renderRects = new XnaRectangle[surface.Surface.ViewWidth * surface.Surface.ViewHeight];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, surface.Surface.ViewWidth);
                    _renderRects[i] = surface.Font.GetRenderRect(position.X, position.Y, surface.FontSize).ToMonoRectangle();
                }
            }

            // Render parts of the surface
            RenderBegin(surface);
            RenderCells(surface);
            RenderTint(surface);
            RenderEnd(surface);

            surface.IsDirty = false;
        }


        protected virtual void RenderBegin(ScreenObjectSurface surface)
        {
            MonoGame.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            MonoGame.Global.GraphicsDevice.Clear(Color.Transparent);
            MonoGame.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
        }

        protected virtual void RenderEnd(ScreenObjectSurface surface)
        {
            MonoGame.Global.SharedSpriteBatch.End();
            MonoGame.Global.GraphicsDevice.SetRenderTarget(null);
        }

        protected virtual void RenderCells(ScreenObjectSurface surface)
        {
            var cellSurface = surface.Surface;
            if (surface.Tint.A != 255)
            {
                var font = ((SadConsole.MonoGame.GameTexture)surface.Font.Image).Texture;

                if (cellSurface.DefaultBackground.A != 0)
                    MonoGame.Global.SharedSpriteBatch.Draw(font, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), surface.Font.GlyphRects[surface.Font.SolidGlyphIndex].ToMonoRectangle(), cellSurface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                int rectIndex = 0;

                for (int y = 0; y < cellSurface.ViewHeight; y++)
                {
                    int i = ((y + cellSurface.ViewPosition.Y) * cellSurface.BufferWidth) + cellSurface.ViewPosition.X;

                    for (int x = 0; x < cellSurface.ViewWidth; x++)
                    {
                        ref ColoredGlyph cell = ref cellSurface.Cells[i];

                        if (!cell.IsVisible) continue;

                        if (!cell.Background.Equals(Color.Transparent) && cell.Background != cellSurface.DefaultBackground)
                            MonoGame.Global.SharedSpriteBatch.Draw(font, _renderRects[rectIndex], surface.Font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                        if (!cell.Foreground.Equals(Color.Transparent))
                            MonoGame.Global.SharedSpriteBatch.Draw(font, _renderRects[rectIndex], surface.Font.GlyphRects[cell.Glyph].ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                        foreach (CellDecorator decorator in cell.Decorators)
                            if (!decorator.Color.Equals(Color.Transparent))
                                MonoGame.Global.SharedSpriteBatch.Draw(font, _renderRects[rectIndex], surface.Font.GlyphRects[decorator.Glyph].ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);

                        cell.IsDirty = false;

                        i++;
                        rectIndex++;
                    }
                }
            }
        }

        protected virtual void RenderTint(ScreenObjectSurface screenObject)
        {
            if (screenObject.Tint.A != 0)
            {
                MonoGame.Global.SharedSpriteBatch.Draw(((SadConsole.MonoGame.GameTexture)screenObject.Font.Image).Texture, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), screenObject.Font.GlyphRects[screenObject.Font.SolidGlyphIndex].ToMonoRectangle(), screenObject.Tint.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                BackingTexture?.Dispose();
                BackingTexture = null;

                disposedValue = true;
            }
        }

         ~ScreenObjectRenderer()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
