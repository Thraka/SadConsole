/* TODO:

- Palette editor doesn't handle duplicate names. It should handle duplicate names and invalidate them.

*/


using SadConsole.Configuration;
using SadConsole.Editor;

Settings.WindowTitle = "SadEditor v0.2";

Builder config =
    new Builder()
        .SetWindowSizeInCells(130, 50)
        .OnStart(StartHandler)
        .OnEnd(EndHandler);

//.UseDefaultConsole()
//.IsStartingScreenFocused(true)
//.SetStartingScreen<ExampleConsole>();

//.SetStartingScreen<KeyboardScreen>()
//.IsStartingScreenFocused(true);

Game.Create(config);
Game.Instance.Run();
Game.Instance.Dispose();

void StartHandler(object? sender, GameHost host)
{
    Core.State.LoadSadConsoleFonts();

    Core.Start();
}


void EndHandler(object? sender, GameHost e)
{
    Core.State.SaveEditorPalette();
}
