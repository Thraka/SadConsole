using System;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using Rectangle = SFML.Graphics.IntRect;
using static SFML.Window.Keyboard;

namespace StarterProject
{
    class Program
    {
        private static FPSCounterComponent fps;
        //private static Windows.CharacterViewer _characterWindow;
        private static int currentConsoleIndex;

        static void Main(string[] args)
        {
            var surface = SadConsole.Engine.Initialize("IBM.font", 80, 25);
            Engine.EngineDrawFrame += Engine_EngineDrawFrame;
            Engine.EngineUpdated += Engine_EngineUpdated;
            Engine.EngineShutdown += Engine_EngineShutdown;
            Engine.EngineStart += Engine_EngineStart;

            fps = new FPSCounterComponent();

            //RandomScrollingConsole surface1 = new RandomScrollingConsole();
            //surface1.IsVisible = true;
            //Engine.ConsoleRenderStack.Clear();
            //Engine.ConsoleRenderStack.Add(surface1);
            //Engine.ActiveConsole = surface1;
            //console = surface1;

            Engine.Run();
        }

        private static void Engine_EngineUpdated(object sender, EventArgs e)
        {
            //if (!_characterWindow.IsVisible)
            {
                // This block of code cycles through the consoles in the SadConsole.Engine.ConsoleRenderStack, showing only a single console
                // at a time. This code is provided to support the custom consoles demo. If you want to enable the demo, uncomment one of the lines
                // in the Initialize method above.
                if (SadConsole.Engine.Keyboard.IsKeyReleased(Key.F1))
                {
                    MoveNextConsole();
                }
                else if (SadConsole.Engine.Keyboard.IsKeyReleased(Key.F2))
                {
                   // _characterWindow.Show(true);
                }
            }

            fps.Update();
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
                                        //new CustomConsoles.WorldGenerationConsole(),
                                        new CustomConsoles.GameObjectConsole(),
                                        new CustomConsoles.CachedConsoleConsole(),
                                        new CustomConsoles.StringParsingConsole(),
                                        new CustomConsoles.CursorConsole(),
                                        new CustomConsoles.DOSConsole(),
                                        //new CustomConsoles.SceneProjectionConsole(),
                                        new CustomConsoles.ControlsTest(),
                                        new CustomConsoles.StaticConsole(),
                                        new CustomConsoles.StretchedConsole(),
                                        new CustomConsoles.BorderedConsole(),
                                        new CustomConsoles.RandomScrollingConsole(),
                                        
                                    };

            // Show the first console (by default all of our demo consoles are hidden)
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = true;

            // Set the first console in the console list as the "active" console. This allows the keyboard to be processed on the console.
            SadConsole.Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[0];

            // Initialize the windows
            //_characterWindow = new Windows.CharacterViewer();
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

        private static void Engine_EngineShutdown(object sender, Engine.ShutdownEventArgs e)
        {

        }

        private static void Engine_EngineDrawFrame(object sender, EventArgs e)
        {
            fps.Draw();
        }

        public class FPSCounterComponent
        {
            SadConsole.Consoles.TextSurfaceRenderer consoleRender;
            SadConsole.Consoles.TextSurface console;
            SadConsole.Consoles.SurfaceEditor editor;

            int frameRate = 0;
            int frameCounter = 0;
            TimeSpan elapsedTime = TimeSpan.Zero;


            public FPSCounterComponent()
            {
                console = new SadConsole.Consoles.TextSurface(10, 1, SadConsole.Engine.DefaultFont);
                editor = new SadConsole.Consoles.SurfaceEditor(console);
                console.DefaultBackground = SFML.Graphics.Color.Black;
                editor.Clear();
                consoleRender = new SadConsole.Consoles.TextSurfaceRenderer();
            }


            public void Update()
            {
                elapsedTime += SadConsole.Engine.GameTimeUpdate.ElapsedGameTime;

                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }
            }


            public void Draw()
            {
                frameCounter++;

                string fps = string.Format("fps: {0}", frameRate);
                editor.Clear();
                editor.Print(0, 0, fps);
                consoleRender.Render(console, new SFML.System.Vector2i(0, 0));
            }
        }
    }
}
