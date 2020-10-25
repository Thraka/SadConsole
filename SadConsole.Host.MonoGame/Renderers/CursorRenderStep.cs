using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadConsole.Host.MonoGame;

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
                                                        ((SadConsole.Host.GameTexture)_screen.Font.Image).Texture,
                                                        new XnaRectangle(_screen.AbsoluteArea.Position.ToMonoPoint() + _screen.Font.GetRenderRect(cursor.Position.X - _screen.Surface.ViewPosition.X, cursor.Position.Y - _screen.Surface.ViewPosition.Y, _screen.FontSize).ToMonoRectangle().Location, _screen.FontSize.ToMonoPoint()),
                                                        _screen.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                                        _screen.Font.GetGlyphSourceRectangle(cursor.CursorRenderCell.Glyph).ToMonoRectangle()
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
