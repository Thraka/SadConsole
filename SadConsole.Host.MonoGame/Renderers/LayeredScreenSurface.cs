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
    /// Draws a <see cref="SadConsole.LayeredScreenSurface"/>.
    /// </summary>
    /// <remarks>
    /// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
    /// </remarks>
    public class LayeredScreenSurface : ScreenSurfaceRenderer
    {
        /// <summary>
        /// Name of this renderer type.
        /// </summary>
        public static new string Name => "layered";

        ///  <inheritdoc/>
        public override void Attach(IScreenSurface screen)
        {
            if (!(screen is SadConsole.LayeredScreenSurface layeredObject))
                throw new Exception($"The {nameof(LayeredScreenSurface)} Renderer must be added to a {nameof(SadConsole.LayeredScreenSurface)}.");

            // Create backing texture.
            if (layeredObject.RenderClipped)
                BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, layeredObject.RenderClippedWidth, layeredObject.RenderClippedHeight, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        }

        ///  <inheritdoc/>
        public override void Render(IScreenSurface screen)
        {
            var layeredObject = (SadConsole.LayeredScreenSurface)screen;

            if (layeredObject.RenderClipped)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screen.AbsolutePosition.X, screen.AbsolutePosition.Y)));

            else
                foreach (SadConsole.LayeredScreenSurface.Layer item in layeredObject.Layers)
                    item.Renderer?.Refresh(item, item.ForceRendererRefresh);
        }

        ///  <inheritdoc/>
        public override void Refresh(IScreenSurface screen, bool force = false)
        {
            var layeredObject = (SadConsole.LayeredScreenSurface)screen;

            // Simple mode
            if (!layeredObject.RenderClipped)
            {
                RefreshBegin(screen);

                foreach (SadConsole.LayeredScreenSurface.Layer item in layeredObject.Layers)
                {
                    if (item.Renderer != null)
                    {
                        // Layers are parented, when rendering clipped, we need an relative position in pixels.
                        SadRogue.Primitives.Point transformedPosition = item.AbsolutePosition - screen.AbsolutePosition;
                        Host.Global.SharedSpriteBatch.Draw(((ScreenSurfaceRenderer)item.Renderer).BackingTexture, new Vector2(transformedPosition.X, transformedPosition.Y), Color.White);
                    }
                } 

                RefreshEnd(screen);
            }
        }
    }
}
