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
            IsFixedTimeStep = true;

            // Initialize the SadConsole engine and the first effects library (provided by the SadConsole.Effects.dll binary)
            SadConsole.Engine.Initialize(GraphicsDevice);

            // Tell SadConsole to track the mouse.
            SadConsole.Engine.UseMouse = true;

            // Load the default font.
            using (var stream = System.IO.File.OpenRead("Fonts/IBM.font"))
                SadConsole.Engine.DefaultFont = SadConsole.Serializer.Deserialize<Font>(stream);

            // Using the default font, resize the window to a Width,Height of cells. This example uses the MS-DOS default of 80 columns by 25 rows.
            SadConsole.Engine.DefaultFont.ResizeGraphicsDeviceManager(_graphics, 80, 25, 0, 0);

            // Create the default console, show the cursor, and let the console accept keyboard input.
            _defaultConsole = new Console(80, 25);
            _defaultConsole.VirtualCursor.IsVisible = true;
            _defaultConsole.CanUseKeyboard = true;

            // Add the default console to the list of consoles.
            SadConsole.Engine.ConsoleRenderStack.Add(_defaultConsole);

            var randomStuffConsole = new SadConsole.Consoles.Console(80, 25);
            randomStuffConsole.FillWithRandomGarbage(true);
            randomStuffConsole.IsVisible = false;

            string text = _defaultConsole.CellData.GetString(2, 0, 27);
            // If you want to use the custom console demo provided by this starter project, uncomment out the line below.
            SadConsole.Engine.ConsoleRenderStack = new ConsoleList() { new CustomConsoles.StreamConsole(),
                                                                       new CustomConsoles.CursorConsole(), 
                                                                       new CustomConsoles.StaticConsole(), 
                                                                       new CustomConsoles.StretchedConsole(), 
                                                                       new CustomConsoles.BorderedConsole(80, 25), 
                                                                       new CustomConsoles.DOSConsole(),
                                                                       new CustomConsoles.WindowTestConsole(),
                                                                       new CustomConsoles.EntityAndConsole(),
                                                                       randomStuffConsole,
                                                                       new CustomConsoles.SplashScreen(),
                                                                     };
            SadConsole.Engine.ConsoleRenderStack[0].IsVisible = true;

            // Set the first console in the console list as the "active" console. This allows the keyboard to be processed on the console.
            SadConsole.Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[0];

            // Initialize the windows
            _characterWindow = new Windows.CharacterViewer();

            // Call the default initialize of the base class.
            base.Initialize();
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
                    currentConsoleIndex++;

                    if (currentConsoleIndex >= SadConsole.Engine.ConsoleRenderStack.Count)
                        currentConsoleIndex = 0;

                    for (int i = 0; i < SadConsole.Engine.ConsoleRenderStack.Count; i++)
                        SadConsole.Engine.ConsoleRenderStack[i].IsVisible = currentConsoleIndex == i;

                    Engine.ActiveConsole = SadConsole.Engine.ConsoleRenderStack[currentConsoleIndex];
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
}
