using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Represents a decorator (glyph and mirror) defined by a font.
    /// </summary>
    public readonly struct GlyphDefinition
    {
        /// <summary>
        /// Gets a glyph definition that has -1 for the glyph.
        /// </summary>
        public static GlyphDefinition Empty { get; } = new GlyphDefinition(-1, 0);

        /// <summary>
        /// The glyph of the decorator.
        /// </summary>
        public int Glyph { get; }

        /// <summary>
        /// The mirror effect of the decorator.
        /// </summary>
        public Mirror Mirror { get; }

        /// <summary>
        /// Creates a new cell decorator instance.
        /// </summary>
        /// <param name="glyph"></param>
        /// <param name="mirror"></param>
        public GlyphDefinition(int glyph, Mirror mirror)
        {
            Glyph = glyph;
            Mirror = mirror;
        }

        /// <summary>
        /// Creates a <see cref="CellDecorator"/> from this definition.
        /// </summary>
        /// <param name="foreground">The color of the decorator.</param>
        /// <returns>A new decorator instance.</returns>
        public CellDecorator CreateCellDecorator(Color foreground) => new CellDecorator(foreground, Glyph, Mirror);

        /// <summary>
        /// Creates a <see cref="Cell"/> from this definition.
        /// </summary>
        /// <param name="foreground">The foreground color of the cell.</param>
        /// <param name="background">The background color of the cell.</param>
        /// <returns>A new cell instance.</returns>
        public Cell CreateCell(Color foreground, Color background) => new Cell(foreground, background, Glyph, Mirror);
    }
}
