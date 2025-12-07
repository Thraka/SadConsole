using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.Editor.Serialization;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal static class SharedToolSettings
{
    public static ColoredGlyph Tip { get; set; }

    static SharedToolSettings()
    {
        Tip = new ColoredGlyph() { Glyph = 1 };
    }
}
