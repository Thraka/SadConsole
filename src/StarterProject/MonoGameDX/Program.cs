using SadConsole.Consoles;
using System;
using SadConsole;

namespace StarterProject
{
    class Program
    {
        private static Windows.CharacterViewer _characterWindow;
        private static int currentConsoleIndex;

        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Engine.Initialize("IBM.font", 80, 25);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Engine.EngineStart += Engine_EngineStart;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Engine.EngineUpdated += Engine_EngineUpdated;

            SadConsole.Engine.EngineDrawFrame += Engine_EngineDrawFrame;
            
            // Start the game.
            SadConsole.Engine.Run();
        }

        private static void Engine_EngineDrawFrame(object sender, EventArgs e)
        {
            // Custom drawing. You don't usually have to do this.
        }

        private static void Engine_EngineUpdated(object sender, EventArgs e)
        {
            if (!_characterWindow.IsVisible)
            {
                // This block of code cycles through the consoles in the SadConsole.Engine.ConsoleRenderStack, showing only a single console
                // at a time. This code is provided to support the custom consoles demo. If you want to enable the demo, uncomment one of the lines
                // in the Initialize method above.
                if (SadConsole.Engine.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F1))
                {
                    MoveNextConsole();
                }
                else if (SadConsole.Engine.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F2))
                {
                    _characterWindow.Show(true);
                }
                else if (SadConsole.Engine.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F3))
                {
                }
            }
        }

        private static void Engine_EngineStart(object sender, EventArgs e)
        {
            // Setup our custom theme.
            Theme.SetupThemes();

            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove it.
            SadConsole.Engine.ConsoleRenderStack.Clear();
            SadConsole.Engine.ActiveConsole = null;
            
            // We'll instead use our demo consoles that show various features of SadConsole.
            SadConsole.Engine.ConsoleRenderStack
                = new ConsoleList() {
                                        //new CustomConsoles.SplashScreen() { SplashCompleted = () => { MoveNextConsole(); } },
                                        //new CustomConsoles.CachedConsoleConsole(),
                                        new CustomConsoles.StringParsingConsole(),
                                        //new CustomConsoles.CursorConsole(),
                                        //new CustomConsoles.DOSConsole(),
                                        //new CustomConsoles.SceneProjectionConsole(),
                                        new CustomConsoles.ControlsTest(),
                                        new CustomConsoles.ViewsAndSubViews(),
                                        new CustomConsoles.StaticConsole(),
                                        new CustomConsoles.StretchedConsole(),
                                        new CustomConsoles.BorderedConsole(),
                                        new CustomConsoles.GameObjectConsole(),
                                        new CustomConsoles.RandomScrollingConsole(),
                                        new CustomConsoles.WorldGenerationConsole(),
                                    };

            // Show the first console (by default all of our demo consoles are hidden)
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = true;

            // Set the first console in the console list as the "active" console. This allows the keyboard to be processed on the console.
            SadConsole.Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[0];

            // Initialize the windows
            _characterWindow = new Windows.CharacterViewer();

            //SadConsole.Effects.Fade a = new SadConsole.Effects.Fade();
            //a.DestinationForeground = Microsoft.Xna.Framework.Color.Turquoise;
            //SadConsole.Engine.MonoGameInstance.Components.Add(new FPSCounterComponent(SadConsole.Engine.MonoGameInstance));
            //SadConsole.Engine.MonoGameInstance.UnlockFPS();

            var testConosle = new SadConsole.Consoles.Console(10, 10);
            testConosle.Print(0, 0, "hello");
            //Serializer.Save(testConosle, "test.console");
            testConosle.Save("test.console", true);
            testConosle = SadConsole.Consoles.Console.Load("test.console");
        }

        

        private static void MoveNextConsole()
        {
            currentConsoleIndex++;

            if (currentConsoleIndex >= SadConsole.Engine.ConsoleRenderStack.Count)
                currentConsoleIndex = 0;

            for (int i = 0; i < SadConsole.Engine.ConsoleRenderStack.Count; i++)
                SadConsole.Engine.ConsoleRenderStack[i].IsVisible = currentConsoleIndex == i;

            Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[currentConsoleIndex];
        }
    }
}
