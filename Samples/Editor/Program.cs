using SadConsole.Configuration;
using SadConsole.Editor;

Settings.WindowTitle = "SadEditor v0.2";

Builder config =
    new Builder()
        .SetWindowSizeInCells(130, 50)
        .OnStart(StartHandler);

//.UseDefaultConsole()
//.IsStartingScreenFocused(true)
//.SetStartingScreen<ExampleConsole>();

//.SetStartingScreen<KeyboardScreen>()
//.IsStartingScreenFocused(true);

Game.Create(config);
Game.Instance.Run();
Game.Instance.Dispose();

static void StartHandler(object? sender, GameHost host)
{
    Core.State.LoadSadConsoleFonts();

    Core.Start();
}
