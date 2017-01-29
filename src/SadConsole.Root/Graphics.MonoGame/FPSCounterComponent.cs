using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Renderers;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public partial class Game
    {
        /// <summary>
        /// A component to draw how many frames per second the engine is performing at.
        /// </summary>
        public class FPSCounterComponent : DrawableGameComponent
        {
            SurfaceRenderer consoleRender;
            BasicSurface surface;
            SurfaceEditor editor;

            int frameRate = 0;
            int frameCounter = 0;
            TimeSpan elapsedTime = TimeSpan.Zero;


            public FPSCounterComponent(Microsoft.Xna.Framework.Game game)
                : base(game)
            {
                surface = new BasicSurface(30, 1);
                editor = new SurfaceEditor(surface);
                surface.DefaultBackground = Color.Black;
                editor.Clear();
                consoleRender = new SurfaceRenderer();
                DrawOrder = 8;
                Global.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
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
                consoleRender.Render(surface);

                Global.GraphicsDevice.SetRenderTarget(null);
                Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                Global.SpriteBatch.Draw(surface.LastRenderResult, Vector2.Zero, Color.White);
                Global.SpriteBatch.End();
            }
        }
    }
}
