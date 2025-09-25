using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsoleFiddle.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface _mainSurface;

    public RootScreen()
    {
        _mainSurface = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

        Children.Add(_mainSurface);

        ControlHost ch = new();

        _mainSurface.SadComponents.Add(ch);

        List<TabItem> tabs = new();
        Panel p1 = new(40, 40);
        p1.Add(new CheckBox("Hello world!") { FocusOnMouseClick = true, TabStop = true });
        p1.Add(new CheckBox("Hello world 2") { Position = new Point(0, 2), TabStop = true });
        p1.Add(new CheckBox("Hello world 3") { Position = new Point(0, 4), TabStop = true });

        tabs.Add(new TabItem("Tab 1", p1));

        TabControl tc = new(tabs, 50, 50);

        ch.Add(tc);

        p1.FocusOnMouseClick = true;
    }
}
