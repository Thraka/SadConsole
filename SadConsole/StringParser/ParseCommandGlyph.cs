using System.Globalization;

namespace SadConsole.StringParser;

/// <summary>
/// Prints a glyph.
/// </summary>
public sealed class ParseCommandSetGlyph : ParseCommandBase
{
    private int _counter;

    /// <summary>
    /// The glyph to set.
    /// </summary>
    public char Glyph { get; set; }

    /// <summary>
    /// Uses a random glyph.
    /// </summary>
    public bool RandomGlyph { get; set; }

    /// <summary>
    /// The minimum glyph to use with the random glyph.
    /// </summary>
    public int RandomGlyphMin { get; set; }

    /// <summary>
    /// The maximumglyph to use with the random glyph.
    /// </summary>
    public int RandomGlyphMax { get; set; }

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    /// <param name="parameters">The string to parse for parameters.</param>
    public ParseCommandSetGlyph(string parameters)
    {
        string[] parts = parameters.Split(new char[] { ':' }, 3);

        // Random glyph requested
        if (parts.Length == 3)
        {
            RandomGlyph = true;
            if (parts[0] == "*")
                _counter = -1;
            else
                _counter = int.Parse(parts[0], CultureInfo.InvariantCulture);

            RandomGlyphMin = int.Parse(parts[1], CultureInfo.InvariantCulture);
            RandomGlyphMax = int.Parse(parts[2], CultureInfo.InvariantCulture);
        }
        // Count and glyph type provided
        else if (parts.Length == 2)
        {
            if (parts[1] == "*")
                _counter = -1;
            else
                _counter = int.Parse(parts[1], CultureInfo.InvariantCulture);

            Glyph = (char)int.Parse(parts[0], CultureInfo.InvariantCulture);
        }
        else
        {
            _counter = 1;
            Glyph = (char)int.Parse(parts[0], CultureInfo.InvariantCulture);
        }

        // No exceptions, set the type
        CommandType = CommandTypes.Glyph;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex,
        ICellSurface? surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        if (RandomGlyph)
            glyphState.GlyphCharacter = (char)SadConsole.GameHost.Instance.Random.Next(RandomGlyphMin, RandomGlyphMax);
        else
            glyphState.GlyphCharacter = Glyph;

        if (_counter != -1)
            _counter--;

        if (_counter == 0)
            commandStack.RemoveSafe(this);
    }
}
