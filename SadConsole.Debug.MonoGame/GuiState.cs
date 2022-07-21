using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Myra.Graphics2D.UI;

namespace SadConsole.Debug.MonoGame
{
    static class GuiState
    {
        public static event EventHandler ShowSadConsoleRenderingChanged;

        public static Widget GuiFinalOutputWindow;


        public static bool ShowSurfacePreview = true;
        public static bool ShowFinalPreview = true;
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

    public class ScreenObjectState
    {
        static int _identifier;

        public int Identifier;
        public bool Found;
        public IScreenObject Object;

        public int PositionX;
        public int PositionY;
        public int Width;
        public int Height;

        public Vector4 Tint;

        public bool IsScreenSurface;

        public int ComponentsSelectedItem;
        public string[] Components;

        public static ScreenObjectState Create(IScreenObject obj)
        {
            var state = new ScreenObjectState()
            {
                Object = obj,
                Identifier = _identifier++,
                Found = true
            };

            
            state.Refresh();
            state.RefreshComponents();
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
                Tint = surface.Tint.ToVector4();
            }
            else
                IsScreenSurface = false;
        }

        public void RefreshComponents()
        {
            if (Object.SadComponents.Count == 0)
                Components = Array.Empty<string>();
            else
            {
                Components = new string[Object.SadComponents.Count];
                for (int i = 0; i < Components.Length; i++)
                    Components[i] = Object.SadComponents[i].GetDebuggerDisplayValue();
            }
        }
    }
}
