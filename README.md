SadConsole
==========

SadConsole is an MonoGame 3.3-based game library that provides an engine to emulate old-school console and command prompt style graphics. One or more textures are used to represent the standard ascii character set. SadConsole allows you to create console instances which can be managed independently of each other. A console is made up of individual cells which can have a foreground, background, character, and a special effect applied to it. 

Right now, SadConsole targets Windows and Linux using MonoGame 3.3. SadConsole 2.0 dropped XNA 4.0 support; it could be re-added by an interested party, pull requests are welcome.

There is source code for a screen editor written in SadConsole. It's the most comprehensive project to date using SadConsole and is available at https://github.com/Thraka/SadConsoleEditor/

See the wiki for more information.

Dependencies
------------
* SadConsole 2.x uses NuGet dependencies and does not require external dependencies.
* SadConsole 1.x:
    * MonoGame 3.2 - http://www.monogame.net/
    * XNA 4.0 Refresh - http://www.microsoft.com/en-us/download/details.aspx?id=27599

Starter Project
---------------
The source code here provides a starter project that demonstrates how to use parts of the engine. The code is located at https://github.com/Thraka/SadConsole/tree/master/StarterProject
