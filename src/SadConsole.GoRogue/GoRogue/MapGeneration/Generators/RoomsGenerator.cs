using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoRogue.MapViews;
using GoRogue.Random;

namespace GoRogue.MapGeneration.Generators
{
    public static class RoomsGenerator
    {
        public static IEnumerable<Rectangle> Generate(ISettableMapView<bool> map, int minRooms, int maxRooms,
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


            var roomCounter = SingletonRandom.DefaultRNG.Next(minRooms, maxRooms + 1);
            var rooms = new List<Rectangle>(roomCounter);

            while (roomCounter != 0)
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

                    var roomInnerRect = new Rectangle(0, 0, width, height);

                    var tryCounterPlace = 10;

                    while (tryCounterPlace != 0)
                    {
                        bool intersected = false;

                        roomInnerRect = roomInnerRect.Move(Coord.Get(SingletonRandom.DefaultRNG.Next(3, map.Width - roomInnerRect.Width - 3), SingletonRandom.DefaultRNG.Next(3, map.Height - roomInnerRect.Height - 3)));

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

        public static IEnumerable<(Rectangle Room, Coord[][] Connections)> ConnectRooms(ISettableMapView<bool> map, IEnumerable<Rectangle> rooms,
                                   int minSidesToConnect,
                                   int maxSidesToConnect,
                                   int cancelSideConnectionSelectChance,
                                   int cancelConnectionPlacementChance,
                                   int cancelConnectionPlacementChanceIncrease)
        {
            if (minSidesToConnect > maxSidesToConnect)
                throw new ArgumentOutOfRangeException(nameof(minSidesToConnect), "The minimum sides with connections must be less than or equal to the maximum amount of sides with connections.");


            var roomHallwayConnections = new List<(Rectangle Room, Coord[][] Connections)>();

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
                List<Coord>[] validPoints = new List<Coord>[4];

                const int INDEX_UP = 0;
                const int INDEX_DOWN = 1;
                const int INDEX_RIGHT = 2;
                const int INDEX_LEFT = 3;

                validPoints[INDEX_UP] = new List<Coord>();
                validPoints[INDEX_DOWN] = new List<Coord>();
                validPoints[INDEX_RIGHT] = new List<Coord>();
                validPoints[INDEX_LEFT] = new List<Coord>();

                // Along top/bottom edges
                for (int x = 1; x < outerRect.Width - 1; x++)
                {
                    var point = outerRect.Position.Translate(x, 0);
                    var testPoint = point + Direction.UP;

                    // Top
                    if (!IsPointMapEdge(map, testPoint) && !IsPointWall(map, testPoint))
                        validPoints[INDEX_UP].Add(point);

                    point = outerRect.Position.Translate(x, outerRect.Height - 1);
                    testPoint = point + Direction.DOWN;

                    // Bottom
                    if (!IsPointMapEdge(map, testPoint) && !IsPointWall(map, testPoint))
                        validPoints[INDEX_DOWN].Add(point);
                }

                // Along the left/right edges
                for (int y = 1; y < outerRect.Height - 1; y++)
                {
                    var point = outerRect.Position.Translate(0, y);
                    var testPoint = point + Direction.LEFT;

                    // Left
                    if (!IsPointMapEdge(map, testPoint) && !IsPointWall(map, testPoint))
                        validPoints[INDEX_RIGHT].Add(point);

                    point = outerRect.Position.Translate(outerRect.Width - 1, y);
                    testPoint = point + Direction.RIGHT;

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

                List<Coord>[] finalConnectionPoints = new List<Coord>[4];
                finalConnectionPoints[0] = new List<Coord>();
                finalConnectionPoints[1] = new List<Coord>();
                finalConnectionPoints[2] = new List<Coord>();
                finalConnectionPoints[3] = new List<Coord>();

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
                    Debugger.Break();

                roomHallwayConnections.Add((room, finalConnectionPoints.Select(l => l.ToArray()).ToArray()));
            }

            return roomHallwayConnections;
        }

        static bool PercentageCheck(int outOfHundred) => outOfHundred > 0 && SingletonRandom.DefaultRNG.Next(101) < outOfHundred;
        static bool PercentageCheck(double outOfHundred) => outOfHundred > 0d && SingletonRandom.DefaultRNG.NextDouble() < outOfHundred;


        static bool IsPointByTwoWalls(IMapView<bool> map, Coord location)
        {
            var points = AdjacencyRule.CARDINALS.Neighbors(location);
            var area = new Rectangle(0, 0, map.Width, map.Height);
            var counter = 0;

            foreach (var point in points)
            {
                if (area.Contains(point) && map[point] == false)
                    counter++;
            }

            return counter == 2;
        }

        static bool IsPointWall(IMapView<bool> map, Coord location)
        {
            return !map[location];
        }
        static bool IsPointMapEdge(IMapView<bool> map, Coord location, bool onlyEdgeTest = false)
        {
            if (onlyEdgeTest)
                return location.X == 0 || location.X == map.Width - 1 || location.Y == 0 || location.Y == map.Height - 1;
            return location.X <= 0 || location.X >= map.Width - 1 || location.Y <= 0 || location.Y >= map.Height - 1;

        }
    }
}
