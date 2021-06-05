using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace FeatureDemo
{
    internal class Program
    {
        private static Windows.CharacterViewer _characterWindow;
        private static Container MainConsole;

        public static int MainWidth = 80;
        public static int MainHeight = 23;
        public static int HeaderWidth = 80;
        public static int HeaderHeight = 2;

        private static void Main(string[] args)
        {
            //SadConsole.Settings.UnlimitedFPS = true;
            //SadConsole.Settings.UseDefaultExtendedFont = true;
            //SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.Stretch;

#if MONOGAME
            Settings.WindowTitle = "Feature Demo (MonoGame)";
#elif SFML
            Settings.WindowTitle = "Feature Demo (SFML)";
#endif

            SadConsole.Game.Create(80, 25); //, "Res/Fonts/C64.font");
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.FrameUpdate += Instance_FrameUpdate;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Instance_FrameUpdate(object sender, GameHost e)
        {
            // Called each logic update.
            //if (!_characterWindow.IsVisible)
            {
                // This block of code cycles through the consoles in the SadConsole.Engine.ConsoleRenderStack, showing only a single console
                // at a time. This code is provided to support the custom consoles demo. If you want to enable the demo, uncomment one of the lines
                // in the Initialize method above.
                if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F1))
                {
                    MainConsole.MoveNextConsole();
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F2))
                {
                    _characterWindow.Show(true);
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F9))
                {
                    SadConsole.Debug.Screen.Show();
//#if MONOGAME
//                    if (!SadConsole.Debug.MonoGame.Debugger.IsOpened)
//                        SadConsole.Debug.MonoGame.Debugger.Start();
//                    else
//                        SadConsole.Debug.MonoGame.Debugger.Stop();
//#endif
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F5))
                {
                    SadConsole.Game.Instance.ToggleFullScreen();

                }

            }
        }

        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
            //if (Settings.UnlimitedFPS)
            //    SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance));

            // Register the types provided by the SadConsole.Extended library
            SadConsole.UI.RegistrarExtended.Register();

            // Splash screens show up at the start of the game.
            //SadConsole.Game.Instance.SetSplashScreens(new SadConsole.SplashScreens.PCBoot());

            // The demo screen 
            MainConsole = new Container();

            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove and then destroy it.
            Game.Instance.Screen = MainConsole;
            Game.Instance.DestroyDefaultStartingConsole();

            // Initialize the windows used by the global keyboard handler in Instance_FrameUpdate
            _characterWindow = new Windows.CharacterViewer(0);
        }

    }
}
