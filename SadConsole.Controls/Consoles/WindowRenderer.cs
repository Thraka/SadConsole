using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Renders a popup window taking into account the modal setting.
    /// </summary>
    public class WindowRenderer: TextSurfaceRenderer
    {
        /// <summary>
        /// Creates a new renderer.
        /// </summary>
        public WindowRenderer() { }

        /// <summary>
        /// Indicates the window will be drawn modal (discolored background rect for the game window)
        /// </summary>
        public bool IsModal { get; set; }

        /// <summary>
        /// The color of the modal background.
        /// </summary>
        public Color ModalTint { get; set; } = Color.Black * 0.25f;

        /// <summary>
        /// Renders a 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="renderingMatrix"></param>

        public override void Render(ITextSurface surface, Matrix renderingMatrix)
        {
            if (IsModal)
            {
                Batch.Begin(samplerState: SamplerState.PointClamp);
                Batch.Draw(surface.Font.FontImage, new Rectangle(0, 0, Engine.Device.PresentationParameters.BackBufferWidth, Engine.Device.PresentationParameters.BackBufferHeight), surface.Font.CharacterIndexRects[surface.Font.SolidCharacterIndex], ModalTint);
                Batch.End();
            }

            base.Render(surface, renderingMatrix);
        }
    }
}
