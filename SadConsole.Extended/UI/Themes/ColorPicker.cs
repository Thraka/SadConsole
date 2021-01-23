using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using ColorPickerControl = SadConsole.UI.Controls.ColorPicker;
using System;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme for the <see cref="Controls.ColorPicker"/> control.
    /// </summary>
    public class ColorPicker : ThemeBase
        {
            /// <inheritdoc />
            public override void Attached(ControlBase control)
            {
                if (!(control is ColorPickerControl)) throw new Exception($"Theme can only be added to a {nameof(ColorPickerControl)}");

                control.Surface = new CellSurface(control.Width, control.Height);
                control.Surface.Clear();
            }

            /// <inheritdoc />
            public override void UpdateAndDraw(ControlBase control, TimeSpan time)
            {
                if (!(control is ColorPickerControl picker)) return;

                if (!control.IsDirty) return;

                RefreshTheme(control.FindThemeColors(), control);

                Color[] colors = Color.White.LerpSteps(Color.Black, control.Height);
                Color[] colorsEnd = picker.MasterColor.LerpSteps(Color.Black, control.Height);

                for (int y = 0; y < control.Height; y++)
                {
                    control.Surface[0, y].Background = colors[y];
                    control.Surface[control.Width - 1, y].Background = colorsEnd[y];

                    control.Surface[0, y].Foreground = new Color(255 - colors[y].R, 255 - colors[y].G, 255 - colors[y].B);
                    control.Surface[control.Width - 1, y].Foreground = new Color(255 - colorsEnd[y].R, 255 - colorsEnd[y].G, 255 - colorsEnd[y].B);

                    Color[] rowColors = colors[y].LerpSteps(colorsEnd[y], control.Width);

                    for (int x = 1; x < control.Width - 1; x++)
                    {
                        control.Surface[x, y].Background = rowColors[x];
                        control.Surface[x, y].Foreground = new Color(255 - rowColors[x].R, 255 - rowColors[x].G, 255 - rowColors[x].B);
                    }
                }

                control.IsDirty = false;
            }

            /// <inheritdoc />
            public override ThemeBase Clone()
            {
                return new ColorPicker()
                {
                    ControlThemeState = ControlThemeState.Clone()
                };
            }
        }
}
