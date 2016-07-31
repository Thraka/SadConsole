#if SFML
using Matrix = SFML.Graphics.Transform;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Renders a popup window taking into account the modal setting.
    /// </summary>
    public class WindowRenderer: ControlsConsoleRenderer
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
        public Color ModalTint { get; set; } = new Color(0, 0, 0, (byte)(255f * 0.25f));

        /// <summary>
        /// Renders a 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="renderingMatrix"></param>

        public override void Render(ITextSurfaceRendered surface, Matrix renderingMatrix)
        {
            if (IsModal && ModalTint.A != 0)
            {
#if SFML
                Batch.Start(surface, Matrix.Identity);
                Batch.DrawQuad(new IntRect(0, 0, (int)Engine.Device.Size.X, (int)Engine.Device.Size.Y), surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], ModalTint);
                Batch.End(Engine.Device, RenderStates.Default);
#elif MONOGAME
                Batch.Begin(samplerState: SamplerState.PointClamp);
                Batch.Draw(surface.Font.FontImage, new Rectangle(0, 0, Engine.Device.PresentationParameters.BackBufferWidth, Engine.Device.PresentationParameters.BackBufferHeight), surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], ModalTint);
                Batch.End();
#endif
            }

            base.Render(surface, renderingMatrix);
        }
    }
}
