using System.Runtime.Serialization;
using SadRogue.Primitives;
using SadConsole.Components;
using SadConsole.Renderers;

namespace SadConsole;

/// <summary>
/// A surface that contains multiple layers
/// </summary>
[DataContract]
[System.Diagnostics.DebuggerDisplay("ScreenSurface (Layered)")]
public class LayeredScreenSurface : ScreenSurface, ILayeredData, ICellSurfaceResize
{
    /// <summary>
    /// The component that contains the layers for this surface.
    /// </summary>
    public LayeredSurface Layers { get; }

    /// <summary>
    /// Creates a new layered surface with the specified width and height, using default cells.
    /// </summary>
    /// <param name="width">The width in cells of the surface.</param>
    /// <param name="height">The height in cells of the surface.</param>
    /// <remarks>The surface of the base class is added to the layers component as the first layer.</remarks>
    public LayeredScreenSurface(int width, int height) : this(width, height, width, height, null) { }

    /// <summary>
    /// Creates a new screen object that can render a surface. Uses the specified cells to generate the surface.
    /// </summary>
    /// <param name="width">The width in cells of the surface.</param>
    /// <param name="height">The height in cells of the surface.</param>
    /// <param name="initialCells">The initial cells to seed the surface.</param>
    /// <remarks>The surface of the base class is added to the layers component as the first layer.</remarks>
    public LayeredScreenSurface(int width, int height, ColoredGlyphBase[] initialCells) : this(width, height, width, height, initialCells) { }

    /// <summary>
    /// Creates a new surface with the specified width and height, with <see cref="SadRogue.Primitives.Color.Transparent"/> for the background and <see cref="SadRogue.Primitives.Color.White"/> for the foreground.
    /// </summary>
    /// <param name="width">The visible width of the surface in cells.</param>
    /// <param name="height">The visible height of the surface in cells.</param>
    /// <param name="bufferWidth">The total width of the surface in cells.</param>
    /// <param name="bufferHeight">The total height of the surface in cells.</param>
    /// <remarks>The surface of the base class is added to the layers component as the first layer.</remarks>
    public LayeredScreenSurface(int width, int height, int bufferWidth, int bufferHeight) : this(width, height, bufferWidth, bufferHeight, null) { }

    /// <summary>
    /// Creates a surface with the specified width and height, with <see cref="SadRogue.Primitives.Color.Transparent"/> for the background and <see cref="SadRogue.Primitives.Color.White"/> for the foreground.
    /// </summary>
    /// <param name="width">The width of the surface in cells.</param>
    /// <param name="height">The height of the surface in cells.</param>
    /// <param name="bufferWidth">The total width of the surface in cells.</param>
    /// <param name="bufferHeight">The total height of the surface in cells.</param>
    /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cells for you.</param>
    /// <remarks>The surface of the base class is added to the layers component as the first layer.</remarks>
    public LayeredScreenSurface(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyphBase[]? initialCells) : base(width, height, bufferWidth, bufferHeight, initialCells)
    {
        Layers = new LayeredSurface();

        Renderer?.Dispose();
        Renderer = GameHost.Instance.GetRenderer(SadConsole.Renderers.Constants.RendererNames.LayeredScreenSurface);

        SadComponents.Add(Layers);
    }

    /// <summary>
    /// Creates a new surface using the existing surface.
    /// </summary>
    /// <param name="surface">The surface.</param>
    /// <param name="font">The font to use with the surface.</param>
    /// <param name="fontSize">The font size.</param>
    /// <remarks>The surface of the base class is added to the layers component as the first layer.</remarks>
    public LayeredScreenSurface(ICellSurface surface, IFont? font = null, Point? fontSize = null) : base(surface, font, fontSize)
    {
        Layers = new LayeredSurface();

        Renderer?.Dispose();
        Renderer = GameHost.Instance.GetRenderer(SadConsole.Renderers.Constants.RendererNames.LayeredScreenSurface);

        // Add the layers component which also adds the layers render step
        SadComponents.Add(Layers);
    }


    /// <summary>
    /// Resizes the surface to the specified width and height.
    /// </summary>
    /// <param name="viewWidth">The viewable width of the surface.</param>
    /// <param name="viewHeight">The viewable height of the surface.</param>
    /// <param name="totalWidth">The maximum width of the surface.</param>
    /// <param name="totalHeight">The maximum height of the surface.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="ICellSurface.DefaultForeground"/>, <see cref="ICellSurface.DefaultBackground"/> and glyph 0.</param>
    public new void Resize(int viewWidth, int viewHeight, int totalWidth, int totalHeight, bool clear) =>
        Layers.Resize(viewWidth, viewHeight, totalWidth, totalHeight, clear);

    /// <summary>
    /// Resizes the surface and view to the specified width and height.
    /// </summary>
    /// <param name="width">The width of the surface and view.</param>
    /// <param name="height">The height of the surface and view.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="ICellSurface.DefaultForeground"/>, <see cref="ICellSurface.DefaultBackground"/> and glyph 0.</param>
    public new void Resize(int width, int height, bool clear) =>
        Layers.Resize(width, height, clear);

    /// <summary>
    /// Returns the value "ScreenSurface (Layered)".
    /// </summary>
    /// <returns>The string "ScreenSurface (Layered)".</returns>
    public override string ToString() =>
        "ScreenSurface (Layered)";
}
