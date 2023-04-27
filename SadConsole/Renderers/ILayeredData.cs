using SadConsole.Components;

namespace SadConsole.Renderers;

/// <summary>
/// Provides the methods and properties used by the LayeredSurface renderer.
/// </summary>
public interface ILayeredData
{
    /// <summary>
    /// Access to the layers.
    /// </summary>
    LayeredSurface Layers { get; }
}
