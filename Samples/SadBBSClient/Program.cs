using SadConsole;
using SadConsole.Configuration;

Builder startup = new Builder()
    .SetWindowSizeInCells(80, 25)
    .ConfigureFonts(true)
    .OnStart(Game_Started)
    ;

Game.Create(startup);

#nullable enable
void Game_Started(object? sender, GameHost host)
{
    Game.Instance.Screen = new SadBBSClient.BbsScreen();
    Game.Instance.DestroyDefaultStartingConsole();
}

Game.Instance.Run();
Game.Instance.Dispose();
