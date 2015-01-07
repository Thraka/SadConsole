using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace SadConsole
{
    public class Font : FontBase
    {
        public string FilePath { get; set; }

        #region Constructors
        public Font() { }

        public Font(string name, XElement fontXmlNode, GraphicsDevice device)
        {
            XAttribute xwidth = fontXmlNode.Attribute("width");
            XAttribute xheight = fontXmlNode.Attribute("height");
            string filename = fontXmlNode.Value;

            if (xwidth == null || xheight == null)
                throw new Exception("Width or Height attribute for font is missing");

            int width;
            int height;

            if (!int.TryParse(xwidth.Value, out width))
                throw new Exception("Width value is invalid: " + xwidth.Value);
            if (!int.TryParse(xheight.Value, out height))
                throw new Exception("Height value is invalid: " + xheight.Value);

            FilePath = filename;
            Name = name;
            CellWidth = width;
            CellHeight = height;

            Generate();
        }
        #endregion

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

        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            Generate();
        }
        #endregion
    }
}
