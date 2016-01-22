    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace SadRogueSharp
    {
        /// <summary>
        /// This is the main type for your game
        /// </summary>
        public class Game1 : Game
        {
            GraphicsDeviceManager graphics;

            public Game1()
                : base()
            {
                graphics = new GraphicsDeviceManager(this);
                Content.RootDirectory = "Content";

                var sadConsoleComponent = new SadConsole.EngineGameComponent(this, () =>
                {
                    // Use the default square font
                    using (var stream = System.IO.File.OpenRead("Fonts/Cheepicus12.font"))
                        SadConsole.Engine.DefaultFont = SadConsole.Serializer.Deserialize<SadConsole.Font>(stream);

                    int width = 60;
                    int height = 30;

                    // Use the IBM console style font
                    //using (var stream = System.IO.File.OpenRead("Fonts/IBM.font"))
                    //    SadConsole.Engine.DefaultFont = SadConsole.Serializer.Deserialize<SadConsole.Font>(stream);

                    //int width = 80;
                    //int height = 30;

                    SadConsole.Engine.DefaultFont.ResizeGraphicsDeviceManager(graphics, width, height, 0, 0);
                    SadConsole.Engine.UseMouse = true;
                    SadConsole.Engine.UseKeyboard = true;

                    var mapConsole = new Consoles.MapConsole(100, 100);
                    var statusConsole = new Consoles.Status();

                    SadConsole.Engine.ConsoleRenderStack.Add(mapConsole);
                    SadConsole.Engine.ConsoleRenderStack.Add(statusConsole);

                    SadConsole.Engine.ActiveConsole = mapConsole;
                });

                Components.Add(sadConsoleComponent);
            }

            /// <summary>
            /// Allows the game to perform any initialization it needs to before starting to run.
            /// This is where it can query for any required services and load any non-graphic
            /// related content.  Calling base.Initialize will enumerate through any components
            /// and initialize them as well.
            /// </summary>
            protected override void Initialize()
            {
                IsMouseVisible = true;

                base.Initialize();
            }

            /// <summary>
            /// LoadContent will be called once per game and is the place to load
            /// all of your content.
            /// </summary>
            protected override void LoadContent()
            {
            }

            /// <summary>
            /// UnloadContent will be called once per game and is the place to unload
            /// all content.
            /// </summary>
            protected override void UnloadContent()
            {
            }

            /// <summary>
            /// Allows the game to run logic such as updating the world,
            /// checking for collisions, gathering input, and playing audio.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
            }

            /// <summary>
            /// This is called when the game should draw itself.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Draw(GameTime gameTime)
            {
                GraphicsDevice.Clear(Color.Black);

                base.Draw(gameTime);
            }
        }
    }
