namespace SadConsole.Consoles
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// Represents the ability to render cell data to the screen.
    /// </summary>
    public interface IRender
    {
        /// <summary>
        /// The SpriteBatch used when rendering cell data.
        /// </summary>
        SpriteBatch Batch { get; }

        /// <summary>
        /// Renders the cell data to the screen.
        /// </summary>
        void Render(ITextSurfaceView cells, Point position, bool usePixelPositioning = false);

        /// <summary>
        /// Renders the cell data to the screen.
        /// </summary>
        void Render(ITextSurfaceView cells, Matrix renderingMatrix);
    }
    

}
