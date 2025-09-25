using SadRogue.Primitives;
using ColorMonoGame = Microsoft.Xna.Framework.Color;
using PointMonoGame = Microsoft.Xna.Framework.Point;

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
    /// Converts a <see cref="Color"/> to a <see cref="Vector3"/>.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Vector3"/> representing the color.</returns>
    public static Vector3 ToVector3(this Color color) =>
        new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);

    /// <summary>
    /// Converts a <see cref="Vector4"/> to a <see cref="Color"/>.
    /// </summary>
    /// <param name="value">The vector color to convert.</param>
    /// <returns>The <see cref="Color"/> representing the color.</returns>
    public static Color ToColor(this Vector4 value) =>
        new Color(value.X, value.Y, value.Z, value.W);

    /// <summary>
    /// Converts a <see cref="Vector3"/> to a <see cref="Color"/>.
    /// </summary>
    /// <param name="value">The vector color to convert.</param>
    /// <returns>The <see cref="Color"/> representing the color.</returns>
    public static Color ToColor(this Vector3 value) =>
        new Color(value.X, value.Y, value.Z);

    /// <summary>
    /// Converts a <see cref="Point"/> to a <see cref="Vector2"/>.
    /// </summary>
    /// <param name="point">The point to convert.</param>
    /// <returns>The <see cref="Vector2"/> representing the point.</returns>
    public static Vector2 ToVector2(this Point point) =>
        new Vector2(point.X, point.Y);

    /// <summary>
    /// Gets a <see cref="Vector2"/> representing the UV coordinates of a <see cref="Point"/> within a <see cref="Point"/> size.
    /// </summary>
    /// <param name="point">The coordinates within the size.</param>
    /// <param name="size">The size.</param>
    /// <returns>The UV coordinates.</returns>
    public static Vector2 ToUV(this Point point, Point size) =>
        new Vector2((float)point.X / size.X, (float)point.Y / size.Y);
}

/// <summary>
/// Extensions to convert <see cref="Vector4"/> to/from <see cref="Color"/>.
/// </summary>
public static class ExtensionsColorNumericsMonoGame
{
    /// <summary>
    /// Converts a <see cref="Vector4"/> to a <see cref="ColorMonoGame"/>.
    /// </summary>
    /// <param name="value">The vector color to convert.</param>
    /// <returns>The <see cref="ColorMonoGame"/> representing the color.</returns>
    public static ColorMonoGame ToMonoGameColor(this Vector4 value) =>
        new ColorMonoGame(value.X, value.Y, value.Z, value.W);

    /// <summary>
    /// Converts a <see cref="Vector3"/> to a <see cref="ColorMonoGame"/>.
    /// </summary>
    /// <param name="value">The vector color to convert.</param>
    /// <returns>The <see cref="ColorMonoGame"/> representing the color.</returns>
    public static ColorMonoGame ToMonoGameColor(this Vector3 value) =>
        new ColorMonoGame(value.X, value.Y, value.Z);

    /// <summary>
    /// Gets a <see cref="Vector2"/> representing the UV coordinates of a <see cref="PointMonoGame"/> within a <see cref="PointMonoGame"/> size.
    /// </summary>
    /// <param name="point">The coordinates within the size.</param>
    /// <param name="size">The size.</param>
    /// <returns>The UV coordinates.</returns>
    public static Vector2 ToUV(this PointMonoGame point, PointMonoGame size) =>
        new Vector2((float)point.X / size.X, (float)point.Y / size.Y);
}
