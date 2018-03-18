#if NOESIS
using Noesis;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#endif

using System.Windows.Input;

namespace Editor.Xaml
{
    public class ColorChangedEventArgs : EventArgs
    {
        public Color OldValue { get; internal set; }
        public Color NewValue { get; internal set; }
    }

    public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs e);

    /// <summary>
    /// Interaction logic for ColorView.xaml
    /// </summary>
    public partial class ColorView : UserControl
    {
        public ColorView()
        {
            InitializeComponent();
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #region DepProperty SelectedColor
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            "SelectedColor", typeof(Color), typeof(ColorView),
            new PropertyMetadata(Colors.Green, StaticOnSelectedColorChanged));

        private static void StaticOnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorView control = (ColorView)d;

            Color newValue = (Color)e.NewValue;
            Color oldValue = (Color)e.OldValue;
            if (newValue != oldValue)
            {
                ColorChangedEventArgs args = new ColorChangedEventArgs
                {
                    OldValue = oldValue,
                    NewValue = newValue
                };
                control.OnSelectedColorChanged(args);
            }
        }
        #endregion

        #region DepProperty Text
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ColorView), new PropertyMetadata(""));
        #endregion




        public ICommand ColorClickCommand
        {
            get { return (ICommand)GetValue(ColorClickCommandProperty); }
            set { SetValue(ColorClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorClickCommandProperty =
            DependencyProperty.Register("ColorClickCommand", typeof(ICommand), typeof(ColorView), new PropertyMetadata(null));
        

#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\ColorView.xaml");
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "MouseLeftButtonUp" && handlerName == "ColorBox_MouseLeftButtonUp")
            {
                ((Border)source).MouseLeftButtonUp += ColorBox_MouseLeftButtonUp;
                return true;
            }
            return false;
        }

#endif

        public event ColorChangedEventHandler SelectedColorChanged;

        protected virtual void OnSelectedColorChanged(ColorChangedEventArgs e)
        {
            SelectedColorChanged?.Invoke(this, e);
        }

        private void ColorBox_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (ColorClickCommand != null)
            {
                if (ColorClickCommand.CanExecute(null))
                {
                    ColorClickCommand.Execute(this);
                }
            }
        }
    }
}
