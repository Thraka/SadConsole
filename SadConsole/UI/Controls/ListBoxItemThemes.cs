using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI;
/// <summary>
/// A generic theme for a <see cref="ListBox"/> item.
/// </summary>
public class ListBoxItemTheme : ThemeStates
{
    /// <summary>
    /// Gets or sets a value to allow printing the background of a colored string. When <see langword="false"/>, the control state background is used.
    /// </summary>
    public bool UseColoredStringBackground { get; set; }

    /// <inheritdoc />
    public override void RefreshTheme(Colors themeColors)
    {
        base.RefreshTheme(themeColors);

        SetForeground(Normal.Foreground);
        SetBackground(Normal.Background);

        Selected.Foreground = themeColors.Appearance_ControlSelected.Foreground;
        MouseOver = themeColors.Appearance_ControlOver.Clone();
    }

    /// <summary>
    /// Draws the <paramref name="item"/> in the specified <paramref name="area"/> of the listbox.
    /// </summary>
    /// <param name="control">The control containing a surface to draw on.</param>
    /// <param name="area">The area to draw the item.</param>
    /// <param name="item">The item object.</param>
    /// <param name="itemState">The state of the item.</param>
    public virtual void Draw(ControlBase control, Rectangle area, object item, ControlStates itemState)
    {
        if (item is ValueTuple<ColoredString, object> colValue)
            item = colValue.Item1;
        else if (item is ValueTuple<string, object> strValue)
            item = strValue.Item1;

        if (item is ColoredString colored)
        {
            ColoredString substring;

            if (colored.Length > area.Width)
                substring = colored.SubString(0, area.Width);
            else
                substring = colored.Clone();

            if (Helpers.HasFlag((int)itemState, (int)ControlStates.Selected) && !Helpers.HasFlag((int)itemState, (int)ControlStates.MouseOver))
                control.Surface.Print(area.X, area.Y, substring.ToString(), Selected);
            else if (Helpers.HasFlag((int)itemState, (int)ControlStates.MouseOver))
                control.Surface.Print(area.X, area.Y, substring.ToString(), MouseOver);
            else
            {
                substring.IgnoreBackground = !UseColoredStringBackground;
                control.Surface.Fill(area, background: Selected.Background);
                control.Surface.Print(area.X, area.Y, substring);
            }
        }
        else
        {
            string value = item.ToString() ?? string.Empty;

            if (value.Length < area.Width)
                value += new string(' ', area.Width - value.Length);
            else if (value.Length > area.Width)
                value = value.Substring(0, area.Width);

            if (Helpers.HasFlag((int)itemState, (int)ControlStates.Selected) && !Helpers.HasFlag((int)itemState, (int)ControlStates.MouseOver))
                control.Surface.Print(area.X, area.Y, value, Selected);
            else
                control.Surface.Print(area.X, area.Y, value, GetStateAppearance(itemState));
        }
    }

    /// <summary>
    /// Creates a copy of this theme.
    /// </summary>
    /// <returns>A new theme object.</returns>
    public new virtual ListBoxItemTheme Clone() => new ListBoxItemTheme()
    {
        Normal = Normal.Clone(),
        Disabled = Disabled.Clone(),
        MouseOver = MouseOver.Clone(),
        MouseDown = MouseDown.Clone(),
        Selected = Selected.Clone(),
        Focused = Focused.Clone(),
    };
}

/// <summary>
/// A theme for a <see cref="ListBox"/> that displays a <see cref="Color"/> object.
/// </summary>
[DataContract]
public class ListBoxItemColorTheme : ListBoxItemTheme
{
    // TODO: Change ValueTuple to specific types

    /// <summary>
    /// When <see langword="false"/>, colored boxes used when drawing the color for (Color, string) tuple will use two characters; otherwise <see langword="true"/> and only one character is used.
    /// </summary>
    public bool UseSingleCharacterForBox { get; set; } = false;

    /// <inheritdoc />
    public override void Draw(ControlBase control, Rectangle area, object item, ControlStates itemState)
    {
        if (item is Color || item is ValueTuple<Color, string> || item is ValueTuple<Color, Color, string>)
        {
            string value = new string(' ', area.Width - 2);

            ColoredGlyph cellLook = GetStateAppearance(itemState).Clone();

            control.Surface.Print(area.X + 1, area.Y, value, cellLook);

            control.Surface.Print(area.X, area.Y, " ", cellLook);
            control.Surface.Print(area.X + area.Width - 1, area.Y, " ", cellLook);


            if (item is Color color)
            {
                cellLook.Background = color;
                control.Surface.Print(area.X + 1, area.Y, value, cellLook);

                if (itemState.HasFlag(ControlStates.Selected))
                {
                    control.Surface.SetGlyph(area.X, area.Y, 16);
                    control.Surface.SetGlyph(area.X + area.Width - 1, area.Y, 17);
                }
            }
            else if (item is ValueTuple<Color, string> color2)
            {
                bool useExtended = false;

                IFont? font = control.AlternateFont ?? control.Parent?.Host?.ParentConsole?.Font;

                if (font != null)
                    useExtended = font.IsSadExtended;

                string colorBoxesCommands;

                StringParser.Default parser = new StringParser.Default();

                if (useExtended)
                    colorBoxesCommands = UseSingleCharacterForBox ? $"[c:r f:{color2.Item1.ToParser()}:2][c:sg 254]m" : $"[c:r f:{color2.Item1.ToParser()}:2][c:sg 301]m[c:sg 302]m";
                else
                    colorBoxesCommands = UseSingleCharacterForBox ? $"[c:r f:{color2.Item1.ToParser()}:2][c:sg 219]m" : $"[c:r f:{color2.Item1.ToParser()}:2][c:sg 219:2]mm";

                colorBoxesCommands = $"[c:r b:{cellLook.Background.ToParser()}]{colorBoxesCommands}";

                control.Surface.Print(area.X, area.Y, parser.Parse(colorBoxesCommands));
                control.Surface.Print(area.X + 3, area.Y, color2.Item2.Align(HorizontalAlignment.Left, area.Width - 3), cellLook);
            }
            else
            {
                cellLook.Foreground = ((ValueTuple<Color, Color, string>)item).Item2;
                cellLook.Background = ((ValueTuple<Color, Color, string>)item).Item1;
                value = ((ValueTuple<Color, Color, string>)item).Item3.Align(HorizontalAlignment.Left, area.Width - 2);
                control.Surface.Print(area.X + 1, area.Y, value, cellLook);

                if (itemState.HasFlag(ControlStates.Selected))
                {
                    control.Surface.SetGlyph(area.X, area.Y, 16);
                    control.Surface.SetGlyph(area.X + area.Width - 1, area.Y, 17);
                }
            }
        }
        else
        {
            base.Draw(control, area, item, itemState);
        }
    }

    /// <inheritdoc />
    public override ListBoxItemTheme Clone() => new ListBoxItemColorTheme()
    {
        Normal = Normal.Clone(),
        Disabled = Disabled.Clone(),
        MouseOver = MouseOver.Clone(),
        MouseDown = MouseDown.Clone(),
        Selected = Selected.Clone(),
        Focused = Focused.Clone(),
        UseSingleCharacterForBox = UseSingleCharacterForBox
    };
}
