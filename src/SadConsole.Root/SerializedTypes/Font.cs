using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class Font
    {
        [DataMember]
        public string Name;

        [DataMember]
        public SadConsole.Font.FontSizes Size;

        public static Font FromFramework(SadConsole.Font font)
        {
            return new Font() { Name = font.Name, Size = font.SizeMultiple };
        }

        public static SadConsole.Font ToFramework(Font font)
        {
            SadConsole.Font newFont;

            // Try to find font
            if (Global.Fonts.ContainsKey(font.Name))
                newFont = Global.Fonts[font.Name].GetFont(font.Size);
            else
                newFont = Global.FontDefault;

            return newFont;
        }
    }
}
