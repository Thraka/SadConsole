using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor
{
    public class Game : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;

        protected override void Initialize()
        {
            SizeChanged += Game_SizeChanged;

            // must be initialized. required by Content loading and rendering (will add itself to the Services)
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            // wpf and keyboard need reference to the host control in order to receive input
            // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();
            // content loading now possible

            Global.GraphicsDevice = GraphicsDevice;
            //Global.GraphicsDeviceManager = GraphicsDeviceManager;
            Global.SpriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
            Global.FontDefault = Global.LoadFont("Fonts\\Cheepicus12.font").GetFont(Font.FontSizes.One);


            var con = new SadConsole.Console(10, 10);
            Global.CurrentScreen = con;
            con.FillWithRandomGarbage();
        }

        private void Game_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Global.RenderRect = new Rectangle(0, 0, (int)e.NewSize.Width, (int)e.NewSize.Height);
            Global.RenderOutput = new RenderTarget2D(GraphicsDevice, Global.RenderRect.Width, Global.RenderRect.Height);
            Global.RenderWidth = Global.RenderRect.Width;
            Global.RenderHeight = Global.RenderRect.Height;
        }

        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();
        }

        protected override void Draw(GameTime time)
        {
            var wpfRenderTarget = (RenderTarget2D)GraphicsDevice.GetRenderTargets()[0].RenderTarget;
            Global.GameTimeRender = time;
            Global.GameTimeElapsedRender = time.ElapsedGameTime.TotalSeconds;

            // Clear draw calls for next run
            Global.DrawCalls.Clear();

            var con = (SadConsole.Console)Global.CurrentScreen;
            con.Print(0, 0, $"{Global.RenderRect} && {this.RenderSize} && {this.Width},{this.Height} && {GraphicsDevice.PresentationParameters.BackBufferWidth},{GraphicsDevice.PresentationParameters.BackBufferHeight} ", Color.White, Color.Black);

            // Make sure all items in the screen are drawn. (Build a list of draw calls)
            Global.CurrentScreen?.Draw(time.ElapsedGameTime);

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
            GraphicsDevice.SetRenderTarget(wpfRenderTarget);
            GraphicsDevice.Clear(Settings.ClearColor);

            // If we're going to draw to the screen, do it.
            if (Settings.DoFinalDraw)
            {
                Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                Global.SpriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);
                Global.SpriteBatch.End();
            }


        }
    }
}