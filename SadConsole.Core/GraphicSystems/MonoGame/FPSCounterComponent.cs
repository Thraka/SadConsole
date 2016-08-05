#if MONOGAME

using Microsoft.Xna.Framework;
using SadConsole.Consoles;
using System;

namespace SadConsole
{
    /// <summary>
    /// A component to draw how many frames per second the engine is performing at.
    /// </summary>
    public class FPSCounterComponent : DrawableGameComponent
    {
        TextSurfaceRenderer consoleRender;
        TextSurface console;
        SurfaceEditor editor;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        public FPSCounterComponent(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            console = new TextSurface(30, 1, Engine.DefaultFont);
            editor = new SurfaceEditor(console);
            console.DefaultBackground = Color.Black;
            editor.Clear();
            consoleRender = new TextSurfaceRenderer();
        }


        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);
            editor.Clear();
            editor.Print(0, 0, fps);
            consoleRender.Render(console, Point.Zero);
        }
    }
}
#endif