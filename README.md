![SadConsole Logo](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/SadConsoleLogo.gif)

[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]
[![kandi X-Ray](https://kandi.openweaver.com/badges/xray.svg)](https://kandi.openweaver.com/csharp/Thraka/SadConsole)

SadConsole is a generic library that emulates old-school console game systems. It provides command prompt-style graphics where one or more tile textures are used to represent an ASCII character set. Console's are made up of a grid of cells, each of which can have its own foreground, background, glyph, and special effect applied to it.

While SadConsole is a generic library that doesn't provide any rendering capabilities, "host" libraries are provided that add renderers to SadConsole. The two hosts provided by this library are for **MonoGame** and **SFML**.

_SadConsole currently targets .NET 6 and .NET 7_

For the latest changes in this release, see the [notes below](#latest-changes)

## Features

Here are some of the features SadConsole supports:

- Show any number of consoles.
- Uses graphical tile-based images to build out an ASCII-character font with support for more than 256 characters.
- Use more than one font file. However, each console is restricted to a single font.
- Independently controlled entities for game objects.
- Keyboard and Mouse support.
- Text UI control framework with windowing support.
- Importers for [DOS ANSI files](https://wikipedia.org/wiki/ANSI_art), [TheDraw text fonts](https://en.wikipedia.org/wiki/TheDraw), [RexPaint](https://www.gridsagegames.com/rexpaint/), and [Playscii](http://vectorpoem.com/playscii/).
- Animated consoles.
- Translating images to text-blocks.
- Highly customizable framework.

#### String display and parsing

![string pic](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/stringparseexample.gif)

#### GUI library

![GUI library pic](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/controls.gif)

#### Scrolling

![scrolling console](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/scrolling-example2.gif)

## Dependencies

SadConsole uses NuGet for its .NET dependencies:

- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) (>= 13.0.3)
- [TheSadRogue.Primitives](https://www.nuget.org/packages/TheSadRogue.Primitives/) (>= 1.6.0)

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
    .ConfigureFonts((config, game) => config.UseBuiltinFontExtended())
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
        startup.ConfigureFonts(Sub(config As FontConfig, gameObject As Game)
                                   config.UseBuiltinFontExtended()
                               End Sub)

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

## Latest changes v10.0.0 Beta 1 and 2 (10/11/2023)

- OnStart startup config is no longer an Action but is now an event.
- Startup config changed again from the last alpha. Types were renamed and rewritten to be extensible. It works the same way as before except that everything is in the SadConsole.Configuration namespace. You must import that namespace to enable the extension methods that build the config. The type to build the config is Builder.
- Tabs orientated left/right didn't display the text properly.
- ColoredGlyph was renamed to ColoredGlyphBase and a new ColoredGlyph that inherits from the base class was added.
- Scrollbars didn't behave properly when in a CompositeControl.
- ListBox mouse logic was improved to use an "ItemsArea" property that designates when the mouse is over the items list specifically.
- StringParser supports variables. There's a dictionary that invokes a delegate which returns a value for the variable. So the value can be determined as the variable is used. See DemoStringParsing.cs in the sample template.
- AnimatedScreenObject's weren't rendering correctly.
- Decorators are no longer array's but rented from a list pool. Use the CellDecoratorHelpers class to manage them. SadConsole does its best to rent and return from the pool as you add or remove decorators.
- Default font used the wrong name. It's been corrected from "IBM_16x8" to "IBM_8x16".
- The ToggleSwitch control wasn't drawing properly.
- NumberBox supports cultured number parsing.
- CellDecorator, as a readonly struct, now declares the properties with "get; init;" which allows mutation with the the "with" keyword.
- EntityManager.AlternativeFont added to support different fonts for entities.
