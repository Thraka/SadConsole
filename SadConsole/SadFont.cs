using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Represents a graphical font used by SadConsole.
    /// </summary>
    [DataContract]
    public class SadFont: IFont
    {
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
        public int Columns { get; protected set; }

        /// <inheritdoc/>
        [DataMember]
        public int Rows { get; protected set; }

        /// <inheritdoc/>
        [DataMember]
        public string Name { get; protected set; }

        /// <summary>
        /// The name of the image file as defined in the .font file.
        /// </summary>
        [DataMember]
        public string FilePath { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public int GlyphHeight { get; protected set; }

        /// <inheritdoc/>
        [DataMember]
        public int GlyphWidth { get; protected set; }

        /// <summary>
        /// The amount of pixels between glyphs.
        /// </summary>
        [DataMember]
        public int GlyphPadding { get; protected set; }

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
        /// Standard decorators used by your app.
        /// </summary>
        [DataMember]
        private Dictionary<string, GlyphDefinition> GlyphDefinitions { get; } = new Dictionary<string, GlyphDefinition>();

        /// <summary>
        /// Creates a new font with the specified settings.
        /// </summary>
        /// <param name="glyphWidth">The pixel width of each glyph.</param>
        /// <param name="glyphHeight">The pixel height of each glyph.</param>
        /// <param name="glyphPadding">The pixel padding between each glyph.</param>
        /// <param name="rows">Number of glyph rows in the <paramref name="image"/>.</param>
        /// <param name="columns">Number of glyph columns in the <paramref name="image"/>.</param>
        /// <param name="solidGlyphIndex">The index of the glyph that is a solid white box.</param>
        /// <param name="image">The texture for of the font.</param>
        /// <param name="name">A font identifier used for serialization of resources using this font.</param>
        /// <param name="glyphRectangles">Glyph mapping dictionary.</param>
        public SadFont(int glyphWidth, int glyphHeight, int glyphPadding, int rows, int columns, int solidGlyphIndex, ITexture image, string name, Dictionary<int, Rectangle> glyphRectangles = null)
        {
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

            ConfigureRects();
        }

        [Newtonsoft.Json.JsonConstructor]
        private SadFont() { }

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
        /// Gets a <see cref="CellDecorator"/> by the <see cref="GlyphDefinition"/> defined by the font file.
        /// </summary>
        /// <param name="name">The name of the decorator to get.</param>
        /// <param name="color">The color to apply to the decorator.</param>
        /// <returns>The decorator instance.</returns>
        /// <remarks>If the decorator does not exist, <see cref="CellDecorator.Empty"/> is returned.</remarks>
        public CellDecorator GetDecorator(string name, Color color)
        {
            if (GlyphDefinitions.ContainsKey(name))
            {
                return GlyphDefinitions[name].CreateCellDecorator(color);
            }

            return CellDecorator.Empty;
        }

        /// <summary>
        /// Gets a <see cref="GlyphDefinition"/> by name that is defined by the font file.
        /// </summary>
        /// <param name="name">The name of the glyph definition.</param>
        /// <returns>The glyph definition.</returns>
        /// <remarks>If the glyph definition doesn't exist, return s<see cref="GlyphDefinition.Empty"/>.</remarks>
        public GlyphDefinition GetGlyphDefinition(string name)
        {
            if (GlyphDefinitions.ContainsKey(name))
            {
                return GlyphDefinitions[name];
            }

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
        /// Builds the <see cref="GlyphRectangles"/> array based on the current font settings.
        /// </summary>
        public void ConfigureRects()
        {
            // If this is empty, it's an old-style font.
            if (GlyphRectangles.Count == 0)
            {
                for (int i = 0; i < Rows * Columns; i++)
                {
                    int cx = i % Columns;
                    int cy = i / Columns;

                    if (GlyphPadding != 0)
                    {
                        GlyphRectangles.Add(i, new Rectangle((cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
                                                         (cy * GlyphHeight) + ((cy + 1) * GlyphPadding),
                                                         GlyphWidth, GlyphHeight));
                    }
                    else
                    {
                        GlyphRectangles.Add(i, new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight));
                    }
                }
            }

            SolidGlyphRectangle = GetGlyphSourceRectangle(SolidGlyphIndex);
            UnsupportedGlyphRectangle = GetGlyphSourceRectangle(UnsupportedGlyphIndex);
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (FilePath.StartsWith("res:"))
            {
                using (Stream fontStream = typeof(SadFont).Assembly.GetManifestResourceStream(FilePath.Substring(4)))
                    Image = GameHost.Instance.GetTexture(fontStream);
            }
            else
                Image = GameHost.Instance.GetTexture(System.IO.Path.Combine(SadConsole.GameHost.SerializerPathHint, FilePath));

            if (Columns == 0)
                Columns = (int)System.Math.Floor((double)Image.Width / (GlyphWidth + GlyphPadding));

            if (Rows == 0)
                Rows = (int)System.Math.Floor((double)Image.Height / (GlyphHeight + GlyphPadding));

            ConfigureRects();
        }

        // TODO
        public static SadFont LoadBMFont(string file, int baseWidth, int baseHeight)
        {
            throw new NotSupportedException();
            //var bmFont = SharpFNT.BitmapFont.FromFile(file);
            
            //var mapping = new Dictionary<int, Rectangle>();
            //var texture = GameHost.Instance.GetTexture(Path.Combine(Path.GetDirectoryName(file), bmFont.Pages[0]));

            //foreach (var key in bmFont.Characters.Keys)
            //{
            //    var bmRect = bmFont.Characters[key];
            //    mapping.Add(key, new Rectangle(bmRect.X, bmRect.Y, bmRect.Width, bmRect.Height));
            //}

            //return new Font(baseWidth, baseHeight, 0, 1, 1, 0, texture, bmFont.Info.Face, mapping);
        }
    }
}
