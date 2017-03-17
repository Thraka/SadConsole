using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Renderers
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
        public Color ModalTint { get; set; } = new Color((byte)0, (byte)0, (byte)0, (byte)(255f * 0.25f));

        /// <summary>
        /// Renders a 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="renderingMatrix"></param>

        public override void Render(ISurface surface, bool force = false)
        {
            if (IsModal && ModalTint.A != 0)
                Global.DrawCalls.Add(new DrawCallColoredRect(new Rectangle(0, 0, Global.RenderWidth, Global.RenderHeight), ModalTint));

            base.Render(surface);
        }
    }
}
