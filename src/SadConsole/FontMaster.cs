#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The font stored by the engine. Used to generate the <see cref="Font"/> type used by the engine.
    /// </summary>
    [DataContract]
    public sealed class FontMaster
    {
        private Dictionary<Font.FontSizes, Font> cachedFonts = new Dictionary<Font.FontSizes, Font>();

        /// <summary>
        /// The name of this font family.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The name of the image file as defined in the .font file.
        /// </summary>
        [DataMember]
        public string FilePath { get; set; }

        /// <summary>
        /// The path to the file per <see cref="SadConsole.Global.SerializerPathHint"/>.
        /// </summary>
        public string LoadedFilePath { get; private set; }

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
        /// Which glyph index is considered completely solid. Used for shading.
        /// </summary>
        [DataMember]
        public int SolidGlyphIndex { get; set; } = 219;

        /// <summary>
        /// The amount of columns the font uses, defaults to 16.
        /// </summary>
        [DataMember]
        public int Columns { get; set; } = 16;

        /// <summary>
        /// True when the font supports SadConsole extended decorators; otherwise false.
        /// </summary>
        [DataMember]
        public bool IsSadExtended { get; set; }

        /// <summary>
        /// The total rows in the font.
        /// </summary>
        public int Rows => (int)System.Math.Ceiling((double)Image.Height / (GlyphHeight + GlyphPadding));

        /// <summary>
        /// The texture used by the font.
        /// </summary>
        public Texture2D Image { get; internal set; }

        /// <summary>
        /// A cached array of rectangles of individual glyphs.
        /// </summary>
        public Rectangle[] GlyphIndexRects;

        /// <summary>
        /// Standard decorators used by your app.
        /// </summary>
        [DataMember]
        private Dictionary<string, GlyphDefinition> GlyphDefinitions { get; } = new Dictionary<string, GlyphDefinition>();

        /// <summary>
        /// Creates a SadConsole font using an existing image.
        /// </summary>
        /// <param name="fontImage">The image for the font.</param>
        /// <param name="glyphWidth">The width of each glyph.</param>
        /// <param name="glyphHeight">The height of each glyph.</param>
        /// <param name="totalColumns">Glyph columns in the font texture, defaults to 16.</param>
        /// <param name="glyphPadding">Pixels between each glyph, defaults to 0.</param>
        public FontMaster(Texture2D fontImage, int glyphWidth, int glyphHeight, int totalColumns = 16, int glyphPadding = 0)
        {
            cachedFonts = new Dictionary<Font.FontSizes, Font>();
            Image = fontImage;
            GlyphWidth = glyphWidth;
            GlyphHeight = glyphHeight;
            Columns = totalColumns;
            GlyphPadding = glyphPadding;

            ConfigureRects();
        }

        [Newtonsoft.Json.JsonConstructor]
        private FontMaster()
        {

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
        public bool HasGlyphDefinition(string name) => GlyphDefinitions.ContainsKey(name);

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
            public SpriteEffects Mirror { get; }

            /// <summary>
            /// Creates a new cell decorator instance.
            /// </summary>
            /// <param name="glyph"></param>
            /// <param name="mirror"></param>
            public GlyphDefinition(int glyph, SpriteEffects mirror)
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

        /// <summary>
        /// After the font has been loaded, (with the <see cref="FilePath"/>, <see cref="GlyphHeight"/>, and <see cref="GlyphWidth"/> fields filled out) this method will create the actual texture.
        /// </summary>
        public void Generate()
        {
            cachedFonts = new Dictionary<Font.FontSizes, Font>();

            LoadedFilePath = System.IO.Path.Combine(Global.SerializerPathHint, FilePath);

            // I know.. bad way to do this.. yuck
            if (!Settings.LoadingEmbeddedFont)
            {
                using (System.IO.Stream fontStream = TitleContainer.OpenStream(LoadedFilePath))
                {
                    Image = Texture2D.FromStream(Global.GraphicsDevice, fontStream);
                }

                ConfigureRects();
            }
        }

        /// <summary>
        /// Builds the <see cref="GlyphIndexRects"/> array based on the current font settings.
        /// </summary>
        public void ConfigureRects()
        {
            GlyphIndexRects = new Rectangle[Rows * Columns];

            for (int i = 0; i < GlyphIndexRects.Length; i++)
            {
                int cx = i % Columns;
                int cy = i / Columns;

                if (GlyphPadding != 0)
                {
                    GlyphIndexRects[i] = new Rectangle((cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
                        (cy * GlyphHeight) + ((cy + 1) * GlyphPadding), GlyphWidth, GlyphHeight);
                }
                else
                {
                    GlyphIndexRects[i] = new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight);
                }
            }
        }

        /// <summary>
        /// Gets a sized font.
        /// </summary>
        /// <param name="multiple">How much to multiple the font size by.</param>
        /// <returns>A font.</returns>
        public Font GetFont(Font.FontSizes multiple)
        {
            if (cachedFonts.ContainsKey(multiple))
            {
                return cachedFonts[multiple];
            }

            var font = new Font(this, multiple);
            cachedFonts.Add(multiple, font);
            return font;
        }

        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (Columns == 0)
            {
                Columns = 16;
            }

            Generate();
        }
    }
}
