using System;

namespace SadConsole.StringParser;

/// <summary>
/// Describes a parser
/// </summary>
public interface IParser
{
    /// <summary>
    /// Generates a colored string from a string of characters.
    /// </summary>
    /// <param name="value">The characters to process.</param>
    /// <param name="surfaceIndex">The index on the backing surface, if it applies.</param>
    /// <param name="surface">The backing surface the parser is interacting with, if it applies.</param>
    /// <param name="initialBehaviors">A set of known behaviors to apply to the parser.</param>
    /// <returns></returns>
    ColoredString Parse(ReadOnlySpan<char> value, int surfaceIndex = -1, ICellSurface? surface = null, ParseCommandStacks? initialBehaviors = null);
}
