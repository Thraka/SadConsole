![SadConsole Logo](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/SadConsoleLogo.gif)

[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]
[![kandi X-Ray](https://kandi.openweaver.com/badges/xray.svg)](https://kandi.openweaver.com/csharp/Thraka/SadConsole)

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

v10.0.3 (03/13/2024)

- [UI] `ScrollBar` has been completely rewritten. Minor breaking changes.
  - `.Maximum` has been changed to `.MaximumValue`.
  - Properties related to the style, such as `BarGlyph`, were moved to a `Style` property which controls how the control looks. Some property names have changed
- [UI] `NumberBox` improvements.
  - Rendering code split from `Textbox`.
  - Added `ShowUpDownButtons` to show up\down buttons.
  - Fixed bug with `UseMinMax` messing up the value and setting it back to 0 when the control loses focus.
- [UI] `ControlBase.FindThemeFont` helper method added.
- [UI] Minor bug fixed where captured controls (such as a scroll bar) wouldn't process the mouse if the control was parented to a composite control and the mouse left the parent area.
- [Core] Fixed `EffectSet` bug where the last effect wasn't applied.
- [Core] `GlyphDefinition` has an init accessor now.
- [Core] Added `ShapeParameter` docs and `CreateFilled` supports ignoring the border.
- [Core] Added `RootComponent` class that can be added to `SadConsole.Game.Instance.RootComponents`. These components run logic before the keyboard and screen updates.
- [Core] Splash screen collection is nulled after it runs, freeing memory.
- [Extended] Classic keyboard handler has `IsReady` flag now to control when it's active.
- [Extended] `ColorPickerPopup` would crash on invalid textbox values.
- [Extended] `GlyphSelectPopup` added. You can use this to display a list of glyphs in your font while debugging your app.
- [Extended] Fixed a bug in the table control that prevented the scroll bars from being displayed.
- [Extended] Cleaned up code and enabled nullable.
- [Host - SFML] Fix bug where it was always running at unlimited FPS.
- [Host - MonoGame] Renderers can set backing texture usage.
- [Host - FNA] Fix bug where the screen clear wasn't working and would default to violet.
- [Host - All] Add `OptimizedScreenSurfaceRenderer` which renders.
- [Host - All] Surface render step can accept an alternative surface with the `SetData` method.
