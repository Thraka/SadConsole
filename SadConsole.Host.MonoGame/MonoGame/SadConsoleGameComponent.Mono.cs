﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Host
{
    public partial class Game
    {
        /// <summary>
        /// A game component that handles updating, input, and rendering of SadConsole.
        /// </summary>
        public class SadConsoleGameComponent : DrawableGameComponent
        {
            internal SadConsoleGameComponent(Game game) : base(game)
            {
                DrawOrder = 5;
                UpdateOrder = 5;
            }

#if NOESIS
            protected override void LoadContent()
            {
                NoesisManager.CreateNoesisGUI();
            }
#endif

            /// <summary>
            /// Draws the SadConsole frame through draw calls when <see cref="SadConsole.Settings.DoDraw"/> is true.
            /// </summary>
            /// <param name="gameTime">Time between drawing frames.</param>
            public override void Draw(GameTime gameTime)
            {
                if (SadConsole.Settings.DoDraw)
                {
#if NOESIS
                    // GUI
                    NoesisManager.noesisGUIWrapper.PreRender();
#endif

                    Host.Game game = (Host.Game)Game;

                    SadConsole.GameHost.Instance.DrawFrameDelta = gameTime.ElapsedGameTime;
                    Global.RenderLoopGameTime = gameTime;

                    // Clear draw calls for next run
                    SadConsole.Game.Instance.DrawCalls.Clear();

                    // Make sure all items in the screen are drawn. (Build a list of draw calls)
                    SadConsole.GameHost.Instance.Screen?.Render(SadConsole.GameHost.Instance.DrawFrameDelta);

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameDraw();

                    // Render to the global output texture
                    GraphicsDevice.SetRenderTarget(Global.RenderOutput);
                    GraphicsDevice.Clear(SadRogue.Primitives.SadRogueColorExtensions.ToMonoColor(SadConsole.Settings.ClearColor));

                    // Render each draw call
                    Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, SadConsole.Host.Settings.MonoGameScreenBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                    foreach (DrawCalls.IDrawCall call in SadConsole.Game.Instance.DrawCalls)
                        call.Draw();

                    Global.SharedSpriteBatch.End();
                    GraphicsDevice.SetRenderTarget(null);

                    // If we're going to draw to the screen, do it.
                    if (SadConsole.Settings.DoFinalDraw)
                    {
                        Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, SadConsole.Host.Settings.MonoGameScreenBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                        Global.SharedSpriteBatch.Draw(Global.RenderOutput, SadRogue.Primitives.SadRogueRectangleExtensions.ToMonoRectangle(SadConsole.Settings.Rendering.RenderRect), Color.White);
                        Global.SharedSpriteBatch.End();
                    }

#if NOESIS
                    // GUI  TODO: Should I move this before rendertarget NULL?
                    NoesisManager.noesisGUIWrapper.Render();
#endif
                }
            }

            /// <summary>
            /// Updates the SadConsole game objects and handles input. Only runs when <see cref="SadConsole.Settings.DoUpdate"/> is true.
            /// </summary>
            /// <param name="gameTime"></param>
            public override void Update(GameTime gameTime)
            {
                if (SadConsole.Settings.DoUpdate)
                {
                    var game = (Game)Game;

#if NOESIS
                    bool blockInput = false;
                    NoesisManager.noesisGUIWrapper.UpdateInput(gameTime, isWindowActive: this.Game.IsActive);

                    blockInput = Global.BlockSadConsoleInput
                        || NoesisManager.noesisGUIWrapper.Input.ConsumedKeyboardKeys.Count != 0
                        || NoesisManager.noesisGUIWrapper.Input.ConsumedMouseButtons.Count != 0
                        || NoesisManager.noesisGUIWrapper.Input.ConsumedMouseDeltaWheel != 0;

                    SadConsole.GameHost.Instance.UpdateFrameDelta = gameTime.ElapsedGameTime;

                    if (Game.IsActive && !blockInput)
                    {
#else
                    SadConsole.GameHost.Instance.UpdateFrameDelta = gameTime.ElapsedGameTime;
                    Global.UpdateLoopGameTime = gameTime;

                    if (Game.IsActive && !Global.BlockSadConsoleInput)
                    {
#endif
                        if (SadConsole.Settings.Input.DoKeyboard)
                        {
                            SadConsole.GameHost.Instance.Keyboard.Update(SadConsole.GameHost.Instance.UpdateFrameDelta);

                            if (SadConsole.GameHost.Instance.FocusedScreenObjects.ScreenObject != null && SadConsole.GameHost.Instance.FocusedScreenObjects.ScreenObject.UseKeyboard)
                            {
                                SadConsole.GameHost.Instance.FocusedScreenObjects.ScreenObject.ProcessKeyboard(SadConsole.GameHost.Instance.Keyboard);
                            }

                        }

                        if (SadConsole.Settings.Input.DoMouse)
                        {
                            SadConsole.GameHost.Instance.Mouse.Update(SadConsole.GameHost.Instance.UpdateFrameDelta);
                            SadConsole.GameHost.Instance.Mouse.Process();
                        }
                    }

                    SadConsole.GameHost.Instance.Screen?.Update(SadConsole.GameHost.Instance.UpdateFrameDelta);

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameUpdate();

#if NOESIS
                    // GUI
                    NoesisManager.noesisGUIWrapper.Update(gameTime);
#endif
                }
            }
        }
    }
}
