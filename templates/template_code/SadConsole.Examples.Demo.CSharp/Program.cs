using SadConsole.Examples;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

SadConsole.Settings.WindowTitle = "SadConsole Examples";

Game.Configuration gameStartup = new Game.Configuration()
    .SetScreenSize(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
    .SetStartingScreen<RootScene>()
    .IsStartingScreenFocused(false)
    .ConfigureFonts((f) => f.UseBuiltinFontExtended())
    ;

Game.Create(gameStartup);
Game.Instance.Run();
Game.Instance.Dispose();

class othertest: ControlsConsole
{
    public othertest() : base(30, 30)
    {
        ScrollBar vert = new ScrollBar(Orientation.Vertical, 2, 10);
        vert.Position = (1, 5);
        vert.Maximum = 100;
        Controls.Add(vert);

        ScrollBar horiz = new ScrollBar(Orientation.Horizontal, 10, 2);
        horiz.Maximum = 100;
        horiz.Position = (1, 1);
        Controls.Add(horiz);
    }

}
