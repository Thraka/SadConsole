using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Input;
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

        //protected override void OnClientSizeChanged(EventArgs e)
        //{
        //    base.OnClientSizeChanged(e);

        //    if (SwapChainRenderTarget != null)
        //    {
        //        Global.OriginalRenderTarget = SwapChainRenderTarget;
        //        Global.ResetRendering();
        //    }
        //}

        protected override void Initialize()
        {
            base.Initialize();

            //Global.OriginalRenderTarget = SwapChainRenderTarget;

            //Global.GraphicsDevice = GraphicsDevice;
            //Global.SpriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
            //Global.FontDefault = Global.LoadFont(SadConsole.Game.WpfFont).GetFont(SadConsole.Font.FontSizes.One);

            //Global.WindowWidth = Global.RenderWidth = (Global.FontDefault.Size.X * SadConsole.Game.WpfConsoleWidth);
            //Global.WindowHeight = Global.RenderHeight = (Global.FontDefault.Size.Y * SadConsole.Game.WpfConsoleHeight);
            //Global.ResetRendering();

            //// Tell the main engine we're ready
            //SadConsole.Game.OnInitialize?.Invoke();

            //// After we've init, clear the graphics device so everything is ready to start
            //Global.GraphicsDevice.SetRenderTarget(Global.OriginalRenderTarget);
        }

        protected override void Draw()
        {
            base.Draw();

            //if (Settings.DoDraw)
            //{
            //    Global.GameTimeRender = gameTime;
            //    Global.GameTimeElapsedRender = gameTime.ElapsedGameTime.TotalSeconds;

            //    // Clear draw calls for next run
            //    Global.DrawCalls.Clear();

            //    // Make sure all items in the screen are drawn. (Build a list of draw calls)
            //    Global.CurrentScreen?.Draw(gameTime.ElapsedGameTime);

            //    // Draw the editor brush on its surface to render later
            //    if (DataContext.Instance.IsEditMode)
            //        if (DataContext.Instance.SelectedTool != null)
            //            DataContext.Instance.SelectedTool.Brush.Draw(gameTime.ElapsedGameTime);

            //    SadConsole.Game.OnDraw?.Invoke(gameTime);

            //    // Render to the global output texture
            //    GraphicsDevice.SetRenderTarget(Global.RenderOutput);

            //    // Inner clear color
            //    if (DataContext.Instance.IsEditMode)
            //    {
            //        if (Config.Instance.DrawEditClearColorInner)
            //            GraphicsDevice.Clear(Config.Instance.EditClearColorInner);
            //        else
            //            GraphicsDevice.Clear(Settings.ClearColor);
            //    }
            //    else
            //        GraphicsDevice.Clear(Settings.ClearColor);

            //    // Render each draw call
            //    Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
            //    foreach (var call in Global.DrawCalls)
            //    {
            //        call.Draw();
            //    }
            //    Global.SpriteBatch.End();
            //    GraphicsDevice.SetRenderTarget(Global.OriginalRenderTarget);
                
            //    // If we're going to draw to the screen, do it.
            //    if (Settings.DoFinalDraw)
            //    {
            //        if (DataContext.Instance.IsEditMode)
            //        {
            //            if (Config.Instance.DrawEditClearColorOuter)
            //                GraphicsDevice.Clear(Config.Instance.EditClearColorOuter);
            //            else
            //                GraphicsDevice.Clear(Settings.ClearColor);
            //        }
            //        else
            //            GraphicsDevice.Clear(Settings.ClearColor);


            //        Editor.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                    
            //        // Draw screen border
            //        if (DataContext.Instance.IsEditMode)
            //        {
            //            var rect1 = Global.RenderRect;
            //            var rect2 = Global.RenderRect;
            //            rect1.Inflate(1, 1);
            //            rect2.Inflate(2, 2);

            //            if (Config.Instance.DrawOuterBorder)
            //            {
            //                Editor.spriteBatch.Draw(Global.FontDefault.FontImage, rect2, Global.FontDefault.SolidGlyphRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            //                Editor.spriteBatch.Draw(Global.FontDefault.FontImage, rect1, Global.FontDefault.SolidGlyphRectangle, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            //            }
            //        }

            //        Editor.spriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);

            //        // Draw the editor brush to the screen
            //        if (DataContext.Instance.IsEditMode)
            //            if (DataContext.Instance.SelectedTool != null)
            //                DataContext.Instance.SelectedTool.Brush.EditorDraw(Editor.spriteBatch);

            //        Editor.spriteBatch.End();
            //    }
            //}
        }
        
        protected override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            base.Update(gameTime);

            //if (Settings.DoUpdate && !DataContext.Instance.IsEditMode)
            //{
            //    Global.GameTimeUpdate = gameTime;
            //    Global.GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;

            //    if (!DataContext.Instance.PauseEditMode)
            //    {
            //        if (Settings.Input.DoKeyboard)
            //        {
            //            Global.KeyboardState.Update(gameTime);
            //            Global.KeyboardState.Process();
            //        }

            //        if (Settings.Input.DoMouse)
            //        {
            //            Global.MouseState.Update(gameTime);
            //            Global.MouseState.Process();
            //        }
            //    }

            //    Global.CurrentScreen?.Update(gameTime.ElapsedGameTime);

            //    SadConsole.Game.OnUpdate?.Invoke(gameTime);
            //}
            //else if (DataContext.Instance.IsEditMode && !DataContext.Instance.PauseEditMode)
            //{
            //    DataContext.Instance.Update(gameTime);
            //}
        }
    }
}
