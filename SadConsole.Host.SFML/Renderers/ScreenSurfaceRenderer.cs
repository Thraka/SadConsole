using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="IScreenSurface"/>.
    /// </summary>
    /// <remarks>
    /// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
    /// </remarks>
    public class ScreenSurfaceRenderer : IRenderer
    {
        /// <summary>
        /// Color used with drawing the texture to the screen. Let's a surface become transparent.
        /// </summary>
        protected Color _finalDrawColor = SadRogue.Primitives.Color.White.ToSFMLColor();

        /// <summary>
        /// Name of this renderer type.
        /// </summary>
        public static string Name => "screenobject";

        /// <summary>
        /// A 0 to 255 value represening how opaque the <see cref="BackingTexture"/> is when drawn to the screen. 255 represents full visibility.
        /// </summary>
        public byte Opaqueness
        {
            get => _finalDrawColor.A;
            set => _finalDrawColor = new Color(_finalDrawColor.R, _finalDrawColor.G, _finalDrawColor.B, value);
        }

        /// <summary>
        /// The texture the surface is drawn to.
        /// </summary>
        public RenderTexture BackingTexture;

        /// <summary>
        /// Cached set of rectangles used in rendering each cell.
        /// </summary>
        protected IntRect[] _renderRects;

        ///  <inheritdoc/>
        public virtual void Attach(IScreenSurface screen)
        {
        }

        ///  <inheritdoc/>
        public virtual void Detatch(IScreenSurface screen)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
        }

        ///  <inheritdoc/>
        public virtual void Render(IScreenSurface screen)
        {
            // If the tint isn't covering everything
            if (screen.Tint.A != 255)
            {
                // Draw call for surface
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y), _finalDrawColor));

                // Draw any cursors
                foreach (var cursor in screen.GetSadComponents<Components.Cursor>())
                {
                    if (cursor.IsVisible && screen.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) && screen.Surface.View.Contains(cursor.Position))
                    {
                        var cursorPosition = screen.AbsoluteArea.Position + screen.Font.GetRenderRect(cursor.Position.X - screen.Surface.ViewPosition.X, cursor.Position.Y - screen.Surface.ViewPosition.Y, screen.FontSize).Position;

                        GameHost.Instance.DrawCalls.Enqueue(
                            new DrawCalls.DrawCallCell(cursor.CursorRenderCell,
                                                        new SadRogue.Primitives.Rectangle(cursorPosition.X, cursorPosition.Y, screen.FontSize.X, screen.FontSize.Y).ToIntRect(),
                                                        screen.Font,
                                                        true
                                                        )
                            );
                    }
                }
            }

            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToSFMLColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToIntRect(), screen.Font.SolidGlyphRectangle.ToIntRect()));
        }

        ///  <inheritdoc/>
        public virtual void Refresh(IScreenSurface screen, bool force = false)
        {
            if (!force && !screen.IsDirty && BackingTexture != null) return;

            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != (int)BackingTexture.Size.X || screen.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTexture((uint)screen.AbsoluteArea.Width, (uint)screen.AbsoluteArea.Height);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || _renderRects[0].Width != screen.FontSize.X || _renderRects[0].Height != screen.FontSize.Y)
            {
                _renderRects = new IntRect[screen.Surface.View.Width * screen.Surface.View.Height];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = Point.FromIndex(i, screen.Surface.View.Width);
                    _renderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToIntRect();
                }
            }

            // Render parts of the surface
            RefreshBegin(screen);

            if (screen.Tint.A != 255)
                RefreshCells(screen.Surface, screen.Font);

            RefreshEnd(screen);

            screen.IsDirty = false;
        }


        /// <summary>
        /// Starts the sprite batch with the <see cref="BackingTexture"/>.
        /// </summary>
        /// <param name="surface">Object being used with the sprite batch.</param>
        protected virtual void RefreshBegin(IScreenSurface surface)
        {
            BackingTexture.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Reset(BackingTexture, RenderStates.Default, Transform.Identity);
        }

        /// <summary>
        /// Ends the sprite batch.
        /// </summary>
        /// <param name="surface">Object being used with the sprite batch.</param>
        protected virtual void RefreshEnd(IScreenSurface surface)
        {
            Host.Global.SharedSpriteBatch.End();
            BackingTexture.Display();
        }

        /// <summary>
        /// Draws each cell with the sprite batch.
        /// </summary>
        /// <param name="surface">The surface being drawn.</param>
        /// <param name="font">The font used with drawing.</param>
        protected virtual void RefreshCells(ICellSurface surface, Font font)
        {
            if (surface.DefaultBackground.A != 0)
                Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y), font.SolidGlyphRectangle.ToIntRect(), surface.DefaultBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)font.Image).Texture);

            int rectIndex = 0;

            for (int y = 0; y < surface.View.Height; y++)
            {
                int i = ((y + surface.ViewPosition.Y) * surface.BufferWidth) + surface.ViewPosition.X;

                for (int x = 0; x < surface.View.Width; x++)
                {
                    ColoredGlyph cell = surface.Cells[i];

                    cell.IsDirty = false;

                    if (!cell.IsVisible) continue;

                    Host.Global.SharedSpriteBatch.DrawCell(cell, _renderRects[rectIndex], !cell.Background.Equals(Color.Transparent) && cell.Background != surface.DefaultBackground, font);

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
                {
                    _renderRects = null;
                }

                BackingTexture?.Dispose();
                BackingTexture = null;

                disposedValue = true;
            }
        }

         ~ScreenSurfaceRenderer()
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
