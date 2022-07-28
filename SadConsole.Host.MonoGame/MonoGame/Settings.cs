using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
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
        public static Microsoft.Xna.Framework.Graphics.BlendState MonoGameSurfaceBlendState { get; set; }
            = new Microsoft.Xna.Framework.Graphics.BlendState()
            {
                AlphaBlendFunction = BlendFunction.Add,
                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                AlphaSourceBlend = Blend.One,
                ColorBlendFunction = BlendFunction.Add,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                ColorSourceBlend = Blend.SourceAlpha
            };

        /// <summary>
        /// The blend state used when drawing surfaces to the screen.
        /// </summary>
        public static Microsoft.Xna.Framework.Graphics.BlendState MonoGameScreenBlendState { get; set; } = new Microsoft.Xna.Framework.Graphics.BlendState()
        {
            AlphaBlendFunction = BlendFunction.Add,
            AlphaDestinationBlend = Blend.InverseSourceAlpha,
            AlphaSourceBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Add,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorSourceBlend = Blend.SourceAlpha
        };

        /// <summary>
        /// The MonoGame graphics profile to target.
        /// </summary>
        public static GraphicsProfile GraphicsProfile { get; set; }
            = GraphicsProfile.Reach;
    }
}
