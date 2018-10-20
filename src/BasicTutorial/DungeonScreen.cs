using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole;

namespace BasicTutorial
{
    class DungeonScreen : SadConsole.Console
    {
        public SadConsole.Maps.SimpleMap Map;

        public DungeonScreen() : base(Program.ScreenWidth, Program.ScreenHeight)
        {
            // Temp 100x100 for now.
            SadConsole.Maps.SimpleMap map = new SadConsole.Maps.SimpleMap(100, 100, new Rectangle(0, 0, 80, 25));
            SadConsole.Maps.Generators.DungeonMaze gen = new SadConsole.Maps.Generators.DungeonMaze();
            gen.Build(ref map);

        }

    }
}
