using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Tools;

internal static class CommonToolSettings
{
    public static ColoredGlyph Tip { get; set; }

    static CommonToolSettings()
    {
        Tip = new ColoredGlyph();
    }
}
