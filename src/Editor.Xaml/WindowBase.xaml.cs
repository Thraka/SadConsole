#if NOESIS
using Noesis;
using System.Collections.Generic;
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
        public static List<WindowBase> Windows = new List<WindowBase>();

        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\WindowBase.xaml");
        }

        public static void Show(FrameworkElement content)
        {
            var window = new WindowBase();
            
        }

        public void Hide()
        {

        }
#endif
    }
}
