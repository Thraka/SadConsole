![SadConsole Logo](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/SadConsoleLogo.gif)

[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]

SadConsole is a generic library that emulates old-school console game systems. It provides command prompt-style graphics where one or more tile textures are used to represent an ASCII character set. Console's are made up of a grid of cells, each of which can have its own foreground, background, glyph, and special effect applied to it.

While SadConsole is a generic library that doesn't provide any rendering capabilities, "host" libraries are provided that add renderers to SadConsole. The two hosts provided by this library are for MonoGame and SFML.

_SadConsole currently targets .NET 6._

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

## Example startup code

```csharp
using SadConsole;

Settings.WindowTitle = "SadConsole Examples";

Game.Configuration gameStartup = new Game.Configuration()
    .SetScreenSize(90, 30)
    .OnStart(onStart)
    .IsStartingScreenFocused(false)
    .ConfigureFonts((f) => f.UseBuiltinFontExtended())
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

## Latest changes v10.0.0 Alpha 1 (XX/XX/2023)

Major changes (possibly breaking)

- [Core] The editor functions that changed glyphs and printed on consoles have moved from being extension methods for the `ICellSurface` interface to the `ISurface` interface. `Console`, `IScreenSurface`, and `ICellSurface`, all implement this new interface. This means you can now use the editing extensions directly on those objects.
- [Core] Because `Console` no longer implements `ICellSurface`, and instead implements `ISurface`, some properties have been moved to the `Surface` property, such as `myConsole.TimesShiftedUp` is now `myConsole.Surface.TimesShiftedUp`.

New features

- [Core] Added `Componenets.LayeredSurface` component. Add this component to a `ScreenSurface` to enable multiple surface layers. Use the `LayeredSurface` to manage the layers.
- [UI] New control, `NumberBox`. The `IsNumeric` system was removed from the `TextBox` and put into its own control.

Normal changes

- Target .NET 6+ exclusively. Core library is nullable aware.
- [Core] Splash screen printing wasn't being shown because of cursor changes.
- [Core] `IFont` now defines glyph definitions.
- [Core] Various `SadFont` properties and methods are now settable/callable.
- [Core] Extensions methods added to hosts to allow manipulation of font textures.
- [Core] `Settings.CreateStartingConsole` setting added to avoid creating the `StartingConsole`.
- [Core] Cursor now has the property `DisablePrintAutomaticLineFeed` which, when true, prevents the cursor from moving to the next line if printing a character at the end of the current line.
- [Core] `Ansi.AnsiWriter` handles sauce now by ignoring the rest of a document once character 26 (CTRL-Z) is encountered.
- [Core] `Ansi.AnsiWriter` has always used a cursor to print, it now sets `UseLinuxLineEndings = true` and `DisablePrintAutomaticLineFeed = true`.
- [Core] Added `SadConsole.SplashScreens.Ansi1` splashscreen, the new SadConsole logo, for use with games.
- [Core] Added `ScreenSurface.QuietSurfaceHandling` property. When `true`, this property prevents the `.Surface` property from raising events and calling virtual methods when the instance changes. This is useful for `AnimatedScreenSurface` instances that have fast moving animations.
- [Core] The `Entity` type now supports animated surfaces. When creating an entity, you must specify it as a **single cell** entity or **animated surface** entity.
- [Core] The effects system had a bug where if you added the same effect with the same cell twice, and the effect should restore the cell state, it wouldn't.
- [Core] `AsciiKey` used by the keyboard system now detects capslock and shifted state to produce capital or lowercase letters.
- [Core] `AsciiKey` exposes a bunch of static dictionaries that help with remapping keys and characters.
- [Core] `ColoredGlyph.IsVisible` now sets `ColoredGlyph.IsDirty` to true when its value changes.
- [Core] `Surface.RenderSteps` moved to the renderer.
- [Core] `RenderSteps` is now a `List` and you must call `RenderSteps.Sort(SadConsole.Renderers.RenderStepComparer.Instance)` when the collection is changed.
- [Core] `Instructions.DrawString` uses `System.TimeSpan` now, and is more accurate.
- [Core] Effects have a `RunEffectOnApply` property that will run the `effect.Update` method once, with a duration of zero, when the effect is added to a manager.
- [Core] `EffectsManager` will apply the active effect to a cell right when the cell is added to the effect. This *was* happening on the next render frame.
- [Core] Surface shifting is much more performant (Thanks Chris3606)
- [Core] `Cursor` has some new methods for erasing: `Erase`, `EraseUp`, `EraseDown`, `EraseLeft`, `EraseRight`, `EraseColumn`, `EraseRow`.
- [Core] Mouse state object now tracks `*ButtonDownDuration` times. When the button is down and the time is zero, this indicates the button was just pressed. Otherwise, you can detect how long the button has been held down.
- [Core] Rename RexPaint `ToLayersComponent` to `ToCellSurface`.
- [Core] Rework `Timer` with `Start/Stop/Restart` methods.
- [UI] Scroll bar with a size of 3 now disables the middle area, and you can use a size of 2 now.
- [UI] Scroll bar supports a thickness other than 1.
- [UI] Control host would get stuck when tabbing to a control that was disabled. Now it skips the control.
- [UI] `TextBox` rewritten. The `IsNumeric` system was removed and added to a new control: `NumberBox`. The `TextBox` no longer has an editing mode and simply starts editing as it's focused and stops editing once it loses focus.
- [UI] `ControlBase.IsDirty` property now calls the protected `OnIsDirtyChanged` method which then raises the `IsDirtyChanged` event.
- [UI] `Panel` control uses `CompositeControl` as a base class. Theme supports lines as a border of the panel.
- [UI] Ven0maus added the `Table` control.
- [ExtendedLib] Border control uses view size now instead of full size of wrapping object.
- [ExtendedLib] `Border.AddToSurface/Window` has been renamed to `Border.CreateForSurface/Window`.

Removed

- [Core] `Algorithms.Line\Circle\Ellipse` have been removed. The latest primitives library provides these methods.
- [Core] `Shapes` namespace removed. The latest primitives library release that SadConsole uses, provides these.

### Host changes

- [MonoGame] NuGet package has a -windows framework target that targets DirectX and adds support for WPF.
- [MonoGame] Fix conversion of Mirror to SpriteEffects.
- [MonoGame\SFML] Surface renderers now skip the glyph if the glyph is 0.
- [MonoGame\SFML] New **SurfaceDirtyCells** renderer added which only draws cells that are marked dirty.
- [MonoGame\SFML] New `Game.Configuration` object used for creating a SadConsole game.
