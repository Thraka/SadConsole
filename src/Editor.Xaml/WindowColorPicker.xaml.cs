#if NOESIS
using Noesis;
#else
using System;
using System.Windows;
using System.Windows.Controls;
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

#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\WindowColorPicker.xaml");
        }
#endif
    }
}
