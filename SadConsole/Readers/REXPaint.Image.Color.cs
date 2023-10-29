﻿namespace SadConsole.Readers;

public partial class REXPaintImage
{
#pragma warning disable CS0660,CS0661 // Type defines operator == or operator != but does not override Object.Equals(object o)
    /// <summary>
    /// A RexPaint color.
    /// </summary>
    public struct Color
#pragma warning restore CS0660,CS0661 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        /// <summary>
        /// The red channel of the color.
        /// </summary>
        public byte R;

        /// <summary>
        /// The green channel of the color.
        /// </summary>
        public byte G;

        /// <summary>
        /// The blue channel of the color.
        /// </summary>
        public byte B;

        /// <summary>
        /// Creates a new RexPaint color with the specified RGB channels.
        /// </summary>
        /// <param name="r">The red channel of the color.</param>
        /// <param name="g">The green channel of the color.</param>
        /// <param name="b">The blue channel of the color.</param>
        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <inheritdoc/>
        public static bool operator ==(Color left, Color right) => left.R == right.R && left.G == right.G && left.B == right.B;

        /// <inheritdoc/>
        public static bool operator !=(Color left, Color right) => left.R != right.R || left.G != right.G || left.B != right.B;

        /// <summary>
        /// Returns the transparent color used by RexPaint: rgb(255, 0, 255).
        /// </summary>
        public static Color Transparent => new Color(255, 0, 255);
    }
}
