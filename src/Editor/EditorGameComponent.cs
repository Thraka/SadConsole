using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoesisGUI.MonoGameWrapper;
using NoesisGUI.MonoGameWrapper.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SadConsole
{
    public class EditorGameComponent : DrawableGameComponent
    {
        private TimeSpan lastUpdateTotalGameTime;

        internal EditorGameComponent(Game game) : base(game)
        {
            DrawOrder = 5;
            UpdateOrder = 5;

            Editor.NoesisManager.CreateNoesisGUI();
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var font in SadConsole.Global.Fonts.Values)
                font.Image.Dispose();

            Editor.Xaml.WindowBase.Windows.Clear();
            Editor.NoesisManager.DestroyGUI();
        }



        protected override void UnloadContent()
        {
            base.UnloadContent();


        }



        public override void Draw(GameTime gameTime)
        {
            if (Settings.DoDraw)
            {
                // GUI
                Editor.NoesisManager.noesisGUIWrapper.PreRender();
                
                // SADCONSOLE
                Global.GameTimeRender = gameTime;
                Global.GameTimeElapsedRender = gameTime.ElapsedGameTime.TotalSeconds;

                // Clear draw calls for next run
                Global.DrawCalls.Clear();

                // Make sure all items in the screen are drawn. (Build a list of draw calls)
                Global.CurrentScreen?.Draw(gameTime.ElapsedGameTime);
                
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

                // If we're going to draw to the screen, do it.
                if (Settings.DoFinalDraw)
                {
                    Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                    Global.SpriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);
                    Global.SpriteBatch.End();
                }

                SadConsole.Game.OnDraw?.Invoke(gameTime);

                // GUI
                Editor.NoesisManager.noesisGUIWrapper.Render();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Settings.DoUpdate)
            {
                // GUI
                this.lastUpdateTotalGameTime = gameTime.TotalGameTime;
                Editor.NoesisManager.noesisGUIWrapper.UpdateInput(gameTime, isWindowActive: this.Game.IsActive);

                bool blockInput = Editor.Globals.BlockSadConsoleInput
                    || Editor.NoesisManager.noesisGUIWrapper.Input.ConsumedKeyboardKeys.Count != 0
                    || Editor.NoesisManager.noesisGUIWrapper.Input.ConsumedMouseButtons.Count != 0
                    || Editor.NoesisManager.noesisGUIWrapper.Input.ConsumedMouseDeltaWheel != 0;

                // SadConsole
                Global.GameTimeUpdate = gameTime;
                Global.GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;

                if (Game.IsActive && !blockInput)
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

                // GUI
                Editor.NoesisManager.noesisGUIWrapper.Update(gameTime);
            }
        }
    }
}
