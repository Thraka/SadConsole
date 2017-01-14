using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Runtime.Serialization;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a text surface to the screen.
    /// </summary>
    [DataContract]
    public class TextSurfaceRenderer
    {
        //private Matrix absolutePositionTransform;
        //private Matrix positionTransform;
        //private Point oldPosition;
        //private Point oldAbsolutePosition;

        public bool CallBatchEnd = true;

        /// <summary>
        /// The sprite batch used for drawing to the screen.
        /// </summary>
        public SpriteBatch Batch { get; private set; }

        /// <summary>
        /// A method called when the <see cref="SpriteBatch"/> has been created and transformed, but before any text characters are drawn.
        /// </summary>
        public Action<SpriteBatch> BeforeRenderCallback { get; set; }

        /// <summary>
        /// A method called when all text characters have been drawn and any tinting has been applied.
        /// </summary>
        public Action<SpriteBatch> AfterRenderCallback { get; set; }

        /// <summary>
        /// Creates a new renderer.
        /// </summary>
        public TextSurfaceRenderer()
        {
            Batch = new SpriteBatch(Global.Device);
        }

        /// <summary>
        /// Renders a surface to the screen.
        /// </summary>
        /// <param name="surface">The surface to render.</param>
        /// <param name="renderingMatrix">Display matrix for the rendered console.</param>
        public virtual void Render(TextSurface surface, Matrix renderingMatrix)
        {
            Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, renderingMatrix);

            BeforeRenderCallback?.Invoke(Batch);

            if (surface.Tint.A != 255)
            {
                Cell cell;


                if (surface.DefaultBackground.A != 0)
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                for (int i = 0; i < surface.RenderCells.Length; i++)
                {
                    cell = surface.RenderCells[i];

                    //if (cell.IsVisible)
                    {
                        if (cell.Background != Color.Transparent && cell.Background != surface.DefaultBackground)
                            Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                        if (cell.Foreground != Color.Transparent)
                            Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphIndexRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.SpriteEffect, 0.4f);
                    }
                }

                if (surface.Tint.A != 0)
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }
            else
            {
                Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }

            AfterRenderCallback?.Invoke(Batch);

            if (CallBatchEnd)
                Batch.End();
        }

        /// <summary>
        /// Renders a surface to the screen.
        /// </summary>
        /// <param name="surface">The surface to render.</param>
        /// <param name="position">Calculates the rendering position on the screen based on the size of the <paramref name="surface"/> parameter.</param>
        /// <param name="usePixelPositioning">Ignores the <paramref name="surface"/> font for positioning and instead treats the <paramref name="position"/> parameter in pixels.</param>
        public void Render(TextSurface surface, Point position, bool usePixelPositioning = false)
        {
            Matrix matrix;

            if (usePixelPositioning)
            {
                //if (oldAbsolutePosition != position)
                //{
                //    absolutePositionTransform = GetPositionTransform(position, surface.Font.Size, true);
                //    oldAbsolutePosition = position;
                //}

                //matrix = absolutePositionTransform;

                matrix = GetPositionTransform(position, surface.Font.Size, true);
            }
            else
            {
                //if (position != oldPosition)
                //{
                //    positionTransform = GetPositionTransform(position, surface.Font.Size, false);
                //    oldPosition = position;
                //}

                //matrix = positionTransform;

                matrix = GetPositionTransform(position, surface.Font.Size, false);
            }

            Render(surface, matrix);
        }

        /// <summary>
        /// Gets the Matrix transform that positions the console on the screen.
        /// </summary>
        /// <returns>The transform.</returns>
        public virtual Matrix GetPositionTransform(Point position, Point CellSize, bool absolutePositioning)
        {
            Point worldLocation;

            //if (absolutePositioning)
                worldLocation = position;
            //else
            //    worldLocation = position.ConsoleLocationToWorld(CellSize.X, CellSize.Y);

            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            Batch = new SpriteBatch(Global.Device);
        }
    }
}
