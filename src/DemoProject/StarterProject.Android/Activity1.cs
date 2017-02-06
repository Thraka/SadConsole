using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;
using SadConsole;

namespace StarterProject
{
    [Activity(Label = "StarterProject.Android"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorLandscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        private static Windows.CharacterViewer _characterWindow;
        private static int currentConsoleIndex;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //SadConsole.Settings.UnlimitedFPS = true;
            //SadConsole.Settings.UseHardwareFullScreen = true;

            // Setup the engine and creat the main window.
            SadConsole.Game.Create("Fonts/IBM.font", 80, 25);
            //SadConsole.Engine.Initialize("IBM.font", 80, 25, (g) => { g.GraphicsDeviceManager.HardwareModeSwitch = false; g.Window.AllowUserResizing = true; });

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Hook the "after render" even though we're not using it.
            SadConsole.Game.OnDraw = DrawFrame;

            // Setup the android view
            SetContentView((View)SadConsole.Game.Instance.Services.GetService(typeof(View)));
            
            // Run the game
            SadConsole.Game.Instance.Run();
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
                if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.A))
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

        private static void Init()
        {
            // Any setup
            if (Settings.UnlimitedFPS)
                SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance));


            // Setup our custom theme.
            StarterProject.Theme.SetupThemes();

            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove it.

            //Global.MouseState.ProcessMouseWhenOffScreen = true;

            // We'll instead use our demo consoles that show various features of SadConsole.
            Global.CurrentScreen.Children.Add(new CustomConsoles.RandomScrollingConsole());
            Global.CurrentScreen.Children.Add(new CustomConsoles.AutoTypingConsole());
            Global.CurrentScreen.Children.Add(new CustomConsoles.StringParsingConsole());
            Global.CurrentScreen.Children.Add(new CustomConsoles.ControlsTest());
            //Global.CurrentScreen.Children.Add(new CustomConsoles.AnsiConsole()); //Code to load ansi files is not platform independent yet.
            Global.CurrentScreen.Children.Add(new CustomConsoles.StretchedConsole());
            Global.CurrentScreen.Children.Add(new CustomConsoles.SubConsoleCursor());
            Global.CurrentScreen.Children.Add(new CustomConsoles.CursorConsole());
            Global.CurrentScreen.Children.Add(new CustomConsoles.ViewsAndSubViews());
            Global.CurrentScreen.Children.Add(new CustomConsoles.GameObjectConsole());
            Global.CurrentScreen.Children.Add(new CustomConsoles.DOSConsole());
            Global.CurrentScreen.Children.Add(new CustomConsoles.BorderedConsole());
            Global.CurrentScreen.Children.Add(new CustomConsoles.ScrollableConsole(25, 6, 70));

            // Not working... it was...
            Global.CurrentScreen.Children.Add(new CustomConsoles.SceneProjectionConsole());

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
            Global.CurrentScreen.Children[0].IsVisible = true;

            // Set the first console in the console list as the "active" console. This allows the keyboard to be processed on the console.
            Console.ActiveConsole = (IConsole)Global.CurrentScreen.Children[0];

            // Initialize the windows
            _characterWindow = new Windows.CharacterViewer();

            //SadConsole.Effects.Fade a = new SadConsole.Effects.Fade();
            //a.DestinationForeground = Microsoft.Xna.Framework.Color.Turquoise;
            //SadConsole.Engine.MonoGameInstance.Components.Add(new FPSCounterComponent(SadConsole.Engine.MonoGameInstance));


        }

        private static void MoveNextConsole()
        {
            currentConsoleIndex++;

            if (currentConsoleIndex >= Global.CurrentScreen.Children.Count)
                currentConsoleIndex = 0;

            for (int i = 0; i < Global.CurrentScreen.Children.Count; i++)
                Global.CurrentScreen.Children[i].IsVisible = currentConsoleIndex == i;

            Console.ActiveConsole = (IConsole)Global.CurrentScreen.Children[currentConsoleIndex];

        }
    }
}

