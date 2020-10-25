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
    public class ControlHostRenderStep : IRenderStep
    {
        private SadConsole.UI.ControlHost _controlsHost;
        private ScreenSurfaceRenderer _baseRenderer;
        private IScreenSurface _screen;

        /// <summary>
        /// The cached texture of the drawn controls layer.
        /// </summary>
        public RenderTarget2D BackingTextureControls;

        /// <inheritdoc/>
        public int SortOrder { get; set; } = 5;

        ///  <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ///  <inheritdoc/>
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(renderer is ScreenSurfaceRenderer)) throw new Exception($"Renderer used with {nameof(ControlHostRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");
            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            _screen = surface;
        }

        ///  <inheritdoc/>
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
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
            if (BackingTextureControls == null || _screen.AbsoluteArea.Width != BackingTextureControls.Width || _screen.AbsoluteArea.Height != BackingTextureControls.Height)
            {
                BackingTextureControls?.Dispose();
                BackingTextureControls = new RenderTarget2D(Host.Global.GraphicsDevice, _screen.AbsoluteArea.Width, _screen.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
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
                Host.Global.GraphicsDevice.SetRenderTarget(BackingTextureControls);
                Host.Global.GraphicsDevice.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, _baseRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                if (_screen.Tint.A != 255)
                    ProcessContainer(_controlsHost);

                Host.Global.SharedSpriteBatch.End();
                Host.Global.GraphicsDevice.SetRenderTarget(null);
            }

            _screen.IsDirty = false;
            _controlsHost.IsDirty = false;
        }


        ///  <inheritdoc/>
        public void RenderStart() { }

        ///  <inheritdoc/>
        public void RenderBeforeTint() {

            // Draw call for controls
            if (_screen.Tint.A != 255)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTextureControls, new Vector2(_screen.AbsoluteArea.Position.X, _screen.AbsoluteArea.Position.Y), _baseRenderer._finalDrawColor));
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

        protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control, Font font, SadRogue.Primitives.Point fontSize, SadRectangle parentViewRect, int bufferWidth)
        {
            font = control.AlternateFont ?? font;

            var fontImage = ((SadConsole.Host.GameTexture)font.Image).Texture;

            if (control.Surface.DefaultBackground.A != 0)
            {
                (int x, int y) = (control.AbsolutePosition - parentViewRect.Position).SurfaceLocationToPixel(fontSize);
                (int width, int height) = new SadRogue.Primitives.Point(control.Surface.View.Width, control.Surface.View.Height) * fontSize;

                Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, width, height), font.SolidGlyphRectangle.ToMonoRectangle(), control.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            }

            for (int i = 0; i < control.Surface.Cells.Length; i++)
            {
                ColoredGlyph cell = control.Surface.Cells[i];

                cell.IsDirty = false;

                if (!cell.IsVisible) continue;

                SadRogue.Primitives.Point cellRenderPosition = SadRogue.Primitives.Point.FromIndex(i, control.Surface.View.Width) + control.AbsolutePosition;

                if (!parentViewRect.Contains(cellRenderPosition)) continue;

                XnaRectangle renderRect = _baseRenderer.CachedRenderRects[(cellRenderPosition - parentViewRect.Position).ToIndex(bufferWidth)];

                if (cell.Background != SadRogue.Primitives.Color.Transparent && cell.Background != control.Surface.DefaultBackground)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(font.SolidGlyphIndex).ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                if (cell.Foreground != SadRogue.Primitives.Color.Transparent)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                foreach (CellDecorator decorator in cell.Decorators)
                    if (decorator.Color != SadRogue.Primitives.Color.Transparent)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(decorator.Glyph).ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
            }
        }
    }
}
