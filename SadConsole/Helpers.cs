using System.Runtime.CompilerServices;

namespace SadConsole;

/// <summary>
/// General code helpers.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Checks for the presense of a flag in a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="flag">The flag.</param>
    /// <returns><see langword="true"/> when the flag exists in the value; otherwise <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag(in int value, in int flag) => (value & flag) == flag;

    /// <summary>
    /// Sets a flag in a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="flag">The flag.</param>
    /// <returns>A new value with the flag added.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetFlag(int value, int flag) => value | flag;

    /// <summary>
    /// Removes a flag from a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="flag">The flag.</param>
    /// <returns>A new value with the flag removed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UnsetFlag(int value, int flag) => value & ~flag;
}
