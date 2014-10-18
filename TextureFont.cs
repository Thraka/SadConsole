using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace SadConsole
{
    public class TextureFont : FontBase
    {
        public TextureFont(string name, int cellWidth, int cellHeight, Texture2D texture)
        {
            Name = name;
            Image = texture;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
    
            ConfigureRects();
        }
    }
}
