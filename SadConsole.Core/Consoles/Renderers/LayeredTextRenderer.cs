#if SFML
using Point = SFML.System.Vector2i;
using Vector2 = SFML.System.Vector2f;
using SFML.System;
using Matrix = SFML.Graphics.Transform;
using SFML.Graphics;
#else
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole.Consoles
{
    /// <summary>
    /// Renders a <see cref="LayeredTextSurface"/>.
    /// </summary>
    public class LayeredTextRenderer : TextSurfaceRenderer
    {
        /// <summary>
        /// Only renders a <see cref="LayeredTextSurface"/>.
        /// </summary>
        /// <param name="surface">The <see cref="LayeredTextSurface"/> to render.</param>
        /// <param name="renderingMatrix">Rendering matrix used with the sprite batch.</param>
        public override void Render(ITextSurfaceRendered surface, Matrix renderingMatrix)
        {
            var layers = ((LayeredTextSurface)surface).GetLayers();

#if MONOGAME
            Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, renderingMatrix);

            BeforeRenderCallback?.Invoke(Batch);

            if (surface.Tint.A != 255)
            {
                Cell cell;

                if (surface.DefaultBackground.A != 0)
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                for (int l = 0; l < layers.Length; l++)
                {
                    if (layers[l].IsVisible)
                    {
                        for (int i = 0; i < layers[l].RenderCells.Length; i++)
                        {
                            cell = layers[l].RenderCells[i];

                            if (cell.IsVisible)
                            {
                                if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != surface.DefaultBackground)
                                    Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], cell.ActualBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                                if (cell.ActualForeground != Color.Transparent)
                                    Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphIndexRects[cell.ActualGlyphIndex], cell.ActualForeground, 0f, Vector2.Zero, cell.ActualSpriteEffect, 0.4f);
                            }
                        }

                    }
                }
            }

            if (surface.Tint.A != 0)
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

            AfterRenderCallback?.Invoke(Batch);

            Batch.End();
#elif SFML
            Batch.Start(surface, renderingMatrix);

            BeforeRenderCallback?.Invoke(Batch);

            if (surface.Tint.A != 255)
            {
                Cell cell;

                if (surface.DefaultBackground.A != 0)
                    Batch.DrawSurfaceFill(surface.DefaultBackground, Color.Transparent);

                for (int l = 0; l < layers.Length; l++)
                {
                    if (layers[l].IsVisible)
                    {
                        for (int i = 0; i < layers[l].RenderCells.Length; i++)
                        {
                            cell = layers[l].RenderCells[i];

                            if (cell.IsVisible)
                            {
                                Batch.DrawCell(cell, surface.RenderRects[i], surface.DefaultBackground, surface.Font);
                            }
                        }
                    }
                }
            }

            if (surface.Tint.A != 0)
                Batch.DrawSurfaceFill(surface.Tint, Color.Transparent);

            AfterRenderCallback?.Invoke(Batch);

            if (CallBatchEnd)
                Batch.End(Engine.Device, RenderStates.Default);
#endif
        }

    }
}
