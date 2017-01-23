using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SadConsole.Console con = null;
            SadConsole.Game.Create("IBM.font", 80, 25);

            SadConsole.Game.OnInitialize = () =>
            {
                SadConsole.Global.ActiveScreen = new SadConsole.Screen();
                SadConsole.Global.ActiveScreen.Children.Add(new SadConsole.Console(20, 20));
                con = new SadConsole.Console(20, 20) { Offset = new Microsoft.Xna.Framework.Point(30, 1) };
                SadConsole.Global.ActiveScreen.Children.Add(con);
            };
            float r = 0;
            SadConsole.Game.OnDraw = (time) =>
            {
                r -= MathHelper.ToRadians(0.5f);
                if (r < 0.0f)
                    r += MathHelper.TwoPi;

                Matrix transform = Matrix.CreateRotationZ(r) * Matrix.CreateScale(0.5f, 2.0f, 1f) * Matrix.CreateTranslation(200f, 0f, 0f);

                SadConsole.Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, transform);
                SadConsole.Global.SpriteBatch.Draw(con.TextSurface.LastRenderResult, new Vector2(0,0));
                SadConsole.Global.SpriteBatch.End();
            };

            SadConsole.Game.Instance.Run();
        }
    }
}
