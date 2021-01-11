using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="UI.ControlHost"/>.
    /// </summary>
    public class ControlHostRenderStep : IRenderStep, IRenderStepTexture
    {
        private UI.ControlHost _controlsHost;
        private GameTexture _cachedTexture;

        /// <summary>
        /// The cached texture of the drawn controls layer.
        /// </summary>
        public RenderTexture BackingTexture { get; private set; }

        /// <inheritdoc/>
        public ITexture CachedTexture => _cachedTexture;

        /// <inheritdoc/>
        public uint SortOrder { get; set; } = Constants.RenderStepSortValues.ControlHost;

        /// <summary>
        /// Sets the <see cref="UI.ControlHost"/>.
        /// </summary>
        /// <param name="data">A <see cref="UI.ControlHost"/> object.</param>
        public void SetData(object data)
        {
            if (data is UI.ControlHost host)
                _controlsHost = host;
            else
                throw new Exception($"{nameof(ControlHostRenderStep)} must have a {nameof(UI.ControlHost)} passed to the {nameof(SetData)} method");
        }

        ///  <inheritdoc/>
        public void Reset()
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            _cachedTexture?.Dispose();
            _cachedTexture = null;
            _controlsHost = null;
        }

        ///  <inheritdoc/>
        public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
        {
            bool result = true;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != (int)BackingTexture.Size.X || screenObject.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                _cachedTexture?.Dispose();

                BackingTexture = new RenderTexture((uint)screenObject.AbsoluteArea.Width, (uint)screenObject.AbsoluteArea.Height);
                _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
                result = true;
            }

            // Redraw is needed
            if (result || _controlsHost.IsDirty || isForced)
            {
                BackingTexture.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(BackingTexture, ((ScreenSurfaceRenderer)renderer).SFMLBlendState, Transform.Identity);

                ProcessContainer(_controlsHost, ((ScreenSurfaceRenderer)renderer), screenObject);

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
        /// <param name="renderer">The renderer used with this step.</param>
        /// <param name="screenObject">The screen surface with font information.</param>
        protected void ProcessContainer(UI.Controls.IContainer controlContainer, ScreenSurfaceRenderer renderer, IScreenSurface screenObject)
        {
            foreach (UI.Controls.ControlBase control in controlContainer)
            {
                if (!control.IsVisible) continue;
                RenderControlCells(control, renderer, screenObject.Font, screenObject.FontSize, screenObject.Surface.View, screenObject.Surface.Width);

                if (control is UI.Controls.IContainer container)
                    ProcessContainer(container, renderer, screenObject);
            }
        }

        ///  <inheritdoc/>
        public void Composing(IRenderer renderer, IScreenSurface screenObject)
        {
            IntRect outputArea = new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y);
            Host.Global.SharedSpriteBatch.DrawQuad(outputArea, outputArea, Color.White, BackingTexture.Texture);
        }

        ///  <inheritdoc/>
        public void Render(IRenderer renderer, IScreenSurface screenObject)
        {
        }

        /// <summary>
        /// Renders the cells of a control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="renderer">The renderer used with this step.</param>
        /// <param name="font">The font to render the cells with.</param>
        /// <param name="fontSize">The size of a cell in pixels.</param>
        /// <param name="parentViewRect">The view of the parent to cull cells from.</param>
        /// <param name="bufferWidth">The width of the parent used to calculate the render rect.</param>
        protected void RenderControlCells(UI.Controls.ControlBase control, ScreenSurfaceRenderer renderer, Font font, Point fontSize, Rectangle parentViewRect, int bufferWidth)
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

                IntRect renderRect = renderer.CachedRenderRects[(cellRenderPosition - parentViewRect.Position).ToIndex(bufferWidth)];

                Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect, true, font);
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
        protected void Dispose(bool disposing)
        {
            Reset();
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
