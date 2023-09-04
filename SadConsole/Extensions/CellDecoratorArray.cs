using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SadConsole;

namespace System;

/// <summary>
/// Extensions for an array of <see cref="CellDecorator"/> types.
/// </summary>
public static class CellDecoratorArray
{
    /// <summary>
    /// Default eqaulity comparer for an array of cell decorators.
    /// </summary>
    public static EqualityComparer<CellDecorator[]> DefaultComparer { get; } = EqualityComparer<CellDecorator[]>.Default;

    /// <summary>
    /// Determines whether the contents of two <see cref="CellDecorator"/> arrays are equal.
    /// </summary>
    /// <param name="self">The first object to compare.</param>
    /// <param name="other">The second object to compare.</param>
    /// <returns>A boolean value to indicate whether or not the two arrays are considered equal.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ItemsMatch(this CellDecorator[] self, CellDecorator[]? other) =>
        DefaultComparer.Equals(self, other);
}
