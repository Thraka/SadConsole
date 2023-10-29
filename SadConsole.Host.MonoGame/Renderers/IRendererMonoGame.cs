using Microsoft.Xna.Framework.Graphics;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace SadConsole.Renderers;

/// <summary>
/// MonoGame-specific renderer settings.
/// </summary>
public interface IRendererMonoGame
{
    /// <summary>
    /// The blend state used by this renderer.
    /// </summary>
    BlendState MonoGameBlendState { get; set; }

    /// <summary>
    /// Cached set of rectangles used in rendering each cell.
    /// </summary>
    XnaRectangle[] CachedRenderRects { get; }
}
