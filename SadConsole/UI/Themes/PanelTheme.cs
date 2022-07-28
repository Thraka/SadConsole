using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;

namespace SadConsole.UI.Themes;

/// <summary>
/// A basic theme for a drawing surface that simply fills the surface based on the state.
/// </summary>
[DataContract]
public class PanelTheme : ThemeBase
{
    /// <summary>
    /// When true, only uses <see cref="ThemeStates.Normal"/> for drawing.
    /// </summary>
    [DataMember]
    public bool UseNormalStateOnly { get; set; } = true;

    /// <summary>
    /// When true, ignores all states and doesn't draw anything.
    /// </summary>
    [DataMember]
    public bool SkipDrawing { get; set; } = false;

    /// <summary>
    /// The current appearance based on the control state.
    /// </summary>
    [DataMember]
    public ColoredGlyph Appearance { get; protected set; }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (SkipDrawing || !(control is Panel))
            return;

        RefreshTheme(control.FindThemeColors(), control);

        if (!UseNormalStateOnly)
            Appearance = ControlThemeState.GetStateAppearance(control.State);
        else
            Appearance = ControlThemeState.Normal;

        control.Surface.Fill(Appearance.Foreground, Appearance.Background, Appearance.Glyph);

        control.IsDirty = false;
    }

    /// <inheritdoc />
    public override ThemeBase Clone() => new PanelTheme()
    {
        ControlThemeState = ControlThemeState.Clone(),
        UseNormalStateOnly = UseNormalStateOnly,
        SkipDrawing = SkipDrawing
    };
}
