/*

Welcome to the SadConsole demo template! This template is the project I use to test
SadConsole as I create new features. It's also a great way to see how to write code
that uses different SadConsole features.

This template will be updated with each release of SadConsole.

=========================================================
Important things you should know about this template
=========================================================

This template imports the following namespaces into every code file:

- SadConsole
- SadRogue.Primitives

Additionally, the SadConsole.Console type is mapped as Console so that it doesn't
conflict with System.Console if you import the System namespace.


Each Demo code file in the project contains two classes. The first is the class
that implements the IDemo interface. This class is what is used by the demo list
box so that you can preview each demo object. The second class is the actual console
demo code, which you can reuse in your own project.

*/

using SadConsole.Examples;
using SadConsole.Configuration;

#if FNA
Settings.WindowTitle = "SadConsole Examples (FNA)";
#elif MONOGAME
Settings.WindowTitle = "SadConsole Examples (MONOGAME)";
#elif SFML
Settings.WindowTitle = "SadConsole Examples (SFML)";
#endif

Builder startup = new Builder()
    .SetScreenSize(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
    .SetStartingScreen<RootScreen>()
    .IsStartingScreenFocused(false) // Dont want RootScreen to be focused because RootScreen automatically focuses the selected demo console
    .ConfigureFonts(true)
    //.SetSplashScreen<SadConsole.SplashScreens.Ansi1>()
    .KeyhookMonoGameDebugger(SadConsole.Input.Keys.F12)
    ;

Game.Create(startup);
Game.Instance.Run();
Game.Instance.Dispose();
