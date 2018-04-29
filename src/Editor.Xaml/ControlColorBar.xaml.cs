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
        private void ThisControl_Loaded(object sender, RoutedEventArgs e)
        {
            var control = this;

            var box = ((Viewbox)control.FindName("SelectorViewBox"));
            var grid = (Grid)control.FindName("ColorGrid");
            var borderLow = (Border)control.FindName("LowColorFill");
            var borderHigh = (Border)control.FindName("HighColorFill");

            // Red channel
            if (control.SelectedColor.R != 0)
            {
                if (control.SelectedColor.R == 255)
                {
                    box.SetValue(Canvas.LeftProperty, grid.ActualWidth + (borderHigh.ActualWidth / 2));
                }
                else
                {
                    box.SetValue(Canvas.LeftProperty, (control.SelectedColor.R / 255f) * grid.ActualWidth);
                }
            }
            // Green channel
            else if (control.SelectedColor.G != 0)
            {
                if (control.SelectedColor.G == 255)
                {
                    box.SetValue(Canvas.LeftProperty, grid.ActualWidth + (borderHigh.ActualWidth / 2));
                }
                else
                {
                    box.SetValue(Canvas.LeftProperty, (control.SelectedColor.G / 255f) * grid.ActualWidth);
                }
            }
            // Blue
            else if (control.SelectedColor.B != 0)
            {
                if (control.SelectedColor.B == 255)
                {
                    box.SetValue(Canvas.LeftProperty, grid.ActualWidth + (borderHigh.ActualWidth / 2));
                }
                else
                {
                    box.SetValue(Canvas.LeftProperty, (control.SelectedColor.B / 255f) * grid.ActualWidth);
                }
            }
            // 0 value
            else
            {
                box.SetValue(Canvas.LeftProperty, 0f - (borderLow.ActualWidth / 2));
            }

        }

        private void Grid_MouseEvent(object sender, MouseEventArgs e)
        {
            var grid = (Grid)this.FindName("ColorGrid");
            var box = ((Viewbox)this.FindName("SelectorViewBox"));
            var mousePos = e.GetPosition(grid).X;

            if (e.LeftButton == MouseButtonState.Pressed && mousePos >= 0 && mousePos < grid.ActualWidth)
            {

                box.SetValue(Canvas.LeftProperty, mousePos);
                internalSet = true;
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
                internalSet = true;
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
                internalSet = true;
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
            if (handlerName == "ColorGrid_LayoutUpdated")
            {
                
                return true;
            }
            if (handlerName == "ThisControl_Loaded")
            {
                ((Grid)source).Loaded += ThisControl_Loaded;
                return true;
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

            control.Background = new SolidColorBrush(newValue);

            if (newValue != oldValue && !control.internalSet)
            {
                var box = ((Viewbox)control.FindName("SelectorViewBox"));
                var grid = (Grid)control.FindName("ColorGrid");
                var borderLow = (Border)control.FindName("LowColorFill");
                var borderHigh = (Border)control.FindName("HighColorFill");

                // Red channel
                if (control.SelectedColor.R != 0)
                {
                    if (control.SelectedColor.R == 255)
                    {
                        box.SetValue(Canvas.LeftProperty, grid.ActualWidth + (borderHigh.ActualWidth / 2));
                    }
                    else
                    {
                        box.SetValue(Canvas.LeftProperty, (control.SelectedColor.R / 255f) * grid.ActualWidth);
                    }
                }
                // Green channel
                else if (control.SelectedColor.G != 0)
                {
                    if (control.SelectedColor.G == 255)
                    {
                        box.SetValue(Canvas.LeftProperty, grid.ActualWidth + (borderHigh.ActualWidth / 2));
                    }
                    else
                    {
                        box.SetValue(Canvas.LeftProperty, (control.SelectedColor.G / 255f) * grid.ActualWidth);
                    }
                }
                // Blue
                else if (control.SelectedColor.B != 0)
                {
                    if (control.SelectedColor.B == 255)
                    {
                        box.SetValue(Canvas.LeftProperty, grid.ActualWidth + (borderHigh.ActualWidth / 2));
                    }
                    else
                    {
                        box.SetValue(Canvas.LeftProperty, (control.SelectedColor.B / 255f) * grid.ActualWidth);
                    }
                }
                // 0 value
                else
                {
                    box.SetValue(Canvas.LeftProperty, 0f - (borderLow.ActualWidth / 2));
                }

                //internalSet = true;
                //SelectedColor = Brush.GradientStops.GetRelativeColor(1);
                //internalSet = false;

            }
        }


    }
}
