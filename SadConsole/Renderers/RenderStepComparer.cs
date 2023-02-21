using System.Collections.Generic;

namespace SadConsole.Renderers;

/// <summary>
/// Compares <see cref="IRenderStep"/> with the <see cref="IRenderStep.SortOrder"/> property.
/// </summary>
public class RenderStepComparer : IComparer<Renderers.IRenderStep>
{
    public static RenderStepComparer Instance { get; } = new RenderStepComparer();

    /// <inheritdoc/>
    public int Compare(IRenderStep? x, IRenderStep? y)
    {
        if (x == null && y == null) return 0;
        if (x == null && y != null) return 1;
        if (x != null && y == null) return -1;

        if (x == y || x.SortOrder == y.SortOrder)
            return 0;

        if (x!.SortOrder < y!.SortOrder)
            return -1;

        // default for if (x.SortOrder > y.SortOrder)
        return 1;
    }
}
