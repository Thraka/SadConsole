using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// Represents a group of consoles.
    /// </summary>
    public partial class Screen
    {
        public List<TextSurface> Surfaces = new List<TextSurface>();

        public Renderers.TextSurfaceRenderer Renderer = new Renderers.TextSurfaceRenderer();

        public virtual void Draw(TimeSpan timeElapsed)
        {

        }

        public virtual void Update(TimeSpan timeElapsed)
        {

        }
    }
}
