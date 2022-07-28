using System;
using System.Globalization;

namespace SadConsole.StringParser;

/// <summary>
/// Sets the mirror of a glyph.
/// </summary>
public sealed class ParseCommandMirror : ParseCommandBase
{
    private int _counter;

    /// <summary>
    /// The mirror mode.
    /// </summary>
    public Mirror Mirror;

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    /// <param name="parameters">The string to parse for parameters.</param>
    public ParseCommandMirror(string parameters)
    {
        var badCommandException = new ArgumentException("command is invalid for mirror: " + parameters);

        string[] paramArray = parameters.Split(':');

        if (paramArray.Length == 2)
            _counter = int.Parse(paramArray[1], CultureInfo.InvariantCulture);
        else
            _counter = -1;

        if (Enum.TryParse(paramArray[0], out Mirror))
            CommandType = CommandTypes.Mirror;
        else
            throw badCommandException;
    }

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    public ParseCommandMirror()
    {

    }

    /// <inheritdoc />
    public override void Build(ref ColoredString.ColoredGlyphEffect glyphState, ColoredString.ColoredGlyphEffect[] glyphString, int surfaceIndex,
        ICellSurface surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.Mirror = Mirror;

        if (_counter != -1)
        {
            _counter--;

            if (_counter == 0)
                commandStack.RemoveSafe(this);
        }
    }
}
