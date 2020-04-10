using System;
using SadConsole;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Input;
using Console = SadConsole.Console;
using System.Threading.Tasks;

namespace StarterProject
{
    class Program
    {
        private static Windows.CharacterViewer _characterWindow;
        private static Container MainConsole;

        static void Main(string[] args)
        {
            //SadConsole.Settings.UnlimitedFPS = true;
            //SadConsole.Settings.UseHardwareFullScreen = true;
            SadConsole.Settings.UseDefaultExtendedFont = true;

            // Setup the engine and creat the main window.
            SadConsole.Game.Create(80, 25);
            //SadConsole.Engine.Initialize("IBM.font", 80, 25, (g) => { g.GraphicsDeviceManager.HardwareModeSwitch = false; g.Window.AllowUserResizing = true; });

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Hook the "after render" even though we're not using it.
            SadConsole.Game.OnDraw = DrawFrame;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game has shut down.
            //

            SadConsole.Game.Instance.Dispose();
        }

        private static void DrawFrame(GameTime time)
        {
            // Custom drawing. You don't usually have to do this.
        }

        private static void Update(GameTime time)
        {
            // Called each logic update.
            if (!_characterWindow.IsVisible)
            {
                // This block of code cycles through the consoles in the SadConsole.Engine.ConsoleRenderStack, showing only a single console
                // at a time. This code is provided to support the custom consoles demo. If you want to enable the demo, uncomment one of the lines
                // in the Initialize method above.
                if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F1))
                {
                    MainConsole.MoveNextConsole();
                }
                else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F2))
                {
                    _characterWindow.Show(true);
                }
                else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F3))
                {
                    SadConsole.Debug.CurrentScreen.Show();
                }
                else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
                {
                    SadConsole.Settings.ToggleFullScreen();
                }
            }
        }

        private static void Init()
        {
            // Any setup
            if (Settings.UnlimitedFPS)
                SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance));

            // Setup our custom theme.
            Theme.SetupThemes();

            SadConsole.Game.Instance.Window.Title = "DemoProject Core";

            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove it.

            //Global.MouseState.ProcessMouseWhenOffScreen = true;
            
            MainConsole = new Container();

            // We'll instead use our demo consoles that show various features of SadConsole.
            Global.CurrentScreen = MainConsole;
            //Global.CurrentScreen.Print(1, 1, "Test".CreateColored(ColorAnsi.CyanBright, ColorAnsi.Cyan), ColorAnsi.BlueBright, ColorAnsi.Blue);

            var console = new SadConsole.ControlsConsole(50, 30)
            {
                new SadConsole.Controls.Button(7, 1)
                {
                    Text = "Click",
                    Position = new Point(5, 5)
                }
            };

            console.Invalidated += (s, e) =>
            {
                Rectangle boxArea = console.Controls[0].Bounds;
                boxArea.Inflate(1, 1);
                ((ControlsConsole)s).DrawBox(boxArea, new Cell(Color.Yellow), null, CellSurface.ConnectedLineThin);
            };


            // Initialize the windows
            _characterWindow = new Windows.CharacterViewer();


        }

        private class ConsoleProcessor : SadConsole.Components.ConsoleComponent
        {
            protected bool _readingLine = false;

            public Func<Task> Logic;

            public void ReadLine()
            {

            }

            public override void Draw(Console console, TimeSpan delta)
            {
                
            }

            public override void ProcessKeyboard(Console console, Keyboard info, out bool handled)
            {
                handled = false;
            }

            public override void ProcessMouse(Console console, MouseConsoleState state, out bool handled)
            {
                handled = false;
            }

            public override void Update(Console console, TimeSpan delta)
            {
                
            }
        }
    }
}
