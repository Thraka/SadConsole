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
    /// Interaction logic for NewDocument.xaml
    /// </summary>
    public partial class WindowNewDocument : UserControl
    {
        public WindowNewDocument()
        {
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\WindowNewDocument.xaml");
        }
#endif
    }
}
