using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="Console"/>.
    /// </summary>
    /// <remarks>
    /// This renderer only caches drawing of the surface's cells. When the <see cref="Render(IScreenSurface)"/> method is called, the cached surface is drawn, then the cursor (if required), and then a tint. This allows the cursor to move and animate on the surface without the entire surface being redrawn each frame.
    ///
    /// If the cursor is not visible, and there is not tint set, this renderer behaves exactly like <see cref="ScreenObjectRenderer"/>.
    /// </remarks>
    public class ConsoleRenderer : ScreenObjectRenderer
    {
        ///  <inheritdoc/>
        public override void Attach(ISurfaceObject screen)
        {
            if (!(screen is Console))
                throw new Exception($"The ConsoleRenderer must be added to a Console.");
        }

        ///  <inheritdoc/>
        public override void Render(ISurfaceObject screen)
        {
            var console = (Console)screen;

            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

            if (console.Cursor.IsVisible && console.IsValidCell(console.Cursor.Position.X, console.Cursor.Position.Y) && screen.Surface.View.Contains(console.Cursor.Position))
            {
                var cursorPosition = screen.AbsoluteArea.Position + screen.Font.GetRenderRect(console.Cursor.Position.X - console.ViewPosition.X, console.Cursor.Position.Y - console.ViewPosition.Y, console.FontSize).Position;
                
                GameHost.Instance.DrawCalls.Enqueue(
                    new DrawCalls.DrawCallCell(console.Cursor.CursorRenderCell,
                                               new SadRogue.Primitives.Rectangle(cursorPosition.X, cursorPosition.Y, screen.FontSize.X, screen.FontSize.Y).ToIntRect(),
                                               screen.Font,
                                               true
                                              )
                    );
            }

            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToSFMLColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToIntRect(), screen.Font.SolidGlyphRectangle.ToIntRect()));
        }

        ///  <inheritdoc/>
        public override void Refresh(ISurfaceObject screen, bool force = false)
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
                    var position = SadRogue.Primitives.Point.FromIndex(i, screen.Surface.View.Width);
                    _renderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToIntRect();
                }
            }
           
            // Rendering code from sadconsole
            RefreshBegin(screen);
            if (screen.Tint.A != 255)
                RefreshCells(screen.Surface, screen.Font);
            RefreshEnd(screen);

            screen.IsDirty = false;
        }
    }
}
