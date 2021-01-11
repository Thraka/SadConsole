using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws the entities of a <see cref="Entities.Renderer"/>.
    /// </summary>
    public class EntityLiteRenderStep : IRenderStep, IRenderStepTexture
    {
        private Entities.Renderer _entityManager;
        private Host.GameTexture _cachedTexture;

        /// <summary>
        /// The cached texture of the drawn entities.
        /// </summary>
        public RenderTexture BackingTexture { get; private set; }

        /// <inheritdoc/>
        public ITexture CachedTexture => _cachedTexture;

        /// <inheritdoc/>
        public uint SortOrder { get; set; } = Constants.RenderStepSortValues.EntityRenderer;

        /// <summary>
        /// Sets the <see cref="Entities.Renderer"/>.
        /// </summary>
        /// <param name="data">A <see cref="Entities.Renderer"/> object.</param>
        public void SetData(object data)
        {
            if (data is Entities.Renderer manager)
                _entityManager = manager;
            else
                throw new ArgumentException($"{nameof(EntityLiteRenderStep)} must have a {nameof(Entities.Renderer)} passed to the {nameof(SetData)} method", nameof(data));
        }

        ///  <inheritdoc/>
        public void Reset()
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            _cachedTexture?.Dispose();
            _cachedTexture = null;
            _entityManager = null;
        }

        ///  <inheritdoc/>
        public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
        {
            bool result = true;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != (int)BackingTexture.Size.X || screenObject.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTexture((uint)screenObject.AbsoluteArea.Width, (uint)screenObject.AbsoluteArea.Height);
                _cachedTexture?.Dispose();
                _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
                result = true;
            }

            // Redraw is needed
            if (result || _entityManager.IsDirty || isForced)
            {
                BackingTexture.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(BackingTexture, ((ScreenSurfaceRenderer)renderer).SFMLBlendState, Transform.Identity);

                ColoredGlyph cell;
                IntRect renderRect;

                foreach (Entities.Entity item in _entityManager.EntitiesVisible)
                {
                    if (!item.IsVisible) continue;

                    renderRect = _entityManager.GetRenderRectangle(item.Position, item.UsePixelPositioning).ToIntRect();

                    cell = item.Appearance;

                    cell.IsDirty = false;

                    Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect, true, screenObject.Font);
                }

                Host.Global.SharedSpriteBatch.End();
                BackingTexture.Display();

                result = true;
                _entityManager.IsDirty = false;
            }

            return result;
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
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
        protected void Dispose(bool disposing) =>
            Reset();

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
