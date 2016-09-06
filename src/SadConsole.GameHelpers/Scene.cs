using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;

namespace SadConsole.Game
{
    class Scene
    {
        public List<GameObject> Objects;

        public Console BackgroundConsole;

        public int Width { get { return BackgroundConsole.Width; } }
        public int Height { get { return BackgroundConsole.Height; } }

        public Scene(Console backConsole)
        {
            BackgroundConsole = backConsole;
            Objects = new List<GameObject>();
        }
    }
}
