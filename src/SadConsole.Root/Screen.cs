using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public class MultiScreen: IScreen
    {
        public List<IScreen> Children { get; set; } = new List<IScreen>();

        public virtual void Draw(TimeSpan timeElapsed)
        {

        }

        public virtual void Update(TimeSpan timeElapsed)
        {

        }
    }

    public class Console: IScreen
    {
        public Point Position { get; set; }

        public virtual void Draw(TimeSpan timeElapsed)
        {

        }

        public virtual void Update(TimeSpan timeElapsed)
        {

        }
    }

    /// <summary>
    /// Represents a group of consoles.
    /// </summary>
    public partial class Screen: IScreen
    {
        public Point Position { get; set; }

        public List<IScreen> Children { get; set; } = new List<IScreen>();

        public List<TextSurface> Surfaces = new List<TextSurface>();

        public Renderers.TextSurfaceRenderer Renderer = new Renderers.TextSurfaceRenderer();

        public virtual void Draw(TimeSpan timeElapsed)
        {

        }

        public virtual void Update(TimeSpan timeElapsed)
        {

        }
    }

    public interface IScreen
    {
        void Draw(TimeSpan timeElapsed);

        void Update(TimeSpan timeElapsed);
    }
}
