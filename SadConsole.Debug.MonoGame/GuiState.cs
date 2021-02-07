using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Debug.MonoGame
{
    static class GuiState
    {
        public static event EventHandler ShowSadConsoleRenderingChanged;

        //private static bool _oldShowSadConsoleRendering;

        public static bool ShowSurfacePreview = true;
        public static bool ShutdownRequested;
        public static bool ShowSadConsoleRendering = false;

        public static IScreenObject _selectedScreenObject;
        public static ScreenObjectState _selectedScreenObjectState;
        public static Dictionary<IScreenObject, ScreenObjectState> ScreenObjectUniques = new Dictionary<IScreenObject, ScreenObjectState>();

        //public static void Update()
        //{
        //    if (_oldShowSadConsoleRendering != ShowSadConsoleRendering)
        //    {
        //        _oldShowSadConsoleRendering = ShowSadConsoleRendering;
        //        ShowSadConsoleRenderingChanged?.Invoke(null, EventArgs.Empty);
        //    }    
        //}

        public static void RaiseShowSadConsoleRenderingChanged() =>
            ShowSadConsoleRenderingChanged?.Invoke(null, EventArgs.Empty);
    }

    class ScreenObjectState
    {
        static int _identifier;

        public int Identifier;
        public bool Found;
        public IScreenObject Object;

        public int PositionX;
        public int PositionY;
        public int Width;
        public int Height;

        public bool IsScreenSurface;

        public static ScreenObjectState Create(IScreenObject obj)
        {
            var state = new ScreenObjectState()
            {
                Object = obj,
                Identifier = _identifier++,
                Found = true
            };

            state.Refresh();

            return state;
        }

        public void Refresh()
        {
            PositionX = Object.Position.X;
            PositionY = Object.Position.Y;

            if (Object is IScreenSurface surface)
            {
                IsScreenSurface = true;
                Width = surface.Surface.Width;
                Height = surface.Surface.Height;
            }
            else
                IsScreenSurface = false;
        }
    }
}
