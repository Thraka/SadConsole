using SadConsole.Configuration;
using SadConsole.Input;

Settings.WindowTitle = "SadConsole Examples - ZZT";

Builder startup = new Builder()
    .SetScreenSize(80, 25)
    .SetStartingScreen(game => {
        using var reader = System.IO.File.OpenRead("DEMO.ZZT");
        var world = ZReader.ZWorld.Load(reader);

        var worldScreen = new ZZTGame.Screens.WorldPlay();
        worldScreen.SadComponents.Add(new KeyboardChangeBoard(world, worldScreen));
        worldScreen.UseKeyboard = true;

        return worldScreen;
    })
    .IsStartingScreenFocused(true)
    .ConfigureFonts()
    .SetSplashScreen<SadConsole.SplashScreens.Ansi1>()
    ;

Game.Create(startup);
Game.Instance.Run();
Game.Instance.Dispose();


class KeyboardChangeBoard : SadConsole.Components.KeyboardConsoleComponent
{
    int mapIndex = 0;
    ZReader.ZWorld world;

    public KeyboardChangeBoard(ZReader.ZWorld world, ZZTGame.Screens.WorldPlay screen)
    {
        this.world = world;
        LoadNextMap(screen);
    }

    public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
    {
        if (keyboard.IsKeyPressed(Keys.Space))
            LoadNextMap((ZZTGame.Screens.WorldPlay)host);

        handled = true;
    }

    private void LoadNextMap(ZZTGame.Screens.WorldPlay worldScreen)
    {
        worldScreen.SetActiveBoard(worldScreen.ImportZZTBoard(world.Boards[mapIndex]).Name);
        mapIndex++;

        if (mapIndex >= world.Boards.Length)
            mapIndex = 0;
    }
}
