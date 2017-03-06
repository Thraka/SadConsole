using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SadConsoleEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Windows.NewDocumentWindow newDocumentWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newDocumentWindow = new Windows.NewDocumentWindow();
        }

        private void MenuNewDocument_Click(object sender, RoutedEventArgs e)
        {
            newDocumentWindow.DataContext = new ViewModel.NewDocumentViewModel();
            newDocumentWindow.ShowDialog();
        }
    }
}
