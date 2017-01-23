using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public class Screen: IScreen
    {
        public Point Offset { get; set; }

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
       

    public interface IScreen
    {
        Point Offset { get; set; }

        List<IScreen> Children { get; set; }

        IScreen Parent { get; set; }

        void Draw(TimeSpan timeElapsed);

        void Update(TimeSpan timeElapsed);
    }

    public class Console : IScreen
    {
        private Point position;
        public Text.ITextSurfaceRendered TextSurface;
        public Renderers.ITextSurfaceRenderer Renderer;

        public List<IScreen> Children { get; set; } = new List<IScreen>();

        public Point Offset { get { return position; } set { position = value; TextSurface.IsDirty = true; } }

        public IScreen Parent { get; set; }

        public Console(int width, int height)
        {
            TextSurface = new Text.TextSurface(width, height);
            Renderer = new Renderers.TextSurfaceRenderer();

            FillWithRandomGarbage();
        }

        public void Draw(TimeSpan timeElapsed)
        {
            Renderer.Render(TextSurface);
            Global.DrawCalls.Add(new Tuple<Text.ITextSurfaceRendered, Point>(TextSurface, TextSurface.Font.GetWorldPosition(Offset)));
        }

        public void Update(TimeSpan timeElapsed)
        {
        }

        /// <summary>
        /// Fills a console with random colors and glyphs.
        /// </summary>
        public void FillWithRandomGarbage(bool useEffect = false)
        {
            Random random = new Random();
            //pulse.Reset();
            int charCounter = 0;
            for (int y = 0; y < TextSurface.Height * TextSurface.Width; y++)
            {
                TextSurface.Cells[y].Glyph = charCounter;
                TextSurface.Cells[y].Foreground = new Color(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 255);
                TextSurface.Cells[y].Background = new Color(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 255);
                charCounter++;
                if (charCounter > 255)
                    charCounter = 0;
            }
        }
    }
}
