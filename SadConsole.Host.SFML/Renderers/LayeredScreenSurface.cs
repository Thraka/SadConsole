using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

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
                BackingTexture = new RenderTexture((uint)layeredObject.RenderClippedWidth, (uint)layeredObject.RenderClippedHeight);
        }

        ///  <inheritdoc/>
        public override void Render(IScreenSurface screen)
        {
            var layeredObject = (SadConsole.LayeredScreenSurface)screen;

            if (layeredObject.RenderClipped)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture.Texture, new SFML.System.Vector2i(screen.AbsolutePosition.X, screen.AbsolutePosition.Y)));

            else
                foreach (SadConsole.LayeredScreenSurface.Layer item in layeredObject.Layers)
                    item.Renderer?.Render(item);
        }

        ///  <inheritdoc/>
        public override void Refresh(IScreenSurface screen, bool force = false)
        {
            var layeredObject = (SadConsole.LayeredScreenSurface)screen;

            // Clipped mode
            if (layeredObject.RenderClipped)
            {

                foreach (SadConsole.LayeredScreenSurface.Layer item in layeredObject.Layers)
                    item.Renderer?.Refresh(item, force | item.ForceRendererRefresh);

                RefreshBegin(screen);

                foreach (SadConsole.LayeredScreenSurface.Layer item in layeredObject.Layers)
                {
                    RenderTexture layerTexture;

                    if (item.Renderer != null && item.IsVisible)
                    {
                        layerTexture = ((ScreenSurfaceRenderer)item.Renderer).BackingTexture;

                        Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(item.AbsolutePosition.X, item.AbsolutePosition.Y,
                                                                           item.AbsolutePosition.X + (int)layerTexture.Size.X,
                                                                           item.AbsolutePosition.Y + (int)layerTexture.Size.Y),
                                                               new IntRect(0, 0, (int)layerTexture.Size.X, (int)layerTexture.Size.Y),
                                                               Color.White,
                                                               layerTexture.Texture);
                    }
                }

                RefreshEnd(screen);
            }
            else
            {
                foreach (SadConsole.LayeredScreenSurface.Layer item in layeredObject.Layers)
                {
                    item.Renderer?.Refresh(item, force | item.ForceRendererRefresh);
                    item.ForceRendererRefresh = false;
                }
            }
        }
    }
}
