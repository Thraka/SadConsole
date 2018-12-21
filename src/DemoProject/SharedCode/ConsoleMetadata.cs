using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;
using ScrollingConsole = SadConsole.ScrollingConsole;

namespace StarterProject
{
    class CustomConsole
    {
        public string Title;
        public string Summary;

        public ScrollingConsole Console
        {
            get;
            set;
        }

        public CustomConsole(ScrollingConsole console, string title, string summary)
        {
            Console = console;
            Title = title;
            Summary = summary;
        }
    }
}
