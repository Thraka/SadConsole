#if NOESIS
using Noesis;
using ReturnType = Noesis.Visual;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using ReturnType = System.Windows.DependencyObject;
#endif
using System.Linq;
using System.Collections.Generic;

namespace Editor.Xaml
{
    public static class Extensions
    {
        public static T FindParent<T>(this ReturnType child) where T : ReturnType
        {
            //get parent item
            ReturnType parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        public static Color GetRelativeColor(this GradientStopCollection gsc, double offset)
        {
            var collection = new List<GradientStop>();
            foreach (var colorStop in gsc)
            {
                collection.Add((GradientStop)colorStop);
            }

            GradientStop before = collection.Where(w => w.Offset == collection.Min(m => m.Offset)).First();
            GradientStop after = collection.Where(w => w.Offset == collection.Max(m => m.Offset)).First();

            foreach (var gs in collection)
            {
                if (gs.Offset < offset && gs.Offset > before.Offset)
                {
                    before = gs;
                }
                if (gs.Offset > offset && gs.Offset < after.Offset)
                {
                    after = gs;
                }
            }

            var color = new Color();

            color.A = (byte)((offset - before.Offset) * (after.Color.A - before.Color.A) / (after.Offset - before.Offset) + before.Color.A);
            color.R = (byte)((offset - before.Offset) * (after.Color.R - before.Color.R) / (after.Offset - before.Offset) + before.Color.R);
            color.G = (byte)((offset - before.Offset) * (after.Color.G - before.Color.G) / (after.Offset - before.Offset) + before.Color.G);
            color.B = (byte)((offset - before.Offset) * (after.Color.B - before.Color.B) / (after.Offset - before.Offset) + before.Color.B);

            return color;
        }
    }
}
