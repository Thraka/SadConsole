using System;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!")
            //Console.ReadKey();


            var game = new SadConsole.Game();
            game.Create(10, 10, Init);
            game.Run();


        }

        private static void Init()
        {
            SadConsole.Global.Screen.Print(1, 1, "Hello from SadConsole");
        }
    }
}
