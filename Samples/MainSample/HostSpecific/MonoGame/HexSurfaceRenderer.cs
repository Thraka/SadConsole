#if MONOGAME

using SadRogue.Primitives;
using Microsoft.Xna.Framework.Graphics;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadConsole;

namespace FeatureDemo.HostSpecific.MonoGame
{
    class HexSurfaceRenderer: SadConsole.Renderers.ScreenSurfaceRenderer
    {
        public override void Refresh(IScreenSurface screen, bool force = false)
        {
            //if (!force && !screen.IsDirty && BackingTexture != null) return;

            //// Update texture if something is out of size.
            //if (BackingTexture == null || screen.AbsoluteArea.Width != BackingTexture.Width || screen.AbsoluteArea.Height != BackingTexture.Height)
            //{
            //    BackingTexture?.Dispose();
            //    BackingTexture = new RenderTarget2D(SadConsole.Host.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, SadConsole.Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            //}

            //// Update cached drawing rectangles if something is out of size.
            //if (CachedRenderRects == null || CachedRenderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || CachedRenderRects[0].Width != screen.FontSize.X || CachedRenderRects[0].Height != screen.FontSize.Y)
            //{
            //    CachedRenderRects = new XnaRectangle[screen.Surface.View.Width * screen.Surface.View.Height];

            //    for (int i = 0; i < CachedRenderRects.Length; i++)
            //    {
            //        var position = Point.FromIndex(i, screen.Surface.View.Width);
            //        CachedRenderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToMonoRectangle();

            //        if (position.Y % 2 == 1)
            //            CachedRenderRects[i].Offset(screen.FontSize.X / 2, 0);
            //    }
            //}

            //// Render parts of the surface
            //RefreshBegin(screen);

            //if (screen.Tint.A != 255)
            //    RefreshCells(screen.Surface, screen.Font);

            //RefreshEnd(screen);

            //screen.IsDirty = false;
        }
    }
}

#endif
