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
        static bool PercentageCheck(int outOfHundred) => outOfHundred > 0 && SingletonRandom.DefaultRNG.Next(101) < outOfHundred;
        static bool PercentageCheck(double outOfHundred) => outOfHundred > 0d && SingletonRandom.DefaultRNG.NextDouble() < outOfHundred;

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

        public static IEnumerable<(GoRogue.Rectangle Room, GoRogue.Coord[][] Connections)> ConnectRooms(ISettableMapView<bool> map, IEnumerable<GoRogue.Rectangle> rooms,
                                   int minSidesToConnect,
                                   int maxSidesToConnect,
                                   int cancelSideConnectionSelectChance,
                                   int cancelConnectionPlacementChance,
                                   int cancelConnectionPlacementChanceIncrease)
        {
            if (minSidesToConnect > maxSidesToConnect)
                throw new ArgumentOutOfRangeException(nameof(minSidesToConnect), "The minimum sides with connections must be less than or equal to the maximum amount of sides with connections.");


            var roomHallwayConnections = new List<(GoRogue.Rectangle Room, GoRogue.Coord[][] Connections)>();

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

            foreach (var room in rooms)
            {
                var outerRect = room.Expand(1, 1);
                var innerRect = room;

                // Get all points along each side
                List<GoRogue.Coord>[] validPoints = new List<GoRogue.Coord>[4];

                const int INDEX_UP = 0;
                const int INDEX_DOWN = 1;
                const int INDEX_RIGHT = 2;
                const int INDEX_LEFT = 3;

                validPoints[INDEX_UP] = new List<GoRogue.Coord>();
                validPoints[INDEX_DOWN] = new List<GoRogue.Coord>();
                validPoints[INDEX_RIGHT] = new List<GoRogue.Coord>();
                validPoints[INDEX_LEFT] = new List<GoRogue.Coord>();

                // Along top/bottom edges
                for (int x = 1; x < outerRect.Width - 1; x++)
                {
                    var point = outerRect.Position.Translate(x, 0);
                    var testPoint = point + GoRogue.Direction.UP;

                    // Top
                    if (!IsPointMapEdge(map, testPoint) && !IsPointWall(map, testPoint))
                        validPoints[INDEX_UP].Add(point);

                    point = outerRect.Position.Translate(x, outerRect.Height - 1);
                    testPoint = point + GoRogue.Direction.DOWN;

                    // Bottom
                    if (!IsPointMapEdge(map, testPoint) && !IsPointWall(map, testPoint))
                        validPoints[INDEX_DOWN].Add(point);
                }

                // Along the left/right edges
                for (int y = 1; y < outerRect.Height - 1; y++)
                {
                    var point = outerRect.Position.Translate(0, y);
                    var testPoint = point + GoRogue.Direction.LEFT;

                    // Left
                    if (!IsPointMapEdge(map, testPoint) && !IsPointWall(map, testPoint))
                        validPoints[INDEX_RIGHT].Add(point);

                    point = outerRect.Position.Translate(outerRect.Width - 1, y);
                    testPoint = point + GoRogue.Direction.RIGHT;

                    // Right
                    if (!IsPointMapEdge(map, testPoint) && !IsPointWall(map, testPoint))
                        validPoints[INDEX_LEFT].Add(point);
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
                if (sidesTotal > maxSidesToConnect)
                {
                    var sides = new List<int>(sidesTotal);

                    for (var i = 0; i < 4; i++)
                    {
                        if (validSides[i])
                            sides.Add(i);
                    }

                    // - loop total sides > max
                    while (sidesTotal > maxSidesToConnect)
                    {
                        // - randomly remove side
                        var index = sides[SingletonRandom.DefaultRNG.Next(sides.Count)];
                        sides.RemoveAt(index);
                        validSides[index] = false;
                        sidesTotal--;
                    }
                }

                // - if total sides marked > min
                if (sidesTotal > minSidesToConnect)
                {
                    // - loop sides
                    for (var i = 0; i < 4; i++)
                    {
                        if (validSides[i])
                        {
                            // - CHECK side placement cancel check OK
                            if (PercentageCheck(cancelSideConnectionSelectChance))
                            {
                                validSides[i] = false;
                                sidesTotal--;
                            }
                        }

                        // - if total sides marked == min
                        if (sidesTotal == minSidesToConnect)
                            break;
                    }
                }

                List<GoRogue.Coord>[] finalConnectionPoints = new List<GoRogue.Coord>[4];
                finalConnectionPoints[0] = new List<GoRogue.Coord>();
                finalConnectionPoints[1] = new List<GoRogue.Coord>();
                finalConnectionPoints[2] = new List<GoRogue.Coord>();
                finalConnectionPoints[3] = new List<GoRogue.Coord>();

                // - loop sides
                for (var i = 0; i < 4; i++)
                {
                    if (validSides[i])
                    {
                        var currentChance = cancelConnectionPlacementChance;
                        var loopMax = 100;

                        // - Loop points
                        while (loopMax != 0 && validPoints[i].Count != 0)
                        {
                            // Get point and pull it out of the list
                            var point = validPoints[i][SingletonRandom.DefaultRNG.Next(validPoints[i].Count)];
                            validPoints[i].Remove(point);

                            // - If point passes availability (no already chosen point next to point)
                            if (IsPointByTwoWalls(map, point))
                            {
                                //     - Add point to list
                                finalConnectionPoints[i].Add(point);
                                map[point] = true;
                            }

                            // Found a point, so start reducing the chance for another
                            if (finalConnectionPoints[i].Count != 0)
                            {

                                if (PercentageCheck(currentChance))
                                {
                                    break;
                                }

                                currentChance += cancelConnectionPlacementChanceIncrease;
                            }

                            loopMax--;
                        }

                        // If we went too long in the loop and nothing was selected, force one.
                        if (loopMax == 0 && finalConnectionPoints[i].Count == 0)
                        {
                            var point = validPoints[i][SingletonRandom.DefaultRNG.Next(validPoints[i].Count)];
                            finalConnectionPoints[i].Add(point);
                            map[point] = true;
                        }
                    }
                }

                if (finalConnectionPoints[0].Count == 0
                    && finalConnectionPoints[1].Count == 0
                    && finalConnectionPoints[2].Count == 0
                    && finalConnectionPoints[3].Count == 0)
                    System.Diagnostics.Debugger.Break();

                roomHallwayConnections.Add((room, finalConnectionPoints.Select(l => l.ToArray()).ToArray()));
            }

            return roomHallwayConnections;
        }

        static bool IsPointByTwoWalls(IMapView<bool> map, GoRogue.Coord location)
        {
            var points = GoRogue.AdjacencyRule.CARDINALS.Neighbors(location);
            var area = new GoRogue.Rectangle(0, 0, map.Width, map.Height);
            var counter = 0;

            foreach (var point in points)
            {
                if (area.Contains(point) && map[point] == false)
                    counter++;
            }

            return counter == 2;
        }

        static bool IsPointWall(IMapView<bool> map, GoRogue.Coord location)
        {
            return !map[location];
        }
        static bool IsPointMapEdge(IMapView<bool> map, GoRogue.Coord location, bool onlyEdgeTest = false)
        {
            if (onlyEdgeTest)
                return location.X == 0 || location.X == map.Width - 1 || location.Y == 0 || location.Y == map.Height - 1;
            else
                return location.X <= 0 || location.X >= map.Width - 1 || location.Y <= 0 || location.Y >= map.Height - 1;

        }
    }

    public static class Maze
    {
        static bool PercentageCheck(int outOfHundred) => outOfHundred > 0 && SingletonRandom.DefaultRNG.Next(101) < outOfHundred;
        static bool PercentageCheck(double outOfHundred) => outOfHundred > 0d && SingletonRandom.DefaultRNG.NextDouble() < outOfHundred;

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
}
