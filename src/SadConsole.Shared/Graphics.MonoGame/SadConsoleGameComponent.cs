using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public partial class Game
    {
        public class SadConsoleGameComponent : DrawableGameComponent
        {
            internal SadConsoleGameComponent(Game game) : base(game)
            {
                DrawOrder = 5;
                UpdateOrder = 5;
            }

            public override void Draw(GameTime gameTime)
            {
                if (Settings.DoDraw)
                {
                    var oldViewPort = GraphicsDevice.Viewport;
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
                    GraphicsDevice.SetRenderTarget(null);

                    GraphicsDevice.Viewport = oldViewPort;

                    // If we're going to draw to the screen, do it.
                    if (Settings.DoFinalDraw)
                    {
                        Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                        Global.SpriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);
                        Global.SpriteBatch.End();
                    }
                }
            }

            public override void Update(GameTime gameTime)
            {
                if (Settings.DoUpdate)
                {
                    Global.GameTimeUpdate = gameTime;
                    Global.GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;

                    if (Game.IsActive)
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
}
