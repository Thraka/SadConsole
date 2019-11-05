#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.DrawCalls
{
    public class DrawCallColoredRect : IDrawCall
    {
        public Rectangle Rectangle;
        public Color Shade;

        public DrawCallColoredRect(Rectangle rectangle, Color shade)
        {
            Rectangle = rectangle;
            Shade = shade;
        }

        public void Draw() => Global.SpriteBatch.Draw(Global.FontDefault.FontImage, Rectangle, Global.FontDefault.SolidGlyphRectangle, Shade);
    }
}