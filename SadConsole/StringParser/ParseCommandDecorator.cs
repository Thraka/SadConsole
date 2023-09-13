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
    public CellDecorator Decorator { get; set; }

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
    /// When <see langword="true"/>, replaces the decorators on the glyph, otherwise it adds them.
    /// </summary>
    public bool Replace { get; set; }

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    /// <param name="parameters">The string to parse for parameters.</param>
    /// <param name="replace">When <see langword="true"/>, replaces the decorators on the glyph, otherwise it adds them.</param>
    public ParseCommandDecorator(string parameters, bool replace)
    {
        var badCommandException = new ArgumentException("command is invalid for decorator: " + parameters);

        // glyph:mirror:color
        // glyph:mirror:color:count
        string[] paramArray = parameters.Split(':');

        CommandType = CommandTypes.Decorator;
        Replace = replace;

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

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex,
        ICellSurface? surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        Decorator = new CellDecorator(Color, Glyph, Mirror);

        // Create decorator list if needed
        if (Replace)
            glyphState.Decorators = new();
        else
            glyphState.Decorators ??= new();

        // If decorator isn't already in the glyph state decorators, add it
        if (glyphState.Decorators.IndexOf(Decorator) == -1)
            glyphState.Decorators.Add(Decorator);

        // If counter is counting down
        if (_counter != -1)
        {
            _counter--;

            // Remove decorator
            if (_counter == 0)
            {
                commandStack.RemoveSafe(this);

                // Remove this decorator from the array
                CellDecoratorHelpers.RemoveDecorator(Decorator, glyphState);
            }
        }
    }
}
