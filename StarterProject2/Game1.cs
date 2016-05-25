namespace StarterProject
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Game1: Game
    {
        GraphicsDeviceManager _graphics;
        Console _defaultConsole;
        //Windows.CharacterViewer _characterWindow;

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
            IsFixedTimeStep = true;

            // Uncomment these two lines to run as fast as possible
            //_graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;

            // Initialize the SadConsole engine and the first effects library (provided by the SadConsole.Effects.dll binary)
            SadConsole.Engine.Initialize(GraphicsDevice);

            // Tell SadConsole to track the mouse.
            SadConsole.Engine.UseMouse = true;

            // Load the default font.
            FontMaster masterFont;

            using (var stream = System.IO.File.OpenRead("Fonts/IBM.font"))
                masterFont = SadConsole.Serializer.Deserialize<FontMaster>(stream);

            Engine.Fonts.Add(masterFont.Name, masterFont);
            Engine.DefaultFont = masterFont.GetFont(1);

            SadConsole.Consoles.TextSurface surface1;
            surface1 = new SadConsole.Consoles.TextSurface(10, 10, Engine.DefaultFont);
            surface1.DefaultBackground = Color.Gray;
            surface1.DefaultForeground = ColorAnsi.Black;
            surface1.Clear();

            surface1.Print(2, 1, "Hello");
            tempSurface = new SadConsole.Consoles.TextSurfaceView(surface1, new Rectangle(1, 0, 4, 6));
            tempSurface2 = surface1;
            surface1.SetForeground(2, 1, Color.CadetBlue);
            tempRenderer = new SadConsole.Consoles.TextSurfaceRenderer();

            // Using the default font, resize the window to a Width,Height of cells. This example uses the MS-DOS default of 80 columns by 25 rows.
            SadConsole.Engine.DefaultFont.ResizeGraphicsDeviceManager(_graphics, 80, 25, 0, 0);

            // Create the default console, show the cursor, and let the console accept keyboard input.
            _defaultConsole = new Console(80, 25);
            _defaultConsole.VirtualCursor.IsVisible = true;
            _defaultConsole.CanUseKeyboard = true;

            // Add the default console to the list of consoles.
            SadConsole.Engine.ConsoleRenderStack.Add(_defaultConsole);

            
            // If you want to use the custom console demo provided by this starter project, uncomment out the line below.
            SadConsole.Engine.ConsoleRenderStack = new ConsoleList() {
                //new Console(surface1)
                                                                       new CustomConsoles.CursorConsole(),
                                                                       new CustomConsoles.StaticConsole(),
                                                                       new CustomConsoles.StretchedConsole(), 
                                                                       new CustomConsoles.BorderedConsole(80, 25), 
            //                                                           new CustomConsoles.WorldGenerationConsole(),
                                                                       new CustomConsoles.DOSConsole(),
            //                                                           new CustomConsoles.WindowTestConsole(),
                                                                       new CustomConsoles.EntityAndConsole(),
                                                                       new CustomConsoles.RandomScrollingConsole(),
                                                                       new CustomConsoles.SceneProjectionConsole(80,25),
                                                                       new CustomConsoles.SplashScreen(),
                                                                     };

            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = true;

            // Set the first console in the console list as the "active" console. This allows the keyboard to be processed on the console.
            SadConsole.Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[0];
            // Initialize the windows
            //_characterWindow = new Windows.CharacterViewer();

            // Uncomment to see FPS. If the unlimited FPS is not uncommented at the top, you'll probably only see 60fps.
            //Components.Add(new FPSCounterComponent(this));

            // Call the default initialize of the base class.
            base.Initialize();
        }


        SadConsole.Consoles.ITextSurface tempSurface;
        SadConsole.Consoles.ITextSurface tempSurface2;
        SadConsole.Consoles.TextSurfaceRenderer tempRenderer;

        protected override void Update(GameTime gameTime)
        {
            // Update the SadConsole engine, handles the mouse, keyboard, and any special effects. You must call this.
            SadConsole.Engine.Update(gameTime, this.IsActive);

            //if (!_characterWindow.IsVisible)
            {
                // This block of code cycles through the consoles in the SadConsole.Engine.ConsoleRenderStack, showing only a single console
                // at a time. This code is provided to support the custom consoles demo. If you want to enable the demo, uncomment one of the lines
                // in the Initialize method above.
                if (SadConsole.Engine.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F1))
                {
                    currentConsoleIndex++;

                    if (currentConsoleIndex >= SadConsole.Engine.ConsoleRenderStack.Count)
                        currentConsoleIndex = 0;

                    for (int i = 0; i < SadConsole.Engine.ConsoleRenderStack.Count; i++)
                        SadConsole.Engine.ConsoleRenderStack[i].IsVisible = currentConsoleIndex == i;

                    Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[currentConsoleIndex];
                }
                else if (SadConsole.Engine.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F2))
                {
                    //_characterWindow.Show(true);
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
            //tempRenderer.Render(tempSurface2, new Point(0, 0));
            //tempRenderer.Render(tempSurface, new Point(15, 0));

            base.Draw(gameTime);
        }
    }


    public class FPSCounterComponent : DrawableGameComponent
    {
        TextSurfaceRenderer consoleRender;
        TextSurface console;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        public FPSCounterComponent(Game game)
            : base(game)
        {
            console = new TextSurface(30, 1);
            console.DefaultBackground = Color.Black;
            console.Clear();
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
            console.Clear();
            console.Print(0, 0, fps);
            consoleRender.Render(console, Point.Zero);
        }
    }
}
