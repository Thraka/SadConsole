# SadConsole #

SadConsole is an MonoGame 3.3-based game library that provides an engine to emulate old-school console and command prompt style graphics. One or more textures are used to represent the standard ascii character set. SadConsole allows you to create console instances which can be managed independently of each other. A console is made up of individual cells which can have a foreground, background, character, and a special effect applied to it. 

Right now, SadConsole targets Windows and Linux using MonoGame 3.3. SadConsole 2.0 dropped XNA 4.0 support; it could be re-added by an interested party, pull requests are welcome.

There is source code for a screen editor written in SadConsole. It's the most comprehensive project to date using SadConsole and is available at https://github.com/Thraka/SadConsoleEditor/

See the wiki for more information.

## Dependencies ##
SadConsole 2.x uses NuGet dependencies for its .NET dependencies, but requires some external dependencies on MacOS and Linux.

### MacOS ###
As a MonoMac project, Xcode must be installed along with the full Xamarin development kit.

### Linux ###
SadConsole for Linux has been built against the Debian package of Mono, on Ubuntu 14.04 LTS. It requires SDL dependencies, specifically `libsdl-mixer` and `libsdl-gfx`.

## Starter Project ##
[StarterProject](./StarterProject) demonstrates how to use SadConsole in a multi-platform environment.
