
#if SFML
using Point = SFML.System.Vector2i;
using Rectangle = SFML.Graphics.IntRect;
using Texture2D = SFML.Graphics.Texture;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using System;
using System.Runtime.Serialization;

namespace SadConsole
{
    /// <summary>
    /// Represents a specific font size from a <see cref="FontMaster"/>.
    /// </summary>
    [DataContract]
    public sealed class Font
    {
        /// <summary>
        /// The size options of a font.
        /// </summary>
        public enum FontSizes
        {
            /// <summary>
            /// One quater the size of the font. (Original Width and Height * 0.25)
            /// </summary>
            Quarter,

            /// <summary>
            /// Half the size of the font. (Original Width and Height * 0.50)
            /// </summary>
            Half,

            /// <summary>
            /// Exact size of the font. (Original Width and Height * 1.0)
            /// </summary>
            One,

            /// <summary>
            /// Two times the size of the font. (Original Width and Height * 2.0)
            /// </summary>
            Two,

            /// <summary>
            /// Two times the size of the font. (Original Width and Height * 3.0)
            /// </summary>
            Three,

            /// <summary>
            /// Two times the size of the font. (Original Width and Height * 4.0)
            /// </summary>
            Four
        }

        /// <summary>
        /// The texture of the font.
        /// </summary>
        public Texture2D FontImage { get; private set; }

        /// <summary>
        /// The width and height of each glyph.
        /// </summary>
        public Point Size { get; private set; }

        /// <summary>
        /// The maximum upper inclusive glyph index of the font.
        /// </summary>
        public int MaxGlyphIndex { get; private set; }

        /// <summary>
        /// Which glyph index is considered completely solid. Used for shading.
        /// </summary>
        public int SolidGlyphIndex { get; set; }

        /// <summary>
        /// A cached array of rectangles of individual glyphs.
        /// </summary>
        public Rectangle[] GlyphIndexRects { get; private set; }

        /// <summary>
        /// How many rows are in this font.
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// The size originally used to create the font from a <see cref="FontMaster"/>.
        /// </summary>
        [DataMember]
        public FontSizes SizeMultiple { get; private set; }

        /// <summary>
        /// The name of the font used when it is registered with the <see cref="Engine.Fonts"/> collection.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        internal Font() { }

        internal Font(FontMaster masterFont, FontSizes fontMultiple)
        {
            Initialize(masterFont, fontMultiple);
        }

        private void Initialize(FontMaster masterFont, FontSizes fontMultiple)
        {
            FontImage = masterFont.Image;
            MaxGlyphIndex = masterFont.Rows * Engine.FontColumns - 1;

            switch (fontMultiple)
            {
                case FontSizes.Quarter:
                    Size = new Point((int)(masterFont.GlyphWidth * 0.25), (int)(masterFont.GlyphHeight * 0.25));
                    break;
                case FontSizes.Half:
                    Size = new Point((int)(masterFont.GlyphWidth * 0.5), (int)(masterFont.GlyphHeight * 0.5));
                    break;
                case FontSizes.One:
                    Size = new Point(masterFont.GlyphWidth, masterFont.GlyphHeight);
                    break;
                case FontSizes.Two:
                    Size = new Point(masterFont.GlyphWidth * 2, masterFont.GlyphHeight * 2);
                    break;
                case FontSizes.Three:
                    Size = new Point(masterFont.GlyphWidth * 3, masterFont.GlyphHeight * 3);
                    break;
                case FontSizes.Four:
                    Size = new Point(masterFont.GlyphWidth * 4, masterFont.GlyphHeight * 4);
                    break;
                default:
                    break;
            }

            if (Size.X == 0 || Size.Y == 0)
                throw new ArgumentException($"This font cannot use size {fontMultiple.ToString()}, at least one axis is 0.", "fontMultiple");
            
            SizeMultiple = fontMultiple;
            Name = masterFont.Name;
            GlyphIndexRects = masterFont.GlyphIndexRects;
            SolidGlyphIndex = masterFont.SolidGlyphIndex;
            Rows = masterFont.Rows;
        }

        /// <summary>
        /// Resizes the graphics device manager based on this font's glyph size.
        /// </summary>
        /// <param name="manager">Graphics device manager to resize.</param>
        /// <param name="width">The width glyphs.</param>
        /// <param name="height">The height glyphs.</param>
        /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
        /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
#if SFML
        public void ResizeGraphicsDeviceManager(RenderWindow manager, int width, int height, int additionalWidth, int additionalHeight)
        {
            manager.Size = new SFML.System.Vector2u((uint)((Size.X * width) + additionalWidth), (uint)((Size.Y * height) + additionalHeight));
            manager.SetView(new View(new FloatRect(0, 0, manager.Size.X, manager.Size.Y)));
            Engine.WindowWidth = (int)manager.Size.X;
            Engine.WindowHeight = (int)manager.Size.Y;
        }

#elif MONOGAME
        public void ResizeGraphicsDeviceManager(GraphicsDeviceManager manager, int width, int height, int additionalWidth, int additionalHeight)
        {
            manager.PreferredBackBufferWidth = (Size.X * width) + additionalWidth;
            manager.PreferredBackBufferHeight = (Size.Y * height) + additionalHeight;
            manager.ApplyChanges();

            Engine.WindowWidth = manager.PreferredBackBufferWidth;
            Engine.WindowHeight = manager.PreferredBackBufferHeight;
        }
#endif

