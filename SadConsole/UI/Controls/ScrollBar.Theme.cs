using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class ScrollBar : ControlBase
{
    private ThemeStyle _themeStyle;

    /// <summary>
    /// The style applied to drawing the control.
    /// </summary>
    [DataMember]
    public ThemeStyle Style
    {
        get => _themeStyle;
        set { _themeStyle = value ?? new ThemeStyle(); }
    }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time) =>
        Style.UpdateAndRedraw(this, time);

    /// <summary>
    /// The drawing code for the scrollbar
    /// </summary>
    [DataContract]
    public class ThemeStyle
    {
        /// <summary>
        /// The glyph for the start button when the control is vertical.
        /// </summary>
        [DataMember]
        public int StartButtonVerticalGlyph = 30;

        /// <summary>
        /// The glyph for the end button when the control is vertical.
        /// </summary>
        [DataMember]
        public int EndButtonVerticalGlyph = 31;

        /// <summary>
        /// The glyph for the start button when the control is horizontal.
        /// </summary>
        [DataMember]
        public int StartButtonHorizontalGlyph = 17;

        /// <summary>
        /// The glyph for the end button when the control is horizontal.
        /// </summary>
        [DataMember]
        public int EndButtonHorizontalGlyph = 16;

        /// <summary>
        /// The glyph for the scroll bar bar where the slider is not located.
        /// </summary>
        [DataMember]
        public int BarGlyph = 176;

        /// <summary>
        /// The glyph for the scroll bar icon.
        /// </summary>
        [DataMember]
        public int GripGlyph = 219;

        /// <summary>
        /// The size of the bar area. Calculated automatically by the control.
        /// </summary>
        public int BarSize;

        /// <summary>
        /// The size of the grip. Calculated automatically by the control.
        /// </summary>
        public int GripSize;

        /// <summary>
        /// The cell the grip starts at. Calculated automatically by the control.
        /// </summary>
        public int GripStart;

        /// <summary>
        /// The cell the grip ends at. Calculated automatically by the control.
        /// </summary>
        public int GripEnd => GripStart + GripSize - 1;

        /// <summary>
        /// Indicates that the mouse is above the up arrow button. Calculated automatically by the control.
        /// </summary>
        public bool IsMouseOverUpButton;

        /// <summary>
        /// Indicates that the mouse is above the down arrow button. Calculated automatically by the control.
        /// </summary>
        public bool IsMouseOverDownButton;

        /// <summary>
        /// Indicates that the mouse is above the gripper. Calculated automatically by the control.
        /// </summary>
        public bool IsMouseOverGripper;

        /// <summary>
        /// Indicates that the mouse is not above the empty part of the bar. Calculated automatically by the control.
        /// </summary>
        public bool IsMouseOverBar;

        /// <summary>
        /// Redraws the control.
        /// </summary>
        /// <param name="control">The control instance.</param>
        /// <param name="time">Time of the update frame.</param>
        public virtual void UpdateAndRedraw(ScrollBar control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            Colors currentColors = control.FindThemeColors();

            control.RefreshThemeStateColors(currentColors);

            ColoredGlyphBase normalAppearance = control.ThemeState.GetStateAppearance(ControlStates.Normal);
            ColoredGlyphBase appearance = control.ThemeState.GetStateAppearance(control.State);

            if (control.State == ControlStates.Disabled)
            {
                normalAppearance = control.ThemeState.GetStateAppearance(ControlStates.Disabled);
            }

            control.Surface.Fill(normalAppearance.Foreground, normalAppearance.Background, BarGlyph);

            IFont font = control.FindThemeFont();
            int startGlyph;
            int endGlyph;

            // Vars based on orientation
            if (control.Orientation == Orientation.Horizontal)
            {
                startGlyph = StartButtonHorizontalGlyph;
                endGlyph = EndButtonHorizontalGlyph;
            }
            else
            {
                startGlyph = StartButtonVerticalGlyph;
                endGlyph = EndButtonVerticalGlyph;
            }

            if (IsMouseOverUpButton)
                appearance.CopyAppearanceTo(control.Surface[0]);

            else if (IsMouseOverDownButton)
                appearance.CopyAppearanceTo(control.Surface[^1]);

            control.Surface[0].Glyph = startGlyph;
            control.Surface[^1].Glyph = endGlyph;

            if (BarSize > 1)
                for (int i = 0; i < GripSize; i++)
                    control.Surface[GripStart + i].Glyph = GripGlyph;

            control.IsDirty = false;
        }
    }
}
