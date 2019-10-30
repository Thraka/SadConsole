using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole
{
    public class Game: GameHost, IDisposable
    {
        public static Game Instance { get; internal set; }

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        
        public void Create(int width, int height, Action onInit)
        {
            (ScreenWidth, ScreenHeight) = (width, height);

            Instance = this;

            SadConsole.Global.Screen = new Console(width, height);

            onInit?.Invoke();
        }

        public void Run()
        {
            bool run = true;
            System.Console.Clear();
            System.Console.CursorVisible = false;

            while (run)
            {
                Global.Screen.Draw();
                Global.Screen.Renderer.Render();
            }
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
