using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

/// <summary>
/// Handles tool modes for ImGui lists.
/// </summary>
public class ToolMode : ITitle
{
    public static ToolMode DrawMode = new(Modes.Draw);
    public static ToolMode EmptyMode = new(Modes.Empty);
    public static ToolMode ObjectsMode = new(Modes.Objects);
    public static ToolMode ZonesMode = new(Modes.Zones);

    public Modes Mode;

    public string Title => Mode.ToString();

    public ToolMode(Modes mode)
    {
        Mode = mode;
    }

    public enum Modes
    {
        Draw,
        Empty,
        Objects,
        Zones
    }
}
