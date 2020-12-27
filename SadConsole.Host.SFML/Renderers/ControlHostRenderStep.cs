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
        private IScreenSurface _screen;
        private Host.GameTexture _cachedTexture;
        private ScreenSurfaceRenderer _baseRenderer;

        /// <summary>
        /// The cached texture of the drawn controls layer.
        /// </summary>
        public RenderTexture BackingTexture { get; private set; }

        /// <inheritdoc/>
        public ITexture CachedTexture => _cachedTexture;

        /// <inheritdoc/>
        public int SortOrder { get; set; } = 80;

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
            _cachedTexture?.Dispose();
            _cachedTexture = null;
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
                _cachedTexture?.Dispose();
                _cachedTexture = null;
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
        public void RenderEnd()
        {
        }

        ///  <inheritdoc/>
        public bool Refresh(IRenderer renderer, bool backingTextureChanged, bool isForced)
        {
            bool result = true;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || _screen.AbsoluteArea.Width != (int)BackingTexture.Size.X || _screen.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTexture((uint)_screen.AbsoluteArea.Width, (uint)_screen.AbsoluteArea.Height);
                _cachedTexture?.Dispose();
                _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
                result = true;
            }

            // Redraw is needed
            if (result || _controlsHost.IsDirty || isForced)
            {
                BackingTexture.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(BackingTexture, _baseRenderer.SFMLBlendState, Transform.Identity);

                ProcessContainer(_controlsHost);

                Host.Global.SharedSpriteBatch.End();
                BackingTexture.Display();

                result = true;
                _controlsHost.IsDirty = false;
            }

            return result;
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
                RenderControlCells(control, _screen.Font, _screen.FontSize, _screen.Surface.View, _screen.Surface.Width);

                if (control is UI.Controls.IContainer container)
                    ProcessContainer(container);
            }
        }

        ///  <inheritdoc/>
        public void Composing()
        {
            if (_screen.Tint.A != 255)
            {
                IntRect outputArea = new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y);
                Host.Global.SharedSpriteBatch.DrawQuad(outputArea, outputArea, Color.White, BackingTexture.Texture);
            }
        }

        ///  <inheritdoc/>
        public void Render() { }

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

            for (int i = 0; i < control.Surface.Count; i++)
            {
                ColoredGlyph cell = control.Surface[i];

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
            _cachedTexture?.Dispose();
            _cachedTexture = null;
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
