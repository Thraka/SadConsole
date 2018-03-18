#if NOESIS
using Noesis;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace Editor.Xaml
{
    /// <summary>
    /// Interaction logic for ControlColorBar.xaml
    /// </summary>
    public partial class ControlColorBar : UserControl
    {


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

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(GradientBrush), typeof(ControlColorBar), new PropertyMetadata(new LinearGradientBrush(Colors.White, Colors.Black, 0.0d)));

        public static readonly DependencyProperty SelectedColorProperty =
           DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ControlColorBar), new PropertyMetadata(Colors.White));

        public ControlColorBar()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\ControlColorBar.xaml");
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "MouseLeftButtonDown" && handlerName == "Grid_MouseLeftButtonDown")
            {
                ((Grid)source).MouseLeftButtonDown += Grid_MouseLeftButtonDown;
                return true;
            }
            return false;
        }
#endif
    }
}
