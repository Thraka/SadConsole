using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole
{
    public interface IFont
    {
        Rectangle SolidGlyphRectangle { get; }
        Rectangle UnsupportedGlyphRectangle { get; }
    }

    /// <summary>
    /// Represents a graphical font used by SadConsole.
    /// </summary>
    [DataContract]
    public sealed class Font
    {
        private int _solidGlyphIndex;
        private int _unsupportedGlyphIndex;

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

        /// <summary>
        /// Which glyph index is considered completely solid. Used for shading.
        /// </summary>
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

        /// <summary>
        /// A cached array of rectangles of individual glyphs.
        /// </summary>
        //public Rectangle[] GlyphRects { get; private set; }

        /// <summary>
        /// How many columns are in the this font.
        /// </summary>
        [DataMember]
        public int Columns { get; private set; }

        /// <summary>
        /// How many rows are in this font.
        /// </summary>
        [DataMember]
        public int Rows { get; private set; }

        /// <summary>
        /// The name of the font used when it is registered with the <see cref="GameHost.Instance.Fonts"/> collection.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// The name of the image file as defined in the .font file.
        /// </summary>
        [DataMember]
        public string FilePath { get; set; }

        /// <summary>
        /// The height of each glyph in pixels.
        /// </summary>
        [DataMember]
        public int GlyphHeight { get; set; }

        /// <summary>
        /// The width of each glyph in pixels.
        /// </summary>
        [DataMember]
        public int GlyphWidth { get; set; }

        /// <summary>
        /// The amount of pixels between glyphs.
        /// </summary>
        [DataMember]
        public int GlyphPadding { get; set; }

        /// <summary>
        /// The glyph index to use when a glyph used during rendering is not available.
        /// </summary>
        [DataMember]
        public int UnsupportedGlyphIndex
        {
            get => _unsupportedGlyphIndex;
            set
            {
                _unsupportedGlyphIndex = value;
                SolidGlyphRectangle = GetGlyphSourceRectangle(value);
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
        public Font(int glyphWidth, int glyphHeight, int glyphPadding, int rows, int columns, int solidGlyphIndex, ITexture image, string name, Dictionary<int, Rectangle> glyphRectangles)
        {
            Columns = columns;
            Rows = rows;
            GlyphWidth = glyphWidth;
            GlyphHeight = glyphHeight;
            GlyphPadding = glyphPadding;
            Image = image;
            Name = name;
            GlyphRectangles = glyphRectangles;

            ConfigureRects();

            SolidGlyphIndex = solidGlyphIndex;
        }

        [Newtonsoft.Json.JsonConstructor]
        private Font() { }

        /// <summary>
        /// Standard decorators used by your app.
        /// </summary>
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
        /// Gets the pixel size of a font based on a <see cref="Sizes"/>.
        /// </summary>
        /// <param name="size">The desired size.</param>
        /// <returns>The width and height of a font cell.</returns>
        public Point GetFontSize(Sizes size)
        {
            return size switch
            {
                Sizes.Quarter   => new Point((int)(GlyphWidth * 0.25), (int)(GlyphHeight * 0.25)),
                Sizes.Half      => new Point((int)(GlyphWidth * 0.5), (int)(GlyphHeight * 0.5)),
                Sizes.Two       => new Point(GlyphWidth * 2, GlyphHeight * 2),
                Sizes.Three     => new Point(GlyphWidth * 3, GlyphHeight * 3),
                Sizes.Four      => new Point(GlyphWidth * 4, GlyphHeight * 4),
                _               => new Point(GlyphWidth, GlyphHeight),
            };
        }

        /// <summary>
        /// Returns the ratio in size difference between the font's glyph width and height.
        /// </summary>
        /// <param name="fontSize">The glyph size of the font used.</param>
        /// <returns>A tuple with the names (X, Y) where X is the difference of width to height and Y is the difference of height to width.</returns>
        public (float X, float Y) GetGlyphRatio(Point fontSize) =>
            ((float)fontSize.X / fontSize.Y, (float)fontSize.Y / fontSize.X);

        /// <summary>
        /// Returns <see langword="true"/> when the glyph has been defined by name.
        /// </summary>
        /// <param name="name">The name of the glyph</param>
        /// <returns><see langword="true"/> when the glyph name exists, otherwise <see langword="false"/>.</returns>
        public bool HasGlyphDefinition(string name) => GlyphDefinitions.ContainsKey(name);

        /// <summary>
        /// Builds the <see cref="GlyphRects"/> array based on the current font settings.
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
                using (Stream fontStream = typeof(Font).Assembly.GetManifestResourceStream(FilePath.Substring(4)))
                    Image = GameHost.Instance.GetTexture(fontStream);
            }
            else
                Image = GameHost.Instance.GetTexture(System.IO.Path.Combine(SadConsole.GameHost.SerializerPathHint, FilePath));

            if (Columns == 0)
                Columns = 16;

            if (Rows == 0)
                Rows = (int)System.Math.Ceiling((double)Image.Height / (GlyphHeight + GlyphPadding));

            ConfigureRects();
        }

        public static Font LoadBMFont(string file, int baseWidth, int baseHeight)
        {
            var bmFont = SharpFNT.BitmapFont.FromFile(file);
            
            var mapping = new Dictionary<int, Rectangle>();
            var texture = GameHost.Instance.GetTexture(Path.Combine(Path.GetDirectoryName(file), bmFont.Pages[0]));

            foreach (var key in bmFont.Characters.Keys)
            {
                var bmRect = bmFont.Characters[key];
                mapping.Add(key, new Rectangle(bmRect.X, bmRect.Y, bmRect.Width, bmRect.Height));
            }

            return new Font(baseWidth, baseHeight, 0, 1, 1, 0, texture, bmFont.Info.Face, mapping);
        }

        /// <summary>
        /// Returns a rectangle that is positioned and sized based on the font and the cell position specified.
        /// </summary>
        /// <param name="x">The x-axis of the cell position.</param>
        /// <param name="y">The y-axis of the cell position.</param>
        /// <param name="fontSize">The size of the output cell.</param>
        /// <returns>A rectangle to representing a specific cell.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle GetRenderRect(int x, int y, Point fontSize) => new Rectangle(x * fontSize.X, y * fontSize.Y, fontSize.X, fontSize.Y);

        /// <summary>
        /// Gets the pixel position of a cell position based on the font size.
        /// </summary>
        /// <param name="position">The cell position to convert.</param>
        /// <param name="fontSize">The size of the font used to calculate the pixel position.</param>
        /// <returns>A new pixel-positioned point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point GetWorldPosition(Point position, Point fontSize) => new Point(position.X * fontSize.X, position.Y * fontSize.Y);
    }
}
