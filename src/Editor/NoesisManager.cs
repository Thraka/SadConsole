using NoesisGUI.MonoGameWrapper;
using NoesisGUI.MonoGameWrapper.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    static class NoesisManager
    {
        public static NoesisWrapper noesisGUIWrapper;

        public static void DestroyGUI()
        {
            if (noesisGUIWrapper != null)
            {
                noesisGUIWrapper.Dispose();
                noesisGUIWrapper = null;
            }
        }

        public static void CreateNoesisGUI()
        {
            var rootPath = Environment.CurrentDirectory;//Path.Combine(Environment.CurrentDirectory, "Data");
            var providerManager = new NoesisProviderManager(
                new FolderXamlProvider(rootPath),
                new FolderFontProvider(rootPath),
                new FolderTextureProvider(rootPath, SadConsole.Global.GraphicsDevice));

            var config = new NoesisConfig(
                SadConsole.Game.Instance.Window,
                SadConsole.Global.GraphicsDeviceManager,
                providerManager,
                rootXamlFilePath: "Views/Root.xaml",
                // uncomment this line to use theme file
                themeXamlFilePath: "Themes/NocturnalStyle.xaml",
                currentTotalGameTime: TimeSpan.Zero);

            config.SetupInputFromWindows();

            noesisGUIWrapper = new NoesisWrapper(config);
            noesisGUIWrapper.ControlTreeRoot.DataContext = Program.MainViewModel;
        }
    }
}
