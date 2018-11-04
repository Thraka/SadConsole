using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SadConsole;
using System.Threading.Tasks;
using GoRogue.MapViews;
using GoRogue.Random;

// Implemented the logic from http://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/

namespace SadConsole.Maps.Generators
{
    public static class Rooms
    {
        public static IEnumerable<GoRogue.Rectangle> Generate(ISettableMapView<bool> map, int minRooms, int maxRooms, 
                                                               int roomMinSize, int roomMaxSize,
                                                               float roomSizeRatioX, float roomSizeRatioY)
        {
            if (minRooms > maxRooms)
                throw new ArgumentOutOfRangeException(nameof(minRooms), "The minimum amount of rooms must be less than or equal to the maximum amount of rooms.");

            if (roomMinSize > roomMaxSize)
                throw new ArgumentOutOfRangeException(nameof(roomMinSize), "The minimum size of a room must be less than or equal to the maximum size of a room.");

            if (roomSizeRatioX == 0f)
                throw new ArgumentOutOfRangeException(nameof(roomSizeRatioX), "Ratio cannot be zero.");

            if (roomSizeRatioY == 0f)
                throw new ArgumentOutOfRangeException(nameof(roomSizeRatioX), "Ratio cannot be zero.");


            var roomCounter = GoRogue.Random.SingletonRandom.DefaultRNG.Next(minRooms, maxRooms + 1);
            var rooms = new List<GoRogue.Rectangle>(roomCounter);

            while(roomCounter != 0)
            {
                var tryCounterCreate = 10;
                var placed = false;

                while (tryCounterCreate != 0)
                {
                    var roomSize = SingletonRandom.DefaultRNG.Next(roomMinSize, roomMaxSize + 1);
                    var width = (int)(roomSize * roomSizeRatioX);  // this helps with non square fonts. So rooms dont look odd
                    var height = (int)(roomSize * roomSizeRatioY);


                    // When accounting for font ratios, these adjustments help prevent all rooms having the same looking square format
                    var adjustmentBase = roomSize / 4;

                    if (adjustmentBase != 0)
                    {
                        var adjustment = SingletonRandom.DefaultRNG.Next(-adjustmentBase, adjustmentBase + 1);
                        var adjustmentChance = SingletonRandom.DefaultRNG.Next(0, 2);

                        if (adjustmentChance == 0)
                            width += (int)(adjustment * roomSizeRatioX);
                        else if (adjustmentChance == 1)
                            height += (int)(adjustment * roomSizeRatioY);
                    }

                    width = Math.Max(roomMinSize, width);
                    height = Math.Max(roomMinSize, height);

                    // Keep room interior odd, helps with placement + tunnels around the outside.
                    if (width % 2 == 0)
                        width += 1;

                    if (height % 2 == 0)
                        height += 1;

                    var roomInnerRect = new GoRogue.Rectangle(0, 0, width, height);

                    var tryCounterPlace = 10;

                    while (tryCounterPlace != 0)
                    {
                        bool intersected = false;

                        roomInnerRect = roomInnerRect.Move(GoRogue.Coord.Get(SingletonRandom.DefaultRNG.Next(3, map.Width - roomInnerRect.Width - 3), SingletonRandom.DefaultRNG.Next(3, map.Height - roomInnerRect.Height - 3)));

                        // 
                        var roomBounds = roomInnerRect.Expand(3, 3);

                        foreach (var point in roomBounds.Positions())
                        {
                            if (map[point])
                            {
                                intersected = true;
                                break;
                            }
                        }

                        if (intersected)
                        {
                            tryCounterPlace--;
                            continue;
                        }

                        foreach (var point in roomInnerRect.Positions())
                            map[point] = true;

                        placed = true;
                        rooms.Add(roomInnerRect);
                        break;
                    }

                    if (placed)
                        break;

                    tryCounterCreate--;
                }

                roomCounter--;
            }

            return rooms;
        }

        
    }

    public static class Maze
    {
        static bool PercentageCheck(int outOfHundred) => outOfHundred != 0 && SingletonRandom.DefaultRNG.Next(101) < outOfHundred;
        static bool PercentageCheck(double outOfHundred) => outOfHundred != 0d && SingletonRandom.DefaultRNG.NextDouble() < outOfHundred;

        class Crawler
        {
            public Stack<GoRogue.Coord> Path = new Stack<GoRogue.Coord>();
            public List<GoRogue.Coord> AllPositions = new List<GoRogue.Coord>();
            public GoRogue.Coord CurrentPosition = GoRogue.Coord.Get(0, 0);
            public bool IsActive = true;
            public GoRogue.Direction Facing = GoRogue.Direction.UP;

            public void MoveTo(GoRogue.Coord position)
            {
                Path.Push(position);
                AllPositions.Add(position);
                CurrentPosition = position;
            }

            public void Backtrack()
            {
                if (Path.Count != 0)
                    CurrentPosition = Path.Pop();
            }
        }

