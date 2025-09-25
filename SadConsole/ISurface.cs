namespace SadConsole;

/// <summary>
/// An interface that provides a Surface property that allows the editor extensions to work.
/// </summary>
public partial interface ISurface
{
    /// <summary>
    /// The surface.
    /// </summary>
    ICellSurface Surface { get; }
}
