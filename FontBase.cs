using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace SadConsole
{
    public abstract class FontBase
    {
        public string Name { get; set; }
    
        public int CellHeight { get; set; }
    
        public int CellWidth { get; set; }
    
        public int CellPadding { get; set; }
    
        public bool IsDefault { get; set; }
    
        [IgnoreDataMember]
        public int Rows { get { return Image.Height / (CellHeight + CellPadding); } }
    
        [IgnoreDataMember]
        public Texture2D Image { get; protected set; }
    
        [IgnoreDataMember]
        public Rectangle[] CharacterIndexRects;
    
        #region Constructors
        protected FontBase() { }
    
        #endregion
    
        #region Methods
    
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
    
        private void GetImageMask()
        {
            Texture2D texture = new Texture2D(Engine.Device, Image.Width, Image.Height,
                                                false, SurfaceFormat.Color);
            Color[] newPixels = new Color[texture.Width * texture.Height];
            Color[] oldPixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(newPixels);
            Image.GetData<Color>(oldPixels);
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
            manager.PreferredBackBufferWidth = (CellWidth * width) + additionalWidth;
            manager.PreferredBackBufferHeight = (CellHeight * height) + additionalHeight;
            manager.ApplyChanges();
    
            Engine.WindowWidth = manager.PreferredBackBufferWidth;
            Engine.WindowHeight = manager.PreferredBackBufferHeight;
        }
    
        #endregion
    }
}
