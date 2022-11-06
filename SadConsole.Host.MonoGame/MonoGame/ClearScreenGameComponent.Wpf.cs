using Microsoft.Xna.Framework;
using SadRogue.Primitives;
using MonoGame.Framework.WpfInterop;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Host
{
    public partial class Game
    {
        public class ClearScreenGameComponent : WpfDrawableGameComponent
        {
            internal ClearScreenGameComponent(Game game) : base(game) => DrawOrder = 0;

            public override void Draw(GameTime gameTime)
            {
                Global.GraphicsDeviceWpfControl = (RenderTarget2D)GraphicsDevice.GetRenderTargets()[0].RenderTarget;
                Game.GraphicsDevice.Clear(SadConsole.Settings.ClearColor.ToMonoColor());
            }
        }
    }
}
