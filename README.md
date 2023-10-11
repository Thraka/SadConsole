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
using SadConsole;
using SadConsole.Configuration;

Settings.WindowTitle = "SadConsole Examples";

Builder startup = new Builder()
    .SetScreenSize(90, 30)
    .OnStart(onStart)
    .IsStartingScreenFocused(true)
    .ConfigureFonts((config, game) => config.UseBuiltinFontExtended())
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
```

## Latest changes v10.0.0 Beta 1 (10/09/2023)

