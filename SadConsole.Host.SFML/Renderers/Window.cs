using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="Window"/>.
    /// </summary>
    /// <remarks>
    /// This renderer only caches drawing of the surface's cells. When the <see cref="Render(IScreenSurface)"/> method is called, the cached surface is drawn, then the cursor (if required), and then a tint. This allows the cursor to move and animate on the surface without the entire surface being redrawn each frame.
    ///
    /// If the cursor is not visible, and there is not tint set, this renderer behaves exactly like <see cref="ScreenObjectRenderer"/>.
    /// </remarks>
    public class Window : ControlsConsole
    {
        ///  <inheritdoc/>
        public override void Attach(ISurfaceRenderData screen)
        {
            if (!(screen is ControlsConsole))
                throw new Exception($"The {nameof(ConsoleRenderer)} must be added to a {nameof(ControlsConsole)}.");
        }

        ///  <inheritdoc/>
        public override void Render(ISurfaceRenderData screen)
        {
            var console = (SadConsole.UI.Window)screen;
            var theme = (SadConsole.UI.Themes.Window)console.Theme;

            if (console.IsModal && theme.ModalTint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(theme.ModalTint.ToSFMLColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, new IntRect(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), screen.Font.SolidGlyphRectangle.ToIntRect()));

            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

            if (console.Cursor.IsVisible && console.IsValidCell(console.Cursor.Position.X, console.Cursor.Position.Y) && screen.Surface.View.Contains(console.Cursor.Position))
            {
                var cursorPosition = screen.AbsoluteArea.Position + screen.Font.GetRenderRect(console.Cursor.Position.X, console.Cursor.Position.Y, console.FontSize).Position;
                
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
    }
}
