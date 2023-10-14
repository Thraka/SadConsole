using SadConsole.Configuration;

Builder startup = new Builder()
    .SetScreenSize(122, 42)
    .SetStartingScreen<ThemeEditor.Container>()
    .IsStartingScreenFocused(true)
    .ConfigureFonts(true)
    .SetSplashScreen<SadConsole.SplashScreens.Ansi1>()
    ;

Game.Create(startup);
Game.Instance.Run();
Game.Instance.Dispose();
