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

        private XnaRectangle[] _rectangles;

        public void Attach(ScreenObjectSurface console)
        {
            // Update rectangles only
            if (BackingTexture != null && (console.AbsoluteArea.Width != BackingTexture.Width || console.AbsoluteArea.Height != BackingTexture.Height))
            {
                _rectangles = new XnaRectangle[console.RenderRects.Length];
                for (int i = 0; i < _rectangles.Length; i++)
                    _rectangles[i] = console.RenderRects[i].ToMonoRectangle();

                return;
            }

            if (BackingTexture != null)
                BackingTexture.Dispose();

            _rectangles = new XnaRectangle[console.RenderRects.Length];
            for (int i = 0; i < _rectangles.Length; i++)
                _rectangles[i] = console.RenderRects[i].ToMonoRectangle();

            BackingTexture = new RenderTarget2D(MonoGame.Global.GraphicsDevice, console.AbsoluteArea.Width, console.AbsoluteArea.Height, false, MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        }

        public void Detatch(ScreenObjectSurface console)
        {
            BackingTexture.Dispose();
        }

        public void Render(ScreenObjectSurface surface)
        {
            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, Vector2.Zero));
        }

        public void Refresh(ScreenObjectSurface surface)
        {
            


            // Rendering code from sadconsole
            RenderBegin(surface);
            RenderCells(surface);
            RenderTint(surface);
            RenderEnd(surface);
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
                var font = ((SadConsole.MonoGame.GameTexture)surface.Font.FontImage).Texture;

                if (cellSurface.DefaultBackground.A != 0)
                {
                    MonoGame.Global.SharedSpriteBatch.Draw(font, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), surface.Font.GlyphRects[surface.Font.SolidGlyphIndex].ToMonoRectangle(), cellSurface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
                }

                for (int i = 0; i < cellSurface.RenderCells.Length; i++)
                {
                    ref Cell cell = ref cellSurface.RenderCells[i];

                    if (!cell.IsVisible)
                    {
                        continue;
                    }

                    if (!cell.Background.Equals(Color.Transparent) && cell.Background != cellSurface.DefaultBackground)
                    {
                        MonoGame.Global.SharedSpriteBatch.Draw(font, _rectangles[i], surface.Font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
                    }

                    if (!cell.Foreground.Equals(Color.Transparent))
                    {
                        MonoGame.Global.SharedSpriteBatch.Draw(font, _rectangles[i], surface.Font.GlyphRects[cell.Glyph].ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);
                    }

                    foreach (CellDecorator decorator in cell.Decorators)
                    {
                        if (!decorator.Color.Equals(Color.Transparent))
                        {
                            MonoGame.Global.SharedSpriteBatch.Draw(font, _rectangles[i], surface.Font.GlyphRects[decorator.Glyph].ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
                        }
                    }
                }
            }
        }

        protected virtual void RenderTint(ScreenObjectSurface surface)
        {
            if (surface.Tint.A != 0)
            {
                MonoGame.Global.SharedSpriteBatch.Draw(((SadConsole.MonoGame.GameTexture)surface.Font.FontImage).Texture, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), surface.Font.GlyphRects[surface.Font.SolidGlyphIndex].ToMonoRectangle(), surface.Tint.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
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

                BackingTexture.Dispose();
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
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
