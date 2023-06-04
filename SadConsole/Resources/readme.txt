Welcome to SadConsole.

Questions? Join the Discord channel
https://discord.gg/pAFNKYjczM

You must add a host NuGet package to your project, either SadConsole.Host.SFML or SadConsole.Host.MonoGame.

Here is an example program.cs file that let's you get started with SadConsole quickly. If you're using Visual Basic, starter code is listed below the C# code.

===============================
using SadConsole;

Settings.WindowTitle = "SadConsole Examples";

Game.Configuration gameStartup = new Game.Configuration()
    .SetScreenSize(90, 30)
    .OnStart(onStart)
    .IsStartingScreenFocused(false)
    .ConfigureFonts((f) => f.UseBuiltinFontExtended())
    ;

Game.Create(gameStartup);
Game.Instance.Run();
Game.Instance.Dispose();

void onStart()
{
    ColoredGlyph boxBorder = new ColoredGlyph(Color.White, Color.Black, 178);
    ColoredGlyph boxFill = new ColoredGlyph(Color.White, Color.Black);

    Game.Instance.StartingConsole.FillWithRandomGarbage(255);
    Game.Instance.StartingConsole.DrawBox(new Rectangle(2, 2, 26, 5), ShapeParameters.CreateFilled(boxBorder, boxFill));
    Game.Instance.StartingConsole.Print(4, 4, "Welcome to SadConsole!");
}
===============================


Example program.vb:

Imports SadConsole
Imports Console = SadConsole.Console
Imports SadRogue.Primitives

Module Module1

    Sub Main()

        ' Setup the engine And create the main window.
        SadConsole.Game.Create(80, 25)

        ' Hook the start event so we can add consoles to the system.
        SadConsole.Game.Instance.OnStart = AddressOf Init

        ' Start the game.
        SadConsole.Game.Instance.Run()
        SadConsole.Game.Instance.Dispose()

    End Sub

    Sub Init()

        Dim startingConsole = DirectCast(GameHost.Instance.Screen, Console)

        startingConsole.FillWithRandomGarbage(255)
        startingConsole.Fill(New Rectangle(3, 3, 27, 3), Color.Violet, Color.Black, 0, Mirror.None)
        startingConsole.Print(6, 5, "Hello from SadConsole", Color.AnsiCyanBright)

    End Sub

End Module
