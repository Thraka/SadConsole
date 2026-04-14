using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Renderers;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadBBSClient;

internal class MessageWindow: ScreenSurface
{
    public MessageWindow(string message) : base(GetWidth(message, 18), 5)
    {
        Position = new(AppSettings.Instance.Width / 2 - Width / 2, AppSettings.Instance.Height / 2 - Height / 2);

        Surface.DefaultBackground = Color.Black;
        Surface.Clear();

        Surface.DrawBox(Surface.Area, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black)));

        ICellSurface subSurface = Surface.GetSubSurface(Surface.Area.Expand(-2, -2));
        subSurface.Print(0, 0, message);

        ControlHost controlsHost = new();
        controlsHost.ClearOnAdded = false;
        SadComponents.Add(controlsHost);

        Button button = new(" Close ");
        
        button.Position = new(Width - 12, Height - 1);
        button.Click += (s, e) => IsVisible = false;
        controlsHost.Add(button);

        button.IsFocused = true;
    }

    private static int GetWidth(string value, int minimum = -1)
    {
        int index = value.IndexOf('\n');
        if (index == -1)
            index = value.Length + 4;
        else
            index++;

        return Math.Max(index, minimum);
    }
}
