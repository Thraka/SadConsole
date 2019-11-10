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
    public class ConsoleRenderer : IRenderer
    {
        public RenderTarget2D BackingTexture;

        private XnaRectangle[] _renderRects;

        public void Attach(ScreenObjectSurface screenObject)
        {
            if (!(screenObject is Console))
                throw new Exception($"The ConsoleRenderer must be added to a Console.");
        }

        public void Detatch(ScreenObjectSurface screenObject)
        {
            BackingTexture.Dispose();
            BackingTexture = null;
        }

        public void Render(ScreenObjectSurface screenObject)
        {
            var console = (Console)screenObject;

            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screenObject.AbsoluteArea.Position.X, screenObject.AbsoluteArea.Position.Y)));

            var viewArea = new XnaRectangle(console.Surface.BufferPosition.ToMonoPoint(), new Microsoft.Xna.Framework.Point(console.Surface.BufferWidth, console.Surface.BufferHeight));

            if (console.Cursor.IsVisible && console.Surface.IsValidCell(console.Cursor.Position.X, console.Cursor.Position.Y) && viewArea.Contains(console.Cursor.Position.ToMonoPoint()))
            {
                GameHost.Instance.DrawCalls.Enqueue(
                    new DrawCalls.DrawCallCell(console.Cursor.CursorRenderCell,
                                               ((SadConsole.MonoGame.GameTexture)screenObject.Font.Image).Texture,
                                               new XnaRectangle(screenObject.AbsolutePosition.ToMonoPoint() + screenObject.Font.GetRenderRect(console.Cursor.Position.X, console.Cursor.Position.Y, console.FontSize).ToMonoRectangle().Location, screenObject.FontSize.ToMonoPoint()),
                                               screenObject.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                               screenObject.Font.GlyphRects[console.Cursor.CursorRenderCell.Glyph].ToMonoRectangle()
                                              )
                    );
            }

            if (screenObject.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screenObject.Tint.ToMonoColor(), ((SadConsole.MonoGame.GameTexture)screenObject.Font.Image).Texture, screenObject.AbsoluteArea.ToMonoRectangle(), screenObject.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }

        public void Refresh(ScreenObjectSurface screenObject)
        { 
            // Update texture if something is out of size.
            if (BackingTexture == null || screenObject.AbsoluteArea.Width != BackingTexture.Width || screenObject.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(MonoGame.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != screenObject.Surface.ViewWidth * screenObject.Surface.ViewHeight || _renderRects[0].Width != screenObject.FontSize.X || _renderRects[0].Height != screenObject.FontSize.Y)
            {
                _renderRects = new XnaRectangle[screenObject.Surface.ViewWidth * screenObject.Surface.ViewHeight];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, screenObject.Surface.ViewWidth);
                    _renderRects[i] = screenObject.Font.GetRenderRect(position.X, position.Y, screenObject.FontSize).ToMonoRectangle();
                }
            }
           
            // Rendering code from sadconsole
            RenderBegin(screenObject);
            RenderCells(screenObject);
            RenderEnd(screenObject);
        }

        protected void RenderBegin(ScreenObjectSurface screenObject)
        {
            MonoGame.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            MonoGame.Global.GraphicsDevice.Clear(Color.Transparent);
            MonoGame.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
        }

        protected void RenderEnd(ScreenObjectSurface screenObject)
        {
            MonoGame.Global.SharedSpriteBatch.End();
            MonoGame.Global.GraphicsDevice.SetRenderTarget(null);
        }

        protected void RenderCells(ScreenObjectSurface screenObject)
        {
            var cellSurface = screenObject.Surface;
            if (screenObject.Tint.A != 255)
            {
                var font = ((SadConsole.MonoGame.GameTexture)screenObject.Font.Image).Texture;

                if (cellSurface.DefaultBackground.A != 0)
                {
                    MonoGame.Global.SharedSpriteBatch.Draw(font, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), screenObject.Font.GlyphRects[screenObject.Font.SolidGlyphIndex].ToMonoRectangle(), cellSurface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
                }

                int rectIndex = 0;

                for (int y = 0; y < cellSurface.ViewHeight; y++)
                {
                    int i = ((y + cellSurface.BufferPosition.Y) * cellSurface.BufferWidth) + cellSurface.BufferPosition.X;

                    for (int x = 0; x < cellSurface.ViewWidth; x++)
                    {
                        ref ColoredGlyph cell = ref cellSurface.Cells[i];

                        if (!cell.IsVisible)
                        {
                            continue;
                        }

                        if (!cell.Background.Equals(Color.Transparent) && cell.Background != cellSurface.DefaultBackground)
                        {
                            MonoGame.Global.SharedSpriteBatch.Draw(font, _renderRects[rectIndex], screenObject.Font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
                        }

                        if (!cell.Foreground.Equals(Color.Transparent))
                        {
                            MonoGame.Global.SharedSpriteBatch.Draw(font, _renderRects[rectIndex], screenObject.Font.GlyphRects[cell.Glyph].ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);
                        }

                        foreach (CellDecorator decorator in cell.Decorators)
                        {
                            if (!decorator.Color.Equals(Color.Transparent))
                            {
                                MonoGame.Global.SharedSpriteBatch.Draw(font, _renderRects[rectIndex], screenObject.Font.GlyphRects[decorator.Glyph].ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
                            }
                        }

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
