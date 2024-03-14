## v10.0.3

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

## v10.0.2

- [UI] `NumberBox` incorrectly applied `DefaultValue` instead of the actual value when `Min\Max` properties were 0.

## v10.0.1

- [Core] Adjust the description and parameter names for the Resize method.
- [MonoGame] Minor adjustment to monogame extension method class names to avoid doc conflicts.

## v10.0.0 (10/28/2023)

Major changes (possibly breaking)

- [Core] The default font's used the wrong name. It's been corrected from **IBM_16x8** to **IBM_8x16** and from **IBM_16x8_ext** to **IBM_8x16_ext**. If you've previously serialized a font with the wrong name, you'll need to change it in the json object. Or use the configuration extension `FixOldFontName()` which will add a name mapping.
- [Core] The editor functions that changed glyphs and printed on consoles have moved from being extension methods for the `ICellSurface` interface to the `ISurface` interface. `Console`, `IScreenSurface`, and `ICellSurface`, all implement this new interface. This means you can now use the editing extensions directly on those objects.
- [Core] Because `Console` no longer implements `ICellSurface`, and instead implements `ISurface`, some properties have been moved to the `Surface` property, such as `myConsole.TimesShiftedUp` is now `myConsole.Surface.TimesShiftedUp`.
- [Core] Themes have been removed. Each control draws itself.
- [Core] `IScreenObject` no longer implements `IEnumerable` to access the children. Use the `.Children` collection property instead.
- [Core] `Update` and `Render` no longer check for `IsEnabled` and `IsVisible`, respectively. When those methods run, the properties are checked on the children before calling the respective method. This moves the check outside of the object itself, and relies on the parent object to do it. This eliminates the need for an object to check itself, and allows you to bypass the check when you want.
- [Core] Decorators are no longer array's but rented from a list pool. Use the `CellDecoratorHelpers` class to manage them. SadConsole does its best to rent and return from the pool as you add or remove decorators.
- [Core] The `OnStart` callback is now an event and not an `Action` delegate.

New features

- [Core] Added `Componenets.LayeredSurface` component. Add this component to a `ScreenSurface` to enable multiple surface layers. Use the `LayeredSurface` to manage the layers.
- [Core] `StringParser` supports variables with the syntax of **$variable_name**. The parser has a dictionary that invokes a delegate which returns a value for the variable. So the value can be determined as the variable is used. See DemoStringParsing.cs in the sample template.
- [Core] `EntityManager.AlternativeFont` added so entities can use a different font than the parent object.
- [UI] New control, `TabControl`. Contributed by **arxae**.
- [ExtendedLib.UI] New control, `Table`. Contributed by **Ven0maus**.

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
- [Core] `Entities.Renderer` renamed `Entities.EntityRenderer`.
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
- [Core] If you load a font that's already been loaded, the existing instance is returned and the recently deserialized font is discarded. Previously the old font was replaced in the `GameHost.Fonts` dictionary.
- [Core] `ColoredGlyph.Decorators` are Poolable Lists now and don't need to be messed with as arrays. Use `SadConsole.CellDecoratorHelpers` to manage the property.
- [UI] Scroll bar with a size of 3 now disables the middle area, and you can use a size of 2 now.
- [UI] Scroll bar supports a thickness other than 1.
- [UI] Control host would get stuck when tabbing to a control that was disabled. Now it skips the control.
- [UI] `ControlBase.IsDirty` property now calls the protected `OnIsDirtyChanged` method which then raises the `IsDirtyChanged` event.
- [UI] `Panel` control uses `CompositeControl` as a base class. Control can draw a border.
- [UI] `ProgressBar` is easier to customize.
- [UI] `Colors.Name` property added.
- [UI] `ListBox.ItemsArea` property added which the mouse now watches for so that it knows it's interacting with the list items specifically.
- [ExtendedLib] Border control uses view size now instead of full size of wrapping object.
- [ExtendedLib] `Border.AddToSurface/Window` has been renamed to `Border.CreateForSurface/Window`.
- [ExtendedLib] `Entities.EntityManager` renamed `Entities.EntityManagerZoned`.
- [ExtendedLib] Added `SmoothMove.IsEnabled` to disable the component and prevent animation.
- [ExtendedLib] Fixed `ColorBar` bug with selecting a color before it's first drawn.

Removed

- [Core] `Algorithms.Line\Circle\Ellipse` have been removed. The latest primitives library provides these methods.
- [Core] `Shapes` namespace removed. The latest primitives library release that SadConsole uses, provides these.

### Host changes

- [MonoGame] NuGet package has a -windows framework target that targets DirectX and adds support for WPF.
- [MonoGame] Fix conversion of Mirror to SpriteEffects.
- [MonoGame\SFML] Surface renderers now skip the glyph if the glyph is 0.
- [MonoGame\SFML] New `SurfaceDirtyCells` renderer added which only draws cells that are marked dirty.
- [MonoGame\SFML] New `SadConsole.Configuration.Builder` object used for creating a SadConsole game.

## v9.2.1 (01/04/2022)

- Rebuild RELEASE build to fix API documentation generation.

## v9.2.0 (12/30/2021)

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

### Host changes

- [MonoGame] Added DrawCallManager to allow injecting custom sprite batch rendering during final scene composition.
- [MonoGame/SFML] Fixed a bug that caused all surfaces to redraw all cells 100% of the time even if nothing changed. Should bring 300%-400% fps increase in surfaces that aren't changing content.
- [MonoGame/SFML] ITexture improvements for GetPixel/SetPixel; Demos on editing textures. (RychuP)
- [MonoGame/SFML] The game host now has a `FrameNumber` property that incremenets each frame cycle.
- [SFML] Fixed `Settings.UnlimitedFPS`. This now works.

