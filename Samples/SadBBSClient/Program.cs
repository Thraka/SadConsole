using SadConsole;
using SadConsole.Configuration;

Builder startup = new Builder()
    .SetWindowSizeInCells(80, 25)
    .ConfigureFonts(true)
    .IsStartingScreenFocused(true)
    .SetStartingScreen<SadBBSClient.BbsScreen>()
    ;

Game.Create(startup);

Game.Instance.Run();
Game.Instance.Dispose();
