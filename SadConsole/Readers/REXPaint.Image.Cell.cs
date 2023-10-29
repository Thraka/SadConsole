﻿namespace SadConsole.Readers;

public partial class REXPaintImage
{
    /// <summary>
    /// A RexPaint layer cell.
    /// </summary>
    public struct Cell
    {
        /// <summary>
        /// The character for the cell.
        /// </summary>
        public int Character;

        /// <summary>
        /// The foreground color of the cell.
        /// </summary>
        public Color Foreground;

        /// <summary>
        /// The background color of the cell.
        /// </summary>
        public Color Background;

        /// <summary>
        /// Creates a new REXPaint cell.
        /// </summary>
        /// <param name="character">The glyph.</param>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        public Cell(int character, Color foreground, Color background)
        {
            Character = character;
            Foreground = foreground;
            Background = background;
        }

        /// <summary>
        /// Returns true when the current color is considered transparent.
        /// </summary>
        /// <returns>True when transparent.</returns>
        public bool IsTransparent() => Background == Color.Transparent;
    }
}
