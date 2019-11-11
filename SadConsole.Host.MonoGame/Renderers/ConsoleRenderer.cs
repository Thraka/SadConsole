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
    /// Draws a <see cref="Console"/>.
    /// </summary>
    /// <remarks>
    /// This renderer only caches drawing of the surface's cells. When the <see cref="Render(ScreenObjectSurface)"/> method is called, the cached surface is drawn, then the cursor (if required), and then a tint. This allows the cursor to move and animate on the surface without the entire surface being redrawn each frame.
    ///
    /// If the cursor is not visible, and there is not tint set, this renderer behaves exactly like <see cref="ScreenObjectRenderer"/>.
    /// </remarks>
    public class ConsoleRenderer : IRenderer
    {
        public RenderTarget2D BackingTexture;

        private XnaRectangle[] _renderRects;

        ///  <inheritdoc/>
        public void Attach(ScreenObjectSurface surface)
        {
            if (!(surface is Console))
                throw new Exception($"The ConsoleRenderer must be added to a Console.");
        }

        ///  <inheritdoc/>
        public void Detatch(ScreenObjectSurface surface)
        {
            BackingTexture.Dispose();
            BackingTexture = null;
        }

        ///  <inheritdoc/>
        public void Render(ScreenObjectSurface surface)
        {
            var console = (Console)surface;

            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(surface.AbsoluteArea.Position.X, surface.AbsoluteArea.Position.Y)));

            if (console.Cursor.IsVisible && console.Surface.IsValidCell(console.Cursor.Position.X, console.Cursor.Position.Y) && surface.Surface.GetViewRectangle().Contains(console.Cursor.Position))
            {
                GameHost.Instance.DrawCalls.Enqueue(
                    new DrawCalls.DrawCallCell(console.Cursor.CursorRenderCell,
                                               ((SadConsole.MonoGame.GameTexture)surface.Font.Image).Texture,
                                               new XnaRectangle(surface.AbsolutePosition.ToMonoPoint() + surface.Font.GetRenderRect(console.Cursor.Position.X, console.Cursor.Position.Y, console.FontSize).ToMonoRectangle().Location, surface.FontSize.ToMonoPoint()),
                                               surface.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                               surface.Font.GlyphRects[console.Cursor.CursorRenderCell.Glyph].ToMonoRectangle()
                                              )
                    );
            }

            if (surface.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(surface.Tint.ToMonoColor(), ((SadConsole.MonoGame.GameTexture)surface.Font.Image).Texture, surface.AbsoluteArea.ToMonoRectangle(), surface.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }

        ///  <inheritdoc/>
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
           
            // Rendering code from sadconsole
            RenderBegin(surface);
            RenderCells(surface);
            RenderEnd(surface);

            surface.IsDirty = false;
        }

        protected void RenderBegin(ScreenObjectSurface surface)
        {
            MonoGame.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            MonoGame.Global.GraphicsDevice.Clear(Color.Transparent);
            MonoGame.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
        }

        protected void RenderEnd(ScreenObjectSurface surface)
        {
            MonoGame.Global.SharedSpriteBatch.End();
            MonoGame.Global.GraphicsDevice.SetRenderTarget(null);
        }

        protected void RenderCells(ScreenObjectSurface surface)
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

         ~ConsoleRenderer()
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
