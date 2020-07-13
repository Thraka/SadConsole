## v9.0 Alpha 9

- Fixed a lot of LayeredScreenSurface bugs.
- **SFML**: Renamed `Renderers.LayeredScreenObject` to `Renderers.LayeredScreenSurface`
- Removed `TintBeforeDrawCall`. Not a good design.
- Added `SadConsole.Instructions.AnimatedValue` to animate a value over a duration.
- Updated `SadConsole.Instructions.FadeTextSurfaceTint` to use `AnimatedValue`.

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
