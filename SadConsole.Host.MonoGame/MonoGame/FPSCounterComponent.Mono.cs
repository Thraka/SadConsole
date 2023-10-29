using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = SadRogue.Primitives.Color;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace SadConsole.Host;

public partial class Game
{
    /// <summary>
    /// A component to draw how many frames per second the engine is performing at.
    /// </summary>
    public class FPSCounterComponent : DrawableGameComponent
    {
        private readonly Console surface;
        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan delta = TimeSpan.Zero;

        /// <inheritdoc/>
        public FPSCounterComponent(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            surface = new Console(30, 1);
            surface.Surface.DefaultBackground = Color.Black;
            surface.Clear();
            DrawOrder = 8;
            Global.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        /// <inheritdoc/>
        public override void Update(GameTime gameTime)
        {
            delta += gameTime.ElapsedGameTime;

            if (delta > TimeSpan.FromSeconds(1))
            {
                delta -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        /// <inheritdoc/>
        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            surface.Clear();
            surface.Print(0, 0, $"fps: {frameRate}", Color.White, Color.Black);
            surface.Render(gameTime.ElapsedGameTime);
            
            Game.GraphicsDevice.SetRenderTarget(null);
            Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            Global.SharedSpriteBatch.Draw(((GameTexture)surface.Renderer.Output).Texture, Vector2.Zero, XnaColor.White);
            Global.SharedSpriteBatch.End();
        }
    }
}
