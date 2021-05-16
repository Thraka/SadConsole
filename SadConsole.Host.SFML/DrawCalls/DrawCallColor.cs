using Rectangle = SFML.Graphics.IntRect;
using Color = SFML.Graphics.Color;
using SFML.Graphics;

namespace SadConsole.DrawCalls
{
    public class DrawCallColor : IDrawCall
    {
        public Texture FontTexture;
        public Rectangle FontSolidRect;
        public Color BoxColor;
        public Rectangle TargetRect;

        public DrawCallColor(Color color, Texture fontTexture, Rectangle targetRect, Rectangle fontSolidRect)
        {
            FontTexture = fontTexture;
            FontSolidRect = fontSolidRect;
            TargetRect = targetRect;
            BoxColor = color;
        }

        public void Draw()
        {
            Host.Global.SharedSpriteBatch.DrawQuad(TargetRect, FontSolidRect, BoxColor, FontTexture);
        }
    }
}
