using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Represents a graphical font used by SadConsole.
/// </summary>
[DataContract]
public class SadFont : IFont
{
    [DataMember(Name="Mapping")]
    private IndexMapping[]? _remapper;
    [DataMember(Name="SkipGlyphGeneration")]
    private bool _skipAutomaticGlyphGeneration;
    private int _solidGlyphIndex;
    private int _unsupportedGlyphIndex;

    /// <inheritdoc/>
    [DataMember]
    public int SolidGlyphIndex
    {
        get => _solidGlyphIndex;
        set
        {
            _solidGlyphIndex = value;
            SolidGlyphRectangle = GetGlyphSourceRectangle(value);
        }
    }

    /// <summary>
    /// The rectangle associated with the <see cref="SolidGlyphIndex"/>.
    /// </summary>
    public Rectangle SolidGlyphRectangle { get; private set; }

    /// <inheritdoc/>
    [DataMember]
    public int Columns { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public int Rows { get; set; }

    /// <summary>
    /// Gets the total glyphs in this font, which represents the last index. Calculated from <see cref="Columns"/> times <see cref="Rows"/>.
    /// </summary>
    public int TotalGlyphs => Columns * Rows;

    /// <inheritdoc/>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// The name of the image file as defined in the .font file.
    /// </summary>
    [DataMember]
    public string FilePath { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public int GlyphHeight { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public int GlyphWidth { get; set; }

    /// <summary>
    /// The amount of pixels between glyphs.
    /// </summary>
    [DataMember]
    public int GlyphPadding { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public int UnsupportedGlyphIndex
    {
        get => _unsupportedGlyphIndex;
        set
        {
            _unsupportedGlyphIndex = value;
            UnsupportedGlyphRectangle = GetGlyphSourceRectangle(value);
        }
    }

    /// <summary>
    /// The rectangle associated with the <see cref="UnsupportedGlyphIndex"/>.
    /// </summary>
    public Rectangle UnsupportedGlyphRectangle { get; private set; }

    /// <summary>
    /// A dictionary that stores the source rectangles of the font by glyph id.
    /// </summary>
    public Dictionary<int, Rectangle> GlyphRectangles { get; set; } = new Dictionary<int, Rectangle>();

    /// <summary>
    /// True when the font supports SadConsole extended decorators; otherwise false.
    /// </summary>
    [DataMember]
    public bool IsSadExtended { get; set; }

    /// <summary>
    /// The texture used by the font.
    /// </summary>
    public ITexture Image { get; set; }

    /// <summary>
    /// A collection of named glyph definitions.
    /// </summary>
    [DataMember]
    public Dictionary<string, GlyphDefinition> GlyphDefinitions { get; set; } = new Dictionary<string, GlyphDefinition>();

    /// <summary>
    /// Creates a new font with the specified settings.
    /// </summary>
    /// <param name="glyphWidth">The pixel width of each glyph.</param>
    /// <param name="glyphHeight">The pixel height of each glyph.</param>
    /// <param name="glyphPadding">The pixel padding between each glyph.</param>
    /// <param name="rows">Number of glyph rows in the <paramref name="image"/>.</param>
    /// <param name="columns">Number of glyph columns in the <paramref name="image"/>.</param>
    /// <param name="solidGlyphIndex">The index of the glyph that is a solid white box.</param>
    /// <param name="image">The <see cref="ITexture"/> of the font.</param>
    /// <param name="name">A font identifier used for serialization of resources using this font.</param>
    /// <param name="glyphRectangles">Glyph mapping dictionary.</param>
    public SadFont(int glyphWidth, int glyphHeight, int glyphPadding, int rows, int columns, int solidGlyphIndex, ITexture image, string name, Dictionary<int, Rectangle>? glyphRectangles = null)
    {
        FilePath = string.Empty;
        Columns = columns;
        Rows = rows;
        GlyphWidth = glyphWidth;
        GlyphHeight = glyphHeight;
        GlyphPadding = glyphPadding;
        Image = image;
        Name = name;

        if (glyphRectangles != null)
            GlyphRectangles = glyphRectangles;

        _solidGlyphIndex = solidGlyphIndex;
        UnsupportedGlyphIndex = 0;

        ConfigureRects();
    }

    [Newtonsoft.Json.JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private SadFont() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets the rendering rectangle for a glyph.
    /// </summary>
    /// <param name="glyph">The index of the glyph to get.</param>
    /// <returns>The rectangle for the glyph if it exists, otherwise returns <see cref="UnsupportedGlyphRectangle"/>.</returns>
    public Rectangle GetGlyphSourceRectangle(int glyph)
    {
        if (glyph >= 0 && GlyphRectangles.TryGetValue(glyph, out Rectangle value))
            return value;

        return UnsupportedGlyphRectangle;
    }

    /// <summary>
    /// Generates a rectangle for the specified glyph based on the glyph index, <see cref="Rows"/>, <see cref="Columns"/>, and <see cref="GlyphPadding"/> values. For the actual font rectangle, use <see cref="GetGlyphSourceRectangle(int)"/>.
    /// </summary>
    /// <param name="glyph">The glyph.</param>
    /// <returns>A rectangle based on where the font thinks the rectangle should be.</returns>
    public Rectangle GenerateGlyphSourceRectangle(int glyph)
    {
        int cx = glyph % Columns;
        int cy = glyph / Columns;

        if (GlyphPadding != 0)
        {
            return new Rectangle((cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
                                 (cy * GlyphHeight) + ((cy + 1) * GlyphPadding),
                                 GlyphWidth, GlyphHeight);
        }
        else
            return new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight);
    }

    /// <summary>
    /// Gets a <see cref="CellDecorator"/> by the <see cref="GlyphDefinition"/> defined by the font file.
    /// </summary>
    /// <param name="name">The name of the decorator to get.</param>
    /// <param name="color">The color to apply to the decorator.</param>
    /// <returns>The decorator instance.</returns>
    /// <remarks>If the decorator does not exist, <see cref="CellDecorator.Empty"/> is returned.</remarks>
    public CellDecorator GetDecorator(string name, Color color)
    {
        if (GlyphDefinitions.ContainsKey(name))
            return GlyphDefinitions[name].CreateCellDecorator(color);

        return CellDecorator.Empty;
    }

    /// <summary>
    /// A safe way to get a <see cref="GlyphDefinition"/> by name that is defined by the font file.
    /// </summary>
    /// <param name="name">The name of the glyph definition.</param>
    /// <returns>The glyph definition.</returns>
    /// <remarks>If the glyph definition doesn't exist, return s<see cref="GlyphDefinition.Empty"/>.</remarks>
    public GlyphDefinition GetGlyphDefinition(string name)
    {
        if (GlyphDefinitions.ContainsKey(name))
            return GlyphDefinitions[name];

        return GlyphDefinition.Empty;
    }

    /// <summary>
    /// Returns <see langword="true"/> when the glyph has been defined by name.
    /// </summary>
    /// <param name="name">The name of the glyph</param>
    /// <returns><see langword="true"/> when the glyph name exists, otherwise <see langword="false"/>.</returns>
    public bool HasGlyphDefinition(string name) =>
        GlyphDefinitions.ContainsKey(name);

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
    /// Builds the <see cref="GlyphRectangles"/> array based on the current font settings, if the <see cref="GlyphRectangles"/> dictionary is empty.
    /// </summary>
    public void ConfigureRects()
    {
        // If this is empty, it's an old-style font.
        if (GlyphRectangles.Count == 0)
            ForceConfigureRects();
        else
        {
            SolidGlyphRectangle = GetGlyphSourceRectangle(SolidGlyphIndex);
            UnsupportedGlyphRectangle = GetGlyphSourceRectangle(UnsupportedGlyphIndex);
        }
    }

    /// <summary>
    /// Builds the <see cref="GlyphRectangles"/> array based on the current font settings.
    /// </summary>
    public void ForceConfigureRects()
    {
        for (int i = 0; i < Rows * Columns; i++)
        {
            int cx = i % Columns;
            int cy = i / Columns;

            if (GlyphPadding != 0)
            {
                GlyphRectangles[i] = new Rectangle((cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
                                                  (cy * GlyphHeight) + ((cy + 1) * GlyphPadding),
                                                  GlyphWidth, GlyphHeight);
            }
            else
                GlyphRectangles[i] = new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight);
        }

        SolidGlyphRectangle = GetGlyphSourceRectangle(SolidGlyphIndex);
        UnsupportedGlyphRectangle = GetGlyphSourceRectangle(UnsupportedGlyphIndex);
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
        if (FilePath.StartsWith("res:"))
        {
            using (Stream fontStream = typeof(SadFont).Assembly.GetManifestResourceStream(FilePath.Substring(4))!)
                Image = GameHost.Instance.GetTexture(fontStream);
        }
        else
            Image = GameHost.Instance.GetTexture(System.IO.Path.Combine(SadConsole.GameHost.SerializerPathHint, FilePath));

        if (Columns == 0)
            Columns = (int)System.Math.Floor((double)Image.Width / (GlyphWidth + GlyphPadding));

        if (Rows == 0)
            Rows = (int)System.Math.Floor((double)Image.Height / (GlyphHeight + GlyphPadding));

        if (!_skipAutomaticGlyphGeneration)
            ConfigureRects();

        if (_remapper != null)
        {
            foreach (IndexMapping item in _remapper)
                GlyphRectangles[item.From] = GenerateGlyphSourceRectangle(item.To);

            _remapper = null;
        }
    }

    private record struct IndexMapping
    {
        public int From;
        public int To;
    }
}
