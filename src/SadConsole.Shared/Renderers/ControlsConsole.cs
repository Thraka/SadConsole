using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a text surface to the screen.
    /// </summary>
    [DataContract]
    public class ControlsConsole : Basic
    {
        /// <summary>
        /// surface to render with the controls state.
        /// </summary>
        public SurfaceBase ControlsSurface;

        /// <summary>
        /// Renders a surface to the screen.
        /// </summary>
        /// <param name="surface">The surface to render.</param>
        public override void Render(Surfaces.SurfaceBase surface, bool force = false)
        {
            RenderBegin(surface, force);
            RenderCells(surface, force);
            RenderCells(ControlsSurface, force);
            ControlsSurface.IsDirty = false;
            RenderTint(surface, force);
            RenderEnd(surface, force);
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
        }
    }
}
