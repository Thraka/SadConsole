using System;
using Console = SadConsole.Console;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using StarterProject.CustomConsoles;

namespace StarterProject
{
    class HeaderConsole: Console
    {
        public HeaderConsole(): base(80, 2)
        {
            DefaultBackground = Color.Transparent;
            DefaultForeground = Theme.Yellow;
        }

        public void SetConsole(string title, string summary)
        {
            Fill(Theme.Yellow, Theme.GrayDark, 0);
            Print(1, 0, title.ToUpper(), Theme.Yellow);
            Print(1, 1, summary, Theme.Gray);
            //Print(0, 2, new string((char)223, 80), Theme.GrayDark, Color.Transparent);
        }
    }
}
