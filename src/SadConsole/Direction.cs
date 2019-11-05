#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    using System;

    /// <summary>
    /// Direction related types and methods.
    /// </summary>
    public static class Directions
    {
        private static Point north = new Point(0, -1);
        private static Point south = new Point(0, 1);
        private static Point west = new Point(-1, 0);
        private static Point east = new Point(1, 0);
        private static Point northwest = north + west;
        private static Point northeast = north + east;
        private static Point southwest = south + west;
        private static Point southeast = south + east;
        private static readonly Point[] points = new Point[] { north, south, west, east, northwest, northeast, southwest, southeast };

        /// <summary>
        /// Returns a point that faces north (0, -1);
        /// </summary>
        public static Point North => north;

        /// <summary>
        /// Returns a point that faces south (0, 1);
        /// </summary>
        public static Point South => south;

        /// <summary>
        /// Returns a point that faces east (1, 0);
        /// </summary>
        public static Point East => east;

        /// <summary>
        /// Returns a point that faces west (-1, 0);
        /// </summary>
        public static Point West => west;

        /// <summary>
        /// Returns a point that faces north west (-1, -1);
        /// </summary>
        public static Point NorthWest => northwest;

        /// <summary>
        /// Returns a point that faces north east (1, -1);
        /// </summary>
        public static Point NorthEast => northeast;

        /// <summary>
        /// Returns a point that faces south west (-1, 1);
        /// </summary>
        public static Point SouthWest => southwest;

        /// <summary>
        /// Returns a point that faces south east (1, 1);
        /// </summary>
        public static Point SouthEast => southeast;

        /// <summary>
        /// Returns an array of directional points that use the index from <see cref="DirectionEnum"/>.
        /// </summary>
        public static Point[] Points => points;

        /// <summary>
        /// A compass direction enumeration.
        /// </summary>
        public enum DirectionEnum : int
        {
            North = 0,
            South = 1,
            East = 2,
            West = 3,
            NorthWest = 4,
            NorthEast = 5,
            SouthWest = 6,
            SouthEast = 7
        }

        /// <summary>
        /// Returns the opposite direction of a <see cref="DirectionEnum"/> value.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The opposite direction.</returns>
        public static DirectionEnum GetOpposite(this DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.North:
                    return DirectionEnum.South;
                case DirectionEnum.South:
                    return DirectionEnum.North;
                case DirectionEnum.East:
                    return DirectionEnum.West;
                case DirectionEnum.West:
                    return DirectionEnum.East;
                case DirectionEnum.NorthWest:
                    return DirectionEnum.SouthEast;
                case DirectionEnum.NorthEast:
                    return DirectionEnum.SouthWest;
                case DirectionEnum.SouthWest:
                    return DirectionEnum.NorthEast;
                case DirectionEnum.SouthEast:
                    return DirectionEnum.NorthWest;
                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// Discards NW/NE/SW/SE directions in favor of N/S/E/W for a <see cref="DirectionEnum"/> value.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="favorNorthSouth">If true, returns N for NW/NE and S for SW/SE. Otherwise W for SW/NW and E for SE/NE.</param>
        /// <returns>The opposite direction.</returns>
        public static DirectionEnum GetCardinalDirection(this DirectionEnum direction, bool favorNorthSouth = true)
        {
            if (favorNorthSouth)
            {
                switch (direction)
                {
                    case DirectionEnum.North:
                    case DirectionEnum.NorthWest:
                    case DirectionEnum.NorthEast:
                        return DirectionEnum.North;
                    case DirectionEnum.South:
                    case DirectionEnum.SouthWest:
                    case DirectionEnum.SouthEast:
                        return DirectionEnum.South;
                    case DirectionEnum.East:
                        return DirectionEnum.East;
                    case DirectionEnum.West:
                        return DirectionEnum.West;
                    default:
                        throw new Exception();
                }
            }
            else
            {
                switch (direction)
                {
                    case DirectionEnum.North:
                        return DirectionEnum.North;
                    case DirectionEnum.South:
                        return DirectionEnum.South;
                    case DirectionEnum.NorthEast:
                    case DirectionEnum.SouthEast:
                    case DirectionEnum.East:
                        return DirectionEnum.East;
                    case DirectionEnum.NorthWest:
                    case DirectionEnum.SouthWest:
                    case DirectionEnum.West:
                        return DirectionEnum.West;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        /// Gets the direction left of the <paramref name="direction"/> value.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="useDiagonal">If true, uses NW/NE/SW/SE in considering the result. Otherwise the result will always be a N/S/E/W value.</param>
        /// <returns>The left direction.</returns>
        public static DirectionEnum TurnLeft(this DirectionEnum direction, bool useDiagonal = false)
        {
            if (useDiagonal)
            {
                switch (direction)
                {
                    case DirectionEnum.North:
                        return DirectionEnum.NorthWest;
                    case DirectionEnum.South:
                        return DirectionEnum.SouthEast;
                    case DirectionEnum.East:
                        return DirectionEnum.NorthEast;
                    case DirectionEnum.West:
                        return DirectionEnum.SouthWest;
                    case DirectionEnum.NorthWest:
                        return DirectionEnum.West;
                    case DirectionEnum.NorthEast:
                        return DirectionEnum.North;
                    case DirectionEnum.SouthWest:
                        return DirectionEnum.South;
                    case DirectionEnum.SouthEast:
                        return DirectionEnum.East;
                    default:
                        throw new Exception();
                }
            }
            else
            {
                switch (direction)
                {
                    case DirectionEnum.North:
                    case DirectionEnum.NorthWest:
                        return DirectionEnum.West;
                    case DirectionEnum.South:
                    case DirectionEnum.SouthEast:
                        return DirectionEnum.East;
                    case DirectionEnum.East:
                    case DirectionEnum.NorthEast:
                        return DirectionEnum.North;
                    case DirectionEnum.West:
                    case DirectionEnum.SouthWest:
                        return DirectionEnum.South;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        /// Gets the direction right of the <paramref name="direction"/> value.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="useDiagonal">If true, uses NW/NE/SW/SE in considering the result. Otherwise the result will always be a N/S/E/W value.</param>
        /// <returns>The right direction.</returns>
        public static DirectionEnum TurnRight(this DirectionEnum direction, bool useDiagonal = false)
        {
            if (useDiagonal)
            {
                switch (direction)
                {
                    case DirectionEnum.North:
                        return DirectionEnum.NorthEast;
                    case DirectionEnum.South:
                        return DirectionEnum.SouthWest;
                    case DirectionEnum.East:
                        return DirectionEnum.SouthEast;
                    case DirectionEnum.West:
                        return DirectionEnum.NorthWest;
                    case DirectionEnum.NorthWest:
                        return DirectionEnum.North;
                    case DirectionEnum.NorthEast:
                        return DirectionEnum.East;
                    case DirectionEnum.SouthWest:
                        return DirectionEnum.West;
                    case DirectionEnum.SouthEast:
                        return DirectionEnum.South;
                    default:
                        throw new Exception();
                }
            }
            else
            {
                switch (direction)
                {
                    case DirectionEnum.North:
                    case DirectionEnum.NorthEast:
                        return DirectionEnum.East;
                    case DirectionEnum.South:
                    case DirectionEnum.SouthWest:
                        return DirectionEnum.West;
                    case DirectionEnum.East:
                    case DirectionEnum.SouthEast:
                        return DirectionEnum.South;
                    case DirectionEnum.West:
                    case DirectionEnum.NorthWest:
                        return DirectionEnum.North;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        /// Gets a list of indexed boolean values to indicate if the direction from the <paramref name="position"/> falls in the (0, 0, <paramref name="width"/>, <paramref name="height"/>) area.
        /// </summary>
        /// <param name="position">The position to test from.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <returns>An array of bool values indicating if the direction is valid or not; indexed with the value of a <see cref="DirectionEnum"/>.</returns>
        public static bool[] GetValidDirections(this Point position, int width, int height) => GetValidDirections(position, new Rectangle(0, 0, width, height));

        /// <summary>
        /// Gets a list of indexed boolean values to indicate if the direction from the <paramref name="position"/> falls in the <paramref name="area"/>.
        /// </summary>
        /// <param name="position">The position to test from.</param>
        /// <param name="area">The area to test.</param>
        /// <returns>An array of bool values indicating if the direction is valid or not; indexed with the value of a <see cref="DirectionEnum"/>.</returns>
        public static bool[] GetValidDirections(this Point position, Rectangle area) => new bool[]
            {
                    area.Contains(position + north),
                    area.Contains(position + south),
                    area.Contains(position + east),
                    area.Contains(position + west),
                    area.Contains(position + northwest),
                    area.Contains(position + northeast),
                    area.Contains(position + southwest),
                    area.Contains(position + southeast),
            };

        /// <summary>
        /// Gets a list of indexed boolean values to indicate if the direction from the <paramref name="position"/> falls in the <paramref name="area"/>.
        /// </summary>
        /// <param name="position">The position to test from.</param>
        /// <param name="area">The area to test.</param>
        /// <returns>An array of bool values indicating if the direction is valid or not; indexed with the value of a <see cref="DirectionEnum"/>.</returns>
        public static bool[] GetValidDirections(this Rectangle area, Point position) => GetValidDirections(position, area);

        /// <summary>
        /// Gets an indexed array of direction positions based on the <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The source position.</param>
        /// <returns>An array of positions indexed with the value of a <see cref="DirectionEnum"/>.</returns>
        public static Point[] GetDirectionPoints(this Point position) => new Point[]
            {
                position + north,
                position + south,
                position + east,
                position + west,
                position + northwest,
                position + northeast,
                position + southwest,
                position + southeast,
            };

        /// <summary>
        /// Gets an array of <see cref="SadConsole.Surfaces.ISurface.Cells"/> indexes of each <see cref="DirectionEnum"/> from the current <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position center.</param>
        /// <param name="areaWidth">The width of the area.</param>
        /// <param name="areaHeight">The height of the area.</param>
        /// <returns>The index of each position. A value of -1 represents an invalid position.</returns>
        public static int[] GetDirectionIndexes(this Point position, int areaWidth, int areaHeight) => GetDirectionIndexes(position, new Rectangle(0, 0, areaWidth, areaHeight));

        /// <summary>
        /// Gets an array of <see cref="SadConsole.Surfaces.ISurface.Cells"/> indexes of each <see cref="DirectionEnum"/> from the current <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position center.</param>
        /// <param name="area">The area containing the position.</param>
        /// <returns>The index of each position. A value of -1 represents an invalid position.</returns>
        public static int[] GetDirectionIndexes(this Point position, Rectangle area)
        {
            if (area.Contains(position))
            {
                bool[] valids = GetValidDirections(position, area);
                Point[] points = GetDirectionPoints(position);

                return new int[]
                {
                valids[0] ? points[0].ToIndex(area.Width) : -1,
                valids[1] ? points[1].ToIndex(area.Width) : -1,
                valids[2] ? points[2].ToIndex(area.Width) : -1,
                valids[3] ? points[3].ToIndex(area.Width) : -1,
                valids[4] ? points[4].ToIndex(area.Width) : -1,
                valids[5] ? points[5].ToIndex(area.Width) : -1,
                valids[6] ? points[6].ToIndex(area.Width) : -1,
                valids[7] ? points[7].ToIndex(area.Width) : -1
                };
            }
            else
            {
                return new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };
            }
        }

        /// <summary>
        /// Returns a heading angle based on the direction.
        /// </summary>
        /// <param name="direction">Direction to convert.</param>
        /// <returns>An angle between 0 (east) and 315 (south east) degrees.</returns>
        public static float ToHeading(this DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.North:
                    return 90;
                case DirectionEnum.South:
                    return 270;
                case DirectionEnum.East:
                    return 0;
                case DirectionEnum.West:
                    return 180;
                case DirectionEnum.NorthWest:
                    return 135;
                case DirectionEnum.NorthEast:
                    return 45;
                case DirectionEnum.SouthWest:
                    return 225;
                case DirectionEnum.SouthEast:
                    return 315;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Returns a direction based on a heading angle
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="compensationHeading"></param>
        /// <returns></returns>
        public static DirectionEnum FromHeading(float heading, float compensationHeading = 0f)
        {
            if (compensationHeading != 0)
            {
                heading = MathHelper.Wrap(heading + (90 /* north */ - compensationHeading), 0, 359);
            }
            else
            {
                heading = MathHelper.Wrap(heading, 0, 359);
            }

            // S
            if (heading > 240 && heading < 300)
            {
                return DirectionEnum.South;
            }
            // N
            else if (heading < 120 && heading > 60)
            {
                return DirectionEnum.North;
            }
            // SW
            else if (heading >= 210 && heading <= 240)
            {
                return DirectionEnum.SouthWest;
            }
            // NE
            else if (heading <= 60 && heading >= 30)
            {
                return DirectionEnum.NorthEast;
            }
            // SE
            else if (heading >= 300 && heading <= 330)
            {
                return DirectionEnum.SouthEast;
            }
            // NW
            else if (heading <= 150 && heading >= 120)
            {
                return DirectionEnum.NorthWest;
            }
            // W
            else if (heading > 150 && heading < 210)
            {
                return DirectionEnum.West;
            }
            // E
            else if ((heading < 30 && heading >= 0) || (heading > 330 || heading <= 360))
            {
                return DirectionEnum.East;
            }
            else
            {
                throw new Exception("Heading out of range");
            }
        }
    }
}
