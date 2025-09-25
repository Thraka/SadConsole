using System;

namespace SadConsole.Extensions;

/// <summary>
/// Extensions for the <see cref="System.ReadOnlySpan{T}"/> type.
/// </summary>
public static class ReadOnlySpanExtensions
{
    /// <summary>
    /// Gets the next instnace of the specified character in a <see cref="char"/> span.
    /// </summary>
    /// <param name="span">The span.</param>
    /// <param name="value">The character to find.</param>
    /// <param name="index">The index of the character.</param>
    /// <returns>True when the character is found.</returns>
    public static bool Next(this System.ReadOnlySpan<char> span, char value, out int index)
    {
        index = span.IndexOf(value);

        return index != -1;
    }
}
