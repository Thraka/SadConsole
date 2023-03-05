using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// The theme for a ListBox control.
/// </summary>
[DataContract]
public class ListBoxTheme : ThemeBase
{
    private bool _drawBorder;

    /// <summary>
    /// Internal flag to indicate the scroll bar needs to be reconfigured.
    /// </summary>
    protected bool _reconfigureSrollBar;

    /// <summary>
    /// The drawing theme for the boarder when <see cref="DrawBorder"/> is true.
    /// </summary>
    public ThemeStates BorderTheme { get; protected set; }

    /// <summary>
    /// The line style for the border when <see cref="DrawBorder"/> is true.
    /// </summary>
    [DataMember]
    public int[] BorderLineStyle { get; set; } = (int[])ICellSurface.ConnectedLineThin.Clone();

    /// <summary>
    /// If false the border will not be drawn.
    /// </summary>
    [DataMember]
    public bool DrawBorder
    {
        get => _drawBorder;
        set { _drawBorder = value; _reconfigureSrollBar = true; }
    }

    /// <summary>
    /// The appearance of the scrollbar used by the listbox control.
    /// </summary>
    [DataMember]
    public ScrollBarTheme ScrollBarTheme;

    /// <summary>
    /// Creates a new theme used by the <see cref="ListBox"/>.
    /// </summary>
    /// <param name="scrollBarTheme">The theme to use to draw the scroll bar.</param>
    public ListBoxTheme(ScrollBarTheme scrollBarTheme)
    {
        ScrollBarTheme = scrollBarTheme;
        BorderTheme = new ThemeStates();
    }

    /// <summary>
    /// Creates a new theme used by the <see cref="ListBox"/> with the default theme for the scroll bar.
    /// </summary>

    public ListBoxTheme()
    {
        ScrollBarTheme = (ScrollBarTheme)Library.Default.GetControlTheme(typeof(ScrollBar));
        BorderTheme = new ThemeStates();
    }

    /// <summary>
    /// Sets up the scroll bar for the listbox.
    /// </summary>
    /// <param name="listbox"></param>
    protected void SetupScrollBar(ListBox listbox)
    {
        if (DrawBorder && listbox.Height < 4)
        {
            DrawBorder = false;
            _reconfigureSrollBar = false;
        }

        if (DrawBorder)
            listbox.SetupScrollBar(Orientation.Vertical, listbox.Height - 2, new Point(listbox.Width - 1, 1));
        else
            listbox.SetupScrollBar(Orientation.Vertical, listbox.Height, new Point(listbox.Width - 1, 0));
    }

    /// <inheritdoc />
    public override void Attached(ControlBase control)
    {
        if (control is not ListBox listbox) throw new Exception("Added ListBoxTheme to a control that isn't a ListBox.");

        base.Attached(control);

        SetupScrollBar(listbox);
    }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (control is not ListBox listbox) return;
        if (!listbox.IsDirty) return;

        if (_reconfigureSrollBar)
        {
            SetupScrollBar(listbox);
            _reconfigureSrollBar = false;
        }

        RefreshTheme(control.FindThemeColors(), control);

        int columnOffset;
        int columnEnd;
        int startingRow;
        int endingRow;

        ColoredGlyph appearance = ControlThemeState.GetStateAppearance(listbox.State);
        ColoredGlyph borderAppearance = BorderTheme.GetStateAppearance(listbox.State);

        // Redraw the control
        listbox.Surface.Fill(
            appearance.Foreground,
            appearance.Background,
            appearance.Glyph);

        ShowHideScrollBar(listbox);

        if (DrawBorder)
        {
            endingRow = listbox.Height - 2;
            startingRow = 1;
            columnOffset = 1;
            columnEnd = listbox.Width - 2;
            listbox.Surface.DrawBox(new Rectangle(0, 0, listbox.Width, listbox.Height), ShapeParameters.CreateStyledBox(BorderLineStyle, new ColoredGlyph(borderAppearance.Foreground, borderAppearance.Background, 0)));
        }
        else
        {
            endingRow = listbox.Height;
            startingRow = 0;
            columnOffset = 0;
            columnEnd = listbox.Width - (listbox.IsScrollBarVisible ? 1 : 0);
            listbox.Surface.Fill(borderAppearance.Foreground, borderAppearance.Background, 0, null);
        }

        listbox.MouseArea = new Rectangle(columnOffset, startingRow, columnEnd, endingRow);

