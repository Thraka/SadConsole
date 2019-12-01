using SadConsole;

namespace FeatureDemo
{
    internal class CustomConsole
    {
        public string Title;
        public string Summary;

        public IScreenSurface Console
        {
            get;
            set;
        }

        public CustomConsole(IScreenSurface console, string title, string summary)
        {
            Console = console;
            Title = title;
            Summary = summary;
        }
    }
}
