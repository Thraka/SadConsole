using System;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace FeatureDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //SadConsole.Settings.UseDefaultExtendedFont = true;

            SadConsole.Game.Create(80, 25);
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }
        
        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
            //SadConsole.Settings.gam.Window.Title = "DemoProject Core";

            Global.Screen.Renderer = null;
        }
    }
}
