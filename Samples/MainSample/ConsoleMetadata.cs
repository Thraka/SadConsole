using Console = SadConsole.Console;

namespace FeatureDemo
{
    internal class CustomConsole
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
