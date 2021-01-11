using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Numerics
{
    public static class ColorExtensionsNumerics
    {
        public static Vector4 ToVector4(this Color color) =>
            new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }
}
