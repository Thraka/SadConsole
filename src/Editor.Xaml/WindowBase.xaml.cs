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
    /// Interaction logic for WindowBase.xaml
    /// </summary>
    public partial class WindowBase : UserControl
    {
        public WindowBase()
        {
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\WindowBase.xaml");
        }
#endif
    }
}
