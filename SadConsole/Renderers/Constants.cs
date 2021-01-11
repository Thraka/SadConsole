using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Renderers.Constants
{
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
}
