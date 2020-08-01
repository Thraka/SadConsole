## v9.0 Beta 1

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

## v9.0 Alpha 9

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

## v9.0 Alpha 8

- Fixed a lot of LayeredScreenSurface bugs.
- **SFML**: Renamed `Renderers.LayeredScreenObject` to `Renderers.LayeredScreenSurface`
- Removed `TintBeforeDrawCall`. Not a good design.
- Added `SadConsole.Instructions.AnimatedValue` to animate a value over a duration.
- Updated `SadConsole.Instructions.FadeTextSurfaceTint` to use `AnimatedValue`.
- `Renderers.ScreenSurfaceRenderer` has an `Opaqueness` setting to render a console surface transparent.

## v9.0 Alpha 7

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

## v9.0 Alpha 6

- Revamped control rendering. `ControlConsole` renderer now has a 2nd surface which only controls are rendered to. This way the control layer system can update independently of the actual console.
- Fixed bug in renderers which the renderer wasn't attached and detached from the object when added.
- `Cursor` is now a component.
- `Cursor` has a new property named `MouseClickReposition` which when `true` allows a mouse click on the host console to move the cursor.
- `Cursor` a lot of quality of life improvements.
- Removed `ISurfaceRenderData` and went back to normal `IScreenSurface` for renderers.
- ControlHost correctly checks the parent of the control when processing the mouse.
- ControlHost no longer listens to the tab button when a control indicates it handled input
- Textbox bug fixes (thanks Doctor Triagony).

## v9.0 Alpha 5

- Moved `SadConsole.Global` into the `GameHost`.
- Rewrote the UI control framework, removing the need for `ControlsConsole`. Any console can now add the `UI.ControlHost` component and support controls.
  - `ControlsConsole` still exists as a simple way to automatically add the `ControlHost`.
- Settings now expose the defaults for `UseKeyboard` and `UseMouse` for screen objects.
