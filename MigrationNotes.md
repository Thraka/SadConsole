
Themes from v9
----------------------

The theme concept has been removed from v10. If you had a control with its own theme, you
need to migrate the theme code to the control itself. Here are some tips and notes:

- `ControlThemeState` changes to `ThemeState`

- `_colorsLastUsed` was declared by the theme when `RefreshTheme` was called. This member no
  longer exists and `RefreshTheme` has changed. If you used this member, instead declare a `Colors` object
  in the `UpdateAndRedraw` method: `Colors _colorsLastUsed = FindThemeColors();` This resolves any references
  to `_colorsLastUsed`. Next, rename the variable to something more useful like `colors` or `currentColors`.

- If `GetOffColor` is used, this has been moved from `ThemeState` to the `Colors` class, for
  example, `currentColors.GetOffColor`

- If your theme declared various properties, variables, and methods, move them to the control. I suggest making the
  control a partial class, then creating a new class with the file name `.Theme.cs` appended. For example, SadConsole
  has the `Checkbox.cs` and `Checkbox.Theme.cs` files. The "theme" code file contains all of the properties, methods,
  and variables used to draw the control.

When drawing a control override the `UpdateAndRedraw` method and do the following:

1. Check if `IsDirty == false` and return.
2. Get the current colors for the control `Colors currentColors = FindThemeColors();`
3. Call `ThemeState.RefreshTheme(currentColors);`
4. (If migrating a v9 theme)
   - Copy any code in the theme's `RefreshTheme` method (if overridden).
   - Copy any code in the theme's `UpdateAndDraw` method.
   - If your code used the `control` parameter or cast `control` to a specific type, do a find and replace operation
     with `control.` and a blank value. You no longer need to reference the control since the drawing code now lives
     in the control itself.
   - Replace references of `ControlThemeState` with `ThemeState`.
5. Draw the control by using the `Surface` property.
6. Set `IsDirty = false`
