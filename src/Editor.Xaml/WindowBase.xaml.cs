#if NOESIS
using Noesis;
using System.Collections.Generic;
using System.Windows.Input;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif


namespace Editor.Xaml
{
    public class WindowSettings
    {
        public string Title { get; set; } = "Crazy title";

        public object ChildContentDataContext { get; set; }

        public ICommand CloseWindowCommand { get; set; }
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
            settings.CloseWindowCommand = new DelegateCommand(_ => window.Hide());


            window.DataContext = settings;
            
            ContentPresenter contentObject = (ContentPresenter)window.FindName("Content");
            contentObject.Content = content;

            if (settings.ChildContentDataContext != null)
                content.DataContext = settings.ChildContentDataContext;

            Globals.RootFrameworkElement.Children.Add(window);
        }
        
        public void Hide()
        {
            Globals.RootFrameworkElement.Children.Remove(this);
            this.DataContext = null;
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
