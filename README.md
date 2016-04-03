# SadConsole #

[![nugetpkg](https://img.shields.io/badge/nuget-SadConsole-orange.svg)](http://www.nuget.org/packages/SadConsole.Core/)

SadConsole is an MonoGame 3.4-based game library that provides an engine to emulate old-school console and command prompt style graphics. One or more textures are used to represent the standard ascii character set. SadConsole allows you to create console instances which can be managed independently of each other. A console is made up of individual cells which can have a foreground, background, character, and a special effect applied to it. 

Right now, SadConsole targets Windows and Linux using MonoGame 3.4. SadConsole 2.0 dropped XNA 4.0 support; it could be re-added by an interested party, pull requests are welcome.

There is source code for a screen editor written in SadConsole. It's the most comprehensive project to date using SadConsole and is available at https://github.com/Thraka/SadConsoleEditor/

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


