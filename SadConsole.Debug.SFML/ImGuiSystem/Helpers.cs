using System.Text;

namespace SadConsole.ImGuiSystem;

public static partial class Helpers
{
    internal unsafe static string StringFromPtr(byte* ptr)
    {
        int i;
        for (i = 0; ptr[i] != 0; i++)
        {
        }

        return Encoding.UTF8.GetString(ptr, i);
    }
}
