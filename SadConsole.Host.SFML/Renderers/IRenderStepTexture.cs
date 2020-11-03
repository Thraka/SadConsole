using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Indicates the render step has an associated texture.
    /// </summary>
    public interface IRenderStepTexture
    {
        /// <summary>
        /// The texture created by the render step.
        /// </summary>
        RenderTexture BackingTexture { get; }
    }
}