        public static void Generate(ISettableMapView<bool> map, int crawlerChangeDirectionImprovement, int saveDeadEndChance)
        {
            List<Crawler> Crawlers = new List<Crawler>();

            var empty = FindEmptySquare(map);
            var randomCounter = 0;
            var randomSuccess = false;

            while (empty != null)
            {
                Crawler crawler = new Crawler();
                Crawlers.Add(crawler);
                crawler.MoveTo(empty);
                var startedCrawler = true;
                var percentChangeDirection = 0;
                //Color randomCrawlerColor = Color.AliceBlue.GetRandomColor(SadConsole.Global.Random);

                while (crawler.Path.Count != 0)
                {
                    // Dig this position
                    map[crawler.CurrentPosition] = true;
                    //map[crawler.CurrentPosition.X, crawler.CurrentPosition.Y].Background = randomCrawlerColor;

                    // Get valid directions (basically is any position outside the map or not?)
                    var points = GoRogue.AdjacencyRule.CARDINALS.NeighborsClockwise(crawler.CurrentPosition).ToArray();
                    var directions = GoRogue.AdjacencyRule.CARDINALS.DirectionsOfNeighborsClockwise(GoRogue.Direction.NONE).ToList();
                    var valids = new bool[4];


                    // Rule out any valids based on their position.
                    // Only process NSEW, do not use diagonals
                    for (var i = 0; i < 4; i++)
                        valids[i] = IsPointWallsExceptSource(map, points[i], directions[i] + 4);

                    // If not a new crawler, exclude where we came from
                    if (!startedCrawler)
                        valids[directions.IndexOf(crawler.Facing + 4)] = false;

                    // Do we have any valid direction to go?
                    if (valids[0] || valids[1] || valids[2] || valids[3])
                    {
                        var index = 0;

                        // Are we just starting this crawler? OR Is the current crawler facing direction invalid?
                        if (startedCrawler || valids[directions.IndexOf(crawler.Facing)] == false)
                        {
                            // Just get anything
                            index = GetDirectionIndex(valids);
                            crawler.Facing = directions[index];
                            percentChangeDirection = 0;
                            startedCrawler = false;
                        }
                        else
                        {
                            // Increase probablity we change direction
                            percentChangeDirection += crawlerChangeDirectionImprovement;

                            if (PercentageCheck(percentChangeDirection))
                            {
                                index = GetDirectionIndex(valids);
                                crawler.Facing = directions[index];
                                percentChangeDirection = 0;
                            }
                            else
                                index = directions.IndexOf(crawler.Facing);
                        }

                        crawler.MoveTo(points[index]);
                    }
                    else
                        crawler.Backtrack();
                }

                empty = FindEmptySquare(map);
            }

            TrimDeadPaths(map, Crawlers, saveDeadEndChance);
        }

