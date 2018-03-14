using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Converters
{
    class FontSizeToName
    {
        private static string[] FontSizeNames = new[] { "Quarter", "Half", "1x", "2x", "3x", "4x" };

        public string Convert(SadConsole.Font.FontSizes value)
        {
            return FontSizeNames[(int)value];
        }

        public SadConsole.Font.FontSizes ConvertBack(string value)
        {
            return (SadConsole.Font.FontSizes)FontSizeNames.ToList().IndexOf(value);
        }
    }
}
