using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public partial class Game
    {
        public class ClearScreenGameComponent : DrawableGameComponent
        {
            internal ClearScreenGameComponent(Game game) : base(game)
            {
                DrawOrder = 0;
            }

            public override void Draw(GameTime gameTime)
            {
                Game.GraphicsDevice.Clear(Settings.ClearColor);
            }
        }
    }
}
