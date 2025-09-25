using SadConsole.Configuration;

Settings.WindowTitle = "My SadConsole Game";

Builder
    .GetBuilder()
    .SetWindowSizeInCells(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
    .SetStartingScreen<SadConsoleGame.Scenes.RootScreen>()
    .IsStartingScreenFocused(true)
    .ConfigureFonts(true)
    .Run();
