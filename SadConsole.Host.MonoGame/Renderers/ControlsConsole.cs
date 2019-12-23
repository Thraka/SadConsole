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
    /// Draws a <see cref="ControlsConsole"/>.
    /// </summary>
    /// <remarks>
    /// This renderer only caches drawing of the surface's cells. When the <see cref="Render(IScreenSurface)"/> method is called, the cached surface is drawn, then the cursor (if required), and then a tint. This allows the cursor to move and animate on the surface without the entire surface being redrawn each frame.
    ///
    /// If the cursor is not visible, and there is not tint set, this renderer behaves exactly like <see cref="ScreenObjectRenderer"/>.
    /// </remarks>
    public class ControlsConsole : ScreenObjectRenderer
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
            var console = (Console)screen;

            // Draw call for texture
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

            if (console.Cursor.IsVisible && console.IsValidCell(console.Cursor.Position.X, console.Cursor.Position.Y) && screen.Surface.View.Contains(console.Cursor.Position))
            {
                GameHost.Instance.DrawCalls.Enqueue(
                    new DrawCalls.DrawCallCell(console.Cursor.CursorRenderCell,
                                               ((SadConsole.MonoGame.GameTexture)screen.Font.Image).Texture,
                                               new XnaRectangle(screen.AbsoluteArea.Position.ToMonoPoint() + screen.Font.GetRenderRect(console.Cursor.Position.X - console.ViewPosition.X, console.Cursor.Position.Y - console.ViewPosition.Y, console.FontSize).ToMonoRectangle().Location, screen.FontSize.ToMonoPoint()),
                                               screen.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                               screen.Font.GlyphRects[console.Cursor.CursorRenderCell.Glyph].ToMonoRectangle()
                                              )
                    );
            }

            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToMonoColor(), ((SadConsole.MonoGame.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToMonoRectangle(), screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }

        ///  <inheritdoc/>
        public override void Refresh(ISurfaceRenderData screen, bool force = false)
        {
            if (!force && !screen.IsDirty && BackingTexture != null) return;

            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != BackingTexture.Width || screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(MonoGame.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || _renderRects[0].Width != screen.FontSize.X || _renderRects[0].Height != screen.FontSize.Y)
            {
                _renderRects = new XnaRectangle[screen.Surface.View.Width * screen.Surface.View.Height];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, screen.Surface.View.Width);
                    _renderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToMonoRectangle();
                }
            }
           
            // Rendering code from sadconsole
            RefreshBegin(screen);

            if (screen.Tint.A != 255)
                RefreshCells(screen.Surface, screen.Font);

            foreach (SadConsole.UI.Controls.ControlBase control in ((SadConsole.UI.ControlsConsole)screen).Controls)
            {
                if (!control.IsVisible) continue;

                RenderControlCells(control);
            }

            RefreshEnd(screen);

            screen.IsDirty = false;
        }

        protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control)
        {
            Font font = control.AlternateFont ?? control.Parent.Font;

            var fontImage = ((SadConsole.MonoGame.GameTexture)font.Image).Texture;

            if (control.Surface.DefaultBackground.A != 0)
            {
                (int x, int y) = (control.Position - control.Parent.ViewPosition).SurfaceLocationToPixel(control.Parent.FontSize);
                (int width, int height) = new SadRogue.Primitives.Point(control.Surface.View.Width, control.Surface.View.Height) * control.Parent.FontSize;

                MonoGame.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, width, height), font.GlyphRects[font.SolidGlyphIndex].ToMonoRectangle(), control.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            }

            var parentViewRect = control.Parent.View;

            for (int i = 0; i < control.Surface.Cells.Length; i++)
            {
                ref ColoredGlyph cell = ref control.Surface.Cells[i];

                if (!cell.IsVisible) continue;

                SadRogue.Primitives.Point cellRenderPosition = SadRogue.Primitives.Point.FromIndex(i, control.Surface.View.Width) + control.Position;

                if (!parentViewRect.Contains(cellRenderPosition)) continue;

                XnaRectangle renderRect = _renderRects[(cellRenderPosition - control.Parent.ViewPosition).ToIndex(control.Parent.BufferWidth)];

                if (cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != control.Surface.DefaultBackground)
                {
                    MonoGame.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GlyphRects[font.SolidGlyphIndex].ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
                }

                if (cell.Foreground != SadRogue.Primitives.Color.Transparent)
                {
                    MonoGame.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GlyphRects[cell.Glyph].ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);
                }

                foreach (CellDecorator decorator in cell.Decorators)
                {
                    if (decorator.Color != SadRogue.Primitives.Color.Transparent)
                    {
                        MonoGame.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GlyphRects[decorator.Glyph].ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
                    }
                }
            }
        }
    }
}
