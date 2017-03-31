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
using System.Windows.Shapes;

namespace SadConsoleEditor.Windows
{
    /// <summary>
    /// Interaction logic for FileWindow.xaml
    /// </summary>
    public partial class NewDocumentWindow : Window
    {
        public NewDocumentWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Button button = (Button)sender;

            //if (button.Command != null && button.Command.CanExecute(button.CommandParameter))
            //    button.Command.Execute(button.CommandParameter);

            Hide();

        }
    }
}
