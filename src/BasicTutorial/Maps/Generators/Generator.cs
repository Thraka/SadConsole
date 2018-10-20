using Microsoft.Xna.Framework;
using SadConsole.Maps;
using SadConsole.Maps.Generators;

namespace BasicTutorial.Maps.Generators
{
    class DoorGenerator: DungeonMaze
    {
        public int CancelDoorPlacementPercent = 50;

        public override void Build(ref SimpleMap map)
        {
            base.Build(ref map);
            PlaceDoors();
        }

        protected void PlaceDoors()
        {
            foreach (var room in RoomHallwayConnections)
            {
                int doorCount = SadConsole.Global.Random.Next(1, 6);
                bool placedDoor = false;

                if (room.Connections.Length != 0)
                    //System.Diagnostics.Debugger.Break();
                    //else
                    for (int i = 0; i < doorCount; i++)
                    {
                        for (int tryCount = 0; tryCount < 3; tryCount++)
                        {
                            Point spot = room.Connections[SadConsole.Global.Random.Next(0, room.Connections.Length)];

                            if (!IsPointNearType(spot, BasicTutorial.Maps.TileTypes.Door))
                            {
                                placedDoor = true;
                                map[spot] = Tile.Factory.Create("door");

                                break;
                            }
                        }

                        if (placedDoor && PercentageCheck(CancelDoorPlacementPercent))
                            break;
                    }
            }
        }
    }
}
