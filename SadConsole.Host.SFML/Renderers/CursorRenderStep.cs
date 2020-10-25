using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    public class CursorRenderStep : IRenderStep
    {
        private ScreenSurfaceRenderer _baseRenderer;
        private IScreenSurface _screen;

        public int SortOrder { get; set; } = 8;

        public void Dispose() { }
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(renderer is ScreenSurfaceRenderer)) throw new Exception($"Renderer used with {nameof(CursorRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");
            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            _screen = surface;
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
        public void RenderBeforeTint()
        {
            // If the tint isn't covering everything
            if (_screen.Tint.A != 255)
            {
                // Draw any cursors
                foreach (var cursor in _screen.GetSadComponents<Components.Cursor>())
                {
                    if (cursor.IsVisible && _screen.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) && _screen.Surface.View.Contains(cursor.Position))
                    {
                        var cursorPosition = _screen.AbsoluteArea.Position + _screen.Font.GetRenderRect(cursor.Position.X - _screen.Surface.ViewPosition.X, cursor.Position.Y - _screen.Surface.ViewPosition.Y, _screen.FontSize).Position;

                        GameHost.Instance.DrawCalls.Enqueue(
                            new DrawCalls.DrawCallCell(cursor.CursorRenderCell,
                                                        new SadRogue.Primitives.Rectangle(cursorPosition.X, cursorPosition.Y, _screen.FontSize.X, _screen.FontSize.Y).ToIntRect(),
                                                        _screen.Font,
                                                        true
                                                        )
                            );
                    }
                }
            }
        }
        public void RenderEnd() { }
        public void RenderStart() { }
    }
}
