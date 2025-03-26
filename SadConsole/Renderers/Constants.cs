namespace SadConsole.Renderers.Constants;

/// <summary>
/// Renderer names used by hosts and types.
/// </summary>
public static class RendererNames
{
    /// <summary>
    /// The default renderer for a screen surface.
    /// </summary>
    public const string Default = "default";

    /// <summary>
    /// The renderer for a <see cref="ScreenSurface"/>.
    /// </summary>
    public const string ScreenSurface = "screensurface";

    /// <summary>
    /// The renderer for a <see cref="ScreenSurface"/> that doesn't use any steps, it directly renders the surface to a cached texture.
    /// </summary>
    public const string OptimizedScreenSurface = "optimizedscreensurface";

    /// <summary>
    /// The renderer for a <see cref="LayeredScreenSurface"/>.
    /// </summary>
    public const string LayeredScreenSurface = "layeredscreensurface";

    /// <summary>
    /// The renderer for a <see cref="UI.Window"/>.
    /// </summary>
    public const string Window = "window";

    /// <summary>
    /// An absent renderer.
    /// </summary>
    public const string None = "none";
}

/// <summary>
/// Renderer names used by hosts and types.
/// </summary>
public static class RenderStepNames
{
    /// <summary>
    /// The render step for a <see cref="IScreenSurface"/>.
    /// </summary>
    public const string Surface = "surface";

    /// <summary>
    /// The render step for a <see cref="IScreenSurface"/> where individual cells are only rendered if they're dirty.
    /// </summary>
    public const string SurfaceDirtyCells = "surface_dirtycells";

    /// <summary>
    /// The render step for a <see cref="IScreenSurface"/> where multiple layers are exposed through a property.
    /// </summary>
    public const string SurfaceLayered = "surface_layered";

    /// <summary>
    /// The render step for a <see cref="UI.ControlHost"/>.
    /// </summary>
    public const string ControlHost = "controlhost";

    /// <summary>
    /// The render step for a <see cref="UI.Window"/>.
    /// </summary>
    public const string Window = "windowmodal";

    /// <summary>
    /// The render step for a <see cref="Components.Cursor"/>.
    /// </summary>
    public const string Cursor = "cursor";

    /// <summary>
    /// The render step for a <see cref="Entities.EntityManager"/>.
    /// </summary>
    public const string EntityManager = "entitymanager";

    /// <summary>
    /// The render to draw the output texture of an <see cref="IRenderer"/>.
    /// </summary>
    public const string Output = "output";

    /// <summary>
    /// The render to draw the tint texture of an <see cref="IScreenSurface"/>.
    /// </summary>
    public const string Tint = "tint";
}

/// <summary>
/// Renderer names used by hosts and types.
/// </summary>
public static class RenderStepSortValues
{
    /// <summary>
    /// The render step for a <see cref="IScreenSurface"/>.
    /// </summary>
    public const uint Surface = 50;

    /// <summary>
    /// The render step for a <see cref="UI.ControlHost"/>.
    /// </summary>
    public const uint ControlHost = 80;

    /// <summary>
    /// The render step for a <see cref="UI.Window"/>.
    /// </summary>
    public const uint Window = 10;

    /// <summary>
    /// The render step for a <see cref="Components.Cursor"/>.
    /// </summary>
    public const uint Cursor = 70;

    /// <summary>
    /// The render step for a <see cref="Entities.EntityManager"/>.
    /// </summary>
    public const uint EntityRenderer = 60;

    /// <summary>
    /// The render to draw the output texture of an <see cref="IRenderer"/>.
    /// </summary>
    public const uint Output = 50;

    /// <summary>
    /// The render to draw the tint texture of an <see cref="IScreenSurface"/>.
    /// </summary>
    public const uint Tint = 90;
}
