using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.Effects;
using SadConsole.Entities;
using Console = SadConsole.Console;

namespace BasicTutorial
{
    class Program
    {

        public const int ScreenWidth = 80;
        public const int ScreenHeight = 25;

        public static readonly Rectangle MapDrawArea = new Rectangle(0, 0, ScreenWidth - 20, ScreenHeight - 5);
        //public static readonly Rectangle 

        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create(ScreenWidth, ScreenHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            SadConsole.Game.Instance.Dispose();
        }

        class Password
        {
            public string Text;
        }

        private static void Update(GameTime time)
        {
            // As an example, we'll use the F5 key to make the game full screen
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                SadConsole.Settings.ToggleFullScreen();
            }
        }

        private static void Init()
        {
            // Any custom loading and prep. We will use a sample console for now

            //Console startingConsole = new Console(Width, Height);
            //startingConsole.FillWithRandomGarbage();
            //startingConsole.Fill(new Rectangle(3, 3, 27, 5), null, Color.Black, 0, SpriteEffects.None);
            //startingConsole.Print(6, 5, "Hello from SadConsole", ColorAnsi.CyanBright);

            SadConsole.Maps.SimpleMap map = new SadConsole.Maps.SimpleMap(80, 25, new Rectangle(0, 0, 80, 25));
            SadConsole.Maps.Generators.DungeonMaze gen = new SadConsole.Maps.Generators.DungeonMaze();
            gen.Build(ref map);

            foreach (var tile in map.Tiles)
            {
                tile.Flags = SadConsole.Helpers.SetFlag(tile.Flags, (int) SadConsole.Maps.TileFlags.Seen);
            }

            // Set our new console as the thing to render and process
            SadConsole.Global.CurrentScreen = map;
        }   
        


    }
}
