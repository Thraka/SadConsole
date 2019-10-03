using System;
using System.Collections.Generic;
using System.Text;
using Mindmagma.Curses;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole
{
    public class Game: GameHost, IDisposable
    {
        public static Game Instance { get; internal set; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        
        private IntPtr _screen;

        public void Create(int width, int height, Action onInit)
        {
            (ScreenWidth, ScreenHeight) = (width, height);

            _screen = NCurses.InitScreen();

            Instance = this;

            SadConsole.Global.Screen = new Console(width, height);

            onInit?.Invoke();
        }

        public void Run()
        {
            NCurses.NoDelay(_screen, true);
            NCurses.NoEcho();
            NCurses.ResizeTerminal(ScreenHeight, ScreenWidth);
            //NCurses.MoveAddString(0, 0, "Click a button or press any key to exit.");

            bool run = true;

            while (run)
            {
                switch (NCurses.GetChar())
                {
                    case -1:
                        break;
                    default:
                        run = false;
                        break;
                }

                Global.Screen.Draw();

                foreach (var item in _drawCalls)
                {
                    item.Draw();
                }

                NCurses.Refresh();
            }

            NCurses.EndWin();
        }

        public void AddDrawCallSurface(CellSurface surface, Point location)
        {
            _drawCalls.Enqueue(new DrawCalls.DrawCallSurface(surface, location));
        }


        public override IRenderSurface CreateSurface(int width, int height)
        {
            var surface = new ConsoleRenderSurface();
            surface._window = NCurses.NewWindow(width, height, 0, 0);

            return surface;
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


    }
}
