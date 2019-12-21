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
        public override void Attach(ISurfaceObject screen)
        {
            if (!(screen is ControlsConsole))
                throw new Exception($"The {nameof(ConsoleRenderer)} must be added to a {nameof(ControlsConsole)}.");
        }

        ///  <inheritdoc/>
        public override void Render(ISurfaceObject screen)
        {
            var console = (SadConsole.UI.Window)screen;
            var theme = (SadConsole.UI.Themes.Window)console.Theme;

            if (console.IsModal && theme.ModalTint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(theme.ModalTint.ToMonoColor(), ((SadConsole.MonoGame.GameTexture)screen.Font.Image).Texture, new Microsoft .Xna.Framework.Rectangle(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), screen.Font.SolidGlyphRectangle.ToMonoRectangle()));

            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

            if (console.Cursor.IsVisible && console.IsValidCell(console.Cursor.Position.X, console.Cursor.Position.Y) && screen.Surface.View.Contains(console.Cursor.Position))
            {
                GameHost.Instance.DrawCalls.Enqueue(
                    new DrawCalls.DrawCallCell(console.Cursor.CursorRenderCell,
                                               ((SadConsole.MonoGame.GameTexture)screen.Font.Image).Texture,
                                               new XnaRectangle(screen.AbsoluteArea.Position.ToMonoPoint() + screen.Font.GetRenderRect(console.Cursor.Position.X, console.Cursor.Position.Y, console.FontSize).ToMonoRectangle().Location, screen.FontSize.ToMonoPoint()),
                                               screen.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                               screen.Font.GlyphRects[console.Cursor.CursorRenderCell.Glyph].ToMonoRectangle()
                                              )
                    );
            }

            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToMonoColor(), ((SadConsole.MonoGame.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToMonoRectangle(), screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }
    }
}
