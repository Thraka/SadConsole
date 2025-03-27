using System;
using System.Collections.Generic;
using System.Numerics;
using SadConsole.Renderers;

namespace SadConsole.Debug;

public class ScreenObjectState
{
    internal static int _identifierCounter;

    public int Identifier;
    public bool Found;
    public IScreenObject Object;
    public string ObjectName;

    public int PositionX;
    public int PositionY;

    public bool IsVisible;
    public bool IsEnabled;

    public bool IsScreenSurface;
    public bool IsWindow;

    public int ComponentsSelectedItem;
    public string[] Components;

    public ScreenObjectState[] Children;

    public ScreenSurfaceState SurfaceState = new ScreenSurfaceState();
    public WindowConsoleState WindowState = new WindowConsoleState();

    public static ScreenObjectState Create(IScreenObject obj)
    {
        var state = new ScreenObjectState()
        {
            Object = obj,
            Identifier = _identifierCounter++,
            Found = true
        };

        GuiState.ScreenObjectUniques.Add(obj, state);

        state.Refresh();
        state.RefreshChildren();
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
        ObjectName = Object.GetDebuggerDisplayValue();

        RefreshComponents();

        if (IsScreenSurface) SurfaceState.Refresh((IScreenSurface)Object);
        if (IsWindow) WindowState.Refresh((UI.Window)Object);
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

    public void RefreshChildren()
    {
        if (Object.Children.Count == 0)
            Children = [];

        else
        {
            List<ScreenObjectState> children = new(Object.Children.Count);

            foreach (IScreenObject child in Object.Children)
                children.Add(Create(child));

            Children = children.ToArray();
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
                List<string> names = [];
                List<IRenderStepTexture> steps = [];

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
