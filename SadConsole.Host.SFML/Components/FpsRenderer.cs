using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Components
{
    /// <summary>
    /// Draws an image on top of a console.
    /// </summary>
    public class FpsRenderer : LogicComponent
    {
        private readonly Console surface;
        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan delta = TimeSpan.Zero;

        public FpsRenderer()
        {
            surface = new Console(30, 1);
            surface.DefaultBackground = Color.Black;
            surface.Clear();
            SortOrder = 8;
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="host">The host of the component.</param>
        /// <param name="delta">Unused.</param>
        public override void Render(IScreenObject host, TimeSpan delta)
        {
            frameCounter++;
            surface.Clear();
            surface.Print(0, 0, $"fps: {frameRate}", Color.White, Color.Black);
            surface.Render(delta);
        }

        public override void Update(IScreenObject host, TimeSpan delta)
        {
            this.delta += delta;

            if (this.delta > TimeSpan.FromSeconds(1))
            {
                this.delta -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }
    }
}
