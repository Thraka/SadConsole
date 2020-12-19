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

            SadConsole.Game.Create(122, 42); //, "Res/Fonts/C64.font");
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
#if MONOGAME
            // This isn't a mobile/xbox game, don't need the title container for serialization.
            SadConsole.Game.Instance.UseTitleContainer = false;
#endif

            // Any setup
            //if (Settings.UnlimitedFPS)
            //    SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance));

            //SadConsole.Game.Instance.Window.Title = "DemoProject Core";

            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove it.

            // Splash screens show up at the start of the game.
            //SadConsole.Game.Instance.SetSplashScreens(new SadConsole.SplashScreens.PCBoot());

            // Register the extended library controls with the default library
            SadConsole.UI.RegistrarExtended.Register();

            GameHost.Instance.Screen = new Container();

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
