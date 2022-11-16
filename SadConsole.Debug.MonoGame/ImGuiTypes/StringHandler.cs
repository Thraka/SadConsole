using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace SadConsole.ImGuiSystem;

public static partial class ImGuiExt
{
    public static unsafe string InputText(string label, string value)
    {
        var str = value ?? "";

        ImGui.InputText(label, ref str, 1024, ImGuiInputTextFlags.CallbackAlways, data =>
        {
            str = Marshal.PtrToStringUTF8((IntPtr)data->Buf) ?? string.Empty;
            return 1;
        });

        return str;
    }
}
