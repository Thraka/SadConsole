using Microsoft.Xna.Framework;
using SadRogue.Primitives;

namespace SadConsole.MonoGame
{
    public partial class Game
    {
        public class ClearScreenGameComponent : DrawableGameComponent
        {
            internal ClearScreenGameComponent(Game game) : base(game) => DrawOrder = 0;

            public override void Draw(GameTime gameTime)
            {
                Game.GraphicsDevice.SetRenderTarget(null);
                Game.GraphicsDevice.Clear(SadConsole.Settings.ClearColor.ToMonoColor());
            }
        }
    }
}
