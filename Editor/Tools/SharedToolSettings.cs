using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Tools;

internal static class SharedToolSettings
{
    public static ColoredGlyph Tip { get; set; }

    static SharedToolSettings()
    {
        Tip = new ColoredGlyph() { Glyph = 1 };
    }

    // public static void DrawSharedToolSettings
}
