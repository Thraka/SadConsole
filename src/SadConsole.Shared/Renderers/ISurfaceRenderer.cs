using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Represents the ability to render cell data to the screen.
    /// </summary>
    public interface ISurfaceRenderer
    {
        /// <summary>
        /// A method called when the <see cref="SpriteBatch"/> has been created and transformed, but before any text characters are drawn.
        /// </summary>
        Action<SpriteBatch> BeforeRenderCallback { get; set; }

        /// <summary>
        /// A method called when all text characters have been drawn but before any tinting has been applied.
        /// </summary>
        Action<SpriteBatch> BeforeRenderTintCallback { get; set; }

        /// <summary>
        /// A method called when all text characters have been drawn and any tinting has been applied.
        /// </summary>
        Action<SpriteBatch> AfterRenderCallback { get; set; }
        
        /// <summary>
        /// Renders the cell data to the screen.
        /// </summary>
        void Render(Surfaces.ISurface cells, bool force = false);
    }
}
