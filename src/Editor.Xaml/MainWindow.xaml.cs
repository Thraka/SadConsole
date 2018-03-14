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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {
        public MainWindow()
        {
            InitializeComponent();
        }


#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\MainWindow.xaml");
        }
#endif
    }
}
