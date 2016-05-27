using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Consoles
{
    public class WindowRenderer: ITextSurfaceRenderer
    {
        //private Matrix absolutePositionTransform;
        //private Matrix positionTransform;
        //private Point oldPosition;
        //private Point oldAbsolutePosition;

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
        public WindowRenderer() { Batch = new SpriteBatch(Engine.Device); }

        public bool IsModal { get; set; }

        public Color ModalTint { get; set; } = Color.Black * 0.25f;

        /// <summary>
        /// Renders a 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="renderingMatrix"></param>
        public virtual void Render(ITextSurface surface, Matrix renderingMatrix)
        {
            if (IsModal)
            {
                Batch.Begin(samplerState: SamplerState.PointClamp);
                Batch.Draw(surface.Font.FontImage, new Rectangle(0, 0, Engine.Device.PresentationParameters.BackBufferWidth, Engine.Device.PresentationParameters.BackBufferHeight), surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], ModalTint);
                Batch.End();
            }

            Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, renderingMatrix);
            
            BeforeRenderCallback?.Invoke(Batch);

            if (surface.Tint.A != 255)
            {
                Cell cell;


                if (surface.DefaultBackground.A != 0)
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                for (int i = 0; i < surface.RenderCells.Length; i++)
                {
                    cell = surface.RenderCells[i];

                    if (cell.IsVisible)
                    {
                        if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != surface.DefaultBackground)
                            Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], cell.ActualBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                        if (cell.ActualForeground != Color.Transparent)
                            Batch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.CharacterIndexRects[cell.ActualCharacterIndex], cell.ActualForeground, 0f, Vector2.Zero, cell.ActualSpriteEffect, 0.4f);
                    }
                }

                if (surface.Tint.A != 0)
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }
            else
            {
                Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }

            AfterRenderCallback?.Invoke(Batch);

            Batch.End();
        }

        public void Render(ITextSurface surface, Point position, bool usePixelPositioning = false)
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

            if (absolutePositioning)
                worldLocation = position;
            else
                worldLocation = position.ConsoleLocationToWorld(CellSize.X, CellSize.Y);

            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
        }
    }
}
