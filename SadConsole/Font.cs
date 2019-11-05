using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Represents a specific font size from a <see cref="FontMaster"/>.
    /// </summary>
    [DataContract]
    public sealed class Font
    {
        private int _solidGlyphIndex;
        private Rectangle _solidGlyphRect;

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
                if (GlyphRects != null)
                    _solidGlyphRect = GlyphRects[value];
            }
        }

        /// <summary>
        /// The rectangle associated with the <see cref="SolidGlyphIndex"/>.
        /// </summary>
        public Rectangle SolidGlyphRectangle => _solidGlyphRect;

        /// <summary>
        /// A cached array of rectangles of individual glyphs.
        /// </summary>
        public Rectangle[] GlyphRects { get; private set; }

        /// <summary>
        /// How many columns are in the this font.
        /// </summary>
        [DataMember]
        public int Columns { get; private set; }

        /// <summary>
        /// How many rows are in this font.
        /// </summary>
        public int Rows => (int)System.Math.Ceiling((double)Image.Height / (GlyphHeight + GlyphPadding));

        /// <summary>
        /// The name of the font used when it is registered with the <see cref="Global.Fonts"/> collection.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// The name of the image file as defined in the .font file.
        /// </summary>
        [DataMember]
        public string FilePath { get; set; }

        /// <summary>
        /// The path to the .font definition file.
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
            switch (size)
            {
                case Sizes.Quarter:
                    return new Point((int)(GlyphWidth * 0.25), (int)(GlyphHeight * 0.25));
                case Sizes.Half:
                    return new Point((int)(GlyphWidth * 0.5), (int)(GlyphHeight * 0.5));
                case Sizes.Two:
                    return new Point(GlyphWidth * 2, GlyphHeight * 2);
                case Sizes.Three:
                    return new Point(GlyphWidth * 3, GlyphHeight * 3);
                case Sizes.Four:
                    return new Point(GlyphWidth * 4, GlyphHeight * 4);
                case Sizes.One:
                default:
                    return new Point(GlyphWidth, GlyphHeight);
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> when the glyph has been defined by name.
        /// </summary>
        /// <param name="name">The name of the glyph</param>
        /// <returns><see langword="true"/> when the glyph name exists, otherwise <see langword="false"/>.</returns>
        public bool HasGlyphDefinition(string name) => GlyphDefinitions.ContainsKey(name);

        /// <summary>
        /// After the font has been loaded, (with the <see cref="FilePath"/>, <see cref="GlyphHeight"/>, and <see cref="GlyphWidth"/> fields filled out) this method will create the actual texture.
        /// </summary>
        public void Generate()
        {
            //LoadedFilePath = System.IO.Path.Combine(SadConsole.Global.SerializerPathHint, FilePath);

            // I know.. bad way to do this.. yuck
            //if (!GameHost.LoadingEmbeddedFont)
            //{
            //    Image = GameHost.Instance.GetTexture(LoadedFilePath);

            //    ConfigureRects();
            //}
        }

        /// <summary>
        /// Builds the <see cref="GlyphRects"/> array based on the current font settings.
        /// </summary>
        public void ConfigureRects()
        {
            GlyphRects = new Rectangle[Rows * Columns];

            for (int i = 0; i < GlyphRects.Length; i++)
            {
                int cx = i % Columns;
                int cy = i / Columns;

                if (GlyphPadding != 0)
                {
                    GlyphRects[i] = new Rectangle((cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
                        (cy * GlyphHeight) + ((cy + 1) * GlyphPadding), GlyphWidth, GlyphHeight);
                }
                else
                {
                    GlyphRects[i] = new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight);
                }
            }

            _solidGlyphRect = GlyphRects[SolidGlyphIndex];
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (Columns == 0)
                Columns = 16;

            if (FilePath.StartsWith("res:"))
            {
                using (Stream fontStream = typeof(Font).Assembly.GetManifestResourceStream(FilePath.Substring(4)))
                    Image = GameHost.Instance.GetTexture(fontStream);
            }
            else
                Image = GameHost.Instance.GetTexture(FilePath);
            

            ConfigureRects();
        }

        /// <summary>
        /// Returns a rectangle that is positioned and sized based on the font and the cell position specified.
        /// </summary>
        /// <param name="x">The x-axis of the cell position.</param>
        /// <param name="y">The y-axis of the cell position.</param>
        /// <returns>A new rectangle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle GetRenderRect(int x, int y, Point size) => new Rectangle(x * size.X, y * size.Y, size.X, size.Y);

        /// <summary>
        /// Gets the pixel position of a cell position based on the font size.
        /// </summary>
        /// <param name="position">The position to convert.</param>
        /// <returns>A new pixel point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point GetWorldPosition(Point position, Point size) => new Point(position.X * size.X, position.Y * size.Y);
    }
}
