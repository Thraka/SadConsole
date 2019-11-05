namespace SadConsole
{
    using System;

    public partial class CellSurface
    {
        /// <summary>
        /// Glyph indexes for a thin line.
        /// </summary>
        public static readonly int[] ConnectedLineThin =
          { 218, 196, 191,
            179, 197, 179,
            192, 196, 217,

                 194,
            195,      180,
                 193};

        /// <summary>
        /// Glyph indexes for a thick line.
        /// </summary>
        public static readonly int[] ConnectedLineThick =
          { 201, 205, 187,
            186, 206, 186,
            200, 205, 188,

                 203,
            204,      185,
                 202};

        /// <summary>
        /// Glyph indexes for a thin line using a SadConsole extended font.
        /// </summary>
        public static readonly int[] ConnectedLineThinExtended =
          { 261, 257, 262,
            256, 295, 258,
            264, 259, 263,

                 001,
            001,      001,
                 001};

        /// <summary>
        /// Glyph indexes for an empty line 0. 
        /// </summary>
        public static readonly int[] ConnectedLineEmpty =
          { 0, 0, 0,
            0, 0, 0,
            0, 0, 0,

               0,
            0,    0,
               0};

        /// <summary>
        /// Creates an array of glyphs that can be used as a connected line.
        /// </summary>
        /// <param name="singleGlyph">The glyph to use for the connected line array.</param>
        /// <returns>An array of glyphs.</returns>
        public static int[] CreateLine(int singleGlyph) =>
            new int[] { singleGlyph, singleGlyph, singleGlyph,
                        singleGlyph, singleGlyph, singleGlyph,
                        singleGlyph, singleGlyph, singleGlyph,
                        singleGlyph, singleGlyph, singleGlyph, singleGlyph };

        /// <summary>
        /// Returns a value that indicates a line style array is not null and contains the required number of elements.
        /// </summary>
        /// <param name="connectedLineStyle">The array to check based on the <see cref="ConnectedLineIndex"/> enum.</param>
        /// <returns>True when the line style is correct.</returns>
        public static bool ValidateLineStyle(in int[] connectedLineStyle) =>
            connectedLineStyle != null && connectedLineStyle.Length == Enum.GetValues(typeof(ConnectedLineIndex)).Length;

        /// <summary>
        /// Array index enum for line glyphs.
        /// </summary>
        public enum ConnectedLineIndex
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            TopLeft, Top, TopRight,
            Left, Middle, Right,
            BottomLeft, Bottom, BottomRight,
            TopMiddleToDown,
            LeftMiddleToRight, RightMiddleToLeft,
            BottomMiddleToTop,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }
    }
}
