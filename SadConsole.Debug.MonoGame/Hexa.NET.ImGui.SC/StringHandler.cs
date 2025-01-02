using System;
using System.Runtime.InteropServices;

namespace Hexa.NET.ImGui.SC;

public static partial class ImGuiSC
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
