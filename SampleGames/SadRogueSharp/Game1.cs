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
    public class Game1 : SadConsole.Game
    {
        public Game1() : base("Cheepicus12.font", 60, 30, null)
        {
            
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

            //var mapConsole = new Consoles.MapConsole(100, 100);
            //var statusConsole = new Consoles.Status();

            //SadConsole.Global.CurrentScreen.Children.Add(mapConsole);
            //SadConsole.Global.CurrentScreen.Children.Add(statusConsole);

            //SadConsole.Global.FocusedConsoles.Set(mapConsole);

            var firstConsole = new SadConsole.Console(60, 30);

            firstConsole.FillWithRandomGarbage();
            firstConsole.Fill(new Rectangle(2, 2, 20, 3), Color.Aqua, Color.Black, 0);
            firstConsole.Print(3, 3, "Hello World!");

            SadConsole.Global.CurrentScreen.Children.Add(firstConsole);
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

    }
}
