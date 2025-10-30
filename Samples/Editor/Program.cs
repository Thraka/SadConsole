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
    // Use this way to load fonts because we want SadConsole to use the normal defaults
    Directory.GetFiles(Core.Settings.FontsFolder, "*.font").ToList().ForEach(file =>
    {
        SadConsole.Game.Instance.LoadFont(file);
    });

    Core.State.LoadSadConsoleFonts();

    Core.Start();
}
