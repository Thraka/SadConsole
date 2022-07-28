using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Represents a font used by the rendering engine.
/// </summary>
public interface IFont  // TODO: We should probably support IDisposable, though you generally don't destroy fonts during your game...
{
    /// <summary>
    /// The name of the font used when it is registered with the <see cref="GameHost.Fonts"/> collection.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The height of each glyph in pixels.
    /// </summary>
    int GlyphHeight { get; }

    /// <summary>
    /// The width of each glyph in pixels.
    /// </summary>
    int GlyphWidth { get; }

    /// <summary>
    /// Which glyph index is considered completely solid. Used for shading.
    /// </summary>
    int SolidGlyphIndex { get; }

    /// <summary>
    /// Gets how many glyphs this font has.
    /// </summary>
    int TotalGlyphs { get; }

    /// <summary>
    /// The rectangle to draw the solid glyph used for shading.
    /// </summary>
    Rectangle SolidGlyphRectangle { get; }

    /// <summary>
    /// The glyph index to use when an unsupported glyph is used during rendering.
    /// </summary>
    int UnsupportedGlyphIndex { get; }

    /// <summary>
    /// The rectangle to draw when a glyph that isn't supported is used by a surface.
    /// </summary>
    Rectangle UnsupportedGlyphRectangle { get; }

    /// <summary>
    /// True when the font supports SadConsole extended decorators; otherwise false.
    /// </summary>
    bool IsSadExtended { get; }

    /// <summary>
    /// The texture used by the font.
    /// </summary>
    ITexture Image { get; }

    /// <summary>
    /// Gets the rendering rectangle for a glyph.
    /// </summary>
    /// <param name="glyph">The index of the glyph to get.</param>
    /// <returns>The rectangle for the glyph.</returns>
    Rectangle GetGlyphSourceRectangle(int glyph);

    /// <summary>
    /// Gets the pixel size of a font based on a <see cref="IFont.Sizes"/>.
    /// </summary>
    /// <param name="size">The desired size.</param>
    /// <returns>The width and height of a font cell.</returns>
    public Point GetFontSize(IFont.Sizes size)
    {
        return size switch
        {
            IFont.Sizes.Quarter => new Point((int)(GlyphWidth * 0.25), (int)(GlyphHeight * 0.25)),
            IFont.Sizes.Half => new Point((int)(GlyphWidth * 0.5), (int)(GlyphHeight * 0.5)),
            IFont.Sizes.Two => new Point(GlyphWidth * 2, GlyphHeight * 2),
            IFont.Sizes.Three => new Point(GlyphWidth * 3, GlyphHeight * 3),
            IFont.Sizes.Four => new Point(GlyphWidth * 4, GlyphHeight * 4),
            _ => new Point(GlyphWidth, GlyphHeight),
        };
    }

    /// <summary>
    /// Gets a <see cref="CellDecorator"/> by the <see cref="GlyphDefinition"/> defined by the font file.
    /// </summary>
    /// <param name="name">The name of the decorator to get.</param>
    /// <param name="color">The color to apply to the decorator.</param>
    /// <returns>The decorator instance.</returns>
    /// <remarks>If the decorator does not exist, <see cref="CellDecorator.Empty"/> is returned.</remarks>
    CellDecorator GetDecorator(string name, Color color);

    /// <summary>
    /// A collection of named glyph definitions.
    /// </summary>
    Dictionary<string, GlyphDefinition> GlyphDefinitions { get; }

    /// <summary>
    /// Returns <see langword="true"/> when the glyph has been defined by name.
    /// </summary>
    /// <param name="name">The name of the glyph</param>
    /// <returns><see langword="true"/> when the glyph name exists, otherwise <see langword="false"/>.</returns>
    bool HasGlyphDefinition(string name);

    /// <summary>
    /// Gets a <see cref="GlyphDefinition"/> by name that is defined by the font file.
    /// </summary>
    /// <param name="name">The name of the glyph definition.</param>
    /// <returns>The glyph definition.</returns>
    /// <remarks>If the glyph definition doesn't exist, return s<see cref="GlyphDefinition.Empty"/>.</remarks>
    GlyphDefinition GetGlyphDefinition(string name);

    /// <summary>
    /// The size options of a font.
    /// </summary>
    public enum Sizes
    {
        /// <summary>
        /// One quater the size of the font. (Original Width and Height * 0.25)
        /// </summary>
        Quarter = 0,

        /// <summary>
        /// Half the size of the font. (Original Width and Height * 0.50)
        /// </summary>
        Half = 1,

        /// <summary>
        /// Exact size of the font. (Original Width and Height * 1.0)
        /// </summary>
        One = 2,

        /// <summary>
        /// Two times the size of the font. (Original Width and Height * 2.0)
        /// </summary>
        Two = 3,

        /// <summary>
        /// Two times the size of the font. (Original Width and Height * 3.0)
        /// </summary>
        Three = 4,

        /// <summary>
        /// Two times the size of the font. (Original Width and Height * 4.0)
        /// </summary>
        Four = 5
    }
}
