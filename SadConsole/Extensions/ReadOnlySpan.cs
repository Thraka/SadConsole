using System;

namespace SadConsole.Extensions;

public static class ReadOnlySpan
{
    public static bool Next(this System.ReadOnlySpan<char> span, char value, out int index)
    {
        index = span.IndexOf(value);

        return index != -1;
    }
}
#nullable restore
