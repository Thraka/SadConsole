#if SFML
using System;

namespace SadConsole
{
    public static partial class Engine
    {
        /// <summary>
        /// Renders the frames per second.
        /// </summary>
        public class FPSCounterComponent
        {
            SadConsole.Consoles.TextSurfaceRenderer consoleRender;
            SadConsole.Consoles.TextSurface console;
            SadConsole.Consoles.SurfaceEditor editor;

            int frameRate = 0;
            int frameCounter = 0;
            TimeSpan elapsedTime = TimeSpan.Zero;

            public FPSCounterComponent()
            {
                console = new SadConsole.Consoles.TextSurface(10, 1, SadConsole.Engine.DefaultFont);
                editor = new SadConsole.Consoles.SurfaceEditor(console);
                console.DefaultBackground = SFML.Graphics.Color.Black;
                editor.Clear();
                consoleRender = new SadConsole.Consoles.TextSurfaceRenderer();
            }


            public void Update()
            {
                elapsedTime += SadConsole.Engine.GameTimeUpdate.ElapsedGameTime;

                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }
            }


            public void Draw()
            {
                frameCounter++;

                string fps = string.Format("fps: {0}", frameRate);
                editor.Clear();
                editor.Print(0, 0, fps);
                consoleRender.Render(console, new SFML.System.Vector2i(0, 0));
            }
        }
    }
}
#endif