using System.Collections.Generic;

namespace SadConsole;

/// <summary>
/// Compares <see cref="IScreenObject"/> with the <see cref="IScreenObject.SortOrder"/> property.
/// </summary>
public class ScreenObjectComparer : IComparer<IScreenObject>
{
    /// <summary>
    /// Shared instance of the <see cref="ScreenObjectComparer"/>.
    /// </summary>
    public static ScreenObjectComparer Instance { get; } = new ScreenObjectComparer();

    /// <inheritdoc/>
    public int Compare(IScreenObject? x, IScreenObject? y)
    {
        if (x == null && y == null) return 0;
        if (x == null && y != null) return 1;
        if (x != null && y == null) return -1;

        if (x == y || x!.SortOrder == y!.SortOrder)
            return 0;

        if (x!.SortOrder < y!.SortOrder)
            return -1;

        // default for if (x.SortOrder > y.SortOrder)
        return 1;
    }
}
