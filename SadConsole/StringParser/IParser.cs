using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.StringParser;

/// <summary>
/// Describes a parser
/// </summary>
public interface IParser
{
    ColoredString Parse(ReadOnlySpan<char> value, int surfaceIndex = -1, ICellSurface surface = null, ParseCommandStacks initialBehaviors = null);
}
