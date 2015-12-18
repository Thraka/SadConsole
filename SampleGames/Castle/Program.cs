using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castle
{
    internal class Program
    {
        private static CastleGame castleGame;
        public static void Main()
        {
            castleGame = new CastleGame();
            castleGame.Run();
        }
    }
}