        static void TrimDeadPaths(ISettableMapView<bool> map, IEnumerable<Crawler> Crawlers, int saveDeadEndChance)
        {
            foreach (var crawler in Crawlers)
            {
                List<GoRogue.Coord> safeDeadEnds = new List<GoRogue.Coord>();
                List<GoRogue.Coord> deadEnds = new List<GoRogue.Coord>();

                while (true)
                {
                    foreach (var point in crawler.AllPositions)
                    {
                        var points = GoRogue.AdjacencyRule.EIGHT_WAY.NeighborsClockwise(point).ToArray();
                        var directions = GoRogue.AdjacencyRule.EIGHT_WAY.DirectionsOfNeighborsClockwise(GoRogue.Direction.NONE).ToList();

                        for (int i = 0; i < 8; i += 2)
                        {
                            if (map[points[i]])
                            {
                                var oppDir = directions[i] + 4;
                                bool found = false;

                                // If we get here, source direction is a floor, opposite direction should be wall
                                if (!map[points[(int) oppDir.Type]])
                                {
                                    switch (oppDir.Type)
                                    {
                                        // Check for a wall pattern in the map. In this case something like where X is a wall
                                        // XXX
                                        // X X
                                        case GoRogue.Direction.Types.UP:
                                            found = !map[points[(int) GoRogue.Direction.Types.UP_LEFT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.UP_RIGHT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.LEFT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.RIGHT]];
                                            break;

                                        case GoRogue.Direction.Types.DOWN:
                                            found = !map[points[(int) GoRogue.Direction.Types.DOWN_LEFT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.DOWN_RIGHT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.LEFT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.RIGHT]];
                                            break;

                                        case GoRogue.Direction.Types.RIGHT:
                                            found = !map[points[(int) GoRogue.Direction.Types.UP_RIGHT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.DOWN_RIGHT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.UP]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.DOWN]];
                                            break;

                                        case GoRogue.Direction.Types.LEFT:
                                            found = !map[points[(int) GoRogue.Direction.Types.UP_LEFT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.DOWN_LEFT]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.UP]] &&
                                                    !map[points[(int) GoRogue.Direction.Types.DOWN]];
                                            break;
                                    }
                                }


                                if (found)
                                    deadEnds.Add(point);

                                break;
                            }
                        }

                    }

                    deadEnds = new List<GoRogue.Coord>(deadEnds.Except(safeDeadEnds));
                    crawler.AllPositions = new List<GoRogue.Coord>(crawler.AllPositions.Except(deadEnds));

                    if (deadEnds.Count == 0)
                        break;

                    foreach (var point in deadEnds)
                    {
                        if (PercentageCheck(saveDeadEndChance))
                        {
                            safeDeadEnds.Add(point);
                        }
                        else
                            map[point] = false;
                    }

                    deadEnds.Clear();
                    break; // For now we only do 1 pass. Have to design this differently
                           // the sadconsole version didn't need to quit like this, unsure
                           // what the difference is...
                }
            }
        }


        static bool IsPointSurroundedByWall(IMapView<bool> map, GoRogue.Coord location)
        {
            var points = GoRogue.AdjacencyRule.EIGHT_WAY.Neighbors(location);

            var index = location.ToPoint().GetDirectionIndexes(map.Width, map.Height);
            var mapBounds = map.Bounds();
            foreach (var point in points)
            {
                if (!mapBounds.Contains(point))
                    return false;

                if (map[point])
                    return false;
            }

            return true;
        }

        static bool IsPointConsideredEmpty(IMapView<bool> map, GoRogue.Coord location)
        {
            return !IsPointMapEdge(map, location) &&  // exclude outer ridge of map
                   IsPointOdd(location) && // check is odd numer position
                   //!IsPointPartOfRegion(map, location, true) && // check if not part of a room region
                   IsPointSurroundedByWall(map, location) && // make sure is surrounded by a wall.
                   IsPointWall(map, location); // The location is a wall
        }

        static bool IsPointWall(IMapView<bool> map, GoRogue.Coord location)
        {
            return !map[location];
        }

        static bool IsPointPartOfRegion(IMapView<bool> map, GoRogue.Coord location, bool includeOuterEdge)
        {
            //Rectangle outerEdge;

            //foreach (var region in map.Regions)
            //{
            //    if (includeOuterEdge)
            //    {
            //        outerEdge = region.InnerRect;
            //        outerEdge.Inflate(1, 1);

            //        if (outerEdge.Contains(location))
            //            return true;
            //    }
            //    else if (region.InnerRect.Contains(location))
            //        return true;
            //}

            return false;
        }

        static bool IsPointOdd(GoRogue.Coord location)
        {
            return location.X % 2 != 0 && location.Y % 2 != 0;
        }

        static bool IsPointMapEdge(IMapView<bool> map, GoRogue.Coord location, bool onlyEdgeTest = false)
        {
            if (onlyEdgeTest)
                return location.X == 0 || location.X == map.Width - 1 || location.Y == 0 || location.Y == map.Height - 1;
            else
                return location.X <= 0 || location.X >= map.Width - 1 || location.Y <= 0 || location.Y >= map.Height - 1;

        }

        static bool IsPointWallsExceptSource(IMapView<bool> map, GoRogue.Coord location, GoRogue.Direction sourceDirection)
        {
            // exclude the outside of the map
            var mapInner = map.Bounds().Expand(-1, -1);

            if (!mapInner.Contains(location))
            // Shortcut out if this location is part of the map edge.
            //if (IsPointMapEdge(location) || IsPointPartOfRegion(location, true))
                return false;

            // Get map indexes for all surrounding locations
            var index = GoRogue.AdjacencyRule.EIGHT_WAY.DirectionsOfNeighborsClockwise().ToArray();

            GoRogue.Direction[] skipped;

            if (sourceDirection == GoRogue.Direction.RIGHT)
                skipped = new GoRogue.Direction[] { sourceDirection, GoRogue.Direction.UP_RIGHT, GoRogue.Direction.DOWN_RIGHT };
            else if (sourceDirection == GoRogue.Direction.LEFT)
                skipped = new GoRogue.Direction[] { sourceDirection, GoRogue.Direction.UP_LEFT, GoRogue.Direction.DOWN_LEFT };
            else if (sourceDirection == GoRogue.Direction.UP)
                skipped = new GoRogue.Direction[] { sourceDirection, GoRogue.Direction.UP_RIGHT, GoRogue.Direction.UP_LEFT };
            else
                skipped = new GoRogue.Direction[] { sourceDirection, GoRogue.Direction.DOWN_RIGHT, GoRogue.Direction.DOWN_LEFT };

            for (int i = 0; i < index.Length; i++)
            {
                if (skipped[0] == index[i] || skipped[1] == index[i] || skipped[2] == index[i])
                    continue;

                //if (index[i] == -1)
                //    return false;

                if (!map.Bounds().Contains(location + index[i]) || map[location + index[i]])
                    return false;
            }

            return true;
        }

        private static GoRogue.Coord FindEmptySquare(IMapView<bool> map)
        {
            // Try random positions first
            for (int i = 0; i < 100; i++)
            {
                var location = map.RandomPosition(false);

                if (IsPointConsideredEmpty(map, location))
                    return location;
            }

            // Start looping through every single one
            for (int i = 0; i < map.Width * map.Height; i++)
            {
                var location = GoRogue.Coord.Get(i % map.Width, i / map.Width);

                if (IsPointConsideredEmpty(map, location))
                    return location;
            }

            return null;
        }

        static int GetDirectionIndex(bool[] valids)
        {
            // 10 tries to find random ok valid
            bool randomSuccess = false;
            int tempDirectionIndex = 0;

            for (int randomCounter = 0; randomCounter < 10; randomCounter++)
            {
                tempDirectionIndex = GoRogue.Random.SingletonRandom.DefaultRNG.Next(4);
                if (valids[tempDirectionIndex])
                {
                    randomSuccess = true;
                    break;
                }
            }

            // Couldn't find an active valid, so just run through each
            if (!randomSuccess)
            {
                if (valids[0])
                    tempDirectionIndex = 0;
                else if (valids[1])
                    tempDirectionIndex = 1;
                else if (valids[2])
                    tempDirectionIndex = 2;
                else
                    tempDirectionIndex = 3;
            }

            return tempDirectionIndex;
        }
    }

    public class DungeonMaze
    {
        public class GenerationSettings
        {
            public int MaxRooms = 15;
            public int MinRooms = 10;
            public int ChanceRemoveRooms = 20;
            public int MinRoomSize = 3;
            public int MaxRoomSize = 11;
            public int ChanceStopRooms = 15;
            public int CrawlerChangeDirectionImprovement = 30;
            public int CrawlerSaveDeadEndsPercent = 20;
            public int RoomConnectionMaxSides = 4;
            public int RoomConnectionMinSides = 1;
            public int RoomConnectionSideCancelPercent = 50;
            public int RoomConnectionSidePlacementPercent = 20;

            public Point FontRatio = new Point(1, 2);
            public bool SquareFont = false;
        }

        protected class Crawler
        {
            public Stack<Point> Path = new Stack<Point>();
            public List<Point> AllPositions = new List<Point>();
            public Point CurrentPosition = Point.Zero;
            public bool IsActive = true;
            public SadConsole.Directions.DirectionEnum Facing;

            public void MoveTo(Point position)
            {
                Path.Push(position);
                AllPositions.Add(position);
                CurrentPosition = position;
            }

            public void Backtrack()
            {
                if (Path.Count != 0)
                    CurrentPosition = Path.Pop();
            }
        }

        public GenerationSettings Settings = new GenerationSettings();

        protected SimpleMap map;

        protected List<Crawler> Crawlers;

        protected List<(Region Region, Point[][] Connections)> RoomHallwayConnections;

        public virtual void Build(ref SimpleMap map)
        {
            this.map = map;
            Crawlers = new List<Crawler>();
            
            BuildRooms();

            RoomHallwayConnections = new List<(Region Region, Point[][] Connections)>(map.Regions.Count);

            StartMazeGenerator();
            ConnectRooms();
            TrimDeadPaths();
        }

        protected void BuildRooms()
        {
            int failedTriesMax = 50;
            int failedTries = 0;
            int roomCounter = 0;

            while (failedTries != failedTriesMax)
            {
                // Generate the room
                int size = SadConsole.Global.Random.Next(Settings.MinRoomSize, Settings.MaxRoomSize + 1);
                int width = size;
                int height = size;
                int adjustment = SadConsole.Global.Random.Next(0, (size / 2) + 1);
                int adjustmentChance = SadConsole.Global.Random.Next(0, 5);

                if (adjustmentChance <= 1)
                    width += adjustment;
                else if (adjustmentChance <= 3)
                    height += adjustment / 2;

                
                //Point center = new Point((width / 2) + 1, (height / 2) + 1);

                width = width / Settings.FontRatio.X;
                height = height / Settings.FontRatio.Y;

                if (width % 2 == 0)
                    width += 1;

                if (height % 2 == 0)
                    height += 1;

                width = Math.Max(Settings.MinRoomSize, width);
                height = Math.Max(Settings.MinRoomSize, height);


                Region room = new Region();
                room.InnerRect = new Rectangle(0, 0, width, height);
                bool isPlaced = false;

                // find a place
                for (int placementCounter = 50; placementCounter > 0; placementCounter--)
                {
                    bool intersected = false;
                    room.InnerRect.Location = new Point(SadConsole.Global.Random.Next(1, map.Width - room.InnerRect.Width), SadConsole.Global.Random.Next(1, map.Height - room.InnerRect.Height));

                    foreach (var existingRoom in map.Regions)
                    {
                        if (existingRoom.OuterRect.Intersects(room.InnerRect))
                        {
                            intersected = true;
                            break;
                        }
                    }

                    if (intersected)
                        continue;
                    else
                    {
                        isPlaced = true;
                        room.OuterRect = room.InnerRect;
                        room.OuterRect.Inflate(1, 1);
                        break;
                    }
                }

                if (!isPlaced)
                {
                    failedTries += 1;
                    continue;
                }

                // room found valid location, carve it
                for (int x = 0; x < room.InnerRect.Width; x++)
                {
                    for (int y = 0; y < room.InnerRect.Height; y++)
                    {
                        Point position = new Point(x + room.InnerRect.X, y + room.InnerRect.Y);
                        map[position.X, position.Y] = Tile.Factory.Create("floor");
                        if (room.IsLit)
                            map[position.X, position.Y].Flags = Helpers.SetFlag(map[position.X, position.Y].Flags, (int)TileFlags.RegionLighted);

                        room.InnerPoints.Add(position);
                    }
                }

                for (int x = 1; x < room.OuterRect.Width - 1; x++)
                {
                    room.OuterPoints.Add(new Point(x + room.OuterRect.X, room.OuterRect.Top));
                    room.OuterPoints.Add(new Point(x + room.OuterRect.X, room.OuterRect.Bottom - 1));
                }

                for (int y = 1; y < room.OuterRect.Height - 1; y++)
                {
                    room.OuterPoints.Add(new Point(room.OuterRect.Left, y + room.OuterRect.Y));
                    room.OuterPoints.Add(new Point(room.OuterRect.Right - 1, y + room.OuterRect.Y));
                }

                room.OuterPoints.Add(room.OuterRect.Location);
                room.OuterPoints.Add(room.OuterRect.Location + room.OuterRect.Size + new Point(-1, -1));
                room.OuterPoints.Add(new Point(room.OuterRect.Right - 1, room.OuterRect.Top));
                room.OuterPoints.Add(new Point(room.OuterRect.Left, room.OuterRect.Bottom - 1));
                
                roomCounter += 1;
                map.Regions.Add(room);

                if (roomCounter == Settings.MaxRooms)
                    break;

                if (roomCounter > Settings.MinRooms)
                {
                    if (PercentageCheck(Settings.ChanceStopRooms))
                        break;
                    //var result = SadConsole.Global.Random.NextDouble();
                    //if (result < Settings.ChanceStopRooms)
                    //    break;
                }

            }

        }

        protected void StartMazeGenerator()
        {
            var empty = FindEmptySquare();
            int randomCounter = 0;
            bool randomSuccess = false;

            while (empty.HasValue)
            {
                Crawler crawler = new Crawler();
                Crawlers.Add(crawler);
                crawler.MoveTo(empty.Value);
                bool startedCrawler = true;
                int percentChangeDirection = 0;
                Color randomCrawlerColor = Color.AliceBlue.GetRandomColor(SadConsole.Global.Random);
                while (crawler.Path.Count != 0)
                {
                    // Dig this position
                    map[crawler.CurrentPosition.X, crawler.CurrentPosition.Y] = Tile.Factory.Create("floor");
                    //map[crawler.CurrentPosition.X, crawler.CurrentPosition.Y].Background = randomCrawlerColor;

                    // Get valid directions (basically is any position outside the map or not?)
                    var valids = SadConsole.Directions.GetValidDirections(crawler.CurrentPosition, map.Width, map.Height);
                    var points = SadConsole.Directions.GetDirectionPoints(crawler.CurrentPosition);

                    // Rule out any valids based on their position.
                    // Only process NSEW, do not use diagonals
                    for (int i = 0; i < 4; i++)
                        if (valids[i])
                            valids[i] = IsPointWallsExceptSource(points[i], Directions.GetOpposite((Directions.DirectionEnum)i));

                    // If not a new crawler, exclude where we came from
                    if (!startedCrawler)
                        valids[(int)Directions.GetOpposite(crawler.Facing)] = false;

                    // Do we have any valid direction to go?
                    if (valids[0] || valids[1] || valids[2] || valids[3])
                    {
                        int index = 0;

                        // Are we just starting this crawler? OR Is the current crawler facing direction invalid?
                        if (startedCrawler || valids[(int)crawler.Facing] == false)
                        {
                            // Just get anything
                            index = GetDirectionIndex(valids);
                            crawler.Facing = (Directions.DirectionEnum)index;
                            percentChangeDirection = 0;
                            startedCrawler = false;
                        }
                        else
                        {
                            // Increase probablity we change direction
                            percentChangeDirection += Settings.CrawlerChangeDirectionImprovement;

                            if (PercentageCheck(percentChangeDirection))
                            {
                                index = GetDirectionIndex(valids);
                                crawler.Facing = (Directions.DirectionEnum)index;
                                percentChangeDirection = 0;
                            }
                            else
                                index = (int)crawler.Facing;
                        }

                        crawler.MoveTo(points[index]);
                    }
                    else
                        crawler.Backtrack();
                }

                empty = FindEmptySquare();
            }
        }

        private void ConnectRooms()
        {
            /*
            - Get all points along a side
            - if point count for side is > 0
              - mark side for placement
            - if total sides marked > max
              - loop total sides > max
                - randomly remove side
            - if total sides marked > min
              - loop sides
                - CHECK side placement cancel check OK
                  - unmark side
                - if total sides marked == min
                  -break loop
            - Loop sides
              - Loop points
                - If point passes availability (no already chosen point next to point)
                  - CHECK point placement OK
                    - Add point to list
            */

            for (int r = 0; r < map.Regions.Count; r++)
            {
                var region = map.Regions[r];

                // Get all points along each side
                List<Point>[] validPoints = new List<Point>[4];

                validPoints[0] = new List<Point>();
                validPoints[1] = new List<Point>();
                validPoints[2] = new List<Point>();
                validPoints[3] = new List<Point>();

                // Along top/bottom edges
                for (int x = 1; x < region.OuterRect.Width - 1; x++)
                {
                    var point = region.OuterRect.Location;
                    point.X += x;
                    var testPoint = point + Directions.North;

                    // Top
                    if (!IsPointMapEdge(testPoint) && !IsPointWall(testPoint))
                        validPoints[(int) SadConsole.Directions.DirectionEnum.North].Add(point);

                    point = region.OuterRect.Location;
                    point.Y += region.OuterRect.Height - 1;
                    point.X += x;
                    testPoint = point + Directions.South;

                    // Bottom
                    if (!IsPointMapEdge(testPoint) && !IsPointWall(testPoint))
                        validPoints[(int) SadConsole.Directions.DirectionEnum.South].Add(point);
                }

                // Along the left/right edges
                for (int y = 1; y < region.OuterRect.Height - 1; y++)
                {
                    var point = region.OuterRect.Location;
                    point.Y += y;
                    var testPoint = point + Directions.West;

                    // Left
                    if (!IsPointMapEdge(testPoint) && !IsPointWall(testPoint))
                        validPoints[(int) SadConsole.Directions.DirectionEnum.East].Add(point);

                    point = region.OuterRect.Location;
                    point.X += region.OuterRect.Width - 1;
                    point.Y += y;
                    testPoint = point + Directions.East;

                    // Right
                    if (!IsPointMapEdge(testPoint) && !IsPointWall(testPoint))
                        validPoints[(int) SadConsole.Directions.DirectionEnum.West].Add(point);
                }

                // - if point count for side is > 0, it's a valid side.
                bool[] validSides = new bool[4];
                var sidesTotal = 0;

                for (int i = 0; i < 4; i++)
                {
                    // - mark side for placement
                    validSides[i] = validPoints[i].Count != 0;
                    if (validSides[i])
                        sidesTotal++;
                }


                // - if total sides marked > max
                if (sidesTotal > Settings.RoomConnectionMaxSides)
                {
                    var sides = new List<int>(sidesTotal);

                    for (var i = 0; i < 4; i++)
                    {
                        if (validSides[i])
                            sides.Add(i);
                    }

                    // - loop total sides > max
                    while (sidesTotal > Settings.RoomConnectionMaxSides)
                    {
                        // - randomly remove side
                        var index = sides[SingletonRandom.DefaultRNG.Next(sides.Count)];
                        sides.RemoveAt(index);
                        validSides[index] = false;
                        sidesTotal--;
                    }
                }

                // - if total sides marked > min
                if (sidesTotal > Settings.RoomConnectionMinSides)
                {
                    // - loop sides
                    for (var i = 0; i < 4; i++)
                    {
                        if (validSides[i])
                        {
                            // - CHECK side placement cancel check OK
                            if (PercentageCheck(Settings.RoomConnectionSideCancelPercent))
                            {
                                validSides[i] = false;
                                sidesTotal--;
                            }
                        }

                        // - if total sides marked == min
                        if (sidesTotal == Settings.RoomConnectionMinSides)
                            break;
                    }
                }

                List<Point>[] finalConnectionPoints = new List<Point>[4];
                finalConnectionPoints[0] = new List<Point>();
                finalConnectionPoints[1] = new List<Point>();
                finalConnectionPoints[2] = new List<Point>();
                finalConnectionPoints[3] = new List<Point>();

                // - loop sides
                for (var i = 0; i < 4; i++)
                {
                    if (validSides[i])
                    {
                        // FORNOW just randomly choose one from the side to become a connection point
                        var randomPoint = SingletonRandom.DefaultRNG.Next(validPoints[i].Count);

                        finalConnectionPoints[i].Add(validPoints[i][randomPoint]);

                        map[finalConnectionPoints[i][0]] = Tile.Factory.Create("floor");

                        // - Loop points
                        foreach (var point in validPoints[i])
                        {
                            //// - If point passes availability (no already chosen point next to point)
                            ////   - CHECK point placement OK
                            ////     - Add point to list


                            //int doorCount = SadConsole.Global.Random.Next(1, 6);
                            //bool placedDoor = false;

                            //if (room.Connections.Length != 0)
                            //    //System.Diagnostics.Debugger.Break();
                            //    //else
                            //    for (int i = 0; i < doorCount; i++)
                            //    {
                            //        for (int tryCount = 0; tryCount < 3; tryCount++)
                            //        {
                            //            Point spot = room.Connections[SadConsole.Global.Random.Next(0, room.Connections.Length)];

                            //            if (!IsPointNearType(spot, BasicTutorial.Maps.TileTypes.Door))
                            //            {
                            //                placedDoor = true;
                            //                map[spot] = Tile.Factory.Create("door");

                            //                break;
                            //            }
                            //        }

                            //        if (placedDoor && PercentageCheck(CancelDoorPlacementPercent))
                            //            break;
                            //    }
                        }
                    }
                }

                if (finalConnectionPoints[0].Count == 0
                    && finalConnectionPoints[1].Count == 0
                    && finalConnectionPoints[2].Count == 0
                    && finalConnectionPoints[3].Count == 0)
                System.Diagnostics.Debugger.Break();

                RoomHallwayConnections.Add((region, finalConnectionPoints.Select(l => l.ToArray()).ToArray()));
            }

        }

        protected void TrimDeadPaths()
        {

            foreach (var crawler in Crawlers)
            {
                List<Point> safeDeadEnds = new List<Point>();
                List<Point> deadEnds = new List<Point>();

                while (true)
                {
                    foreach (var point in crawler.AllPositions)
                    {
                        var valids = SadConsole.Directions.GetValidDirections(point, map.Width, map.Height);
                        var points = SadConsole.Directions.GetDirectionPoints(point);

                        for (int i = 0; i < 4; i++)
                        {
                            if (map[points[i]].Type == Tile.TileTypeFloor)
                            {
                                Directions.DirectionEnum oppDir = ((Directions.DirectionEnum)i).GetOpposite();
                                bool found = false;

                                if (map[points[(int)oppDir]].Type == Tile.TileTypeWall)
                                {
                                    switch (oppDir)
                                    {
                                        case Directions.DirectionEnum.North:
                                            found = map[points[(int)Directions.DirectionEnum.NorthWest]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.NorthEast]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.West]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.East]].Type == Tile.TileTypeWall;
                                            break;

                                        case Directions.DirectionEnum.South:
                                            found = map[points[(int)Directions.DirectionEnum.SouthWest]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.SouthEast]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.West]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.East]].Type == Tile.TileTypeWall;
                                            break;

                                        case Directions.DirectionEnum.East:
                                            found = map[points[(int)Directions.DirectionEnum.SouthEast]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.NorthEast]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.North]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.South]].Type == Tile.TileTypeWall;
                                            break;

                                        case Directions.DirectionEnum.West:
                                            found = map[points[(int)Directions.DirectionEnum.SouthWest]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.NorthWest]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.North]].Type == Tile.TileTypeWall &&
                                                    map[points[(int)Directions.DirectionEnum.South]].Type == Tile.TileTypeWall;
                                            break;
                                    }
                                }

                                if (found)
                                    deadEnds.Add(point);

                                break;
                            }
                        }

                    }

                    deadEnds = new List<Point>(deadEnds.Except(safeDeadEnds));
                    crawler.AllPositions = new List<Point>(crawler.AllPositions.Except(deadEnds));

                    if (deadEnds.Count == 0)
                        break;

                    foreach (var point in deadEnds)
                    {
                        if (PercentageCheck(Settings.CrawlerSaveDeadEndsPercent))
                        {
                            safeDeadEnds.Add(point);
                        }
                        else
                            map[point] = Tile.Factory.Create("wall");
                    }

                    deadEnds.Clear();
                }
            }
        }


        protected int GetDirectionIndex(bool[] valids)
        {
            // 10 tries to find random ok valid
            bool randomSuccess = false;
            int tempDirectionIndex = 0;

            for (int randomCounter = 0; randomCounter < 10; randomCounter++)
            {
                tempDirectionIndex = SadConsole.Global.Random.Next(4);
                if (valids[tempDirectionIndex])
                {
                    randomSuccess = true;
                    break;
                }
            }

            // Couldn't find an active valid, so just run through each
            if (!randomSuccess)
            {
                if (valids[0])
                    tempDirectionIndex = 0;
                else if (valids[1])
                    tempDirectionIndex = 1;
                else if (valids[2])
                    tempDirectionIndex = 2;
                else
                    tempDirectionIndex = 3;
            }

            return tempDirectionIndex;
        }

        protected Point? FindEmptySquare()
        {
            // we want a square that is odd.
            for (int i = 0; i < map.Width * map.Height; i++)
            {
                Point location = new Point(i % map.Width, i / map.Width);

                if (IsPointConsideredEmpty(location))
                {
                    return location;
                }
            }

            return null;
        }

        protected bool IsPointWallsExceptSource(Point location, Directions.DirectionEnum sourceDir)
        {
            // Shortcut out if this location is part of the map edge.
            if (IsPointMapEdge(location) || IsPointPartOfRegion(location, true))
                return false;

            // Get map indexes for all surrounding locations
            var index = location.GetDirectionIndexes(map.Width, map.Height);

            int[] skipped;

            if (sourceDir == Directions.DirectionEnum.East)
                skipped = new int[] { (int)sourceDir, (int)Directions.DirectionEnum.NorthEast, (int)Directions.DirectionEnum.SouthEast };
            else if (sourceDir == Directions.DirectionEnum.West)
                skipped = new int[] { (int)sourceDir, (int)Directions.DirectionEnum.NorthWest, (int)Directions.DirectionEnum.SouthWest };
            else if (sourceDir == Directions.DirectionEnum.North)
                skipped = new int[] { (int)sourceDir, (int)Directions.DirectionEnum.NorthEast, (int)Directions.DirectionEnum.NorthWest };
            else
                skipped = new int[] { (int)sourceDir, (int)Directions.DirectionEnum.SouthEast, (int)Directions.DirectionEnum.SouthWest };

            for (int i = 0; i < index.Length; i++)
            {
                if (skipped[0] == i || skipped[1] == i || skipped[2] == i)
                    continue;

                if (index[i] == -1)
                    return false;

                if (map[index[i]].Type != Tile.TileTypeWall)
                    return false;
            }

            return true;
        }

        protected bool IsPointConsideredEmpty(Point location)
        {
            return  !IsPointMapEdge(location) &&  // exclude outer ridge of map
                    IsPointOdd(location) && // check is odd numer position
                    !IsPointPartOfRegion(location, true) && // check if not part of a room region
                    IsPointSurroundedByWall(location) && // make sure is surrounded by a wall.
                    IsPointWall(location); // The location is a wall
        }

        protected bool IsPointOdd(Point location)
        {
            return location.X % 2 != 0 && location.Y % 2 != 0;
        }

        protected bool IsPointMapEdge(Point location, bool onlyEdgeTest = false)
        {
            if (onlyEdgeTest)
                return location.X == 0 || location.X == map.Width - 1 || location.Y == 0 || location.Y == map.Height - 1;
            else
                return location.X <= 0 || location.X >= map.Width - 1 || location.Y <= 0 || location.Y >= map.Height - 1;

        }

        protected bool IsPointPartOfRegion(Point location, bool includeOuterEdge)
        {
            Rectangle outerEdge;

            foreach (var region in map.Regions)
            {
                if (includeOuterEdge)
                {
                    outerEdge = region.InnerRect;
                    outerEdge.Inflate(1, 1);

                    if (outerEdge.Contains(location))
                        return true;
                }
                else if (region.InnerRect.Contains(location))
                    return true;
            }

            return false;
        }

        protected bool IsPointSurroundedByWall(Point location)
        {
            var index = location.GetDirectionIndexes(map.Width, map.Height);

            for (int i = 0; i < index.Length; i++)
            {
                if (index[i] == -1)
                    return false;

                if (map[index[i]].Type != Tile.TileTypeWall)
                    return false;
            }

            return true;
        }

        protected bool IsPointWall(Point location)
        {
            return map[location].Type == Tile.TileTypeWall;
        }

        protected bool IsPointNearType(Point location, int type, bool includeDiagonal = false)
        {
            Point[] points = location.GetDirectionPoints();
            bool[] validPoints = location.GetValidDirections(map.Width, map.Height);

            if (!includeDiagonal)
            {
                return (validPoints[0] ? map[points[0]].Type == type : false) ||
                       (validPoints[1] ? map[points[1]].Type == type : false) ||
                       (validPoints[2] ? map[points[2]].Type == type : false) ||
                       (validPoints[3] ? map[points[3]].Type == type : false);
            }
            else
            {
                return (validPoints[0] ? map[points[0]].Type == type : false) ||
                       (validPoints[1] ? map[points[1]].Type == type : false) ||
                       (validPoints[2] ? map[points[2]].Type == type : false) ||
                       (validPoints[3] ? map[points[3]].Type == type : false) ||
                       (validPoints[4] ? map[points[4]].Type == type : false) ||
                       (validPoints[5] ? map[points[5]].Type == type : false) ||
                       (validPoints[6] ? map[points[6]].Type == type : false) ||
                       (validPoints[7] ? map[points[7]].Type == type : false);
            }
        }

        protected bool PercentageCheck(int outOfHundred) => outOfHundred != 0 && SingletonRandom.DefaultRNG.Next(101) < outOfHundred;
        protected bool PercentageCheck(double outOfHundred) => outOfHundred != 0d && SingletonRandom.DefaultRNG.NextDouble() < outOfHundred;
    }
}