### Other changes

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
- [Core] The `EffectsManager` used by a surface now works on cell instances, not cell indicies.
- [Core] Resizing a surface without the `clear` parameter keeps existing effects instead of dropping them.
- [Core] `AnimatedSurface.FromImage` helper added which converts image-based animations to an animated surface. (RychuP)
- [Core] Added TheDraw font reader: `SadConsole.Readers.TheDrawFont`. **Not a SadConsole Font.**
- [Core] [Playscii](http://vectorpoem.com/playscii/) support added in the `SadConsoles.Readers` namespace. (RychuP)
- [Core] Entity renderer has a `RemoveAll` method to clear out all the entities.
- [Core] Entity renderer now has a `SkipExistsChecks` property which can greatly improve performance when adding/removing entities (when you already have a lot of entities).
- [Core] For entities, added `AnimatedAppearanceComponent` which can be added to an entity to animate the glyph like the `AnimatedSurface` did for the old entity type.
- [UI] Fix various minor bugs with controls.
- [UI] `Textbox` has more events related to text changing.
- [UI] `Textbox` behaviors have changed slightly. For example, `EditingText` event doesn't fire when the text ends up being the same prior to editing.
- [UI] `ListBox` items can be inserted as a `ValueTuple<string, object>` which will use the string (can be a `ColoredString`) as the display text of the item.
- [StringParser] Introduced new interface for string parsing: `StringParser.IParser`.
- [StringParser] Moved current parse code to `StringParser.Default`.
- [StringParser] `ColoredString.Parse` is obsolete and forwards to `ColoredString.Parser.Parse`.
- [StringParser] String processor now has a decorators command: `[c:d glyph:mirror:color[:count]]`
- [StringParser] String processor no longer hides effects until they're used. All processed strings will set a null effect that will be set on the cells that are printed.
- [ExtendedLib] Added ClassicConsoleKeyboardHandler and C64KeyboardHandler for cursor handlers.
- [ExtendedLib] Added MoveViewPortKeyboardHandler component.
- [ExtendedLib] Added `surface.PrintFadingText` extension method that prints text using the `DrawString` instruction with an effect.

## v9.1.1 (08/07/2021)

- `StartingConsole` is now focused at the start of the game.
- Shape drawing routines now accept an overload of `ShapeParameters`. Original shape routines are marked as obsolete.
- Line drawing math was rewritten and is better now.
- `Cursor` component has a `.KeyboardPreview` event that lets you cancel the keyboard press for the cursor and do your own thing.
- `Game.Instance.StartingConsole` is now focused from the start.
- `Cursor.MouseClickReposition` logic was incorrectly moving the cursor without a click.
- {BREAKING} `Cursor.PrintAppearanceMatchesHost` added and defaults to `true`. If you were using a customized `Cursor.PrintApperance`, you must set this property to `false`.
- [ExtendedLib] `Border` helper now has a `BorderParameters` method that lets you set the colors and style of the border.
- [ExtendedLib] New `AnimatedBoxGrow` helper that animates a box from small to large.

## v9.1.0 (07/05/2021)

- `ListBox` can display colored strings now.
- Serialization contracts type is exposed for deriving: `SadConsole.Serializer.Contracts`.
- Some `On*` members of `ScreenSurface` weren't virtual; fixed.
- `Cursor` changes it's cached surface when the parent object's surface changes.
- `Cursor` bug fixes.
- `Entity` is serializable now.
- `Entities.Renderer` can now have entities added/removed without a host object.
- Added new extension methods in the `SadConsole.Quick` namespace to speed up construction of objects.
- `ScreenObject.SadComponentAdded/Removed` callbacks renamed to OnSadComponentAdded/Removed`.
- `IScreenObject.Surface` can now be set.

## v9.0 (06/05/2021)

- [ExtendedLib] Added `SadConsole.Transisions.Fade` instruction to easily allow fading between two objects.
- Changed `Entity.Appearance` to protected.
- Changed fonts to `IFont` and added `SadFont` which represents the font system SadConsole uses.
- Merged code base back to normal develop branch!!
- Control `DrawingSurface` renamed to `DrawingArea`
- New control `SurfaceViewer` control which allows you to add a surface as a control with scroll bars to change the view of it.
- `Control.Resize` now supported.

## v9.0 Beta 4 (01/14/2021)

- Fixed bug that prevents right/middle mouse click detections.
- Added label text to progress bar when in horizontal mode.
- Adjusted the rendering step system. Now steps have a shared output texture to draw to.
- The new entity type has an `Effect` property now.
- `ICellEffect.RestoreCellOnFinshed` was renamed to `RestoreCellOnRemoved`.
- `ICellEffect.CloneOnApply` was renamed to `CloneOnAdd`.
- Move `Timer` to `SadConsole.Components` namespace
- Move `Entities.Zone` to the SadConsole.Extended library.
- ScreenObjects now have a add/remove virtual method for components.
- RenderSteps are now on `IScreenSurface` instead of `IRenderer`.
- `ICellSurface` had a few methods moved to interfaces and they're implemented by `CellSurface`.
- `ColoredGlyph.IsDirtySet` event added which triggers when `IsDirty` is set to `true`.
- Fixed bug with `Entity` not drawing the effect.
- `SortOrder` for various objects changed to `uint`.
- Printing on a surface with effects works a bit faster now.

## v9.0 Beta 3 (12/19/2020)

- New library added `SadConsole.Extended`. This has different classes, controls, consoles, the can be used by many other projects.
- Fixed bug in `SadConsole.UI.Colors` when creating a copy.
- Changed theme color parts to `AdjustableColor` types.
- Adjusted all themes to use a different base class.
- `ScrollBar` operates on all scroll spots.
- Added mousewheel support to `ListBox` and `ScrollBar`.
- `Checkbox` and `RadioButton` now use `ToggleButtonBase` and use the same theme but configured for each control.
- `Settings.WindowTitle` works in SFML now.
- `IRenderStep` added which allows hooking custom rendering steps into the `Renderer` without writing a whole new renderer.
- `Window`, `Cursor`, and `ControlHost` converted to use a `IRenderStep` and the normal object renderer.
- Base types are using primitives library where they can.
- Entity system changed to a simpler system. No more animations on entities.
- Cleaned up a lot (300+) project warnings (missing docs, that sort of thing).

## v9.0 Beta 2 (10/09/2020)

- AHHHHH bug in Font with setting UnsupportedGlyphIndex found 1 day after beta 1.........
- Window title defaults to "Window".
- Window title alignment defaults to "Center".
- Invalidated/OnInvalidated removed from Window/ControlsConsole.
- Window has DrawBorder() method to redraw the border and title.
- More changes to colors added:
  - `SadConsole.UI.Themes.Colors` renamed to `SadConsole.UI.Colors`.
  - `SadConsole.UI.Colors.ColorNames` added as an enum of each color defined by the colors type (such as Red and BlueDark).
  - `SadConsole.UI.Colors.Brightness` enum that can be used with various extension methods to brighten or darkn a color.
  - Added new `SadConsole.UI.AdjustableColor` type which allows you to create a color that maps to a `Colors` entry or a custom value. Apply a `Brightness` to it and return a computed color without affecting the original color value.
- Added `ColoredString.SetGlyph` to set the glyph on all characters in the string.
- Changed button click logic so a long click still triggers a click.

## v9.0 Beta 1 (07/31/2020)

- Removed accidental dependency on SharpFNT.
- Fixed font init bug. (thanks bepis)
- ColorExtension additions.
  - `ToInteger` removed. The PackedValue property is suitable.
  - `ColorAnsi` type removed. Ansi colors are defined by the primitives library.
  - `ColorMappings` is automatically filled with every static color defined by the primitives library (including ansi colors).
  - `FromName` is a handy method to look up a color in the dictionary.
- Simplified gamehost startup by moving common default font loading routines into the base class.
- `Cursor.Print` handles `IgnoreEffect` better and allows the parsed string to set effects. (Thanks axoeu)
- Add new ClearEffect command for string parser: [c:ceffect] or [c:ce]. (Thanks regulark)
- Fixed mouse click and double click!
- Control changes
  - Recoded the way controls are hosted. The `Parent` property now points an `IContainer` which is generally the host component.
  - New `CompositeControl` base type which is an `IContainer` and allows you to create a single control from multiple controls.
  - `ListBox` converted to a `CompositeControl`.
  - `ControlHost.FocusedControl` loses it's focus state when the host loses focus. The state is restored when the host gets focus.
  - `ControlBase` has more accurate mouse processing. For example, pressing a button down on a different control and releasing it on a new control doesn't trigger a click.
  - Added new `Panel` control. 
- Controls handle mouse interactions better.
- The theme library (when set and at start) will add the library colors to the `ColorExtensions.ColorMappings` dictionary with the format of **theme.color-name**.

## v9.0 Alpha 9 (07/24/2020)

- RandomGarbage uses the current font.
- Add `SadConsole.Game.Instance.SetSplashScreens` method for adding startup splash screens to games.
- Add `Cursor.SetPrintAppearance` overloads.
- `CodeInstruction` now uses a TimeSpan delta parameter.
- The `ControslConsole.ControlsHostComponent` property was renamed to `Controls`.
- Rename `ControlsHost.Controls` property to `GetControlsArray` method. `ControlsHost` implements `IEnumerable` to get the controls: `foreach (var control in controlsComponent)`
- Fixed alpha blending on MonoGame. (Thanks DoctorTriagony)
- Added `Host.Settings` options for both MonoGame and SFML related to blending. (Thanks DoctorTriagony)
- Renderers can override the blend setting.
- `ListBox` control logic updated to allow better positioning of the `ScrollBar`.
- Theme for `ListBox` updated to use the `Lines` color.

## v9.0 Alpha 8 (07/12/2020)

- Fixed a lot of LayeredScreenSurface bugs.
- **SFML**: Renamed `Renderers.LayeredScreenObject` to `Renderers.LayeredScreenSurface`
- Removed `TintBeforeDrawCall`. Not a good design.
- Added `SadConsole.Instructions.AnimatedValue` to animate a value over a duration.
- Updated `SadConsole.Instructions.FadeTextSurfaceTint` to use `AnimatedValue`.
- `Renderers.ScreenSurfaceRenderer` has an `Opaqueness` setting to render a console surface transparent.

## v9.0 Alpha 7 (07/10/2020)

- Various `ControlHost` control management bugs fixed (thanks coldwarrl).
- Rendering of Fonts improved.
- Start of `Font` system overhaul.
- Fixed bug `DrawString` instruction with cursors.
- Improved `ITexture` from GameHosts. Has more methods like `Get/SetPixel` and `ToSurface`.
- Improved `Cursor` usability.
- Draw method on objects changed to Render to communicate intent.
- LayeredScreenSurface completely rewritten.
- Hosts now use strings to define renderers and allow you to redefine them in the host.
- Surface now have a `TintBeforeDrawCall` which when false tints the screen draw call to allow transparent surfaces.

## v9.0 Alpha 6 (05/28/2020)

- Revamped control rendering. `ControlConsole` renderer now has a 2nd surface which only controls are rendered to. This way the control layer system can update independently of the actual console.
- Fixed bug in renderers which the renderer wasn't attached and detached from the object when added.
- `Cursor` is now a component.
- `Cursor` has a new property named `MouseClickReposition` which when `true` allows a mouse click on the host console to move the cursor.
- `Cursor` a lot of quality of life improvements.
- Removed `ISurfaceRenderData` and went back to normal `IScreenSurface` for renderers.
- ControlHost correctly checks the parent of the control when processing the mouse.
- ControlHost no longer listens to the tab button when a control indicates it handled input
- Textbox bug fixes (thanks Doctor Triagony).

## v9.0 Alpha 5 (05/27/2020)

- Moved `SadConsole.Global` into the `GameHost`.
- Rewrote the UI control framework, removing the need for `ControlsConsole`. Any console can now add the `UI.ControlHost` component and support controls.
  - `ControlsConsole` still exists as a simple way to automatically add the `ControlHost`.
- Settings now expose the defaults for `UseKeyboard` and `UseMouse` for screen objects.

## v9.0 Alpha 4 (05/36/2020)

- Forgot to document the changes.... >.>

## v9.0 Alpha 3 (03/30/2020)

- Controls have a `Tag` property for user-definable data.
- `Window` now has the following features:
  - Shown event when the window is made visible.
  - `OnShown` protected virtual method.
  - `OnHidden` protected virtual method.
- `Draw/Update` have restored the TimeSpan parameter

## v9.0 Alpha 2 (03/14/2020)

- Renamed `Theme.Library.Default.Init` method to `Theme.Library.Default.ApplyDefaults'
- Made `Theme.Library.Default.ApplyDefaults` method public.
- Fixed font loading bug (thanks coldwarrl)

## v9.0 Alpha 1 (01/26/2020)

- New object model based on `ScreenObject`.
- `Console.Surface` represents the cell surface of the console. Editing functions like `Console.Print` moved to extension methods.
- `Font`/`FontMaster` are now merged into a single type: `Font`.
- Font sizes are always represented in an X,Y value now, and are separate from the font itself.\
  Every object that uses `Font` also now has a `FontSize` property.
- All UI objects were moved into the `SadConsole.UI` namespace.
- The theme system was overhauled (and mostly backported to SadConsole v8).
- A lot of misc name changes related to `Console` and `ScreenObject`, like `SadConsole.Input.MouseConsoleState` is now `SadConsole.Input.MouseScreenObjectState`.
- `SadConsole.Global` had a lot of changes.
- `SadConsole.Global.Screen` is the root console displayed on the screen. Everything is attached to it.
- `Entity` base class changed to `ScreenObject`
- App creation is slightly different with a few names migrated and events for draw/update callbacks

  ```csharp
  SadConsole.Game.Create(80, 25);
  SadConsole.Game.Instance.OnStart = Init;
  SadConsole.Game.Instance.FrameUpdate += Instance_FrameUpdate;
  SadConsole.Game.Instance.Run();
  SadConsole.Game.Instance.Dispose();
  ```

## v9.0 (09/27/2019)

- Started work on v9

## 08/14/2020 V8.99.3

- Added `Settings.FullScreenPreventScaleChangeForNone` to keep `ToggleFullScreen` in `None` resize mode. (thanks ASCII Guy)
- Added `ParseCommandClearEffect` with string parser command `ceffect` or `ce`. (thanks regulark)
- Updated templates.
- Update to MonoGame 3.8.

## 07/13/2020 V8.99.2

- Added DrawImageComponent.
- Added templates.
- Fix bug where creating an entity and specifying the font didn't actually set it. (thanks thateye)
- `AnimatedConsole` defaults to `UseMouse = false` (thanks freiling)

## 04/11/2020 V8.99.1

- Ported controls console tab logic from v9. Fixed logic bug when tabbing controls would tab to controls with `control.TabStop = false`.
- Fixed bug with changing the cursor render cell affecting the special effect on the cell. (thanks axoeu)
- Exposed the Theme Library Init method as ApplyDefaults.
- The embedded fonts always load to `Global.FontEmbedded` and `Global.FontEmbeddedExtended`. This way you can provide a new default and also use the embedded versions when you want.
- The theme colors object has some static methods to create either the default theme or a theme based on classic EGA ANSI colors.
- ControlsConsole and Window have a `Invalidated` event that can be used for drawing after the theme as refreshed the console.
- DrawingSurface always calls OnDraw each frame now.

## 11/26/2019 V8.99.0

- **Breaking changes to themes and controls**

  The control themes system is now a lot simpler.

  - Themes only come from two places, the control directly or from the library.
  - Each control has a `ThemeColors` property that controls the colors of the theme. If set to `null` the colors are pulled from the parent console/window.
  - The Console/Window types have a `ThemeColors` property that specify the colors for all controls. If set to `null` the colors are instead pulled from the library.
  - The library has been simplified. Each control type is registered with a theme via the `Library.SetControlTheme`. To overwrite a default theme, set it in the library.

  When you create a new a control, and thus have a new theme, register it with the library `SadConsole.Themes.Library.Default.SetControlTheme(typeof(yourControl), new ThemeType());` When a control is created it queries the library for a theme, which is a clone of the one registered with the library. If you want to override the library-supplied theme, just set the `Control.Theme` property.

  Setting colors for a console or a control is easy. Each control and `ControlsConsole` or `Window` has a `ThemeColors` property. Set this to override the default colors. Setting this at the console level will feed down to all controls. Setting this on a control only affects the control.

I know I'm not supposed to add breaking changes without increasing the major version. However, V9 is already labeled and I wasn't planning on this change. This change was actually created for V9 as a way to solve the problems with v8's theme system. But it proved so simple to implement that I wanted to port it.

Because of the breaking change, I'm setting this version to a strange one, *v8.99*.

## 11/04/2019 V8.9.1

- Fix `FontMaster.Rows` calculation (Thanks Kaev)
- Fix `Themes.ButtonLinesTheme` and all other themes not cloning colors properly. (Thanks Venomaus)

## 08/20/2019 V8.9.0

- Add `Console.Erase`
- Add `Console.GetComponent` and `Console.GetComponents` to find components added to a console.
- Add `CellSurface.CreateLine` to create an array of int's for a line.
- Instructions supports a `RemoveOnFinished` property now.

## 07/27/2019 V8.8.1

- Fix docs

## 07/27/2019 V8.8

#### Engine

- Added built-in CurrentScreen debugger. Invoke with `SadConsole.Debug.CurrentScreen.Show()`.
- Fixed ContainerConsole not processing components (thanks Chris3606).
- Fixed bug with `Console.SetSurface` not marking the console as `IsDirty = true`.
- `Console.SetGlyph` now accepts a `GlyphDefinition` name.
- Minor speed improvement to `console.Fill`.
- Fixed **really bad bug*- that caused `console.Print` to skip printing if the length of the text ran off the end of the console.
- Fixed **really bad bug*- that caused `console.Parent` assignment to result in `console.Parent` becoming null.
- **BREAKING CHANGE*- `Console.OnFocused` and `Console.OnFocusLost` are now public. 
- Console no longer calls `OnFocused` and `OnFocusLost` when `IsFocused` is set. Instead, `Global.FocusedConsole` calls the appropriate focused callback. Fixes #218 (thanks Aezoc)

#### Controls
- Fixed minor bugs related to themes.
- Added a Label control.
- The `ButtonLinesTheme` uses decorators when the extended font is enabled. This enables clean 1 height buttons that don't have cells drawing over others.

## 06/22/2019 V8.7.1

- Added `Settings.DefaultConsoleUseKeyboard` to control if consoles (when created) will use the keyboard or not. Current value is `true`.
- Fixed mouse not being processed correctly when the screen bounds don't align with the game window.
- Fixed bug with holding down a key on the keyboard not respecting RepeatDelay. (thanks Aunel)
- Fixed mouse processing when the parent is not visible but a child is. (thanks hootless) Fixes #216
- Minor bug fix introduced with 8.7.0

## 04/13/2019 V8.6.0

- `SadConsole.Timer` is a component now. The callbacks were changed to events.
- `FontMaster.GetGlyphDefinition` added.

## 03/29/2019 V8.5.0

- `ScrollingConsole` no longer crashes when creating a big console on DirectX (Thanks Chris3606)
- `EntityViewSyncComponent` and `MultipleConsoleEntityDrawingComponent` both have a `HandleIsVisible` property that if set to `false` will cause the component to **not*- set `entity.IsVisible` at all. It still syncs the render offset though. (Thanks Nelson, Chris3606)

## 03/18/2019 V8.4.1

- Updated Themes.Colors to work a little better.

## 03/17/2019 V8.4.0

- Added a + operator for ColoredString and string.
- Removing components from a console works now.
- Added OnColorsChanged to library so you can update control colors on custom controls
- Fixed being able to set Window/Console theme to null.

## 03/09/2019 V8.3.0

- `Cursor.Move` now has an overload for `int x, int y`.
- The `Global.CurrentScreen` defaults to a new `Console` instead of a `ContainerConsole`.
- `Settings.ResizeMode` now supports `None` which helps the dynamic console scenario.

## 03/01/2019 V8.2.0

- Fixed a bug in `WindowTheme.Clone`
- Trimmed Cell.Decorators memory usage
- Various Decorator bugs.
- Added `surface.ClearDecorators`

## 02/27/2019 V8.1.0

- Reenabled `Control.AlternativeFont`
- Rebuilt repo structure
- NuGet package supports **.snuget*- and **SourceLink**
- Rectangle extensions added: `ToPixels`, `ToConsole`
- Keyboard methods like IsKeyPressed now ignore SHIFT and support checking AsciiKey for SHIFT state
- CAPSLOCK and NUMLOCK no longer use windows dll

## 02/24/2019 V8.0

Special thanks to the SadConsole Discord on all the feedback provided.
Shout to the GoRogue project.

[SadConsole GitHub](https://github.com/SadConsole/SadConsole)
[SadConsole Documentation](http://sadconsole.com/docs/)
[SadConsole Discord](https://discord.gg/mttxqAs)

File changes:

- SadConsole and SadConsole.Universal have been replaced by the SadConsole for .NET Standard library.

Code changes:

- Overhaul of the theme system.
- Overhaul of the SurfaceBase/ScreenObject/Console system.
- Changed Listbox.Slider to Listbox.Scrollbar. Also changed any property that used the word Slider to ScrollBar
- Window messagebox and prompts have a library parameter to theme. If not specified, uses the default theme.
- [Fixed #165] Window stealing mouse focus from scrollbar (thanks VGA256)
- [Fixed #164] Controls should be aware of what theme is being used 
- Upgraded to MonoGame 3.7
- Renamed base types and removed some others
- SurfaceBase is now CellSurface and not abstract
- ScreenObject is now Console and inherits from SurfaceBase
- Console is now ScrollingConsole.
- AnimatedSurface is now AnimatedConsole.
- LayeredSurface is now LayeredConsole and has a new subtype, CellSurfaceLayer.
- Mouse input works on all objects that inherit from Console, which is everything besodes CellSurface.
- Added new ConsoleComponent types.
- Consoles have a collection of Components that can hook into Update, Draw, Keyboard, Mouse methods.
- EntityManager is now a ConsoleComponent.
- Instructions reimplemented as ConsoleComponents.
- InstructionSet has fluent-type methods to construct the set. See the SplashScreen console in the demo project.
- [Fixed #199] New PredicateInstruction (thanks VGA256)
- SurfaceBase.DefaultBackground is now Black instead of Transparent. You may need to use console.DefaultBackground = Color.Transparent; console.Clear(); to restore old behavior.
- Control.CanFocus is now respected.
- Fixed CellSurface JSON converter.
- Fixed ControlsConsole renderer not respecting viewport.
- Fixed inputbox not working with numbers.
- Redesigned ListboxItemTheme to be passable on Listbox ctor.
- Consoles no longer use a cached drawcall.
- Window resize option Settings.ResizeMode now supports Fit. (Thanks ajhmain)
- Console.CalculatedPosition is now in pixels. Consoles of different font sizes now play nice with parenting. Things will be offset properly now.
- Settings.ResizeWindow method allows you to easily resize the game window.
- Added TextBox.KeyPressed event to allow you to cancel input of keys on a textbox.
- MouseWheel support has been added to the listbox control.
- Listbox.SingleClickItemExecute has been added to allow the ItemExecute event to trigger without a doubleclick.

## 12/29/2018 V7.3.0

- Windows now default MoveToFrontOnMouseClick = true.
- Consoles are brought forward and focused via LeftMouseDown instead of LeftMouseClick. (Thanks VGA256) #188
- You can set ListBox.SelectedItem = null now (Thanks darrellp) #183
- TextBox supports moving the cursor and inserting characters now (Thanks darrellp) #145
- Window could steal the mouse focus while dragging some other control across the title bar (Thanks VGA256) #165
- ColoredString.Parse now uses CultureInvariant (Thanks GPugnet) #173
- SadConsole.Standard was missing the non extended font embedded resource.
- Various helper methods in ColoredGlyph, ColoredString, and SurfaceBase added (Thanks INeedAUniqueUsername) #187

## 11/19/2018 V7.2.0

- ControlsConsole.Controls collection now uses a foreach loop to make sure the collection is not modified when processing the mouse.
- Button theme would crash if ShowEnds was on and the width of the button was < 3. (reported by Hoonius)
- Renamed and promoted the method that forwarded entity animation states to the entity: OnAnimationStateChanged. Override this on custom entities to detect the state changes.
- Blink event did not respect the BlinkCounter property. (reported by Hoonius)
- Default font is now a non-extended IBM 8x16 font.
- Added SadConsole.Settings.UseDefaultExtendedFont setting and when set to true, loads the IBM 8x16 extended font instead of the normal font. Must be set before creating the game.

## 10/18/2018 V7.1.0

- Moved SurfaceBase.GetIndexFromPoint to Helpers class.
- Fixed bug in EntityManager that did not remove entity/hotspot/zone parents when the EntityManager's parent was cleared.
- EntityManager does not support .Clear calls on collections. Instead, use the RemoveAll extension method.
- An extension method was added: SurfaceBase.CenterViewPortOnPoint
- An extension method was added: Rectangle.CenterOnPoint

## 09/08/2018 V7.0.3 / 7.0.4

- Fixed bug with textbox displaying two carets.
- If TextBox was first control in console, rendering was wrong.
- Added int overload for Helpers.*Flag related methods.

## 08/30/2018 V7.0.2

- Fixed render bug with Entity/Animation if no parent was attached.
- Fixed ColoredString + operator.
- SadConsole IBM Extended font embedded in library now.

## 08/29/2018 V7.0.0

- Draw(SpriteBatch batch, Point position, Point size, Font font) has been removed.
- Cell/CellState have a Decorators list which are used to add extra glyph draws to individual cells.
- CellDecorator class added that has a color, glyph, and mirror setting.
- SadConsole.Serialization uses Newtonsoft.Json instead of the default .NET classes.
- SadConsole.Serialization supports GZIP compression now.
- Settings.SerializationIsCompressed can be set to true to set all internal save/load to use compression.
- New SurfaceBase class which all Surface's inherit from.
- SurfaceBase has an IRenderer on it directly now.
- SurfaceEditor has been removed and is now implemented on SurfaceBase directly.
- Console no longer combines Renderer and TextSurface for drawing.
- Control themes completely rewritten. Themes control all drawing for a control now.
- Windows/ControlsConsole use a theme for drawing.
- InputBox renamed TextBox.
- Removed GameHelpers namespace. Types moved to root namespace.
- GameObject renamed to Entity.
- Surface.RenderArea changed to Surface.ViewPort
- Readded Zone and HotSpot types.
- Removed random level generation.
- Added Entities.EntityManager which helps control entity visibility and offsets based on a parent console. Also handles zones/hotspots.

## 06/11/2018

- Fix a stack overflow problem in the window object introduced by the previous mouse bug fix.
- Cell states can be stored and restored with variables now. Each cell still has a backing cellstate that can be used.
- NoDrawSurface.FromSurface is now static. (as it should have been)

## 04/06/2018

- Fixed bug in Mouse processing. (Thanks VGA256)

## 03/18/2018

- Added WindowResized event to the Game class.
- Added some new color extension methods (clear channel, alpha only).

## 03/17/2018
      
- Added Settings.WindowMinimumSize.

## 03/16/2018

- Controls now refresh when you change the theme with Control.Theme = variable.
- Fixed keyboard problem with selection button.
- Font class now has a link back to the FontMaster instance.
- Added SadConsole.Settings.GraphicsProfile for monogame hidef vs reach.
- Added a Mouse.Clear to clear state (like keyboard has)
- Game has a OnDestroy callback now for when the game starts to shut down.
- VirtualCursor bugs fixed. Also supports print effects again.
- DrawString instruction uses a VirtualCursor all the time (.Cursor property) to fix issues with printing. (thanks vga256)
- VirtualCursor uses the solid square character as default now instead of underscore.
- FontMaster caches Font objects created with GetFont. The same instance is passed around now instead of a new one.
- Fade effect has UseCellDestinationReverse to reverse the logic of the of using the back/fore cell color.

## 02/14/2018

- Keyboard input was reversing '[' with '{' and ']' with '}'
- Controls now refresh when you change the theme with Control.Theme = variable.

## 02/10/2018

- Fixed logic for TabStop with ControlsConsole. It was stopping a focused control from being tabbed off of, not stopping a control from being tabbed to.

## 02/01/2018

- SurfaceEditor now supports basic shape functions. Easier to draw shapes than previous model. (Line only supported for now, more to come)
- Lines can be auto connected through static SurfaceEditor.ConnectLines method.
- Promoted a bunch of private members for the Window class to protected.
- Window title row can be positioned by the class now. Use the protected titleLocationY variable.
- ListBox.HideBorder instantly redraws instead of waiting for a `IsDirty` flag.

## 12/24/2017

- Added SadConsole.GameHelpers.Directions class and helpers.

## 11/14/2017

- Fixed the possability of the GraphicsDevice.ViewPort being changed by a console rendered during the Update loop.
- Fixed the possability of the last surface created during the Update loop being erased.
- Added progress bar control.

## 09/26/2017

- Added new FNA library support. Mostly untested.

## 08/11/2017

- Fixed mouse processing other consoles after it was found over the top-most
- Created a LayeredSurface.Load single param overload to block accidentally calling BasicSurface.Load
- MouseHandler for a Console has changed. If it assigned, the normal mouse processing logic will no longer run.

## 08/09/2017

- Fixed effect manager remove methods.

## 07/25/2017

- Added new Palette and Timer classes.
- Added extension method to ColorGradient that returns a Color[] based on the stops.
- Fixed a bug in InputBox that happened when you clicked twice on the control. Thanks arxae and naknode!

## 06/08/2016

- Fixed BasicSurface ctor. Now references the passed in cell array instead of copying it.
- Surface.ResetArea has been renamed to Surface.SetRenderCells.
- Exposed Surface.SetRenderCells as public.

## 04/22/2016

- LayeredSurface and SurfaceView did not reset the RenderTexture when font changed.
- LayeredSurface deserializes the render view correctly now.
- Scene did not serialize correctly.
- GameObject did not recalculate position when PositionOffset was changed.
- New NoDrawSurface added. Does not use LastRenderResult.
- Normal surfaces dispose of LastRenderResult in the destructor.

## 04/17/2016

- Shapes were not marking the surface as dirty.
- Fixed bug in listbox when parent console used a render view.
- A window did not calculate positioning of children.
- Layered surface has RenderArea parameter for constructor now.
- GameObject had a bug that prevented deserialization.

## 03/31/2016

- `IScreen.RelativePosition` is now `IScreen.CalculatedPosition`.
- `GameObject.PositionOffset` is back.
- `GameHelpers.Scene` is back.
- Fixed a bug calculating the mouse cell on a scrolled console.
- Resized window back to original size when existing fullscreen to handle monogame bug.

## 03/16/2016

Major update to all of SadConsole.

A lot of refactoring has happened. Many of the types in SadConsole have been moved around to a more logical position and a lot of redundancy has been removed. 

For example, the **Console*- type used to be located at the `SadConsole.Consoles.Console` which felt very obtuse. **Console*- is a core type that doesn't need its own namespace, much like **Cell**. This type is now located in the root namespace `SadConsole`.

Other things have simplified naming too, like `Console.CanUseKeyboard` is just `Console.UseKeyboard` now.

### Core engine

`SadConsole.Engine` has been removed. Its role was to coordinate all the parts of SadConsole. Instead this has been split into three parts, `SadConsole.Global` which represents state (time passed, keyboard/mouse, current thing to render), `SadConsole.Game` which is the `MonoGame.Game` instance, and `SadConsole.Settings` which provides full screen, toggle drawing on/off etc.

| Class |     |
| ----- | --- |
| SadConsole.Global   | Global state, like time elapsed, keyboard/mouse input state, the active thing to render. |
| SadConsole.Game     | `Microsoft.Xna.Framework.Game` game instance that you can run instead of providing your own. |
| SadConsole.Settings | Various settings like fullscreen, device clear color, enable/disable keyboard or mouse, other settings. | 

### Core types

The `SadConsole.Consoles` namespace does not exist anymore and is instead broken up into two different namespaces that better represents the types contained in it:

| Namespace |     |
| --------- | --- |
| SadConsole.Surfaces  | All types of surfaces that are attached to a Console. |
| SadConsole.Renderers | All renderers that render surfaces and are attached to a Console. |

The **TextSurface*- naming convention has been simplified to **Surface**. And some of the interface and base class complexity of the **TextSurface*- stuff has been simplified into fewer types.

### Rendering

The rendering system in SadConsole has had some improvements. Instead of each *- Renderer*-  having its own *- SpriteBatch*- , there is a single `SadConosle.Global.SpriteBatch` which is reused by all renderers. This reduces memory and reduces CPU cycles that were wasted everytime a renderer was created.

Each **Surface*- now provides a **RenderTarget2D*- type which is a texture. Whenever a surface is rendered, it is drawn onto this texture. At the end of the global Draw call, all surfaces that are in the drawing pipeline are rendered to a single **RenderTarget2D*- texture at `SadConsole.Global.RenderOutput`. This final texture (which contains all drawing from SadConsole) is then drawn to the screen. This simplifies fullscreen and stretch modes. This new system also allows anyone to bypass any part of SadConsole rendering and use the rendered textures to draw on any sort of 3D model or scene of their game. For example, you could build up a 3D scene of an old computer terminal and then use SadConsole on the screen of the computer.

The rendering system is now completely cached. Each **ISurface*- type has a **IsDirty*- flag which causes the backing **RenderTarget2D*- to be updated.

### Notable types

Here is a list of types that have changed and what replaced them. The root `SadConsole` namespace is implied in all of these.

| Old Class        | New Class     |
| ---------------- | --- |
| Engine           | Replaced by **Global**, **Game**, and **Settings**. | 
| ICellAppearance  | Removed - Use **Cell**. |
| CellAppearance   | Removed - Use **Cell**. |
| Consoles.IConsole     | IConsole. Still exists, implements **IScreen*- now. |
| Consoles.IConsoleList | Removed. |
| Consoles.Console      | Console |
| Consoles.ConsoleList  | Removed. All **IScreen*- types have both **Parent*- and **Children*- properties. |
| Consoles.ITextSurface        | Surfaces.ISurface |
| Consoles.TextSurfaceBasic    | Removed. Merged into **Surfaces.Surface*- |
| Consoles.TextSurface         | Surfaces.BasicSurface |
| Consoles.TextSurfaceView     | Surfaces.SurfaceView  |
| Consoles.AnimatedTextSurface | Surfaces.AnimatedSurface  |
| Consoles.LayeredTextSurface  | Surfaces.LayeredSurface  |
| Consoles.Cursor              | Cursor  |
| Consoles.SurfaceEditor       | Surfaces.SurfaceEditor  |
| Consoles.ITextSurfaceRenderer       | Renderers.ISurfaceRenderer |
| Consoles.TextSurfaceRenderer        | Renderers.SurfaceRenderer |
| Consoles.LayeredTextRenderer        | Renderers.LayeredSurfaceRenderer |
| Consoles.ITextSurfaceRendererUpdate | Removed - All surfaces support cached rendering. |
| Consoles.CachedTextSurfaceRenderer  | Removed - All surfaces support cached rendering. |
| Input.MouseInfo       | Renamed to Input.Mouse |
| Input.KeyboardInfo    | Renamed to Input.Keyboard | 

Besides the **Consoles*- namespace, startup, and **Engine*- -> **Global*- changes, not much else has changed.

Some methods and/or properties have been renamed. Here are some of them.

| Old name              | New name |
| --------------------- | -------- |
| Input.Keyboard.ProcessKeys | Input.Keyboard.Process |
| InputIndicatesProcessMouse   | InputIndicatesProcess |
| Engine.ActiveConsole       | Global.InputTargets -- This is a new type that allows a Push/Pop/Set system for who gets keyboard/exclusive mouse input |

### Input

Input has been overhauled a bit. Keyboard is mostly the same except for some minor method refactoring. Mouse has change a lot. Previously each console evaulated mouse state for itself. This is no longer how mouse input works. Instead mouse input is driven by the `SadConsole.InputIndicatesUpdate` method which cycles through the `SadConsole.Global.Screen` gathering all console types. Then, each console has the `ProcessMouse` method called. If `true` is returned, mouse processing stops. This happens unless the `Global.InputTargets.Console` has the `IsExclusiveMouse` property set to `true`. If `true`, mouse is always sent to this console and never to anything else.

### Startup code

The code to start SadConsole from a dedicated SadConsole project is pretty much the same. But now that `Engine` is gone, `Global` is used and the names of the draw/update events are simplier. They also are direct delegates instead of event.

```csharp
static void Main(string[] args)
{
    // Setup the engine and creat the main window.
    SadConsole.Game.Create("IBM.font", 80, 25);

    // Hook the start event so we can add consoles to the system.
    SadConsole.Game.OnInitialize = Init;

    // Hook the update event that happens each frame so we can trap keys and respond.
    SadConsole.Game.OnUpdate = Update;

    // Hook the "after render" even though we're not using it.
    SadConsole.Game.OnDraw = DrawFrame;
            
    // Start the game.
    SadConsole.Game.Instance.Run();

    //
    // Code here will not run until the game has shut down.
    //
}

private static void DrawFrame(GameTime time)
{
    // Custom drawing. You don't usually have to do this.
}

private static void Update(GameTime time)
{
    // Called each logic update.
}

private static void Init()
{
    // Any setup
}
```

## 01/06/2016

### Core

- Full screen works even better now (Monogame only)

### Controls

- Fix for window.center in full screen mode

## 01/03/2016

### Core

- Cursor can now be created with just a text surface alone. Allows for better standalone (no console) scenarios.
- MonoGame supports init with just a standalone graphics device object.
- Fixed bug in [c:u] (the undo command) removing in the wrong order.
- Updated code for fullscreen support. Use Engine.ToggleFullscreen(). (Monogame only)

### Controls

Changed Button and SelectionButton control.
- New base class.
- Constructor changed.
- Event name ButtonClicked simplified to Click.
- Button click method Click changed to DoClick.

### Ansi

- Loading multiple docs through a single writer now correctly resets the cursor state.

## 11/23/2016

### Core

- Point extension ToPositionMatrix added.
- SadConsole.Engine.DeviceManager is now set to the instance used to init monogame.
- When deserializing a Console type, the TextSurface did not have the RenderCells set properly.

### GameHelpers

- Added RangeInt and RangeDouble types for generating a value between two numbers.
- Fixed some bugs with init of a scene.

## 10/24/2016

### Core

- Added DoUpdate and DoRender flags to Engine.
- MonoGame correctly calls the Shutdown event now.
- AnimatedTextSurface.State can be set publically now.

### GameHelpers

- Added AnimationStateChanged to GameObject which forwards the AnimatedTextSurface.AnimationStateChanged event automatically.

## 10/09/2016

### Core

- Helper method added to cells that lets them render themselves to screen without the TextSurface system.
- Fixed LayeredTextSurface saving out information from base class when it is already contained in the Layers array.
- Fixed LayeredTextSurface Add method that did not actually add a new layer if you supplied a source TextSurface.

### GameHelpers

- Improved debug attributes of zone.
- Added hotspot type.

## 10/05/2016

- Minor bug fixes all around.

## 10/01/2016

### Core

- Minor bug fixes.

### GameHelpers

- Added a Zone type and a Scene type.

## 09/26/2016

### Core

- AnimatedTextSurface now clears the frame from CreateFrame with the default foreground and background colors.

### Controls

- Fixed possible recursive bug with mouse handling that Core had but was also in the Window console type.
- Refactored Control base class. Protected members modified.
- Added Control.Bounds for calculating the space the control uses visually.

## 09/11/2016

### Core

- Fixed possible recursive bug with mouse handling when calling ProcessMouse on console B while in ProcessMouse on console A.

### Controls

- Fixed released (previously captured) control not giving back Engine.ActiveConsole.
- Scroll bar now hides slider position when IsEnabled = False.

## 09/08/2016

Versions are no longer the same across all libraries.

### Core

- Updated all serialization for base types.
- TextSurfaceView now a proper type that doesn't inherit from TextSurface for its function.
- Cursor supports using the ColoredString parser system.
- Fixed RexPaint support.
- Main game window centers after resizing.
- Virtual cursor now support wrapping lines at the word level and linux line endings. 
- Rewrote the initialization system. Cut out the MonoGame.Game object and wrapped it behind the scenes. Easier for new users.
- Fixed various bugs in the AnimatedTextSurface.

### GameHelpers

- Reworked serialization.
- GameObject now implements ITextSurfaceRendered, bypasses using the Animation for rendering and instead consumes the animation data.

## Version 3.1.0

- Fixed bug with effects that are added more than once not knowing they already existed in the effects manager.

## Version 3.0.0 (06/07/2016)

- Rewrote how the Console interacts with backing data and rendering.
- CellSurface is gone and is replaced by TextSurface.
    - TextSurfaces have viewports and the interface to read/write to the cell data.
    - TextSurfaceView is an additional type that surfaces part of a TextSurface as a new type. Cells are shared between the two but the view uses a separate set of x,y coordinates.
    - Cannot be resized anymore. Create a newly sized surface and copy the data to it.
- CellsRenderer is gone and is replaced by TextSurfaceRenderer
    - This now is a very trimmed down class that *only- handles drawing to the screen.
    - ViewPort systems were moved to the TextSurface.
- Engine initialization was simplified with a lot of helper code. Starting game construction should be quicker and easier.
    - Startup code reduced from around 13 lines to 1 line.
- Decoupled Console.Cursor from Console. It's now its own class and uses IConsole.
- Controls library updated.
    - Now that resize on a surface isn't supported, many controls are built differently. Some constructors changed.
    - Fixed a bug that stopped processing mouse on any control added after a scroll bar.
    - Windows now use a WindowRenderer instead of the standard renderer.
- Added a new Print overload for a text surface that allows you to opt into which things you want to update (foreground, background, spriteeffect).
- Entity system removed. Replaced with Consoles.AnimatedTextSurface and in the GameHelpers library, SadConsole.Game.GameObject
- Tons more... https://github.com/Thraka/SadConsole/pull/26

## Version 2.0.1.1 (10/13/2015)

- Updated the NuGet package and wiki. When you install from NuGet you get font files and a browser page will open that points the NuGet starter page on the wiki.

## Version 2.0.0 (4/24/2014)

#### All Libraries

- __Biggest change:__ refactored SadConsole to use shared projects for easier code maintenance and cross-platform usage. Currently only Windows projects exist; pull requests welcome for OS X and Linux (should be fairly easy).
- Dropped XNA support (could be re-added with shared projects, pull requests welcome!).
- Moved to MonoGame 3.3.
- Transitioned to NuGet for our MonoGame dependency.
- Added NuGet support

#### StarterProject

- Replaced call to Microsoft.Xna.Framework.Game.Exit with a call to Environment.Exit, as XNA's is obsoleted in MonoGame 3.3.

## Version 1.3.115 ()

#### Core

- Added some CellSurface.Fill aand FillArea overloads.
- Fixed bug in CellApperance where it copy the SpriteEffect.
- Added CellsRenderer.RenderBox which returns a rectangle of the area of the screen that will be drawn on.
- Added Consoles.LayeredConsole which uses multiple cellsurfaces in a single renderpass. You set the current CellData property by calling .SetActiveLayer(index).
*- It's not much more effecient than using multiple consoles, however it cuts down on the overhead slightly. The console editor tool uses this type.
- Added load/save methods to console, cellsurface, and layeredconsole. More to come.
- Engine.Serializer now has a Save/Load method that does file handling for you. The other Serialize/Deserialize methods only use a stream.
- CellSurface Copy Fixes.
- ConsoleList can now become "focused." Unsure why I had it throw an exception in the past.
- Save/Load removed from various types. Use SadConsole.Serializer instead.

#### Entities

- BREAKING CHANGES
- Entity no longer uses a dictionary to hold the animations, it now just uses a list.
- Animation supports changing the name now.
- Entity can play an animation that is not part of the added animations.
- Added Animation.Restart method.

#### Controls

- Fixed a bug when removing items from a listbox.
- Added some quick helper dialogs to Window. Static methods for prompting a yes/no question and notification.

#### GameHelpers

- Created new binary called SadConsole.GameHelpers.
- A GameObject type has been created (along with a collection type) which represents a point on the console. 
*- Generally these aren't rendered, but they allow you to query the collection for a GameObject named X or one located at a specific point. You can also do queries that return all GameObjects with a specific color or character for example, using lync.
*- GameObjects have a Settings collection which represent name-value-pairs. For example, create a Door GameObject which a setting named Destination and a value of the name of another file. Then code your game to look for when something touches the same location as the GameObject and then move it to the appropriate screen.
- GameConsole is a LayeredConsole with special metadata that contains a GameObjectCollection for each layer.

## Version 1.3.114 (12/12/2014)

#### Core
- Fixed logic flaw in CellSurface.Copy which caused invalid cells to be requested for copy.
- Other misc fixes discovered or needed by the SadConsoleEditor project.
- Added Circle and Ellipse shapes along with Algorithms.

#### Controls

- Added CheckBox.

#### Starter Project

- Fixed bug that caused the splash screen to run too fast.
- Added another console which is just displays the output of the RandomGarbage method.

## Version 1.3.113 (9/23/2014)

#### Core

- Added Entity.PositionOffset which is now taken into consideration when rendering an entity.
*- By setting this property to the position of a console (when the console and entity have the same cellsize) will cause the entity to look as if it's hosted by the console.
- Cell now has a SpriteEffect property which allows you to mirror and flip the cell to create more characters.
*- Also implemented on the ICellAppearence 
- CellsRenderer now correctly resets the "custom view" flag if a custom view is set to the exact dimensions of the surface.

#### Controls

- Fixed a bug in rendering a controls console with the ViewArea changed to start at something other than 0,0
- Fixed some drawing bugs in ListBox


## Version 1.3.112 (9/7/2014)

#### Core

- Added the SadConsole.EngineGameComponent XNA-based GameComponent.
*- This simplifies initialization and framework code you need to hook up in your Game class. Added just like any other GameComponent object. 
- Added some new methods to the Color extensions provided by this library
*- RedOnly, GreenOnly, and BlueOnly. These methods will return a new color with only the appropriate RGB channel set from the existing color.


#### Controls

- Fixed a bug with the listbox.items that caused newly added items to render only in black & white until the mouse moved into the control area.
- Added Listbox.ideBorder property to show/hide the border of the box.
- Added ControlBase.AlternateFont property which allows a control to render with a different font than the ControlsConsole has.
*- Take care to keep the font sizes the same though, otherwise it will look weird.
- Fixed bug in inputbox which triggered the TextChanged event when the Text property was set to the same value it already was.
- Slider control is easier to use when dragging the current position.

#### Starter Project

- Added an example character viewer window which is shown by pressing F2



## Version 1.3.111 (4/15/2014)

- Upgraded the references to MonoGame to the official 3.2.

#### Core

- Added a new virtual method for CellRenderer: OnCellDataChanged. This is called when the CellData property changes, passing references to the new and old CellSurfaces.
- Loading/Saving a cellsurface was not processing the effect information.
- Entities now implement some of the IConsole interface (those relating to input) as virtual methods so you can override them in derived classes.
- Entities now correctly create a default animation that can render. (though it is blank)
- Some ground work for rendering an entity using another consoles sprite batch.
- CellsRenderer.Batch set has been promoted to protected from private.


#### Controls

- When a window is shown, and then shown again in the future, it will pop back up to the top of the screen when not removed from its parent.



## Version 1.3.110 (2/11/2014)

#### Core

- Resizing a surface had a crash bug in it in certain edge cases where when you resized one edge smaller than the opposite side.

#### Effects

- Fixed bug with Recolor that when it was removed from a cell would set the background to the original foreground.
- Fixed a bug with the cellsurface which was causing it not to call the Effect.Clear method when an effect was removed.

## Version 1.3.109 (2/10/2014)

- !!! MERGED the animation library into the core library. I didn't see a logical reason to have this be its own library...
    - Remove reference to the SadConsole.Animation.dll from your projects.
- !!! MERGED the effects library into the core library. With so few effects, that are handy to have, I didn't see a need for it to have its own library.
    - Remove reference to the SadConsole.Effects.dll from your projects.
    - Remove any calls to SadConsole.Effects.EffectsLibrary1.Register()
- SadConsole splash screen now provided by the engine. Must be called at the start of any application using SadConsole.
- Starter project updated with the SadConsole splash screen.

#### Core

- Changes to Console.Cursor:
    - Added AutomaticallyShiftRowsUp property. Defaults to true. Set to false if you do not want the console to shift its rows up when the cursor travels past the last cell.
    - Promoted CursorRenderCell from a field to a property.
    - Promoted PrintAppearance from a field to a property.
    - Removed CursorAppearance property as it is already exposed via the CursorRenderCell property.
    - Added DataContract attirbute to Cursor class.
    - Created a Render method that the Console class will now call when rendering the cursor instead of doing it itself. 
    - Added the PrintOnlyCharacterData property, defaults to false.
        - This allows you to override this method in a derived class to control how the cursor appears on the screen.
    - Removed CursorCharacter, use CursorRenderCell.CharacterIndex instead.
- When a Cell copies itself to something else, if the CharacterIndex is -1, it will skip copying the CharacterIndex.
- Added SadConsole.Algorithms.GradientFill. (Thanks StackOverflow.GameDeveloper [micklh and anko])
- CellSurface now implements IEnumerable<Cell>.
- CellSurface respects ICellEffect.CloneOnApply when an effect is added to a cell.
- Minor refactors to CellSurface and Console.Cursor.
- Added an extension method to the XNA Texture2D class, DrawImageToSurface which will draw a texture to a CellSurface.
- Added the ColorGradient class to the Microsoft.Xna.Framework namespace, which represents a gradient with multiple color stops.
- Cursor supports printing a ColoredString object.
- Added explicit serialization to all effects.
- Fixed bug in the input library that detected the ? as / and / as ?

#### Effects

- ICellEffect now has a property named CloneOnApply which flags the effect that when applied to a CellSurface Cell, it should be cloned rather than used directly.
- Added a new base class which implements ICellEffect, named CellEffectBase. All the existing effects no inherit from it instead of implementing ICellEffect directly.
- FadeForeground, FadeBackground, and Pulse effects have been removed.
- Fade effect has been changed to indicate if it should apply to the foreground, background, or both colors of a cell.
- Fade effect has added the AutoReverse property to emulate the Pulse effect that has been removed.
- The EffectChain effect has been moved to this library from Core.
- Created the Delay effect.
- Created the ConcurrentEffect which allows you to add more than one effect to it, all of which process and apply at the same time.
- Added explicit serialization to all effects.

#### Animation

- Added the DoubleAnimation type which calculates a double value over time using an easing function
- Added the CodeInstruction instruction which calls a delegate method.
- Added the DoubleInstruction instruction which calls a delegate using a DoubleAnimation object.
- Added the FadeCellRenderer instruction which animates the tint of a cell renderer.
- Added the EasingFunctions namespace with some easing types used by the DoubleAnimation type.
    - Linear [ this is a non-easing, straight linear calculation from one value to another ]
        - If you use EasingMode.None on any easing class, it will default to linear.
    - Bounce
    - Circle
    - Expo
    - Quad
    - Sine
    - More to come in future versions...
- Modified the DrawStringInstruction to use ColoredString instead of a combination of the Text and TextAppearance property.
- Added the ConcurrentInstruction type which, like the InstructionSet class, allows you to add more than one instruction and treat it as a whole. This is different because when the instruction is run, it will run all child instructions at the same time, instead of only the "current" instruction as the InstructionSet does.


## Version 1.3.108

#### Core

- The virtual cursors position is now serialized.
- VirtualCursor now has the LeftWrap method which behaves like the existing RightWrap method.
- VirtualCursor now has the NewLine method which just calls the CarriageReturn and LineFeed methods in a single call.
- Backspace on the console keyboard input uses VirtualCursor.LeftWrap instead of VirtualCursor.Left for moving the cursor.
- The input key processing incorrectly assigned character 0 (null) to the character used by the SPACE key. Now it correctly assigns the ' ' character (#32 I think).

#### Controls
- Added ControlBase.OnPositionChanged() protected method that is called whenever a control moves.
- Fixed bug in List control where the scrollbar didn't scroll anymore with the mouse.

## Version 1.3.107

- Created Starter Project.

#### Core

- Fixed bug with "string".CreateGradient where the string would be empty.
- Fixed bug with printing text that runs off the end of a console not printing.
- Keyboard input on Console no longer prints a blank character for the F1-F24 function keys, ESC, and PAUSE keys.

## Version 1.3.106

- CONTAINS BREAKING CHANGES
- VERSIONING SCHEMA HAS CHANGED [major].[minor].[build].[platform]
    - Right now, the platform schema is:
        - 1 - MonoGame OpenGL
        - 2 - MonoGame DirectX
        - 3 - XNA 4.0 PC
        - 4 - XNA 4.0 XBOX 360
        - 5 - XNA 4.0 Windows Phone 8

#### Core

- Overhaul of cell and effect system.
- CellSurface now manages the effects for cells.
- The Cell.Effect property is just a placeholder for quick reference.
- During the CellsRenderer.Update method, all known effects from the CellData property will run their Update method.
    - Then, the Effect.Apply(Cell) method will be called on each cell associated with the effect.
    - This keeps all cells synched with the effect they are using.
- ICell no longer has the Effect property. Instead the CharacterIndex property has been added
    - This is a better description of the interface, which describes what the cell looks like for rendering.
    - The effect was not part of rendering, only part of changing what the cell looked like.
    - The Cell class have been updated with this change.
- Input - Mouse data and event now have the following new properties:
    - ScrollWheelValue
    - ScrollWheelValueChange
- CellSurface now tracks how many times the rows were scrolled up with the TimesShiftedUp property.
- Moved Ansi functions that were in the Cursor to a new library: SadConsole.Ansi

## Version 1.3.0.105

- MAY CONTAIN BREAKING CHANGES
- DataContract serialization added for the Controls, Effects, Animation, and Core libraries.

#### Core

- Blink, EffectsChain, and Delay were not registered with the engine.
- Serializer class has a ConsoleTypes property which is an array of all the types used by default when serializing anything related to a console, including all registered effects.

#### Controls

- Theme library has changed to be a non-static class with a static property: Default. Use this Default property as the static accessor to the themes instead of the class itself.
    - Allows the entire theme library to be [de]serialized and replaced at any momemt.
- Fixed a bug where the InputBox when IsNumeric true and AllowDecimal false, the . key could be used.
- Added Controls.Serializer which has a single property, ControlTypes. This property is an array of all types related to Window and ControlsConsole. Includes the SadConsole.Serializer.ConsoleTypes.

## Version 1.3.0.104

- CONTAINS BREAKING CHANGES

#### Core

- Revamped the entire Console inherits from CellSurface system.
- The Console class now inherits from CellRenderer which handles rendering cell data
- The CellSurface is implemented as a property on the CellRenderer, allowing you to
    - Change the cell data to be rendered on any console
    - Share the same cell data between consoles to provide multiple views of a single set of cell data
- IConsole now implements IUpdate, IRender, IInput
- Font is no longer attached to the CellSurface, but is on the CellRenderer.
- The CellSurface no longer knows about cell size and only worries about managing cell data.
- Lots of code refactors to be better aligned with intent and common teminology.
    - Location has generally changed to Position
    - Mouse.LeftClicked and RightClicked are now LeftButtonClicked and RightButtonClicked

## Version 1.2.0.103

- CONTAINS BREAKING CHANGES

- Added more code documentation comments
- Converted SadConsole.Consoles.Window public fields to properties and added documentation comments.
- Finished converting all SadConsole.Console.Console public fields to properties.

#### Core

- Engine sprite rendering performance increased untold amount (thanks Shawn Hargreaves)
- The DefaultBackground of a console is now drawn (very very low impact) to cover the entire console before rendering. Then, any background cell render that matches the DefaultBackground is skipped
- Cells that are using character index 0 will skip foreground character rendering.
- Console.AutoCursorOnFocus was not being correctly respected in the Console.Focused and Console.Unfocused method code
- ConsoleList now uses a cached list of consoles when calling the Update, Render, ProcessKeyboard, and ProcessMouse methods
- Added Resized, MouseButtonClicked, MouseMove, MouseExit, MouseEnter events to Console
- Font rows was calculated wrong when using a padding value > 0
- Engine.ScreenCellsX and Engine.ScreenCellsY have been removed.
    - These are useless since a font can be any cell size now, per console.
    - These have been replaced with Engine.WindowWidth, Engine.WindowHeight, Engine.GetScreenSizeInCells(Font), and GetScreenSizeInCells(CellSurface surface)
- CellSurface did not set new cells that are created during resize to the default foreground/background of the surface
- Added Console.OnLocationChanged protected method which is called when the location property value changes
- Added ICell. Cell is no longer the base class for Cell, but standalone
- Added Console.MoveToFrontOnMouseFocus property
- Added the Shapes.Line shape to draw a line on a console. The line can have a starting/middle/ending characters and appearance
- Added ConsoleList.Clear() method
- SadConsole.Serializer has been created which provides two methods for serialization of objects
- Font.Save and Font.Load have been removed. Use SadConsole.Serializer to load/save fonts
- Font files are now JSON based instead of XML
- Added SadConsole.Algorithms class which has handy mathmatical functions
    - Added Line function
    - Added FloodFill function
- CellSurface has two new methods to help get information when you have reference to a cell: GetCellIndex and GetCellPosition
- CellSurface.IsValidCell was not calculating correctly
- Console.VirtualCursor can no longer be null. Use Console.VirtualCursor.IsVisible true to hide the cursor
- Print(int x, int y, string text, Cell appearance) has changed to use ICell
- Perf improvements for ViewPortConsole and Console rendering

#### Controls

- Perf improvements for ControlsConsole rendering
- ControlsConsole.AutoCursorOnFocus defaults to false
- Console.Parent and IParent management methods all work together now
- Mouse processing on a ControlsConsole would send events to a control even if it was hidden
- Minor refinements to how the Window class works. Now keeps track of last active console if shown modal. When hidden, sets focus back to that console
- Fixed bug with scrollbars where if you held the mouse down, and exited the window, the scrollbar still held focus of the mouse events
- Fixed critical bug with controls that captured mouse control and then released it. This killed a modal window's exclusive setting
- Removed arrow-key based tab code from button control
- ControlBase.ProcessKeyboard is no longer abstract, instead it is virtual
- NEW CONTROL: SelectionButton; Provides a button-like control that changes focus to a designated previous or next selection button when the arrow keys are pushed
    - Library has a new SelectionButtonTheme property
- NEW CONTROL: DrawingSurface; A simple surface for drawing text that can be moved and sized like a control
    - Library does not have a theme for the DrawingSurface control. Use the DefaultForeground and DefaultBackground properties
- ControlBase fields converted to properties: CanFocus, IsDirty, TabStop, IsVisible, Location
- Window did not raise the Closed event when Hide() was called
- Removed ControlBase.CanFocus property
- Added ControlBase.FocusOnClick property
- Controls focus on click by default
- Fixed control not losing focus when mouse left screen without touching part of an existing console first
    - ControlsConsole.OnMouseExit now processes the mouse over every control one last time so they all get the exit events
- Fixed bug in Window where it would process the mouse even if it was invisible
- Window will try to show a second time if the Show() method is called while it has a parent and is visible
- Window will detect if it automatically added itself to the render stack and automatically remove itself when closed

#### Input

- MouseEventArgs did not properly set RightClick.
- MouseEventArgs all fields convereted to properties.
- Mouse.RightClicked was not working. (thanks Jannis)

## Version 1.1.0.102

#### Core

- Added RenderTransform property to Console type. If used, the automatic positioning transform will not be used, so the Location property will have no effect.
- Added GetLocationTransform method to Console type. This will return the positioning matrix used by the console render method.
- ColoredString now has four fields that control whether or not the character, foreground, background, or effect should be drawn. These fields should be respected when something uses ColoredString to draw
- CellSurface now respects the ColoredString.Ignore- properties when printing.
- Fixed exception text on CellSurface where it said DrawString. Correctly says Print.
- Replaced IReadOnly<> with System.Collections.ObjectModel.ReadOnlyCollection<> for XNA compatibility.

#### Controls

- TextAlignment on the Button control defaults to HorizontalAlignment.Center.
- WindowStyle now has a FillStyle property which styles the inside of the window.
- Window.Redraw() method updates the border drawing box to use the current theme at time of redraw
- Window.Redraw() method updates the console's default fore\back with the current theme before Clear() is called.
- When console is focused or unfocused, DetermineAppearance() is called on the console's focused control
- Controls now take into account if the parent console is the active (focused) console for the focused appearance
- Shift+Tab now focuses the previous tabbed control.
- RadioButton now displays selected part of the theme when it is selected.
- All control Theme properties are now virtual.
- New Feature: Tabbing from a control console to another control console
    - Added CanTabToNextConsole property which allows the tab system to focus the next console from the parent (if available)
    - Added NextTabConsole property which forcibly sets the next console to tab to (when CanTabToNextConsole is true) instead of the next console in the parent. The console reference must exist in the parent to work.
    - Added PreviousTabConsole property which forcibly sets the previous console to tab to (when CanTabToNextConsole is true) instead of the next console in the parent. The console reference must exist in the parent to work.
    - TabNextControl() method will respect the CanTabToNextConsole property.
    - TabPreviousControl() method will respect the CanTabToNextConsole property.
- Fixed a minor bug in the Window console that would remove the console from ConsoleRenderStack when hidden without verifying that the console was indeed in their.

## Version 1.1.0.101

- Added\Fixed more documentation code comments.
- Refactored and renamed quite a few methods. See the rest of the changes for more information.

#### Core

- When cloning an EffectsChain, the active effect on the clone referenced the clone source instead of the new cloned chain
- Added Delay effect.
- Fixed bug where Cell's effect property didn't behave like Cell.
- CellEffects are all using double for time counting instead of float.
- Moved cell.effect update to the Update method. Was calling it in the Render method for ViewPortConsole.
- CreateColored string extension did not set the backing string information.
- ColoredString has the UpdateWithDefaults() method which will update the backing string foreground, background, and cell effect with the current defaults.
- ColoredString supports the + operator to combine two ColoredString objects into a single ColoredString object.
- More constructor overloads created for the ColoredString and ColoredCharacter classes.
- Blink effect did not respect BlinkCount -1 for infinite blink.
- The addition of the ColoredString class has made a lot of the CellSurface.DrawString methods obsolete. Some were covered by other overloads. The following have been removed:
    - DrawString(int x, int y, string text, Color foreground, Color background, ICellEffect effect)
    - DrawString(int x, int y, string text, Color[] foregrounds, Color[] backgrounds)
    - DrawStrings(int x, int y, string[] strings, Color[] colors)
    - DrawStringGradient(int x, int y, string text, Color startingForeground, Color endingForeground, Color startingBackground, Color endingBackground)
    - DrawStringGradient(int x, int y, string text, Color startingForeground, Color endingForeground)
- The remaining CellSurface.DrawString methods have been renamed to CellSurface.Print
- CellSurface.DrawCharacter(int x, int y, int character) has been removed because it was a duplicate of CellSurface.SetCharacter(int x, int y, int character)
- The following two CellSurface.DrawCharacter overloads were renamed to CellSurface.SetCharacter:
    - DrawCharacter(int x, int y, int character, Color foreground)
    - DrawCharacter(int x, int y, int character, Color foreground, Color background)

#### Effects

- CellEffects are all using double for time counting instead of float.

#### Animation

- Moved cell.effect update to the Update method. Was calling it in the Render method for Entity.

## Version 1.1.0.100

- Renamed namespaces and binaries from SadConsole8 to SadConsole.
- Version # changed to be more in line with the previous SadConsoleXNA engine.

#### Core

- Completed code comments on Cursor and CellApperance.
- Added some helper methods to Cursor, CellSurface, and Appearance.
- Refactored cursor methods that were created only for ansi processing.
- Mouse handling improvements.
- Console bugs fixed.
- Improved cursor support on console.
- Console list doesn't process keyboard if the console isn't visible.
- Adjustments for mouse processing.
- Minor fixes to viewportconsole.
- Enhancements to effects.
- Better chaining control with delays and repeats.
- Misc. bugs fixed in various effects.
- Added StartDelay to ICellEffect.
- Added ColoredCharacter and ColoredString classes.
- Added DrawString overload to CellSurface  which takes ColoredString.
- Enhancements to String extensions to help with building ColoredString objects.
- Minor updates cell effects, fixed bugs not respecting Finished flag.

#### Controls

- Listbox\input\window updates
- Updated default theme colors.
- Minor adjustment to controlsconsole to use the passed in spritebatch during render.
- Minor theme issues fixed with some controls.

#### Animation

- Minor updates on trying to track down bugs on showing entity on top of console.
- EntityRenderer added new render method where you can pass the spritebatch.
- InstructionBase altered.
- InstructionSet finished.
- DrawString and Wait instructions created.

## Version 1.0

This engine is derived from the previous SadConsole engine available for the PC, Silverlight, and Windows Phone 7 platforms. The following list outlines the major changes to the engine since the release of that engine which is now deprecated.

- BasicConsole is now Console
- Cells no longer track their font rectangles and locations. This is now chached in the font object, for the entire font.
- Fonts now support cell padding
- CellSurfaces have taken things from Console that were in previous version (Font, Size)
- CellSurface can designate cell size and font will scale.
- Console object has positioning and drawing
- Console position is now applied to a transform instead of individual cell rects as in the old engine.
- Engine has been simplified and is no longer a static class
- Engine now has a ConsoleList that is used strickly for rendering consoles
- Engine now has an ActiveConsole which gets keyboard and mouse events.
- IProcessKeyboard and IProcessMouse have been integrated into IConsole directly
- EffectChain is renamed to EffectsChain
- Engine effects registration has changed and can now load the instance of an effect for you.
- Consoles now support absolute pixel positioning in addition to cell positioning.
