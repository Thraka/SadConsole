using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadConsole.Host.MonoGame;

namespace SadConsole.Renderers
{
    public class WindowRenderStep : IRenderStep
    {
        private ScreenSurfaceRenderer _baseRenderer;
        private UI.Window _screen;

        public int SortOrder { get; set; } = 3;

        public void Dispose() { }
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(renderer is ScreenSurfaceRenderer))
                throw new Exception($"Renderer used with {nameof(WindowRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");
            if (!(surface is UI.Window))
                throw new Exception($"The window render step must be used with a {nameof(UI.Window)} type.");

            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            _screen = (UI.Window)surface;
        }
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
        {
            _screen = null;
            _baseRenderer = null;
        }
        public void RefreshEnd() { }
        public void RefreshEnding() { }
        public bool RefreshPreStart() =>
            false;
        public void RenderBeforeTint() { }
        public void RenderEnd() { }
        public void RenderStart()
        {
            var colors = _screen.Controls.GetThemeColors();

            if (_screen.IsModal && colors.ModalBackground.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(colors.ModalBackground.ToMonoColor(), ((SadConsole.Host.GameTexture)_screen.Font.Image).Texture, new Microsoft.Xna.Framework.Rectangle(0, 0, Settings.Rendering.RenderWidth, Settings.Rendering.RenderHeight), _screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
        }
    }
}
