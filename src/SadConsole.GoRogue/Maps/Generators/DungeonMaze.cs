using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SadConsole;
using System.Threading.Tasks;

namespace SadConsole.Maps.Generators
{
    public class DungeonMaze
    {
        public class GenerationSettings
        {
            public int MaxRooms = 50;
            public int MinRooms = 20;
            public int ChanceRemoveRooms = 20;
            public int MinRoomSize = 3;
            public int MaxRoomSize = 11;
            public int ChanceStopRooms = 15;
            public int CrawlerChangeDirectionImprovement = 30;
            public int CrawlerSaveDeadEndsPercent = 10;
            public int CancelDoorPlacementPercent = 50;

            //public float 
            public Point FontRatio = new Point(1, 2);
            public bool SquareFont = false;
        }

        private class Crawler
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

        //private Map map;

        private SimpleMap map;

        private List<Crawler> Crawlers;

        public void Build(ref SimpleMap map)
        {
            this.map = map;
            Crawlers = new List<Crawler>();

            BuildRooms();
            StartMazeGenerator();
            ConnectRooms();
            TrimDeadPaths();
        }

        private void BuildRooms()
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


                SimpleMap.Region room = new SimpleMap.Region();
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

        private void StartMazeGenerator()
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

        private void TrimDeadPaths()
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


        private int GetDirectionIndex(bool[] valids)
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

        private void ConnectRooms()
        {
            bool[] regionsFinished = new bool[map.Regions.Count];

            while (regionsFinished.Contains(false))
            {
                int selectedIndex = SadConsole.Global.Random.Next(map.Regions.Count);

                if (regionsFinished[selectedIndex])
                    continue;

                regionsFinished[selectedIndex] = true;

                var region = map.Regions[selectedIndex];

                List<Point> validSpots = new List<Point>(region.OuterRect.Width * 2 + region.OuterRect.Height * 2);

                // Along top/bottom edges
                for (int x = 1; x < region.OuterRect.Width - 1; x++)
                {
                    var point = region.OuterRect.Location;
                    point.X += x;
                    var testPoint = point + Directions.North;

                    if (!IsPointMapEdge(testPoint) && !IsPointWall(testPoint))
                        validSpots.Add(point);

                    point = region.OuterRect.Location;
                    point.Y += region.OuterRect.Height - 1;
                    point.X += x;
                    testPoint = point + Directions.South;

                    if (!IsPointMapEdge(testPoint) && !IsPointWall(testPoint))
                        validSpots.Add(point);
                }

                // Along the left/right edges
                for (int y = 1; y < region.OuterRect.Height - 1; y++)
                {
                    var point = region.OuterRect.Location;
                    point.Y += y;
                    var testPoint = point + Directions.West;

                    if (!IsPointMapEdge(testPoint) && !IsPointWall(testPoint))
                        validSpots.Add(point);

                    point = region.OuterRect.Location;
                    point.X += region.OuterRect.Width - 1;
                    point.Y += y;
                    testPoint = point + Directions.East;

                    if (!IsPointMapEdge(testPoint) && !IsPointWall(testPoint))
                        validSpots.Add(point);
                }

                int doorCount = SadConsole.Global.Random.Next(1, 6);
                bool placedDoor = false;

                if (validSpots.Count != 0)
                    //System.Diagnostics.Debugger.Break();
                    //else
                    for (int i = 0; i < doorCount; i++)
                    {
                        for (int tryCount = 0; tryCount < 3; tryCount++)
                        {
                            Point spot = validSpots[SadConsole.Global.Random.Next(0, validSpots.Count)];

                            if (!IsPointNearType(spot, Tile.TileTypeDoor))
                            {
                                placedDoor = true;
                                map[spot] = Tile.Factory.Create("door");

                                break;
                            }
                        }

                        if (placedDoor && PercentageCheck(Settings.CancelDoorPlacementPercent))
                            break;
                    }
            }
        }

        private Point? FindEmptySquare()
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

        private bool IsPointWallsExceptSource(Point location, Directions.DirectionEnum sourceDir)
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

        private bool IsPointConsideredEmpty(Point location)
        {
            return  !IsPointMapEdge(location) &&  // exclude outer ridge of map
                    IsPointOdd(location) && // check is odd numer position
                    !IsPointPartOfRegion(location, true) && // check if not part of a room region
                    IsPointSurroundedByWall(location) && // make sure is surrounded by a wall.
                    IsPointWall(location); // The location is a wall
        }

        private bool IsPointOdd(Point location)
        {
            return location.X % 2 != 0 && location.Y % 2 != 0;
        }

        private bool IsPointMapEdge(Point location, bool onlyEdgeTest = false)
        {
            if (onlyEdgeTest)
                return location.X == 0 || location.X == map.Width - 1 || location.Y == 0 || location.Y == map.Height - 1;
            else
                return location.X <= 0 || location.X >= map.Width - 1 || location.Y <= 0 || location.Y >= map.Height - 1;

        }

        private bool IsPointPartOfRegion(Point location, bool includeOuterEdge)
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

        private bool IsPointSurroundedByWall(Point location)
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
        
        private bool IsPointWall(Point location)
        {
            return map[location].Type == Tile.TileTypeWall;
        }

        private bool IsPointNearType(Point location, int type, bool includeDiagonal = false)
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

        private bool PercentageCheck(int outOfHundred)
        {
            return SadConsole.Global.Random.Next(101) < outOfHundred;
        }


    }
}
