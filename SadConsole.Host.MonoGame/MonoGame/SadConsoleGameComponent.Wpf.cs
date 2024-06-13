﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace SadConsole.Host;

/// <summary>
/// A game component that handles updating, input, and rendering of SadConsole.
/// </summary>
public class SadConsoleGameComponent : WpfDrawableGameComponent
{
    internal SadConsoleGameComponent(WpfGame game) : base(game)
    {
        DrawOrder = 5;
        UpdateOrder = 5;
    }

    /// <summary>
    /// Draws the SadConsole frame through draw calls when <see cref="SadConsole.Settings.DoDraw"/> is true.
    /// </summary>
    /// <param name="gameTime">Time between drawing frames.</param>
    public override void Draw(GameTime gameTime)
    {
        if (((SadConsole.Host.Game)Game).ResetRenderingNextFrame)
        {
            ((SadConsole.Host.Game)Game).ResetRenderingNextFrame = false;
            ((SadConsole.Host.Game)Game).ResetRendering();
        }

        SadConsole.GameHost.Instance.DrawFrameDelta = gameTime.ElapsedGameTime;
        Global.RenderLoopGameTime = gameTime;

        if (SadConsole.Settings.DoDraw)
        {
            Host.Game game = (Host.Game)Game;

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
            {
                call.Draw();
            }

            Global.SharedSpriteBatch.End();
            Global.ResetGraphicsDevice();

            // If we're going to draw to the screen, do it.
            if (SadConsole.Settings.DoFinalDraw)
            {
                Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, SadConsole.Host.Settings.MonoGameScreenBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                Global.SharedSpriteBatch.Draw(Global.RenderOutput, SadRogue.Primitives.SadRogueRectangleExtensions.ToMonoRectangle(SadConsole.Settings.Rendering.RenderRect), Color.White);
                Global.SharedSpriteBatch.End();
            }
        }
    }

    /// <summary>
    /// Updates the SadConsole game objects and handles input. Only runs when <see cref="SadConsole.Settings.DoUpdate"/> is true.
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Update(GameTime gameTime)
    {
        SadConsole.GameHost.Instance.UpdateFrameDelta = gameTime.ElapsedGameTime;
        Global.UpdateLoopGameTime = gameTime;

        if (SadConsole.Settings.DoUpdate)
        {
            // Process any pre-Screen logic components
            foreach (SadConsole.Components.RootComponent item in SadConsole.Game.Instance.RootComponents)
                item.Run(GameHost.Instance.UpdateFrameDelta);

            var game = (Game)Game;

            if (Game.IsActive)
            {
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
        }
    }
}
