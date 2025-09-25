![SadConsole Logo](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/SadConsoleLogo.gif)

[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]

SadConsole is a C#-based .NET cross-platform terminal, ascii, console, game engine. It simulates these types of programs and with it you can make ascii-styled games for modern platforms. At its heart, SadConsole is really a giant tile-based game engine. However, its object model is conceptually similar to a traditional console app.

While SadConsole is a generic library that doesn't provide any rendering capabilities, "host" libraries are provided that add renderers to SadConsole. The two hosts provided by this library are for **SadConsole.Host.MonoGame** and **SadConsole.Host.SFML**. When adding a host library to your project, you don't need to reference the base **SadConsole** package. If you use MonoGame, you'll also need to add a rendering NuGet package, such as **MonoGame.Framework.DesktopGL**.

_SadConsole currently targets .NET 6, .NET 7, .NET 8, and .NET 9_

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

Note: .NET 6 has been dropped by the host libraries in favor of .NET 8 as the minimum version. The core SadConsole library still supports .NET 6.

- [Breaking] The MonoGame host no longer has the `ClearScreenComponent`. Instead, the clear screen happens right before the final draw of the main SadConsole component.
- [Breaking] `Builder.Run` was renamed to `Builder.ProcessConfigs` and a new `Builder.Run` was added to make it simpler to configure and start the game.
- [Breaking] Custom controls should NO LONGER set `IsDirty = false` when exiting `UpdateAndRedraw`.
- [Core] A little speed improvement to resize in special cases.
- [Core] Added `SetGlyph` method overload to `ICellSurface` which takes a `GlyphDefinition` to update the glyph and mirror of a cell.
- [Core] Fixed a bug with dragging a surface over another surface incorrectly triggering `MoveToFront` functionality.
- [Extended] Added `MouseDrag` component.
- [Hosts] Added `OptimizedScreenSurfaceRenderer` which is a renderer that only draws dirty cells.
- [UI] During control rendering, hosts will set controls `IsDirty = false` when they're actually drawn instead of when the update method of the control says it should be drawn.
- [UI] Fixed combo box popup with non-1x sized fonts.
