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
        }

        public void Detatch(ScreenObjectSurface screenObject)
        {
            BackingTexture.Dispose();
            BackingTexture = null;
        }

        public void Render(ScreenObjectSurface screenObject)
        {
            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screenObject.AbsoluteArea.Position.X, screenObject.AbsoluteArea.Position.Y)));
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
            if (_renderRects == null || _renderRects.Length != screenObject.Surface.Width * screenObject.Surface.Height || _renderRects[0].Width != screenObject.FontSize.X || _renderRects[0].Height != screenObject.FontSize.Y)
            {
                _renderRects = new XnaRectangle[screenObject.Surface.Width * screenObject.Surface.Height];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, screenObject.Surface.Width);
                    _renderRects[i] = screenObject.Font.GetRenderRect(position.X, position.Y, screenObject.FontSize).ToMonoRectangle();
                }
            }
           
            // Rendering code from sadconsole
            RenderBegin(screenObject);
            RenderCells(screenObject);
            RenderTint(screenObject);
            RenderEnd(screenObject);
        }


        protected virtual void RenderBegin(ScreenObjectSurface screenObject)
        {
            MonoGame.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            MonoGame.Global.GraphicsDevice.Clear(Color.Transparent);
            MonoGame.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
        }

        protected virtual void RenderEnd(ScreenObjectSurface screenObject)
        {
            MonoGame.Global.SharedSpriteBatch.End();
            MonoGame.Global.GraphicsDevice.SetRenderTarget(null);
        }

        protected virtual void RenderCells(ScreenObjectSurface screenObject)
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

                for (int y = 0; y < cellSurface.Height; y++)
                {
                    int i = ((y + cellSurface.BufferPosition.Y) * cellSurface.BufferWidth) + cellSurface.BufferPosition.X;

                    for (int x = 0; x < cellSurface.Width; x++)
                    {
                        ref Cell cell = ref cellSurface.Cells[i];

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
