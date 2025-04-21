using Console = SadConsole.Console;
using SadConsole;
using SadConsole.Configuration;
using SadRogue.Primitives;
using SadConsole.UI;

// Configure how SadConsole starts up
Builder startup = new Builder()
    .SetWindowSizeInCells(90, 30)
    .UseDefaultConsole()
    .OnStart(Game_Started)
    .ConfigureFonts(true)
    .SkipMonoGameGameCreation()
    ;

Settings.DoFinalDraw = false;

// Setup the engine and start the game
Game.Create(startup);

void Game_Started(object? sender, GameHost host)
{
    ColoredGlyph boxBorder = new(Color.White, Color.Black, 178);
    ColoredGlyph boxFill = new(Color.White, Color.Black);

    Game.Instance.StartingConsole.Surface.DefaultBackground = Color.Transparent;
    Game.Instance.StartingConsole.Surface.Clear();

    SadConsole.Settings.ClearColor = Color.Transparent;

    //Game.Instance.StartingConsole.FillWithRandomGarbage(255);
    Game.Instance.StartingConsole.DrawBox(new Rectangle(2, 2, 26, 5), ShapeParameters.CreateFilled(boxBorder, boxFill));
    Game.Instance.StartingConsole.Print(4, 4, "Welcome to SadConsole!");
}

using var game = new MonoGameFirst.Game1();
game.Run();
