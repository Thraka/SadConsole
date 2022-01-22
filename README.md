![SadConsole Logo](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/SadConsoleLogo.gif)

[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]

SadConsole is a generic library that emulates old-school console game systems. It provides command prompt-style graphics where one or more tile textures are used to represent an ASCII character set. Console's are made up of a grid of cells, each of which can have its own foreground, background, glyph, and special effect applied to it.

While SadConsole is a generic library that doesn't provide any rendering capabilities, "host" libraries are provided that add renderers to SadConsole. The two hosts provided by this library are for MonoGame and SFML.

_SadConsole currently targets .NET 6, .NET 5, .NET Core 3.1, and .NET Standard 2.1._

For the latest changes in this release, see the [notes below](#latest-changes)

## Features

Here are some of the features SadConsole supports:

- Show any number of consoles.
- Uses graphical tile-based images to build out an ASCII-character font with support for more than 256 characters.
- Use more than one font file. However, each console is restricted to a single font.
- Independently controlled entities for game objects.
- Keyboard and Mouse support.
- Text UI control framework.
- Windowing capabilities.
- Importers for [DOS ANSI files](https://wikipedia.org/wiki/ANSI_art), [TheDraw text fonts](https://en.wikipedia.org/wiki/TheDraw), [RexPaint](https://www.gridsagegames.com/rexpaint/), and [Playscii](http://vectorpoem.com/playscii/).
- Animation support.
- Translating images to text-blocks.
- Highly customizable system.

#### String display and parsing

![string pic](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/stringparseexample.gif)

#### GUI library

![GUI library pic](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/controls.gif)

#### Scrolling

![scrolling console](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/scrolling-example2.gif)

## Dependencies

SadConsole uses NuGet for its .NET dependencies:

- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) (>= 13.0.1)
- [TheSadRogue.Primitives](https://www.nuget.org/packages/TheSadRogue.Primitives/) (>= 1.0.0)

[nuget]: http://www.nuget.org/packages/SadConsole/

## Latest changes

**v9.2.2 (01/22/2022)**

(v9.2.2 Fix conversion of Mirror to SpriteEffects)\
(v9.2.1 Fix API documentation generation)

**9.2.0**

### Breaking changes

- [Breaking] `Print(int x, int y, ColoredGlyph glyph)` renamed to `SetGlyph`.
- [Breaking] Surface `SetEffect` method signatures have changed.
- [Breaking] Renamed `Animation.ConvertImageFile` to `Animation.FromImage`.
- [Breaking] Removed `ColorGradient` as this type is implemented in the SadRogue.Primitives library as `Gradient`.

### Behavioral changes

- [Behavior] `ColoredString.String.Set` forced the string through the parser. This has now changed to use the characters directly.
- [Behavior] All `ColoredString` contructors that used the `(string)` overload used the string parser. This is no longer the case.
- [Behavior] `ColoredString.IgnoreEffect` no longer defaults to `true`.
- [Behavior] Using `Surface.Print` methods that used the string parser for fore/back/mirror will force those settings **after** the string was parsed and not before.
- [Behavior] Surface `Clear` and `Fill` methods now clear the effect.
- [Behavior] `Print` statements have been updated to all act the same.
  - New overload added that accepts decorators.
  - Print clears the effect over the glyphs printed.
  - Print that uses the string processor now processes the string and then sets the appropriate overloaded settings. For example, the overload that sets the foreground and background colors will process the string and then set the foreground and background of the entire string. This is a change from previous behavior which set the colors at the start of the string processor and allowed the processor to override the overload.

## Host changes

- [MonoGame] Added DrawCallManager to allow injecting custom sprite batch rendering during final scene composition.
- [MonoGame/SFML] Fixed a bug that caused all surfaces to redraw all cells 100% of the time even if nothing changed. Should bring 300%-400% fps increase in surfaces that aren't changing content.
- [MonoGame/SFML] ITexture improvements for GetPixel/SetPixel; Demos on editing textures. (RychuP)
- [MonoGame/SFML] The game host now has a `FrameNumber` property that increments each frame cycle.
- [SFML] Fixed `Settings.UnlimitedFPS`. This now works.

## Other changes

- [Core] Fixed bug that caused redraws every frame even if nothing had changed.
- [Core] Cursor didn't respect `Cursor.UseStringParser` because of how `ColoredString` was always using the string parser. This is fixed now.
- [Core] Cursor has a `Cursor.MouseClickRepositionHandlesMouse` property which sets the handled flag on mouse left-click for the cursor reposition.
- [Core] Cursor updates the space character appearance while printing. Previously only the first character was used to determine the space's appearance.
- [Core] `DrawString` instruction overrides reset now, fixing a bug with having the instruction run more than once.
- [Core] `Surface.ShiftLeft|Right|Up\Down` methods now move decorators.
- [Core] New `Surface.ShiftRow` and `Surface.ShiftColumn` methods added. (Chris3606)
- [Core] `ColoredString.SetDecorators` added, to fit in with `SetForeground`, `SetBackground`, etc.
- [Core] Renamed `EffectsChain` to `EffectSet` and added new `CodeEffect` type.
- [Core] Effects use `TimeSpan` instead of double.
- [Core] The `EffectsManager` used by a surface now works on cell instances, not cell indices.
- [Core] Resizing a surface without the `clear` parameter keeps existing effects instead of dropping them.
- [Core] `AnimatedSurface.FromImage` helper added which converts image-based animations to an animated surface. (RychuP)
- [Core] Added TheDraw font reader: `SadConsole.Readers.TheDrawFont`. **Not a SadConsole Font.**
- [Core] [Playscii](http://vectorpoem.com/playscii/) support added in the `SadConsoles.Readers` namespace. (RychuP)
- [Core] Entity renderer has a `RemoveAll` method to clear out all the entities.
- [Core] Entity renderer now has a `SkipExistsChecks` property which can greatly improve performance when adding/removing entities (when you already have a lot of entities).
- [Core] For entities, added `AnimatedAppearanceComponent` which can be added to an entity to animate the glyph like the `AnimatedSurface` did for the old entity type.
- [UI] Fix various minor bugs with controls.
- [UI] `Textbox` has more events related to text changing.
- [UI] `Textbox` behaviors have changed slightly. For example, `EditingText` event doesn't fire when the text ends up being the same before editing.
- [UI] `ListBox` items can be inserted as a `ValueTuple<string, object>` which will use the string (can be a `ColoredString`) as the display text of the item.
- [StringParser] Introduced new interface for string parsing: `StringParser.IParser`.
- [StringParser] Moved current parse code to `StringParser.Default`.
- [StringParser] `ColoredString.Parse` is obsolete and forwards to `ColoredString.Parser.Parse`.
- [StringParser] String processor now has a decorators command: `[c:d glyph:mirror:color[:count]]`
- [StringParser] String processor no longer hides effects until they're used. All processed strings will set a null effect that will be set on the cells that are printed.
- [ExtendedLib] Added ClassicConsoleKeyboardHandler and C64KeyboardHandler for cursor handlers.
- [ExtendedLib] Added MoveViewPortKeyboardHandler component.
- [ExtendedLib] Added `surface.PrintFadingText` extension method that prints text using the `DrawString` instruction with an effect.
