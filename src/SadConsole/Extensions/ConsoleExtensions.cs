#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    public static class ConsoleExtensions
    {
#if XNA
        public static Matrix GetPositionTransform(this Console surface, Point position, bool usePixelPositioning = false)
        {
            Point worldLocation = usePixelPositioning ? position : new Point(position.X * surface.Font.Size.X, position.Y * surface.Font.Size.Y);
            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
        }
#else
        public static System.Numerics.Matrix3x2 GetPositionTransform(this Console surface, Point position, bool usePixelPositioning = false)
        {
            var worldLocation = usePixelPositioning ? position : new Point(position.X * surface.Font.Size.X, position.Y * surface.Font.Size.Y);
            return System.Numerics.Matrix3x2.CreateTranslation(worldLocation.ToVector2());
        }
#endif
    }
}
