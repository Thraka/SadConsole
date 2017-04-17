using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Surfaces;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Renders a <see cref="LayeredSurface"/>.
    /// </summary>
    [System.Runtime.Serialization.DataContract]
    public class LayeredSurfaceRenderer : SurfaceRenderer
    {
        LayeredSurface.Layer[] layers;

        /// <summary>
        /// Only renders a <see cref="LayeredSurface"/>.
        /// </summary>
        /// <param name="surface">The <see cref="LayeredSurface"/> to render.</param>
        /// <param name="renderingMatrix">Rendering matrix used with the sprite batch.</param>
        public override void Render(ISurface surface, bool force = false)
        {
            layers = ((LayeredSurface)surface).GetLayers();

            RenderBegin(surface, force);
            RenderCells(surface, force);
            RenderTint(surface, force);
            RenderEnd(surface, force);

            layers = null;
        }

        public override void RenderCells(ISurface surface, bool force = false)
        {
            if (surface.IsDirty || force)
            {
                if (surface.Tint.A != 255)
                {
                    Cell cell;

                    if (surface.DefaultBackground.A != 0)
                        Global.SpriteBatch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphRects[surface.Font.SolidGlyphIndex], surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                    for (int l = 0; l < layers.Length; l++)
                    {
                        if (layers[l].IsVisible)
                        {
                            for (int i = 0; i < layers[l].RenderCells.Length; i++)
                            {
                                cell = layers[l].RenderCells[i];

                                if (cell.IsVisible)
                                {
                                    Global.SpriteBatch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphRects[surface.Font.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                                    Global.SpriteBatch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.Mirror, 0.4f);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
