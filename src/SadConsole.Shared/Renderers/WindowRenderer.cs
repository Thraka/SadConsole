using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.DrawCalls;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Renders a popup window taking into account the modal setting.
    /// </summary>
    public class Window: ControlsConsole
    {
        /// <summary>
        /// Creates a new renderer.
        /// </summary>
        public Window() { }

        /// <summary>
        /// Indicates the window will be drawn modal (discolored background rect for the game window)
        /// </summary>
        public bool IsModal { get; set; }

        /// <summary>
        /// The color of the modal background.
        /// </summary>
        public Color ModalTint { get; set; } = new Color((byte)0, (byte)0, (byte)0, (byte)(255f * 0.25f));
    }
}
