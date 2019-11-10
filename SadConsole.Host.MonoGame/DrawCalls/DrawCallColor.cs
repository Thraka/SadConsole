using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;

namespace SadConsole.DrawCalls
{
    public class DrawCallColor : IDrawCall
    {
        public Texture2D Texture;
        public Rectangle FontSolidRect;
        public Color Color;
        public Rectangle TargetRect;

        public DrawCallColor(Color color, Texture2D texture, Rectangle targetRect, Rectangle fontSolidRect)
        {
            Texture = texture;
            FontSolidRect = fontSolidRect;
            TargetRect = targetRect;
            Color = color;
        }

        public void Draw()
        {
            MonoGame.Global.SharedSpriteBatch.Draw(Texture, TargetRect, FontSolidRect, Color, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
        }
    }
}
