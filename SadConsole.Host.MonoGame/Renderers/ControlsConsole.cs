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
        public RenderTarget2D BackingTextureControls;

        /// <summary>
        /// Name of this renderer type.
        /// </summary>
        public static new string Name => "controls";

        ///  <inheritdoc/>
        public override void Attach(IScreenSurface screen)
        {
            if (screen.HasSadComponent<UI.ControlHost>(out _))
                return;

            throw new Exception($"The {nameof(ControlsConsole)} renderer must be added to an object contains a {nameof(UI.ControlHost)} component.");
        }

        ///  <inheritdoc/>
        public override void Detatch(IScreenSurface screen)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            BackingTextureControls?.Dispose();
            BackingTextureControls = null;
        }

        ///  <inheritdoc/>
        public override void Render(IScreenSurface screen)
        {
            // If the tint is covering the whole area, don't draw anything
            if (screen.Tint.A != 255)
            {
                // Draw call for surface
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

                // Draw call for controls
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTextureControls, new Vector2(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

                // Draw call for cursors
                foreach (var cursor in screen.GetSadComponents<Components.Cursor>())
                {

                    if (cursor.IsVisible && screen.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) && screen.Surface.View.Contains(cursor.Position))
                    {
                        GameHost.Instance.DrawCalls.Enqueue(
                            new DrawCalls.DrawCallCell(cursor.CursorRenderCell,
                                                        ((SadConsole.Host.GameTexture)screen.Font.Image).Texture,
                                                        new XnaRectangle(screen.AbsoluteArea.Position.ToMonoPoint() + screen.Font.GetRenderRect(cursor.Position.X - screen.Surface.ViewPosition.X, cursor.Position.Y - screen.Surface.ViewPosition.Y, screen.FontSize).ToMonoRectangle().Location, screen.FontSize.ToMonoPoint()),
                                                        screen.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                                        screen.Font.GetGlyphSourceRectangle(cursor.CursorRenderCell.Glyph).ToMonoRectangle()
                                                        )
                            );
                    }
                }
            }

            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToMonoColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToMonoRectangle(), screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }

        ///  <inheritdoc/>
        public override void Refresh(IScreenSurface screen, bool force = false)
        {
            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != BackingTexture.Width || screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(MonoGame.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                BackingTextureControls?.Dispose();
                BackingTextureControls = new RenderTarget2D(MonoGame.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                force = true;
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

            // Render the main console
            if (force || screen.IsDirty)
            {
                RefreshBegin(screen);

                if (screen.Tint.A != 255)
                    RefreshCells(screen.Surface, screen.Font);

                RefreshEnd(screen);
            }

            UI.ControlHost uiComponent = screen.GetSadComponent<UI.ControlHost>();

            // render controls
            if (force || uiComponent.IsDirty)
            {
                MonoGame.Global.GraphicsDevice.SetRenderTarget(BackingTextureControls);
                MonoGame.Global.GraphicsDevice.Clear(Color.Transparent);
                MonoGame.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                if (screen.Tint.A != 255)
                {
                    foreach (UI.Controls.ControlBase control in uiComponent.Controls)
                    {
                        if (!control.IsVisible) continue;
                        RenderControlCells(control);
                    }
                }

                MonoGame.Global.SharedSpriteBatch.End();
                MonoGame.Global.GraphicsDevice.SetRenderTarget(null);
            }

            screen.IsDirty = false;
            uiComponent.IsDirty = false;
        }


        protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control)
        {
            Font font = control.AlternateFont ?? control.Parent.ParentConsole.Font;

            var fontImage = ((SadConsole.Host.GameTexture)font.Image).Texture;

            if (control.Surface.DefaultBackground.A != 0)
            {
                (int x, int y) = (control.Position - control.Parent.ParentConsole.Surface.ViewPosition).SurfaceLocationToPixel(control.Parent.ParentConsole.FontSize);
                (int width, int height) = new SadRogue.Primitives.Point(control.Surface.View.Width, control.Surface.View.Height) * control.Parent.ParentConsole.FontSize;

                MonoGame.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, width, height), font.SolidGlyphRectangle.ToMonoRectangle(), control.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            }

            var parentViewRect = control.Parent.ParentConsole.Surface.View;

            for (int i = 0; i < control.Surface.Cells.Length; i++)
            {
                ColoredGlyph cell = control.Surface.Cells[i];

                cell.IsDirty = false;

                if (!cell.IsVisible) continue;

                SadRogue.Primitives.Point cellRenderPosition = SadRogue.Primitives.Point.FromIndex(i, control.Surface.View.Width) + control.Position;

                if (!parentViewRect.Contains(cellRenderPosition)) continue;

                XnaRectangle renderRect = _renderRects[(cellRenderPosition - control.Parent.ParentConsole.Surface.ViewPosition).ToIndex(control.Parent.ParentConsole.Surface.BufferWidth)];

                if (cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != control.Surface.DefaultBackground)
                {
                    MonoGame.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(font.SolidGlyphIndex).ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
                }

                if (cell.Foreground != SadRogue.Primitives.Color.Transparent)
                {
                    MonoGame.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);
                }

                foreach (CellDecorator decorator in cell.Decorators)
                {
                    if (decorator.Color != SadRogue.Primitives.Color.Transparent)
                    {
                        MonoGame.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(decorator.Glyph).ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    _renderRects = null;

                BackingTexture?.Dispose();
                BackingTexture = null;
                BackingTextureControls?.Dispose();
                BackingTextureControls = null;

                disposedValue = true;
            }
        }
    }
}
