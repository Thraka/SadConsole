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
    public class WindowSettings
    {
        public string Title { get; set; } = "Crazy title";
    }


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

        public static void Show(FrameworkElement content, WindowSettings settings)
        {
            var window = new WindowBase();
            window.DataContext = settings;
            ContentPresenter contentObject = (ContentPresenter)window.FindName("Content");
            contentObject.Content = content;

            Globals.RootFrameworkElement.Children.Add(window);
        }
        
        public void Hide()
        {
            Globals.RootFrameworkElement.Children.Remove(this);
        }

        public void BringToFront()
        {

        }

        public void SendToBack()
        {

        }
#endif
    }
}
