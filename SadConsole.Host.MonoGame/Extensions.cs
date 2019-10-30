using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Host.MonoGame
{
    public static class Extensions
    {
        public static Microsoft.Xna.Framework.Graphics.SpriteEffects ToMonoGame(this SadConsole.Mirror mirror)
        {
            switch (mirror)
            {
                case Mirror.None:
                    return Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                case Mirror.Vertical:
                    return Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
                case Mirror.Horizontal:
                    return Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                default:
                    return Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
            }
        }
    }
}
