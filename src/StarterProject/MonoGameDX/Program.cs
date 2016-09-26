using SadConsole.Consoles;
using System;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Input;

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
                    new ScrollingConsole(10, 10, 20),

                                        new CustomConsoles.SplashScreen() { SplashCompleted = () => { MoveNextConsole(); } },
                                        new CustomConsoles.StretchedConsole(),
                                        //new CustomConsoles.CachedConsoleConsole(),
                                        new CustomConsoles.StringParsingConsole(),
                                        //new CustomConsoles.CursorConsole(),
                                        //new CustomConsoles.DOSConsole(),
                                        //new CustomConsoles.SceneProjectionConsole(),
                                        new CustomConsoles.ControlsTest(),
                                        new CustomConsoles.ViewsAndSubViews(),
                                        new CustomConsoles.StaticConsole(),
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

    class ScrollingConsole: SadConsole.Consoles.Console
    {
        SadConsole.Consoles.ControlsConsole controlsContainer;
        SadConsole.Controls.ScrollBar scrollBar;

        int scrollingCounter;

        public ScrollingConsole(int width, int height, int bufferHeight): base(width - 1, bufferHeight)
        {
            controlsContainer = new ControlsConsole(1, height);

            textSurface.RenderArea = new Rectangle(0, 0, width, height);

            scrollBar = SadConsole.Controls.ScrollBar.Create(System.Windows.Controls.Orientation.Vertical, height);
            scrollBar.IsEnabled = false;
            scrollBar.ValueChanged += ScrollBar_ValueChanged;

            controlsContainer.Add(scrollBar);
            controlsContainer.Position = new Point(Position.X + width - 1, Position.Y);
            controlsContainer.IsVisible = true;
            controlsContainer.MouseCanFocus = false;
            controlsContainer.ProcessMouseWithoutFocus = true;

            scrollingCounter = 0;
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Do our scroll according to where the scroll bar value is
            textSurface.RenderArea = new Rectangle(0, scrollBar.Value, textSurface.Width, textSurface.RenderArea.Height);
        }

        protected override void OnPositionChanged(Point oldLocation)
        {
            // Keep the controls console (which is our scroll bar) in sync with where this console is.
            controlsContainer.Position = new Point(Position.X + Width, Position.Y);
        }

        protected override void OnVisibleChanged()
        {
            // Show and hide the scroll bar.
            controlsContainer.IsVisible = this.IsVisible;
        }

        public override void Render()
        {
            // Draw our console and then draw the scroll bar.
            base.Render();
            controlsContainer.Render();
        }

        public override void Update()
        {
            // Update our console and then update the scroll bar
            base.Update();
            controlsContainer.Update();

            // If we detect that this console has shifted the data up for any reason (like the virtual cursor reached the
            // bottom of the entire text surface, OR we reached the bottom of the render area, we need to adjust the 
            // scroll bar and follow the cursor
            if (TimesShiftedUp != 0 | _virtualCursor.Position.Y == textSurface.RenderArea.Height + scrollingCounter)
            {
                // Once the buffer has finally been filled enough to need scrolling, turn on the scroll bar
                scrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (scrollingCounter < textSurface.Height - textSurface.RenderArea.Height)
                    // Record how much we've scrolled to enable how far back the bar can see
                    scrollingCounter += TimesShiftedUp != 0 ? TimesShiftedUp : 1;

                scrollBar.Maximum = (textSurface.Height + scrollingCounter) - textSurface.Height;

                // This will follow the cursor since we move the render area in the event.
                scrollBar.Value = scrollingCounter;

                // Reset the shift amount.
                TimesShiftedUp = 0;
            }
        }

        public override bool ProcessMouse(MouseInfo info)
        {
            // Check the scroll bar for mouse info first. If mouse not handled by scroll bar, then..
            if (!controlsContainer.ProcessMouse(info))
            {
                // Process this console normally.
                return base.ProcessMouse(info);
            }

            // If we get here, then the mouse was over the scroll bar.
            return true;
        }
    }
}
