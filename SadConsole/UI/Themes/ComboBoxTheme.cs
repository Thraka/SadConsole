using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// The theme of a combobox control.
/// </summary>
[DataContract]
public class ComboBoxTheme : ThemeBase
{
    private ListBoxTheme _listBoxTheme;

    /// <summary>
    /// The theme to use with the popup listbox control.
    /// </summary>
    [AllowNull]
    [DataMember]
    public ListBoxTheme ListBoxTheme
    {
        get => _listBoxTheme;
        set => _listBoxTheme = value ?? (ListBoxTheme)Library.Default.GetControlTheme(typeof(ListBox));
    }

    /// <summary>
    /// When <see langword="true"/>, uses the <see cref="PopupHorizontal"/> value from the interior of the control. When <see langword="false"/>, it's used from the outside of the control.
    /// </summary>
    [DataMember]
    public bool PopupInnerAligned { get; set; }

    /// <summary>
    /// Sets the horizontal orientation of the of the dropdown popup.
    /// </summary>
    [DataMember]
    public HorizontalAlignment PopupHorizontal { get; set; }

    /// <summary>
    /// Sets the vertical orientation of the of the dropdown popup.
    /// </summary>
    [DataMember]
    public VerticalAlignment PopupVertical { get; set; }

    /// <summary>
    /// The glyph to use on the control when it's collapsed.
    /// </summary>
    [DataMember]
    public int CollapsedButtonGlyph { get; set; }

    /// <summary>
    /// The glyph to use on the control when it's expanded.
    /// </summary>
    [DataMember]
    public int ExpandedButtonGlyph { get; set; }

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
        
        _listBoxTheme = (ListBoxTheme)Library.Default.GetControlTheme(typeof(ListBox));
        _listBoxTheme.DrawBorder = true;

        PopupInnerAligned = true;
        PopupHorizontal = HorizontalAlignment.Left;
        PopupVertical = VerticalAlignment.Bottom;
    }

    /// <inheritdoc />
    public override void RefreshTheme(Colors themeColors, ControlBase control)
    {
        base.RefreshTheme(themeColors, control);

        IconThemeState.RefreshTheme(_colorsLastUsed);

        ControlThemeState.Normal.Background = GetOffColor(ControlThemeState.Normal.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.MouseOver.Background = GetOffColor(ControlThemeState.MouseOver.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.MouseDown.Background = GetOffColor(ControlThemeState.MouseDown.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.Focused.Background = GetOffColor(ControlThemeState.Focused.Background, _colorsLastUsed.ControlHostBackground);

        IconThemeState.Normal.Foreground = _colorsLastUsed.Lines;
        IconThemeState.MouseOver.Foreground = _colorsLastUsed.Lines;
        IconThemeState.MouseDown.Foreground = _colorsLastUsed.Lines;
        IconThemeState.Focused.Foreground = _colorsLastUsed.Lines;

        IconThemeState.Normal.Background = ControlThemeState.Normal.Background;
        IconThemeState.MouseOver.Background = ControlThemeState.MouseOver.Background;
        IconThemeState.MouseDown.Background = ControlThemeState.MouseDown.Background;
        IconThemeState.Focused.Background = ControlThemeState.Focused.Background;
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
            CollapsedButtonGlyph = CollapsedButtonGlyph,
            ExpandedButtonGlyph = ExpandedButtonGlyph,
            ListBoxTheme = (ListBoxTheme)(ListBoxTheme.Clone() ?? Library.Default.GetControlTheme(typeof(ListBox))),
        };
}

