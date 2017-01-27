using System;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace StarterProject
{
    class Program
    {
        private static Windows.CharacterViewer _characterWindow;
        private static int currentConsoleIndex;

        static void Main(string[] args)
        {
            SadConsole.Settings.UnlimitedFPS = true;

            // Setup the engine and creat the main window.
            SadConsole.Game.Create("IBM.font", 80, 25);
            //SadConsole.Engine.Initialize("IBM.font", 80, 25, (g) => { g.GraphicsDeviceManager.HardwareModeSwitch = false; g.Window.AllowUserResizing = true; });

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Engine_EngineStart;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Engine_EngineUpdated;

            // Hook the "after render" even though we're not using it.
            SadConsole.Game.OnDraw = Engine_EngineDrawFrame;
            
            // Start the game.
            SadConsole.Game.Instance.Run();
        }

        private static void Engine_EngineDrawFrame(GameTime time)
        {
            // Custom drawing. You don't usually have to do this.
        }

        private static void Engine_EngineUpdated(GameTime time)
        {
            if (!_characterWindow.IsVisible)
            {
                // This block of code cycles through the consoles in the SadConsole.Engine.ConsoleRenderStack, showing only a single console
                // at a time. This code is provided to support the custom consoles demo. If you want to enable the demo, uncomment one of the lines
                // in the Initialize method above.
                if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F1))
                {
                    MoveNextConsole();
                }
                else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F2))
                {
                    _characterWindow.Show(true);
                }
                else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F3))
                {
                }
                else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
                {
                    SadConsole.Settings.ToggleFullScreen();
                }
            }
        }

        private static void Engine_EngineStart()
        {
            // Setup our custom theme.
            Theme.SetupThemes();
            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove it.

            //Global.MouseState.ProcessMouseWhenOffScreen = true;

            // We'll instead use our demo consoles that show various features of SadConsole.
            Global.ActiveScreen.Children.Add(new CustomConsoles.AutoTypingConsole());
                //= new ConsoleList() {
                //    //new CustomConsoles.CachedConsoleConsole(),
                //                        //new CustomConsoles.RandomScrollingConsole(),
                //                        new CustomConsoles.AutoTypingConsole(),
                //                        //new CustomConsoles.SplashScreen() { SplashCompleted = () => { MoveNextConsole(); } },
                //                        new CustomConsoles.StretchedConsole(),
                //                        new CustomConsoles.StringParsingConsole(),
                //                        new CustomConsoles.ControlsTest(),
                //                        //new CustomConsoles.FadingExample(),
                //                        new CustomConsoles.DOSConsole(),
                //                        new CustomConsoles.AnsiConsole(),
                //                        new CustomConsoles.ViewsAndSubViews(),
                //                        new CustomConsoles.StaticConsole(),
                //                        new CustomConsoles.SceneProjectionConsole(),
                //                        new CustomConsoles.BorderedConsole(),
                //                        new CustomConsoles.GameObjectConsole(),
                //                        new CustomConsoles.WorldGenerationConsole(),
                //                    };

            // Show the first console (by default all of our demo consoles are hidden)
            Global.ActiveScreen.Children[0].IsVisible = true;

            // Set the first console in the console list as the "active" console. This allows the keyboard to be processed on the console.
            Console.ActiveConsole = (IConsole)Global.ActiveScreen.Children[0];

            // Initialize the windows
            _characterWindow = new Windows.CharacterViewer();

            //SadConsole.Effects.Fade a = new SadConsole.Effects.Fade();
            //a.DestinationForeground = Microsoft.Xna.Framework.Color.Turquoise;
            //SadConsole.Engine.MonoGameInstance.Components.Add(new FPSCounterComponent(SadConsole.Engine.MonoGameInstance));
            
        }

        private static void MoveNextConsole()
        {
            currentConsoleIndex++;

            if (currentConsoleIndex >= Global.ActiveScreen.Children.Count)
                currentConsoleIndex = 0;

            for (int i = 0; i < Global.ActiveScreen.Children.Count; i++)
                Global.ActiveScreen.Children[i].IsVisible = currentConsoleIndex == i;

            Console.ActiveConsole = (IConsole)Global.ActiveScreen.Children[currentConsoleIndex];
        }
    }
}
