using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterProject
{
    interface IConsoleMetadata: IConsole
    {
        ConsoleMetadata Metadata { get; }
    }

    struct ConsoleMetadata
    {
        public string Title;
        public string Summary;
    }
}
