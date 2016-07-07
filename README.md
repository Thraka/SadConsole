# SadConsole

[![Join the chat at https://gitter.im/Thraka/SadConsole](https://badges.gitter.im/Thraka/SadConsole.svg)](https://gitter.im/Thraka/SadConsole?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![nugetpkg](https://img.shields.io/badge/nuget-SadConsole-orange.svg)](http://www.nuget.org/packages/SadConsole.Core/)

SadConsole is an MonoGame 3.5-based game library (using .NET 4.6) that provides an engine to emulate old-school console and command prompt style graphics. One or more textures are used to represent the standard ascii character set. SadConsole allows you to create console instances which can be managed independently of each other. A console is made up of individual cells which can have a foreground, background, character, and a special effect applied to it. 

Right now, SadConsole targets Windows and Linux using MonoGame 3.5.

There is source code for a screen editor written in SadConsole. It's the most comprehensive project to date using SadConsole and is available at https://github.com/Thraka/SadConsoleEditor/  
**NOTE**: The editor is not yet upgraded to SadConsole 3.0 and cannot be used with it.

See the wiki for more information.

Some sample games are provided in the [SampleGames](https://github.com/Thraka/SadConsole/tree/master/SampleGames) folder and are now part of the Visual Studio solution file.

## Features

Here are some of the features SadConsole supports.

* Show any number of consoles
* Uses PNG graphic fonts supporting more than 256 characters
* Multiple fonts in your game
* Draggable console windows whithin the game
* Text GUI controls
* Full keyboard support
* Full mouse support
* Read ansi files from the good old DOS days
* Animation engine
* Instruction engine

#### Demo video
http://youtu.be/ZukjZIqDfJw

## Dependencies
SadConsole 2.x uses NuGet dependencies for its .NET dependencies, but requires some external dependencies on MacOS and Linux.

### MacOS
As a MonoMac project, Xcode must be installed along with the full Xamarin development kit.

### Linux
SadConsole for Linux has been built against the Debian package of Mono, on Ubuntu 14.04 LTS. It requires SDL dependencies, specifically `libsdl-mixer` and `libsdl-gfx`.

## Starter Project
[StarterProject](./StarterProject) demonstrates how to use SadConsole in a multi-platform environment.


# 2.0 --> 3.0 Migration Notes

If you're migrating an existing 2.0 project to 3.0 here are some things to remember.

1. If you're using the **SadConsole.Consoles.Console** type, the structure has changed a bit.
	1. You do not need to access the `CellData` or `_cellData` variables to manipulate the surface. Do a find/replace with an empty string.
	2. Anything related to **Character** was changed to **Glyph**. For example `SetCharacter` is now `SetGlyph` and `CharacterIndex` is `GlyphIndex`.
	3. If you accessed the CellData[indexor] directly, use textSurface[indexor].
2. The **SadConsole.Entities** namespace has been removed.
	1. There is no longer a specific entity type.
	2. The animation portion of an entity has moved to the **SadConsole.Consoles.AnimatedTextSurface** type.
		- You can render animations as the surface of any console because of this
	3. The GameHelpers library has a **SadConsole.Game.GameObject** type that acts like the old entity type.
		- This type though is not part of the console system like entity was.
3. Initialization is simpler. Use this line now. Now need to load a font or resize manually, it's all handled for you
	
	```csharp
	var rootConsole = SadConsole.Engine.Initialize(_graphics, "IBM.font", 80, 25);
	```

	Also not that Engine.Initialize will return a console to the specified screen size (in this case 80x25) and automatically adds it to the `SadConsole.Engine.ConsoleRenderStack`.
