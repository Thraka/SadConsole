using System;

namespace StarterProject
{
#if WINDOWS || XBOX
    static class Program
    {
        public static Game1 Game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game = new Game1())
            {
                Game.Run();
            }
        }
    }
#endif
}

