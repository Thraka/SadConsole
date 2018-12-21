using Microsoft.Xna.Framework;


namespace SadConsole
{
    public static class RectangleExtensions
    {
        public static void CenterViewPortOnPoint(this IScreenObjectViewPort surface, Point target)
        {
            surface.ViewPort = surface.ViewPort.CenterOnPoint(target, surface.Width, surface.Height);
        }

        public static Rectangle CenterOnPoint(this Rectangle rect, Point target, int maxWidth, int maxHeight)
        {
            var newRect = rect;
            newRect.Location = new Point(target.X - newRect.Width / 2, target.Y - newRect.Height / 2);

            if (newRect.Right > maxWidth)
                newRect.X -= newRect.Right - maxWidth;
            else if (newRect.Left < 0)
                newRect.X = 0;

            if (newRect.Bottom > maxHeight)
                newRect.Y -= newRect.Bottom - maxHeight;
            else if (newRect.Top < 0)
                newRect.Y = 0;

            return newRect;
        }
    }
}
