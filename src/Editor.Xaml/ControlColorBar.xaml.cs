#if NOESIS
using Noesis;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
#endif
using System.Linq;
using System.Collections.Generic;

namespace Editor.Xaml
{
    /// <summary>
    /// Interaction logic for ControlColorBar.xaml
    /// </summary>
    public partial class ControlColorBar : UserControl
    {
        private bool internalSet = false;

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public GradientBrush Brush
        {
            get { return (GradientBrush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

#if NOESIS
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(GradientBrush), typeof(ControlColorBar), new PropertyMetadata(null, StaticOnBrushChanged));
#else
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(GradientBrush), typeof(ControlColorBar), new PropertyMetadata(new LinearGradientBrush(Colors.White, Colors.Black, 0.0d), StaticOnBrushChanged));
#endif


        public static readonly DependencyProperty SelectedColorProperty =
           DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ControlColorBar), new PropertyMetadata(Colors.White, StaticOnSelectedColorChanged));

        public ControlColorBar()
        {
            InitializeComponent();
        }

        private void Grid_MouseEvent(object sender, MouseEventArgs e)
        {
            var grid = (Grid)this.FindName("ColorGrid");
            var box = ((Viewbox)this.FindName("SelectorViewBox"));
            var mousePos = e.GetPosition(grid).X;

            if (e.LeftButton == MouseButtonState.Pressed && mousePos >= 0 && mousePos < grid.ActualWidth)
            {
                box.SetValue(Canvas.LeftProperty, mousePos);
                //internalSet = true;
                SelectedColor = Brush.GradientStops.GetRelativeColor(mousePos / grid.ActualWidth);
                internalSet = false;
            }
        }

        private void BorderStart_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var box = ((Viewbox)this.FindName("SelectorViewBox"));
                var grid = (Grid)this.FindName("ColorGrid");
                var border = (Border)sender;
                box.SetValue(Canvas.LeftProperty, 0f - (border.ActualWidth / 2));
                //internalSet = true;
                SelectedColor = Brush.GradientStops.GetRelativeColor(0);
                internalSet = false;
            }
        }

        private void BorderEnd_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var box = ((Viewbox)this.FindName("SelectorViewBox"));
                var grid = (Grid)this.FindName("ColorGrid");
                var border = (Border)sender;
                box.SetValue(Canvas.LeftProperty, grid.ActualWidth + (border.ActualWidth / 2));
                //internalSet = true;
                SelectedColor = Brush.GradientStops.GetRelativeColor(1);
                internalSet = false;
            }
        }

#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\ControlColorBar.xaml");
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (handlerName == "Grid_MouseEvent")
            {
                if (eventName == "MouseMove")
                {
                    ((Grid)source).MouseMove += Grid_MouseEvent;
                    return true;
                }
                if (eventName == "MouseLeftButtonDown")
                {
                    ((Grid)source).MouseLeftButtonDown += Grid_MouseEvent;
                    return true;
                }
            }
            if (handlerName == "BorderStart_MouseMove")
            {
                if (eventName == "MouseMove")
                {
                    ((Border)source).MouseMove += BorderStart_MouseMove;
                    return true;
                }
                if (eventName == "MouseLeftButtonDown")
                {
                    ((Border)source).MouseLeftButtonDown += BorderStart_MouseMove;
                    return true;
                }
            }
            if (handlerName == "BorderEnd_MouseMove")
            {
                if (eventName == "MouseMove")
                {
                    ((Border)source).MouseMove += BorderEnd_MouseMove;
                    return true;
                }
                if (eventName == "MouseLeftButtonDown")
                {
                    ((Border)source).MouseLeftButtonDown += BorderEnd_MouseMove;
                    return true;
                }
            }
            return false;
        }
#endif

        private static void StaticOnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ControlColorBar control = (ControlColorBar)d;

            GradientBrush newValue = (GradientBrush)e.NewValue;
            GradientBrush oldValue = (GradientBrush)e.OldValue;
            if (newValue != oldValue)
            {
                var borderStart = (Border)control.FindName("LowColorFill");
                var borderEnd = (Border)control.FindName("HighColorFill");
                borderStart.Background = new SolidColorBrush(newValue.GradientStops.GetRelativeColor(0));
                borderEnd.Background = new SolidColorBrush(newValue.GradientStops.GetRelativeColor(1));
            }
        }

        private static void StaticOnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ControlColorBar control = (ControlColorBar)d;

            Color newValue = (Color)e.NewValue;
            Color oldValue = (Color)e.OldValue;
            if (newValue != oldValue && !control.internalSet)
            {
                control.Background = new SolidColorBrush(newValue);
            }
        }

    }

    public static class Extensions
    {
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

            color.ScA = (float)((offset - before.Offset) * (after.Color.ScA - before.Color.ScA) / (after.Offset - before.Offset) + before.Color.ScA);
            color.ScR = (float)((offset - before.Offset) * (after.Color.ScR - before.Color.ScR) / (after.Offset - before.Offset) + before.Color.ScR);
            color.ScG = (float)((offset - before.Offset) * (after.Color.ScG - before.Color.ScG) / (after.Offset - before.Offset) + before.Color.ScG);
            color.ScB = (float)((offset - before.Offset) * (after.Color.ScB - before.Color.ScB) / (after.Offset - before.Offset) + before.Color.ScB);

            return color;
        }
    }
}
