using System;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!")
            //Console.ReadKey();

            SadConsole.Settings.UnlimitedFPS = true;
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
            ((Game)SadConsole.Game.Instance).MonoGameInstance.Components.Add(new SadConsole.MonoGame.Game.FPSCounterComponent(((Game)SadConsole.Game.Instance).MonoGameInstance));
            SadConsole.Global.Screen.Print(1, 1, "Hello from SadConsole 9.0");
            SadConsole.Global.Screen.Print(10, 10, "Hello from SadConsole 9.0", SadRogue.Primitives.Color.AnsiCyan);
            SadConsole.Global.Screen.Print(5, 18, new ColorGradient(Color.AliceBlue, Color.DarkOrange, Color.LightPink, Color.Red.GetRandomColor(Global.Random)).ToColoredString("Some color is fun to play with when you got it!"));
        }

    }
}
