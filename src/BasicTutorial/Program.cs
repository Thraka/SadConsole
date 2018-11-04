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

        public const int ScreenWidth = 100;
        public const int ScreenHeight = 30;

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

        static SadConsole.Maps.SimpleMap map;
        static GoRogue.MapViews.ArrayMap<bool> tempMap;
        static IEnumerable<GoRogue.Rectangle> mapRooms;
        static Queue<Action> MapBuildSteps;
        static Timer mapbuildtimer;

        private static void Update(GameTime time)
        {
            // As an example, we'll use the F5 key to make the game full screen
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                SadConsole.Settings.ToggleFullScreen();
            }

            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                if (MapBuildSteps.Count != 0)
                    MapBuildSteps.Dequeue().Invoke();
            }

            //mapbuildtimer.Update(Global.GameTimeElapsedUpdate);
        }

        private static void BuildMaze()
        {
            //SadConsole.Maps.Generators.DungeonMaze gen = new SadConsole.Maps.Generators.DungeonMaze();
            //Maps.Generators.DoorGenerator gen = new Maps.Generators.DoorGenerator();
            //gen.Build(ref map);
            SadConsole.Maps.Generators.Maze.Generate(tempMap, 10, 0);

            for (int y = 0; y < tempMap.Height; y++)
            for (int x = 0; x < tempMap.Width; x++)
            {
                if (tempMap[x, y])
                    map[x, y] = SadConsole.Maps.Tile.Factory.Create("floor");
            }

            foreach (var tile in map)
                tile.Flags = SadConsole.Helpers.SetFlag(tile.Flags, (int)SadConsole.Maps.TileFlags.Seen | (int)SadConsole.Maps.TileFlags.InLOS | (int)SadConsole.Maps.TileFlags.Lighted);

        }

        private static void BuildRooms()
        {
            //SadConsole.Maps.Generators.DungeonMaze gen = new SadConsole.Maps.Generators.DungeonMaze();
            //Maps.Generators.DoorGenerator gen = new Maps.Generators.DoorGenerator();
            //gen.Build(ref map);
            mapRooms = SadConsole.Maps.Generators.Rooms.Generate(tempMap, 4, 10, 3, 15, 1f, 0.5f);

            foreach (var room in mapRooms)
            {
                foreach (var p in room.Positions())
                {
                    map[p] = SadConsole.Maps.Tile.Factory.Create("floor");
                }
            }

            foreach (var tile in map)
                tile.Flags = SadConsole.Helpers.SetFlag(tile.Flags, (int)SadConsole.Maps.TileFlags.Seen | (int)SadConsole.Maps.TileFlags.InLOS | (int)SadConsole.Maps.TileFlags.Lighted);
        }

        private static void ConnectRooms()
        {
            var RoomHallwayConnections = SadConsole.Maps.Generators.Rooms.ConnectRooms(tempMap, mapRooms, 4, 1, 50, 70, 10);
            //var RoomHallwayConnections = SadConsole.Maps.Generators.Rooms.ConnectRooms(tempMap, mapRooms, 4, 1, 50);

            bool PercentageCheck(int outOfHundred) => outOfHundred != 0 && GoRogue.Random.SingletonRandom.DefaultRNG.Next(101) < outOfHundred;

            int TurnDoorIntoWallChance = 20;

            foreach (var room in RoomHallwayConnections)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (room.Connections[i].Length != 0)
                    {
                        foreach (var point in room.Connections[i])
                        {
                            //if (!IsPointByTwoWalls(tempMap, point))
                            //{
                            //    // fill in this point because it's invalid
                            //    map[point] = SadConsole.Maps.Tile.Factory.Create("wall");
                            //    map[point].Flags = SadConsole.Helpers.SetFlag(map[point].Flags, (int)SadConsole.Maps.TileFlags.Seen | (int)SadConsole.Maps.TileFlags.InLOS | (int)SadConsole.Maps.TileFlags.Lighted);
                            //}
                            //else
                            {
                                if (!PercentageCheck(TurnDoorIntoWallChance))
                                {
                                    map[point] = SadConsole.Maps.Tile.Factory.Create("door");
                                    map[point].Flags = SadConsole.Helpers.SetFlag(map[point].Flags, (int)SadConsole.Maps.TileFlags.Seen | (int)SadConsole.Maps.TileFlags.InLOS | (int)SadConsole.Maps.TileFlags.Lighted);
                                }
                                else
                                //{
                                //    map[point] = SadConsole.Maps.Tile.Factory.Create("floor");
                                //    map[point].Flags = SadConsole.Helpers.SetFlag(map[point].Flags, (int)SadConsole.Maps.TileFlags.Seen | (int)SadConsole.Maps.TileFlags.InLOS | (int)SadConsole.Maps.TileFlags.Lighted);
                                //}
                                {
                                    map[point] = SadConsole.Maps.Tile.Factory.Create("wall");
                                    map[point].Flags = SadConsole.Helpers.SetFlag(map[point].Flags, (int)SadConsole.Maps.TileFlags.Seen | (int)SadConsole.Maps.TileFlags.InLOS | (int)SadConsole.Maps.TileFlags.Lighted);
                                }
                            }
                        }

                    }
                }
            }


            
        }

        private static void Redo()
        {
            map = new SadConsole.Maps.SimpleMap(80, 25, new Rectangle(0, 0, 80, 25));
            tempMap = new GoRogue.MapViews.ArrayMap<bool>(map.Width, map.Height);

            MapBuildSteps.Enqueue(BuildMaze);
            MapBuildSteps.Enqueue(ConnectRooms);
            MapBuildSteps.Enqueue(Redo);

            SadConsole.Global.CurrentScreen = map;

            BuildRooms();
        }


        private static void Init()
        {
            mapbuildtimer = new Timer(1, (t, d) => {

                if (MapBuildSteps.Count != 0)
                    MapBuildSteps.Dequeue().Invoke();
            });
            mapbuildtimer.Repeat = true;
            

            MapBuildSteps = new Queue<Action>();
            MapBuildSteps.Enqueue(BuildRooms);
            MapBuildSteps.Enqueue(BuildMaze);
            MapBuildSteps.Enqueue(ConnectRooms);
            MapBuildSteps.Enqueue(Redo);

            SadConsole.Maps.Tile.Factory.Add(new BasicTutorial.Maps.TileBlueprints.Door());

            // Any custom loading and prep. We will use a sample console for now

            //Console startingConsole = new Console(Width, Height);
            //startingConsole.FillWithRandomGarbage();
            //startingConsole.Fill(new Rectangle(3, 3, 27, 5), null, Color.Black, 0, SpriteEffects.None);
            //startingConsole.Print(6, 5, "Hello from SadConsole", ColorAnsi.CyanBright);

            map = new SadConsole.Maps.SimpleMap(80, 25, new Rectangle(0, 0, 80, 25));
            tempMap = new GoRogue.MapViews.ArrayMap<bool>(map.Width, map.Height);

            // Set our new console as the thing to render and process
            SadConsole.Global.CurrentScreen = map;

            map.Surface.SetRenderCells();


        }   
        


    }
}
