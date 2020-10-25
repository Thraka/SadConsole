using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="SadConsole.UI.ControlHost"/>.
    /// </summary>
    public class ControlHostRenderStep : IRenderStep
    {
        private SadConsole.UI.ControlHost _controlsHost;
        private ScreenSurfaceRenderer _baseRenderer;
        private IScreenSurface _screen;

        /// <summary>
        /// The cached texture of the drawn controls layer.
        /// </summary>
        public RenderTexture BackingTextureControls;

        /// <inheritdoc/>
        public int SortOrder { get; set; } = 5;

        ///  <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ///  <inheritdoc/>
        public void OnAdded(IRenderer renderer, IScreenSurface screen)
        {
            if (!(renderer is ScreenSurfaceRenderer)) throw new Exception($"Renderer used with {nameof(ControlHostRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");
            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            _screen = screen;
        }

        ///  <inheritdoc/>
        public void OnRemoved(IRenderer renderer, IScreenSurface screen)
        {
            BackingTextureControls?.Dispose();
            BackingTextureControls = null;
            _screen = null;
            _baseRenderer = null;
        }

        ///  <inheritdoc/>
        public bool RefreshPreStart()
        {
            if (!_screen.HasSadComponent<UI.ControlHost>(out UI.ControlHost host))
                throw new Exception("ControlHostRenderStep is being run on object without a control host component.");

            _controlsHost = host;

            // Update texture if something is out of size.
            if (BackingTextureControls == null || _screen.AbsoluteArea.Width != (int)BackingTextureControls.Size.X || _screen.AbsoluteArea.Height != (int)BackingTextureControls.Size.Y)
            {
                BackingTextureControls?.Dispose();
                BackingTextureControls = new RenderTexture((uint)_screen.AbsoluteArea.Width, (uint)_screen.AbsoluteArea.Height);
                return true;
            }

            return _controlsHost.IsDirty;
        }

        ///  <inheritdoc/>
        public void RefreshEnding() { }

        ///  <inheritdoc/>
        public void RefreshEnd()
        {
            if (_baseRenderer.IsForced || _controlsHost.IsDirty)
            {
                BackingTextureControls.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(BackingTextureControls, _baseRenderer.SFMLBlendState, Transform.Identity);

                if (_screen.Tint.A != 255)
                    ProcessContainer(_controlsHost);

                Host.Global.SharedSpriteBatch.End();
                BackingTextureControls.Display();
            }

            _screen.IsDirty = false;
            _controlsHost.IsDirty = false;
        }

        ///  <inheritdoc/>
        public void RenderStart() { }

        ///  <inheritdoc/>
        public void RenderBeforeTint()
        {
            if (_screen.Tint.A != 255)
                // Draw call for control surface
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTextureControls.Texture, new SFML.System.Vector2i(_screen.AbsoluteArea.Position.X, _screen.AbsoluteArea.Position.Y), _baseRenderer._finalDrawColor));
        }

        ///  <inheritdoc/>
        public void RenderEnd() { }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                BackingTextureControls?.Dispose();
                BackingTextureControls = null;
            }
        }
        protected void ProcessContainer(UI.Controls.IContainer controlContainer)
        {
            foreach (UI.Controls.ControlBase control in controlContainer)
            {
                if (!control.IsVisible) continue;
                RenderControlCells(control, _screen.Font, _screen.FontSize, _screen.Surface.View, _screen.Surface.BufferWidth);

                if (control is UI.Controls.IContainer container)
                    ProcessContainer(container);
            }
        }

        protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control, Font font, Point fontSize, Rectangle parentViewRect, int bufferWidth)
        {
            font = control.AlternateFont ?? font;

            //if (control.Surface.DefaultBackground.A != 0)
            //{
            //    (int x, int y) = (control.AbsolutePosition - parentViewRect.Position).SurfaceLocationToPixel(fontSize);
            //    (int width, int height) = new Point(control.Surface.View.Width, control.Surface.View.Height) * fontSize;

            //    Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(x, y, x + width, y + height), font.SolidGlyphRectangle.ToIntRect(), control.Surface.DefaultBackground.ToSFMLColor(), ((SadConsole.Host.GameTexture)font.Image).Texture);
            //}

            for (int i = 0; i < control.Surface.Cells.Length; i++)
            {
                ColoredGlyph cell = control.Surface.Cells[i];

                cell.IsDirty = false;

                if (!cell.IsVisible) continue;

                Point cellRenderPosition = Point.FromIndex(i, control.Surface.View.Width) + control.AbsolutePosition;

                if (!parentViewRect.Contains(cellRenderPosition)) continue;

                IntRect renderRect = _baseRenderer.CachedRenderRects[(cellRenderPosition - parentViewRect.Position).ToIndex(bufferWidth)];

                Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect, cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != control.Surface.DefaultBackground, font);
            }
        }
    }
}
