using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.DrawCalls
{
    public class DrawCallTexture : IDrawCall
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Color Tint;

        public DrawCallTexture(Texture2D texture, Vector2 position, Color? tint = null)
        {
            if (texture == null) throw new System.NullReferenceException($"{nameof(texture)} cannot be null.");

            Texture = texture;
            Position = position;

            if (tint.HasValue)
                Tint = tint.Value;
            else
                Tint = Color.White;

        }

        public void Draw() =>
            Host.Global.SharedSpriteBatch.Draw(Texture, Position, Tint);
    }
}
