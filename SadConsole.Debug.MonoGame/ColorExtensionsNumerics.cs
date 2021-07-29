using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Numerics
{
    /// <summary>
    /// Extensions to convert <see cref="Vector4"/> to/from <see cref="Color"/>.
    /// </summary>
    public static class ColorExtensionsNumerics
    {
        /// <summary>
        /// Converts a <see cref="Color"/> to a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The <see cref="Vector4"/> representing the color.</returns>
        public static Vector4 ToVector4(this Color color) =>
            new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

        /// <summary>
        /// Converts a <see cref="Vector4"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The vector color to convert.</param>
        /// <returns>The <see cref="Color"/> representing the color.</returns>
        public static Color ToColor(this Vector4 value) =>
            new Color(value.X, value.Y, value.Z, value.W);
    }
}
