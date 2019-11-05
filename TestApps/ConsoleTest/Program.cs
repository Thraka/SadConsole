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
            Game.Instance.MonoGameInstance.Components.Add(new SadConsole.MonoGame.Game.FPSCounterComponent(Game.Instance.MonoGameInstance));
            //Global.Screen.Surface.Print(1, 1, "Hello from SadConsole 9.0");
            //Global.Screen.Surface.Print(10, 15, "Hello from SadConsole 9.0", Color.AnsiCyan);
            //Global.Screen.Surface.Print(5, 18, new ColorGradient(Color.AliceBlue, Color.DarkOrange, Color.LightPink, Color.Red.GetRandomColor(Global.Random)).ToColoredString("Some color is fun to play with when you got it!"));

            var screen = new ScreenObjectSurface(new CellSurface(5, 5, 10, 10));
            screen.Surface.DefaultBackground = Color.Green;
            screen.Surface.Print(0, 0, "0123456789");
            screen.Surface.Print(0, 9, "0123456789");
            for (int i = 0; i < 10; i++)
            {
                char value = Convert.ToString(i + 1)[0];
                screen.Surface.SetGlyph(0, i + 1, value);
                screen.Surface.SetGlyph(9, i + 1, value);
                screen.Surface.SetGlyph(i + 1, i + 1, value);
            }
            screen.Surface.BufferPosition = new Point(5,5);
            screen.Surface.Width = 8;
            screen.Surface.Height = 8;
            screen.Parent = Global.Screen;
            screen.Position = new Point(1, 1);

            var surface = new CellSurface(10, 10);
            screen.Surface.Copy(surface);
            surface.DefaultBackground = Color.Blue;
            screen = new ScreenObjectSurface(surface);
            screen.Position = new Point(1, 11);
            screen.Parent = Global.Screen;

        }

    }
}
