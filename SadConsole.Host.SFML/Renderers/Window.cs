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
    /// If the cursor is not visible, and there is not tint set, this renderer behaves exactly like <see cref="ScreenSurfaceRenderer"/>.
    /// </remarks>
    public class Window : ControlsConsole
    {
        /// <summary>
        /// Name of this renderer type.
        /// </summary>
        public static new string Name => "window";

        ///  <inheritdoc/>
        public override void Attach(IScreenSurface screen)
        {
            if (!(screen is UI.Window))
                throw new Exception($"The Window Renderer must be added to a {nameof(UI.Window)}.");
        }

        ///  <inheritdoc/>
        public override void Render(IScreenSurface screen)
        {
            var window = (SadConsole.UI.Window)screen;
            var colors = window.ControlHostComponent.GetThemeColors();

            if (window.IsModal && colors.ModalBackground.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(colors.ModalBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, new IntRect(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), screen.Font.SolidGlyphRectangle.ToIntRect()));

            if (screen.Tint.A != 255)
            {
                // Draw call for window
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y), _finalDrawColor));

                // Draw call for controls
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTextureControls.Texture, new SFML.System.Vector2i(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y), _finalDrawColor));

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
    }
}
