using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SadConsole;
using System.Threading.Tasks;
using GoRogue.MapViews;
using GoRogue.Random;

namespace SadConsole.Maps.Generators
{
    /// <summary>
    /// Generates a maze-room dungeon.
    /// </summary>
    public class DungeonMazeGenerator
    {
        /// <summary>
        /// Settings for the generator.
        /// </summary>
        public class GeneratorSettings
        {
            /// <summary>
            /// The minimum amount of rooms to generate.
            /// </summary>
            public int RoomsCountMin = 4;

            /// <summary>
            /// The maximum amount of rooms to generate.
            /// </summary>
            public int RoomsCountMax = 10;

            /// <summary>
            /// The minimum size of a room.
            /// </summary>
            public int RoomsSizeMin = 3;

            /// <summary>
            /// The maximum size of a room.
            /// </summary>
            public int RoomsSizeMax = 15;

            /// <summary>
            /// Adjusts the width of the room in relation to <see cref="RoomsSizeFontRatioY"/>.
            /// </summary>
            public float RoomsSizeFontRatioX = 1f;

            /// <summary>
            /// Adjusts the height of the room in relation to <see cref="RoomsSizeFontRatioX"/>.
            /// </summary>
            public float RoomsSizeFontRatioY = 0.5f;

            /// <summary>
            /// The minimum sides that must be connected in a room to the maze.
            /// </summary>
            public int RoomsConnectionsMinSides = 1;

            /// <summary>
            /// The maximum sides that can be connected in a room to the maze.
            /// </summary>
            public int RoomsConnectionsMaxSides = 4;

            /// <summary>
            /// The chance (out of 100) to stop nominating sides for connections.
            /// </summary>
            public int RoomsConnectionsCancelSideSelectChance = 50;
            
            /// <summary>
            /// The chance (out of 100) to stop placing connections on a selected side.
            /// </summary>
            public int RoomsConnectionsCancelPlacementChance = 70;

            /// <summary>
            /// The amount to add to <see cref="RoomsConnectionsCancelPlacementChance"/> after each connection.
            /// </summary>
            public int RoomsConnectionsCancelPlacementChanceIncrease = 10;

            /// <summary>
            /// The chance improvement to add to the maze crawler to turn every time a spot is placed.
            /// </summary>
            public int MazeChangeDirectionImprovement = 10;

            /// <summary>
            /// The chance (out of 100) to keep a dead end in the maze.
            /// </summary>
            public int MazeSaveDeadEndChance = 0;

            /// <summary>
            /// The blueprint to use with the <see cref="Tile.Factory"/> to generate a floor tile.
            /// </summary>
            public string TileBlueprintFloor = "floor";

            /// <summary>
            /// The blueprint to use with the <see cref="Tile.Factory"/> to generate a wall tile.
            /// </summary>
            public string TileBlueprintWall = "wall";
        }

        /// <summary>
        /// The map from GoRogue used in generation.
        /// </summary>
        public ArrayMap<bool> GoRogueMap { get; protected set; }

        /// <summary>
        /// The SadConsole map generated from the <see cref="GoRogueMap"/>.
        /// </summary>
        public Maps.MapConsole SadConsoleMap { get; protected set; }

        /// <summary>
        /// Settings used to generate the map.
        /// </summary>
        public GeneratorSettings Settings { get; protected set; }

        /// <summary>
        /// The rooms generated for the map.
        /// </summary>
        public IReadOnlyList<Region> Rooms { get; protected set; }

        /// <summary>
        /// The width of the map.
        /// </summary>
        protected int MapWidth;

        /// <summary>
        /// The height of the map.
        /// </summary>
        protected int MapHeight;

        /// <summary>
        /// The width of the viewport used in the map.
        /// </summary>
        protected int ViewPortWidth;

        /// <summary>
        /// The height of the viewport used in the map.
        /// </summary>
        protected int ViewPortHeight;


        /// <summary>
        /// Creates a new dungeon generator.
        /// </summary>
        protected DungeonMazeGenerator() { }

