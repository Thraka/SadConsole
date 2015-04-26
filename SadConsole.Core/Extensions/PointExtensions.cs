using SadConsole;

namespace Microsoft.Xna.Framework
{
    public static class PointExtensions
    {
        public static Point ConsoleLocationToWorld(this Point point, int cellWidth, int cellHeight)
        {
            return new Point(point.X * cellWidth, point.Y * cellHeight);
        }

        public static Point WorldLocationToConsole(this Point point, int cellWidth, int cellHeight)
        {
            return new Point(point.X / cellWidth, point.Y / cellHeight);
        }

        public static int ToIndex(this Point point, int areaWidth)
        {
            return point.Y * areaWidth + point.X;
        }
    }
}
