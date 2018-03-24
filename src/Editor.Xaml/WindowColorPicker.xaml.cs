#if NOESIS
using System.ComponentModel;
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
    /// Interaction logic for WindowColorPicker.xaml
    /// </summary>
    public partial class WindowColorPicker : UserControl
    {
        public WindowColorPicker()
        {
            InitializeComponent();
        }



        public BindableColor SelectedColor
        {
            get { return (BindableColor)GetValue(bindableColorProperty); }
            set { SetValue(bindableColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for bindableColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bindableColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(BindableColor), typeof(WindowColorPicker), new PropertyMetadata(new BindableColor()));



#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\WindowColorPicker.xaml");
        }
#endif
    }


#if NOESIS
    public class BindableColor : ViewModels.ViewModelBase
    {
#else
    public class BindableColor
    {
        void OnPropertyChanged() { }
#endif
        Color _color;
        public Color Color
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged(); }
        }

        //public static readonly DependencyProperty ColorProperty =
        //    DependencyProperty.Register("Color", typeof(Color), typeof(BindableColor), new PropertyMetadata(Colors.ForestGreen));

        public byte R
        {
            get { return Color.R; }
            set { Color = new Color() { R = value, G = Color.G, B = Color.B, A = Color.A }; OnPropertyChanged(); }
        }

        public byte G
        {
            get { return Color.G; }
            set { Color = new Color() { R = Color.R, G = value, B = Color.B, A = Color.A }; OnPropertyChanged(); }
        }

        public byte B
        {
            get { return Color.B; }
            set { Color = new Color() { R = Color.R, G = Color.G, B = value, A = Color.A }; OnPropertyChanged(); }
        }

        //public byte R
        //{
        //    get { return (byte)GetValue(RProperty); }
        //    set { SetValue(RProperty, value); }
        //}

        //public static readonly DependencyProperty RProperty =
        //    DependencyProperty.Register("R", typeof(byte), typeof(BindableColor), new PropertyMetadata(128, StaticOnRChanged));



        //public byte G
        //{
        //    get { return (byte)GetValue(GProperty); }
        //    set { SetValue(GProperty, value); }
        //}

        //public static readonly DependencyProperty GProperty =
        //    DependencyProperty.Register("G", typeof(byte), typeof(BindableColor), new PropertyMetadata((byte)128, StaticOnGChanged));

        //public byte B
        //{
        //    get { return (byte)GetValue(BProperty); }
        //    set { SetValue(BProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for B.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty BProperty =
        //    DependencyProperty.Register("B", typeof(byte), typeof(BindableColor), new PropertyMetadata(128, StaticOnBChanged));






        //private static void StaticOnRChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    BindableColor control = (BindableColor)d;

        //    byte oldValue = (byte)e.OldValue;
        //    byte newValue = (byte)e.NewValue;

        //    if (oldValue != newValue)
        //        control.Color = new Color() { R = newValue, G = control.G, B = control.B, A = 255 };
        //}

        //private static void StaticOnGChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    BindableColor control = (BindableColor)d;

        //    byte oldValue = (byte)e.OldValue;
        //    byte newValue = (byte)e.NewValue;

        //    if (oldValue != newValue)
        //        control.Color = new Color() { R = control.Color.R, G = newValue, B = control.Color.B, A = 255 };
        //}

        //private static void StaticOnBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    BindableColor control = (BindableColor)d;

        //    byte oldValue = (byte)e.OldValue;
        //    byte newValue = (byte)e.NewValue;

        //    if (oldValue != newValue)
        //        control.Color = new Color() { R = control.R, G = control.G, B = newValue, A = 255 };
        //}

        //private static void StaticOnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    BindableColor control = (BindableColor)d;

        //    Color oldColor = (Color)e.OldValue;
        //    Color newColor = (Color)e.OldValue;

        //    if (oldColor != newColor)
        //    {
        //        if (oldColor.R != newColor.R)
        //        {
        //            control.R = newColor.R;
        //        }

        //        if (oldColor.B != newColor.B)
        //        {
        //            control.B = newColor.B;
        //        }

        //        if (oldColor.G != newColor.G)
        //        {
        //            control.G = newColor.G;
        //        }
        //    }
        //}
    }
}
