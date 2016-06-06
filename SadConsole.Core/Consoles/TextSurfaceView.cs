using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Consoles
{
    public class TextSurfaceView : TextSurface
    {
        private ITextSurface data;
        protected Rectangle originalArea;
        
        public TextSurfaceView(ITextSurface surface, Rectangle area): base(area.Width, area.Height, surface.Font)
        {
            data = surface;
            DefaultBackground = surface.DefaultBackground;
            DefaultForeground = surface.DefaultForeground;
            base.font = surface.Font;
            base.width = surface.Width;
            base.height = surface.Height;

            this.originalArea = area;
            this.cells = data.Cells;

            base.width = area.Width;
            base.height = area.Height;

            RenderRects = new Rectangle[area.Width * area.Height];
            RenderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = new Rectangle(x * Font.Size.X, y * Font.Size.Y, Font.Size.X, Font.Size.Y);
                    RenderCells[index] = base.cells[(y + area.Top) * width + (x + area.Left)];
                    index++;
                }
            }

            cells = RenderCells;

            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);

        }
        
        public new void Save(string file)
        {
            TextSurfaceViewSerialized.Save(this, file);
        }

        public static TextSurfaceView Load(string file, ITextSurface surfaceBase)
        {
            return TextSurfaceViewSerialized.Load(file, surfaceBase);
        }

        [DataContract]
        public class TextSurfaceViewSerialized
        {
            [DataMember]
            Rectangle Area;

            [DataMember]
            string FontName;

            [DataMember]
            int FontMultiple;

            [DataMember]
            Color Tint;

            public static void Save(TextSurfaceView surfaceBase, string file)
            {
                TextSurfaceViewSerialized data = new TextSurfaceViewSerialized();
                data.Area = surfaceBase.originalArea;
                data.FontName = surfaceBase.font.Name;
                data.FontMultiple = surfaceBase.font.SizeMultiple;
                data.Tint = surfaceBase.Tint;

                SadConsole.Serializer.Save(data, file);
            }

            public static TextSurfaceView Load(string file, ITextSurface parent)
            {
                TextSurfaceViewSerialized data = Serializer.Load<TextSurfaceViewSerialized>(file);
                TextSurfaceView newSurface = new TextSurfaceView(parent, data.Area);
                
                // Try to find font
                if (Engine.Fonts.ContainsKey(data.FontName))
                    newSurface.font = Engine.Fonts[data.FontName].GetFont(data.FontMultiple);
                else
                    newSurface.font = Engine.DefaultFont;

                newSurface.Tint = data.Tint;

                return newSurface;
            }
        }
    }
}
