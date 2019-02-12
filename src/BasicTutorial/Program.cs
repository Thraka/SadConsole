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
using SadConsole.Input;
using Console = SadConsole.Console;

namespace BasicTutorial
{
    class Program
    {
        public const int ScreenWidth = 100;
        public const int ScreenHeight = 30;


        //public static readonly Rectangle 

        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create(ScreenWidth, ScreenHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            //SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            SadConsole.Maps.Tile.Factory.Add(new BasicTutorial.Maps.TileBlueprints.Door());

            GameState.FirstDungeonSetup();
            Global.CurrentScreen.Children.Add(GameState.Dungeon);
            
        }
    }
}
