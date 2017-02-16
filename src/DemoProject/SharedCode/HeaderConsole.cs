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
            textSurface.DefaultBackground = Color.Transparent;
            textSurface.DefaultForeground = Theme.Yellow;
        }

        public void SetConsole(IConsoleMetadata console)
        {
            Fill(Theme.Yellow, Theme.GrayDark, 0);
            Print(1, 0, console.Metadata.Title.ToUpper(), Theme.Yellow);
            Print(1, 1, console.Metadata.Summary, Theme.Gray);
            //Print(0, 2, new string((char)223, 80), Theme.GrayDark, Color.Transparent);
        }
    }
}
