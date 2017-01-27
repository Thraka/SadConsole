using Microsoft.Xna.Framework;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public abstract class DrawCall
    {
        public abstract void Draw();
    }

    public class DrawCallSurface : DrawCall
    {
        public ISurface Surface;
        public Vector2 Position;

        public DrawCallSurface(ISurface surface, Vector2 position)
        {
            Surface = surface;
            Position = position;
        }

        public override void Draw()
        {
            Global.SpriteBatch.Draw(Surface.LastRenderResult, Position, Color.White);
        }
    }
}
