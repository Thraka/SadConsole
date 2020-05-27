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
    /// Draws a <see cref="IScreenSurface"/>.
    /// </summary>
    /// <remarks>
    /// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
    /// </remarks>
    public class ScreenObjectRenderer : IRenderer
    {
        /// <summary>
        /// Name of this renderer type.
        /// </summary>
        public static string Name => "screenobject";

        public RenderTarget2D BackingTexture;

        protected XnaRectangle[] _renderRects;

        ///  <inheritdoc/>
        public virtual void Attach(ISurfaceRenderData screen)
        {
        }

        ///  <inheritdoc/>
        public virtual void Detatch(ISurfaceRenderData screen)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
        }

        ///  <inheritdoc/>
        public virtual void Render(ISurfaceRenderData screen)
        {
            // If the tint is covering the whole area, don't draw anything
            if (screen.Tint.A != 255)
            {
                // Draw call for surface
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

                if (screen is IScreenObject screenObject)
                {
                    // Draw call for cursors
                    foreach (var cursor in screenObject.GetSadComponents<Components.Cursor>())
                    {
                        if (cursor.IsVisible && screen.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) && screen.Surface.View.Contains(cursor.Position))
                        {
                            GameHost.Instance.DrawCalls.Enqueue(
                                new DrawCalls.DrawCallCell(cursor.CursorRenderCell,
                                                           ((SadConsole.Host.GameTexture)screen.Font.Image).Texture,
                                                           new XnaRectangle(screen.AbsoluteArea.Position.ToMonoPoint() + screen.Font.GetRenderRect(cursor.Position.X - screen.Surface.ViewPosition.X, cursor.Position.Y - screen.Surface.ViewPosition.Y, screen.FontSize).ToMonoRectangle().Location, screen.FontSize.ToMonoPoint()),
                                                           screen.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                                           screen.Font.GlyphRects[cursor.CursorRenderCell.Glyph].ToMonoRectangle()
                                                          )
                                );
                        }
                    }
                }
            }

            // If tint is visible, draw it
            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToMonoColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToMonoRectangle(), screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }

        ///  <inheritdoc/>
        public virtual void Refresh(ISurfaceRenderData screen, bool force = false)
        {
            if (!force && !screen.IsDirty && BackingTexture != null) return;

            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != BackingTexture.Width || screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(MonoGame.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || _renderRects[0].Width != screen.FontSize.X || _renderRects[0].Height != screen.FontSize.Y)
            {
                _renderRects = new XnaRectangle[screen.Surface.View.Width * screen.Surface.View.Height];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, screen.Surface.View.Width);
                    _renderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToMonoRectangle();
                }
            }

            // Render parts of the surface
            RefreshBegin(screen);

            if (screen.Tint.A != 255)
                RefreshCells(screen.Surface, screen.Font);

            RefreshEnd(screen);

            screen.IsDirty = false;
        }


        protected virtual void RefreshBegin(ISurfaceRenderData screen)
        {
            MonoGame.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            MonoGame.Global.GraphicsDevice.Clear(Color.Transparent);
            MonoGame.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
        }

        protected virtual void RefreshEnd(ISurfaceRenderData screen)
        {
            MonoGame.Global.SharedSpriteBatch.End();
            MonoGame.Global.GraphicsDevice.SetRenderTarget(null);
        }

        protected virtual void RefreshCells(ICellSurface cellSurface, Font font)
        {
            var fontImage = ((SadConsole.Host.GameTexture)font.Image).Texture;

            if (cellSurface.DefaultBackground.A != 0)
                MonoGame.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), font.GlyphRects[font.SolidGlyphIndex].ToMonoRectangle(), cellSurface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

            int rectIndex = 0;

            for (int y = 0; y < cellSurface.View.Height; y++)
            {
                int i = ((y + cellSurface.ViewPosition.Y) * cellSurface.BufferWidth) + cellSurface.ViewPosition.X;

                for (int x = 0; x < cellSurface.View.Width; x++)
                {
                    ColoredGlyph cell = cellSurface.Cells[i];

                    cell.IsDirty = false;

                    if (!cell.IsVisible) continue;

                    if (!cell.Background.Equals(Color.Transparent) && cell.Background != cellSurface.DefaultBackground)
                        MonoGame.Global.SharedSpriteBatch.Draw(fontImage, _renderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                    if (!cell.Foreground.Equals(Color.Transparent))
                        MonoGame.Global.SharedSpriteBatch.Draw(fontImage, _renderRects[rectIndex], font.GlyphRects[cell.Glyph].ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                    foreach (CellDecorator decorator in cell.Decorators)
                        if (!decorator.Color.Equals(Color.Transparent))
                            MonoGame.Global.SharedSpriteBatch.Draw(fontImage, _renderRects[rectIndex], font.GlyphRects[decorator.Glyph].ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);

                    i++;
                    rectIndex++;
                }
            }
        }

        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    _renderRects = null;

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
