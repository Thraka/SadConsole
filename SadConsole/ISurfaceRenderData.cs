using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Provides the basic information needed to render a surface.
    /// </summary>
    public interface ISurfaceRenderData
    {
        /// <summary>
        /// The pixel area on the screen this surface occupies.
        /// </summary>
        Rectangle AbsoluteArea { get; }

        /// <summary>
        /// Font used with rendering.
        /// </summary>
        Font Font { get; set; }

        /// <summary>
        /// The size of the <see cref="Font"/> cells applied to the object when rendering.
        /// </summary>
        Point FontSize { get; set; }

        /// <summary>
        /// When <see langword="true"/>, the <see cref="Draw"/> method forces the <see cref="Renderer"/> to refresh the backing texture with the latest state of the object.
        /// </summary>
        bool ForceRendererRefresh { get; set; }

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// The surface the screen object represents.
        /// </summary>
        ICellSurface Surface { get; }

        /// <summary>
        /// A tint used in rendering.
        /// </summary>
        Color Tint { get; set; }
    }
}
