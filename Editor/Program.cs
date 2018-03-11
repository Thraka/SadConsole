using System;
using System.Windows.Forms;
using MugenMvvmToolkit;
using MugenMvvmToolkit.WinForms.Infrastructure;

namespace SadConsole.Editor
{
    internal static class Program
    {
        public static string[] FontSizeNames = new[] { "Quarter", "Half", "1x", "2x", "3x", "4x" };


        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var bootstrapper = new Bootstrapper<ViewModels.MainViewModel>(new MugenContainer());
            bootstrapper.Start();
        }
    }
}