        /// <summary>
        /// Creates a new map from the specified generator settings.
        /// </summary>
        /// <param name="settings">The settings used to generate the dungeon.</param>
        /// <param name="mapWidth">The width of the map.</param>
        /// <param name="mapHeight">The height of the map.</param>
        /// <param name="viewPortWidth">The width of the viewport used in the map.</param>
        /// <param name="viewPortHeight">The height of the viewport used in the map.</param>
        /// <returns>The generator.</returns>
        public static DungeonMazeGenerator Create(GeneratorSettings settings, int mapWidth, int mapHeight, int viewPortWidth, int viewPortHeight)
        {
            var generator = new DungeonMazeGenerator
            {
                Settings = settings,
                MapWidth = mapWidth,
                MapHeight = mapHeight,
                ViewPortWidth = viewPortWidth,
                ViewPortHeight = viewPortHeight
            };

            generator.Generate();

            return generator;
        }

        /// <summary>
        /// Creates a new map using default settings and the specified map width and height.
        /// </summary>
        /// <param name="mapWidth">The width of the map.</param>
        /// <param name="mapHeight">The height of the map.</param>
        /// <param name="viewPortWidth">The width of the viewport used in the map.</param>
        /// <param name="viewPortHeight">The height of the viewport used in the map.</param>
        /// <returns>The generator.</returns>
        public static DungeonMazeGenerator Create(int mapWidth, int mapHeight, int viewPortWidth, int viewPortHeight)
        {
            return Create(new GeneratorSettings(), mapWidth, mapHeight, viewPortWidth, viewPortHeight);
        }
        
        /// <summary>
        /// Generates the map.
        /// </summary>
        protected virtual void Generate()
        {
            SadConsoleMap = new SadConsole.Maps.MapConsole(MapWidth, MapHeight);
            GoRogueMap = new GoRogue.MapViews.ArrayMap<bool>(MapWidth, MapHeight);

            // Generate rooms
            var mapRooms = GoRogue.MapGeneration.Generators.RoomsGenerator.Generate(GoRogueMap, Settings.RoomsCountMin, Settings.RoomsCountMax, 
                                                                                                Settings.RoomsSizeMin, Settings.RoomsSizeMax, 
                                                                                                Settings.RoomsSizeFontRatioX, Settings.RoomsSizeFontRatioY);

            // Generate maze
            GoRogue.MapGeneration.Generators.MazeGenerator.Generate(GoRogueMap, Settings.MazeChangeDirectionImprovement);
            
            // Conenct rooms to maze
            var connections = GoRogue.MapGeneration.Connectors.RoomDoorConnector.ConnectRooms(GoRogueMap, mapRooms, 
                                                                         Settings.RoomsConnectionsMinSides, Settings.RoomsConnectionsMaxSides, 
                                                                         Settings.RoomsConnectionsCancelSideSelectChance, 
                                                                         Settings.RoomsConnectionsCancelPlacementChance, Settings.RoomsConnectionsCancelPlacementChanceIncrease);
            // Transform rooms into regions
            Rooms = (from c in connections
                    let innerRect = (Rectangle)c.Room
                    let outerRect = (Rectangle)c.Room.Expand(1, 1)
                    select new Region()
                    {
                        IsRectangle = true,
                        InnerRect = innerRect,
                        OuterRect = outerRect,
                        InnerPoints = new List<Point>(innerRect.GetPoints()),
                        OuterPoints = new List<Point>(outerRect.GetPoints()),
                        Connections = c.Connections.SelectMany(a => a).Select(a => (Point)a).ToList()
                    }).ToList();

            // Copy regions to map
            SadConsoleMap.Regions = new List<Region>(Rooms);

            // Create tiles in the SadConsole map
            foreach (var position in GoRogueMap.Positions())
            {
                if (GoRogueMap[position])
                    SadConsoleMap[position] = Tile.Factory.Create(Settings.TileBlueprintFloor);
                else
                    SadConsoleMap[position] = Tile.Factory.Create(Settings.TileBlueprintWall);
            }

            foreach (var region in Rooms)
            {
                foreach (var point in region.InnerPoints)
                {
                    SadConsoleMap[point].SetFlag(TileFlags.RegionLighted);
                }
            }
        }
    }
}
