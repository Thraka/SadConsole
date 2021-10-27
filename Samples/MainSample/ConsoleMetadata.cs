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

    class RestartableSurface : ScreenSurface, IRestartable
    {
        public RestartableSurface(int w, int h) : base(w, h) { }

        public void Restart()
        {
            Children.Clear();
            SadComponents.Clear();
            Start();
        }

        protected virtual void Start() { }
    }

    interface IRestartable
    {
        public void Restart();
    }
}
