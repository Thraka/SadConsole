using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    class OutputSurfaceRenderStep : IRenderStep
    {
        ///  <inheritdoc/>
        public int SortOrder { get; set; } = 50;

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

        public void Render(IRenderer renderer, IScreenSurface screenObject)
        {
            var monoRenderer = (ScreenSurfaceRenderer)renderer;

            if (screenObject.Tint.A != 255)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(monoRenderer._backingTexture, new Vector2(screenObject.AbsoluteArea.Position.X, screenObject.AbsoluteArea.Position.Y), monoRenderer._finalDrawColor));
        }

        ///  <inheritdoc/>
        public void Dispose() =>
            Reset();
    }
}
