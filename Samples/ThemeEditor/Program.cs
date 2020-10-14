using System;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace ThemeEditor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SadConsole.Settings.WindowTitle = "SadConsole Theme Editor v9.0";
            SadConsole.Settings.UseDefaultExtendedFont = true;
            //SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.None;
            //SadConsole.Host.Settings.UseHardwareFullScreen = true;

            SadConsole.Game.Create(160, 50); //, "Res/Fonts/C64.font");
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }


        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
            // Any setup
            //if (Settings.UnlimitedFPS)
            //    SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance));

            //SadConsole.Game.Instance.Window.Title = "DemoProject Core";

            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove it.

            // Splash screens show up at the start of the game.
            //SadConsole.Game.Instance.SetSplashScreens(new SadConsole.SplashScreens.PCBoot());

            SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(Controls.ColorBar), new Controls.ColorBar.ThemeType());
            SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(Controls.ColorPicker), new Controls.ColorPicker.ThemeType());
            SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(Controls.HueBar), new Controls.HueBar.ThemeType());

            GameHost.Instance.Screen.Renderer = null;
            GameHost.Instance.Screen.Children.Add(new Container());

            //GameHost.Instance.Screen = con;

            // Initialize the windows
        }

        class MouseTest : SadConsole.Components.MouseConsoleComponent
        {
            public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
            {
                if (state.Mouse.IsOnScreen)
                {
                    host.Position = state.WorldCellPosition;
                    handled = true;
                }
                else
                    handled = false;
            }
        }
    }
}
