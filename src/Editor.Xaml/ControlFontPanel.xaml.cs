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
    /// Interaction logic for Font.xaml
    /// </summary>
    public partial class ControlFontPanel : UserControl
    {
        public ControlFontPanel()
        {
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Views\\ControlFontPanel.xaml");
        }
#endif
    }
}
