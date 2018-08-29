using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Xna.Framework;
using SadConsole;
using StarterProject;
using Console = SadConsole.Console;
using Window = Windows.UI.Xaml.Window;
using StarterProject;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409



namespace Game1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
		readonly Game1 _game;

        private StarterProject.Windows.CharacterViewer _characterWindow;
        private Container MainConsole;

        public GamePage()
        {
            this.InitializeComponent();

			// Create the game.
			var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
            
            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Hook the "after render" even though we're not using it.
            SadConsole.Game.OnDraw = DrawFrame;


            // Init
            Theme.SetupThemes();

            SadConsole.Game.Instance.Window.Title = "DemoProject OpenGL";

            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove it.

            //Global.MouseState.ProcessMouseWhenOffScreen = true;

            MainConsole = new Container();

            // We'll instead use our demo consoles that show various features of SadConsole.
            Global.CurrentScreen = MainConsole;

            // Initialize the windows
            _characterWindow = new StarterProject.Windows.CharacterViewer();


        }

        private void DrawFrame(GameTime time)
        {
            // Custom drawing. You don't usually have to do this.
        }

        private void Update(GameTime time)
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
                }
                else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
                {
                    SadConsole.Settings.ToggleFullScreen();
                }
            }
        }
    }
}
