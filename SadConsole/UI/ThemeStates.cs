﻿using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI;

/// <summary>
/// Has the basic appearances of each control state.
/// </summary>
[DataContract]
public class ThemeStates
{
    /// <summary>
    /// The normal appearance of the control.
    /// </summary>
    [DataMember]
    public ColoredGlyphBase Normal { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of the control when it is disabled.
    /// </summary>
    [DataMember]
    public ColoredGlyphBase Disabled { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of the control when it is focused.
    /// </summary>
    [DataMember]
    public ColoredGlyphBase Focused { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of the control when it is in a selected state.
    /// </summary>
    [DataMember]
    public ColoredGlyphBase Selected { get; set; } = new ColoredGlyph();

    /// <summary>
    /// The appearance of the control when the mouse is over it.
    /// </summary>
    [DataMember]
    public ColoredGlyphBase MouseOver { get; set; } = new ColoredGlyph();

    /// <summary>
    /// THe appearance of the control when a mouse button is held down.
    /// </summary>
    [DataMember]
    public ColoredGlyphBase MouseDown { get; set; } = new ColoredGlyph();

    /// <summary>
    /// Sets the same foreground color to all theme states.
    /// </summary>
    /// <param name="color">The foreground color.</param>
    public void SetForeground(Color color)
    {
        Normal.Foreground = color;
        Disabled.Foreground = color;
        MouseOver.Foreground = color;
        MouseDown.Foreground = color;
        Selected.Foreground = color;
        Focused.Foreground = color;
    }

    /// <summary>
    /// Sets the same background color to all theme states.
    /// </summary>
    /// <param name="color">The background color.</param>
    public void SetBackground(Color color)
    {
        Normal.Background = color;
        Disabled.Background = color;
        MouseOver.Background = color;
        MouseDown.Background = color;
        Selected.Background = color;
        Focused.Background = color;
    }

    /// <summary>
    /// Sets the same glyph to all theme states.
    /// </summary>
    /// <param name="glyph">The glyph.</param>
    public void SetGlyph(int glyph)
    {
        Normal.Glyph = glyph;
        Disabled.Glyph = glyph;
        MouseOver.Glyph = glyph;
        MouseDown.Glyph = glyph;
        Selected.Glyph = glyph;
        Focused.Glyph = glyph;
    }

    /// <summary>
    /// Sets the same mirror setting to all theme states.
    /// </summary>
    /// <param name="mirror">The mirror setting.</param>
    public void SetMirror(Mirror mirror)
    {
        Normal.Mirror = mirror;
        Disabled.Mirror = mirror;
        MouseOver.Mirror = mirror;
        MouseDown.Mirror = mirror;
        Selected.Mirror = mirror;
        Focused.Mirror = mirror;
    }

    /// <summary>
    /// Gets an appearance defined by this theme from the <paramref name="state" /> parameter.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns>A cell appearance.</returns>
    public ColoredGlyphBase GetStateAppearance(ControlStates state)
    {
        if (Helpers.HasFlag((int)state, (int)ControlStates.Disabled))
            return Disabled;

        if (Helpers.HasFlag((int)state, (int)ControlStates.MouseLeftButtonDown) || Helpers.HasFlag((int)state, (int)ControlStates.MouseRightButtonDown))
            return MouseDown;

        if (Helpers.HasFlag((int)state, (int)ControlStates.MouseOver))
            return MouseOver;

        if (Helpers.HasFlag((int)state, (int)ControlStates.Focused))
            return Focused;

        if (Helpers.HasFlag((int)state, (int)ControlStates.Selected))
            return Selected;

        return Normal;
    }

    /// <summary>
    /// Gets an appearance defined by this theme from the <paramref name="state" /> parameter. Treats mouse related states as normal.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns>A cell appearance.</returns>
    public ColoredGlyphBase GetStateAppearanceNoMouse(ControlStates state)
    {
        if (Helpers.HasFlag((int)state, (int)ControlStates.Disabled))
            return Disabled;

        if (Helpers.HasFlag((int)state, (int)ControlStates.Focused))
            return Focused;

        if (Helpers.HasFlag((int)state, (int)ControlStates.Selected))
            return Selected;

        return Normal;
    }

    /// <summary>
    /// Performs a deep copy of this theme.
    /// </summary>
    /// <returns>A new instance of the theme.</returns>
    public ThemeStates Clone() => new ThemeStates()
    {
        Normal = Normal.Clone(),
        Disabled = Disabled.Clone(),
        MouseOver = MouseOver.Clone(),
        MouseDown = MouseDown.Clone(),
        Selected = Selected.Clone(),
        Focused = Focused.Clone(),
    };

    /// <summary>
    /// Reloads the theme values based on the colors provided.
    /// </summary>
    /// <param name="colors">The colors to create the theme with.</param>
    public virtual void RefreshTheme(Colors colors)
    {
        colors.Appearance_ControlNormal.CopyAppearanceTo(Normal);
        colors.Appearance_ControlDisabled.CopyAppearanceTo(Disabled);
        colors.Appearance_ControlOver.CopyAppearanceTo(MouseOver);
        colors.Appearance_ControlMouseDown.CopyAppearanceTo(MouseDown);
        colors.Appearance_ControlSelected.CopyAppearanceTo(Selected);
        colors.Appearance_ControlFocused.CopyAppearanceTo(Focused);
    }
}
