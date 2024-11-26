﻿using SadRogue.Primitives;

namespace System.Numerics;

/// <summary>
/// Extensions to convert <see cref="Vector4"/> to/from <see cref="Color"/>.
/// </summary>
public static class ExtensionsColorNumerics
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

    public static Vector2 ToVector2(this Point point) =>
        new Vector2(point.X, point.Y);

    public static Vector2 ToUV(this Point point, Point size) =>
        new Vector2((float)point.X / size.X, (float)point.Y / size.Y);
}
