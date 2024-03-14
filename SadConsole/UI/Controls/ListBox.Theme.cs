using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class ListBox
{
    private bool _drawBorder;

    /// <summary>
    /// Internal flag to indicate the scroll bar needs to be reconfigured.
    /// </summary>
    protected bool _reconfigureSrollBar;

    /// <summary>
    /// The drawing theme for the border when <see cref="DrawBorder"/> is true.
    /// </summary>
    public ThemeStates BorderTheme { get; protected set; } = new ThemeStates();

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
    /// The appearance of the items displayed by the listbox control.
    /// </summary>
    [DataMember]
    public ListBoxItemTheme ItemTheme { get; set; } = new ListBoxItemTheme();

    /// <summary>
    /// The area on the control where items are drawn.
    /// </summary>
    public Rectangle ItemsArea { get; set; }

    /// <summary>
    /// Sets up the scroll bar for the listbox.
    /// </summary>
    protected void SetupScrollBar()
    {
        if (DrawBorder && Height < 4)
            DrawBorder = false;

        if (DrawBorder)
            SetupScrollBar(Orientation.Vertical, Height - 2, new Point(Width - 1, 1));
        else
            SetupScrollBar(Orientation.Vertical, Height, new Point(Width - 1, 0));

        ShowHideScrollBar();
    }

    /// <summary>
    /// Shows the scroll bar when there are too many items to display; otherwise, hides it.
    /// </summary>
    protected void ShowHideScrollBar()
    {
        int heightOffset = DrawBorder ? 2 : 0;

        // process the scroll bar
        int scrollbarItems = Items.Count - (Height - heightOffset);

        if (scrollbarItems > 0)
        {
            ScrollBar.MaximumValue = scrollbarItems;
            IsScrollBarVisible = true;
        }
        else
        {
            ScrollBar.MaximumValue = 0;
            IsScrollBarVisible = false;
        }
    }

    /// <inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        ThemeState.SetForeground(ThemeState.Normal.Foreground);
        ThemeState.SetBackground(ThemeState.Normal.Background);
        ItemTheme.RefreshTheme(colors);

        //ScrollBarTheme.RefreshTheme(colors, ScrollBar);
        ItemTheme.RefreshTheme(colors);

        BorderTheme.RefreshTheme(colors);
        BorderTheme.SetForeground(colors.Lines);
        BorderTheme.SetBackground(ThemeState.Normal.Background);
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty)
        {
            base.UpdateAndRedraw(time);
            return;
        }

        if (_reconfigureSrollBar)
        {
            SetupScrollBar();
            _reconfigureSrollBar = false;
        }

        RefreshThemeStateColors(FindThemeColors());

        int columnOffset;
        int columnEnd;
        int startingRow;
        int endingRow;

        ColoredGlyphBase appearance = ThemeState.GetStateAppearance(State);
        ColoredGlyphBase borderAppearance = BorderTheme.GetStateAppearance(State);

        // Redraw the control
        Surface.Fill(
            appearance.Foreground,
            appearance.Background,
            appearance.Glyph);

        ShowHideScrollBar();

        if (DrawBorder)
        {
            endingRow = Height - 2;
            startingRow = 1;
            columnOffset = 1;
            columnEnd = Width - 2;
            Surface.DrawBox(new Rectangle(0, 0, Width, Height), ShapeParameters.CreateStyledBox(BorderLineStyle, new ColoredGlyph(borderAppearance.Foreground, borderAppearance.Background, 0)));
        }
        else
        {
            endingRow = Height;
            startingRow = 0;
            columnOffset = 0;
            columnEnd = Width - (IsScrollBarVisible ? 1 : 0);
            Surface.Fill(borderAppearance.Foreground, borderAppearance.Background, 0, null);
        }

        ItemsArea = (columnOffset, startingRow, columnEnd, endingRow);
        MouseArea = (0, 0, Width, Height);

        VisibleItemsTotal = Items.Count >= endingRow ? endingRow : Items.Count;
        VisibleItemsMax = MouseArea.Height;

        int offset = IsScrollBarVisible ? ScrollBar.Value : 0;
        for (int i = 0; i < endingRow; i++)
        {
            int itemIndexRelative = i + offset;
            if (itemIndexRelative < Items.Count)
            {
                ControlStates state = 0;

                if (State.HasFlag(ControlStates.MouseOver) && ItemIndexMouseOver == itemIndexRelative)
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseOver);

                if (State.HasFlag(ControlStates.MouseLeftButtonDown))
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseLeftButtonDown);

                if (State.HasFlag(ControlStates.MouseRightButtonDown))
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseRightButtonDown);

                if (State.HasFlag(ControlStates.Disabled))
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.Disabled);

                if (itemIndexRelative == SelectedIndex)
                    state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.Selected);

                ItemTheme.Draw(this, new Rectangle(columnOffset, i + startingRow, columnEnd, 1), Items[itemIndexRelative], state);
            }
        }

        IsDirty = Helpers.HasFlag((int)State, (int)ControlStates.MouseOver);

        base.UpdateAndRedraw(time);
    }
}
