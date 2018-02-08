using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

#if WPF
using MonoGame_Game = MonoGame.Framework.WpfInterop.WpfGame;
using DrawableGameComponent = MonoGame.Framework.WpfInterop.WpfDrawableGameComponent;
#else
using MonoGame_Game = Microsoft.Xna.Framework.Game;
#endif

namespace SadConsole
{
    public partial class Game
    {
        public class ClearScreenGameComponent : DrawableGameComponent
        {
            internal ClearScreenGameComponent(MonoGame_Game game) : base(game)
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
