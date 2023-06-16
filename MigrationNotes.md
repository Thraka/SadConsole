
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

Drawing a control in v10
-----------------------------

When drawing a control override the `UpdateAndRedraw` method and do the following:

1. Check if `IsDirty == false` and return.
2. Get the current colors for the control `Colors currentColors = FindThemeColors();`
3. Call `RefreshThemeStateColors(currentColors);`

   If migrating a v9 theme, override `RefreshThemeStateColors` and copy any code in the theme's `RefreshTheme` method (if overridden).

4. (If migrating a v9 theme)
   - Copy any code in the theme's `UpdateAndDraw` method.
   - If your code used the `control` parameter or cast `control` to a specific type, do a find and replace operation
     with `control.` and a blank value. You no longer need to reference the control since the drawing code now lives
     in the control itself.
   - Replace references of `ControlThemeState` with `ThemeState`.
5. Draw the control by using the `Surface` property.
6. Set `IsDirty = false`


Here is the drawing code for the button control:
```csharp
public override void UpdateAndRedraw(TimeSpan time)
{
    // Step 1
    if (!IsDirty) return;

    // Step 2
    Colors currentColors = FindThemeColors();

    // Step 3
    RefreshThemeStateColors(currentColors);

    // Steps 4 and 5: Draw the control
    ColoredGlyph appearance = ThemeState.GetStateAppearance(State);
    ColoredGlyph endGlyphAppearance = ThemeState.GetStateAppearance(State);

    endGlyphAppearance.Foreground = currentColors.Lines;

    int middle = (Height != 1 ? Height / 2 : 0);

    // Redraw the control
    Surface.Fill(
        appearance.Foreground,
        appearance.Background,
        appearance.Glyph, null);

    if (ShowEnds && Width >= 3)
    {
        Surface.Print(1, middle, Text.Align(TextAlignment, Width - 2));
        Surface.SetCellAppearance(0, middle, endGlyphAppearance);
        Surface[0, middle].Glyph = LeftEndGlyph;
        Surface.SetCellAppearance(Width - 1, middle, endGlyphAppearance);
        Surface[Width - 1, middle].Glyph = RightEndGlyph;
    }
    else
        Surface.Print(0, middle, Text.Align(TextAlignment, Width));

    // Step 6
    IsDirty = false;
}
```
