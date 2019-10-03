using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public static class Global
    {
        public static ConsoleBase Screen;
        public static Random Random { get; set; } = new Random();
    }
}
