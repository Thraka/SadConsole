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
    /// This renderer only caches drawing of the surface's cells. When the <see cref="Render(ISurfaceRenderData)"/> method is called, the cached surface is drawn, then the cursor (if required), and then a tint. This allows the cursor to move and animate on the surface without the entire surface being redrawn each frame.
    ///
    /// If the cursor is not visible, and there is not tint set, this renderer behaves exactly like <see cref="ScreenObjectRenderer"/>.
    /// </remarks>
    public class Window : ControlsConsole
    {
        /// <summary>
        /// Name of this renderer type.
        /// </summary>
        public static new string Name => "window";

        ///  <inheritdoc/>
        public override void Attach(ISurfaceRenderData screen)
        {
            if (!(screen is UI.Window))
                throw new Exception($"The Window Renderer must be added to a {nameof(UI.Window)}.");
        }

        ///  <inheritdoc/>
        public override void Render(ISurfaceRenderData screen)
        {
            var console = (SadConsole.UI.Window)screen;
            var colors = console.ControlHostComponent.GetThemeColors();

            if (console.IsModal && colors.ModalBackground.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(colors.ModalBackground.ToMonoColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, new Microsoft .Xna.Framework.Rectangle(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), screen.Font.SolidGlyphRectangle.ToMonoRectangle()));

            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

            if (console.Cursor.IsVisible && console.IsValidCell(console.Cursor.Position.X, console.Cursor.Position.Y) && screen.Surface.View.Contains(console.Cursor.Position))
            {
                GameHost.Instance.DrawCalls.Enqueue(
                    new DrawCalls.DrawCallCell(console.Cursor.CursorRenderCell,
                                               ((SadConsole.Host.GameTexture)screen.Font.Image).Texture,
                                               new XnaRectangle(screen.AbsoluteArea.Position.ToMonoPoint() + screen.Font.GetRenderRect(console.Cursor.Position.X, console.Cursor.Position.Y, console.FontSize).ToMonoRectangle().Location, screen.FontSize.ToMonoPoint()),
                                               screen.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                               screen.Font.GlyphRects[console.Cursor.CursorRenderCell.Glyph].ToMonoRectangle()
                                              )
                    );
            }

            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToMonoColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToMonoRectangle(), screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }
    }
}
