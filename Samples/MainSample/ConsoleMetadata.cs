using SadConsole;

namespace FeatureDemo
{
    internal class CustomConsole
    {
        public string Title;
        public string Summary;

        public IScreenObject Console
        {
            get;
            set;
        }

        public CustomConsole(IScreenObject console, string title, string summary)
        {
            Console = console;
            Title = title;
            Summary = summary;
        }
    }
}
