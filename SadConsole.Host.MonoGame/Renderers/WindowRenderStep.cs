using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadConsole.Host.MonoGame;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Renders the dimmed background screen when a window is modal.
    /// </summary>
    public class WindowRenderStep : IRenderStep
    {
        private UI.Window _screen;

        ///  <inheritdoc/>
        public int SortOrder { get; set; } = 10;

        ///  <inheritdoc/>
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(surface is UI.Window))
                throw new Exception($"The window render step must be used with a {nameof(UI.Window)} type.");

            _screen = (UI.Window)surface;
        }

        ///  <inheritdoc/>
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
        {
            _screen = null;
        }

        ///  <inheritdoc/>
        public void OnSurfaceChanged(IRenderer renderer, IScreenSurface surface) =>
            _screen = (UI.Window)surface;

        /// <summary>
        /// Does nothing.
        /// </summary>
        public bool Refresh(IRenderer renderer, bool backingTextureChanged, bool isForced) =>
            false;

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Composing() { }

        ///  <inheritdoc/>
        public void Render()
        {
            UI.Colors colors = _screen.Controls.GetThemeColors();

            if (_screen.IsModal && colors.ModalBackground.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(colors.ModalBackground.ToMonoColor(), ((Host.GameTexture)_screen.Font.Image).Texture, new XnaRectangle(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), _screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Dispose() { }
    }
}
