using SadBBSClient;
using SadConsole;
using SadConsole.Configuration;

// TODO: Load settings


Builder startup = new Builder()
    .SetWindowSizeInCells(AppSettings.Instance.Width, AppSettings.Instance.Height)
    .ConfigureFonts(true)
    .IsStartingScreenFocused(true)
    .SetStartingScreen<SadBBSClient.BbsScreen>()
    ;

Game.Create(startup);

Game.Instance.Run();
Game.Instance.Dispose();
