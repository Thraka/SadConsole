![SadConsole Logo](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/SadConsoleLogo.gif)

[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]

SadConsole is a C#-based .NET cross-platform terminal, ascii, console, game engine. It simulates these types of programs and with it you can make ascii-styled games for modern platforms. At its heart, SadConsole is really a giant tile-based game engine. However, its object model is conceptually similar to a traditional console app.

While SadConsole is a generic library that doesn't provide any rendering capabilities, "host" libraries are provided that add renderers to SadConsole. The two hosts provided by this library are for **SadConsole.Host.MonoGame** and **SadConsole.Host.SFML**. When adding a host library to your project, you don't need to reference the base **SadConsole** package. If you use MonoGame, you'll also need to add a rendering NuGet package, such as **MonoGame.Framework.DesktopGL**.

_SadConsole currently targets .NET 6, .NET 7, and .NET 8_

For the latest changes in this release, see the [notes below](#latest-changes)

## Features

Here are some of the features SadConsole supports:

- Show any number of consoles of any size.
- Uses graphical tile-based images to build out an ASCII-character font with support for more than 256 characters.
- Fonts are simply sprite sheet tilesets tied to ascii codes, you can use full graphical tiles if you want.
- Use more than one font file. However, each console is restricted to a single font.
- Full GUI system for interactive controls such as list boxes, buttons, and text fields.
- Importers for [DOS ANSI files](https://wikipedia.org/wiki/ANSI_art), [TheDraw text fonts](https://en.wikipedia.org/wiki/TheDraw), [RexPaint](https://www.gridsagegames.com/rexpaint/), and [Playscii](http://vectorpoem.com/playscii/).
- Animated consoles and instruction system to chain commands together.
- String encoding system for colors and effects while printing.
- Entity support for drawing thousands of movable objects on the screen
- Translating images to text-blocks.
- Keyboard and mouse support.
- Highly customizable framework.

#### String display and parsing

![string pic](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/stringparseexample.gif)

#### GUI library

![GUI library pic](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/controls.gif)

#### Scrolling

![scrolling console](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/scrolling-example2.gif)

[nuget]: http://www.nuget.org/packages/SadConsole/

## Example startup code

```csharp
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
```

```vb
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
```

## Latest changes

v10.4.2 (10/23/2024)

- [Core] Fixed small bug when cursor's effect was removed.
- [Core] `StringParser`'s base command object has a virtual `Removed` method.
- [Core] `StringParser`'s Recolor command can use 'w' in place of length to specify that it recolors 'word' length: until a space or \0 character is reached.
- [Core] `StringParser`'s Blink command now has the blink effect `RestoreCellOnRemoved` set to true.
- [Core] `NamedColor` supporting type which is just a collection of named color values for serialization.
- [Core] Fix some spelling errors.
