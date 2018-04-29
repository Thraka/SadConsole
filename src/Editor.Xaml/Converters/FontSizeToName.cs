#if NOESIS
using Noesis;
using System.Linq;
using System;
using System.Globalization;
#else
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
#endif

namespace Editor.Xaml.Converters
{
    class FontSizeToFontName : IValueConverter
    {
        private static string[] FontSizeNames = new[] { "Quarter", "Half", "1x", "2x", "3x", "4x" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fontSize = (SadConsole.Font.FontSizes)value;
            return FontSizeNames[(int)fontSize];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SadConsole.Font.FontSizes)FontSizeNames.ToList().IndexOf((string)value);
        }
    }
}
