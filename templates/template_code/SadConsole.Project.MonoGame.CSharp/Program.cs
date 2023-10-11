﻿using SadConsole.Configuration;

Settings.WindowTitle = "My SadConsole Game";

Builder gameStartup = new Builder()
    .SetScreenSize(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
    .SetStartingScreen<SadConsoleGame.Scenes.RootScreen>()
    .IsStartingScreenFocused(false)
    .ConfigureFonts((config, game) => config.UseBuiltinFontExtended())
    ;

Game.Create(gameStartup);
Game.Instance.Run();
Game.Instance.Dispose();