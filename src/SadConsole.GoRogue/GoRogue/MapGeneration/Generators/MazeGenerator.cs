using System.Collections.Generic;
using System.Linq;
using GoRogue.MapViews;
using GoRogue.Random;

namespace GoRogue.MapGeneration.Generators
{
    public static class MazeGenerator
    {
        public static void Generate(ISettableMapView<bool> map, int crawlerChangeDirectionImprovement, int saveDeadEndChance)
        {
            // Implemented the logic from http://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/

            var crawlers = new List<Crawler>();

            var empty = FindEmptySquare(map);
            var randomCounter = 0;
            var randomSuccess = false;

            while (empty != null)
            {
                Crawler crawler = new Crawler();
                crawlers.Add(crawler);
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
                    var points = AdjacencyRule.CARDINALS.NeighborsClockwise(crawler.CurrentPosition).ToArray();
                    var directions = AdjacencyRule.CARDINALS.DirectionsOfNeighborsClockwise(Direction.NONE).ToList();
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

            TrimDeadPaths(map, crawlers, saveDeadEndChance);
        }

        static void TrimDeadPaths(ISettableMapView<bool> map, IEnumerable<Crawler> crawlers, int saveDeadEndChance)
        {
            foreach (var crawler in crawlers)
            {
                List<Coord> safeDeadEnds = new List<Coord>();
                List<Coord> deadEnds = new List<Coord>();

                while (true)
                {
                    foreach (var point in crawler.AllPositions)
                    {
                        var points = AdjacencyRule.EIGHT_WAY.NeighborsClockwise(point).ToArray();
                        var directions = AdjacencyRule.EIGHT_WAY.DirectionsOfNeighborsClockwise(Direction.NONE).ToList();

                        for (int i = 0; i < 8; i += 2)
                        {
                            if (map[points[i]])
                            {
                                var oppDir = directions[i] + 4;
                                bool found = false;

                                // If we get here, source direction is a floor, opposite direction should be wall
                                if (!map[points[(int)oppDir.Type]])
                                {
                                    switch (oppDir.Type)
                                    {
                                        // Check for a wall pattern in the map. In this case something like where X is a wall
                                        // XXX
                                        // X X
                                        case Direction.Types.UP:
                                            found = !map[points[(int)Direction.Types.UP_LEFT]] &&
                                                    !map[points[(int)Direction.Types.UP_RIGHT]] &&
                                                    !map[points[(int)Direction.Types.LEFT]] &&
                                                    !map[points[(int)Direction.Types.RIGHT]];
                                            break;

                                        case Direction.Types.DOWN:
                                            found = !map[points[(int)Direction.Types.DOWN_LEFT]] &&
                                                    !map[points[(int)Direction.Types.DOWN_RIGHT]] &&
                                                    !map[points[(int)Direction.Types.LEFT]] &&
                                                    !map[points[(int)Direction.Types.RIGHT]];
                                            break;

                                        case Direction.Types.RIGHT:
                                            found = !map[points[(int)Direction.Types.UP_RIGHT]] &&
                                                    !map[points[(int)Direction.Types.DOWN_RIGHT]] &&
                                                    !map[points[(int)Direction.Types.UP]] &&
                                                    !map[points[(int)Direction.Types.DOWN]];
                                            break;

                                        case Direction.Types.LEFT:
                                            found = !map[points[(int)Direction.Types.UP_LEFT]] &&
                                                    !map[points[(int)Direction.Types.DOWN_LEFT]] &&
                                                    !map[points[(int)Direction.Types.UP]] &&
                                                    !map[points[(int)Direction.Types.DOWN]];
                                            break;
                                    }
                                }


                                if (found)
                                    deadEnds.Add(point);

                                break;
                            }
                        }

                    }

                    deadEnds = new List<Coord>(deadEnds.Except(safeDeadEnds));
                    crawler.AllPositions = new List<Coord>(crawler.AllPositions.Except(deadEnds));

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

        static bool PercentageCheck(int outOfHundred) => outOfHundred > 0 && SingletonRandom.DefaultRNG.Next(101) < outOfHundred;
        static bool PercentageCheck(double outOfHundred) => outOfHundred > 0d && SingletonRandom.DefaultRNG.NextDouble() < outOfHundred;

        class Crawler
        {
            public Stack<Coord> Path = new Stack<Coord>();
            public List<Coord> AllPositions = new List<Coord>();
            public Coord CurrentPosition = Coord.Get(0, 0);
            public bool IsActive = true;
            public Direction Facing = Direction.UP;

            public void MoveTo(Coord position)
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

        static bool IsPointSurroundedByWall(IMapView<bool> map, Coord location)
        {
            var points = AdjacencyRule.EIGHT_WAY.Neighbors(location);

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

        static bool IsPointConsideredEmpty(IMapView<bool> map, Coord location)
        {
            return !IsPointMapEdge(map, location) &&  // exclude outer ridge of map
                   IsPointOdd(location) && // check is odd numer position
                                           //!IsPointPartOfRegion(map, location, true) && // check if not part of a room region
                   IsPointSurroundedByWall(map, location) && // make sure is surrounded by a wall.
                   IsPointWall(map, location); // The location is a wall
        }

        static bool IsPointWall(IMapView<bool> map, Coord location)
        {
            return !map[location];
        }

        // this is old code from sadconsole that operated on rooms (region) which is not represented in this generator.
        static bool IsPointPartOfRegion(IMapView<bool> map, Coord location, bool includeOuterEdge)
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

        static bool IsPointOdd(Coord location)
        {
            return location.X % 2 != 0 && location.Y % 2 != 0;
        }

        static bool IsPointMapEdge(IMapView<bool> map, Coord location, bool onlyEdgeTest = false)
        {
            if (onlyEdgeTest)
                return location.X == 0 || location.X == map.Width - 1 || location.Y == 0 || location.Y == map.Height - 1;
            return location.X <= 0 || location.X >= map.Width - 1 || location.Y <= 0 || location.Y >= map.Height - 1;

        }

        static bool IsPointWallsExceptSource(IMapView<bool> map, Coord location, Direction sourceDirection)
        {
            // exclude the outside of the map
            var mapInner = map.Bounds().Expand(-1, -1);

            if (!mapInner.Contains(location))
                // Shortcut out if this location is part of the map edge.
                //if (IsPointMapEdge(location) || IsPointPartOfRegion(location, true))
                return false;

            // Get map indexes for all surrounding locations
            var index = AdjacencyRule.EIGHT_WAY.DirectionsOfNeighborsClockwise().ToArray();

            Direction[] skipped;

            if (sourceDirection == Direction.RIGHT)
                skipped = new[] { sourceDirection, Direction.UP_RIGHT, Direction.DOWN_RIGHT };
            else if (sourceDirection == Direction.LEFT)
                skipped = new[] { sourceDirection, Direction.UP_LEFT, Direction.DOWN_LEFT };
            else if (sourceDirection == Direction.UP)
                skipped = new[] { sourceDirection, Direction.UP_RIGHT, Direction.UP_LEFT };
            else
                skipped = new[] { sourceDirection, Direction.DOWN_RIGHT, Direction.DOWN_LEFT };

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

        private static Coord FindEmptySquare(IMapView<bool> map)
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
                var location = Coord.Get(i % map.Width, i / map.Width);

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
                tempDirectionIndex = SingletonRandom.DefaultRNG.Next(4);
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
