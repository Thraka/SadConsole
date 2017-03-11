using System;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Input;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.StringParser;
using SadConsole.Surfaces;

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

            // Setup the engine and creat the main window.
            SadConsole.Game.Create("Fonts/IBM.font", 80, 25);
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
        }

        private static void DrawFrame(GameTime time)
        {
            // Custom drawing. You don't usually have to do this.
            
        }

        private static void Update(GameTime time)
        {
            
            //// Called each logic update.
            //if (!_characterWindow.IsVisible)
            //{
            //    // This block of code cycles through the consoles in the SadConsole.Engine.ConsoleRenderStack, showing only a single console
            //    // at a time. This code is provided to support the custom consoles demo. If you want to enable the demo, uncomment one of the lines
            //    // in the Initialize method above.
            //    if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F1))
            //    {
            //        MainConsole.MoveNextConsole();
            //    }
            //    else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F2))
            //    {
            //        _characterWindow.Show(true);
            //    }
            //    else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F3))
            //    {
            //    }
            //    else if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            //    {
            //        SadConsole.Settings.ToggleFullScreen();
            //    }
            //}
        }

        private static void Init()
        {

            //// Any setup
            //if (Settings.UnlimitedFPS)
            //    SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance));

            //// Setup our custom theme.
            //Theme.SetupThemes();

            //SadConsole.Game.Instance.Window.Title = "DemoProject DirectX";

            //// By default SadConsole adds a blank ready-to-go console to the rendering system. 
            //// We don't want to use that for the sample project so we'll remove it.

            ////Global.MouseState.ProcessMouseWhenOffScreen = true;

            //MainConsole = new Container();

            //// We'll instead use our demo consoles that show various features of SadConsole.
            //Global.CurrentScreen = MainConsole;

            //// Initialize the windows
            //_characterWindow = new Windows.CharacterViewer();

            SadConsole.GameHelpers.GameObject player = new SadConsole.GameHelpers.GameObject(2, 2);
            player.Animation.CurrentFrame[0].Glyph = 1;
            player.Animation.CurrentFrame[1].Glyph = 1;
            player.Animation.CurrentFrame[2].Glyph = 1;
            player.Animation.CurrentFrame[3].Glyph = 1;

            player.Animation.Center = new Point(1);

            SadConsole.Global.CurrentScreen.Children.Add(player);
        }
        

    }


    class ScrollingConsole : SadConsole.ConsoleContainer
    {
        SadConsole.Console mainConsole;
        SadConsole.ControlsConsole controlsContainer;
        SadConsole.Controls.ScrollBar scrollBar;

        int scrollingCounter;

        public ScrollingConsole(int width, int height, int bufferHeight)
        {
            controlsContainer = new ControlsConsole(1, height);

            mainConsole = new Console(width - 1, bufferHeight);
            mainConsole.TextSurface.RenderArea = new Rectangle(0, 0, width - 1, height);
            mainConsole.VirtualCursor.IsVisible = true;

            scrollBar = SadConsole.Controls.ScrollBar.Create(System.Windows.Controls.Orientation.Vertical, height);
            scrollBar.IsEnabled = false;
            scrollBar.ValueChanged += ScrollBar_ValueChanged;

            controlsContainer.Add(scrollBar);
            controlsContainer.Position = new Point(1 + mainConsole.TextSurface.Width, Position.Y);

            Children.Add(mainConsole);
            Children.Add(controlsContainer);

            scrollingCounter = 0;
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Do our scroll according to where the scroll bar value is
            mainConsole.TextSurface.RenderArea = new Rectangle(0, scrollBar.Value, mainConsole.TextSurface.Width, mainConsole.TextSurface.RenderArea.Height);
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            // If we detect that this console has shifted the data up for any reason (like the virtual cursor reached the
            // bottom of the entire text surface, OR we reached the bottom of the render area, we need to adjust the 
            // scroll bar and follow the cursor
            if (mainConsole.TimesShiftedUp != 0 | mainConsole.VirtualCursor.Position.Y >= mainConsole.TextSurface.RenderArea.Height + scrollingCounter)
            {
                // Once the buffer has finally been filled enough to need scrolling, turn on the scroll bar
                scrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (scrollingCounter < mainConsole.TextSurface.Height - mainConsole.TextSurface.RenderArea.Height)
                    // Record how much we've scrolled to enable how far back the bar can see
                    scrollingCounter += mainConsole.TimesShiftedUp != 0 ? mainConsole.TimesShiftedUp : 1;

                scrollBar.Maximum = (mainConsole.TextSurface.Height + scrollingCounter) - mainConsole.TextSurface.Height;

                // This will follow the cursor since we move the render area in the event.
                scrollBar.Value = scrollingCounter;

                // Reset the shift amount.
                mainConsole.TimesShiftedUp = 0;
            }
        }

        public override bool ProcessKeyboard(Keyboard state)
        {
            // Send keyboard inptut to the main console
            return mainConsole.ProcessKeyboard(state);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            // Check the scroll bar for mouse info first. If mouse not handled by scroll bar, then..

            // Create a mouse state based on the controlsContainer
            if (!controlsContainer.ProcessMouse(new MouseConsoleState(controlsContainer, state.Mouse)))
            {
                // Process this console normally.
                return mainConsole.ProcessMouse(state);
            }

            // If we get here, then the mouse was over the scroll bar.
            return true;
        }
    }

    class ParseCommandRetext : ParseCommandBase
    {
        public int Counter;
        public char Glyph;

        public ParseCommandRetext(string parameters)
        {
            string[] parts = parameters.Split(new char[] { ':' }, 2);

            // Count and glyph type provided
            if (parts.Length == 2)
                Counter = int.Parse(parts[1]);
            else
                Counter = -1;

            // Get character
            Glyph = parts[0][0];

            // No exceptions, set the type
            CommandType = CommandTypes.Glyph;
        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex,
                                   ISurface surface, SurfaceEditor editor, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            glyphState.Glyph = Glyph;

            if (Counter != -1)
            {
                Counter--;

                if (Counter == 0)
                    commandStack.RemoveSafe(this);
            }
        }
    }
}