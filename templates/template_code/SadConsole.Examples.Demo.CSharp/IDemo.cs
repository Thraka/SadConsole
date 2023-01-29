namespace SadConsole.Examples;

internal interface IDemo
{
    string Title { get; }
    string Description { get; }
    string CodeFile { get; }

    void PostCreateDemoScreen(IScreenSurface demoScreen) { }
    IScreenSurface CreateDemoScreen();
}
