using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Runtime.Serialization;

namespace SadConsole
{
    /// <summary>
    /// Represents a specific font size from a <see cref="FontMaster"/>.
    /// </summary>
    public sealed class Font
    {
        private int solidGlyphIndex;
        private Rectangle solidGlyphRect;

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
        public int SolidGlyphIndex { get { return solidGlyphIndex; } set { solidGlyphIndex = value; solidGlyphRect = GlyphRects[value]; } }

        /// <summary>
        /// The rectangle associated with the <see cref="SolidGlyphIndex"/>.
        /// </summary>
        public Rectangle SolidGlyphRectangle { get { return solidGlyphRect; } }

        /// <summary>
        /// A cached array of rectangles of individual glyphs.
        /// </summary>
        public Rectangle[] GlyphRects { get; private set; }

        /// <summary>
        /// How many columns are in the this font.
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// How many rows are in this font.
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// The size originally used to create the font from a <see cref="FontMaster"/>.
        /// </summary>
        public FontSizes SizeMultiple { get; private set; }

        /// <summary>
        /// The name of the font used when it is registered with the <see cref="Engine.Fonts"/> collection.
        /// </summary>
        public string Name { get; private set; }

        internal Font() { }

        internal Font(FontMaster masterFont, FontSizes fontMultiple)
        {
            Initialize(masterFont, fontMultiple);
        }

        private void Initialize(FontMaster masterFont, FontSizes fontMultiple)
        {
            FontImage = masterFont.Image;
            MaxGlyphIndex = masterFont.Rows * masterFont.Columns - 1;

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
            GlyphRects = masterFont.GlyphIndexRects;
            SolidGlyphIndex = masterFont.SolidGlyphIndex;
            Rows = masterFont.Rows;
            Columns = masterFont.Columns;
        }

        /// <summary>
        /// Resizes the graphics device manager based on this font's glyph size.
        /// </summary>
        /// <param name="manager">Graphics device manager to resize.</param>
        /// <param name="width">The width glyphs.</param>
        /// <param name="height">The height glyphs.</param>
        /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
        /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
        public void ResizeGraphicsDeviceManager(GraphicsDeviceManager manager, int width, int height, int additionalWidth, int additionalHeight)
        {
            int oldWidth = manager.PreferredBackBufferWidth;
            int oldHeight = manager.PreferredBackBufferHeight;

            manager.PreferredBackBufferWidth = (Size.X * width) + additionalWidth;
            manager.PreferredBackBufferHeight = (Size.Y * height) + additionalHeight;

            Global.WindowWidth = Global.RenderWidth = manager.PreferredBackBufferWidth;
            Global.WindowHeight = Global.RenderHeight = manager.PreferredBackBufferHeight;

            int diffWidth = (Global.RenderWidth - oldWidth) / 2;
            int diffHeight = (Global.RenderHeight - oldHeight) / 2;

            // Center screen
            //if (Game.Instance != null)
            //    Game.Instance.Window.Position = new Point(Game.Instance.Window.Position.X - diffWidth, Game.Instance.Window.Position.Y - diffHeight);

            manager.ApplyChanges();
        }

        public Rectangle GetRenderRect(int x, int y)
        {
            return new Rectangle(x * Size.X, y * Size.Y, Size.X, Size.Y);
        }

        public Point GetWorldPosition(int x, int y)
        {
            return GetWorldPosition(new Point(x, y));
        }

        public Point GetWorldPosition(Point position)
        {
            return new Point(position.X * Size.X, position.Y * Size.Y);
        }

        
    }

    /// <summary>
    /// The font stored by the engine. Used to generate the <see cref="Font"/> type used by the engine.
    /// </summary>
    [DataContract]
    public class FontMaster
    {
        /// <summary>
        /// The name of this font family.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Where this font was loaded from.
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
        /// The total rows in the font.
        /// </summary>
        public int Rows { get { return Image.Height / (GlyphHeight + GlyphPadding); } }
        /// <summary>
        /// The texture used by the font.
        /// </summary>
        public Texture2D Image { get; private set; }

        /// <summary>
        /// A cached array of rectangles of individual glyphs.
        /// </summary>
        public Rectangle[] GlyphIndexRects;

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
            Image = fontImage;
            GlyphWidth = glyphWidth;
            GlyphHeight = glyphHeight;
            Columns = totalColumns;
            GlyphPadding = glyphPadding;

            ConfigureRects();
        }

#region Methods
        /// <summary>
        /// After the font has been loaded, (with the <see cref="FilePath"/>, <see cref="GlyphHeight"/>, and <see cref="GlyphWidth"/> fields filled out) this method will create the actual texture.
        /// </summary>
        public void Generate()
        {
            string file = System.IO.Path.Combine(Global.SerializerPathHint, FilePath);


            using (System.IO.Stream fontStream = TitleContainer.OpenStream(file))
                Image = Texture2D.FromStream(Global.GraphicsDevice, fontStream);

            ConfigureRects();
        }

        /// <summary>
        /// Builds the <see cref="GlyphIndexRects"/> array based on the current font settings.
        /// </summary>
        public void ConfigureRects()
        {
            GlyphIndexRects = new Rectangle[Rows * Columns];

            for (int i = 0; i < GlyphIndexRects.Length; i++)
            {
                var cx = i % Columns;
                var cy = i / Columns;

                if (GlyphPadding != 0)
                    GlyphIndexRects[i] = new Rectangle((cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
                                                           (cy * GlyphHeight) + ((cy + 1) * GlyphPadding), GlyphWidth, GlyphHeight);
                else
                    GlyphIndexRects[i] = new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight);
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
            if (Columns == 0)
                Columns = 16;

            Generate();
        }
#endregion
    }
}
