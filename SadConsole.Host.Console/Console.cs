using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public class Console: ConsoleBase
    {
        public Console(int width, int height) : base(width, height)
        {
            RenderSurface = Game.Instance.CreateSurface(width, height);
        }

        public override void Draw()
        {
            Game.Instance.AddDrawCallSurface(this, Position);
        }
    }
}
