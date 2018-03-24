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

namespace Editor.Xaml.Converters
{
    class ColorToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (SolidColorBrush)value;
            return brush.Color;
        }
    }

    class ColorToRByte : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return color.R;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (byte)value;
            return new Color() { R = brush, A = 255 };
        }
    }

    class ColorToGByte : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return color.G;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (byte)value;
            return new Color() { G = brush, A = 255 };
        }
    }

    class ColorToBByte : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return color.B;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = (byte)value;
            return new Color() { B = brush, A = 255 };
        }
    }

    class ByteToColorR : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = System.Convert.ToByte(value);
            return new Color() { R = brush, A = 255 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return System.Convert.ToUInt16(color.R);
        }
    }

    class ByteToColorG : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = System.Convert.ToByte(value);
            return new Color() { G = brush, A = 255 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return System.Convert.ToUInt16(color.G);
        }
    }

    class ByteToColorB : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = System.Convert.ToByte(value);
            return new Color() { B = brush, A = 255 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return System.Convert.ToUInt16(color.B);
        }
    }
}
