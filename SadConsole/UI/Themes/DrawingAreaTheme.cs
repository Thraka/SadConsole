using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// A basic theme for a drawing surface that simply fills the surface based on the state.
/// </summary>
[DataContract]
public class DrawingAreaTheme : ThemeBase
{
    /// <summary>
    /// When true, only uses <see cref="ThemeStates.Normal"/> for drawing.
    /// </summary>
    [DataMember]
    public bool UseNormalStateOnly { get; set; } = true;

    /// <summary>
    /// The current appearance based on the control state.
    /// </summary>
    public ColoredGlyph Appearance { get; protected set; }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (!(control is DrawingArea drawingSurface)) return;

        RefreshTheme(control.FindThemeColors(), control);

        if (!UseNormalStateOnly)
            Appearance = ControlThemeState.GetStateAppearance(control.State);
        else
            Appearance = ControlThemeState.Normal;

        drawingSurface.OnDraw?.Invoke(drawingSurface, time);
        control.IsDirty = false;
    }

    /// <inheritdoc />
    public override ThemeBase Clone() => new DrawingAreaTheme()
    {
        ControlThemeState = ControlThemeState.Clone(),
        UseNormalStateOnly = UseNormalStateOnly
    };
}
