using System;
using System.Collections.Generic;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using Hexa.NET.ImGui;
using System.Numerics;

namespace SadConsole.Debug;

public static partial class Debugger
{
    /// <summary>
    /// The settings used by the debugger.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The color of the focused object.
        /// </summary>
        public static Vector4 Color_FocusedObj = Color.AnsiCyanBright.ToVector4();
    }
}
