using Microsoft.Xna.Framework;


namespace SadConsole.DrawCalls
{
    public class DrawCallScreenObject : IDrawCall
    {
        public Console Surface;
        public Vector2 Position;

        public DrawCallScreenObject(Console surface, Point position, bool pixelPositioned)
        {
            if (pixelPositioned)
                Position = position.ToVector2();
            else
                Position = surface.Font.GetWorldPosition(position).ToVector2();

            Surface = surface;
        }

        public void Draw()
        {
            Global.SpriteBatch.Draw(Surface.LastRenderResult, Position, Color.White);
        }
    }
}
