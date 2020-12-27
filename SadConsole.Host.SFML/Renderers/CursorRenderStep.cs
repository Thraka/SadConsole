using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Renders a cursor.
    /// </summary>
    public class CursorRenderStep : IRenderStep
    {
        private ScreenSurfaceRenderer _baseRenderer;
        private IScreenSurface _screen;

        ///  <inheritdoc/>
        public int SortOrder { get; set; } = 8;

        ///  <inheritdoc/>
        public void OnAdded(IRenderer renderer, IScreenSurface surface)
        {
            if (!(renderer is ScreenSurfaceRenderer))
                throw new Exception($"Renderer used with {nameof(CursorRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");

            _baseRenderer = (ScreenSurfaceRenderer)renderer;
            _screen = surface;
        }

        ///  <inheritdoc/>
        public void OnRemoved(IRenderer renderer, IScreenSurface surface)
        {
            _screen = null;
            _baseRenderer = null;
        }

        ///  <inheritdoc/>
        public void OnSurfaceChanged(IRenderer renderer, IScreenSurface surface) =>
            _screen = surface;

        ///  <inheritdoc/>
        public bool Refresh(IRenderer renderer, bool backingTextureChanged, bool isForced) =>
            false;

        ///  <inheritdoc/>
        public void Composing() { }

        ///  <inheritdoc/>
        public void Render()
        {
            // If the tint isn't covering everything
            if (_screen.Tint.A != 255)
            {
                // Draw any cursors
                foreach (Components.Cursor cursor in _screen.GetSadComponents<Components.Cursor>())
                {
                    if (cursor.IsVisible && _screen.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) && _screen.Surface.View.Contains(cursor.Position))
                    {
                        Point cursorPosition = _screen.AbsoluteArea.Position + _screen.Font.GetRenderRect(cursor.Position.X - _screen.Surface.ViewPosition.X, cursor.Position.Y - _screen.Surface.ViewPosition.Y, _screen.FontSize).Position;

                        GameHost.Instance.DrawCalls.Enqueue(
                            new DrawCalls.DrawCallCell(cursor.CursorRenderCell,
                                                        new Rectangle(cursorPosition.X, cursorPosition.Y, _screen.FontSize.X, _screen.FontSize.Y).ToIntRect(),
                                                        _screen.Font,
                                                        true
                                                        )
                            );
                    }
                }
            }
        }

        ///  <inheritdoc/>
        public void Dispose() { }
    }
}