        public Rectangle GetRenderRect(int x, int y)
        {
            Rectangle rect = new Rectangle(x * Size.X, y * Size.Y, Size.X, Size.Y);
#if SFML
            // SFML handles rects for rendering differnetly
            rect.Width = rect.Left + rect.Width;
            rect.Height = rect.Top + rect.Height;
#endif
            return rect;
        }

        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (Engine.Fonts.ContainsKey(Name))
            {
                var master = Engine.Fonts[Name];
                Initialize(master, SizeMultiple);
            }
            else
            {
                throw new Exception($"A font is being used that has not been added to the engine. Name: {Name}");
            }
        }
    }

    /// <summary>
    /// The font stored by the engine. Used to generate the <see cref="Font"/> type used by the engine.
    /// </summary>
    public class FontMaster
    {
        /// <summary>
        /// The name of this font family.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Where this font was loaded from.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The height of each glyph in pixels.
        /// </summary>
        public int GlyphHeight { get; set; }

        /// <summary>
        /// The width of each glyph in pixels.
        /// </summary>
        public int GlyphWidth { get; set; }

        /// <summary>
        /// The amount of pixels between glyphs.
        /// </summary>
        public int GlyphPadding { get; set; }

        /// <summary>
        /// Which glyph index is considered completely solid. Used for shading.
        /// </summary>
        public int SolidGlyphIndex { get; set; } = 219;

        /// <summary>
        /// The total rows in the font.
        /// </summary>
        [IgnoreDataMember]
#if SFML
        public int Rows { get { return (int)Image.Size.Y / (GlyphHeight + GlyphPadding); } }
#elif MONOGAME
        public int Rows { get { return Image.Height / (GlyphHeight + GlyphPadding); } }
#endif
        /// <summary>
        /// The texture used by the font.
        /// </summary>
        [IgnoreDataMember]
        public Texture2D Image { get; private set; }

        /// <summary>
        /// A cached array of rectangles of individual glyphs.
        /// </summary>
        [IgnoreDataMember]
        public Rectangle[] GlyphIndexRects;

#region Methods
        /// <summary>
        /// After the font has been loaded, (with the <see cref="FilePath"/>, <see cref="GlyphHeight"/>, and <see cref="GlyphWidth"/> fields filled out) this method will create the actual texture.
        /// </summary>
        public void Generate()
        {
#if SFML
            Image = new Texture2D(FilePath);
#elif MONOGAME
            using (System.IO.Stream fontStream = System.IO.File.OpenRead(FilePath))
                Image = Texture2D.FromStream(Engine.Device, fontStream);
#endif
            ConfigureRects();
        }

        /// <summary>
        /// Builds the <see cref="GlyphIndexRects"/> array based on the current font settings.
        /// </summary>
        public void ConfigureRects()
        {
            GlyphIndexRects = new Rectangle[Rows * Engine.FontColumns];

            for (int i = 0; i < GlyphIndexRects.Length; i++)
            {
                var cx = i % Engine.FontColumns;
                var cy = i / Engine.FontColumns;

                if (GlyphPadding != 0)
                    GlyphIndexRects[i] = new Rectangle((cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
                                                           (cy * GlyphHeight) + ((cy + 1) * GlyphPadding), GlyphWidth, GlyphHeight);
                else
                    GlyphIndexRects[i] = new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight);

#if SFML
                // rects are used differently with SFML
                GlyphIndexRects[i].Width = GlyphIndexRects[i].Left + GlyphIndexRects[i].Width;
                GlyphIndexRects[i].Height = GlyphIndexRects[i].Top + GlyphIndexRects[i].Height;
#endif
            }
        }

        /// <summary>
        /// Gets a sized font.
        /// </summary>
        /// <param name="multiple">How much to multiple the font size by.</param>
        /// <returns>A font.</returns>
        public Font GetFont(Font.FontSizes multiple)
        {
            return new Font(this, multiple);
        }

        ///// <summary>
        ///// Not used... I think I was going to do something with this...
        ///// </summary>
        //private void GetImageMask()
        //{
        //    Texture2D texture = new Texture2D(Engine.Device, Image.Width, Image.Height,
        //                                        false, SurfaceFormat.Color);
        //    Color[] newPixels = new Color[texture.Width * texture.Height];
        //    Color[] oldPixels = new Color[texture.Width * texture.Height];
        //    texture.GetData<Color>(newPixels);
        //    Image.GetData<Color>(oldPixels);
        //}
        
        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            Generate();
        }
#endregion
    }
}
