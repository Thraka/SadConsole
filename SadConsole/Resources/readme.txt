Welcome to SadConsole.

Questions? Join the Discord channel
https://discord.gg/pAFNKYjczM

You must add a host NuGet package to your project, either SadConsole.Host.SFML or SadConsole.Host.MonoGame. If you use MonoGame, you'll also need to add a rendering NuGet package, such as MonoGame.Framework.DesktopGL.

If you're new to SadConsole, read the getting started tutorial at https://sadconsole.com/articles/tutorials/getting-started/part-1-drawing.html

Here is an example program file that let's you get started with SadConsole quickly. If you're using Visual Basic, starter code is listed below the C# code.

C#
===============================
using Console = SadConsole.Console;
using SadConsole;
using SadConsole.Configuration;
using SadRogue.Primitives;

Settings.WindowTitle = "SadConsole Examples";

// Configure how SadConsole starts up
Builder startup = new Builder()
    .SetScreenSize(90, 30)
    .UseDefaultConsole()
    .OnStart(Game_Started)
    .IsStartingScreenFocused(true)
    .ConfigureFonts(true)
    ;

// Setup the engine and start the game
Game.Create(startup);
Game.Instance.Run();
Game.Instance.Dispose();

void Game_Started(object? sender, GameHost host)
{
    ColoredGlyph boxBorder = new(Color.White, Color.Black, 178);
    ColoredGlyph boxFill = new(Color.White, Color.Black);

    Game.Instance.StartingConsole.FillWithRandomGarbage(255);
    Game.Instance.StartingConsole.DrawBox(new Rectangle(2, 2, 26, 5), ShapeParameters.CreateFilled(boxBorder, boxFill));
    Game.Instance.StartingConsole.Print(4, 4, "Welcome to SadConsole!");
}
===============================


Visual Basic
===============================
Imports SadConsole
Imports Console = SadConsole.Console
Imports SadConsole.Configuration
Imports SadRogue.Primitives

Module Module1

    Sub Main()

        Dim startup As New Builder()

        ' Configure how SadConsole starts up
        startup.SetScreenSize(90, 30)
        startup.UseDefaultConsole()
        startup.OnStart(AddressOf Game_Started)
        startup.IsStartingScreenFocused(True)
        startup.ConfigureFonts(True)

        ' Setup the engine and start the game
        SadConsole.Game.Create(startup)
        SadConsole.Game.Instance.Run()
        SadConsole.Game.Instance.Dispose()

    End Sub

    Sub Game_Started(sender As Object, host As GameHost)

        Dim boxBorder = New ColoredGlyph(Color.White, Color.Black, 178)
        Dim boxFill = New ColoredGlyph(Color.White, Color.Black)

        Game.Instance.StartingConsole.FillWithRandomGarbage(255)
        Game.Instance.StartingConsole.DrawBox(New Rectangle(2, 2, 26, 5), ShapeParameters.CreateFilled(boxBorder, boxFill))
        Game.Instance.StartingConsole.Print(4, 4, "Welcome to SadConsole!")

    End Sub

End Module
===============================
