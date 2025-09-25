namespace SadConsole;

/// <summary>
/// An interface that provides a Surface property which can be set.
/// </summary>
public partial interface ISurfaceSettable
{
    /// <summary>
    /// Sets the surface.
    /// </summary>
    ICellSurface Surface { get; set; }
}
