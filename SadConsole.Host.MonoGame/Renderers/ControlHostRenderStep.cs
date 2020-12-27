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
    /// Draws a <see cref="SadConsole.UI.ControlHost"/>.
    /// </summary>
    public class ControlHostRenderStep : IRenderStep, IRenderStepTexture
    {
        private SadConsole.UI.ControlHost _controlsHost;
        private IScreenSurface _screen;
        private Host.GameTexture _cachedTexture;
        private ScreenSurfaceRenderer _baseRenderer;

        /// <summary>
        /// The cached texture of the drawn surface.
        /// </summary>
        public RenderTarget2D BackingTexture { get; private set; }

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
        public bool Refresh(IRenderer renderer, bool backingTextureChanged, bool isForced)
        {
            bool result = true;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || _screen.AbsoluteArea.Width != BackingTexture.Width || _screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, _screen.AbsoluteArea.Width, _screen.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                _cachedTexture?.Dispose();
                _cachedTexture = new Host.GameTexture(BackingTexture);
                result = true;
            }

            if (result || _controlsHost.IsDirty || isForced)
            {
                Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
                Host.Global.GraphicsDevice.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, _baseRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                ProcessContainer(_controlsHost);

                Host.Global.SharedSpriteBatch.End();
                Host.Global.GraphicsDevice.SetRenderTarget(null);

                result = true;
                _controlsHost.IsDirty = false;
            }

            return result;
        }

        ///  <inheritdoc/>
        public void Composing()
        {
            if (_screen.Tint.A != 255)
                Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, Color.White);
        }

        ///  <inheritdoc/>
        public void Render()
        {
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

        /// <summary>
        /// Renders the cells of a control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="font">The font to render the cells with.</param>
        /// <param name="fontSize">The size of a cell in pixels.</param>
        /// <param name="parentViewRect">The view of the parent to cull cells from.</param>
        /// <param name="bufferWidth">The width of the parent used to calculate the render rect.</param>
        protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control, Font font, SadRogue.Primitives.Point fontSize, SadRectangle parentViewRect, int bufferWidth)
        {
            font = control.AlternateFont ?? font;

            var fontImage = ((SadConsole.Host.GameTexture)font.Image).Texture;

            ColoredGlyph cell;

            for (int i = 0; i < control.Surface.Count; i++)
            {
                cell = control.Surface[i];
                cell.IsDirty = false;
                if (!cell.IsVisible) continue;

                SadRogue.Primitives.Point cellRenderPosition = SadRogue.Primitives.Point.FromIndex(i, control.Surface.View.Width) + control.AbsolutePosition;

                if (!parentViewRect.Contains(cellRenderPosition)) continue;

                XnaRectangle renderRect = _baseRenderer.CachedRenderRects[(cellRenderPosition - parentViewRect.Position).ToIndex(bufferWidth)];

                if (cell.Background != SadRogue.Primitives.Color.Transparent)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(font.SolidGlyphIndex).ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                if (cell.Foreground != SadRogue.Primitives.Color.Transparent)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                foreach (CellDecorator decorator in cell.Decorators)
                    if (decorator.Color != SadRogue.Primitives.Color.Transparent)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(decorator.Glyph).ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
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
