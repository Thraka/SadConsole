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
            SadConsole.Game.Create("IBM.font", 80, 25);

            SadConsole.Game.OnInitialize = () =>
            {
                SadConsole.Global.ActiveScreen = new SadConsole.Screen();
                SadConsole.Global.ActiveScreen.Surfaces.Add(new SadConsole.TextSurface(20, 20));
                SadConsole.Global.ActiveScreen.Surfaces[0].FillWithRandomGarbage();
            };

            SadConsole.Game.GameInstance.Run();
        }
    }
}
