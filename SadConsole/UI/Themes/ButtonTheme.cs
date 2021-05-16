using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme of the button control
    /// </summary>
    [DataContract]
    public class ButtonTheme : ThemeBase
    {
        /// <summary>
        /// When true, renders the "end" glyphs on the button.
        /// </summary>
        [DataMember]
        public bool ShowEnds { get; set; } = true;

        /// <summary>
        /// The theme state used with the left end of the button.Defaults to '&lt;'.
        /// </summary>
        [DataMember]
        public int LeftEndGlyph { get; set; }

        /// <summary>
        /// The theme state used with the right end of the button. Defaults to '>'.
        /// </summary>
        [DataMember]
        public int RightEndGlyph { get; set; }

        /// <summary>
        /// The theme state used with the left end of the button.
        /// </summary>
        public ThemeStates EndsThemeState { get; protected set; }

        /// <summary>
        /// Creates a new button theme with the specified left and right brackets.
        /// </summary>
        /// <param name="leftEndGlyph">Specified the left end. Defaults to '$lt;'.</param>
        /// <param name="rightEndGlyph">Specified the right bracket. Defaults to '>'.</param>
        public ButtonTheme(int leftEndGlyph = '<', int rightEndGlyph = '>')
        {
            EndsThemeState = new ThemeStates();
            LeftEndGlyph = leftEndGlyph;
            RightEndGlyph = rightEndGlyph;
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors colors, ControlBase control)
        {
            base.RefreshTheme(colors, control);

            EndsThemeState.RefreshTheme(_colorsLastUsed);

            EndsThemeState.Normal.Foreground = _colorsLastUsed.Lines;
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            if (!button.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance = ControlThemeState.GetStateAppearance(control.State);
            ColoredGlyph endGlyphAppearance = EndsThemeState.GetStateAppearance(control.State);

            int middle = (button.Height != 1 ? button.Height / 2 : 0);

            // Redraw the control
            button.Surface.Fill(
                appearance.Foreground,
                appearance.Background,
                appearance.Glyph, null);

            if (ShowEnds && button.Width >= 3)
            {
                var lines = control.State == ControlStates.Disabled ? appearance.Foreground : _colorsLastUsed.Lines;
                button.Surface.Print(1, middle, (button.Text).Align(button.TextAlignment, button.Width - 2));
                button.Surface.SetCellAppearance(0, middle, endGlyphAppearance);
                button.Surface[0, middle].Glyph = LeftEndGlyph;
                button.Surface.SetCellAppearance(button.Width - 1, middle, endGlyphAppearance);
                button.Surface[button.Width - 1, middle].Glyph = RightEndGlyph;
            }
            else
                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width));

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new ButtonTheme()
        {
            ShowEnds = ShowEnds,
            ControlThemeState = ControlThemeState.Clone(),
            EndsThemeState = EndsThemeState.Clone(),
            RightEndGlyph = RightEndGlyph,
            LeftEndGlyph = LeftEndGlyph
        };
    }
}
