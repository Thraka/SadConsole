using System;
using System.Windows.Forms;
using MugenMvvmToolkit;
using MugenMvvmToolkit.WinForms.Infrastructure;

namespace SadConsole.Editor
{
    internal static class Program
    {
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