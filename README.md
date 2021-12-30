![SadConsole Logo](images/SadConsoleLogo.gif)


[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![Join us on Reddit](https://img.shields.io/badge/reddit-SadConsole-red.svg)](http://reddit.com/r/sadconsole)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]

SadConsole is an MonoGame 3.7 and FNA game library that provides an engine to emulate old-school console and command prompt style graphics. One or more textures are used to represent the standard ascii character set. SadConsole allows you to create console instances which can be managed independently of each other. A console is made up of individual cells which can have a foreground, background, character, and a special effect applied to it. 

SadConsole targets .NET Standard 2.0

## Features

Here are some of the features SadConsole supports.

* Show any number of consoles
* Uses PNG graphic fonts supporting more than 256 characters
* Multiple fonts in your game
* Draggable console windows within the game
* Text GUI controls
* Full keyboard support
* Full mouse support
* Read ansi files from the good old DOS days
* Animation engine
* Instruction engine

#### String display and parsing
![string pic](images/stringparseexample.gif)

#### GUI library
![GUI library pic](images/controls.gif)

#### Scrolling
![scrolling console](images/scrolling-example2.gif)

## Dependencies
SadConsole uses NuGet for its .NET dependencies.

## Demo Project
[DemoProject](https://github.com/Thraka/SadConsole/tree/master/Samples/MainSample) shows how to use SadConsole in a multi-platform environment and demonstrates various things you can do with SadConsole.

[nuget]: http://www.nuget.org/packages/SadConsole/
