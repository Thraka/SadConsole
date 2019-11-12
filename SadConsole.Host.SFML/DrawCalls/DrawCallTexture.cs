using SFML.Graphics;

namespace SadConsole.DrawCalls
{
    public class DrawCallTexture : IDrawCall
    {
        public Texture Texture;
        public SFML.System.Vector2i Position;

        public DrawCallTexture(Texture texture, SFML.System.Vector2i position)
        {
            Texture = texture;
            Position = position;
        }

        public void Draw() =>
            Host.Global.SharedSpriteBatch.DrawQuad(new IntRect(Position.X, Position.Y, Position.X + (int)Texture.Size.X, Position.Y + (int)Texture.Size.Y),
                                                   new IntRect(0, 0, (int)Texture.Size.X, (int)Texture.Size.Y),
                                                   Color.White,
                                                   Texture);
    }
}
