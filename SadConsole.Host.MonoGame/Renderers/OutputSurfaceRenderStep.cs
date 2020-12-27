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
        private ScreenSurfaceRenderer _renderer;
        private IScreenSurface _screen;

        public int SortOrder => 50;

        public bool CheckRefresh(IRenderer renderer, bool backingTextureChanged) => false;
        public void Dispose() { }
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            _renderer = (ScreenSurfaceRenderer)renderer;
        }
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
        {
            _renderer = null;
        }
        public void OnSurfaceChanged(IRenderer renderer, IScreenSurface surface)
        {
            _screen = surface;
        }
        public bool Refresh(IRenderer renderer, bool backingTextureChanged, bool isForced) =>
            false;

        public void Composing() { }

        public void Render()
        {
            if (_renderer.DoOutputRender)
            {
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(_renderer._backingTexture, new Vector2(_screen.AbsoluteArea.Position.X, _screen.AbsoluteArea.Position.Y), _renderer._finalDrawColor));

                if (_screen.Tint.A != 0)
                    GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(_screen.Tint.ToMonoColor(), ((SadConsole.Host.GameTexture)_screen.Font.Image).Texture, _screen.AbsoluteArea.ToMonoRectangle(), _screen.Font.SolidGlyphRectangle.ToMonoRectangle()));
            }
        }
    }
}
