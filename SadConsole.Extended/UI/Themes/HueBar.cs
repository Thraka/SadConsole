using SadRogue.Primitives;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.UI.Controls;
using HueBarControl = SadConsole.UI.Controls.HueBar;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme for the <see cref="Controls.HueBar"/> control.
    /// </summary>
    public class HueBar : ThemeBase
    {
        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            if (!(control is HueBarControl)) throw new Exception($"Theme can only be added to a {nameof(HueBarControl)}");

            control.Surface = new CellSurface(control.Width, control.Height);
            control.Surface.Clear();
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is HueBarControl bar)) return;
            if (!bar.IsDirty) return;

            control.Surface.Fill(Color.White, Color.Black, 0, null);

            bar._positions = control.Width;
            ColorGradient gradient = new ColorGradient(Color.Red, Color.Yellow, Color.Green, Color.Turquoise, Color.Blue, Color.Purple, Color.Red);

            for (int x = 0; x < control.Width; x++)
            {
                control.Surface[x, 0].Glyph = 219;
                control.Surface[x, 0].Foreground = gradient.Lerp((float)x / (float)(control.Width - 1));
            }

            control.Surface[bar.SelectedPosition, 1].Glyph = 30;
            control.Surface[bar.SelectedPosition, 1].Foreground = Color.LightGray;//this[_selectedPosition, 0].Foreground;

            // Build an array of all the colors
            Color[] colors = new Color[control.Width];
            for (int x = 0; x < control.Width; x++)
                colors[x] = control.Surface[x, 0].Foreground;

            List<int> colorIndexesFinished = new List<int>(control.Width);

            foreach (var stop in gradient.Stops)
            {
                ColorMine.ColorSpaces.Rgb rgbColorStop = new ColorMine.ColorSpaces.Rgb() { R = stop.Color.R, G = stop.Color.G, B = stop.Color.B };
                Tuple<Color, double, int>[] colorWeights = new Tuple<Color, double, int>[control.Width];

                // Create a color weight for every cell compared to the color stop
                for (int x = 0; x < control.Width; x++)
                {
                    if (!colorIndexesFinished.Contains(x))
                    {
                        ColorMine.ColorSpaces.Rgb rgbColor = new ColorMine.ColorSpaces.Rgb() { R = colors[x].R, G = colors[x].G, B = colors[x].B };
                        ColorMine.ColorSpaces.Cmy cmyColor = rgbColor.To<ColorMine.ColorSpaces.Cmy>();

                        colorWeights[x] = new Tuple<Color, double, int>(colors[x], rgbColorStop.Compare(cmyColor, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison()), x);
                    }
                    else
                        colorWeights[x] = new Tuple<Color, double, int>(colors[x], 10000, x);
                }

                var foundColor = colorWeights.OrderBy(t => t.Item2).First();

                control.Surface[foundColor.Item3, 0].Foreground = stop.Color;
                colorIndexesFinished.Add(foundColor.Item3);
            }



            control.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone()
        {
            return new HueBar()
            {
                ControlThemeState = ControlThemeState.Clone()
            };
        }
    }
}
