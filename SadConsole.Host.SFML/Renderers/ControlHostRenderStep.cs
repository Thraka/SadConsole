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
    public class ControlHostRenderStep : IRenderStep, IRenderStepTexture
    {
        private SadConsole.UI.ControlHost _controlsHost;
        private ScreenSurfaceRenderer _baseRenderer;
        private IScreenSurface _screen;

        /// <summary>
        /// The cached texture of the drawn controls layer.
        /// </summary>
        public RenderTexture BackingTexture { get; private set; }

        /// <inheritdoc/>
        public int SortOrder { get; set; } = 5;

        ///  <inheritdoc/>
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(renderer is ScreenSurfaceRenderer))
                throw new Exception($"Renderer used with {nameof(ControlHostRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");

            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            OnSurfaceChanged(renderer, surface);
        }

        ///  <inheritdoc/>
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            _screen = null;
            _baseRenderer = null;
            _controlsHost = null;
        }

        ///  <inheritdoc/>
        public void OnSurfaceChanged(IRenderer renderer, IScreenSurface surface)
        {
            if (surface == null)
            {
                BackingTexture?.Dispose();
                BackingTexture = null;
                _screen = null;
                _controlsHost = null;
            }
            else
            {
                if (!surface.HasSadComponent(out UI.ControlHost host))
                    throw new Exception($"{nameof(ControlHostRenderStep)} is being run on object without a {nameof(UI.ControlHost)} component.");

                _screen = surface;
                _controlsHost = host;
                // BackingTexture is handled by prestart.
            }
        }

        ///  <inheritdoc/>
        public void RenderStart()
        {
            // Draw call for controls
            if (_screen.Tint.A != 255)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(_screen.AbsoluteArea.Position.X, _screen.AbsoluteArea.Position.Y), _baseRenderer._finalDrawColor));
        }

        ///  <inheritdoc/>
        public void RenderEnd()
        {
        }

        ///  <inheritdoc/>
        public bool RefreshPreStart()
        {
            // Update texture if something is out of size.
            if (BackingTexture == null || _screen.AbsoluteArea.Width != (int)BackingTexture.Size.X || _screen.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTexture((uint)_screen.AbsoluteArea.Width, (uint)_screen.AbsoluteArea.Height);
                return true;
            }

            return false;
        }

        ///  <inheritdoc/>
        public void Refresh()
        {
            if (_baseRenderer.IsForced || _controlsHost.IsDirty)
            {
                BackingTexture.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(BackingTexture, _baseRenderer.SFMLBlendState, Transform.Identity);

                ProcessContainer(_controlsHost);

                Host.Global.SharedSpriteBatch.End();
                BackingTexture.Display();
            }

            _controlsHost.IsDirty = false;
        }

        /// <summary>
        /// Processes a container from the control host.
        /// </summary>
        /// <param name="controlContainer">The container.</param>
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

        /// <summary>
        /// Renders the cells of a control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="font">The font to render the cells with.</param>
        /// <param name="fontSize">The size of a cell in pixels.</param>
        /// <param name="parentViewRect">The view of the parent to cull cells from.</param>
        /// <param name="bufferWidth">The width of the parent used to calculate the render rect.</param>
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

                Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect, true, font);
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
        protected void Dispose(bool disposing)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
        }

        ///  <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes the object for collection.
        /// </summary>
        ~ControlHostRenderStep() =>
            Dispose(false);
    }
}
