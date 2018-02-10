using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor
{
    public class Game: MonoGame.Forms.Controls.UpdateWindow
    {
        GameTime gameTime = new GameTime();

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            
        }

        protected override void Initialize()
        {
            base.Initialize();

            

            Global.OriginalRenderTarget = SwapChainRenderTarget;

            Global.GraphicsDevice = GraphicsDevice;
            //Global.GraphicsDeviceManager = GraphicsDeviceManager;
            Global.SpriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
            Global.FontDefault = Global.LoadFont(SadConsole.Game.WpfFont).GetFont(SadConsole.Font.FontSizes.One);
            //Global.FontDefault.ResizeGraphicsDeviceManager(SadConsole.Game.wp, consoleHeight, 0, 0);

            Global.WindowWidth = Global.RenderWidth = (Global.FontDefault.Size.X * SadConsole.Game.WpfConsoleWidth);
            Global.WindowHeight = Global.RenderHeight = (Global.FontDefault.Size.Y * SadConsole.Game.WpfConsoleHeight);


            Global.ResetRendering();

            // Tell the main engine we're ready
            SadConsole.Game.OnInitialize?.Invoke();

            // After we've init, clear the graphics device so everything is ready to start
            Global.GraphicsDevice.SetRenderTarget(Global.OriginalRenderTarget);
        }

        protected override void Draw()
        {
            base.Draw();



            if (Settings.DoDraw)
            {
#if WPF
                    Global.OriginalRenderTarget = (Microsoft.Xna.Framework.Graphics.RenderTarget2D)GraphicsDevice.GetRenderTargets()[0].RenderTarget;
#endif
                Global.GameTimeRender = gameTime;
                Global.GameTimeElapsedRender = gameTime.ElapsedGameTime.TotalSeconds;

                // Clear draw calls for next run
                Global.DrawCalls.Clear();

                // Make sure all items in the screen are drawn. (Build a list of draw calls)
                Global.CurrentScreen?.Draw(gameTime.ElapsedGameTime);

                SadConsole.Game.OnDraw?.Invoke(gameTime);

                // Render to the global output texture
                GraphicsDevice.SetRenderTarget(Global.RenderOutput);
                GraphicsDevice.Clear(Settings.ClearColor);

                // Render each draw call
                Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                foreach (var call in Global.DrawCalls)
                {
                    call.Draw();
                }

                Global.SpriteBatch.End();
                GraphicsDevice.SetRenderTarget(Global.OriginalRenderTarget);

                // If we're going to draw to the screen, do it.
                if (Settings.DoFinalDraw)
                {
                    Editor.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                    Editor.spriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);
                    Editor.spriteBatch.End();
                }
            }






        
        }

        protected override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            base.Update(gameTime);

            if (Settings.DoUpdate)
            {
                Global.GameTimeUpdate = gameTime;
                Global.GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;

                //if (Game.IsActive)
                {
                    if (Settings.Input.DoKeyboard)
                    {
                        Global.KeyboardState.Update(gameTime);
                        Global.KeyboardState.Process();
                    }

                    if (Settings.Input.DoMouse)
                    {
                        Global.MouseState.Update(gameTime);
                        Global.MouseState.Process();
                    }
                }

                Global.CurrentScreen?.Update(gameTime.ElapsedGameTime);

                SadConsole.Game.OnUpdate?.Invoke(gameTime);
            }
        }
    }
}
