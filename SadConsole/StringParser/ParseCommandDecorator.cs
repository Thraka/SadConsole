using System;
using System.Globalization;
using SadRogue.Primitives;

namespace SadConsole.StringParser;

/// <summary>
/// Sets the mirror of a glyph.
/// </summary>
public sealed class ParseCommandDecorator : ParseCommandBase
{
    private int _counter;

    /// <summary>
    /// The decorator created by the command settings.
    /// </summary>
    public CellDecorator? Decorator { get; set; } = null;

    /// <summary>
    /// The glyph of the decorator.
    /// </summary>
    public int Glyph { get; set; }

    /// <summary>
    /// The color of the decorator.
    /// </summary>
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// The mirror to apply to the decorator.
    /// </summary>
    public Mirror Mirror { get; set; } = Mirror.None;

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    /// <param name="parameters">The string to parse for parameters.</param>
    public ParseCommandDecorator(string parameters)
    {
        var badCommandException = new ArgumentException("command is invalid for decorator: " + parameters);

        // glyph:mirror:color
        // glyph:mirror:color:count
        string[] paramArray = parameters.Split(':');

        CommandType = CommandTypes.Decorator;

        if (paramArray.Length == 3)
        {
            // Is glyph:mirror:color
            if (int.TryParse(paramArray[0], out int glyph))
            {
                _counter = -1;

                if (!Enum.TryParse(paramArray[1], out Mirror mirror))
                    throw badCommandException;

                Mirror = mirror;
                Glyph = glyph;

                Color = Color.FromParser(paramArray[2], out _, out _, out _, out _, out _);
            }
            else
                throw badCommandException;

        }
        // Is glyph:mirror:color:count
        else if (paramArray.Length == 4)
        {
            if (!int.TryParse(paramArray[0], out int glyph))
                throw badCommandException;

            if (!Enum.TryParse(paramArray[1], out Mirror mirror))
                throw badCommandException;

            Mirror = mirror;
            Glyph = glyph;

            Color = Color.FromParser(paramArray[1], out _, out _, out _, out _, out _);

            _counter = int.Parse(paramArray[2], CultureInfo.InvariantCulture);
        }
        else
            throw badCommandException;
    }

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    public ParseCommandDecorator(int counter = -1)
    {
        _counter = counter;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex,
        ICellSurface? surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        // Create decorator if needed
        if (!Decorator.HasValue) Decorator = new CellDecorator(Color, Glyph, Mirror);

        // If decorator isn't already in the glyph state decorators, add it
        if (Array.IndexOf(glyphState.Decorators, Decorator.Value) == -1)
        {
            CellDecorator[] decs = new CellDecorator[glyphState.Decorators.Length + 1];
            glyphState.Decorators.CopyTo(decs, 0);
            decs[decs.GetUpperBound(0)] = Decorator.Value;
            glyphState.Decorators = decs;
        }

        // If counter is counting down
        if (_counter != -1)
        {
            _counter--;

            // Remove decorator
            if (_counter == 0)
            {
                commandStack.RemoveSafe(this);

                // Remove this decorator from the array
                CellDecorator[] decs = new CellDecorator[glyphState.Decorators.Length - 1];
                int insertIndex = 0;
                for (int i = 0; i < glyphState.Decorators.Length; i++)
                {
                    if (glyphState.Decorators[i] != Decorator)
                    {
                        decs[insertIndex] = glyphState.Decorators[i];
                        insertIndex++;
                    }
                }
                glyphState.Decorators = decs;

            }
        }
    }
}
