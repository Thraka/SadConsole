namespace SadConsole.Consoles
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// Represents the ability to render cell data to the screen.
    /// </summary>
    public interface ITextSurfaceRenderer
    {
        /// <summary>
        /// The SpriteBatch used when rendering cell data.
        /// </summary>
        SpriteBatch Batch { get; }

        /// <summary>
        /// A method called when the <see cref="SpriteBatch"/> has been created and transformed, but before any text characters are drawn.
        /// </summary>
        Action<SpriteBatch> BeforeRenderCallback { get; set; }

        /// <summary>
        /// A method called when all text characters have been drawn and any tinting has been applied.
        /// </summary>
        Action<SpriteBatch> AfterRenderCallback { get; set; }

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
