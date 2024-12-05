using System;
using System.Collections.Generic;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using Hexa.NET.ImGui;
using System.Numerics;

namespace SadConsole.Debug;

public static partial class Debugger
{
    public static class Settings
    {
        public static Vector4 Color_Labels = Color.AnsiCyanBright.ToVector4();
        public static Vector4 Color_FocusedObj = Color.AnsiCyanBright.ToVector4();
    }
}
