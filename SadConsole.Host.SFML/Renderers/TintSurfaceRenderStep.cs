using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Renderers;
using SadRogue.Primitives;
using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Renders a tint on top of where the output texture is drawn.
    /// </summary>
    public class TintSurfaceRenderStep : IRenderStep
    {
        ///  <inheritdoc/>
        public int SortOrder { get; set; } = Constants.RenderStepSortValues.Tint;

        /// <summary>
        /// Not used.
        /// </summary>
        public void SetData(object data) { }

        ///  <inheritdoc/>
        public void Reset() { }

        ///  <inheritdoc/>
        public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) =>
            false;

        ///  <inheritdoc/>
        public void Composing(IRenderer renderer, IScreenSurface screenObject) { }

        ///  <inheritdoc/>
        public void Render(IRenderer renderer, IScreenSurface screenObject)
        {
            var sfmlRenderer = (ScreenSurfaceRenderer)renderer;

            if (screenObject.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screenObject.Tint.ToSFMLColor(), ((SadConsole.Host.GameTexture)screenObject.Font.Image).Texture, screenObject.AbsoluteArea.ToIntRect(), screenObject.Font.SolidGlyphRectangle.ToIntRect()));
        }

        ///  <inheritdoc/>
        public void Dispose() =>
            Reset();
    }
}
