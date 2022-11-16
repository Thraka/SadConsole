using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Debug.MonoGame.ImGuiTypes;

public static class ConvertHorizontalAlignment
{
    public static string[] Names;

    static ConvertHorizontalAlignment()
    {
        List<string> names = new List<string>();

        foreach(var item in Enum.GetValues<HorizontalAlignment>())
            names.Add(Enum.GetName(item));

        Names = names.ToArray();
    }
}
