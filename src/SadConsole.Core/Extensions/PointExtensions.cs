using SadConsole;

#if SFML
using Point = SFML.System.Vector2i;
namespace SFML.System
#elif MONOGAME
namespace Microsoft.Xna.Framework
#endif
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

        public static Point TranslateFont(this Point point, Font sourceFont, Font targetFont)
        {
            var world = point.ConsoleLocationToWorld(sourceFont.Size.X, sourceFont.Size.Y);
            return world.WorldLocationToConsole(targetFont.Size.X, targetFont.Size.Y);
        }
    }

#if SFML
    public static class Vector2fExtensions
    {
        public static float Length(this Vector2f v)
        {
            return (float)global::System.Math.Sqrt((v.X * v.X) + (v.Y * v.Y));
        }
    }
#endif
}
