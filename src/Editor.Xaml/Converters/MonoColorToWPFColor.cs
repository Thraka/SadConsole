#if NOESIS
using Noesis;
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

using MonoColor = Microsoft.Xna.Framework.Color;

namespace Editor.Xaml.Converters
{
    class MonoColorToWPFColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (MonoColor)value;
            return new Color() { A = color.A, B = color.B, G = color.G, R = color.R };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return new MonoColor(color.R, color.G, color.B, color.A);
        }
    }
}