        listbox.VisibleItemsTotal = listbox.Items.Count >= endingRow ? endingRow : listbox.Items.Count;
        listbox.VisibleItemsMax = listbox.MouseArea.Height;

        int offset = listbox.IsScrollBarVisible ? listbox.ScrollBar.Value : 0;
        for (int i = 0; i < endingRow; i++)
        {
            int itemIndexRelative = i + offset;
            if (itemIndexRelative < listbox.Items.Count)
            {
                ControlStates state = 0;

                if (listbox.State.HasFlag(ControlStates.MouseOver) && listbox.ItemIndexMouseOver == itemIndexRelative)
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseOver);

                if (listbox.State.HasFlag(ControlStates.MouseLeftButtonDown))
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseLeftButtonDown);

                if (listbox.State.HasFlag(ControlStates.MouseRightButtonDown))
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseRightButtonDown);

                if (listbox.State.HasFlag(ControlStates.Disabled))
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.Disabled);

                if (itemIndexRelative == listbox.SelectedIndex)
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.Selected);

                listbox.ItemTheme.Draw(listbox, new Rectangle(columnOffset, i + startingRow, columnEnd, 1), listbox.Items[itemIndexRelative], state);
            }
        }

        //if (listbox.IsScrollBarVisible)
        //{
        //    listbox.ScrollBar.IsDirty = true;
        //    listbox.ScrollBar.Update(time);
        //    int y = 0;

        //    for (int ycell = 0; ycell < listbox.ScrollBar.Height; ycell++)
        //    {
        //        listbox.Surface.SetGlyph(listbox.ScrollBarRenderLocation.X, listbox.ScrollBarRenderLocation.Y + y, listbox.ScrollBar.Surface[0, ycell].Glyph);
        //        listbox.Surface.SetCellAppearance(listbox.ScrollBarRenderLocation.X, listbox.ScrollBarRenderLocation.Y + y, listbox.ScrollBar.Surface[0, ycell]);
        //        y++;
        //    }
        //}

        listbox.IsDirty = Helpers.HasFlag((int)listbox.State, (int)ControlStates.MouseOver);
    }

    /// <inheritdoc />
    public override void RefreshTheme(Colors colors, ControlBase control)
    {
        base.RefreshTheme(colors, control);

        var listbox = (ListBox)control;

        ControlThemeState.SetForeground(ControlThemeState.Normal.Foreground);
        ControlThemeState.SetBackground(ControlThemeState.Normal.Background);
        listbox.ItemTheme.RefreshTheme(_colorsLastUsed);

        listbox.ScrollBar.Theme = ScrollBarTheme;

        ScrollBarTheme?.RefreshTheme(_colorsLastUsed, listbox.ScrollBar);

        BorderTheme.RefreshTheme(_colorsLastUsed);
        BorderTheme.SetForeground(_colorsLastUsed.Lines);
        BorderTheme.SetBackground(ControlThemeState.Normal.Background);
    }

    /// <inheritdoc />
    public override ThemeBase Clone() => new ListBoxTheme((ScrollBarTheme)ScrollBarTheme.Clone())
    {
        ControlThemeState = ControlThemeState.Clone(),
        BorderTheme = BorderTheme.Clone(),
        BorderLineStyle = (int[])BorderLineStyle.Clone(),
        DrawBorder = DrawBorder,
    };

    /// <summary>
    /// Shows the scroll bar when there are too many items to display; otherwise, hides it.
    /// </summary>
    /// <param name="control">Reference to the listbox being processed.</param>
    public void ShowHideScrollBar(ListBox control)
    {
        int heightOffset = DrawBorder ? 2 : 0;

        // process the scroll bar
        int scrollbarItems = control.Items.Count - (control.Height - heightOffset);

        if (scrollbarItems > 0)
        {
            control.ScrollBar.Maximum = scrollbarItems;
            control.IsScrollBarVisible = true;
        }
        else
        {
            control.ScrollBar.Maximum = 0;
            control.IsScrollBarVisible = false;
        }
    }
}

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
    /// <param name="control">The listbox that contains the item.</param>
    /// <param name="area">The area to draw the item.</param>
    /// <param name="item">The item object.</param>
    /// <param name="itemState">The state of the item.</param>
    public virtual void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
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
    // TODO: Change ValueTyple to specific types

    /// <summary>
    /// When <see langword="false"/>, colored boxes used when drawing the color for (Color, string) tuple will use two characters; otherwise <see langword="true"/> and only one character is used.
    /// </summary>
    public bool UseSingleCharacterForBox { get; set; } = false;

    /// <inheritdoc />
    public override void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
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
