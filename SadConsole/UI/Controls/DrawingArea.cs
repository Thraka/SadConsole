using System;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls;

/// <summary>
/// A simple surface for drawing text that can be moved and sized like a control.
/// </summary>
[DataContract]
public class DrawingArea : ControlBase
{
    /// <summary>
    /// When true, only uses <see cref="ThemeStates.Normal"/> for drawing.
    /// </summary>
    [DataMember]
    public bool UseNormalStateOnly { get; set; } = true;

    /// <summary>
    /// The current appearance based on the control state.
    /// </summary>
    public ColoredGlyphBase? Appearance { get; protected set; }

    /// <summary>
    /// Called when the surface is redrawn.
    /// </summary>
    public Action<DrawingArea, TimeSpan>? OnDraw { get; set; }

    /// <summary>
    /// Creates a new drawing surface control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control.</param>
    public DrawingArea(int width, int height) : base(width, height)
    {
        UseMouse = false;
        UseKeyboard = false;
        TabStop = false;
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        RefreshThemeStateColors(FindThemeColors());

        if (!UseNormalStateOnly)
            Appearance = ThemeState.GetStateAppearance(State);
        else
            Appearance = ThemeState.Normal;

        OnDraw?.Invoke(this, time);
    }
}
