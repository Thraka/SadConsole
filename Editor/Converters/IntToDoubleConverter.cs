using MugenMvvmToolkit.Binding.Converters;
using MugenMvvmToolkit.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoColor = Microsoft.Xna.Framework.Color;
using DrawingColor = System.Drawing.Color;

namespace SadConsole.Editor.Converters
{
    class ColorToDrawingConverter : ValueConverterBase<MonoColor, DrawingColor>
    {
        protected override DrawingColor Convert(MonoColor value, Type targetType, object parameter, CultureInfo culture, IDataContext context)
        {
            return value.ToDrawingColor();
        }

        protected override MonoColor ConvertBack(DrawingColor value, Type targetType, object parameter, CultureInfo culture, IDataContext context)
        {
            return value.ToMonoGameColor();
        }
    }

    class DrawingColorToColorConverter : ValueConverterBase<DrawingColor, MonoColor>
    {
        protected override MonoColor Convert(DrawingColor value, Type targetType, object parameter, CultureInfo culture, IDataContext context)
        {
            return value.ToMonoGameColor();
        }

        protected override DrawingColor ConvertBack(MonoColor value, Type targetType, object parameter, CultureInfo culture, IDataContext context)
        {
            return value.ToDrawingColor();
        }
    }

    class DoubleToIntConverter : ValueConverterBase<double, int>
    {
        protected override int Convert(double value, Type targetType, object parameter, CultureInfo culture, IDataContext context)
        {
            return (int)value;
        }

        protected override double ConvertBack(int value, Type targetType, object parameter, CultureInfo culture, IDataContext context)
        {
            return (double)value;
        }
    }
}
