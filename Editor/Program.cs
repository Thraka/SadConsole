/* TODO:

- Palette editor doesn't handle duplicate names. It should handle duplicate names and invalidate them.
- Add a palette save\load system so you can import\export palettes.

*/


using SadConsole.Configuration;
using SadConsole.Editor;

Settings.WindowTitle = "SadEditor v3.0 Beta 2";

Builder config =
    new Builder()
        .ConfigureWindow((windowCfg, builder, host) =>
        {
            host.GetDeviceScreenSize(out int width, out int height); 
            windowCfg.SetWindowSizeInPixels(width - (int)(width * 0.10f), height - (int)(height * 0.10f));
        })
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
