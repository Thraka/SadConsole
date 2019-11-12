using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;

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
        public RenderTexture BackingTexture;

        private IntRect[] _renderRects;

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
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(surface.AbsoluteArea.Position.X, surface.AbsoluteArea.Position.Y)));
        }

        public void Refresh(ScreenObjectSurface surface)
        {
            if (!surface.IsDirty) return;

            // Update texture if something is out of size.
            if (BackingTexture == null || surface.AbsoluteArea.Width != (int)BackingTexture.Size.X || surface.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTexture((uint)surface.AbsoluteArea.Width, (uint)surface.AbsoluteArea.Height);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != surface.Surface.ViewWidth * surface.Surface.ViewHeight || _renderRects[0].Width != surface.FontSize.X || _renderRects[0].Height != surface.FontSize.Y)
            {
                _renderRects = new IntRect[surface.Surface.ViewWidth * surface.Surface.ViewHeight];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, surface.Surface.ViewWidth);
                    _renderRects[i] = surface.Font.GetRenderRect(position.X, position.Y, surface.FontSize).ToSFML();
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
            BackingTexture.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Reset(BackingTexture, RenderStates.Default, Transform.Identity);
        }

        protected virtual void RenderEnd(ScreenObjectSurface surface)
        {
            Host.Global.SharedSpriteBatch.End();
            BackingTexture.Display();
        }

        protected virtual void RenderCells(ScreenObjectSurface surface)
        {
            var cellSurface = surface.Surface;
            if (surface.Tint.A != 255)
            {
                if (cellSurface.DefaultBackground.A != 0)
                    Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y), surface.Font.GlyphRects[surface.Font.SolidGlyphIndex].ToSFML(), cellSurface.DefaultBackground.ToSFML(), ((SadConsole.Host.GameTexture)surface.Font.Image).Texture);

                int rectIndex = 0;

                for (int y = 0; y < cellSurface.ViewHeight; y++)
                {
                    int i = ((y + cellSurface.ViewPosition.Y) * cellSurface.BufferWidth) + cellSurface.ViewPosition.X;

                    for (int x = 0; x < cellSurface.ViewWidth; x++)
                    {
                        ref ColoredGlyph cell = ref cellSurface.Cells[i];

                        if (!cell.IsVisible) continue;

                        Host.Global.SharedSpriteBatch.DrawCell(cell, _renderRects[rectIndex], !cell.Background.Equals(Color.Transparent) && cell.Background != cellSurface.DefaultBackground, surface.Font);

                        cell.IsDirty = false;

                        i++;
                        rectIndex++;
                    }
                }
            }
        }

        protected virtual void RenderTint(ScreenObjectSurface surface)
        {
            if (surface.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(surface.Tint.ToSFML(), ((SadConsole.Host.GameTexture)surface.Font.Image).Texture, surface.AbsoluteArea.ToSFML(), surface.Font.SolidGlyphRectangle.ToSFML()));
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
