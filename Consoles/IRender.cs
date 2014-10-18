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
        /// Indicates whether or not this console is visible.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// A transform used when rendering the cell area.
        /// </summary>
        Matrix? Transform { get; set; }

        /// <summary>
        /// When true, positions the render area based on pixels rather than font cell size.
        /// </summary>
        bool UseAbsolutePositioning { get; set; }

        /// <summary>
        /// The SpriteBatch used when rendering cell data.
        /// </summary>
        SpriteBatch Batch { get; }

        /// <summary>
        /// The rendering size of each cell.
        /// </summary>
        Point CellSize { get; set; }

        /// <summary>
        /// Gets or sets the position to render the cells.
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// The area of the cell data to render.
        /// </summary>
        Rectangle ViewArea { get; set; }

        /// <summary>
        /// The cell data used when rendering.
        /// </summary>
        CellSurface CellData { get; set; }

        /// <summary>
        /// Gets or sets the font used when rendering this surface.
        /// </summary>
        FontBase Font { get; set; }

        /// <summary>
        /// Renders the cell data to the screen.
        /// </summary>
        void Render();
    }
}
