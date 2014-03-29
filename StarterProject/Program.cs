using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarterProject
{
    class Program
    {
        public static Game1 Game;
        static void Main(string[] args)
        {
            Game = new Game1();
            Game.Run();
        }
    }
}