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
            if (!(renderer is ScreenSurfaceRenderer)) throw new Exception($"Renderer used with {nameof(CursorRenderStep)} must be of type {nameof(ScreenSurfaceRenderer)}");
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
        public bool RefreshPreStart() =>
            false;

        ///  <inheritdoc/>
        public void Refresh() { }

        ///  <inheritdoc/>
        public void RenderStart()
        {
            // If the tint isn't covering everything
            if (_screen.Tint.A != 255)
            {
                // Draw any cursors
                foreach (Components.Cursor cursor in _screen.GetSadComponents<Components.Cursor>())
                {
                    if (cursor.IsVisible && _screen.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) && _screen.Surface.View.Contains(cursor.Position))
                    {
                        GameHost.Instance.DrawCalls.Enqueue(
                            new DrawCalls.DrawCallGlyph(cursor.CursorRenderCell,
                                                        ((SadConsole.Host.GameTexture)_screen.Font.Image).Texture,
                                                        new XnaRectangle(_screen.Font.GetRenderRect(cursor.Position.X - _screen.Surface.ViewPosition.X,
                                                                                                    cursor.Position.Y - _screen.Surface.ViewPosition.Y,
                                                                                                    _screen.FontSize).Translate(_screen.AbsolutePosition).Position.ToMonoPoint(),
                                                                            _screen.FontSize.ToMonoPoint()),
                                                        _screen.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                                        _screen.Font.GetGlyphSourceRectangle(cursor.CursorRenderCell.Glyph).ToMonoRectangle()
                                                        )
                            );
                    }
                }
            }
        }

        ///  <inheritdoc/>
        public void RenderEnd() { }

        ///  <inheritdoc/>
        public void Dispose() { }
    }
}
