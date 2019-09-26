#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using System.Numerics;
#endif

namespace SadConsole.DrawCalls
{
    public class DrawCallTexture : IDrawCall
    {
        public Texture2D Texture;
        public Vector2 Position;

        public DrawCallTexture(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public void Draw() => Global.SpriteBatch.Draw(Texture, Position, Color.White);
    }
}