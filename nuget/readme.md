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
- Fixed inputbox not working with nummbers.
- Redesigned ListboxItemTheme to be passable on Listbox ctor.
- Consoles no longer use a cached drawcall.
- Window resize option Settings.ResizeMode now supports Fit. (Thanks ajhmain)
- Console.CalculatedPosition is now in pixels. Consoles of different font sizes now play nice with parenting. Things will be offset properly now.
- Settings.ResizeWindow method allows you to easily resize the game window.
- Added TextBox.KeyPressed event to allow you to cancel input of keys on a textbox.
- MouseWheel support has been added to the listbox control.
- Listbox.SingleClickItemExecute has been added to allow the ItemExecute event to trigger without a doubleclick.