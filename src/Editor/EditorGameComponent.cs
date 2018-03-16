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
        public static NoesisWrapper noesisGUIWrapper;
        private TimeSpan lastUpdateTotalGameTime;

        internal EditorGameComponent(Game game) : base(game)
        {
            DrawOrder = 5;
            UpdateOrder = 5;

            CreateNoesisGUI();
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var font in SadConsole.Global.Fonts.Values)
                font.Image.Dispose();


        }

        public static void DestroyGUI()
        {
            if (noesisGUIWrapper != null)
            {
                noesisGUIWrapper.Dispose();
                noesisGUIWrapper = null;
            }
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();


        }

private void CreateNoesisGUI()
{
    var rootPath = Environment.CurrentDirectory;//Path.Combine(Environment.CurrentDirectory, "Data");
    var providerManager = new NoesisProviderManager(
        new FolderXamlProvider(rootPath),
        new FolderFontProvider(rootPath),
        new FolderTextureProvider(rootPath, this.GraphicsDevice));

    var config = new NoesisConfig(
        this.Game.Window,
        SadConsole.Global.GraphicsDeviceManager,
        providerManager,
        rootXamlFilePath: "Views/Root.xaml",
        // uncomment this line to use theme file
        themeXamlFilePath: "Themes/NocturnalStyle.xaml",
        currentTotalGameTime: this.lastUpdateTotalGameTime);

    config.SetupInputFromWindows();

    noesisGUIWrapper = new NoesisWrapper(config);
    noesisGUIWrapper.ControlTreeRoot.DataContext = new Editor.ViewModels.MainViewModel();
}

        public override void Draw(GameTime gameTime)
        {
            if (Settings.DoDraw)
            {
                // GUI
                noesisGUIWrapper.PreRender();
                
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
                noesisGUIWrapper.Render();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Settings.DoUpdate)
            {
                // GUI
                this.lastUpdateTotalGameTime = gameTime.TotalGameTime;
                noesisGUIWrapper.UpdateInput(gameTime, isWindowActive: this.Game.IsActive);

                bool blockInput = Editor.Globals.BlockSadConsoleInput
                    || noesisGUIWrapper.Input.ConsumedKeyboardKeys.Count != 0
                    || noesisGUIWrapper.Input.ConsumedMouseButtons.Count != 0
                    || noesisGUIWrapper.Input.ConsumedMouseDeltaWheel != 0;

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
                noesisGUIWrapper.Update(gameTime);
            }
        }
    }
}
