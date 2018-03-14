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
    /// Interaction logic for Root.xaml
    /// </summary>
    public partial class Root : Grid
    {
        public Root()
        {
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\Root.xaml");
        }
#endif
    }
}
