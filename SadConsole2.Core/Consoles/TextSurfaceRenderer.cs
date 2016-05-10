using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Draws a text surface to the screen.
    /// </summary>
    public class TextSurfaceRenderer : IRender
    {
        /// <summary>
        /// The sprite batch used for drawing to the screen.
        /// </summary>
        public SpriteBatch Batch { get; private set; }

        private Matrix absolutePositionTransform;
        private Matrix positionTransform;
        private Point oldPosition;
        private Point oldAbsolutePosition;

        /// <summary>
        /// Creates a new renderer.
        /// </summary>
        public TextSurfaceRenderer() { Batch = new SpriteBatch(Engine.Device); }

        /// <summary>
        /// Renders a 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="renderingMatrix"></param>
        public void Render(ITextSurfaceView surface, Matrix renderingMatrix)
        {

            Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, renderingMatrix);

            //OnBeforeRender();

            if (surface.Tint.A != 255)
            {
                Cell cell;


                if (surface.DefaultBackground.A != 0)
                    Batch.Draw(Engine.BackgroundCell, surface.AbsoluteArea, null, surface.DefaultBackground);

                for (int i = 0; i < surface.RenderCells.Length; i++)
                {
                    cell = surface.RenderCells[i];

                    if (cell.IsVisible)
                    {
                        if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != surface.DefaultBackground)
                            Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], cell.ActualBackground, 0f, Vector2.Zero, SpriteEffects.None , 0.1f);

                        if (cell.ActualForeground != Color.Transparent)
                            Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.CharacterIndexRects[cell.ActualCharacterIndex], cell.ActualForeground, 0f, Vector2.Zero, cell.ActualSpriteEffect, 0.2f);
                    }
                }

                if (surface.Tint.A != 0)
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None , 0.3f);
            }
            else
            {
                Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None , 0.3f);
            }

            //OnAfterRender();

            Batch.End();
        }

        public void Render(ITextSurfaceView surface, Point position, bool usePixelPositioning = false)
        {
            Matrix matrix;

            if (usePixelPositioning)
            {
                if (oldAbsolutePosition != position)
                {
                    absolutePositionTransform = GetPositionTransform(position, surface.Font.Size, true);
                    oldAbsolutePosition = position;
                }

                matrix = absolutePositionTransform;
            }
            else
            {
                if (position != oldPosition)
                {
                    positionTransform = GetPositionTransform(position, surface.Font.Size, false);
                    oldPosition = position;
                }

                matrix = positionTransform;
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

            if (absolutePositioning)
                worldLocation = position;
            else
                worldLocation = position.ConsoleLocationToWorld(CellSize.X, CellSize.Y);

            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
        }
    }
}
