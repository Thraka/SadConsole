using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

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
        public RenderTexture BackingTextureControls;

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
            if (screen.Tint.A != 255)
            {
                // Draw call for surface
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

                // Draw call for control surface
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTextureControls.Texture, new SFML.System.Vector2i(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y)));

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
        public override void Refresh(IScreenSurface screen, bool force = false)
        {
            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != (int)BackingTexture.Size.X || screen.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTexture((uint)screen.AbsoluteArea.Width, (uint)screen.AbsoluteArea.Height);
                BackingTextureControls?.Dispose();
                BackingTextureControls = new RenderTexture((uint)screen.AbsoluteArea.Width, (uint)screen.AbsoluteArea.Height);
                force = true;
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


            // Render the main console
            if (force || screen.IsDirty)
            {
                RefreshBegin(screen);
                if (screen.Tint.A != 255)
                    RefreshCells(screen.Surface, screen.Font);

                RefreshEnd(screen);
            }

            UI.ControlHost uiComponent = screen.GetSadComponent<UI.ControlHost>();

            if (force || uiComponent.IsDirty)
            {
                BackingTextureControls.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(BackingTextureControls, RenderStates.Default, Transform.Identity);

                if (screen.Tint.A != 255)
                    foreach (UI.Controls.ControlBase control in ((IScreenSurface)screen).GetSadComponent<UI.ControlHost>().Controls)
                    {
                        if (!control.IsVisible) continue;

                        RenderControlCells(control);
                    }

                Host.Global.SharedSpriteBatch.End();
                BackingTextureControls.Display();
            }

            screen.IsDirty = false;
            uiComponent.IsDirty = false;
        }

        protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control)
        {
            Font font = control.AlternateFont ?? control.Parent.ParentConsole.Font;

            if (control.Surface.DefaultBackground.A != 0)
            {
                (int x, int y) = (control.Position - control.Parent.ParentConsole.Surface.ViewPosition).SurfaceLocationToPixel(control.Parent.ParentConsole.FontSize);
                (int width, int height) = new Point(control.Surface.View.Width, control.Surface.View.Height) * control.Parent.ParentConsole.FontSize;

                Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(x, y, x + width, y + height), font.GlyphRects[font.SolidGlyphIndex].ToIntRect(), control.Surface.DefaultBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)font.Image).Texture);
            }

            var parentViewRect = control.Parent.ParentConsole.Surface.View;

            for (int i = 0; i < control.Surface.Cells.Length; i++)
            {
                ColoredGlyph cell = control.Surface.Cells[i];

                cell.IsDirty = false;

                if (!cell.IsVisible) continue;

                Point cellRenderPosition = Point.FromIndex(i, control.Surface.View.Width) + control.Position;

                if (!parentViewRect.Contains(cellRenderPosition)) continue;

                IntRect renderRect = _renderRects[(cellRenderPosition - control.Parent.ParentConsole.Surface.ViewPosition).ToIndex(control.Parent.ParentConsole.Surface.BufferWidth)];

                Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect, !cell.Background.Equals(Color.Transparent) && cell.Background != control.Surface.DefaultBackground, font);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _renderRects = null;
                }

                BackingTexture?.Dispose();
                BackingTexture = null;
                BackingTextureControls?.Dispose();
                BackingTextureControls = null;

                disposedValue = true;
            }
        }
    }
}
