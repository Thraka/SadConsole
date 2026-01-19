![SadConsole Logo](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/SadConsoleLogo.gif)

[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]

SadConsole is a C#-based .NET cross-platform terminal, ascii, console, game engine. It simulates these types of programs and with it you can make ascii-styled games for modern platforms. At its heart, SadConsole is really a giant tile-based game engine. However, its object model is conceptually similar to a traditional console app.

While SadConsole is a generic library that doesn't provide any rendering capabilities, "host" libraries are provided that add renderers to SadConsole. The two hosts provided by this library are for **SadConsole.Host.MonoGame** and **SadConsole.Host.SFML**. When adding a host library to your project, you don't need to reference the base **SadConsole** package. If you use MonoGame, you'll also need to add a rendering NuGet package, such as **MonoGame.Framework.DesktopGL**.

_SadConsole currently targets .NET 8, .NET 9, and .NET 10_

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

## Latest changes

- [Core] `ColoredGlyph` deserialization correctly handles decorators.
- [Core] `LayeredSurface` layers can be hidden now.
- [Controls] Rendering controls on a surface with a viewport now correctly handles mouse input and drawing of the controls.
- [Core] `AnimatedValue` was sending the finished event before it was completed.
- [Core] `AnimatedScreenSurface` didn't set the new frame sizes when deserialized.
- [Core] `ITitle` moved from debug library to core.
- [Host] Hosts have new logic for converting images into surfaces.
- [MonoGame] Render steps now use local spritebatches, which can help with memory allocation in some cases.
- [Core] Entities now have `IHasLayer` from the primitives library which matches the `ZIndex` property.
- [Core] Entities expose the `IHasID` interface now.
- [Core] `EntityManager` correctly handles animated entity visibility.
- [UI] `ListBox` and `ComboBox` have generic versions now.
- [UI] `ComboBox` Placement of the dropdown is correctly kept on the screen.
- [Core] Changing `Game.Instance.DefaultFont` and `DefaultFontSize` now update all objects on the screen with the new defaults when they use the defaults.
