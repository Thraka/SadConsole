using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public class Screen: IScreen
    {
        public Point Position { get; set; }

        public List<IScreen> Children { get; set; } = new List<IScreen>();

        public IScreen Parent { get; set; }

        public virtual void Draw(TimeSpan timeElapsed)
        {
            foreach (var child in Children)
                child.Draw(timeElapsed);
        }

        public virtual void Update(TimeSpan timeElapsed)
        {
            foreach (var child in Children)
                child.Update(timeElapsed);
        }
    }
    
    /// <summary>
    /// A visible screen object.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// The top-left coordinate of the screen object.
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Child screen objects related to this one.
        /// </summary>
        List<IScreen> Children { get; set; }

        /// <summary>
        /// A parented screen object.
        /// </summary>
        IScreen Parent { get; set; }

        /// <summary>
        /// Indicates this screen object is visible.
        /// </summary>
        bool IsVisible { get; set; }

        void Draw(TimeSpan timeElapsed);

        void Update(TimeSpan timeElapsed);
    }

    //public class Console : IScreen
    //{
    //    private Point position;
    //    public Surface.ISurface TextSurface;
    //    public Renderers.ISurfaceRenderer Renderer;

    //    public List<IScreen> Children { get; set; } = new List<IScreen>();

    //    public Point Position { get { return position; } set { position = value; TextSurface.IsDirty = true; } }

    //    public IScreen Parent { get; set; }

    //    public Console(int width, int height)
    //    {
    //        TextSurface = new Surface.Basic(width, height);
    //        Renderer = new Renderers.SurfaceRenderer();

    //        FillWithRandomGarbage();
    //    }

    //    public void Draw(TimeSpan timeElapsed)
    //    {
    //        Renderer.Render(TextSurface);
    //        Global.DrawCalls.Add(new Tuple<Surface.ISurface, Point>(TextSurface, TextSurface.Font.GetWorldPosition(Position)));
    //    }

    //    public void Update(TimeSpan timeElapsed)
    //    {
    //    }

    //    /// <summary>
    //    /// Fills a console with random colors and glyphs.
    //    /// </summary>
    //    public void FillWithRandomGarbage(bool useEffect = false)
    //    {
    //        Random random = new Random();
    //        //pulse.Reset();
    //        int charCounter = 0;
    //        for (int y = 0; y < TextSurface.Height * TextSurface.Width; y++)
    //        {
    //            TextSurface.Cells[y].Glyph = charCounter;
    //            TextSurface.Cells[y].Foreground = new Color(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 255);
    //            TextSurface.Cells[y].Background = new Color(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 255);
    //            charCounter++;
    //            if (charCounter > 255)
    //                charCounter = 0;
    //        }
    //    }
    //}
}
