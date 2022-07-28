using System;
using System.Collections.Generic;
using System.Text;

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
    /// The render step for a <see cref="Entities.Renderer"/>.
    /// </summary>
    public const string EntityRenderer = "entityrenderer";

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
    /// The render step for a <see cref="Entities.Renderer"/>.
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
