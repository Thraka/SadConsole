using SadRogue.Primitives;
using SadConsole;
using System;
using SadConsole.UI.Controls;
using ColorBarControl = SadConsole.UI.Controls.ColorBar;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme for the <see cref="Controls.ColorBar"/> control.
    /// </summary>
    public class ColorBar : ThemeBase
    {
        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            if (!(control is ColorBarControl)) throw new Exception($"Theme can only be added to a {nameof(ColorBarControl)}");

            control.Surface = new CellSurface(control.Width, control.Height);
            control.Surface.Clear();
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is ColorBarControl bar)) return;
            if (!bar.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);

            control.Surface.Fill(Color.White, Color.Black, 0, null);

            bar._positions = control.Width;
            bar._colorSteps = bar.StartingColor.LerpSteps(bar.EndingColor, control.Width);

            for (int x = 0; x < control.Width; x++)
            {
                control.Surface[x, 0].Glyph = 219;
                control.Surface[x, 0].Foreground = bar._colorSteps[x];
            }

            control.Surface[bar.SelectedPosition, 1].Glyph = 30;
            control.Surface[bar.SelectedPosition, 1].Foreground = Color.LightGray;//this[_selectedPosition, 0].Foreground;

            control.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone() =>
            new ColorBar()
            {
                ControlThemeState = ControlThemeState.Clone()
            };
    }
}
