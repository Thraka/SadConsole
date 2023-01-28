using SFML.Graphics;

namespace SadConsole.DrawCalls
{
    public class DrawCallTexture : IDrawCall
    {
        public Texture Texture;
        public SFML.System.Vector2i Position;
        public Color Tint;

        public DrawCallTexture(Texture texture, SFML.System.Vector2i position, Color? tint = null)
        {
            Texture = texture;
            Position = position;

            if (tint.HasValue)
                Tint = tint.Value;
            else
                Tint = Color.White;
        }

        public void Draw() =>
            Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(Position.X, Position.Y, (int)Texture.Size.X, (int)Texture.Size.Y),
                                                   new IntRect(0, 0, (int)Texture.Size.X, (int)Texture.Size.Y),
                                                   Tint,
                                                   Texture);
    }
}
