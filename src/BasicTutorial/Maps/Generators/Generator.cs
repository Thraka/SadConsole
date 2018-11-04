using GoRogue.MapGeneration;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Maps;
using SadConsole.Maps.Generators;

namespace BasicTutorial.Maps.Generators
{
    class DoorGenerator: DungeonMaze
    {

        private bool Debug_ShowUniqueAreas = true;


        public int CancelDoorPlacementPercent = 10;

        public override void Build(ref SimpleMap map)
        {
            base.Build(ref map);

            var areas = MapAreaFinder.MapAreasFor(new GoRogue.MapViews.LambdaMapView<bool>(map.Width, map.Height, (point) => this.map[point].Type == Maps.TileTypes.Floor), GoRogue.AdjacencyRule.CARDINALS);

            foreach (var mapArea in areas)
            {
                Cell randomAppearance = new Cell(Color.White, new Color(GoRogue.Random.SingletonRandom.DefaultRNG.Next(256), GoRogue.Random.SingletonRandom.DefaultRNG.Next(256), GoRogue.Random.SingletonRandom.DefaultRNG.Next(256)));
                foreach (var mapAreaPosition in mapArea.Positions)
                {
                    map[mapAreaPosition].ChangeAppearance(randomAppearance);
                }
            }

            PlaceDoors();
        }

        private Tile CheckTileForArea(GoRogue.Coord point)
        {
            return map[point].Type == Maps.TileTypes.Floor ? map[point] : null;
        }

        protected void PlaceDoors()
        {
            foreach (var room in RoomHallwayConnections)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (room.Connections[i].Length != 0)
                    {
                        foreach (var point in room.Connections[i])
                        {
                            if (!IsPointByTwoWalls(point))
                            {
                                // fill in this point because it's invalid
                                map[point] = Tile.Factory.Create("wall");
                            }
                            else
                            {
                                if (!PercentageCheck(CancelDoorPlacementPercent))
                                    map[point] = Tile.Factory.Create("door");
                            }
                        }

                    }
                }
            }


        }

        /// <summary>
        /// Detects a connection where a door could be placed that is by two connections, Which would create an odd looking door.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="location">Location to check.</param>
        /// <returns>true when there are exactly two walls found.</returns>
        protected bool IsPointByTwoWalls(Point location)
        {
            Point[] points = location.GetDirectionPoints();
            bool[] validPoints = location.GetValidDirections(map.Width, map.Height);

            int counter = 0;
            for (int x = 0; x < 4; x++)
            {
                if (validPoints[x] && map[points[x]].Type == BasicTutorial.Maps.TileTypes.Wall)
                    counter++;
            }

            return counter == 2;
        }
    }
}
