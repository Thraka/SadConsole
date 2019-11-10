using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.MonoGame
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
                if (SadConsole.Settings.DoDraw)
                {
                    MonoGame.Game game = (MonoGame.Game)Game;

                    SadConsole.Global.DrawFrameDelta = gameTime.ElapsedGameTime;

                    // Clear draw calls for next run
                    SadConsole.Game.Instance.DrawCalls.Clear();

                    // Make sure all items in the screen are drawn. (Build a list of draw calls)
                    SadConsole.Global.Screen?.Draw();

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameDraw();

                    // Render to the global output texture
                    GraphicsDevice.SetRenderTarget(Global.RenderOutput);
                    GraphicsDevice.Clear(SadRogue.Primitives.SadRogueColorExtensions.ToMonoColor(SadConsole.Settings.ClearColor));

                    // Render each draw call
                    Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                    foreach (DrawCalls.IDrawCall call in SadConsole.Game.Instance.DrawCalls)
                    {
                        call.Draw();
                    }

                    Global.SharedSpriteBatch.End();
                    GraphicsDevice.SetRenderTarget(null);

                    // If we're going to draw to the screen, do it.
                    if (SadConsole.Settings.DoFinalDraw)
                    {
                        Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                        Global.SharedSpriteBatch.Draw(Global.RenderOutput, SadRogue.Primitives.SadRogueRectangleExtensions.ToMonoRectangle(SadConsole.Settings.Rendering.RenderRect), Color.White);
                        Global.SharedSpriteBatch.End();
                    }
                }
            }

            public override void Update(GameTime gameTime)
            {
                if (SadConsole.Settings.DoUpdate)
                {
                    var game = (Game)Game;

                    SadConsole.Global.UpdateFrameDelta = gameTime.ElapsedGameTime;

                    if (Game.IsActive)
                    {
                        if (SadConsole.Settings.Input.DoKeyboard)
                        {
                            SadConsole.Global.Keyboard.Update(SadConsole.Global.UpdateFrameDelta);

                            if (SadConsole.Global.FocusedConsoles.Console != null && SadConsole.Global.FocusedConsoles.Console.UseKeyboard)
                            {
                                SadConsole.Global.FocusedConsoles.Console.ProcessKeyboard(SadConsole.Global.Keyboard);
                            }

                        }

                        if (SadConsole.Settings.Input.DoMouse)
                        {
                            SadConsole.Global.Mouse.Update(SadConsole.Global.UpdateFrameDelta);
                            SadConsole.Global.Mouse.Process();
                        }
                    }

                    SadConsole.Global.Screen?.Update();

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameUpdate();
                }
            }
        }
    }
}
