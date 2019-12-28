using SadConsole;

namespace SadConsoleEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            //SadConsole.Settings.UnlimitedFPS = true;
            //SadConsole.Settings.UseHardwareFullScreen = true;
            SadConsole.Settings.UseDefaultExtendedFont = true;

            // Load our program settings
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ProgramSettings));
            using (var fileObject = System.IO.File.OpenRead("Settings.json"))
                Config.Program = serializer.ReadObject(fileObject) as ProgramSettings;

            // Setup the engine and creat the main window.
            SadConsole.Game.Create(Config.Program.WindowWidth, Config.Program.WindowHeight);
            //SadConsole.Engine.Initialize("IBM.font", 80, 25, (g) => { g.GraphicsDeviceManager.HardwareModeSwitch = false; g.Window.AllowUserResizing = true; });

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.Instance.OnStart = Init;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game has shut down.
            //

            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            Config.Program.ScreenFont = Global.DefaultFont;
            Config.Program.ScreenFontSize = Global.DefaultFont.GetFontSize(Font.Sizes.One);

            // Any setup
            //            if (Settings.UnlimitedFPS)
            //                SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance));

            Settings.WindowTitle = "SadConsole Editor - v" + System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString();
            var windowTheme = (SadConsole.UI.Themes.Window)SadConsole.UI.Themes.Library.Default.GetConsoleTheme(typeof(SadConsole.UI.Window));
            windowTheme.BorderLineStyle = ICellSurface.ConnectedLineThick;
            SadConsole.UI.Themes.Library.Default.SetConsoleTheme(typeof(SadConsole.UI.Window), windowTheme);

            //Global.MouseState.ProcessMouseWhenOffScreen = true;

            // We'll instead use our demo consoles that show various features of SadConsole.
            Global.Screen = new MainConsole();
            
            MainConsole.Instance.ShowStartup();
        }
    }
}
