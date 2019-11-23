using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadRectangle = SadRogue.Primitives.Rectangle;
using SadConsole.Host.MonoGame;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="ScreenObjectSurface"/> when it uses a <see cref="LayeredCellSurface"/> type for the <see cref="ScreenObjectSurface.Surface"/>.
    /// </summary>
    /// <remarks>
    /// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
    /// </remarks>
    public class LayeredScreenObject : ScreenObjectRenderer
    {
        ///  <inheritdoc/>
        public override void Attach(ScreenObjectSurface screen)
        {
            if (!(screen.Surface is LayeredCellSurface))
                throw new Exception($"The {nameof(LayeredConsole)} renderer must be added to a screen object that has a {nameof(LayeredCellSurface)} for a surface.");
        }

        ///  <inheritdoc/>
        public override void Refresh(ScreenObjectSurface screen, bool force = false)
        {
            if (!force && !screen.IsDirty && BackingTexture != null) return;

            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != BackingTexture.Width || screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(MonoGame.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, MonoGame.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != screen.Surface.ViewWidth * screen.Surface.ViewHeight || _renderRects[0].Width != screen.FontSize.X || _renderRects[0].Height != screen.FontSize.Y)
            {
                _renderRects = new XnaRectangle[screen.Surface.ViewWidth * screen.Surface.ViewHeight];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, screen.Surface.ViewWidth);
                    _renderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToMonoRectangle();
                }
            }
           
            // Rendering code from sadconsole
            RefreshBegin(screen);

            if (screen.Tint.A != 255)
            {
                foreach (var layer in ((LayeredCellSurface)screen.Surface).Layers)
                    RefreshCells(layer.Surface, screen.Font);
            }

            RefreshTint(screen);

            RefreshEnd(screen);

            screen.IsDirty = false;
        }
    }
}
