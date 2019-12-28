#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace SadConsoleEditor
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Load our program settings
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ProgramSettings));
            using (var fileObject = System.IO.File.OpenRead("Settings.json"))
                Settings.Config = serializer.ReadObject(fileObject) as ProgramSettings;


            // Setup the engine and creat the main window.
            SadConsole.Game.Create(Settings.Config.ProgramFontFile, Settings.Config.WindowWidth, Settings.Config.WindowHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            SadConsole.Game.OnUpdate = (t) => { };

            // Start the game.
            SadConsole.Game.Instance.Run();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Init()
        {
            SadConsole.Game.Instance.Window.Title = "SadConsole Editor - v" + System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString();

            
            // Load screen font
            var font = SadConsole.Global.LoadFont(Settings.Config.ScreenFontFile);
            Settings.Config.ScreenFont = font.GetFont(SadConsole.Font.FontSizes.One);

            // Setup GUI themes
            Settings.SetupThemes();

            // Helper editor for any text surface
            SadConsoleEditor.Settings.QuickEditor = new SadConsole.Surfaces.SurfaceEditor(new SadConsole.Surfaces.SadConsole.Surfaces.Basic(10, 10, SadConsole.Global.FontDefault));

            // Setup system to run
            SadConsole.Global.CurrentScreen.Children.Add(new MainScreen());

            MainScreen.Instance.ShowStartup();
        }
    }
#endif
}
