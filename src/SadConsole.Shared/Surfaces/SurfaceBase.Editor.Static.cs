using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Surfaces
{
    public partial class SurfaceBase
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
            256, 000, 258,
            264, 259, 263,

                 001,
            001,      001,
                 001};


        /// <summary>
        /// Returns a value that indicates a line style array is not null and contains the required number of elements.
        /// </summary>
        /// <param name="connectedLineStyle">The array to check based on the <see cref="ConnectedLineIndex"/> enum.</param>
        /// <returns>True when the line style is correct.</returns>
        public static bool ValidateLinestyle(in int[] connectedLineStyle)
        {
            return connectedLineStyle != null && connectedLineStyle.Length == Enum.GetValues(typeof(ConnectedLineIndex)).Length;
        }

        /// <summary>
        /// Array index enum for line glyphs.
        /// </summary>
        public enum ConnectedLineIndex : int
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
