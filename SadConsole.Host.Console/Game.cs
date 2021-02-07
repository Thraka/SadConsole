using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole
{
    public class Game : GameHost
    {
        private int _preFullScreenWidth;
        private int _preFullScreenHeight;
        private bool _handleResizeNone;

        public new static Game Instance
        {
            get => (Game)GameHost.Instance;
            protected set => GameHost.Instance = value;
        }

        internal string _font;


        private Game() { }

        public static void Create(int cellCountX, int cellCountY)
        {
            var game = new Game();
            game.ScreenCellsX = cellCountX;
            game.ScreenCellsY = cellCountY;

            Instance = game;
            game.Initialize();
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Game()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        private void Initialize()
        {
            if (string.IsNullOrEmpty(_font))
                LoadEmbeddedFont();
            else
                Global.DefaultFont = LoadFont(_font);

            System.Console.SetWindowSize(ScreenCellsX, ScreenCellsY);
            System.Console.SetBufferSize(ScreenCellsX, ScreenCellsY);

            WindowSize = new Point(ScreenCellsX, ScreenCellsY);

            //SadConsole.Settings.Rendering.RenderWidth = WindowSize.X;
            //SadConsole.Settings.Rendering.RenderHeight = WindowSize.Y;
            //ResetRendering();

            //_keyboard = new Keyboard();
            //_mouse = new Mouse();
            //Host.Global.UpdateTimer = new SFML.System.Clock();
            //Host.Global.DrawTimer = new SFML.System.Clock();

            // Create the default console.
            SadConsole.Global.Screen = new Console(ScreenCellsX, ScreenCellsY);
        }

        public override void Run()
        {
            bool run = true;

            System.Console.CursorVisible = false;

            OnStart?.Invoke();

            while (run)
            {
                //SadConsole.Host.Global.GraphicsDevice.Clear(SadConsole.Settings.ClearColor.ToSFML());
                System.Console.Clear();

                // Update game loop part
                if (SadConsole.Settings.DoUpdate)
                {
                    //SadConsole.Global.UpdateFrameDelta = TimeSpan.FromSeconds(Host.Global.UpdateTimer.ElapsedTime.AsSeconds());

                    //if (SadConsole.Host.Global.GraphicsDevice.HasFocus())
                    //{
                    //    if (SadConsole.Settings.Input.DoKeyboard)
                    //    {
                    //        SadConsole.Global.Keyboard.Update(SadConsole.Global.UpdateFrameDelta);

                    //        if (SadConsole.Global.FocusedConsoles.Console != null && SadConsole.Global.FocusedConsoles.Console.UseKeyboard)
                    //        {
                    //            SadConsole.Global.FocusedConsoles.Console.ProcessKeyboard(SadConsole.Global.Keyboard);
                    //        }

                    //    }

                    //    if (SadConsole.Settings.Input.DoMouse)
                    //    {
                    //        SadConsole.Global.Mouse.Update(SadConsole.Global.UpdateFrameDelta);
                    //        SadConsole.Global.Mouse.Process();
                    //    }
                    //}

                    SadConsole.Global.Screen?.Update();

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameUpdate();

                    //Host.Global.UpdateTimer.Restart();
                }

                // Draw game loop part
                if (SadConsole.Settings.DoDraw)
                {
                    //SadConsole.Global.DrawFrameDelta = TimeSpan.FromSeconds(Host.Global.DrawTimer.ElapsedTime.AsSeconds());

                    // Clear draw calls for next run
                    SadConsole.Game.Instance.DrawCalls.Clear();

                    // Make sure all items in the screen are drawn. (Build a list of draw calls)
                    SadConsole.Global.Screen?.Draw();

                    ((SadConsole.Game)SadConsole.Game.Instance).InvokeFrameDraw();

                    // Render to the global output texture
                    //Host.Global.RenderOutput.Clear(SadConsole.Settings.ClearColor.ToSFML());

                    // Render each draw call
                    //Host.Global.SharedSpriteBatch.Reset(Host.Global.RenderOutput, RenderStates.Default, Transform.Identity);

                    foreach (DrawCalls.IDrawCall call in SadConsole.Game.Instance.DrawCalls)
                        call.Draw();

                    //Host.Global.SharedSpriteBatch.End();
                    //Host.Global.RenderOutput.Display();

                    // If we're going to draw to the screen, do it.
                    if (SadConsole.Settings.DoFinalDraw)
                    {
                        //Host.Global.SharedSpriteBatch.Reset(Host.Global.GraphicsDevice, RenderStates.Default, Transform.Identity);
                        //Host.Global.SharedSpriteBatch.DrawQuad(Settings.Rendering.RenderRect.ToSFML(), new IntRect(0, 0, (int)Host.Global.RenderOutput.Size.X, (int)Host.Global.RenderOutput.Size.Y), SFML.Graphics.Color.White, Host.Global.RenderOutput.Texture);
                        //Host.Global.SharedSpriteBatch.End();
                    }
                }

                //SadConsole.Host.Global.GraphicsDevice.Display();
                //SadConsole.Host.Global.GraphicsDevice.DispatchEvents();

                //Host.Global.DrawTimer.Restart();
            }

            OnEnd?.Invoke();
        }

        public override ITexture GetTexture(string resourcePath) =>
            new FauxTexture();

        public override ITexture GetTexture(Stream textureStream) =>
            new FauxTexture();

        public override IRenderer GetDefaultRenderer(IScreenSurface screenObject) =>
            new Renderers.ConsoleRenderer();

        public override IKeyboardState GetKeyboardState()
        {
            throw new NotImplementedException();
        }

        public override IMouseState GetMouseState()
        {
            throw new NotImplementedException();
        }

        internal void InvokeFrameDraw() =>
            OnFrameDraw();

        internal void InvokeFrameUpdate() =>
            OnFrameUpdate();
    }
}
