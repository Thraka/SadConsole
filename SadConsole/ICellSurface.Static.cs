using System;

namespace SadConsole
{
    public partial interface ICellSurface
    {
        /// <summary>
        /// Glyph indexes for a thin line.
        /// </summary>
        public static int[] ConnectedLineThin => new[]
          { 218, 196, 191, // ┌─┐
            179, 197, 179, // │┼│
            192, 196, 217, // └─┘

                 194,      //  ┬
            195,      180, // ├ ┤
                 193};     //  ┴

        /// <summary>
        /// Glyph indexes for a thick line.
        /// </summary>
        public static int[] ConnectedLineThick => new[]
          { 201, 205, 187, // ╔═╗
            186, 206, 186, // ║╬║
            200, 205, 188, // ╚═╝

                 203,      //  ╦
            204,      185, // ╠ ╣
                 202};     //  ╩

        /// <summary>
        /// Glyph indexes for a thin line using a SadConsole extended font.
        /// </summary>
        public static int[] ConnectedLineThinExtended => new[]
          { 261, 257, 262,
            256, 295, 258,
            264, 259, 263,

                 001,
            001,      001,
                 001};

        /// <summary>
        /// Glyph indexes for an empty line 0. 
        /// </summary>
        public static int[] ConnectedLineEmpty => new[]
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
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <param name="connectedLineStyle">The array to check based on the <see cref="ConnectedLineIndex"/> enum.</param>
        /// <returns>True when the line style is correct.</returns>
        public static bool ValidateLineStyle<T>(in T[] connectedLineStyle) =>
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
