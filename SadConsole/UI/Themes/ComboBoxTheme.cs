using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// The theme of a checkbox control.
/// </summary>
[DataContract]
public class ComboBoxTheme : ThemeBase
{
    private int _dropDownHeight;
    private ListBoxTheme _listBoxTheme;

    /// <summary>
    /// The drop down height. The listbox will fit in this height.
    /// </summary>
    public int DropDownHeight
    {
        get => _dropDownHeight;
        set
        {
            if (value < 2) throw new Exception($"{nameof(DropDownHeight)} must be greater than 2");

            _dropDownHeight = value;
        }
    }

    public ListBoxTheme ListBoxTheme
    {
        get => _listBoxTheme;
        set => _listBoxTheme = value ?? (ListBoxTheme)Library.Default.GetControlTheme(typeof(ListBox));
    }

    /// <summary>
    /// 
    /// </summary>
    [DataMember]
    public int CollapsedButtonGlyph { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DataMember]
    public int ExpandedButtonGlyph { get; set; }

    /// <summary>
    /// An optional color of the <see cref="CollapsedButtonGlyph"/>.
    /// </summary>
    [DataMember]
    public Color? CollapsedGlyphColor { get; set; }

    /// <summary>
    /// An optional color of the <see cref="ExpandedButtonGlyph"/>.
    /// </summary>
    [DataMember]
    public Color? ExpandedGlyphColor { get; set; }

    /// <summary>
    /// The theme state used with the icon of the combobox.
    /// </summary>
    public ThemeStates IconThemeState { get; protected set; }

    /// <summary>
    /// Creates a new combobox theme.
    /// </summary>
    public ComboBoxTheme()
    {
        IconThemeState = new ThemeStates();

        CollapsedButtonGlyph = 16; // ▶
        ExpandedButtonGlyph = 31; // ▼
        
        ListBoxTheme = (ListBoxTheme)Library.Default.GetControlTheme(typeof(ListBox));
        ListBoxTheme.DrawBorder = false;
    }

    /// <inheritdoc />
    public override void RefreshTheme(Colors themeColors, ControlBase control)
    {
        base.RefreshTheme(themeColors, control);

        IconThemeState.RefreshTheme(_colorsLastUsed);
        //ListBoxTheme.RefreshTheme(themeColors, ((ComboBox)control).ListBoxControl);
        //HeaderTheme.RefreshTheme(themeColors, ((ComboBox)control).HeaderControl);
    }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (control is not ComboBox comboBox) return;
        if (!control.IsDirty) return;

        RefreshTheme(control.FindThemeColors(), control);

        ColoredGlyph appearance = ControlThemeState.GetStateAppearance(comboBox.State);
        ColoredGlyph iconAppearance = IconThemeState.GetStateAppearance(comboBox.State);

        comboBox.Surface.Fill(appearance.Foreground, appearance.Background, 0);
        iconAppearance.CopyAppearanceTo(comboBox.Surface[comboBox.Width - 1, 0]);

        comboBox.Surface.Print(0, 0, comboBox.Text.Align(comboBox.TextAlignment, comboBox.Surface.Width - 2));

        comboBox.Surface[comboBox.Width - 1, 0].Glyph = comboBox.IsSelected ? ExpandedButtonGlyph : CollapsedButtonGlyph;

        comboBox.IsDirty = false;
    }

    /// <inheritdoc />
    public override ThemeBase Clone() =>
        new ComboBoxTheme()
        {
            ControlThemeState = ControlThemeState.Clone(),
            IconThemeState = IconThemeState.Clone(),
            CollapsedGlyphColor = CollapsedGlyphColor,
            ExpandedGlyphColor = ExpandedGlyphColor,
            CollapsedButtonGlyph = CollapsedButtonGlyph,
            ExpandedButtonGlyph = ExpandedButtonGlyph,
            ListBoxTheme = ListBoxTheme ?? (ListBoxTheme)Library.Default.GetControlTheme(typeof(ListBox)),
        };

    private class HeaderBoxTheme: CheckBoxTheme
    {
        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (control is not ToggleButtonBase checkbox) return;
            if (!control.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);

            ColoredGlyph appearance = ControlThemeState.GetStateAppearance(checkbox.State);
            ColoredGlyph bracketAppearance = BracketsThemeState.GetStateAppearance(checkbox.State);
            ColoredGlyph iconAppearance = IconThemeState.GetStateAppearance(checkbox.State);

            checkbox.Surface.Fill(appearance.Foreground, appearance.Background, null);

            // If we are doing text, then print it otherwise we're just displaying the button part
            if (checkbox.Width <= 2)
                iconAppearance.CopyAppearanceTo(checkbox.Surface[0, 0]);

            if (checkbox.Width >= 3)
            {
                bracketAppearance.CopyAppearanceTo(checkbox.Surface[0, 0]);
                iconAppearance.CopyAppearanceTo(checkbox.Surface[1, 0]);
                bracketAppearance.CopyAppearanceTo(checkbox.Surface[2, 0]);

                checkbox.Surface[0, 0].Glyph = LeftBracketGlyph;
                checkbox.Surface[2, 0].Glyph = RightBracketGlyph;
            }

            if (checkbox.Width >= 5)
                checkbox.Surface.Print(4, 0, checkbox.Text.Align(checkbox.TextAlignment, checkbox.Width - 4));

            checkbox.IsDirty = false;
        }
    }
}

