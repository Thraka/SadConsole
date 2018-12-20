using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;
using Console = SadConsole.Console;

namespace StarterProject
{
    class CustomConsole
    {
        public string Title;
        public string Summary;

        public Console Console
        {
            get;
            set;
        }

        public CustomConsole(Console console, string title, string summary)
        {
            Console = console;
            Title = title;
            Summary = summary;
        }
    }
}
