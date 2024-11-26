using System;
using System.Collections.Generic;
using System.Numerics;
using SadConsole.Renderers;

namespace SadConsole.Debug;

static class GuiState
{
    public static event EventHandler ShowSadConsoleRenderingChanged;

    public static FinalOutputWindow GuiFinalOutputWindow;


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

    public bool IsVisible;
    public bool IsEnabled;

    public bool IsScreenSurface;
    public bool IsWindow;

    public int ComponentsSelectedItem;
    public string[] Components;

    public ScreenSurfaceState SurfaceState = new ScreenSurfaceState();
    public WindowConsoleState WindowState = new WindowConsoleState();

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

        IsVisible = Object.IsVisible;
        IsEnabled = Object.IsEnabled;

        IsScreenSurface = Object is IScreenSurface;
        IsWindow = Object is UI.Window;

        RefreshComponents();

        if (IsScreenSurface) SurfaceState.Refresh(Object as IScreenSurface);
        if (IsWindow) WindowState.Refresh(Object as UI.Window);
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

    public class ScreenSurfaceState
    {
        public Vector4 Tint;
        public Vector4 View;
        public int Width;
        public int Height;

        public int RenderStepSelectedItem;
        public string[] RenderStepsNames;
        public IRenderStepTexture[] RenderSteps;

        public void Refresh(IScreenSurface surface)
        {
            Tint = surface.Tint.ToVector4();
            Width = surface.Surface.Width;
            Height = surface.Surface.Height;

            RefreshRendersteps(surface);
        }

        private void RefreshRendersteps(IScreenSurface surface)
        {
            RenderStepsNames = Array.Empty<string>();
            RenderSteps = Array.Empty<IRenderStepTexture>();

            if (surface.Renderer!.Steps.Count != 0)
            {
                List<string> names = new List<string>();
                List<IRenderStepTexture> steps = new List<IRenderStepTexture>();

                names.Add("Final");
                steps.Add(null);

                foreach (var step in surface.Renderer!.Steps)
                {
                    if (step is IRenderStepTexture stepTexture)
                    {
                        names.Add(step.GetDebuggerDisplayValue());
                        steps.Add(stepTexture);
                    }
                }

                RenderStepsNames = names.ToArray();
                RenderSteps = steps.ToArray();

                return;
            }
        }
    }

    public class WindowConsoleState
    {
        public int TitleAlignment;

        public void Refresh(UI.Window console)
        {
            TitleAlignment = (int)console.TitleAlignment;
        }
    }
}
