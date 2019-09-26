#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A palette of colors.
    /// </summary>
    public class Palette : IEnumerable<Color>
    {
        private readonly Color[] colors;

        /// <summary>
        /// How many colors the palette has.
        /// </summary>
        public int Length => colors.Length;

        /// <summary>
        /// Gets or sets a color in the palette by index.
        /// </summary>
        /// <param name="index">Index of the color.</param>
        /// <returns>A color.</returns>
        public Color this[int index]
        {
            get => colors[index];
            set => colors[index] = value;
        }

        /// <summary>
        /// Creates a new palette with the specified amount of colors.
        /// </summary>
        /// <param name="colors">The number of colors.</param>
        public Palette(int colors)
        {
            this.colors = new Color[colors];

            for (int i = 0; i < colors; i++)
            {
                this.colors[i] = new Color();
            }
        }

        /// <summary>
        /// Creates a new palette of colors from a list of existing colors.
        /// </summary>
        /// <param name="colors">The list of colors this palette is made from.</param>
        public Palette(IEnumerable<Color> colors) => this.colors = new List<Color>(colors).ToArray();

        /// <summary>
        /// Shifts the entire palette once to the left.
        /// </summary>
        public void ShiftLeft()
        {
            Color lostColor = colors[0];
            Array.Copy(colors, 1, colors, 0, colors.Length - 1);
            colors[colors.Length - 1] = lostColor;
        }

        /// <summary>
        /// Shifts the entire palette once to the right.
        /// </summary>
        public void ShiftRight()
        {
            Color lostColor = colors[colors.Length - 1];
            Array.Copy(colors, 0, colors, 1, colors.Length - 1);
            colors[0] = lostColor;
        }

        /// <summary>
        /// Shifts a range of colors in the palette once to the left.
        /// </summary>
        /// <param name="startingIndex">The starting index in the palette.</param>
        /// <param name="count">The amount of colors to shift from the starting index.</param>
        public void ShiftLeft(int startingIndex, int count)
        {
            Color lostColor = colors[startingIndex];
            Array.Copy(colors, startingIndex + 1, colors, startingIndex, count - 1);
            colors[startingIndex + count - 1] = lostColor;
        }

        /// <summary>
        /// Shifts a range of colors in the palette once to the right.
        /// </summary>
        /// <param name="startingIndex">The starting index in the palette.</param>
        /// <param name="count">The amount of colors to shift from the starting index.</param>
        public void ShiftRight(int startingIndex, int count)
        {
            Color lostColor = colors[startingIndex + count - 1];
            Array.Copy(colors, startingIndex, colors, startingIndex + 1, count - 1);
            colors[startingIndex] = lostColor;
        }

        /// <summary>
        /// Gets the closest color in the palette to the provided color.
        /// </summary>
        /// <param name="color">The color to find.</param>
        /// <returns>The closest matching color.</returns>
        public Color GetNearest(Color color) => colors[GetNearestIndex(color)];

        /// <summary>
        /// Gets the index of the closest color in the palette to the provided color.
        /// </summary>
        /// <param name="color">The color to find.</param>
        /// <returns>The palette index of the closest color.</returns>
        public int GetNearestIndex(Color color)
        {
            int lowestDistanceIndex = -1;
            int lowestDistance = int.MaxValue;
            int currentDistance;
            for (int i = 0; i < colors.Length; i++)
            {
                currentDistance = Math.Abs(colors[i].R - color.R) + Math.Abs(colors[i].G - color.G) + Math.Abs(colors[i].B - color.B);

                if (currentDistance < lowestDistance)
                {
                    lowestDistance = currentDistance;
                    lowestDistanceIndex = i;
                }
            }
            return lowestDistanceIndex;
        }

        /// <summary>
        /// Gets the list of colors in the palette.
        /// </summary>
        /// <returns>The colors in the palette.</returns>
        public IEnumerator<Color> GetEnumerator() => ((IEnumerable<Color>)colors).GetEnumerator();

        /// <summary>
        /// Gets the list of colors in the palette.
        /// </summary>
        /// <returns>The colors in the palette.</returns>
        IEnumerator IEnumerable.GetEnumerator() => colors.GetEnumerator();
    }
}
