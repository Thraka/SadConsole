using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws the entities of a <see cref="Entities.EntityLiteManager"/>.
    /// </summary>
    public class EntityLiteRenderStep : IRenderStep, IRenderStepTexture
    {
        private Entities.EntityLiteManager _entityManager;
        private ScreenSurfaceRenderer _baseRenderer;
        private IScreenSurface _screen;

        /// <summary>
        /// The cached texture of the drawn entities.
        /// </summary>
        public RenderTexture BackingTexture { get; private set; }

        /// <inheritdoc/>
        public int SortOrder { get; set; } = 5;

        ///  <inheritdoc/>
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(renderer is ScreenSurfaceRenderer)) throw new Exception($"Renderer used with {nameof(EntityLiteRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");
            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            _screen = surface;

            OnSurfaceChanged(renderer, surface);
        }

        ///  <inheritdoc/>
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            _screen = null;
            _baseRenderer = null;
            _entityManager = null;
        }

        ///  <inheritdoc/>
        public void OnSurfaceChanged(IRenderer renderer, IScreenSurface surface)
        {
            if (surface == null)
            {
                BackingTexture?.Dispose();
                BackingTexture = null;
                _screen = null;
                _entityManager = null;
            }
            else
            {
                if (!_screen.HasSadComponent(out Entities.EntityLiteManager host))
                    throw new Exception("EntityLiteManager is being run on object without a control host component.");
                _screen = surface;
                _entityManager = host;
                // BackingTexture is handled by prestart.
            }
        }

        ///  <inheritdoc/>
        public void RenderStart()
        {
            if (_screen.Tint.A != 255)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(_screen.AbsoluteArea.Position.X, _screen.AbsoluteArea.Position.Y), _baseRenderer._finalDrawColor));
        }

        ///  <inheritdoc/>
        public void RenderEnd() { }

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
            if (_baseRenderer.IsForced || _entityManager.IsDirty)
            {
                BackingTexture.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(BackingTexture, _baseRenderer.SFMLBlendState, Transform.Identity);

                ColoredGlyph cell;
                Rectangle offsetArea = _screen.AbsoluteArea.WithPosition(_screen.Surface.ViewPosition * _screen.FontSize);
                Point cellRenderPosition;
                IntRect renderRect;

                foreach (Entities.EntityLite item in _entityManager.Entities)
                {
                    if (!item.IsVisible) continue;


                    if (item.UsePixelPositioning)
                    {
                        if (!offsetArea.Expand(item.FontSize.X, item.FontSize.Y).Contains(item.Position)) continue;

                        cellRenderPosition = item.Position - offsetArea.Position;
                        renderRect = new Rectangle(cellRenderPosition.X, cellRenderPosition.Y, item.FontSize.X, item.FontSize.Y).ToIntRect();
                    }
                    else
                    {
                        if (!_screen.Surface.View.Contains(item.Position)) continue;

                        renderRect = _baseRenderer.CachedRenderRects[(item.Position - _screen.Surface.View.Position).ToIndex(_screen.Surface.ViewWidth)];
                    }

                    cell = item.Appearance;

                    cell.IsDirty = false;

                    Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect, true, item.Font);
                }

                Host.Global.SharedSpriteBatch.End();
                BackingTexture.Display();
            }

            _entityManager.IsDirty = false;
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
        ~EntityLiteRenderStep() =>
            Dispose(false);
    }
}
