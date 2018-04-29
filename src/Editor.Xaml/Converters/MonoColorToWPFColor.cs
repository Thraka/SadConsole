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
            return ((MonoColor)value).ToWpfColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Color)value).ToMonoColor();
        }
    }

    public static class Extensions
    {
        public static MonoColor ToMonoColor(this Color color)
        {
            return new MonoColor(color.R, color.G, color.B, color.A);
        }

        public static Color ToWpfColor(this MonoColor color)
        {
            return new Color() { R = color.R, G = color.G, B = color.B, A = color.A };
        }
    }
}
