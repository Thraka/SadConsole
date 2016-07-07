namespace StarterProject
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Game1: Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        Console _defaultConsole;
        Windows.CharacterViewer _characterWindow;

        int currentConsoleIndex = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Let the XNA framework show the mouse.
            IsMouseVisible = true;

            // Uncomment these two lines to run as fast as possible
            //_graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;

            // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
            var rootConsole = SadConsole.Engine.Initialize(_graphics, "IBM.font", 80, 25);

            Theme.SetupThemes();


            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project.
            SadConsole.Engine.ConsoleRenderStack.Clear();
            SadConsole.Engine.ActiveConsole = null;

            rootConsole.IsVisible = false;
            rootConsole.Fill(Color.Blue, Color.DarkGreen, 0, null);
            rootConsole.Print(2, 2, "Hello");
            rootConsole.Position = new Point(2, 0);

            // We'll instead use our demo consoles that show various features of SadConsole.
            SadConsole.Engine.ConsoleRenderStack
                = new ConsoleList() {
                                        new CustomConsoles.SplashScreen() { SplashCompleted = () => { MoveNextConsole(); } },//Engine.ConsoleRenderStack.Remove(Engine.ConsoleRenderStack[0]); currentConsoleIndex--; } },
                                        new CustomConsoles.CursorConsole(),
                                        new CustomConsoles.DOSConsole(),
                                        new CustomConsoles.SceneProjectionConsole(),
                                        new CustomConsoles.ControlsTest(),
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

            // Uncomment to see FPS. If the unlimited FPS is not uncommented at the top, you'll probably only see 60fps.
            //Components.Add(new FPSCounterComponent(this));

            // Call the default initialize of the base class.
            base.Initialize();
        }
        
        private void MoveNextConsole()
        {
            currentConsoleIndex++;

            if (currentConsoleIndex >= SadConsole.Engine.ConsoleRenderStack.Count)
                currentConsoleIndex = 0;

            for (int i = 0; i < SadConsole.Engine.ConsoleRenderStack.Count; i++)
                SadConsole.Engine.ConsoleRenderStack[i].IsVisible = currentConsoleIndex == i;

            Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[currentConsoleIndex];
        }

        protected override void Update(GameTime gameTime)
        {
            // Update the SadConsole engine, handles the mouse, keyboard, and any special effects. You must call this.
            SadConsole.Engine.Update(gameTime, this.IsActive);

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
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen with black, like a traditional console.
            GraphicsDevice.Clear(Color.Black);

            // Draw the consoles to the screen.
            SadConsole.Engine.Draw(gameTime);

            base.Draw(gameTime);
        }
    }


    public class FPSCounterComponent : DrawableGameComponent
    {
        TextSurfaceRenderer consoleRender;
        TextSurface console;
        SurfaceEditor editor;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        public FPSCounterComponent(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            console = new TextSurface(30, 1, Engine.DefaultFont);
            editor = new SurfaceEditor(console);
            console.DefaultBackground = Color.Black;
            editor.Clear();
            consoleRender = new TextSurfaceRenderer();
        }


        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);
            editor.Clear();
            editor.Print(0, 0, fps);
            consoleRender.Render(console, Point.Zero);
        }
    }
}
