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
        public int SortOrder { get; set; } = 1;

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

        ///  <inheritdoc/>
        public bool RefreshPreStart() =>
            false;

        ///  <inheritdoc/>
        public void Refresh() { }

        ///  <inheritdoc/>
        public void RenderStart()
        {
            UI.Colors colors = _screen.Controls.GetThemeColors();

            if (_screen.IsModal && colors.ModalBackground.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(colors.ModalBackground.ToMonoColor(), ((SadConsole.Host.GameTexture)_screen.Font.Image).Texture, new Microsoft.Xna.Framework.Rectangle(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), _screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }

        ///  <inheritdoc/>
        public void RenderEnd() { }

        ///  <inheritdoc/>
        public void Dispose() { }
    }
}
