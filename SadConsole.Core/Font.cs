using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace SadConsole
{
    public sealed class Font
    {
        [IgnoreDataMember]
        public Texture2D FontImage { get; private set; }

        [IgnoreDataMember]
        public Point Size { get; private set; }

        [IgnoreDataMember]
        public int MaxCharacter { get; private set; }

        [IgnoreDataMember]
        public int SolidCharacterIndex { get; set; }

        [IgnoreDataMember]
        public Rectangle[] CharacterIndexRects { get; private set; }

        public int SizeMultiple { get; private set; }

        public string Name { get; private set; }

        internal Font() { }

        internal Font(FontMaster masterFont, int fontMultiple)
        {
            Initialize(masterFont, fontMultiple);
        }

        private void Initialize(FontMaster masterFont, int fontMultiple)
        {
            FontImage = masterFont.Image;
            MaxCharacter = masterFont.Rows * Engine.FontColumns - 1;
            Size = new Point(masterFont.CellWidth * fontMultiple, masterFont.CellHeight * fontMultiple);
            SizeMultiple = fontMultiple;
            Name = masterFont.Name;
            CharacterIndexRects = new Rectangle[masterFont.CharacterIndexRects.Length];
            masterFont.CharacterIndexRects.CopyTo(CharacterIndexRects, 0);
            SolidCharacterIndex = masterFont.SolidCharacterIndex;
        }

        /// <summary>
        /// Resizes the graphics device manager to this font cell size.
        /// </summary>
        /// <param name="manager">Graphics device manager to resize.</param>
        /// <param name="width">The width in cell count.</param>
        /// <param name="height">The height in cell count.</param>
        /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
        /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
        public void ResizeGraphicsDeviceManager(GraphicsDeviceManager manager, int width, int height, int additionalWidth, int additionalHeight)
        {
            manager.PreferredBackBufferWidth = (Size.X * width) + additionalWidth;
            manager.PreferredBackBufferHeight = (Size.Y * height) + additionalHeight;
            manager.ApplyChanges();

            Engine.WindowWidth = manager.PreferredBackBufferWidth;
            Engine.WindowHeight = manager.PreferredBackBufferHeight;
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

    public class FontMaster
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public int CellHeight { get; set; }

        public int CellWidth { get; set; }

        public int CellPadding { get; set; }

        public bool IsDefault { get; set; }

        public int SolidCharacterIndex { get; set; } = 219;

        [IgnoreDataMember]
        public int Rows { get { return Image.Height / (CellHeight + CellPadding); } }

        [IgnoreDataMember]
        public Texture2D Image { get; private set; }

        [IgnoreDataMember]
        public Rectangle[] CharacterIndexRects;

        #region Methods
        /// <summary>
        /// After the font has been loaded, (with the FilePath, CellHeight, and CellWidth fields filled out) this method will create the actual texture.
        /// </summary>
        public void Generate()
        {
            using (System.IO.Stream fontStream = System.IO.File.OpenRead(FilePath))
            {
                Image = Texture2D.FromStream(Engine.Device, fontStream);
            }

            ConfigureRects();
        }

        public void ConfigureRects()
        {
            CharacterIndexRects = new Rectangle[Rows * Engine.FontColumns];

            for (int i = 0; i < CharacterIndexRects.Length; i++)
            {
                var cx = i % Engine.FontColumns;
                var cy = i / Engine.FontColumns;

                if (CellPadding != 0)
                    CharacterIndexRects[i] = new Rectangle((cx * CellWidth) + ((cx + 1) * CellPadding),
                                                           (cy * CellHeight) + ((cy + 1) * CellPadding), CellWidth, CellHeight);
                else
                    CharacterIndexRects[i] = new Rectangle(cx * CellWidth, cy * CellHeight, CellWidth, CellHeight);
            }
        }

        /// <summary>
        /// Gets a sized font.
        /// </summary>
        /// <param name="multiple">How much to multiple the font size by.</param>
        /// <returns>A font.</returns>
        public Font GetFont(int multiple = 1)
        {
            return new Font(this, multiple);
        }


        private void GetImageMask()
        {
            Texture2D texture = new Texture2D(Engine.Device, Image.Width, Image.Height,
                                                false, SurfaceFormat.Color);
            Color[] newPixels = new Color[texture.Width * texture.Height];
            Color[] oldPixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(newPixels);
            Image.GetData<Color>(oldPixels);
        }
        
        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            Generate();
        }
        #endregion
    }
}
