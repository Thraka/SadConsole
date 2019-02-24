using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;

namespace BasicTutorial
{
    static class GameState
    {
        public static DungeonScreen Dungeon;

        public static void FirstDungeonSetup()
        {
            // Generate a map
            var gen = SadConsole.Maps.Generators.DungeonMazeGenerator.Create(100, 100, DungeonScreen.ScreenRegionMap.Width, DungeonScreen.ScreenRegionMap.Height);

            Maps.Generators.DoorGenerator.Generate(gen.SadConsoleMap, gen.Rooms, "door", 20);

            // Temp SHOW ALL TILES
            //for (var x = 0; x < gen.SadConsoleMap.Width; x++)
            //for (var y = 0; y < gen.SadConsoleMap.Height; y++)
            //    gen.SadConsoleMap[x, y].Flags = SadConsole.Helpers.SetFlag(gen.SadConsoleMap[x, y].Flags, (int)SadConsole.Maps.TileFlags.Seen | (int)SadConsole.Maps.TileFlags.InLOS | (int)SadConsole.Maps.TileFlags.Lighted);

            // Create player
            gen.SadConsoleMap.ControlledGameObject = new GameObjects.Player(gen.SadConsoleMap);
            Dungeon = new DungeonScreen(gen.SadConsoleMap);

            gen.SadConsoleMap.ControlledGameObject.MoveTo(gen.Rooms[0].InnerRect.Center);

            gen.SadConsoleMap.GameObjects.Entities.Add(gen.SadConsoleMap.ControlledGameObject);
            gen.SadConsoleMap.CenterViewPortOnPoint(gen.SadConsoleMap.ControlledGameObject.Position);
            ((GameObjects.Player)gen.SadConsoleMap.ControlledGameObject).RefreshVisibilityTiles();
        }
    }
}
