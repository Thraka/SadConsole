using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Host
{
    public static class Settings
    {
        /// <summary>
        /// Tells MonoGame to use a full screen resolution change instead of soft (quick) full screen. Must be set before the game is created.
        /// </summary>
        public static bool UseHardwareFullScreen { get; set; } = false;

        /// <summary>
        /// The blend state used with <see cref="SadConsole.Renderers.IRenderer"/> on surfaces.
        /// </summary>
        public static Microsoft.Xna.Framework.Graphics.BlendState MonoGameSurfaceBlendState { get; set; } = Microsoft.Xna.Framework.Graphics.BlendState.Additive;

        /// <summary>
        /// The blend state used when drawing surfaces to the screen.
        /// </summary>
        public static Microsoft.Xna.Framework.Graphics.BlendState MonoGameScreenBlendState { get; set; } = Microsoft.Xna.Framework.Graphics.BlendState.NonPremultiplied;
    }
}
