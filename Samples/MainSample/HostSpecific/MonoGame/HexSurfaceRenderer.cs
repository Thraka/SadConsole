#if MONOGAME

using SadRogue.Primitives;
using Microsoft.Xna.Framework.Graphics;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadConsole;

namespace FeatureDemo.HostSpecific.MonoGame
{
    class HexSurfaceRenderer: SadConsole.Renderers.ScreenObjectRenderer
    {
        public override void Refresh(ISurfaceRenderData screen, bool force = false)
        {
            if (!force && !screen.IsDirty && BackingTexture != null) return;

            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != BackingTexture.Width || screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(SadConsole.MonoGame.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, SadConsole.MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || _renderRects[0].Width != screen.FontSize.X || _renderRects[0].Height != screen.FontSize.Y)
            {
                _renderRects = new XnaRectangle[screen.Surface.View.Width * screen.Surface.View.Height];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, screen.Surface.View.Width);
                    _renderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToMonoRectangle();

                    if (position.Y % 2 == 1)
                        _renderRects[i].Offset(screen.FontSize.X / 2, 0);
                }
            }

            // Render parts of the surface
            RefreshBegin(screen);

            if (screen.Tint.A != 255)
                RefreshCells(screen.Surface, screen.Font);

            RefreshEnd(screen);

            screen.IsDirty = false;
        }
    }
}

#endif
