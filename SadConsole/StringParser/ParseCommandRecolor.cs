using System;
using System.Globalization;
using SadRogue.Primitives;

namespace SadConsole.StringParser;

/// <summary>
/// Recolors a glyph.
/// </summary>
public sealed class ParseCommandRecolor : ParseCommandBase
{
    /// <summary>
    /// Use the default foreground and background based on the <see cref="ParseCommandBase.CommandType"/>.
    /// </summary>
    public bool Default;

    /// <summary>
    /// Keeps the red channel of the existing color of the glyphs this command is applied to.
    /// </summary>
    public bool KeepRed;

    /// <summary>
    /// Keeps the green channel of the existing color of the glyphs this command is applied to.
    /// </summary>
    public bool KeepGreen;

    /// <summary>
    /// Keeps the blue channel of the existing color of the glyphs this command is applied to.
    /// </summary>
    public bool KeepBlue;

    /// <summary>
    /// Keeps the alpha channel of the existing color of the glyphs this command is applied to.
    /// </summary>
    public bool KeepAlpha;

    /// <summary>
    /// The current red value
    /// </summary>
    public byte R { get; set; }

    /// <summary>
    /// The current green value
    /// </summary>
    public byte G { get; set; }

    /// <summary>
    /// The current blue value
    /// </summary>
    public byte B { get; set; }

    /// <summary>
    /// The current alpha value
    /// </summary>
    public byte A { get; set; }

    private int _counter;
    private bool wordLength;

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    /// <param name="parameters">The string to parse for parameters.</param>
    public ParseCommandRecolor(string parameters)
    {
        //[c:r f|b:color[:count]]
        var badCommandException = new ArgumentException("command is invalid for Recolor: " + parameters);

        string[] parametersArray = parameters.Split(':');

        if (parametersArray.Length == 3)
        {
            if (parametersArray[2] == "w")
            {
                wordLength = true;
                _counter = -1;
            }
            else
            {
                wordLength = false;
                _counter = int.Parse(parametersArray[2], CultureInfo.InvariantCulture);
            }
        }
        else
        {
            wordLength = false;
            _counter = -1;
        }

        if (parametersArray.Length >= 2)
        {
            CommandType = parametersArray[0] == "b" ? CommandTypes.Background : CommandTypes.Foreground;
            Color color = Color.White.FromParser(parametersArray[1], out KeepRed, out KeepGreen, out KeepBlue, out KeepAlpha, out Default);

            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }
        else
        {
            throw badCommandException;
        }
    }

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    public ParseCommandRecolor()
    {

    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex,
        ICellSurface? surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        byte r;
        byte g;
        byte b;
        byte a;

        if (Default)
        {
            if (CommandType == CommandTypes.Background)
            {
                (surface != null ? surface.DefaultBackground : Color.Transparent).Deconstruct(out r, out g, out b, out a);
            }
            else
            {
                (surface != null ? surface.DefaultForeground : Color.White).Deconstruct(out r, out g, out b, out a);
            }
        }
        else
        {
            if (CommandType == CommandTypes.Background)
            {
                glyphState.Background.Deconstruct(out r, out g, out b, out a);
            }
            else
            {
                glyphState.Foreground.Deconstruct(out r, out g, out b, out a);
            }

            if (!KeepRed)
            {
                r = R;
            }

            if (!KeepGreen)
            {
                g = G;
            }

            if (!KeepBlue)
            {
                b = B;
            }

            if (!KeepAlpha)
            {
                a = A;
            }
        }

        if (CommandType == CommandTypes.Background)
        {
            glyphState.Background = new Color(r, g, b, a);
        }
        else
        {
            glyphState.Foreground = new Color(r, g, b, a);
        }

        if (_counter != -1)
        {
            _counter--;

            if (_counter == 0)
            {
                commandStack.RemoveSafe(this);
            }
        }
        else if (wordLength)
        {
            if (char.IsWhiteSpace(processedString[stringIndex]) || processedString[stringIndex] == '\0')
            {
                commandStack.RemoveSafe(this);
            }
        }
    }
}
